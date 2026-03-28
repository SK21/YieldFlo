using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using System;
using System.Threading;
using YieldFlo.Classes;

namespace YieldFlo.Communication.Map
{
    /// <summary>
    /// Esri ArcGIS World Imagery satellite tile provider for GMap.NET.
    /// Free for non-commercial use; tiles are cached locally by GMap.NET.
    /// </summary>
    public class ArcGIS_World_Imagery_Provider : GMapProvider
    {
        public static readonly ArcGIS_World_Imagery_Provider Instance =
            new ArcGIS_World_Imagery_Provider();

        private const string TileUrlTemplate =
            "https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{0}/{1}/{2}";

        private static readonly TimeSpan MinRequestInterval = TimeSpan.FromMilliseconds(120);
        private static readonly object   ThrottleLock       = new object();
        private static DateTime _lastRequest = DateTime.MinValue;

        public override Guid            Id         => new Guid("F4E1A7A7-3B5D-4FDC-9C84-9B2390C7B04C");
        public override string          Name       => "ArcGISWorldImagery";
        public override GMapProvider[]  Overlays   => new GMapProvider[] { this };
        public override PureProjection  Projection => MercatorProjection.Instance;

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            TimeSpan delay;
            lock (ThrottleLock)
            {
                var elapsed = DateTime.Now - _lastRequest;
                delay = elapsed < MinRequestInterval ? MinRequestInterval - elapsed : TimeSpan.Zero;
                _lastRequest = DateTime.Now + delay;
            }

            if (delay > TimeSpan.Zero)
                Thread.Sleep(delay);

            string url = string.Format(TileUrlTemplate, zoom, pos.Y, pos.X);
            try
            {
                return GetTileImageUsingHttp(url);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ArcGIS_World_Imagery_Provider/GetTileImage: " + ex.Message);
                return null;
            }
        }
    }
}
