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
    
    public partial class Rpt_FcuCommissioning
    {
        public System.Guid ReportID { get; set; }
        public Nullable<System.Guid> SystemReportID_fk { get; set; }
        public string LocationAHU { get; set; }
        public string ManufactureAHU { get; set; }
        public string ModelNoAHU { get; set; }
        public string SerialNoAHU { get; set; }
        public string CapacityAHU { get; set; }
        public string DesignAirAHU { get; set; }
        public string DesignAirAHUPa { get; set; }
        public string MediaAHU { get; set; }
        public string DesignWaterAHU { get; set; }
        public string FanTypeAHU { get; set; }
        public string FanArrangementAHU { get; set; }
        public string MSSBNameAHU { get; set; }
        public string MSSBLocationAHU { get; set; }
        public string MotorMakeMD { get; set; }
        public string FrameMD { get; set; }
        public string ElectricalMD { get; set; }
        public string MotorPowerMD { get; set; }
        public string FullLoadMD { get; set; }
        public string PoleMD { get; set; }
        public string PoleRPMMD { get; set; }
        public string FuseDetailsMD { get; set; }
        public string FanPulleyFan { get; set; }
        public string FanBushFan { get; set; }
        public string FanBoreFan { get; set; }
        public string MotorPulleyFan { get; set; }
        public string MotorBushFan { get; set; }
        public string MotorBoreFan { get; set; }
        public string BeltsMakeFan { get; set; }
        public string BeltsNoFan { get; set; }
        public string BeltsSizeFan { get; set; }
        public string ShaftCentreFan { get; set; }
        public string OverloadMakeMS { get; set; }
        public string OverloadRangeMS { get; set; }
        public string VSDMakeMS { get; set; }
        public string VSDRangeMS { get; set; }
        public string ValveACT { get; set; }
        public string ModelACT { get; set; }
        public string SizeACT { get; set; }
        public string CoilACT { get; set; }
        public string ValveBAL { get; set; }
        public string ModelBAL { get; set; }
        public string SizeBAL { get; set; }
        public string FilterType1 { get; set; }
        public string FilterSize1 { get; set; }
        public string NoFilters1 { get; set; }
        public string FilterType2 { get; set; }
        public string FilterSize2 { get; set; }
        public string NoFilters2 { get; set; }
        public string SupplyAirDES { get; set; }
        public string ReturnAirDES { get; set; }
        public string OutsideAirDES { get; set; }
        public string PchwDES { get; set; }
        public string SchwDES { get; set; }
        public string TotalStaticDES { get; set; }
        public string SucStaticDES { get; set; }
        public string DisStaticDES { get; set; }
        public string FilterDiffDES { get; set; }
        public string PchwDiffDES { get; set; }
        public string ScheDiffDES { get; set; }
        public string VoltageDES { get; set; }
        public string AmpsDES { get; set; }
        public string SupplyAirFIN { get; set; }
        public string ReturnAirFIN { get; set; }
        public string OutsideAirFIN { get; set; }
        public string PchwFIN { get; set; }
        public string SchwFIN { get; set; }
        public string TotalStaticFIN { get; set; }
        public string SucStaticFIN { get; set; }
        public string DisStaticFIN { get; set; }
        public string FilterDiffFIN { get; set; }
        public string PchwDiffFIN { get; set; }
        public string ScheDiffFIN { get; set; }
        public string VoltageFIN { get; set; }
        public string AmpsFIN { get; set; }
        public string Instrument1 { get; set; }
        public string Model1 { get; set; }
        public string Serial1 { get; set; }
        public string Instrument2 { get; set; }
        public string Model2 { get; set; }
        public string Serial2 { get; set; }
        public string TechnicianComments { get; set; }
        public string ICAComments { get; set; }
        public string OverloadRangeMS2 { get; set; }
        public string OverloadRangeMS3 { get; set; }
        public string FilterSize1B { get; set; }
        public string FilterSize1C { get; set; }
        public string FilterSize2B { get; set; }
        public string FilterSize2C { get; set; }
        public string FuseDetailsMDPH { get; set; }
        public Nullable<bool> ShowSucStatic { get; set; }
        public Nullable<bool> ShowDisStatic { get; set; }
        public Nullable<bool> ShowFilterDiff { get; set; }
        public string CHWStaticFin { get; set; }
        public string HeadBuilding { get; set; }
        public string HeadService { get; set; }
        public string HeadCustomer { get; set; }
        public string BMSFanSpeed { get; set; }
        public string BMSVelocity { get; set; }
        public string BMSPressure { get; set; }
        public string BMSOther { get; set; }
        public string SupplyAirPER { get; set; }
        public string ReturnAirPER { get; set; }
        public string OutsideAirPER { get; set; }
        public string PchwPER { get; set; }
        public string HHWDES { get; set; }
        public string HHWFIN { get; set; }
        public string HHWPER { get; set; }
        public string WitnessName { get; set; }
        public Nullable<System.DateTime> WitnessDate { get; set; }
        public string WitnessSignature { get; set; }
        public string CapacityAHUHeat { get; set; }
        public string BMSFanSpeedType { get; set; }
        public string BMSVelocityType { get; set; }
    
        public virtual tblSystemReport tblSystemReport { get; set; }
    }
}
