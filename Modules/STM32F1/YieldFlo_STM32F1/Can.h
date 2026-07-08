
// Bare-metal bxCAN register driver for STM32F103 — no external library needed.
// Adapted from nopnop2002's Arduino-STM32-CAN (via Pauli's port, hardware-
// verified against a PCAN adapter at 250 kbps). Local modifications:
//   - TX is SINGLE-SHOT (NART set): a frame that gets no ACK is dropped after
//     one attempt instead of being retried forever — a lone module on a dead
//     or unterminated bench never error-floods its way to Bus Off.
//   - ABOM set here in CANInit (was a separate re-enter-init step in Comm.ino).
//   - CANInit returns false on any failure instead of hanging in while(true).
//   - CANSend never blocks: if the previous frame is still pending it is
//     aborted so the freshest data wins (was a 1,000,000-iteration busy-wait).

/* Symbolic names for bit rate of CAN message                                */
typedef enum {CAN_50KBPS, CAN_100KBPS, CAN_125KBPS, CAN_250KBPS, CAN_500KBPS, CAN_1000KBPS} BITRATE;

/* Real speed for bit rate of CAN message                                    */
const uint32_t SPEED[6] = {50*1000, 100*1000, 125*1000, 250*1000, 500*1000, 1000*1000};

/* Symbolic names for formats of CAN message                                 */
typedef enum {STANDARD_FORMAT = 0, EXTENDED_FORMAT} CAN_FORMAT;

/* Symbolic names for type of CAN message                                    */
typedef enum {DATA_FRAME = 0, REMOTE_FRAME}         CAN_FRAME;


typedef struct
{
  uint32_t id;        /* 29 bit identifier                               */
  uint8_t  data[8];   /* Data field                                      */
  uint8_t  len;       /* Length of data field in bytes                   */
  uint8_t  ch;        /* Object channel(Not use)                         */
  uint8_t  format;    /* 0 - STANDARD, 1- EXTENDED IDENTIFIER            */
  uint8_t  type;      /* 0 - DATA FRAME, 1 - REMOTE FRAME                */
} CAN_msg_t;

typedef struct
{
    uint16_t baud_rate_prescaler;                /// [1 to 1024]
    uint8_t time_segment_1;                      /// [1 to 16]
    uint8_t time_segment_2;                      /// [1 to 8]
    uint8_t resynchronization_jump_width;        /// [1 to 4] (recommended value is 1)
} CAN_bit_timing_config_t;

/*
 * Calculation of bit timing dependent on peripheral clock rate.
 * Returns true on success. Sample point targets 87.5% (CANopen recommendation).
 */
