//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AirInfoApi
{
    using System;
    using System.Collections.Generic;
    
    public partial class Rpt_FanTestDirectDrive
    {
        public System.Guid ReportID { get; set; }
        public Nullable<System.Guid> SystemReportID_fk { get; set; }
        public string LocationAHU { get; set; }
        public string ManufactureAHU { get; set; }
        public string ModelNoAHU { get; set; }
        public string SerialNoAHU { get; set; }
        public string DesignAirLsAHU { get; set; }
        public string DesignAirPaAHU { get; set; }
        public string FanControl { get; set; }
        public string SupplyExhaust { get; set; }
        public string MSSBNameAHU { get; set; }
        public string MSSBLocationAHU { get; set; }
        public string MotorMakeMD { get; set; }
        public string FrameMD { get; set; }
        public string ElectricalMD { get; set; }
        public string MotorPowerMD { get; set; }
        public string FullLoadMD { get; set; }
        public string PoleMD { get; set; }
        public string PoleRPMMD { get; set; }
        public string OverloadMakeMS { get; set; }
        public string OverloadRangeMS { get; set; }
        public string VSDMakeMS { get; set; }
        public string VSDRangeMS { get; set; }
        public string FilterType1 { get; set; }
        public string FilterSize1 { get; set; }
        public string NoFilters1 { get; set; }
        public string FilterType2 { get; set; }
        public string FilterSize2 { get; set; }
        public string NoFilters2 { get; set; }
        public string TotalAirDES { get; set; }
        public string TotalAirFIN { get; set; }
        public string TotalStaticDES { get; set; }
        public string TotalStaticFIN { get; set; }
        public string SucStaticDES { get; set; }
        public string SucStaticFIN { get; set; }
        public string DisStaticFIN { get; set; }
        public string FilterDiffFIN { get; set; }
        public string VoltageDES { get; set; }
        public string VoltageFIN { get; set; }
        public string AmpsDES { get; set; }
        public string AmpsFIN { get; set; }
        public string VSDFinalHz { get; set; }
        public string StaticPressureRef { get; set; }
        public string Instrument1 { get; set; }
        public string Make1 { get; set; }
        public string Serial1 { get; set; }
        public string Instrument2 { get; set; }
        public string Make2 { get; set; }
        public string Serial2 { get; set; }
        public string FilterSize1B { get; set; }
        public string FilterSize1C { get; set; }
        public string FilterSize2B { get; set; }
        public string FilterSize2C { get; set; }
        public string FuseDetailsMDPH { get; set; }
        public string OverloadRangeMS2 { get; set; }
        public string OverloadRangeMS3 { get; set; }
        public string BMSFanSpeed { get; set; }
        public string BMSVelocity { get; set; }
        public string BMSPressure { get; set; }
        public string BMSOther { get; set; }
        public string TotalAirPER { get; set; }
        public string HeadBuilding { get; set; }
        public string WitnessName { get; set; }
        public Nullable<System.DateTime> WitnessDate { get; set; }
        public string WitnessSignature { get; set; }
        public string BMSFanSpeedType { get; set; }
        public string BMSVelocityType { get; set; }
    
        public virtual tblSystemReport tblSystemReport { get; set; }
    }
}
