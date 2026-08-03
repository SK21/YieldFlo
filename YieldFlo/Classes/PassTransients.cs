using System;
using System.Collections.Generic;
using YieldFlo.Database;

namespace YieldFlo.Classes
{
    /// <summary>
    /// Replaces the cells at each end of a pass with the yield measured just
    /// beside them. Display and export only — the stored rows are never changed.
    ///
    /// Why they need replacing: the ends of a pass are not measurements of the
    /// ground under them. Entering the crop the machine is still filling, so the
    /// first cells are painted with flow that has not reached the rate that ground
    /// actually yielded; leaving it, the elevator keeps delivering for far longer
    /// than the transport delay (measured ~100 s on a real machine against a 10 s
    /// delay), so the last cells are painted with flow that has already decayed.
    /// Both read low, which is the blue seen along every headland.
    ///
    /// This does not reconstruct the missing grain — that would be the back-spread,
    /// which was scoped and rejected: it adds no bushels (the tail is already in the
    /// job total via clsDataCollector's tail drain), it needs a write-behind buffer
    /// with flush paths where a bug loses the end of a pass, and it can only correct
    /// the trailing edge, leaving the headland striped. Trimming needs no buffering,
    /// no new stored data, fixes both ends, and — because it only declines to trust
    /// cells rather than needing grain that was never recorded — it improves jobs
    /// that were already collected.
    ///
    /// The honest description of what this does: those cells carry no information
    /// about their own ground, so it paints them with the nearest ground that does.
    /// </summary>
    public static class PassTransients
    {
        /// <summary>
        /// Seconds trimmed at each end of a pass, and the length of the reference
        /// window taken just inside it.
        ///
        /// A FIRST ESTIMATE, not a measurement: the blue blocks in the 2026-08-03
        /// field video run about 3-6 one-second cells. Deliberately conservative —
        /// trimming too far paints over real crop variation, and unlike the blue it
        /// replaces, that loss is invisible. Tune against the rising and falling
        /// edges in the diagnostic log once a build with the Sections column has run.
        /// </summary>
        public const double TrimSec = 6.0;

        // Must match frmYieldMap.MaxBridgeSeconds — a pass is exactly what the map
        // draws as one unbroken ribbon, so both have to break in the same places or
        // the trim would land somewhere the operator sees no boundary.
        private const double MaxGapSec = 3.0;

        // Below this the reference window is too thin to average safely and the end
        // is left alone. Better a blue end than one painted from two stray points.
        private const int MinReferencePoints = 3;

        /// <summary>
        /// Rewrites the transient cells in place. Returns the number of COMPLETED
        /// passes found, which the map uses to notice a pass has just ended: that
        /// changes cells it has already drawn, so it has to rebuild rather than
        /// append.
        /// </summary>
        public static int Apply(List<YieldDataPoint> points)
        {
            if (points == null || points.Count == 0) return 0;

            int completed = 0;
            int i = 0;

            while (i < points.Count)
            {
                // Skip anything that is not flowing — zero-yield marker rows are how
                // a pass end is recorded, and they separate one pass from the next.
                if (points[i].YieldRate <= 0) { i++; continue; }

                int start = i;
                int end   = i;
                while (end + 1 < points.Count
                       && points[end + 1].YieldRate > 0
                       && (points[end + 1].Timestamp - points[end].Timestamp).TotalSeconds > 0
                       && (points[end + 1].Timestamp - points[end].Timestamp).TotalSeconds <= MaxGapSec)
                    end++;

                // A pass running to the end of the data is still being harvested. Its
                // last cells are simply the newest ones, not a pass end, and trimming
                // them would blank the live edge of the map and then unblank it as the
                // next row arrived.
                bool terminated = end < points.Count - 1;
                if (terminated) completed++;

                TrimSegment(points, start, end, terminated);
                i = end + 1;
            }

            return completed;
        }

        private static void TrimSegment(List<YieldDataPoint> points, int start, int end, bool terminated)
        {
            DateTime tStart = points[start].Timestamp;
            DateTime tEnd   = points[end].Timestamp;

            // Needs a transient at each end plus a reference window between them, or
            // there is no undisturbed ground left to take a value from.
            if ((tEnd - tStart).TotalSeconds < 3.0 * TrimSec) return;

            // Leading edge — reference is the window immediately after it, so the
            // replacement follows the crop locally instead of flattening the pass to
            // one number.
            double avg = Mean(points, start, end,
                              p => (p.Timestamp - tStart).TotalSeconds,
                              TrimSec, 2.0 * TrimSec);
            if (avg > 0)
                Fill(points, start, end,
                     p => (p.Timestamp - tStart).TotalSeconds, TrimSec, avg);

            if (!terminated) return;

            avg = Mean(points, start, end,
                       p => (tEnd - p.Timestamp).TotalSeconds,
                       TrimSec, 2.0 * TrimSec);
            if (avg > 0)
                Fill(points, start, end,
                     p => (tEnd - p.Timestamp).TotalSeconds, TrimSec, avg);
        }

        // Mean yield of the points whose distance-from-the-end falls in [lo, hi).
        // Returns 0 when too few to trust, which the caller reads as "leave it alone".
        private static double Mean(List<YieldDataPoint> points, int start, int end,
                                   Func<YieldDataPoint, double> offset, double lo, double hi)
        {
            double sum = 0;
            int n = 0;
            for (int i = start; i <= end; i++)
            {
                double d = offset(points[i]);
                if (d >= lo && d < hi) { sum += points[i].YieldRate; n++; }
            }
            return n >= MinReferencePoints ? sum / n : 0;
        }

        private static void Fill(List<YieldDataPoint> points, int start, int end,
                                 Func<YieldDataPoint, double> offset, double within, double value)
        {
            for (int i = start; i <= end; i++)
                if (offset(points[i]) < within) points[i].YieldRate = value;
        }
    }
}