bool ComputeCANTimings(const uint32_t peripheral_clock_rate,
                       const uint32_t target_bitrate,
                       CAN_bit_timing_config_t* const out_timings)
{
    if (out_timings == NULL || target_bitrate < 1000) return false;

    memset(out_timings, 0, sizeof(*out_timings));

    /*
     * Hardware configuration
     */
    static const uint8_t MaxBS1 = 16;
    static const uint8_t MaxBS2 = 8;

    /*
     * Ref. "Automatic Baudrate Detection in CANopen Networks", U. Koppe, MicroControl GmbH & Co. KG
     *      CAN in Automation, 2003
     *
     * According to the source, optimal quanta per bit are:
     *   Bitrate        Optimal Maximum
     *   1000 kbps      8       10
     *   500  kbps      16      17
     *   250  kbps      16      17
     *   125  kbps      16      17
     */
    const uint8_t max_quanta_per_bit = (uint8_t)((target_bitrate >= 1000000) ? 10 : 17);

    static const uint16_t MaxSamplePointLocationPermill = 900;

    /*
     * Computing (prescaler * BS):
     *   BITRATE = 1 / (PRESCALER * (1 / PCLK) * (1 + BS1 + BS2))       -- See the Reference Manual
     *   BITRATE = PCLK / (PRESCALER * (1 + BS1 + BS2))                 -- Simplified
     * let:
     *   BS = 1 + BS1 + BS2                                             -- Number of time quanta per bit
     *   PRESCALER_BS = PRESCALER * BS
     * ==>
     *   PRESCALER_BS = PCLK / BITRATE
     */
    const uint32_t prescaler_bs = peripheral_clock_rate / target_bitrate;

    /*
     * Searching for such prescaler value so that the number of quanta per bit is highest.
     */
    uint8_t bs1_bs2_sum = (uint8_t)(max_quanta_per_bit - 1);

    while ((prescaler_bs % (1U + bs1_bs2_sum)) != 0)
    {
        if (bs1_bs2_sum <= 2)
        {
            return false;          // No solution
        }
        bs1_bs2_sum--;
    }

    const uint32_t prescaler = prescaler_bs / (1U + bs1_bs2_sum);
    if ((prescaler < 1U) || (prescaler > 1024U))
    {
        return false;              // No solution
    }

    /*
     * Now we have a constraint: (BS1 + BS2) == bs1_bs2_sum.
     * We need to find such values so that the sample point is as close as possible to the optimal value,
     * which is 87.5%, which is 7/8.
     *
     *   Solve[(1 + bs1)/(1 + bs1 + bs2) == 7/8, bs2]  (* Where 7/8 is 0.875, the recommended sample point location *)
     *   {{bs2 -> (1 + bs1)/7}}
     *
     * Hence:
     *   bs2 = (1 + bs1) / 7
     *   bs1 = (7 * bs1_bs2_sum - 1) / 8
     *
     * Sample point location can be computed as follows:
     *   Sample point location = (1 + bs1) / (1 + bs1 + bs2)
     *
     * Since the optimal solution is so close to the maximum, we prepare two solutions, and then pick the best one:
     *   - With rounding to nearest
     *   - With rounding to zero
     */
    uint8_t bs1 = (uint8_t)(((7 * bs1_bs2_sum - 1) + 4) / 8);       // Trying rounding to nearest first
    uint8_t bs2 = (uint8_t)(bs1_bs2_sum - bs1);

    {
        const uint16_t sample_point_permill = (uint16_t)(1000U * (1U + bs1) / (1U + bs1 + bs2));

        if (sample_point_permill > MaxSamplePointLocationPermill)   // Strictly more!
        {
            bs1 = (uint8_t)((7 * bs1_bs2_sum - 1) / 8);             // Nope, too far; now rounding to zero
            bs2 = (uint8_t)(bs1_bs2_sum - bs1);
        }
    }

    const bool valid = (bs1 >= 1) && (bs1 <= MaxBS1) && (bs2 >= 1) && (bs2 <= MaxBS2);

    if ((target_bitrate != (peripheral_clock_rate / (prescaler * (1U + bs1 + bs2)))) ||
        !valid)
    {
        return false;              // Algorithm failed to converge — unsupported bit rate
    }

    out_timings->baud_rate_prescaler = (uint16_t) prescaler;
    out_timings->resynchronization_jump_width = 1;      // One is recommended by UAVCAN, CANOpen, and DeviceNet
    out_timings->time_segment_1 = bs1;
    out_timings->time_segment_2 = bs2;

    return true;
}

/**
 * Initializes the CAN filter registers.
 *
 * @preconditions   - This register can be written only when the filter initialization mode is set (FINIT=1) in the CAN_FMR register.
 * @params: index   - Specified filter index. index 27:14 are available in connectivity line devices only.
 * @params: scale   - Select filter scale.
 *                    0: Dual 16-bit scale configuration
 *                    1: Single 32-bit scale configuration
 * @params: mode    - Select filter mode.
 *                    0: Two 32-bit registers of filter bank x are in Identifier Mask mode
 *                    1: Two 32-bit registers of filter bank x are in Identifier List mode
 * @params: fifo    - Select filter assigned.
 *                    0: Filter assigned to FIFO 0
 *                    1: Filter assigned to FIFO 1
 * @params: bank1   - Filter bank register 1
 * @params: bank2   - Filter bank register 2
 *
 */
void CANSetFilter(uint8_t index, uint8_t scale, uint8_t mode, uint8_t fifo, uint32_t bank1, uint32_t bank2) {
  if (index > 27) return;

  CAN1->FA1R &= ~(0x1UL<<index);               // Deactivate filter

  if (scale == 0) {
    CAN1->FS1R &= ~(0x1UL<<index);             // Set filter to Dual 16-bit scale configuration
  } else {
    CAN1->FS1R |= (0x1UL<<index);              // Set filter to single 32 bit configuration
  }
    if (mode == 0) {
    CAN1->FM1R &= ~(0x1UL<<index);             // Set filter to Mask mode
  } else {
    CAN1->FM1R |= (0x1UL<<index);              // Set filter to List mode
  }

  if (fifo == 0) {
    CAN1->FFA1R &= ~(0x1UL<<index);            // Set filter assigned to FIFO 0
  } else {
    CAN1->FFA1R |= (0x1UL<<index);             // Set filter assigned to FIFO 1
  }

  CAN1->sFilterRegister[index].FR1 = bank1;    // Set filter bank registers1
  CAN1->sFilterRegister[index].FR2 = bank2;    // Set filter bank registers2

  CAN1->FA1R |= (0x1UL<<index);                // Activate filter

}

