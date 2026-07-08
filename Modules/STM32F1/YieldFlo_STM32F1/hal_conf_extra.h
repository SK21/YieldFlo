// The official STM32 core builds with the HAL CAN module disabled by default.
// This file is picked up automatically by the core and enables it. The bundled
// Can.h driver talks to the bxCAN registers directly, but keeping the module
// enabled is harmless and matches the original build config. Do not delete.
#pragma once
#define HAL_CAN_MODULE_ENABLED
