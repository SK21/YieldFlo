using System;
using System.Collections.Generic;

namespace YieldFlo.Classes
{
    /// <summary>
    /// Remembers which ground a job has already cut, so a pass that overlaps an
    /// earlier one is not charged as new acres a second time.
    ///
    /// Without this, area is distance x full header width every tick, whatever was
    /// underneath. Overlap then corrupts the data twice: the job's acres inflate,
    /// so the average yield reads low by the overlapped fraction; and every
    /// overlapped tick divides a part-header's flow by a full header's width, which
    /// paints a cold streak on ground that yielded normally. Headland laps and
    /// point-row cleanup are where a combine spends much of its day, so this is
    /// structure invented across a large part of the map, not a rounding error.
    ///
    /// Ground is tracked as a bitmap of 0.25 m cells in a local metric frame whose
    /// origin is the job's first fix. Tiles of 256x256 cells (64 m square, 8 KB)
    /// are allocated as the machine reaches them: a 160 acre field is about 160
    /// tiles, so 1.3 MB.
    ///
    /// The grid supplies a RATIO, never an absolute area; the caller scales exact
    /// swath geometry by it, so with no overlap the answer is bit-identical to what
    /// the app computed before this existed.
    ///
    /// Querying and marking are deliberately different shapes. Marking takes the
    /// cells whose CENTRES fall in the swath rectangle, which tiles exactly —
    /// counting every cell the rectangle merely clips would dilate each pass by half
    /// a cell on each side, and the neighbouring pass would then find 5% of virgin
    /// ground already cut. Querying samples a line across the header at the current
    /// position and asks how many of those points sit on cut ground, which is the
    /// question that matters — how much of the header is in standing crop — and is
    /// independent of how far the machine moved in one tick. That independence is
    /// what makes the answer the same at 10 Hz as at 1 Hz, when a tick's travel is
    /// shorter than a single cell.
    ///
    /// Marking lags querying by MarkLagM of travel, and that lag is load-bearing.
    /// A GPS tick advances 0.2-0.5 m, comparable to a cell, so a swath and its
    /// immediate predecessor land in many of the same cells. Marking as it went,
    /// the grid answered "already cut" to the machine about its own header, and a
    /// virgin pass measured 0.64 new instead of 1.0 — a 36% phantom overlap on
    /// ground nobody had touched, which is worse than the error being fixed. Holding
    /// the last few metres out of the grid removes the self-collision without
    /// weakening real overlap detection: a neighbouring pass is minutes old, not
    /// metres.
    /// </summary>
    public class clsCoverageGrid
    {
        // Cell size sets the resolution of every edge in the system — the ragged
        // margin of a marked swath, and the inset the query needs to clear it — and
        // both of those errors land on the small overlaps that matter most: an
        // operator running 10% into the last pass is only 0.9 m in. At 0.5 m cells
        // those two errors together mis-stated such a pass by 5%. Quartering the
        // cell area costs 4x the memory, which is 1.3 MB on a 160 acre field, and
        // brings it near 1%.
        //
        // Going far below GPS repeatability buys nothing, but not for the reason it
        // is tempting to give. Quantisation contributes about 0.32 x cell to the
        // error in where the cut/standing boundary sits — 0.08 m here, 0.16 m at
        // 0.5 m — and that adds in quadrature with the receiver's PASS-TO-PASS
        // repeatability. Absolute accuracy is irrelevant: the grid works in a local
        // frame, so any offset the two passes share cancels. At RTK's 2 cm the cell
        // is essentially the whole error and 0.25 m earns its memory; past about
        // 0.3 m repeatability the two cell sizes land within 9% of each other and
        // the extra 4x memory buys an improvement the input noise will not let you
        // see.
        //
        // Position noise also biases acres DOWN, at any cell size: measured overlap
        // is max(true + error, 0), so noise on nominally abutting passes manufactures
        // roughly 0.4 sigma of overlap that was never there. That is a property of
        // the GPS, not of the grid — a smaller cell reduces it slightly rather than
        // causing it, so it is an argument for better positioning, not bigger cells.
        public const double CellSizeM = 0.25;

        private const int TileCells = 256;                        // 64 m square
        private const int TileBytes = TileCells * TileCells / 8;  // 8 KB
        private const double DegToM = 111320.0;

        // A GPS glitch that survives the caller's plausibility check could otherwise
        // allocate tiles across a continent. 4096 tiles is ~32 MB and about 4,100
        // acres of contiguous ground — far past any real field.
        private const int MaxTiles = 4096;

        // How far behind the header ground stays out of the grid. Must clear one
        // cell of dilation plus a tick's travel; 3 m does that with room to spare and
        // is still far short of any real overlap, which comes from an adjacent pass.
        // The cost is that ground genuinely re-cut inside 3 m of travel — a spot turn
        // on the spot — reads as new.
        private const double MarkLagM = 3.0;