/**
 * Initializes the CAN controller with specified bit rate.
 * Returns false if the bit timing cannot be computed for the peripheral
 * clock, or if the controller never reaches normal mode (typically a
 * wiring/transceiver fault — bxCAN needs to see 11 recessive bits).
 *
 * @params: bitrate - Specified bitrate. Must be one of the BITRATE constants.
 * @params: remap   - Select CAN port.
 *                    =0:CAN_RX mapped to PA11, CAN_TX mapped to PA12
 *                    =1:Not used
 *                    =2:CAN_RX mapped to PB8, CAN_TX mapped to PB9 (not available on 36-pin package)
 *                    =3:CAN_RX mapped to PD0, CAN_TX mapped to PD1 (available on 100-pin and 144-pin package)
 *
 */
bool CANInit(BITRATE bitrate, int remap)
{
  // Reference manual: RM0008 (STM32F101/102/103/105/107), bxCAN chapter.

  RCC->APB1ENR |= 0x2000000UL;       // Enable CAN clock
  RCC->APB2ENR |= 0x1UL;             // Enable AFIO clock
  AFIO->MAPR   &= 0xFFFF9FFF;        // reset CAN remap
                                     // CAN_RX mapped to PA11, CAN_TX mapped to PA12

  if (remap == 0) {
    RCC->APB2ENR |= 0x4UL;           // Enable GPIOA clock
    GPIOA->CRH   &= ~(0xFF000UL);    // Configure PA12(0b0000) and PA11(0b0000)
                                     // 0b0000
                                     //   MODE=00(Input mode)
                                     //   CNF=00(Analog mode)

    GPIOA->CRH   |= 0xB8FFFUL;       // Configure PA12(0b1011) and PA11(0b1000)
                                     // 0b1011
                                     //   MODE=11(Output mode, max speed 50 MHz)
                                     //   CNF=10(Alternate function output Push-pull
                                     // 0b1000
                                     //   MODE=00(Input mode)
                                     //   CNF=10(Input with pull-up / pull-down)

    GPIOA->ODR |= 0x1UL << 11;       // PA11 pull-up
  }

  if (remap == 2) {
    AFIO->MAPR   |= 0x00004000;      // set CAN remap
                                     // CAN_RX mapped to PB8, CAN_TX mapped to PB9 (not available on 36-pin package)

    RCC->APB2ENR |= 0x8UL;           // Enable GPIOB clock
    GPIOB->CRH   &= ~(0xFFUL);       // Configure PB9(0b0000) and PB8(0b0000)
                                     // 0b0000
                                     //   MODE=00(Input mode)
                                     //   CNF=00(Analog mode)

    GPIOB->CRH   |= 0xB8UL;          // Configure PB9(0b1011) and PB8(0b1000)
                                     // 0b1011
                                     //   MODE=11(Output mode, max speed 50 MHz)
                                     //   CNF=10(Alternate function output Push-pull
                                     // 0b1000
                                     //   MODE=00(Input mode)
                                     //   CNF=10(Input with pull-up / pull-down)

    GPIOB->ODR |= 0x1UL << 8;        // PB8 pull-up
  }

  if (remap == 3) {
    AFIO->MAPR   |= 0x00005000;      // set CAN remap
                                     // CAN_RX mapped to PD0, CAN_TX mapped to PD1 (available on 100-pin and 144-pin package)

    RCC->APB2ENR |= 0x20UL;          // Enable GPIOD clock
    GPIOD->CRL   &= ~(0xFFUL);       // Configure PD1(0b0000) and PD0(0b0000)
                                     // 0b0000
                                     //   MODE=00(Input mode)
                                     //   CNF=00(Analog mode)

    GPIOD->CRH   |= 0xB8UL;          // Configure PD1(0b1011) and PD0(0b1000)
                                     // 0b1000
                                     //   MODE=00(Input mode)
                                     //   CNF=10(Input with pull-up / pull-down)
                                     // 0b1011
                                     //   MODE=11(Output mode, max speed 50 MHz)
                                     //   CNF=10(Alternate function output Push-pull

    GPIOD->ODR |= 0x1UL << 0;        // PD0 pull-up
  }

  CAN1->MCR |= 0x1UL;                   // Require CAN1 to Initialization mode
  while (!(CAN1->MSR & 0x1UL));         // Wait for Initialization mode

  // MCR = INRQ(0x01) | NART(0x10) | ABOM(0x40):
  //   NART — single-shot TX, no automatic retransmission (deliberate: see
  //          file header). Change to 0x41 for hardware auto-retransmission.
  //   ABOM — automatic Bus-Off recovery after 128 × 11 recessive bits.
  CAN1->MCR = 0x51UL;

  // Set bit timing register
  CAN_bit_timing_config_t timings;
  uint32_t target_bitrate = SPEED[bitrate];
  if (!ComputeCANTimings(HAL_RCC_GetPCLK1Freq(), target_bitrate, &timings))
  {
    return false;                       // unsupported bit rate for this clock
  }
  CAN1->BTR = (((timings.resynchronization_jump_width - 1U) &    3U) << 24U) |
              (((timings.time_segment_1 - 1U)               &   15U) << 16U) |
              (((timings.time_segment_2 - 1U)               &    7U) << 20U) |
              ((timings.baud_rate_prescaler - 1U)           & 1023U);

  // Configure Filters to default values
  CAN1->FMR |= 0x1UL;                   // Set to filter initialization mode
  CAN1->FMR &= 0xFFFFC0FF;              // Clear CAN2 start bank

  // bxCAN has 28 filters.
  // These filters are shared by both CAN1 and CAN2.
  // STM32F103 has only CAN1, so all 28 are used for CAN1
  CAN1->FMR |= 0x1C << 8;               // Assign all filters to CAN1

  // Set filter 0
  // Single 32-bit scale configuration
  // Two 32-bit registers of filter bank x are in Identifier Mask mode
  // Filter assigned to FIFO 0
  // Filter bank register to all 0 — accept every frame
  CANSetFilter(0, 1, 0, 0, 0x0UL, 0x0UL);

  CAN1->FMR &= ~(0x1UL);                // Deactivate initialization mode

  uint16_t TimeoutMilliseconds = 1000;
  CAN1->MCR &= ~(0x1UL);                // Require CAN1 to normal mode

  // Wait for normal mode
  // If the connection is not correct, it will not return to normal mode.
  for (uint16_t wait_ack = 0; wait_ack < TimeoutMilliseconds; wait_ack++) {
    if ((CAN1->MSR & 0x1UL) == 0) {
      return true;
    }
    delayMicroseconds(1000);
  }
  return false;
}


