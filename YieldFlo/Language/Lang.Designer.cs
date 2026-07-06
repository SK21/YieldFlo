// Strongly-typed resource accessor — matches the RC pattern exactly.
// ResourceManager.GetString("key", null) uses Thread.CurrentThread.CurrentUICulture,
// which is set at startup from the saved language setting.  MSBuild compiles each
// Lang.{culture}.resx into a satellite assembly ({culture}\YieldFlo.resources.dll)
// that ResourceManager finds automatically; missing keys fall back to English.
namespace YieldFlo.Language
{
    internal static class Lang
    {
        private static global::System.Resources.ResourceManager resourceMan;

        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (global::System.Object.ReferenceEquals(resourceMan, null))
                    resourceMan = new global::System.Resources.ResourceManager(
                        "YieldFlo.Language.Lang", typeof(Lang).Assembly);
                return resourceMan;
            }
        }

        private static string Get(string key) =>
            ResourceManager.GetString(key, null);

        // ── Common ───────────────────────────────────────────────────────────
        internal static string lgClose          => Get("lgClose");
        internal static string lgNew            => Get("lgNew");
        internal static string lgSave           => Get("lgSave");
        internal static string lgDelete         => Get("lgDelete");
        internal static string lgLoad           => Get("lgLoad");
        internal static string lgSaveApply      => Get("lgSaveApply");
        internal static string lgYes            => Get("lgYes");
        internal static string lgNo             => Get("lgNo");
        internal static string lgOn             => Get("lgOn");
        internal static string lgOff            => Get("lgOff");
        internal static string lgRefresh        => Get("lgRefresh");
        internal static string lgNone           => Get("lgNone");
        internal static string lgName           => Get("lgName");
        internal static string lgCrop           => Get("lgCrop");
        internal static string lgHeader         => Get("lgHeader");
        internal static string lgProfile        => Get("lgProfile");
        internal static string lgField          => Get("lgField");
        internal static string lgOffset         => Get("lgOffset");
        internal static string lgScale          => Get("lgScale");
        internal static string lgAppReads       => Get("lgAppReads");
        internal static string lgMeter          => Get("lgMeter");
        internal static string lgApplyCal       => Get("lgApplyCal");
        internal static string lgLanguage       => Get("lgLanguage");
        internal static string lgRestart        => Get("lgRestart");
        internal static string lgLangNote       => Get("lgLangNote");

        // ── Form titles ──────────────────────────────────────────────────────
        internal static string lgTitleMenu        => Get("lgTitleMenu");
        internal static string lgTitleSettings    => Get("lgTitleSettings");
        internal static string lgTitleJobs        => Get("lgTitleJobs");
        internal static string lgTitleCrops       => Get("lgTitleCrops");
        internal static string lgTitleHeaders     => Get("lgTitleHeaders");
        internal static string lgTitleProfiles    => Get("lgTitleProfiles");
        internal static string lgTitleFields      => Get("lgTitleFields");
        internal static string lgTitleYieldCal    => Get("lgTitleYieldCal");
        internal static string lgTitleMoistureCal => Get("lgTitleMoistureCal");
        internal static string lgTitleJobReport   => Get("lgTitleJobReport");
        internal static string lgTitleYieldMap    => Get("lgTitleYieldMap");
        internal static string lgTitleLanguage    => Get("lgTitleLanguage");

        // ── Main form ────────────────────────────────────────────────────────
        internal static string lgYield         => Get("lgYield");
        internal static string lgMoisture      => Get("lgMoisture");
        internal static string lgSpeed         => Get("lgSpeed");
        internal static string lgSensors       => Get("lgSensors");
        internal static string lgElevFlow      => Get("lgElevFlow");
        internal static string lgMoistureLabel => Get("lgMoistureLabel");
        internal static string lgGPS           => Get("lgGPS");
        internal static string lgModule        => Get("lgModule");
        internal static string lgNoActiveJob   => Get("lgNoActiveJob");
        internal static string lgJobStatusOn   => Get("lgJobStatusOn");
        internal static string lgJobStatusOff  => Get("lgJobStatusOff");
        internal static string lgStopJobPrompt => Get("lgStopJobPrompt");
        internal static string lgStopJob       => Get("lgStopJob");
        internal static string lgMenu          => Get("lgMenu");
        internal static string lgStart         => Get("lgStart");
        internal static string lgPause         => Get("lgPause");
        internal static string lgStop          => Get("lgStop");
        internal static string lgExit          => Get("lgExit");

        // ── Menu form ────────────────────────────────────────────────────────
        internal static string lgReports => Get("lgReports");

        // ── Settings form ────────────────────────────────────────────────────
        internal static string lgUnits         => Get("lgUnits");
        internal static string lgImperial      => Get("lgImperial");
        internal static string lgMetric        => Get("lgMetric");
        internal static string lgTheme         => Get("lgTheme");
        internal static string lgDark          => Get("lgDark");
        internal static string lgLight         => Get("lgLight");
        internal static string lgModuleComm    => Get("lgModuleComm");
        internal static string lgWiFi          => Get("lgWiFi");
        internal static string lgWifiInfo      => Get("lgWifiInfo");
        internal static string lgCanDriver     => Get("lgCanDriver");
        internal static string lgComPort       => Get("lgComPort");
        internal static string lgResumeOnStart => Get("lgResumeOnStart");
        internal static string lgSettingsSaved => Get("lgSettingsSaved");

        // ── Yield Cal form ───────────────────────────────────────────────────
        internal static string lgProcessingDelay    => Get("lgProcessingDelay");
        internal static string lgSeconds            => Get("lgSeconds");
        internal static string lgSensorBaseline     => Get("lgSensorBaseline");
        internal static string lgBaselineNote       => Get("lgBaselineNote");
        internal static string lgSetBaseline        => Get("lgSetBaseline");
        internal static string lgBaselineNoModule   => Get("lgBaselineNoModule");
        internal static string lgYieldFactor        => Get("lgYieldFactor");
        internal static string lgFactorNote         => Get("lgFactorNote");
        internal static string lgCalibrationRun     => Get("lgCalibrationRun");
        internal static string lgStartRun           => Get("lgStartRun");
        internal static string lgStopRun            => Get("lgStopRun");
        internal static string lgMeasuredBlank      => Get("lgMeasuredBlank");
        internal static string lgActualWeight       => Get("lgActualWeight");
        internal static string lgCalibrationTip     => Get("lgCalibrationTip");
        internal static string lgCalSaved           => Get("lgCalSaved");
        internal static string lgEnterWeighedAmt    => Get("lgEnterWeighedAmt");
        internal static string lgNoMeasuredData     => Get("lgNoMeasuredData");
        internal static string lgNewFactorResult    => Get("lgNewFactorResult");
        internal static string lgFactorUpdated      => Get("lgFactorUpdated");
        internal static string lgSetBaselineConfirm => Get("lgSetBaselineConfirm");
        internal static string lgMeasuredMetric     => Get("lgMeasuredMetric");
        internal static string lgMeasuredImperial   => Get("lgMeasuredImperial");

        // ── Moisture Cal form ────────────────────────────────────────────────
        internal static string lgMoistureSection    => Get("lgMoistureSection");
        internal static string lgPercentSavedToCrop => Get("lgPercentSavedToCrop");
        internal static string lgPercentPerCount    => Get("lgPercentPerCount");
        internal static string lgTemperatureSection => Get("lgTemperatureSection");
        internal static string lgDegCSavedToProfile => Get("lgDegCSavedToProfile");
        internal static string lgDegCPerCount       => Get("lgDegCPerCount");
        internal static string lgNoLiveMoistReading => Get("lgNoLiveMoistReading");
        internal static string lgNoLiveTempReading  => Get("lgNoLiveTempReading");
        internal static string lgSaved              => Get("lgSaved");

        // ── Jobs form ────────────────────────────────────────────────────────
        internal static string lgJobName           => Get("lgJobName");
        internal static string lgNotes             => Get("lgNotes");
        internal static string lgColJobName        => Get("lgColJobName");
        internal static string lgColStatus         => Get("lgColStatus");
        internal static string lgColDate           => Get("lgColDate");
        internal static string lgColAcres          => Get("lgColAcres");
        internal static string lgColField          => Get("lgColField");
        internal static string lgSelectCropFirst   => Get("lgSelectCropFirst");
        internal static string lgSelectHeaderFirst => Get("lgSelectHeaderFirst");
        internal static string lgSelectJobFirst    => Get("lgSelectJobFirst");
        internal static string lgEnterJobName      => Get("lgEnterJobName");
        internal static string lgMustHaveOneJob    => Get("lgMustHaveOneJob");
        internal static string lgDeleteJobPrompt   => Get("lgDeleteJobPrompt");
        internal static string lgSwitchJobPrompt   => Get("lgSwitchJobPrompt");

        // ── Crops form ───────────────────────────────────────────────────────
        internal static string lgCategoryLabel    => Get("lgCategoryLabel");
        internal static string lgTestWeight       => Get("lgTestWeight");
        internal static string lgMktMoisture      => Get("lgMktMoisture");
        internal static string lgEnterCropName    => Get("lgEnterCropName");
        internal static string lgMustHaveOneCrop  => Get("lgMustHaveOneCrop");
        internal static string lgDeleteCropPrompt => Get("lgDeleteCropPrompt");

        // ── Headers form ─────────────────────────────────────────────────────
        internal static string lgType               => Get("lgType");
        internal static string lgWidth              => Get("lgWidth");
        internal static string lgEnterHeaderName    => Get("lgEnterHeaderName");
        internal static string lgMustHaveOneHeader  => Get("lgMustHaveOneHeader");
        internal static string lgDeleteHeaderPrompt => Get("lgDeleteHeaderPrompt");

        // ── Profiles form ────────────────────────────────────────────────────
        internal static string lgCombineId           => Get("lgCombineId");
        internal static string lgEnterProfileName    => Get("lgEnterProfileName");
        internal static string lgMustHaveOneProfile  => Get("lgMustHaveOneProfile");
        internal static string lgDeleteProfilePrompt => Get("lgDeleteProfilePrompt");

        // ── Fields form ──────────────────────────────────────────────────────
        internal static string lgEnterFieldName    => Get("lgEnterFieldName");
        internal static string lgDeleteFieldPrompt => Get("lgDeleteFieldPrompt");

        // ── Job Report form ──────────────────────────────────────────────────
        internal static string lgJobReport    => Get("lgJobReport");
        internal static string lgPrint        => Get("lgPrint");
        internal static string lgFieldColon   => Get("lgFieldColon");
        internal static string lgAreaColon    => Get("lgAreaColon");
        internal static string lgTotalColon   => Get("lgTotalColon");
        internal static string lgAvgYield     => Get("lgAvgYield");
        internal static string lgAvgMoisture  => Get("lgAvgMoisture");
        internal static string lgDataPoints   => Get("lgDataPoints");
        internal static string lgExportCsv    => Get("lgExportCsv");
        internal static string lgExportFailed => Get("lgExportFailed");
        internal static string lgExported     => Get("lgExported");

        // ── Yield Map ────────────────────────────────────────────────────────
        internal static string lgLow    => Get("lgLow");
        internal static string lgHigh   => Get("lgHigh");
        internal static string lgNoJobs => Get("lgNoJobs");
    }
}