        private readonly Dictionary<long, byte[]> _tiles = new Dictionary<long, byte[]>();

        // Distinct cells touched by the swath being measured right now. Reused rather
        // than allocated per call: this runs at GPS rate for a whole harvest day.
        private readonly HashSet<long> _swathCells = new HashSet<long>();

        // Swaths measured but not yet committed to the grid, oldest first.
        private readonly Queue<long[]> _pendingCells = new Queue<long[]>();
        private readonly Queue<double> _pendingLengths = new Queue<double>();
        private double _pendingDistanceM;

        private bool _originSet;
        private double _lat0, _lon0, _mPerDegLon;
        private bool _capacityWarned;

        /// <summary>Tiles currently allocated — one per 64 m square of ground touched.</summary>
        public int TileCount { get { return _tiles.Count; } }

        public void Reset()
        {
            _tiles.Clear();
            _swathCells.Clear();
            _pendingCells.Clear();
            _pendingLengths.Clear();
            _pendingDistanceM = 0;
            _originSet = false;
            _capacityWarned = false;
        }

        /// <summary>
        /// Commits the lagged swaths immediately. Call at a pass boundary: the last
        /// few metres of a finished pass are cut ground like any other, and the next
        /// pass may cross them within seconds.
        /// </summary>
        public void Flush()
        {
            while (_pendingCells.Count > 0) CommitOldest();
            _pendingDistanceM = 0;
        }

        /// <summary>
        /// Marks the rectangle swept between two consecutive header positions and
        /// returns the fraction of it that is ground this job had not already cut,
        /// 0..1.
        ///
        /// Returns 1 when there is nothing to judge — first position of a pass, a
        /// stationary tick, no header width — so the caller falls back to plain
        /// swath area rather than silently dropping acres.
        /// </summary>
        public double MarkSwath(double lat1, double lon1, double lat2, double lon2, double widthM)
        {
            if (widthM <= 0) return 1.0;

            if (!_originSet)
            {
                _lat0 = lat1;
                _lon0 = lon1;
                _mPerDegLon = DegToM * Math.Cos(lat1 * Math.PI / 180.0);
                _originSet = true;
            }

            double x1 = (lon1 - _lon0) * _mPerDegLon;
            double y1 = (lat1 - _lat0) * DegToM;
            double x2 = (lon2 - _lon0) * _mPerDegLon;
            double y2 = (lat2 - _lat0) * DegToM;

            double dx = x2 - x1, dy = y2 - y1;
            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len < 1e-6) return 1.0;

            // The rectangle is oriented by the two positions, not by GPS heading:
            // heading is at its noisiest exactly where overlap matters most, on a
            // headland turn, and a mis-oriented 30 ft rectangle marks ground the
            // machine never touched.
            double ux = dx / len, uy = dy / len;
            double nx = -uy, ny = ux;

            double fraction = QueryHeaderLine(0.5 * (x1 + x2), 0.5 * (y1 + y2), nx, ny, widthM);
            CollectSwathCells(x1, y1, dx, dy, ux, uy, nx, ny, len, widthM);

            // Queried against the grid, committed to it only once the header is
            // MarkLagM further on.
            var cells = new long[_swathCells.Count];
            _swathCells.CopyTo(cells);
            _pendingCells.Enqueue(cells);
            _pendingLengths.Enqueue(len);
            _pendingDistanceM += len;

            while (_pendingDistanceM > MarkLagM && _pendingCells.Count > 0)
                CommitOldest();

            return fraction;
        }

        /// <summary>
        /// Fraction of a line across the header, centred on (cx,cy), that is standing
        /// crop. Sampled every half cell so the estimate is a spatial average of the
        /// width rather than a count of quantised cells.
        ///
        /// The line is inset half a cell at each end. Marking takes cells by their
        /// centres, so the marked strip's edge is ragged by up to half a cell, and a
        /// query sampling the full width catches that ragged edge on both sides: a
        /// field run re-driving its own track read 6/37 new instead of 0, which
        /// credited 16% of the swath as fresh acres over ground already cut.
        ///
        /// Half a cell is exactly the raggedness and not a millimetre more. Every
        /// centimetre of inset is a blind band subtracted straight off the small
        /// overlaps this exists to catch: on a 10% overlap, insetting a full cell
        /// instead of half doubled the residual error from 1% to 2.2%.
        ///
        /// The inset is symmetric, so a true half-overlap still measures 0.5.
        /// </summary>
        private double QueryHeaderLine(double cx, double cy, double nx, double ny, double widthM)
        {
            double span = widthM - CellSizeM;

            // A header only a cell or two wide has nothing left after insetting;
            // keep half of it rather than collapsing to a point.
            if (span < widthM * 0.5) span = widthM * 0.5;

            int samples = Math.Max(4, (int)Math.Ceiling(span / (CellSizeM * 0.5)));
            int fresh = 0;

            for (int i = 0; i < samples; i++)
            {
                // Sample at the centre of each of `samples` equal slices, so the
                // header's two edges are weighted like everything between them.
                double off = -span * 0.5 + span * (i + 0.5) / samples;
                if (!IsSet(CellKey(cx + nx * off, cy + ny * off))) fresh++;
            }

            return (double)fresh / samples;
        }