#define STM32_CAN_TIR_TXRQ              (1U << 0U)  // Bit 0: Transmit Mailbox Request
#define STM32_CAN_RIR_RTR               (1U << 1U)  // Bit 1: Remote Transmission Request
#define STM32_CAN_RIR_IDE               (1U << 2U)  // Bit 2: Identifier Extension
#define STM32_CAN_TIR_RTR               (1U << 1U)  // Bit 1: Remote Transmission Request
#define STM32_CAN_TIR_IDE               (1U << 2U)  // Bit 2: Identifier Extension

#define CAN_EXT_ID_MASK                 0x1FFFFFFFU
#define CAN_STD_ID_MASK                 0x000007FFU

/**
 * Decodes CAN messages from the data registers and populates a
 * CAN message struct with the data fields.
 *
 * @preconditions A valid CAN message is received
 * @params CAN_rx_msg - CAN message structure for reception
 *
 */
void CANReceive(CAN_msg_t* CAN_rx_msg)
{
  uint32_t id = CAN1->sFIFOMailBox[0].RIR;
  if ((id & STM32_CAN_RIR_IDE) == 0) { // Standard frame format
      CAN_rx_msg->format = STANDARD_FORMAT;
      CAN_rx_msg->id = (CAN_STD_ID_MASK & (id >> 21U));
  }
  else {                               // Extended frame format
      CAN_rx_msg->format = EXTENDED_FORMAT;
      CAN_rx_msg->id = (CAN_EXT_ID_MASK & (id >> 3U));
  }

  if ((id & STM32_CAN_RIR_RTR) == 0) { // Data frame
      CAN_rx_msg->type = DATA_FRAME;
  }
  else {                               // Remote frame
      CAN_rx_msg->type = REMOTE_FRAME;
  }

  CAN_rx_msg->len = (CAN1->sFIFOMailBox[0].RDTR) & 0xFUL;

  CAN_rx_msg->data[0] = 0xFFUL &  CAN1->sFIFOMailBox[0].RDLR;
  CAN_rx_msg->data[1] = 0xFFUL & (CAN1->sFIFOMailBox[0].RDLR >> 8);
  CAN_rx_msg->data[2] = 0xFFUL & (CAN1->sFIFOMailBox[0].RDLR >> 16);
  CAN_rx_msg->data[3] = 0xFFUL & (CAN1->sFIFOMailBox[0].RDLR >> 24);
  CAN_rx_msg->data[4] = 0xFFUL &  CAN1->sFIFOMailBox[0].RDHR;
  CAN_rx_msg->data[5] = 0xFFUL & (CAN1->sFIFOMailBox[0].RDHR >> 8);
  CAN_rx_msg->data[6] = 0xFFUL & (CAN1->sFIFOMailBox[0].RDHR >> 16);
  CAN_rx_msg->data[7] = 0xFFUL & (CAN1->sFIFOMailBox[0].RDHR >> 24);

  // Release FIFO 0 output mailbox.
  // Make the next incoming message available.
  CAN1->RF0R |= 0x20UL;
}

