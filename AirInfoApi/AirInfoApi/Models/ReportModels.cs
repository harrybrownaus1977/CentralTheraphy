using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace AirInfoApi.Models
{
    public class ReportModels
    {
    }

    public class UserViewModel
    {
        public string Username { set; get; }
        public string Password { set; get; }
        public string Token { set; get; }
        public string Message { set; get; }

        public string UserId { set; get; }


    }

    public class ProjectViewModel
    {
        public Guid ID { set; get; }
        public string Name { set; get; }
        public string JobNumber { get; set; }
        public string Address { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }

        public string CustomerName { get; set; }

    }

    public class GroupViewModel
    {
        public Guid ID { set; get; }
        public string Name { set; get; }
    }

    public class SystemViewModel
    {
        public Guid ID { set; get; }
        public string Name { set; get; }
    }

    public class SystemReportViewModel
    {
        public Guid ID { set; get; }
        public string Name { set; get; }
        public string TestReference { set; get; }
        public int Status { set; get; }
        public DateTime Date { set; get; }

    }

    public class MasterReportsViewModel
    {
        public Guid TemplateID { set; get; }
        public string Name { set; get; }

    }

    public class PreCommissioningReportViewModel
    {
        public Guid ReportID { set; get; }
        public Guid SystemReportID { set; get; }
        public string NonConformance { set; get; }
        public CommSystemReportViewModel Header { set; get; }

        public List<ItemsViewModel> Items { set; get; }

    }

    public class CommSystemReportViewModel
    {
        public Guid SystemReportID { set; get; }
        public Guid SystemID { set; get; }
        public Guid TemplateID { set; get; }
        public string Name { set; get; }
        public string TestReference { set; get; }
        public int Status { set; get; }
        public DateTime Date { set; get; }
    }

    public class ItemsViewModel
    {
        public Guid ReportItemID { set; get; }

        public Guid ReportID { set; get; }
        public Guid ReportItemListID { set; get; }
        public string ItemName { set; get; }
        public string CategoryName { set; get; }
        public bool IsNotApplicable { set; get; }
        public bool IsSatisfactory { set; get; }
        public bool IsNonConforming { set; get; }
    }


    public class FcuCommissioningReportViewModel
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
        public CommSystemReportViewModel Header { set; get; }
        public List<CommCommentViewModel> Comments { set; get; }
    }

    public class FcuDirectDriveCommissioningReportViewModel
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
        public string SupplyAirDES { get; set; }
        public string ReturnAirDES { get; set; }
        public string OutsideAirDES { get; set; }
        public string VoltageDES { get; set; }
        public string AmpsDES { get; set; }
        public string SupplyAirFIN { get; set; }
        public string ReturnAirFIN { get; set; }
        public string OutsideAirFIN { get; set; }
        public string TotalStaticFIN { get; set; }
        public string SucStaticFIN { get; set; }
        public string DisStaticFIN { get; set; }
        public string FilterDiffFIN { get; set; }
        public string VoltageFIN { get; set; }
        public string AmpsFIN { get; set; }
        public string Instrument1 { get; set; }
        public string Model1 { get; set; }
        public string Serial1 { get; set; }
        public string Instrument2 { get; set; }
        public string Model2 { get; set; }
        public string Serial2 { get; set; }
        public string OverloadRangeMS2 { get; set; }
        public string OverloadRangeMS3 { get; set; }
        public string FilterSize1B { get; set; }
        public string FilterSize1C { get; set; }
        public string FilterSize2B { get; set; }
        public string FilterSize2C { get; set; }
        public string FuseDetailsMDPH { get; set; }
        public string CHWStaticFin { get; set; }
        public string BMSFanSpeed { get; set; }
        public string BMSVelocity { get; set; }
        public string BMSPressure { get; set; }
        public string BMSOther { get; set; }
        public string SupplyAirPER { get; set; }
        public string ReturnAirPER { get; set; }
        public string OutsideAirPER { get; set; }
        public string HeadBuilding { get; set; }
        public string WitnessName { get; set; }
        public Nullable<System.DateTime> WitnessDate { get; set; }
        public string WitnessSignature { get; set; }
        public string BMSFanSpeedType { get; set; }
        public string BMSVelocityType { get; set; }
        public CommSystemReportViewModel Header { set; get; }
        public List<CommCommentViewModel> Comments { set; get; }
    }

    public class VavCommissioningReportViewModel
    {
        public System.Guid ReportID { get; set; }
        public Nullable<System.Guid> SystemReportID_fk { get; set; }
        public Nullable<int> NoVavs { get; set; }
        public Nullable<bool> SetUpComplete { get; set; }
        public string HeadBuilding { get; set; }
        public Nullable<double> AHUDesignTotal { get; set; }
        public VavCommissioningDataViewModel ReportData { set; get; }
        public CommSystemReportViewModel Header { set; get; }
        public List<CommCommentViewModel> Comments { set; get; }
    }

    public class VavCommissioningDataViewModel
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> ReportID_fk { get; set; }
        public string VavReference { get; set; }
        public Nullable<int> NoGrilles { get; set; }
        public string VavManufacturer { get; set; }
        public string VavType { get; set; }
        public string ReheatType { get; set; }
        public string GrilleType { get; set; }
        public Nullable<int> GrilleSizeX { get; set; }
        public Nullable<int> GrilleSizeY { get; set; }
        public Nullable<double> GrilleFactor { get; set; }
        public string TestApp { get; set; }
        public Nullable<double> VMin { get; set; }
        public Nullable<double> VMax { get; set; }
        public Nullable<double> ResultsDesVMin { get; set; }
        public Nullable<double> ResultsDesVMax { get; set; }
        public Nullable<double> ResultsFinVMin { get; set; }
        public Nullable<double> ResultsFinVMax { get; set; }
        public string BmsKFactor { get; set; }
        public Nullable<double> BmsVMin { get; set; }
        public Nullable<double> BmsVMax { get; set; }
        public Nullable<double> BMSLsTotal { get; set; }
        public string BMSDamperPosition { get; set; }
        public Nullable<double> InletSize { get; set; }
    }

    public class PumpCommissioningReportViewModel
    {
        public System.Guid ReportID { get; set; }
        public Nullable<System.Guid> SystemReportID_fk { get; set; }
        public string LocationPump { get; set; }
        public string WaterTypePump { get; set; }
        public string SBoardRef { get; set; }
        public string SBoardLoc { get; set; }
        public string ManufacturePump { get; set; }
        public string ModelNoPump { get; set; }
        public string SerialNoPump { get; set; }
        public string PumpType { get; set; }
        public string ImpSize { get; set; }
        public string PumpLs { get; set; }
        public string PumpKPA { get; set; }
        public string CoilVesselLs { get; set; }
        public string CoilVesselKPA { get; set; }
        public string MotorMakeMD { get; set; }
        public string FrameMD { get; set; }
        public string ElectricalMD { get; set; }
        public string MotorPowerMD { get; set; }
        public string FullLoadMD { get; set; }
        public string PoleMD { get; set; }
        public string PoleRPMMD { get; set; }
        public string FuseDetailsMD { get; set; }
        public string OverloadMakeMS { get; set; }
        public string OverloadRangeMS { get; set; }
        public string VSDMakeMS { get; set; }
        public string VSDRangeMS { get; set; }
        public string SystemOpp { get; set; }
        public string TestPoint { get; set; }
        public string Aligned { get; set; }
        public string Strainer { get; set; }
        public string Curve { get; set; }
        public string Suction1 { get; set; }
        public string Discharge1 { get; set; }
        public string Delta1 { get; set; }
        public string RPM1 { get; set; }
        public string Amps1 { get; set; }
        public string Hz1 { get; set; }
        public string Suction2 { get; set; }
        public string Discharge2 { get; set; }
        public string Delta2 { get; set; }
        public string RPM2 { get; set; }
        public string Amps2 { get; set; }
        public string Hz2 { get; set; }
        public string Suction3 { get; set; }
        public string Discharge3 { get; set; }
        public string Delta3 { get; set; }
        public string RPM3 { get; set; }
        public string Amps3 { get; set; }
        public string Hz3 { get; set; }
        public string ClosedPressure { get; set; }
        public string OpenPressure { get; set; }
        public string ClosedHz { get; set; }
        public string OpenHz { get; set; }
        public string PumpFinalLS { get; set; }
        public string PumpFinalKPA { get; set; }
        public string PumpFinalHz { get; set; }
        public string PumpOp { get; set; }
        public string PumpStatic { get; set; }
        public string Instrument1 { get; set; }
        public string Model1 { get; set; }
        public string Serial1 { get; set; }
        public string Instrument2 { get; set; }
        public string Model2 { get; set; }
        public string Serial2 { get; set; }
        public string TechnicianComments { get; set; }
        public string ICAComments { get; set; }
        public string FuseDetailsMDPH { get; set; }
        public string OverloadRangeMS2 { get; set; }
        public string OverloadRangeMS3 { get; set; }
        public string FullFlowLS { get; set; }
        public string FinalFlowLS { get; set; }
        public string HeadBuilding { get; set; }
        public string HeadService { get; set; }
        public string HeadCustomer { get; set; }
        public string BMSPumpSpeed { get; set; }
        public string BMSVelocity { get; set; }
        public string BMSPressure { get; set; }
        public string BMSOther { get; set; }
        public string PumpFinalFinLs { get; set; }
        public string PumpFinalFinKPa { get; set; }
        public string PumpFinalFinPer { get; set; }
        public string CoilLs { get; set; }
        public string CoilKPa { get; set; }
        public string CoilFinLs { get; set; }
        public string CoilFinKPa { get; set; }
        public string CoilFinPer { get; set; }
        public string WitnessName { get; set; }
        public Nullable<System.DateTime> WitnessDate { get; set; }
        public string WitnessSignature { get; set; }
        public string BMSPumpSpeedType { get; set; }
        public string BMSVelocityType { get; set; }
        public CommSystemReportViewModel Header { set; get; }
        public List<CommCommentViewModel> Comments { set; get; }
    }

    public class SplitPackageCommissioningReportViewModel
    {
        public System.Guid ReportID { get; set; }
        public Nullable<System.Guid> SystemReportID_fk { get; set; }
        public string FanLoc { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNo { get; set; }
        public string SerialNo { get; set; }
        public string Capacity { get; set; }
        public string FanType { get; set; }
        public string FanArrange { get; set; }
        public string MSSBName { get; set; }
        public string MSSBLoc { get; set; }
        public string TotDes { get; set; }
        public string TotAct { get; set; }
        public string OutDes { get; set; }
        public string OutAct { get; set; }
        public string ReturnDes { get; set; }
        public string ReturnAct { get; set; }
        public string ExtDes { get; set; }
        public string ExtAct { get; set; }
        public string SucDes { get; set; }
        public string SucAct { get; set; }
        public string DisDes { get; set; }
        public string DisAct { get; set; }
        public string MotorMake { get; set; }
        public string FrameClass { get; set; }
        public string ElectSupply { get; set; }
        public string MotorPower { get; set; }
        public string FullLoadAmps { get; set; }
        public string Pole { get; set; }
        public string RPM { get; set; }
        public string FuseDetails { get; set; }
        public string FanDia { get; set; }
        public string FanBush { get; set; }
        public string FanBore { get; set; }
        public string MotorDia { get; set; }
        public string MotorBush { get; set; }
        public string MotorBore { get; set; }
        public string BeltsMake { get; set; }
        public string BeltsNo { get; set; }
        public string BeltsSize { get; set; }
        public string ShaftCentre { get; set; }
        public string FilterType { get; set; }
        public string FilterSize { get; set; }
        public string NoOfFilters { get; set; }
        public string Location { get; set; }
        public string CondManu { get; set; }
        public string CondMNo { get; set; }
        public string CondSNo { get; set; }
        public string CompManu { get; set; }
        public string CompModNo { get; set; }
        public string CompElec { get; set; }
        public string CompFull { get; set; }
        public string Ref { get; set; }
        public string Kg { get; set; }
        public string HeatingType { get; set; }
        public string CondFanMake { get; set; }
        public string CondFanNo { get; set; }
        public string CondFan { get; set; }
        public string MotorPowerC { get; set; }
        public string FullLoadAmpsC { get; set; }
        public string PoleC { get; set; }
        public string RPMC { get; set; }
        public string FuseDetailsC { get; set; }
        public string SupS1 { get; set; }
        public string SupS2 { get; set; }
        public string SupS3 { get; set; }
        public string SupS4 { get; set; }
        public string CompS1 { get; set; }
        public string CompS2 { get; set; }
        public string CompS3 { get; set; }
        public string CompS4 { get; set; }
        public string CompVoltS1 { get; set; }
        public string CompVoltS2 { get; set; }
        public string CompVoltS3 { get; set; }
        public string CompVoltS4 { get; set; }
        public string AmbiS1 { get; set; }
        public string AmbiS2 { get; set; }
        public string AmbiS3 { get; set; }
        public string AmbiS4 { get; set; }
        public string SucS1 { get; set; }
        public string SucS2 { get; set; }
        public string SucS3 { get; set; }
        public string SucS4 { get; set; }
        public string DisS1 { get; set; }
        public string DisS2 { get; set; }
        public string DisS3 { get; set; }
        public string DisS4 { get; set; }
        public string OilS1 { get; set; }
        public string OilS2 { get; set; }
        public string OilS3 { get; set; }
        public string OilS4 { get; set; }
        public string CompSucS1 { get; set; }
        public string CompSucS2 { get; set; }
        public string CompSucS3 { get; set; }
        public string CompSucS4 { get; set; }
        public string LiqS1 { get; set; }
        public string LiqS2 { get; set; }
        public string LiqS3 { get; set; }
        public string LiqS4 { get; set; }
        public string SuperS1 { get; set; }
        public string SuperS2 { get; set; }
        public string SuperS3 { get; set; }
        public string SuperS4 { get; set; }
        public string SubS1 { get; set; }
        public string SubS2 { get; set; }
        public string SubS3 { get; set; }
        public string SubS4 { get; set; }
        public string EvapEntering1 { get; set; }
        public string EvapEntering2 { get; set; }
        public string EvapEntering3 { get; set; }
        public string EvapEntering4 { get; set; }
        public string EvapLeaving1 { get; set; }
        public string EvapLeaving2 { get; set; }
        public string EvapLeaving3 { get; set; }
        public string EvapLeaving4 { get; set; }
        public string CondVoltS1 { get; set; }
        public string CondVoltS2 { get; set; }
        public string CondVoltS3 { get; set; }
        public string CondVoltS4 { get; set; }
        public string FCondAmpsS1 { get; set; }
        public string FCondAmpsS2 { get; set; }
        public string FCondAmpsS3 { get; set; }
        public string FCondAmpsS4 { get; set; }
        public string ICAComments { get; set; }
        public string Capacity2 { get; set; }
        public string FuseDetailsMDPH { get; set; }
        public string Kg2 { get; set; }
        public string Kg3 { get; set; }
        public string Kg4 { get; set; }
        public string FuseDetailsCMDPH { get; set; }
        public string FilterSizeB { get; set; }
        public string FilterSizeC { get; set; }
        public string HeadBuilding { get; set; }
        public string HeadService { get; set; }
        public string HeadCustomer { get; set; }
        public CommSystemReportViewModel Header { set; get; }
        public List<CommCommentViewModel> Comments { set; get; }
    }

    public class AirCommissioningReportViewModel
    {
        public System.Guid ReportID { get; set; }
        public Nullable<System.Guid> SystemReportID_fk { get; set; }
        public string ZoneSystem { get; set; }
        public Nullable<bool> ZoneSetUpComplete { get; set; }
        public string ICAComments { get; set; }
        public string HeadBuilding { get; set; }
        public string HeadService { get; set; }
        public string HeadCustomer { get; set; }

        public List<AirCommissioningDataViewModel> ReportData { set; get; }
        public CommSystemReportViewModel Header { set; get; }
        public List<CommCommentViewModel> Comments { set; get; }
    }

    public class AirCommissioningDataViewModel
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> ReportID_fk { get; set; }
        public string BranchRef { get; set; }
        public string AreaZone { get; set; }
        public Nullable<int> NoGrilles { get; set; }
        public string GrilleType { get; set; }
        public Nullable<double> SizeA { get; set; }
        public Nullable<double> SizeB { get; set; }
        public Nullable<double> Factor { get; set; }
        public Nullable<double> CorrectedArea { get; set; }
        public string TestApparatus { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public string ZoneType { get; set; }
        public string GrilleRef { get; set; }
        public double DesFlow { get; set; }
    }

    public class CommCommentViewModel
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> SysRepID_fk { get; set; }
        public Nullable<System.Guid> TechnicianID_fk { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string Comments { get; set; }
    }

    public class BearerToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty(".issued")]
        public string Issued { get; set; }

        [JsonProperty(".expires")]
        public string Expires { get; set; }
    }

    //public class CreateUserViewModel
    //{
    //    public int Id { set; get; }
    //    public string Name { set; get; }
    //    public List<TagViewModel> Tags { set; get; }
    //}
    //public class TagViewModel
    //{
    //    public int Id { set; get; }
    //    public string Code { set; get; }
    //}


}