        /// <summary>
        /// Gathers the cells whose centres lie inside the swept rectangle, into
        /// _swathCells. Half-open on both axes so that consecutive swaths — and
        /// parallel passes exactly a header apart — tile without sharing a cell.
        /// </summary>
        private void CollectSwathCells(double x1, double y1, double dx, double dy,
                                       double ux, double uy, double nx, double ny,
                                       double len, double widthM)
        {
            _swathCells.Clear();

            double half = widthM * 0.5;
            double cx1 = x1 + nx * half, cy1 = y1 + ny * half;
            double cx2 = x1 - nx * half, cy2 = y1 - ny * half;

            double minX = Math.Min(Math.Min(cx1, cx2), Math.Min(cx1 + dx, cx2 + dx));
            double maxX = Math.Max(Math.Max(cx1, cx2), Math.Max(cx1 + dx, cx2 + dx));
            double minY = Math.Min(Math.Min(cy1, cy2), Math.Min(cy1 + dy, cy2 + dy));
            double maxY = Math.Max(Math.Max(cy1, cy2), Math.Max(cy1 + dy, cy2 + dy));

            int ix0 = (int)Math.Floor(minX / CellSizeM), ix1 = (int)Math.Floor(maxX / CellSizeM);
            int iy0 = (int)Math.Floor(minY / CellSizeM), iy1 = (int)Math.Floor(maxY / CellSizeM);

            for (int iy = iy0; iy <= iy1; iy++)
            {
                double py = (iy + 0.5) * CellSizeM;
                for (int ix = ix0; ix <= ix1; ix++)
                {
                    double px = (ix + 0.5) * CellSizeM;
                    double rx = px - x1, ry = py - y1;

                    double along = rx * ux + ry * uy;
                    if (along < 0 || along >= len) continue;

                    double across = rx * nx + ry * ny;
                    if (across < -half || across >= half) continue;

                    _swathCells.Add(((long)ix << 32) | (uint)iy);
                }
            }
        }

        private void CommitOldest()
        {
            long[] cells = _pendingCells.Dequeue();
            _pendingDistanceM -= _pendingLengths.Dequeue();
            if (_pendingDistanceM < 0) _pendingDistanceM = 0;

            for (int i = 0; i < cells.Length; i++) SetCell(cells[i]);
        }

        private static long CellKey(double x, double y)
        {
            int ix = (int)Math.Floor(x / CellSizeM);
            int iy = (int)Math.Floor(y / CellSizeM);
            return ((long)ix << 32) | (uint)iy;
        }

        /// <summary>True if this cell has already been cut.</summary>
        private bool IsSet(long key)
        {
            byte[] tile;
            if (!_tiles.TryGetValue(TileKey(key), out tile)) return false;

            int bit = BitIndex(key);
            return (tile[bit >> 3] & (byte)(1 << (bit & 7))) != 0;
        }

        private void SetCell(long key)
        {
            long tileKey = TileKey(key);

            byte[] tile;
            if (!_tiles.TryGetValue(tileKey, out tile))
            {
                if (_tiles.Count >= MaxTiles)
                {
                    if (!_capacityWarned)
                    {
                        _capacityWarned = true;
                        Props.WriteErrorLog("CoverageGrid: tile cap reached — overlap no longer tracked");
                    }
                    // Leaving the cell unmarked degrades to the pre-grid behaviour:
                    // acres are counted twice, which is the old bug, rather than
                    // ground being called already-cut and acres vanishing.
                    return;
                }
                tile = new byte[TileBytes];
                _tiles[tileKey] = tile;
            }

            int bit = BitIndex(key);
            tile[bit >> 3] |= (byte)(1 << (bit & 7));
        }

        // Arithmetic shift and mask, not division and remainder: both must floor
        // toward negative infinity or cells west/south of the origin fold onto the
        // wrong tile.
        private static long TileKey(long key)
        {
            int ix = (int)(key >> 32);
            int iy = (int)(key & 0xFFFFFFFF);
            return ((long)(ix >> 8) << 32) | (uint)(iy >> 8);
        }

        private static int BitIndex(long key)
        {
            int ix = (int)(key >> 32);
            int iy = (int)(key & 0xFFFFFFFF);
            return ((iy & 255) << 8) | (ix & 255);
        }
    }
}