/**
 * Encodes CAN messages using the CAN message struct and populates the
 * data registers with the sent.
 *
 * Never blocks. With single-shot TX the mailbox is normally already empty
 * (a failed frame is abandoned after one attempt); if a frame is still
 * pending — bus stuck dominant, or heavy traffic losing arbitration — the
 * stale frame is aborted so this fresher one takes its place.
 *
 * @params CAN_tx_msg - CAN message structure for transmission
 *
 */
void CANSend(CAN_msg_t* CAN_tx_msg)
{
  if (!(CAN1->TSR & CAN_TSR_TME0)) {   // mailbox 0 still occupied
    CAN1->TSR |= CAN_TSR_ABRQ0;        // abort the pending frame
    uint32_t t = micros();             // abort completes within a frame time
    while (!(CAN1->TSR & CAN_TSR_TME0)) {
      if (micros() - t > 1000) return; // still busy — drop this frame
    }
  }

  uint32_t out = 0;
  if (CAN_tx_msg->format == EXTENDED_FORMAT) { // Extended frame format
      out = ((CAN_tx_msg->id & CAN_EXT_ID_MASK) << 3U) | STM32_CAN_TIR_IDE;
  }
  else {                                       // Standard frame format
      out = ((CAN_tx_msg->id & CAN_STD_ID_MASK) << 21U);
  }

  // Remote frame
  if (CAN_tx_msg->type == REMOTE_FRAME) {
      out |= STM32_CAN_TIR_RTR;
  }

  CAN1->sTxMailBox[0].TDTR &= ~(0xF);
  CAN1->sTxMailBox[0].TDTR |= CAN_tx_msg->len & 0xFUL;

  CAN1->sTxMailBox[0].TDLR  = (((uint32_t) CAN_tx_msg->data[3] << 24) |
                               ((uint32_t) CAN_tx_msg->data[2] << 16) |
                               ((uint32_t) CAN_tx_msg->data[1] <<  8) |
                               ((uint32_t) CAN_tx_msg->data[0]      ));
  CAN1->sTxMailBox[0].TDHR  = (((uint32_t) CAN_tx_msg->data[7] << 24) |
                               ((uint32_t) CAN_tx_msg->data[6] << 16) |
                               ((uint32_t) CAN_tx_msg->data[5] <<  8) |
                               ((uint32_t) CAN_tx_msg->data[4]      ));

  // Send Go
  CAN1->sTxMailBox[0].TIR = out | STM32_CAN_TIR_TXRQ;
}

 /**
 * Returns whether there are CAN messages available.
 *
 * @returns If pending CAN messages are in the CAN controller
 *
 */
uint8_t CANMsgAvail(void)
{
  // Check for pending FIFO 0 messages
  return CAN1->RF0R & 0x3UL;
}
