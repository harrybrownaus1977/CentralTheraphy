﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AirInfoApi.Models;

namespace AirInfoApi.Controllers
{
    public class ReportsController : ApiController
    {
        AirInfoAPIEntities context = new AirInfoAPIEntities();

        /// <summary>
        /// Check the username and password to validate the user. generate new authentication token.
        /// </summary>
        /// <returns>
        /// The user object oUser with the authentication token.
        /// </returns>
        /// <remarks>
        /// Need to implement same encrypt/decrypt function for password. At the moment it just check for the user name only. 
        /// </remarks>
        [Route("reports/login")]
        [HttpPost]
        //username = Megabits
        public HttpResponseMessage login([FromBody] UserViewModel oUser)
        {            
            try
            {
                //generate Bearer token based on the user.
                BearerToken token;
                using (var httpClient = new HttpClient())
                {
                    var tokenRequest =
                        new List<KeyValuePair<string, string>>
                            {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", oUser.Username),
                        new KeyValuePair<string, string>("password", oUser.Password)
                            };
                    HttpContent encodedRequest = new FormUrlEncodedContent(tokenRequest);
                    HttpResponseMessage response = httpClient.PostAsync("http://203.143.85.102/plesk-site-preview/api.airinfo.com.au/Token", encodedRequest).Result;
                    //HttpResponseMessage response = httpClient.PostAsync("http://localhost:6719/Token", encodedRequest).Result;
                    if (response.IsSuccessStatusCode == true)
                    {
                        token = response.Content.ReadAsAsync<BearerToken>().Result;
                        oUser.Token = token.AccessToken;

                        var user = context.AspNetUsers.Where(x => x.Email == oUser.Username).FirstOrDefault();
                        oUser.UserId = user.Id.ToString();

                        //Above two lines should uncomment after changed user logins to AspNetxxx tables
                        //Below two lines should comment after changed user logins to AspNetxxx tables
                        //var user = context.tblUsers.Where(x => x.Username == "Megabits").FirstOrDefault();
                        //oUser.UserId = user.UserID.ToString();
                        oUser.Password = "";
                        oUser.Username = "";
                    }
                    else
                    {
                        oUser.Message = "Invalid Username or password.";
                        oUser.Password = "";
                        oUser.Username = "";
                        return Request.CreateResponse(oUser);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, oUser);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }

        }

        /// <summary>
        /// Get assigned projects for the specific user.
        /// </summary>
        /// <returns>
        /// The list of projects assigned for specific user.
        /// </returns>        
        //[Authorize]
        [Route("reports/GetProjectsByUserID")]
        [HttpPost]
        public HttpResponseMessage GetProjectsByUserID([FromBody] string userid)
        {
            try
            {
                //var val = Guid.Parse(userid);
                List<ProjectViewModel> projectList = new List<ProjectViewModel>();
                var projectids = context.tblProjectTechnicians.Where(x => x.UserID_fk == userid).ToList();
                foreach (var projid in projectids)
                {
                    var proj = context.tblProjects.Where(y => y.ProjectID == projid.ProjectID_fk).Select(y => new ProjectViewModel
                    {
                        ID = y.ProjectID,
                        Name = y.Name,
                        JobNumber = y.JobNumber,
                        Address = y.Address,
                        Suburb = y.Suburb,
                        State = y.State,
                        PostCode = y.PostCode,
                        CustomerName = y.tblCustomer.BusinessName,
                    }).FirstOrDefault();
                    projectList.Add(proj);
                }
                return Request.CreateResponse(HttpStatusCode.OK, projectList);
            }
            catch(Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
            
        }

        /// <summary>
        /// Get assigned groups for specific project.
        /// </summary>
        /// <returns>
        /// The list of groups for specific project.
        /// </returns>        
        [Authorize]
        [Route("reports/GetGroupsByProjectID")]
        [HttpPost]
        public HttpResponseMessage GetGroupsByProjectID([FromBody] string projectid)
        {
            try
            {
                var val = Guid.Parse(projectid);
                List<SystemViewModel> lstSystems = new List<SystemViewModel>();
                var groupList = context.tblProjectGroups.Where(x => x.ProjectID_fk == val).Select(x => new GroupViewModel
                {
                    ID = x.GroupID,
                    Name = x.Name,
                    Systems = context.tblGroupSystems.Where(y => y.GroupID_fk == x.GroupID).Select(z => new SystemViewModel
                                {
                                    ID = z.SystemID,
                                    Name = z.Reference
                                }).ToList()
                }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, groupList);
            }
            catch(Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
            
        }

        /// <summary>
        /// Get assigned systems for specific group.
        /// </summary>
        /// <returns>
        /// The list of systems for specific group.
        /// </returns>
        [Authorize]
        [Route("reports/GetSystemsByGroupID")]
        [HttpPost]
        public HttpResponseMessage GetSystemsByGroupID([FromBody] string groupid)
        {
            try
            {
                var val = Guid.Parse(groupid);
                var systemList = context.tblGroupSystems.Where(x => x.GroupID_fk == val).Select(x => new SystemViewModel
                {
                    ID = x.SystemID,
                    Name = x.Reference,
                }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, systemList);
            }
            catch(Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
            
        }

        /// <summary>
        /// Get assigned report templates for specific system.
        /// </summary>
        /// <returns>
        /// The list of report templates assigned for specific group.
        /// </returns>
        [Authorize]
        [Route("reports/GetTemplatesBySystemID")]
        [HttpPost]
        public HttpResponseMessage GetTemplatesBySystemID([FromBody] string systemid)
        {
            try
            {
                var val = Guid.Parse(systemid);
                List<MasterReportsViewModel> masterreportList = new List<MasterReportsViewModel>();
                var assignedmasterids = context.tblSystemMasterReports.Where(x => x.SystemID_fk == val).ToList();
                foreach (var assignedmasterid in assignedmasterids)
                {
                    var masterRep = context.luMasterReportLists.Where(y => y.TemplateID == assignedmasterid.TemplateID_fk).Select(y => new MasterReportsViewModel
                    {
                        TemplateID = y.TemplateID,
                        Name = y.DisplayName,
                    }).FirstOrDefault();
                    masterreportList.Add(masterRep);
                }
                return Request.CreateResponse(HttpStatusCode.OK, masterreportList);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }            
        }

        /// <summary>
        /// Get created reports by using selected templates, for specific system.
        /// </summary>
        /// <returns>
        /// The list of reports created by using templates, for specific system.
        /// </returns>
        [Authorize]
        [Route("reports/GetReportsBySystemID")]
        [HttpPost]
        public HttpResponseMessage GetReportsBySystemID([FromBody] string systemid)
        {
            try
            {
                var val = Guid.Parse(systemid);
                var systemReportList = context.tblSystemReports.Where(x => x.SystemID_fk == val).Select(x => new SystemReportViewModel
                {
                    ID = x.SystemReportID,
                    Name = x.Name,
                    TestReference = x.TestReference,
                    Status = x.Status.Value,
                    Date = x.DateTime.Value,
                }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, systemReportList);
            }
            catch(Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
            
        }

        /// <summary>
        /// Get PreCommissioning report details for given system report id. This common method is using for all 4 pre commissioning reports.
        /// </summary>
        /// <returns>
        /// The oPreCommissioningReport object which contains all the information.
        /// </returns>
        [Authorize]
        [Route("reports/GetPreCommissioningReportBySystemReportID")]
        [HttpPost]
        public HttpResponseMessage GetPreCommissioningReportBySystemReportID([FromBody] string systemreportid)
        {
            try
            {
                var val = Guid.Parse(systemreportid);
                var oPreCommissioningReport = context.Rpt_PreCommissioning.Where(x => x.SystemReportID_fk == val).Select(x => new PreCommissioningReportViewModel
                {
                    ReportID = x.ReportID,
                    SystemReportID = x.SystemReportID_fk.Value,
                    NonConformance = x.NonConformance,
                }
                ).FirstOrDefault();

                //Get header information for the report
                var oReportHeader = context.tblSystemReports.Where(x => x.SystemReportID == val).Select(x => new CommSystemReportViewModel
                {
                    SystemReportID = x.SystemReportID,
                    SystemID = x.SystemID_fk.Value,
                    TemplateID = x.TemplateID_fk.Value,
                    Name = x.Name,
                    TestReference = x.TestReference,
                    Status = x.Status.Value,
                    Date = x.DateTime.Value,
                }
                ).FirstOrDefault();
                oPreCommissioningReport.Header = oReportHeader;

                //Get items information of the project
                var oReportItems = context.Rpt_PreCommissioningItems.Where(x => x.ReportID_fk == oPreCommissioningReport.ReportID).Select(x => new ItemsViewModel
                {
                    ReportItemID = x.ReportItemID,
                    ReportID = x.ReportID_fk.Value,
                    ReportItemListID = x.ReportItemListID_fk.Value,
                    ItemName = x.Rpt_PreCommissioningItemsList.ItemName,
                    CategoryName = x.Rpt_PreCommissioningItemsList.CategoryName,
                    IsNotApplicable = x.IsNotApplicable.Value,
                    IsSatisfactory = x.IsSatisfactory.Value,
                    IsNonConforming = x.IsNonConforming.Value,
                }
                ).ToList();

                oPreCommissioningReport.Items = oReportItems;

                return Request.CreateResponse(HttpStatusCode.OK, oPreCommissioningReport);
            }
            catch(Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
            
        }

        /// <summary>
        /// Save PreCommissioning report data into the relavant tables. This common method is using for all 4 pre commissioning reports.
        /// It should provide systemid and templateid along with the report related data.
        /// </summary>
        /// <returns>
        /// The Satatus based on the success or failed.
        /// </returns>
        [Authorize]
        [Route("reports/SavePreCommissioningReport")]
        [HttpPost]
        public HttpResponseMessage SavePreCommissioningReport([FromBody] PreCommissioningReportViewModel oPreCommissioningReport)
        {
            try
            {
                //Save report header data and generate system report id.
                tblSystemReport oSystemReport = new tblSystemReport();
                oSystemReport.SystemReportID = Guid.NewGuid();
                oSystemReport.SystemID_fk = oPreCommissioningReport.Header.SystemID;
                oSystemReport.TemplateID_fk = oPreCommissioningReport.Header.TemplateID;
                oSystemReport.Name = oPreCommissioningReport.Header.Name;
                oSystemReport.TestReference = oPreCommissioningReport.Header.TestReference;
                oSystemReport.Status = oPreCommissioningReport.Header.Status;
                oSystemReport.DateTime = oPreCommissioningReport.Header.Date;
                context.tblSystemReports.Add(oSystemReport);
                context.SaveChanges();
                var SysReportId = oSystemReport.SystemReportID;

                //Save data to the report table.
                Rpt_PreCommissioning oRptPreCommissioning = new Rpt_PreCommissioning();
                oRptPreCommissioning.ReportID = Guid.NewGuid();
                oRptPreCommissioning.SystemReportID_fk = SysReportId;
                oRptPreCommissioning.NonConformance = oPreCommissioningReport.NonConformance;
                context.Rpt_PreCommissioning.Add(oRptPreCommissioning);
                context.SaveChanges();
                var RepId = oRptPreCommissioning.ReportID;

                //Save items of the report.
                List<ItemsViewModel> lstItems = new List<ItemsViewModel>();
                lstItems = oPreCommissioningReport.Items;
                foreach (var item in lstItems)
                {
                    Rpt_PreCommissioningItems oRptCommissioningItem = new Rpt_PreCommissioningItems();
                    oRptCommissioningItem.ReportItemID = Guid.NewGuid();
                    oRptCommissioningItem.ReportID_fk = RepId;
                    oRptCommissioningItem.ReportItemListID_fk = item.ReportItemListID;
                    oRptCommissioningItem.IsNotApplicable = item.IsNotApplicable;
                    oRptCommissioningItem.IsSatisfactory = item.IsSatisfactory;
                    oRptCommissioningItem.IsNonConforming = item.IsNonConforming;
                    context.Rpt_PreCommissioningItems.Add(oRptCommissioningItem);
                    context.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch(Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
        }

        /// <summary>
        /// Get Fcu Commissioning report details for given system report id.
        /// </summary>
        /// <returns>
        /// The oFcuCommissioningReport object which contains all the information.
        /// </returns>        
        [Authorize]
        [Route("reports/GetFcuCommissioningReportBySystemReportID")]
        [HttpPost]
        public HttpResponseMessage GetFcuCommissioningReportBySystemReportID([FromBody] string systemreportid)
        {
            try
            {
                //Get report details from the report table by providing systemreportid.
                var val = Guid.Parse(systemreportid);
                var oFcuCommissioningReport = context.Rpt_FcuCommissioning.Where(x => x.SystemReportID_fk == val).Select(x => new FcuCommissioningReportViewModel
                {
                    ReportID = x.ReportID,
                    SystemReportID_fk = x.SystemReportID_fk.Value,
                    LocationAHU = x.LocationAHU,
                    ManufactureAHU = x.ManufactureAHU,
                    ModelNoAHU = x.ModelNoAHU,
                    SerialNoAHU = x.SerialNoAHU,
                    CapacityAHU = x.CapacityAHU,
                    DesignAirAHU = x.DesignAirAHU,
                    DesignAirAHUPa = x.DesignAirAHUPa,
                    MediaAHU = x.MediaAHU,
                    DesignWaterAHU = x.DesignWaterAHU,
                    FanTypeAHU = x.FanTypeAHU,
                    FanArrangementAHU = x.FanArrangementAHU,
                    MSSBNameAHU = x.MSSBNameAHU,
                    MSSBLocationAHU = x.MSSBLocationAHU,
                    MotorMakeMD = x.MotorMakeMD,
                    FrameMD = x.FrameMD,
                    ElectricalMD = x.ElectricalMD,
                    MotorPowerMD = x.MotorPowerMD,
                    FullLoadMD = x.FullLoadMD,
                    PoleMD = x.PoleMD,
                    PoleRPMMD = x.PoleRPMMD,
                    FuseDetailsMD = x.FuseDetailsMD,
                    FanPulleyFan = x.FanPulleyFan,
                    FanBushFan = x.FanBushFan,
                    FanBoreFan = x.FanBoreFan,
                    MotorPulleyFan = x.MotorPulleyFan,
                    MotorBushFan = x.MotorBushFan,
                    MotorBoreFan = x.MotorBoreFan,
                    BeltsMakeFan = x.BeltsMakeFan,
                    BeltsNoFan = x.BeltsNoFan,
                    BeltsSizeFan = x.BeltsSizeFan,
                    ShaftCentreFan = x.ShaftCentreFan,
                    OverloadMakeMS = x.OverloadMakeMS,
                    OverloadRangeMS = x.OverloadRangeMS,
                    VSDMakeMS = x.VSDMakeMS,
                    VSDRangeMS = x.VSDRangeMS,
                    ValveACT = x.ValveACT,
                    ModelACT = x.ModelACT,
                    SizeACT = x.SizeACT,
                    CoilACT = x.CoilACT,
                    ValveBAL = x.ValveBAL,
                    ModelBAL = x.ModelBAL,
                    SizeBAL = x.SizeBAL,
                    FilterType1 = x.FilterType1,
                    FilterSize1 = x.FilterSize1,
                    NoFilters1 = x.NoFilters1,
                    FilterType2 = x.FilterType2,
                    FilterSize2 = x.FilterSize2,
                    NoFilters2 = x.NoFilters2,
                    SupplyAirDES = x.SupplyAirDES,
                    ReturnAirDES = x.ReturnAirDES,
                    OutsideAirDES = x.OutsideAirDES,
                    PchwDES = x.PchwDES,
                    SchwDES = x.SchwDES,
                    TotalStaticDES = x.TotalStaticDES,
                    SucStaticDES = x.SucStaticDES,
                    DisStaticDES = x.DisStaticDES,
                    FilterDiffDES = x.FilterDiffDES,
                    PchwDiffDES = x.PchwDiffDES,
                    ScheDiffDES = x.ScheDiffDES,
                    VoltageDES = x.VoltageDES,
                    AmpsDES = x.AmpsDES,
                    SupplyAirFIN = x.SupplyAirFIN,
                    ReturnAirFIN = x.SupplyAirFIN,
                    OutsideAirFIN = x.OutsideAirFIN,
                    PchwFIN = x.PchwFIN,
                    SchwFIN = x.SchwFIN,
                    TotalStaticFIN = x.TotalStaticFIN,
                    SucStaticFIN = x.SucStaticFIN,
                    DisStaticFIN = x.DisStaticFIN,
                    FilterDiffFIN = x.FilterDiffFIN,
                    PchwDiffFIN = x.PchwDiffFIN,
                    ScheDiffFIN = x.ScheDiffFIN,
                    VoltageFIN = x.VoltageFIN,
                    AmpsFIN = x.AmpsFIN,
                    Instrument1 = x.Instrument1,
                    Model1 = x.Model1,
                    Serial1 = x.Serial1,
                    Instrument2 = x.Instrument2,
                    Model2 = x.Model2,
                    Serial2 = x.Serial2,
                    TechnicianComments = x.TechnicianComments,
                    ICAComments = x.ICAComments,
                    OverloadRangeMS2 = x.OverloadRangeMS2,
                    OverloadRangeMS3 = x.OverloadRangeMS3,
                    FilterSize1B = x.FilterSize1B,
                    FilterSize1C = x.FilterSize1C,
                    FilterSize2B = x.FilterSize2B,
                    FilterSize2C = x.FilterSize2C,
                    FuseDetailsMDPH = x.FuseDetailsMDPH,
                    ShowSucStatic = x.ShowSucStatic,
                    ShowDisStatic = x.ShowDisStatic,
                    ShowFilterDiff = x.ShowFilterDiff,
                    CHWStaticFin = x.CHWStaticFin,
                    HeadBuilding = x.HeadBuilding,
                    HeadService = x.HeadService,
                    HeadCustomer = x.HeadCustomer,
                    BMSFanSpeed = x.BMSFanSpeed,
                    BMSVelocity = x.BMSVelocity,
                    BMSPressure = x.BMSPressure,
                    BMSOther = x.BMSOther,
                    SupplyAirPER = x.SupplyAirPER,
                    ReturnAirPER = x.ReturnAirPER,
                    OutsideAirPER = x.OutsideAirPER,
                    PchwPER = x.PchwPER,
                    HHWPER = x.HHWPER,
                    HHWDES = x.HHWDES,
                    HHWFIN = x.HHWFIN,
                    WitnessName = x.WitnessName,
                    WitnessDate = x.WitnessDate,
                    WitnessSignature = x.WitnessSignature,
                    CapacityAHUHeat = x.CapacityAHUHeat,
                    BMSFanSpeedType = x.BMSFanSpeedType,
                    BMSVelocityType = x.BMSVelocityType,
                }
                ).FirstOrDefault();

                //Get report header information and assign them to oFcuCommissioningReport object.
                var oReportHeader = context.tblSystemReports.Where(x => x.SystemReportID == val).Select(x => new CommSystemReportViewModel
                {
                    SystemReportID = x.SystemReportID,
                    SystemID = x.SystemID_fk.Value,
                    TemplateID = x.TemplateID_fk.Value,
                    Name = x.Name,
                    TestReference = x.TestReference,
                    Status = x.Status.Value,
                    Date = x.DateTime.Value,
                }
                ).FirstOrDefault();
                oFcuCommissioningReport.Header = oReportHeader;

                //Get report comments and assign them to the oFcuCommissioningReport object.
                var oReportComments = context.tblSystemReportComments.Where(x => x.SysRepID_fk == val).Select(x => new CommCommentViewModel
                {
                    ID = x.ID,
                    SysRepID_fk = x.SysRepID_fk,
                    TechnicianID_fk = x.TechnicianID_fk,
                    DateCreated = x.DateCreated,
                    Comments = x.Comments,
                }
                ).ToList();
                oFcuCommissioningReport.Comments = oReportComments;

                return Request.CreateResponse(HttpStatusCode.OK, oFcuCommissioningReport);
            }
            catch(Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
            
        }

        /// <summary>
        /// Save Fcu Commissioning report data into the relavant tables.
        /// It should provide systemid and templateid along with the report related data. 
        /// </summary>
        /// <returns>
        /// The Satatus based on the success or failed.
        /// </returns>
        [Authorize]
        [Route("reports/SaveFcuCommissioningReport")]
        [HttpPost]
        public HttpResponseMessage SaveFcuCommissioningReport([FromBody] FcuCommissioningReportViewModel oFcuCommissioningReport)
        {
            try
            {
                //Save report header information.
                tblSystemReport oSystemReport = new tblSystemReport();
                oSystemReport.SystemReportID = Guid.NewGuid();
                oSystemReport.SystemID_fk = oFcuCommissioningReport.Header.SystemID;
                oSystemReport.TemplateID_fk = oFcuCommissioningReport.Header.TemplateID;
                oSystemReport.Name = oFcuCommissioningReport.Header.Name;
                oSystemReport.TestReference = oFcuCommissioningReport.Header.TestReference;
                oSystemReport.Status = oFcuCommissioningReport.Header.Status;
                oSystemReport.DateTime = oFcuCommissioningReport.Header.Date;
                context.tblSystemReports.Add(oSystemReport);
                context.SaveChanges();
                var SysReportId = oSystemReport.SystemReportID;

                //Save report details information.
                Rpt_FcuCommissioning oRptFcuCommissioning = new Rpt_FcuCommissioning();
                oRptFcuCommissioning.ReportID = Guid.NewGuid();
                oRptFcuCommissioning.SystemReportID_fk = SysReportId;
                oRptFcuCommissioning.LocationAHU = oFcuCommissioningReport.LocationAHU;
                oRptFcuCommissioning.ManufactureAHU = oFcuCommissioningReport.ManufactureAHU;
                oRptFcuCommissioning.ModelNoAHU = oFcuCommissioningReport.ModelNoAHU;
                oRptFcuCommissioning.SerialNoAHU = oFcuCommissioningReport.SerialNoAHU;
                oRptFcuCommissioning.CapacityAHU = oFcuCommissioningReport.CapacityAHU;
                oRptFcuCommissioning.DesignAirAHU = oFcuCommissioningReport.DesignAirAHU;
                oRptFcuCommissioning.DesignAirAHUPa = oFcuCommissioningReport.DesignAirAHUPa;
                oRptFcuCommissioning.MediaAHU = oFcuCommissioningReport.MediaAHU;
                oRptFcuCommissioning.DesignWaterAHU = oFcuCommissioningReport.DesignWaterAHU;
                oRptFcuCommissioning.FanTypeAHU = oFcuCommissioningReport.FanTypeAHU;
                oRptFcuCommissioning.FanArrangementAHU = oFcuCommissioningReport.FanArrangementAHU;
                oRptFcuCommissioning.MSSBNameAHU = oFcuCommissioningReport.MSSBNameAHU;
                oRptFcuCommissioning.MSSBLocationAHU = oFcuCommissioningReport.MSSBLocationAHU;
                oRptFcuCommissioning.MotorMakeMD = oFcuCommissioningReport.MotorMakeMD;
                oRptFcuCommissioning.FrameMD = oFcuCommissioningReport.FrameMD;
                oRptFcuCommissioning.ElectricalMD = oFcuCommissioningReport.ElectricalMD;
                oRptFcuCommissioning.MotorPowerMD = oFcuCommissioningReport.MotorPowerMD;
                oRptFcuCommissioning.FullLoadMD = oFcuCommissioningReport.FullLoadMD;
                oRptFcuCommissioning.PoleMD = oFcuCommissioningReport.PoleMD;
                oRptFcuCommissioning.PoleRPMMD = oFcuCommissioningReport.PoleRPMMD;
                oRptFcuCommissioning.FuseDetailsMD = oFcuCommissioningReport.FuseDetailsMD;
                oRptFcuCommissioning.FanPulleyFan = oFcuCommissioningReport.FanPulleyFan;
                oRptFcuCommissioning.FanBushFan = oFcuCommissioningReport.FanBushFan;
                oRptFcuCommissioning.FanBoreFan = oFcuCommissioningReport.FanBoreFan;
                oRptFcuCommissioning.MotorPulleyFan = oFcuCommissioningReport.MotorPulleyFan;
                oRptFcuCommissioning.MotorBushFan = oFcuCommissioningReport.MotorBushFan;
                oRptFcuCommissioning.MotorBoreFan = oFcuCommissioningReport.MotorBoreFan;
                oRptFcuCommissioning.BeltsMakeFan = oFcuCommissioningReport.BeltsMakeFan;
                oRptFcuCommissioning.BeltsNoFan = oFcuCommissioningReport.BeltsNoFan;
                oRptFcuCommissioning.BeltsSizeFan = oFcuCommissioningReport.BeltsSizeFan;
                oRptFcuCommissioning.ShaftCentreFan = oFcuCommissioningReport.ShaftCentreFan;
                oRptFcuCommissioning.OverloadMakeMS = oFcuCommissioningReport.OverloadMakeMS;
                oRptFcuCommissioning.OverloadRangeMS = oFcuCommissioningReport.OverloadRangeMS;
                oRptFcuCommissioning.VSDMakeMS = oFcuCommissioningReport.VSDMakeMS;
                oRptFcuCommissioning.VSDRangeMS = oFcuCommissioningReport.VSDRangeMS;
                oRptFcuCommissioning.ValveACT = oFcuCommissioningReport.ValveACT;
                oRptFcuCommissioning.ModelACT = oFcuCommissioningReport.ModelACT;
                oRptFcuCommissioning.SizeACT = oFcuCommissioningReport.SizeACT;
                oRptFcuCommissioning.CoilACT = oFcuCommissioningReport.CoilACT;
                oRptFcuCommissioning.ValveBAL = oFcuCommissioningReport.ValveBAL;
                oRptFcuCommissioning.ModelBAL = oFcuCommissioningReport.ModelBAL;
                oRptFcuCommissioning.SizeBAL = oFcuCommissioningReport.SizeBAL;
                oRptFcuCommissioning.FilterType1 = oFcuCommissioningReport.FilterType1;
                oRptFcuCommissioning.FilterSize1 = oFcuCommissioningReport.FilterSize1;
                oRptFcuCommissioning.NoFilters1 = oFcuCommissioningReport.NoFilters1;
                oRptFcuCommissioning.FilterType2 = oFcuCommissioningReport.FilterType2;
                oRptFcuCommissioning.FilterSize2 = oFcuCommissioningReport.FilterSize2;
                oRptFcuCommissioning.NoFilters2 = oFcuCommissioningReport.NoFilters2;
                oRptFcuCommissioning.SupplyAirDES = oFcuCommissioningReport.SupplyAirDES;
                oRptFcuCommissioning.ReturnAirDES = oFcuCommissioningReport.ReturnAirDES;
                oRptFcuCommissioning.OutsideAirDES = oFcuCommissioningReport.OutsideAirDES;
                oRptFcuCommissioning.PchwDES = oFcuCommissioningReport.PchwDES;
                oRptFcuCommissioning.SchwDES = oFcuCommissioningReport.SchwDES;
                oRptFcuCommissioning.TotalStaticDES = oFcuCommissioningReport.TotalStaticDES;
                oRptFcuCommissioning.SucStaticDES = oFcuCommissioningReport.SucStaticDES;
                oRptFcuCommissioning.DisStaticDES = oFcuCommissioningReport.DisStaticDES;
                oRptFcuCommissioning.FilterDiffDES = oFcuCommissioningReport.FilterDiffDES;
                oRptFcuCommissioning.PchwDiffDES = oFcuCommissioningReport.PchwDiffDES;
                oRptFcuCommissioning.ScheDiffDES = oFcuCommissioningReport.ScheDiffDES;
                oRptFcuCommissioning.VoltageDES = oFcuCommissioningReport.VoltageDES;
                oRptFcuCommissioning.AmpsDES = oFcuCommissioningReport.AmpsDES;
                oRptFcuCommissioning.SupplyAirFIN = oFcuCommissioningReport.SupplyAirFIN;
                oRptFcuCommissioning.ReturnAirFIN = oFcuCommissioningReport.SupplyAirFIN;
                oRptFcuCommissioning.OutsideAirFIN = oFcuCommissioningReport.OutsideAirFIN;
                oRptFcuCommissioning.PchwFIN = oFcuCommissioningReport.PchwFIN;
                oRptFcuCommissioning.SchwFIN = oFcuCommissioningReport.SchwFIN;
                oRptFcuCommissioning.TotalStaticFIN = oFcuCommissioningReport.TotalStaticFIN;
                oRptFcuCommissioning.SucStaticFIN = oFcuCommissioningReport.SucStaticFIN;
                oRptFcuCommissioning.DisStaticFIN = oFcuCommissioningReport.DisStaticFIN;
                oRptFcuCommissioning.FilterDiffFIN = oFcuCommissioningReport.FilterDiffFIN;
                oRptFcuCommissioning.PchwDiffFIN = oFcuCommissioningReport.PchwDiffFIN;
                oRptFcuCommissioning.ScheDiffFIN = oFcuCommissioningReport.ScheDiffFIN;
                oRptFcuCommissioning.VoltageFIN = oFcuCommissioningReport.VoltageFIN;
                oRptFcuCommissioning.AmpsFIN = oFcuCommissioningReport.AmpsFIN;
                oRptFcuCommissioning.Instrument1 = oFcuCommissioningReport.Instrument1;
                oRptFcuCommissioning.Model1 = oFcuCommissioningReport.Model1;
                oRptFcuCommissioning.Serial1 = oFcuCommissioningReport.Serial1;
                oRptFcuCommissioning.Instrument2 = oFcuCommissioningReport.Instrument2;
                oRptFcuCommissioning.Model2 = oFcuCommissioningReport.Model2;
                oRptFcuCommissioning.Serial2 = oFcuCommissioningReport.Serial2;
                oRptFcuCommissioning.TechnicianComments = oFcuCommissioningReport.TechnicianComments;
                oRptFcuCommissioning.ICAComments = oFcuCommissioningReport.ICAComments;
                oRptFcuCommissioning.OverloadRangeMS2 = oFcuCommissioningReport.OverloadRangeMS2;
                oRptFcuCommissioning.OverloadRangeMS3 = oFcuCommissioningReport.OverloadRangeMS3;
                oRptFcuCommissioning.FilterSize1B = oFcuCommissioningReport.FilterSize1B;
                oRptFcuCommissioning.FilterSize1C = oFcuCommissioningReport.FilterSize1C;
                oRptFcuCommissioning.FilterSize2B = oFcuCommissioningReport.FilterSize2B;
                oRptFcuCommissioning.FilterSize2C = oFcuCommissioningReport.FilterSize2C;
                oRptFcuCommissioning.FuseDetailsMDPH = oFcuCommissioningReport.FuseDetailsMDPH;
                oRptFcuCommissioning.ShowSucStatic = oFcuCommissioningReport.ShowSucStatic;
                oRptFcuCommissioning.ShowDisStatic = oFcuCommissioningReport.ShowDisStatic;
                oRptFcuCommissioning.ShowFilterDiff = oFcuCommissioningReport.ShowFilterDiff;
                oRptFcuCommissioning.CHWStaticFin = oFcuCommissioningReport.CHWStaticFin;
                oRptFcuCommissioning.HeadBuilding = oFcuCommissioningReport.HeadBuilding;
                oRptFcuCommissioning.HeadService = oFcuCommissioningReport.HeadService;
                oRptFcuCommissioning.HeadCustomer = oFcuCommissioningReport.HeadCustomer;
                oRptFcuCommissioning.BMSFanSpeed = oFcuCommissioningReport.BMSFanSpeed;
                oRptFcuCommissioning.BMSVelocity = oFcuCommissioningReport.BMSVelocity;
                oRptFcuCommissioning.BMSPressure = oFcuCommissioningReport.BMSPressure;
                oRptFcuCommissioning.BMSOther = oFcuCommissioningReport.BMSOther;
                oRptFcuCommissioning.SupplyAirPER = oFcuCommissioningReport.SupplyAirPER;
                oRptFcuCommissioning.ReturnAirPER = oFcuCommissioningReport.ReturnAirPER;
                oRptFcuCommissioning.OutsideAirPER = oFcuCommissioningReport.OutsideAirPER;
                oRptFcuCommissioning.PchwPER = oFcuCommissioningReport.PchwPER;
                oRptFcuCommissioning.HHWPER = oFcuCommissioningReport.HHWPER;
                oRptFcuCommissioning.HHWDES = oFcuCommissioningReport.HHWDES;
                oRptFcuCommissioning.HHWFIN = oFcuCommissioningReport.HHWFIN;
                oRptFcuCommissioning.WitnessName = oFcuCommissioningReport.WitnessName;
                oRptFcuCommissioning.WitnessDate = oFcuCommissioningReport.WitnessDate;
                oRptFcuCommissioning.WitnessSignature = oFcuCommissioningReport.WitnessSignature;
                oRptFcuCommissioning.CapacityAHUHeat = oFcuCommissioningReport.CapacityAHUHeat;
                oRptFcuCommissioning.BMSFanSpeedType = oFcuCommissioningReport.BMSFanSpeedType;
                oRptFcuCommissioning.BMSVelocityType = oFcuCommissioningReport.BMSVelocityType;
                context.Rpt_FcuCommissioning.Add(oRptFcuCommissioning);
                context.SaveChanges();

                //Save report comments information.
                List<CommCommentViewModel> lstComments = new List<CommCommentViewModel>();
                lstComments = oFcuCommissioningReport.Comments;
                foreach (var item in lstComments)
                {
                    tblSystemReportComment oFcuCommissioningComment = new tblSystemReportComment();
                    oFcuCommissioningComment.ID = Guid.NewGuid();
                    oFcuCommissioningComment.SysRepID_fk = SysReportId;
                    oFcuCommissioningComment.TechnicianID_fk = item.TechnicianID_fk;
                    oFcuCommissioningComment.DateCreated = item.DateCreated;
                    oFcuCommissioningComment.Comments = item.Comments;
                    context.tblSystemReportComments.Add(oFcuCommissioningComment);
                    context.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
        }

        /// <summary>
        /// Get Fcu Commissioning Direct Drive report details for given system report id.
        /// </summary>
        /// <returns>
        /// The oFcuDirectDriveCommissioningReport object which contains all the information.
        /// </returns>
        [Authorize]
        [Route("reports/GetFcuDirectDriveCommissioningReportBySystemReportID")]
        [HttpPost]
        public HttpResponseMessage GetFcuDirectDriveCommissioningReportBySystemReportID([FromBody] string systemreportid)
        {
            try
            {
                //Get report details information by providing systemreportid
                var val = Guid.Parse(systemreportid);
                var oFcuDirectDriveCommissioningReport = context.Rpt_FcuCommissioningDirectDrive.Where(x => x.SystemReportID_fk == val).Select(x => new FcuDirectDriveCommissioningReportViewModel
                {
                    ReportID = x.ReportID,
                    SystemReportID_fk = x.SystemReportID_fk.Value,
                    LocationAHU = x.LocationAHU,
                    ManufactureAHU = x.ManufactureAHU,
                    ModelNoAHU = x.ModelNoAHU,
                    SerialNoAHU = x.SerialNoAHU,
                    CapacityAHU = x.CapacityAHU,
                    DesignAirAHU = x.DesignAirAHU,
                    DesignAirAHUPa = x.DesignAirAHUPa,
                    MediaAHU = x.MediaAHU,
                    FanTypeAHU = x.FanTypeAHU,
                    FanArrangementAHU = x.FanArrangementAHU,
                    MSSBNameAHU = x.MSSBNameAHU,
                    MSSBLocationAHU = x.MSSBLocationAHU,
                    MotorMakeMD = x.MotorMakeMD,
                    FrameMD = x.FrameMD,
                    ElectricalMD = x.ElectricalMD,
                    MotorPowerMD = x.MotorPowerMD,
                    FullLoadMD = x.FullLoadMD,
                    PoleMD = x.PoleMD,
                    PoleRPMMD = x.PoleRPMMD,
                    OverloadMakeMS = x.OverloadMakeMS,
                    OverloadRangeMS = x.OverloadRangeMS,
                    VSDMakeMS = x.VSDMakeMS,
                    VSDRangeMS = x.VSDRangeMS,
                    FilterType1 = x.FilterType1,
                    FilterSize1 = x.FilterSize1,
                    NoFilters1 = x.NoFilters1,
                    FilterType2 = x.FilterType2,
                    FilterSize2 = x.FilterSize2,
                    NoFilters2 = x.NoFilters2,
                    SupplyAirDES = x.SupplyAirDES,
                    ReturnAirDES = x.ReturnAirDES,
                    OutsideAirDES = x.OutsideAirDES,
                    VoltageDES = x.VoltageDES,
                    AmpsDES = x.AmpsDES,
                    SupplyAirFIN = x.SupplyAirFIN,
                    ReturnAirFIN = x.SupplyAirFIN,
                    OutsideAirFIN = x.OutsideAirFIN,
                    TotalStaticFIN = x.TotalStaticFIN,
                    SucStaticFIN = x.SucStaticFIN,
                    DisStaticFIN = x.DisStaticFIN,
                    FilterDiffFIN = x.FilterDiffFIN,
                    VoltageFIN = x.VoltageFIN,
                    AmpsFIN = x.AmpsFIN,
                    Instrument1 = x.Instrument1,
                    Model1 = x.Model1,
                    Serial1 = x.Serial1,
                    Instrument2 = x.Instrument2,
                    Model2 = x.Model2,
                    Serial2 = x.Serial2,
                    OverloadRangeMS2 = x.OverloadRangeMS2,
                    OverloadRangeMS3 = x.OverloadRangeMS3,
                    FilterSize1B = x.FilterSize1B,
                    FilterSize1C = x.FilterSize1C,
                    FilterSize2B = x.FilterSize2B,
                    FilterSize2C = x.FilterSize2C,
                    FuseDetailsMDPH = x.FuseDetailsMDPH,
                    CHWStaticFin = x.CHWStaticFin,
                    HeadBuilding = x.HeadBuilding,
                    BMSFanSpeed = x.BMSFanSpeed,
                    BMSVelocity = x.BMSVelocity,
                    BMSPressure = x.BMSPressure,
                    BMSOther = x.BMSOther,
                    SupplyAirPER = x.SupplyAirPER,
                    ReturnAirPER = x.ReturnAirPER,
                    OutsideAirPER = x.OutsideAirPER,
                    WitnessName = x.WitnessName,
                    WitnessDate = x.WitnessDate,
                    WitnessSignature = x.WitnessSignature,
                    BMSFanSpeedType = x.BMSFanSpeedType,
                    BMSVelocityType = x.BMSVelocityType,
                }
                ).FirstOrDefault();

                //Get report header information and add them to the oFcuDirectDriveCommissioningReport object.
                var oReportHeader = context.tblSystemReports.Where(x => x.SystemReportID == val).Select(x => new CommSystemReportViewModel
                {
                    SystemReportID = x.SystemReportID,
                    SystemID = x.SystemID_fk.Value,
                    TemplateID = x.TemplateID_fk.Value,
                    Name = x.Name,
                    TestReference = x.TestReference,
                    Status = x.Status.Value,
                    Date = x.DateTime.Value,
                }
                ).FirstOrDefault();
                oFcuDirectDriveCommissioningReport.Header = oReportHeader;

                //Get report comments information and add them to the oFcuDirectDriveCommissioningReport object.
                var oReportComments = context.tblSystemReportComments.Where(x => x.SysRepID_fk == val).Select(x => new CommCommentViewModel
                {
                    ID = x.ID,
                    SysRepID_fk = x.SysRepID_fk,
                    TechnicianID_fk = x.TechnicianID_fk,
                    DateCreated = x.DateCreated,
                    Comments = x.Comments,
                }
                ).ToList();
                oFcuDirectDriveCommissioningReport.Comments = oReportComments;

                return Request.CreateResponse(HttpStatusCode.OK, oFcuDirectDriveCommissioningReport);
            }
            catch(Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
            
        }

        /// <summary>
        /// Save Fcu Commissioning Direct Drive report data into the relavant tables. 
        /// It should provide systemid and templateid along with the report related data.
        /// </summary>
        /// <returns>
        /// The Satatus based on the success or failed.
        /// </returns>
        [Authorize]
        [Route("reports/SaveFcuDirectDriveCommissioningReport")]
        [HttpPost]
        public HttpResponseMessage SaveFcuDirectDriveCommissioningReport([FromBody] FcuDirectDriveCommissioningReportViewModel oFcuDirectDriveCommissioningReport)
        {
            try
            {
                //Save report header data and create systemreportid.
                tblSystemReport oSystemReport = new tblSystemReport();
                oSystemReport.SystemReportID = Guid.NewGuid();
                oSystemReport.SystemID_fk = oFcuDirectDriveCommissioningReport.Header.SystemID;
                oSystemReport.TemplateID_fk = oFcuDirectDriveCommissioningReport.Header.TemplateID;
                oSystemReport.Name = oFcuDirectDriveCommissioningReport.Header.Name;
                oSystemReport.TestReference = oFcuDirectDriveCommissioningReport.Header.TestReference;
                oSystemReport.Status = oFcuDirectDriveCommissioningReport.Header.Status;
                oSystemReport.DateTime = oFcuDirectDriveCommissioningReport.Header.Date;
                context.tblSystemReports.Add(oSystemReport);
                context.SaveChanges();
                var SysReportId = oSystemReport.SystemReportID;

                //Save report detail data along with the created systemreportid.
                Rpt_FcuCommissioningDirectDrive oRptFcuDirectDriveCommissioning = new Rpt_FcuCommissioningDirectDrive();
                oRptFcuDirectDriveCommissioning.ReportID = Guid.NewGuid();
                oRptFcuDirectDriveCommissioning.SystemReportID_fk = SysReportId;
                oRptFcuDirectDriveCommissioning.LocationAHU = oFcuDirectDriveCommissioningReport.LocationAHU;
                oRptFcuDirectDriveCommissioning.ManufactureAHU = oFcuDirectDriveCommissioningReport.ManufactureAHU;
                oRptFcuDirectDriveCommissioning.ModelNoAHU = oFcuDirectDriveCommissioningReport.ModelNoAHU;
                oRptFcuDirectDriveCommissioning.SerialNoAHU = oFcuDirectDriveCommissioningReport.SerialNoAHU;
                oRptFcuDirectDriveCommissioning.CapacityAHU = oFcuDirectDriveCommissioningReport.CapacityAHU;
                oRptFcuDirectDriveCommissioning.DesignAirAHU = oFcuDirectDriveCommissioningReport.DesignAirAHU;
                oRptFcuDirectDriveCommissioning.DesignAirAHUPa = oFcuDirectDriveCommissioningReport.DesignAirAHUPa;
                oRptFcuDirectDriveCommissioning.MediaAHU = oFcuDirectDriveCommissioningReport.MediaAHU;
                oRptFcuDirectDriveCommissioning.FanTypeAHU = oFcuDirectDriveCommissioningReport.FanTypeAHU;
                oRptFcuDirectDriveCommissioning.FanArrangementAHU = oFcuDirectDriveCommissioningReport.FanArrangementAHU;
                oRptFcuDirectDriveCommissioning.MSSBNameAHU = oFcuDirectDriveCommissioningReport.MSSBNameAHU;
                oRptFcuDirectDriveCommissioning.MSSBLocationAHU = oFcuDirectDriveCommissioningReport.MSSBLocationAHU;
                oRptFcuDirectDriveCommissioning.MotorMakeMD = oFcuDirectDriveCommissioningReport.MotorMakeMD;
                oRptFcuDirectDriveCommissioning.FrameMD = oFcuDirectDriveCommissioningReport.FrameMD;
                oRptFcuDirectDriveCommissioning.ElectricalMD = oFcuDirectDriveCommissioningReport.ElectricalMD;
                oRptFcuDirectDriveCommissioning.MotorPowerMD = oFcuDirectDriveCommissioningReport.MotorPowerMD;
                oRptFcuDirectDriveCommissioning.FullLoadMD = oFcuDirectDriveCommissioningReport.FullLoadMD;
                oRptFcuDirectDriveCommissioning.PoleMD = oFcuDirectDriveCommissioningReport.PoleMD;
                oRptFcuDirectDriveCommissioning.PoleRPMMD = oFcuDirectDriveCommissioningReport.PoleRPMMD;
                oRptFcuDirectDriveCommissioning.OverloadMakeMS = oFcuDirectDriveCommissioningReport.OverloadMakeMS;
                oRptFcuDirectDriveCommissioning.OverloadRangeMS = oFcuDirectDriveCommissioningReport.OverloadRangeMS;
                oRptFcuDirectDriveCommissioning.VSDMakeMS = oFcuDirectDriveCommissioningReport.VSDMakeMS;
                oRptFcuDirectDriveCommissioning.VSDRangeMS = oFcuDirectDriveCommissioningReport.VSDRangeMS;
                oRptFcuDirectDriveCommissioning.FilterType1 = oFcuDirectDriveCommissioningReport.FilterType1;
                oRptFcuDirectDriveCommissioning.FilterSize1 = oFcuDirectDriveCommissioningReport.FilterSize1;
                oRptFcuDirectDriveCommissioning.NoFilters1 = oFcuDirectDriveCommissioningReport.NoFilters1;
                oRptFcuDirectDriveCommissioning.FilterType2 = oFcuDirectDriveCommissioningReport.FilterType2;
                oRptFcuDirectDriveCommissioning.FilterSize2 = oFcuDirectDriveCommissioningReport.FilterSize2;
                oRptFcuDirectDriveCommissioning.NoFilters2 = oFcuDirectDriveCommissioningReport.NoFilters2;
                oRptFcuDirectDriveCommissioning.SupplyAirDES = oFcuDirectDriveCommissioningReport.SupplyAirDES;
                oRptFcuDirectDriveCommissioning.ReturnAirDES = oFcuDirectDriveCommissioningReport.ReturnAirDES;
                oRptFcuDirectDriveCommissioning.OutsideAirDES = oFcuDirectDriveCommissioningReport.OutsideAirDES;
                oRptFcuDirectDriveCommissioning.VoltageDES = oFcuDirectDriveCommissioningReport.VoltageDES;
                oRptFcuDirectDriveCommissioning.AmpsDES = oFcuDirectDriveCommissioningReport.AmpsDES;
                oRptFcuDirectDriveCommissioning.SupplyAirFIN = oFcuDirectDriveCommissioningReport.SupplyAirFIN;
                oRptFcuDirectDriveCommissioning.ReturnAirFIN = oFcuDirectDriveCommissioningReport.SupplyAirFIN;
                oRptFcuDirectDriveCommissioning.OutsideAirFIN = oFcuDirectDriveCommissioningReport.OutsideAirFIN;
                oRptFcuDirectDriveCommissioning.TotalStaticFIN = oFcuDirectDriveCommissioningReport.TotalStaticFIN;
                oRptFcuDirectDriveCommissioning.SucStaticFIN = oFcuDirectDriveCommissioningReport.SucStaticFIN;
                oRptFcuDirectDriveCommissioning.DisStaticFIN = oFcuDirectDriveCommissioningReport.DisStaticFIN;
                oRptFcuDirectDriveCommissioning.FilterDiffFIN = oFcuDirectDriveCommissioningReport.FilterDiffFIN;
                oRptFcuDirectDriveCommissioning.VoltageFIN = oFcuDirectDriveCommissioningReport.VoltageFIN;
                oRptFcuDirectDriveCommissioning.AmpsFIN = oFcuDirectDriveCommissioningReport.AmpsFIN;
                oRptFcuDirectDriveCommissioning.Instrument1 = oFcuDirectDriveCommissioningReport.Instrument1;
                oRptFcuDirectDriveCommissioning.Model1 = oFcuDirectDriveCommissioningReport.Model1;
                oRptFcuDirectDriveCommissioning.Serial1 = oFcuDirectDriveCommissioningReport.Serial1;
                oRptFcuDirectDriveCommissioning.Instrument2 = oFcuDirectDriveCommissioningReport.Instrument2;
                oRptFcuDirectDriveCommissioning.Model2 = oFcuDirectDriveCommissioningReport.Model2;
                oRptFcuDirectDriveCommissioning.Serial2 = oFcuDirectDriveCommissioningReport.Serial2;
                oRptFcuDirectDriveCommissioning.OverloadRangeMS2 = oFcuDirectDriveCommissioningReport.OverloadRangeMS2;
                oRptFcuDirectDriveCommissioning.OverloadRangeMS3 = oFcuDirectDriveCommissioningReport.OverloadRangeMS3;
                oRptFcuDirectDriveCommissioning.FilterSize1B = oFcuDirectDriveCommissioningReport.FilterSize1B;
                oRptFcuDirectDriveCommissioning.FilterSize1C = oFcuDirectDriveCommissioningReport.FilterSize1C;
                oRptFcuDirectDriveCommissioning.FilterSize2B = oFcuDirectDriveCommissioningReport.FilterSize2B;
                oRptFcuDirectDriveCommissioning.FilterSize2C = oFcuDirectDriveCommissioningReport.FilterSize2C;
                oRptFcuDirectDriveCommissioning.FuseDetailsMDPH = oFcuDirectDriveCommissioningReport.FuseDetailsMDPH;
                oRptFcuDirectDriveCommissioning.CHWStaticFin = oFcuDirectDriveCommissioningReport.CHWStaticFin;
                oRptFcuDirectDriveCommissioning.HeadBuilding = oFcuDirectDriveCommissioningReport.HeadBuilding;
                oRptFcuDirectDriveCommissioning.BMSFanSpeed = oFcuDirectDriveCommissioningReport.BMSFanSpeed;
                oRptFcuDirectDriveCommissioning.BMSVelocity = oFcuDirectDriveCommissioningReport.BMSVelocity;
                oRptFcuDirectDriveCommissioning.BMSPressure = oFcuDirectDriveCommissioningReport.BMSPressure;
                oRptFcuDirectDriveCommissioning.BMSOther = oFcuDirectDriveCommissioningReport.BMSOther;
                oRptFcuDirectDriveCommissioning.SupplyAirPER = oFcuDirectDriveCommissioningReport.SupplyAirPER;
                oRptFcuDirectDriveCommissioning.ReturnAirPER = oFcuDirectDriveCommissioningReport.ReturnAirPER;
                oRptFcuDirectDriveCommissioning.OutsideAirPER = oFcuDirectDriveCommissioningReport.OutsideAirPER;
                oRptFcuDirectDriveCommissioning.WitnessName = oFcuDirectDriveCommissioningReport.WitnessName;
                oRptFcuDirectDriveCommissioning.WitnessDate = oFcuDirectDriveCommissioningReport.WitnessDate;
                oRptFcuDirectDriveCommissioning.WitnessSignature = oFcuDirectDriveCommissioningReport.WitnessSignature;
                oRptFcuDirectDriveCommissioning.BMSFanSpeedType = oFcuDirectDriveCommissioningReport.BMSFanSpeedType;
                oRptFcuDirectDriveCommissioning.BMSVelocityType = oFcuDirectDriveCommissioningReport.BMSVelocityType;
                context.Rpt_FcuCommissioningDirectDrive.Add(oRptFcuDirectDriveCommissioning);
                context.SaveChanges();

                //Save report comments informationn.
                List<CommCommentViewModel> lstComments = new List<CommCommentViewModel>();
                lstComments = oFcuDirectDriveCommissioningReport.Comments;
                foreach (var item in lstComments)
                {
                    tblSystemReportComment oFcuCommissioningComment = new tblSystemReportComment();
                    oFcuCommissioningComment.ID = Guid.NewGuid();
                    oFcuCommissioningComment.SysRepID_fk = SysReportId;
                    oFcuCommissioningComment.TechnicianID_fk = item.TechnicianID_fk;
                    oFcuCommissioningComment.DateCreated = item.DateCreated;
                    oFcuCommissioningComment.Comments = item.Comments;
                    context.tblSystemReportComments.Add(oFcuCommissioningComment);
                    context.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
        }


        /// <summary>
        /// Get Vav Commissioning report details for given system report id. 
        /// </summary>
        /// <returns>
        /// The oVavCommissioningReport object which contains all the information.
        /// </returns>
        [Authorize]
        [Route("reports/GetVavCommissioningReportBySystemReportID")]
        [HttpPost]
        public HttpResponseMessage GetVavCommissioningReportBySystemReportID([FromBody] string systemreportid)
        {
            try
            {
                //Get main report data by providing systemreportid.
                var val = Guid.Parse(systemreportid);
                var oVavCommissioningReport = context.Rpt_VavCommissioning.Where(x => x.SystemReportID_fk == val).Select(x => new VavCommissioningReportViewModel
                {
                    ReportID = x.ReportID,
                    SystemReportID_fk = x.SystemReportID_fk,
                    NoVavs = x.NoVavs,
                    SetUpComplete = x.SetUpComplete,
                    HeadBuilding = x.HeadBuilding,
                    AHUDesignTotal = x.AHUDesignTotal,
                }
                ).FirstOrDefault();

                //Get report header information and assign to oVavCommissioningReport object.
                var oReportHeader = context.tblSystemReports.Where(x => x.SystemReportID == val).Select(x => new CommSystemReportViewModel
                {
                    SystemReportID = x.SystemReportID,
                    SystemID = x.SystemID_fk.Value,
                    TemplateID = x.TemplateID_fk.Value,
                    Name = x.Name,
                    TestReference = x.TestReference,
                    Status = x.Status.Value,
                    Date = x.DateTime.Value,
                }
                ).FirstOrDefault();
                oVavCommissioningReport.Header = oReportHeader;

                //Get report data by providing report id and assign to oVavCommissioningReport object.
                var oReportData = context.Rpt_VavCommissioningVavs.Where(x => x.ReportID_fk == oVavCommissioningReport.ReportID).Select(x => new VavCommissioningDataViewModel
                {
                    ID = x.ID,
                    ReportID_fk = x.ReportID_fk,
                    VavReference = x.VavReference,
                    NoGrilles = x.NoGrilles,
                    VavManufacturer = x.VavManufacturer,
                    VavType = x.VavType,
                    ReheatType = x.ReheatType,
                    GrilleType = x.GrilleType,
                    GrilleSizeX = x.GrilleSizeX,
                    GrilleSizeY = x.GrilleSizeY,
                    GrilleFactor = x.GrilleFactor,
                    TestApp = x.TestApp,
                    VMin = x.VMin,
                    VMax = x.VMax,
                    ResultsDesVMin = x.ResultsDesVMin,
                    ResultsDesVMax = x.ResultsDesVMax,
                    ResultsFinVMin = x.ResultsFinVMin,
                    ResultsFinVMax = x.ResultsFinVMax,
                    BmsKFactor = x.BmsKFactor,
                    BmsVMin = x.BmsVMin,
                    BmsVMax = x.BmsVMax,
                    BMSLsTotal = x.BMSLsTotal,
                    BMSDamperPosition = x.BMSDamperPosition,
                    InletSize = x.InletSize,
                }).FirstOrDefault();
                oVavCommissioningReport.ReportData = oReportData;

                //Get report comments data and assign to oVavCommissioningReport object.
                var oReportComments = context.tblSystemReportComments.Where(x => x.SysRepID_fk == val).Select(x => new CommCommentViewModel
                {
                    ID = x.ID,
                    SysRepID_fk = x.SysRepID_fk,
                    TechnicianID_fk = x.TechnicianID_fk,
                    DateCreated = x.DateCreated,
                    Comments = x.Comments,
                }
                ).ToList();
                oVavCommissioningReport.Comments = oReportComments;

                return Request.CreateResponse(HttpStatusCode.OK, oVavCommissioningReport);
            }
            catch(Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
            
        }


        /// <summary>
        /// Save Vav Commissioning report data into the relavant tables. 
        /// It should provide systemid and templateid along with the report related data.
        /// </summary>
        /// <returns>
        /// The Satatus based on the success or failed.
        /// </returns>
        [Authorize]
        [Route("reports/SaveVavCommissioningReport")]
        [HttpPost]
        public HttpResponseMessage SaveVavCommissioningReport([FromBody] VavCommissioningReportViewModel oVavCommissioningReport)
        {
            try
            {
                //Save report header data and create systemreportid.
                tblSystemReport oSystemReport = new tblSystemReport();
                oSystemReport.SystemReportID = Guid.NewGuid();
                oSystemReport.SystemID_fk = oVavCommissioningReport.Header.SystemID;
                oSystemReport.TemplateID_fk = oVavCommissioningReport.Header.TemplateID;
                oSystemReport.Name = oVavCommissioningReport.Header.Name;
                oSystemReport.TestReference = oVavCommissioningReport.Header.TestReference;
                oSystemReport.Status = oVavCommissioningReport.Header.Status;
                oSystemReport.DateTime = oVavCommissioningReport.Header.Date;
                context.tblSystemReports.Add(oSystemReport);
                context.SaveChanges();
                var SysReportId = oSystemReport.SystemReportID;

                //Save report data along with the created systemreportid and create a reportid
                Rpt_VavCommissioning oRptVavCommissioning = new Rpt_VavCommissioning();
                oRptVavCommissioning.ReportID = Guid.NewGuid();
                oRptVavCommissioning.SystemReportID_fk = SysReportId;
                oRptVavCommissioning.NoVavs = oVavCommissioningReport.NoVavs;
                oRptVavCommissioning.SetUpComplete = oVavCommissioningReport.SetUpComplete;
                oRptVavCommissioning.HeadBuilding = oVavCommissioningReport.HeadBuilding;
                oRptVavCommissioning.AHUDesignTotal = oVavCommissioningReport.AHUDesignTotal;
                context.Rpt_VavCommissioning.Add(oRptVavCommissioning);
                context.SaveChanges();
                var RepId = oRptVavCommissioning.ReportID;

                //Save report detail data along with the created reportid.
                Rpt_VavCommissioningVavs oData = new Rpt_VavCommissioningVavs();
                oData.ID = Guid.NewGuid();
                oData.ReportID_fk = RepId;
                oData.VavReference = oVavCommissioningReport.ReportData.VavReference;
                oData.NoGrilles = oVavCommissioningReport.ReportData.NoGrilles;
                oData.VavManufacturer = oVavCommissioningReport.ReportData.VavManufacturer;
                oData.VavType = oVavCommissioningReport.ReportData.VavType;
                oData.ReheatType = oVavCommissioningReport.ReportData.ReheatType;
                oData.GrilleType = oVavCommissioningReport.ReportData.GrilleType;
                oData.GrilleSizeX = oVavCommissioningReport.ReportData.GrilleSizeX;
                oData.GrilleSizeY = oVavCommissioningReport.ReportData.GrilleSizeY;
                oData.GrilleFactor = oVavCommissioningReport.ReportData.GrilleFactor;
                oData.TestApp = oVavCommissioningReport.ReportData.TestApp;
                oData.VMin = oVavCommissioningReport.ReportData.VMin;
                oData.VMax = oVavCommissioningReport.ReportData.VMax;
                oData.ResultsDesVMin = oVavCommissioningReport.ReportData.ResultsDesVMin;
                oData.ResultsDesVMax = oVavCommissioningReport.ReportData.ResultsDesVMax;
                oData.ResultsFinVMin = oVavCommissioningReport.ReportData.ResultsFinVMin;
                oData.ResultsFinVMax = oVavCommissioningReport.ReportData.ResultsFinVMax;
                oData.BmsKFactor = oVavCommissioningReport.ReportData.BmsKFactor;
                oData.BmsVMin = oVavCommissioningReport.ReportData.BmsVMin;
                oData.BmsVMax = oVavCommissioningReport.ReportData.BmsVMax;
                oData.BMSLsTotal = oVavCommissioningReport.ReportData.BMSLsTotal;
                oData.BMSDamperPosition = oVavCommissioningReport.ReportData.BMSDamperPosition;
                oData.InletSize = oVavCommissioningReport.ReportData.InletSize;
                context.Rpt_VavCommissioningVavs.Add(oData);
                context.SaveChanges();

                //Save report comments information.
                List<CommCommentViewModel> lstComments = new List<CommCommentViewModel>();
                lstComments = oVavCommissioningReport.Comments;
                foreach (var item in lstComments)
                {
                    tblSystemReportComment oVavCommissioningComment = new tblSystemReportComment();
                    oVavCommissioningComment.ID = Guid.NewGuid();
                    oVavCommissioningComment.SysRepID_fk = SysReportId;
                    oVavCommissioningComment.TechnicianID_fk = item.TechnicianID_fk;
                    oVavCommissioningComment.DateCreated = item.DateCreated;
                    oVavCommissioningComment.Comments = item.Comments;
                    context.tblSystemReportComments.Add(oVavCommissioningComment);
                    context.SaveChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
        }


        /// <summary>
        /// Get Pump Commissioning report details for given system report id. 
        /// </summary>
        /// <returns>
        /// The oPumpCommissioningReport object which contains all the information.
        /// </returns>
        [Authorize]
        [Route("reports/GetPumpCommissioningReportBySystemReportID")]
        [HttpPost]
        public HttpResponseMessage GetPumpCommissioningReportBySystemReportID([FromBody] string systemreportid)
        {
            try
            {
                //Get report data by providing systemreportid.
                var val = Guid.Parse(systemreportid);
                var oPumpCommissioningReport = context.Rpt_PumpCommissioning.Where(x => x.SystemReportID_fk == val).Select(x => new PumpCommissioningReportViewModel
                {
                    ReportID = x.ReportID,
                    SystemReportID_fk = x.SystemReportID_fk,
                    LocationPump = x.LocationPump,
                    WaterTypePump = x.WaterTypePump,
                    SBoardRef = x.SBoardRef,
                    SBoardLoc = x.SBoardLoc,
                    ManufacturePump = x.ManufacturePump,
                    ModelNoPump = x.ModelNoPump,
                    SerialNoPump = x.SerialNoPump,
                    PumpType = x.PumpType,
                    ImpSize = x.ImpSize,
                    PumpLs = x.PumpLs,
                    PumpKPA = x.PumpKPA,
                    CoilVesselKPA = x.CoilVesselKPA,
                    CoilVesselLs = x.CoilVesselLs,
                    MotorMakeMD = x.MotorMakeMD,
                    FrameMD = x.FrameMD,
                    ElectricalMD = x.ElectricalMD,
                    MotorPowerMD = x.MotorPowerMD,
                    FullLoadMD = x.FullLoadMD,
                    PoleMD = x.PoleMD,
                    PoleRPMMD = x.PoleRPMMD,
                    FuseDetailsMD = x.FuseDetailsMD,
                    OverloadMakeMS = x.OverloadMakeMS,
                    OverloadRangeMS = x.OverloadRangeMS,
                    VSDMakeMS = x.VSDMakeMS,
                    VSDRangeMS = x.VSDRangeMS,
                    SystemOpp = x.SystemOpp,
                    TestPoint = x.TestPoint,
                    Aligned = x.Aligned,
                    Strainer = x.Strainer,
                    Curve = x.Curve,
                    Suction1 = x.Suction1,
                    Discharge1 = x.Discharge1,
                    Delta1 = x.Delta1,
                    RPM1 = x.RPM1,
                    Amps1 = x.Amps1,
                    Hz1 = x.Hz1,
                    Suction2 = x.Suction2,
                    Discharge2 = x.Discharge2,
                    Delta2 = x.Delta2,
                    RPM2 = x.RPM2,
                    Amps2 = x.Amps2,
                    Hz2 = x.Hz2,
                    Suction3 = x.Suction3,
                    Discharge3 = x.Discharge3,
                    Delta3 = x.Delta3,
                    RPM3 = x.RPM3,
                    Amps3 = x.Amps3,
                    Hz3 = x.Hz3,
                    ClosedPressure = x.ClosedPressure,
                    OpenPressure = x.OpenPressure,
                    ClosedHz = x.ClosedHz,
                    OpenHz = x.OpenHz,
                    PumpFinalLS = x.PumpFinalLS,
                    PumpFinalKPA = x.PumpFinalKPA,
                    PumpFinalHz = x.PumpFinalHz,
                    PumpOp = x.PumpOp,
                    PumpStatic = x.PumpStatic,
                    Instrument1 = x.Instrument1,
                    Model1 = x.Model1,
                    Serial1 = x.Serial1,
                    Instrument2 = x.Instrument2,
                    Model2 = x.Model2,
                    Serial2 = x.Serial2,
                    TechnicianComments = x.TechnicianComments,
                    ICAComments = x.ICAComments,
                    FuseDetailsMDPH = x.FuseDetailsMDPH,
                    OverloadRangeMS2 = x.OverloadRangeMS2,
                    OverloadRangeMS3 = x.OverloadRangeMS3,
                    FullFlowLS = x.FullFlowLS,
                    FinalFlowLS = x.FinalFlowLS,
                    HeadBuilding = x.HeadBuilding,
                    HeadService = x.HeadService,
                    HeadCustomer = x.HeadCustomer,
                    BMSPumpSpeed = x.BMSPumpSpeed,
                    BMSVelocity = x.BMSVelocity,
                    BMSPressure = x.BMSPressure,
                    BMSOther = x.BMSOther,
                    PumpFinalFinLs = x.PumpFinalFinLs,
                    PumpFinalFinKPa = x.PumpFinalFinKPa,
                    PumpFinalFinPer = x.PumpFinalFinPer,
                    CoilLs = x.CoilLs,
                    CoilKPa = x.CoilKPa,
                    CoilFinLs = x.CoilFinLs,
                    CoilFinKPa = x.CoilFinKPa,
                    CoilFinPer = x.CoilFinPer,
                    WitnessName = x.WitnessName,
                    WitnessDate = x.WitnessDate,
                    WitnessSignature = x.WitnessSignature,
                    BMSPumpSpeedType = x.BMSPumpSpeedType,
                    BMSVelocityType = x.BMSVelocityType,
                }
                ).FirstOrDefault();

                //Get report header data and assign to the oPumpCommissioningReport object.
                var oReportHeader = context.tblSystemReports.Where(x => x.SystemReportID == val).Select(x => new CommSystemReportViewModel
                {
                    SystemReportID = x.SystemReportID,
                    SystemID = x.SystemID_fk.Value,
                    TemplateID = x.TemplateID_fk.Value,
                    Name = x.Name,
                    TestReference = x.TestReference,
                    Status = x.Status.Value,
                    Date = x.DateTime.Value,
                }
                ).FirstOrDefault();
                oPumpCommissioningReport.Header = oReportHeader;

                //Get report comments data and assign to the oPumpCommissioningReport object.
                var oReportComments = context.tblSystemReportComments.Where(x => x.SysRepID_fk == val).Select(x => new CommCommentViewModel
                {
                    ID = x.ID,
                    SysRepID_fk = x.SysRepID_fk,
                    TechnicianID_fk = x.TechnicianID_fk,
                    DateCreated = x.DateCreated,
                    Comments = x.Comments,
                }
                ).ToList();
                oPumpCommissioningReport.Comments = oReportComments;

                return Request.CreateResponse(HttpStatusCode.OK, oPumpCommissioningReport);
            }
            catch(Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }            
        }


        /// <summary>
        /// Save Pump Commissioning report data into the relavant tables. 
        /// It should provide systemid and templateid along with the report related data.
        /// </summary>
        /// <returns>
        /// The Satatus based on the success or failed.
        /// </returns>
        [Authorize]
        [Route("reports/SavePumpCommissioningReport")]
        [HttpPost]
        public HttpResponseMessage SavePumpCommissioningReport([FromBody] PumpCommissioningReportViewModel oPumpCommissioningReport)
        {
            try
            {
                //Save report header data and create systemreportid.
                tblSystemReport oSystemReport = new tblSystemReport();
                oSystemReport.SystemReportID = Guid.NewGuid();
                oSystemReport.SystemID_fk = oPumpCommissioningReport.Header.SystemID;
                oSystemReport.TemplateID_fk = oPumpCommissioningReport.Header.TemplateID;
                oSystemReport.Name = oPumpCommissioningReport.Header.Name;
                oSystemReport.TestReference = oPumpCommissioningReport.Header.TestReference;
                oSystemReport.Status = oPumpCommissioningReport.Header.Status;
                oSystemReport.DateTime = oPumpCommissioningReport.Header.Date;
                context.tblSystemReports.Add(oSystemReport);
                context.SaveChanges();
                var SysReportId = oSystemReport.SystemReportID;

                //Save report data along with the systemreportid and create reportid.
                Rpt_PumpCommissioning oRptPumpCommissioning = new Rpt_PumpCommissioning();
                oRptPumpCommissioning.ReportID = Guid.NewGuid();
                oRptPumpCommissioning.SystemReportID_fk = SysReportId;
                oRptPumpCommissioning.LocationPump = oPumpCommissioningReport.LocationPump;
                oRptPumpCommissioning.WaterTypePump = oPumpCommissioningReport.WaterTypePump;
                oRptPumpCommissioning.SBoardRef = oPumpCommissioningReport.SBoardRef;
                oRptPumpCommissioning.SBoardLoc = oPumpCommissioningReport.SBoardLoc;
                oRptPumpCommissioning.ManufacturePump = oPumpCommissioningReport.ManufacturePump;
                oRptPumpCommissioning.ModelNoPump = oPumpCommissioningReport.ModelNoPump;
                oRptPumpCommissioning.SerialNoPump = oPumpCommissioningReport.SerialNoPump;
                oRptPumpCommissioning.PumpType = oPumpCommissioningReport.PumpType;
                oRptPumpCommissioning.ImpSize = oPumpCommissioningReport.ImpSize;
                oRptPumpCommissioning.PumpLs = oPumpCommissioningReport.PumpLs;
                oRptPumpCommissioning.PumpKPA = oPumpCommissioningReport.PumpKPA;
                oRptPumpCommissioning.CoilVesselKPA = oPumpCommissioningReport.CoilVesselKPA;
                oRptPumpCommissioning.CoilVesselLs = oPumpCommissioningReport.CoilVesselLs;
                oRptPumpCommissioning.MotorMakeMD = oPumpCommissioningReport.MotorMakeMD;
                oRptPumpCommissioning.FrameMD = oPumpCommissioningReport.FrameMD;
                oRptPumpCommissioning.ElectricalMD = oPumpCommissioningReport.ElectricalMD;
                oRptPumpCommissioning.MotorPowerMD = oPumpCommissioningReport.MotorPowerMD;
                oRptPumpCommissioning.FullLoadMD = oPumpCommissioningReport.FullLoadMD;
                oRptPumpCommissioning.PoleMD = oPumpCommissioningReport.PoleMD;
                oRptPumpCommissioning.PoleRPMMD = oPumpCommissioningReport.PoleRPMMD;
                oRptPumpCommissioning.FuseDetailsMD = oPumpCommissioningReport.FuseDetailsMD;
                oRptPumpCommissioning.OverloadMakeMS = oPumpCommissioningReport.OverloadMakeMS;
                oRptPumpCommissioning.OverloadRangeMS = oPumpCommissioningReport.OverloadRangeMS;
                oRptPumpCommissioning.VSDMakeMS = oPumpCommissioningReport.VSDMakeMS;
                oRptPumpCommissioning.VSDRangeMS = oPumpCommissioningReport.VSDRangeMS;
                oRptPumpCommissioning.SystemOpp = oPumpCommissioningReport.SystemOpp;
                oRptPumpCommissioning.TestPoint = oPumpCommissioningReport.TestPoint;
                oRptPumpCommissioning.Aligned = oPumpCommissioningReport.Aligned;
                oRptPumpCommissioning.Strainer = oPumpCommissioningReport.Strainer;
                oRptPumpCommissioning.Curve = oPumpCommissioningReport.Curve;
                oRptPumpCommissioning.Suction1 = oPumpCommissioningReport.Suction1;
                oRptPumpCommissioning.Discharge1 = oPumpCommissioningReport.Discharge1;
                oRptPumpCommissioning.Delta1 = oPumpCommissioningReport.Delta1;
                oRptPumpCommissioning.RPM1 = oPumpCommissioningReport.RPM1;
                oRptPumpCommissioning.Amps1 = oPumpCommissioningReport.Amps1;
                oRptPumpCommissioning.Hz1 = oPumpCommissioningReport.Hz1;
                oRptPumpCommissioning.Suction2 = oPumpCommissioningReport.Suction2;
                oRptPumpCommissioning.Discharge2 = oPumpCommissioningReport.Discharge2;
                oRptPumpCommissioning.Delta2 = oPumpCommissioningReport.Delta2;
                oRptPumpCommissioning.RPM2 = oPumpCommissioningReport.RPM2;
                oRptPumpCommissioning.Amps2 = oPumpCommissioningReport.Amps2;
                oRptPumpCommissioning.Hz2 = oPumpCommissioningReport.Hz2;
                oRptPumpCommissioning.Suction3 = oPumpCommissioningReport.Suction3;
                oRptPumpCommissioning.Discharge3 = oPumpCommissioningReport.Discharge3;
                oRptPumpCommissioning.Delta3 = oPumpCommissioningReport.Delta3;
                oRptPumpCommissioning.RPM3 = oPumpCommissioningReport.RPM3;
                oRptPumpCommissioning.Amps3 = oPumpCommissioningReport.Amps3;
                oRptPumpCommissioning.Hz3 = oPumpCommissioningReport.Hz3;
                oRptPumpCommissioning.ClosedPressure = oPumpCommissioningReport.ClosedPressure;
                oRptPumpCommissioning.OpenPressure = oPumpCommissioningReport.OpenPressure;
                oRptPumpCommissioning.ClosedHz = oPumpCommissioningReport.ClosedHz;
                oRptPumpCommissioning.OpenHz = oPumpCommissioningReport.OpenHz;
                oRptPumpCommissioning.PumpFinalLS = oPumpCommissioningReport.PumpFinalLS;
                oRptPumpCommissioning.PumpFinalKPA = oPumpCommissioningReport.PumpFinalKPA;
                oRptPumpCommissioning.PumpFinalHz = oPumpCommissioningReport.PumpFinalHz;
                oRptPumpCommissioning.PumpOp = oPumpCommissioningReport.PumpOp;
                oRptPumpCommissioning.PumpStatic = oPumpCommissioningReport.PumpStatic;
                oRptPumpCommissioning.Instrument1 = oPumpCommissioningReport.Instrument1;
                oRptPumpCommissioning.Model1 = oPumpCommissioningReport.Model1;
                oRptPumpCommissioning.Serial1 = oPumpCommissioningReport.Serial1;
                oRptPumpCommissioning.Instrument2 = oPumpCommissioningReport.Instrument2;
                oRptPumpCommissioning.Model2 = oPumpCommissioningReport.Model2;
                oRptPumpCommissioning.Serial2 = oPumpCommissioningReport.Serial2;
                oRptPumpCommissioning.TechnicianComments = oPumpCommissioningReport.TechnicianComments;
                oRptPumpCommissioning.ICAComments = oPumpCommissioningReport.ICAComments;
                oRptPumpCommissioning.FuseDetailsMDPH = oPumpCommissioningReport.FuseDetailsMDPH;
                oRptPumpCommissioning.OverloadRangeMS2 = oPumpCommissioningReport.OverloadRangeMS2;
                oRptPumpCommissioning.OverloadRangeMS3 = oPumpCommissioningReport.OverloadRangeMS3;
                oRptPumpCommissioning.FullFlowLS = oPumpCommissioningReport.FullFlowLS;
                oRptPumpCommissioning.FinalFlowLS = oPumpCommissioningReport.FinalFlowLS;
                oRptPumpCommissioning.HeadBuilding = oPumpCommissioningReport.HeadBuilding;
                oRptPumpCommissioning.HeadService = oPumpCommissioningReport.HeadService;
                oRptPumpCommissioning.HeadCustomer = oPumpCommissioningReport.HeadCustomer;
                oRptPumpCommissioning.BMSPumpSpeed = oPumpCommissioningReport.BMSPumpSpeed;
                oRptPumpCommissioning.BMSVelocity = oPumpCommissioningReport.BMSVelocity;
                oRptPumpCommissioning.BMSPressure = oPumpCommissioningReport.BMSPressure;
                oRptPumpCommissioning.BMSOther = oPumpCommissioningReport.BMSOther;
                oRptPumpCommissioning.PumpFinalFinLs = oPumpCommissioningReport.PumpFinalFinLs;
                oRptPumpCommissioning.PumpFinalFinKPa = oPumpCommissioningReport.PumpFinalFinKPa;
                oRptPumpCommissioning.PumpFinalFinPer = oPumpCommissioningReport.PumpFinalFinPer;
                oRptPumpCommissioning.CoilLs = oPumpCommissioningReport.CoilLs;
                oRptPumpCommissioning.CoilKPa = oPumpCommissioningReport.CoilKPa;
                oRptPumpCommissioning.CoilFinLs = oPumpCommissioningReport.CoilFinLs;
                oRptPumpCommissioning.CoilFinKPa = oPumpCommissioningReport.CoilFinKPa;
                oRptPumpCommissioning.CoilFinPer = oPumpCommissioningReport.CoilFinPer;
                oRptPumpCommissioning.WitnessName = oPumpCommissioningReport.WitnessName;
                oRptPumpCommissioning.WitnessDate = oPumpCommissioningReport.WitnessDate;
                oRptPumpCommissioning.WitnessSignature = oPumpCommissioningReport.WitnessSignature;
                oRptPumpCommissioning.BMSPumpSpeedType = oPumpCommissioningReport.BMSPumpSpeedType;
                oRptPumpCommissioning.BMSVelocityType = oPumpCommissioningReport.BMSVelocityType;
                context.Rpt_PumpCommissioning.Add(oRptPumpCommissioning);
                context.SaveChanges();
                var RepId = oRptPumpCommissioning.ReportID;

                //Save report commennts.
                List<CommCommentViewModel> lstComments = new List<CommCommentViewModel>();
                lstComments = oPumpCommissioningReport.Comments;
                foreach (var item in lstComments)
                {
                    tblSystemReportComment oVavCommissioningComment = new tblSystemReportComment();
                    oVavCommissioningComment.ID = Guid.NewGuid();
                    oVavCommissioningComment.SysRepID_fk = SysReportId;
                    oVavCommissioningComment.TechnicianID_fk = item.TechnicianID_fk;
                    oVavCommissioningComment.DateCreated = item.DateCreated;
                    oVavCommissioningComment.Comments = item.Comments;
                    context.tblSystemReportComments.Add(oVavCommissioningComment);
                    context.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
        }


        /// <summary>
        /// Get Split Commissioning report details for given system report id. 
        /// </summary>
        /// <returns>
        /// The oSplitCommissioningReport object which contains all the information.
        /// </returns>
        [Authorize]
        [Route("reports/GetSplitCommissioningReportBySystemReportID")]
        [HttpPost]
        public HttpResponseMessage GetSplitCommissioningReportBySystemReportID([FromBody] string systemreportid)
        {
            try
            {
                //Get report data by providing systemreportid.
                var val = Guid.Parse(systemreportid);
                var oSplitCommissioningReport = context.Rpt_SplitPackage.Where(x => x.SystemReportID_fk == val).Select(x => new SplitPackageCommissioningReportViewModel
                {
                    ReportID = x.ReportID,
                    SystemReportID_fk = x.SystemReportID_fk,
                    FanLoc = x.FanLoc,
                    Manufacturer = x.Manufacturer,
                    ModelNo = x.ModelNo,
                    SerialNo = x.SerialNo,
                    Capacity = x.Capacity,
                    FanType = x.FanType,
                    FanArrange = x.FanArrange,
                    MSSBName = x.MSSBName,
                    MSSBLoc = x.MSSBLoc,
                    TotDes = x.TotDes,
                    TotAct = x.TotAct,
                    OutDes = x.OutDes,
                    OutAct = x.OutAct,
                    ReturnDes = x.ReturnDes,
                    ReturnAct = x.ReturnAct,
                    ExtDes = x.ExtDes,
                    ExtAct = x.ExtAct,
                    SucDes = x.SucDes,
                    SucAct = x.SucAct,
                    DisDes = x.DisDes,
                    DisAct = x.DisAct,
                    MotorMake = x.MotorMake,
                    FrameClass = x.FrameClass,
                    ElectSupply = x.ElectSupply,
                    MotorPower = x.MotorPower,
                    FullLoadAmps = x.FullLoadAmps,
                    Pole = x.Pole,
                    RPM = x.RPM,
                    FuseDetails = x.FuseDetails,
                    FanDia = x.FanDia,
                    FanBush = x.FanBush,
                    FanBore = x.FanBore,
                    MotorDia = x.MotorDia,
                    MotorBush = x.MotorBush,
                    MotorBore = x.MotorBore,
                    BeltsMake = x.BeltsMake,
                    BeltsNo = x.BeltsNo,
                    BeltsSize = x.BeltsSize,
                    ShaftCentre = x.ShaftCentre,
                    FilterType = x.FilterType,
                    FilterSize = x.FilterSize,
                    NoOfFilters = x.NoOfFilters,
                    Location = x.Location,
                    CondManu = x.CondManu,
                    CondMNo = x.CondMNo,
                    CondSNo = x.CondSNo,
                    CompManu = x.CompManu,
                    CompModNo = x.CompModNo,
                    CompElec = x.CompElec,
                    CompFull = x.CompFull,
                    Ref = x.Ref,
                    Kg = x.Kg,
                    HeatingType = x.HeatingType,
                    CondFanMake = x.CondFanMake,
                    CondFanNo = x.CondFanNo,
                    CondFan = x.CondFan,
                    MotorPowerC = x.MotorPowerC,
                    FullLoadAmpsC = x.FullLoadAmpsC,
                    PoleC = x.PoleC,
                    RPMC = x.RPMC,
                    FuseDetailsC = x.FuseDetailsC,
                    SubS1 = x.SubS1,
                    SupS2 = x.SupS2,
                    SupS3 = x.SupS3,
                    SupS4 = x.SupS4,
                    CompS1 = x.CompS1,
                    CompS2 = x.CompS2,
                    CompS3 = x.CompS3,
                    CompS4 = x.CompS4,
                    CompVoltS1 = x.CompVoltS1,
                    CompVoltS2 = x.CompVoltS2,
                    CompVoltS3 = x.CompVoltS3,
                    CompVoltS4 = x.CompVoltS4,
                    AmbiS1 = x.AmbiS1,
                    AmbiS2 = x.AmbiS2,
                    AmbiS3 = x.AmbiS3,
                    AmbiS4 = x.AmbiS4,
                    SucS1 = x.SucS1,
                    SucS2 = x.SucS2,
                    SucS3 = x.SucS3,
                    SucS4 = x.SucS4,
                    DisS1 = x.DisS1,
                    DisS2 = x.DisS2,
                    DisS3 = x.DisS3,
                    DisS4 = x.DisS4,
                    OilS1 = x.OilS1,
                    OilS2 = x.OilS2,
                    OilS3 = x.OilS3,
                    OilS4 = x.OilS4,
                    CompSucS1 = x.CompSucS1,
                    CompSucS2 = x.CompSucS2,
                    CompSucS3 = x.CompSucS3,
                    CompSucS4 = x.CompSucS4,
                    LiqS1 = x.LiqS1,
                    LiqS2 = x.LiqS2,
                    LiqS3 = x.LiqS3,
                    LiqS4 = x.LiqS4,
                    SuperS1 = x.SuperS1,
                    SuperS2 = x.SuperS2,
                    SuperS3 = x.SuperS3,
                    SuperS4 = x.SuperS4,
                    SubS2 = x.SubS2,
                    SubS3 = x.SubS3,
                    SubS4 = x.SubS4,
                    EvapEntering1 = x.EvapEntering1,
                    EvapEntering2 = x.EvapEntering2,
                    EvapEntering3 = x.EvapEntering3,
                    EvapEntering4 = x.EvapEntering4,
                    EvapLeaving1 = x.EvapLeaving1,
                    EvapLeaving2 = x.EvapLeaving2,
                    EvapLeaving3 = x.EvapLeaving3,
                    EvapLeaving4 = x.EvapLeaving4,
                    CondVoltS1 = x.CondVoltS1,
                    CondVoltS2 = x.CondVoltS2,
                    CondVoltS3 = x.CondVoltS3,
                    CondVoltS4 = x.CondVoltS4,
                    FCondAmpsS1 = x.FCondAmpsS1,
                    FCondAmpsS2 = x.FCondAmpsS2,
                    FCondAmpsS3 = x.FCondAmpsS3,
                    FCondAmpsS4 = x.FCondAmpsS4,
                    ICAComments = x.ICAComments,
                    Capacity2 = x.Capacity2,
                    FuseDetailsMDPH = x.FuseDetailsMDPH,
                    Kg2 = x.Kg2,
                    Kg3 = x.Kg3,
                    Kg4 = x.Kg4,
                    FuseDetailsCMDPH = x.FuseDetailsCMDPH,
                    FilterSizeB = x.FilterSizeB,
                    FilterSizeC = x.FilterSizeC,
                    HeadBuilding = x.HeadBuilding,
                    HeadService = x.HeadService,
                    HeadCustomer = x.HeadCustomer,
                }
                ).FirstOrDefault();

                //Get report header data and assign them to the oSplitCommissioningReport object.
                var oReportHeader = context.tblSystemReports.Where(x => x.SystemReportID == val).Select(x => new CommSystemReportViewModel
                {
                    SystemReportID = x.SystemReportID,
                    SystemID = x.SystemID_fk.Value,
                    TemplateID = x.TemplateID_fk.Value,
                    Name = x.Name,
                    TestReference = x.TestReference,
                    Status = x.Status.Value,
                    Date = x.DateTime.Value,
                }
                ).FirstOrDefault();
                oSplitCommissioningReport.Header = oReportHeader;

                //Get report comments and assign them to the oSplitCommissioningReport object.
                var oReportComments = context.tblSystemReportComments.Where(x => x.SysRepID_fk == val).Select(x => new CommCommentViewModel
                {
                    ID = x.ID,
                    SysRepID_fk = x.SysRepID_fk,
                    TechnicianID_fk = x.TechnicianID_fk,
                    DateCreated = x.DateCreated,
                    Comments = x.Comments,
                }
                ).ToList();
                oSplitCommissioningReport.Comments = oReportComments;

                return Request.CreateResponse(HttpStatusCode.OK, oSplitCommissioningReport);
            }
            catch(Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
            
        }


        /// <summary>
        /// Save Split Commissioning report data into the relavant tables. 
        /// It should provide systemid and templateid along with the report related data.
        /// </summary>
        /// <returns>
        /// The Satatus based on the success or failed.
        /// </returns>
        [Authorize]
        [Route("reports/SaveSplitCommissioningReport")]
        [HttpPost]
        public HttpResponseMessage SaveSplitCommissioningReport([FromBody] SplitPackageCommissioningReportViewModel oSplitCommissioningReport)
        {
            try
            {
                //Save report header data and create new systemreportid.
                tblSystemReport oSystemReport = new tblSystemReport();
                oSystemReport.SystemReportID = Guid.NewGuid();
                oSystemReport.SystemID_fk = oSplitCommissioningReport.Header.SystemID;
                oSystemReport.TemplateID_fk = oSplitCommissioningReport.Header.TemplateID;
                oSystemReport.Name = oSplitCommissioningReport.Header.Name;
                oSystemReport.TestReference = oSplitCommissioningReport.Header.TestReference;
                oSystemReport.Status = oSplitCommissioningReport.Header.Status;
                oSystemReport.DateTime = oSplitCommissioningReport.Header.Date;
                context.tblSystemReports.Add(oSystemReport);
                context.SaveChanges();
                var SysReportId = oSystemReport.SystemReportID;

                //Save report data along with the created systemreportid and create new reportid.
                Rpt_SplitPackage oRptSplitCommissioning = new Rpt_SplitPackage();
                oRptSplitCommissioning.ReportID = Guid.NewGuid();
                oRptSplitCommissioning.SystemReportID_fk = SysReportId;
                oRptSplitCommissioning.FanLoc = oSplitCommissioningReport.FanLoc;
                oRptSplitCommissioning.Manufacturer = oSplitCommissioningReport.Manufacturer;
                oRptSplitCommissioning.ModelNo = oSplitCommissioningReport.ModelNo;
                oRptSplitCommissioning.SerialNo = oSplitCommissioningReport.SerialNo;
                oRptSplitCommissioning.Capacity = oSplitCommissioningReport.Capacity;
                oRptSplitCommissioning.FanType = oSplitCommissioningReport.FanType;
                oRptSplitCommissioning.FanArrange = oSplitCommissioningReport.FanArrange;
                oRptSplitCommissioning.MSSBName = oSplitCommissioningReport.MSSBName;
                oRptSplitCommissioning.MSSBLoc = oSplitCommissioningReport.MSSBLoc;
                oRptSplitCommissioning.TotDes = oSplitCommissioningReport.TotDes;
                oRptSplitCommissioning.TotAct = oSplitCommissioningReport.TotAct;
                oRptSplitCommissioning.OutDes = oSplitCommissioningReport.OutDes;
                oRptSplitCommissioning.OutAct = oSplitCommissioningReport.OutAct;
                oRptSplitCommissioning.ReturnDes = oSplitCommissioningReport.ReturnDes;
                oRptSplitCommissioning.ReturnAct = oSplitCommissioningReport.ReturnAct;
                oRptSplitCommissioning.ExtDes = oSplitCommissioningReport.ExtDes;
                oRptSplitCommissioning.ExtAct = oSplitCommissioningReport.ExtAct;
                oRptSplitCommissioning.SucDes = oSplitCommissioningReport.SucDes;
                oRptSplitCommissioning.SucAct = oSplitCommissioningReport.SucAct;
                oRptSplitCommissioning.DisDes = oSplitCommissioningReport.DisDes;
                oRptSplitCommissioning.DisAct = oSplitCommissioningReport.DisAct;
                oRptSplitCommissioning.MotorMake = oSplitCommissioningReport.MotorMake;
                oRptSplitCommissioning.FrameClass = oSplitCommissioningReport.FrameClass;
                oRptSplitCommissioning.ElectSupply = oSplitCommissioningReport.ElectSupply;
                oRptSplitCommissioning.MotorPower = oSplitCommissioningReport.MotorPower;
                oRptSplitCommissioning.FullLoadAmps = oSplitCommissioningReport.FullLoadAmps;
                oRptSplitCommissioning.Pole = oSplitCommissioningReport.Pole;
                oRptSplitCommissioning.RPM = oSplitCommissioningReport.RPM;
                oRptSplitCommissioning.FuseDetails = oSplitCommissioningReport.FuseDetails;
                oRptSplitCommissioning.FanDia = oSplitCommissioningReport.FanDia;
                oRptSplitCommissioning.FanBush = oSplitCommissioningReport.FanBush;
                oRptSplitCommissioning.FanBore = oSplitCommissioningReport.FanBore;
                oRptSplitCommissioning.MotorDia = oSplitCommissioningReport.MotorDia;
                oRptSplitCommissioning.MotorBush = oSplitCommissioningReport.MotorBush;
                oRptSplitCommissioning.MotorBore = oSplitCommissioningReport.MotorBore;
                oRptSplitCommissioning.BeltsMake = oSplitCommissioningReport.BeltsMake;
                oRptSplitCommissioning.BeltsNo = oSplitCommissioningReport.BeltsNo;
                oRptSplitCommissioning.BeltsSize = oSplitCommissioningReport.BeltsSize;
                oRptSplitCommissioning.ShaftCentre = oSplitCommissioningReport.ShaftCentre;
                oRptSplitCommissioning.FilterType = oSplitCommissioningReport.FilterType;
                oRptSplitCommissioning.FilterSize = oSplitCommissioningReport.FilterSize;
                oRptSplitCommissioning.NoOfFilters = oSplitCommissioningReport.NoOfFilters;
                oRptSplitCommissioning.Location = oSplitCommissioningReport.Location;
                oRptSplitCommissioning.CondManu = oSplitCommissioningReport.CondManu;
                oRptSplitCommissioning.CondMNo = oSplitCommissioningReport.CondMNo;
                oRptSplitCommissioning.CondSNo = oSplitCommissioningReport.CondSNo;
                oRptSplitCommissioning.CompManu = oSplitCommissioningReport.CompManu;
                oRptSplitCommissioning.CompModNo = oSplitCommissioningReport.CompModNo;
                oRptSplitCommissioning.CompElec = oSplitCommissioningReport.CompElec;
                oRptSplitCommissioning.CompFull = oSplitCommissioningReport.CompFull;
                oRptSplitCommissioning.Ref = oSplitCommissioningReport.Ref;
                oRptSplitCommissioning.Kg = oSplitCommissioningReport.Kg;
                oRptSplitCommissioning.HeatingType = oSplitCommissioningReport.HeatingType;
                oRptSplitCommissioning.CondFanMake = oSplitCommissioningReport.CondFanMake;
                oRptSplitCommissioning.CondFanNo = oSplitCommissioningReport.CondFanNo;
                oRptSplitCommissioning.CondFan = oSplitCommissioningReport.CondFan;
                oRptSplitCommissioning.MotorPowerC = oSplitCommissioningReport.MotorPowerC;
                oRptSplitCommissioning.FullLoadAmpsC = oSplitCommissioningReport.FullLoadAmpsC;
                oRptSplitCommissioning.PoleC = oSplitCommissioningReport.PoleC;
                oRptSplitCommissioning.RPMC = oSplitCommissioningReport.RPMC;
                oRptSplitCommissioning.FuseDetailsC = oSplitCommissioningReport.FuseDetailsC;
                oRptSplitCommissioning.SubS1 = oSplitCommissioningReport.SubS1;
                oRptSplitCommissioning.SupS2 = oSplitCommissioningReport.SupS2;
                oRptSplitCommissioning.SupS3 = oSplitCommissioningReport.SupS3;
                oRptSplitCommissioning.SupS4 = oSplitCommissioningReport.SupS4;
                oRptSplitCommissioning.CompS1 = oSplitCommissioningReport.CompS1;
                oRptSplitCommissioning.CompS2 = oSplitCommissioningReport.CompS2;
                oRptSplitCommissioning.CompS3 = oSplitCommissioningReport.CompS3;
                oRptSplitCommissioning.CompS4 = oSplitCommissioningReport.CompS4;
                oRptSplitCommissioning.CompVoltS1 = oSplitCommissioningReport.CompVoltS1;
                oRptSplitCommissioning.CompVoltS2 = oSplitCommissioningReport.CompVoltS2;
                oRptSplitCommissioning.CompVoltS3 = oSplitCommissioningReport.CompVoltS3;
                oRptSplitCommissioning.CompVoltS4 = oSplitCommissioningReport.CompVoltS4;
                oRptSplitCommissioning.AmbiS1 = oSplitCommissioningReport.AmbiS1;
                oRptSplitCommissioning.AmbiS2 = oSplitCommissioningReport.AmbiS2;
                oRptSplitCommissioning.AmbiS3 = oSplitCommissioningReport.AmbiS3;
                oRptSplitCommissioning.AmbiS4 = oSplitCommissioningReport.AmbiS4;
                oRptSplitCommissioning.SucS1 = oSplitCommissioningReport.SucS1;
                oRptSplitCommissioning.SucS2 = oSplitCommissioningReport.SucS2;
                oRptSplitCommissioning.SucS3 = oSplitCommissioningReport.SucS3;
                oRptSplitCommissioning.SucS4 = oSplitCommissioningReport.SucS4;
                oRptSplitCommissioning.DisS1 = oSplitCommissioningReport.DisS1;
                oRptSplitCommissioning.DisS2 = oSplitCommissioningReport.DisS2;
                oRptSplitCommissioning.DisS3 = oSplitCommissioningReport.DisS3;
                oRptSplitCommissioning.DisS4 = oSplitCommissioningReport.DisS4;
                oRptSplitCommissioning.OilS1 = oSplitCommissioningReport.OilS1;
                oRptSplitCommissioning.OilS2 = oSplitCommissioningReport.OilS2;
                oRptSplitCommissioning.OilS3 = oSplitCommissioningReport.OilS3;
                oRptSplitCommissioning.OilS4 = oSplitCommissioningReport.OilS4;
                oRptSplitCommissioning.CompSucS1 = oSplitCommissioningReport.CompSucS1;
                oRptSplitCommissioning.CompSucS2 = oSplitCommissioningReport.CompSucS2;
                oRptSplitCommissioning.CompSucS3 = oSplitCommissioningReport.CompSucS3;
                oRptSplitCommissioning.CompSucS4 = oSplitCommissioningReport.CompSucS4;
                oRptSplitCommissioning.LiqS1 = oSplitCommissioningReport.LiqS1;
                oRptSplitCommissioning.LiqS2 = oSplitCommissioningReport.LiqS2;
                oRptSplitCommissioning.LiqS3 = oSplitCommissioningReport.LiqS3;
                oRptSplitCommissioning.LiqS4 = oSplitCommissioningReport.LiqS4;
                oRptSplitCommissioning.SuperS1 = oSplitCommissioningReport.SuperS1;
                oRptSplitCommissioning.SuperS2 = oSplitCommissioningReport.SuperS2;
                oRptSplitCommissioning.SuperS3 = oSplitCommissioningReport.SuperS3;
                oRptSplitCommissioning.SuperS4 = oSplitCommissioningReport.SuperS4;
                oRptSplitCommissioning.SubS2 = oSplitCommissioningReport.SubS2;
                oRptSplitCommissioning.SubS3 = oSplitCommissioningReport.SubS3;
                oRptSplitCommissioning.SubS4 = oSplitCommissioningReport.SubS4;
                oRptSplitCommissioning.EvapEntering1 = oSplitCommissioningReport.EvapEntering1;
                oRptSplitCommissioning.EvapEntering2 = oSplitCommissioningReport.EvapEntering2;
                oRptSplitCommissioning.EvapEntering3 = oSplitCommissioningReport.EvapEntering3;
                oRptSplitCommissioning.EvapEntering4 = oSplitCommissioningReport.EvapEntering4;
                oRptSplitCommissioning.EvapLeaving1 = oSplitCommissioningReport.EvapLeaving1;
                oRptSplitCommissioning.EvapLeaving2 = oSplitCommissioningReport.EvapLeaving2;
                oRptSplitCommissioning.EvapLeaving3 = oSplitCommissioningReport.EvapLeaving3;
                oRptSplitCommissioning.EvapLeaving4 = oSplitCommissioningReport.EvapLeaving4;
                oRptSplitCommissioning.CondVoltS1 = oSplitCommissioningReport.CondVoltS1;
                oRptSplitCommissioning.CondVoltS2 = oSplitCommissioningReport.CondVoltS2;
                oRptSplitCommissioning.CondVoltS3 = oSplitCommissioningReport.CondVoltS3;
                oRptSplitCommissioning.CondVoltS4 = oSplitCommissioningReport.CondVoltS4;
                oRptSplitCommissioning.FCondAmpsS1 = oSplitCommissioningReport.FCondAmpsS1;
                oRptSplitCommissioning.FCondAmpsS2 = oSplitCommissioningReport.FCondAmpsS2;
                oRptSplitCommissioning.FCondAmpsS3 = oSplitCommissioningReport.FCondAmpsS3;
                oRptSplitCommissioning.FCondAmpsS4 = oSplitCommissioningReport.FCondAmpsS4;
                oRptSplitCommissioning.ICAComments = oSplitCommissioningReport.ICAComments;
                oRptSplitCommissioning.Capacity2 = oSplitCommissioningReport.Capacity2;
                oRptSplitCommissioning.FuseDetailsMDPH = oSplitCommissioningReport.FuseDetailsMDPH;
                oRptSplitCommissioning.Kg2 = oSplitCommissioningReport.Kg2;
                oRptSplitCommissioning.Kg3 = oSplitCommissioningReport.Kg3;
                oRptSplitCommissioning.Kg4 = oSplitCommissioningReport.Kg4;
                oRptSplitCommissioning.FuseDetailsCMDPH = oSplitCommissioningReport.FuseDetailsCMDPH;
                oRptSplitCommissioning.FilterSizeB = oSplitCommissioningReport.FilterSizeB;
                oRptSplitCommissioning.FilterSizeC = oSplitCommissioningReport.FilterSizeC;
                oRptSplitCommissioning.HeadBuilding = oSplitCommissioningReport.HeadBuilding;
                oRptSplitCommissioning.HeadService = oSplitCommissioningReport.HeadService;
                oRptSplitCommissioning.HeadCustomer = oSplitCommissioningReport.HeadCustomer;
                context.Rpt_SplitPackage.Add(oRptSplitCommissioning);
                context.SaveChanges();
                var RepId = oRptSplitCommissioning.ReportID;

                //Save report comments.
                List<CommCommentViewModel> lstComments = new List<CommCommentViewModel>();
                lstComments = oSplitCommissioningReport.Comments;
                foreach (var item in lstComments)
                {
                    tblSystemReportComment oSplitCommissioningComment = new tblSystemReportComment();
                    oSplitCommissioningComment.ID = Guid.NewGuid();
                    oSplitCommissioningComment.SysRepID_fk = SysReportId;
                    oSplitCommissioningComment.TechnicianID_fk = item.TechnicianID_fk;
                    oSplitCommissioningComment.DateCreated = item.DateCreated;
                    oSplitCommissioningComment.Comments = item.Comments;
                    context.tblSystemReportComments.Add(oSplitCommissioningComment);
                    context.SaveChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
        }


        /// <summary>
        /// Get Air Commissioning report details for given system report id.
        /// </summary>
        /// <returns>
        /// The oAirCommissioningReport object which contains all the information.
        /// </returns>
        [Authorize]
        [Route("reports/GetAirCommissioningReportBySystemReportID")]
        [HttpPost]
        public HttpResponseMessage GetAirCommissioningReportBySystemReportID([FromBody] string systemreportid)
        {
            try
            {
                //Get report data by providing systemreportid.
                var val = Guid.Parse(systemreportid);
                var oAirCommissioningReport = context.Rpt_AirBalance.Where(x => x.SystemReportID_fk == val).Select(x => new AirCommissioningReportViewModel
                {
                    ReportID = x.ReportID,
                    SystemReportID_fk = x.SystemReportID_fk,
                    ZoneSystem = x.ZoneSystem,
                    ZoneSetUpComplete = x.ZoneSetUpComplete,
                    ICAComments = x.ICAComments,
                    HeadBuilding = x.HeadBuilding,
                    HeadService = x.HeadService,
                    HeadCustomer = x.HeadCustomer,
                }
                ).FirstOrDefault();

                //Get report header data and assign to the oAirCommissioningReport object.
                var oReportHeader = context.tblSystemReports.Where(x => x.SystemReportID == val).Select(x => new CommSystemReportViewModel
                {
                    SystemReportID = x.SystemReportID,
                    SystemID = x.SystemID_fk.Value,
                    TemplateID = x.TemplateID_fk.Value,
                    Name = x.Name,
                    TestReference = x.TestReference,
                    Status = x.Status.Value,
                    Date = x.DateTime.Value,
                }
                ).FirstOrDefault();
                oAirCommissioningReport.Header = oReportHeader;

                //Get report details data and assign them to the oAirCommissioningReport object.
                var oReportData = context.Rpt_AirbalanceZones.Where(x => x.ReportID_fk == oAirCommissioningReport.ReportID).Select(x => new AirCommissioningDataViewModel
                {
                    ID = x.ID,
                    ReportID_fk = x.ReportID_fk,
                    BranchRef = x.BranchRef,
                    AreaZone = x.AreaZone,
                    NoGrilles = x.NoGrilles,
                    GrilleType = x.GrilleType,
                    SizeA = x.SizeA,
                    SizeB = x.SizeB,
                    Factor = x.Factor,
                    CorrectedArea = x.CorrectedArea,
                    TestApparatus = x.TestApparatus,
                    DisplayOrder = x.DisplayOrder,
                    ZoneType = x.ZoneType,
                    GrilleRef = x.GrilleRef,
                    DesFlow = x.DesFlow,
                }).ToList();
                oAirCommissioningReport.ReportData = oReportData;

                //Get report comments data and assign to the oAirCommissioningReport object.
                var oReportComments = context.tblSystemReportComments.Where(x => x.SysRepID_fk == val).Select(x => new CommCommentViewModel
                {
                    ID = x.ID,
                    SysRepID_fk = x.SysRepID_fk,
                    TechnicianID_fk = x.TechnicianID_fk,
                    DateCreated = x.DateCreated,
                    Comments = x.Comments,
                }
                ).ToList();
                oAirCommissioningReport.Comments = oReportComments;

                return Request.CreateResponse(HttpStatusCode.OK, oAirCommissioningReport);
            }
            catch(Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
            
        }


        /// <summary>
        /// Save Air Commissioning report data into the relavant tables.
        /// It should provide systemid and templateid along with the report related data.
        /// </summary>
        /// <returns>
        /// The Satatus based on the success or failed.
        /// </returns>
        [Authorize]
        [Route("reports/SaveAirCommissioningReport")]
        [HttpPost]
        public HttpResponseMessage SaveAirCommissioningReport([FromBody] AirCommissioningReportViewModel oAirCommissioningReport)
        {
            try
            {
                //Save report header data and create new systemreportid.
                tblSystemReport oSystemReport = new tblSystemReport();
                oSystemReport.SystemReportID = Guid.NewGuid();
                oSystemReport.SystemID_fk = oAirCommissioningReport.Header.SystemID;
                oSystemReport.TemplateID_fk = oAirCommissioningReport.Header.TemplateID;
                oSystemReport.Name = oAirCommissioningReport.Header.Name;
                oSystemReport.TestReference = oAirCommissioningReport.Header.TestReference;
                oSystemReport.Status = oAirCommissioningReport.Header.Status;
                oSystemReport.DateTime = oAirCommissioningReport.Header.Date;
                context.tblSystemReports.Add(oSystemReport);
                context.SaveChanges();
                var SysReportId = oSystemReport.SystemReportID;

                //Save report data along with the created systemreportid and create new reportid.
                Rpt_AirBalance oRptAirCommissioning = new Rpt_AirBalance();
                oRptAirCommissioning.ReportID = Guid.NewGuid();
                oRptAirCommissioning.SystemReportID_fk = SysReportId;
                oRptAirCommissioning.ZoneSystem = oAirCommissioningReport.ZoneSystem;
                oRptAirCommissioning.ZoneSetUpComplete = oAirCommissioningReport.ZoneSetUpComplete;
                oRptAirCommissioning.ICAComments = oAirCommissioningReport.ICAComments;
                oRptAirCommissioning.HeadBuilding = oAirCommissioningReport.HeadBuilding;
                oRptAirCommissioning.HeadService = oAirCommissioningReport.HeadService;
                oRptAirCommissioning.HeadCustomer = oAirCommissioningReport.HeadCustomer;
                context.Rpt_AirBalance.Add(oRptAirCommissioning);
                context.SaveChanges();
                var RepId = oRptAirCommissioning.ReportID;

                //Save report details data along with the created reportid.
                List<AirCommissioningDataViewModel> lstData = new List<AirCommissioningDataViewModel>();
                lstData = oAirCommissioningReport.ReportData;
                foreach (var item in lstData)
                {
                    Rpt_AirbalanceZones oData = new Rpt_AirbalanceZones();
                    oData.ID = Guid.NewGuid();
                    oData.ReportID_fk = RepId;
                    oData.BranchRef = item.BranchRef;
                    oData.AreaZone = item.AreaZone;
                    oData.NoGrilles = item.NoGrilles;
                    oData.GrilleType = item.GrilleType;
                    oData.SizeA = item.SizeA;
                    oData.SizeB = item.SizeB;
                    oData.Factor = item.Factor;
                    oData.CorrectedArea = item.CorrectedArea;
                    oData.TestApparatus = item.TestApparatus;
                    oData.DisplayOrder = item.DisplayOrder;
                    oData.ZoneType = item.ZoneType;
                    oData.GrilleRef = item.GrilleRef;
                    oData.DesFlow = item.DesFlow;
                    context.Rpt_AirbalanceZones.Add(oData);
                    context.SaveChanges();
                }

                //Save report commnts data.
                List<CommCommentViewModel> lstComments = new List<CommCommentViewModel>();
                lstComments = oAirCommissioningReport.Comments;
                foreach (var item in lstComments)
                {
                    tblSystemReportComment oVavCommissioningComment = new tblSystemReportComment();
                    oVavCommissioningComment.ID = Guid.NewGuid();
                    oVavCommissioningComment.SysRepID_fk = SysReportId;
                    oVavCommissioningComment.TechnicianID_fk = item.TechnicianID_fk;
                    oVavCommissioningComment.DateCreated = item.DateCreated;
                    oVavCommissioningComment.Comments = item.Comments;
                    context.tblSystemReportComments.Add(oVavCommissioningComment);
                    context.SaveChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
        }


        /// <summary>
        /// Get Ahu Test report details for given system report id.
        /// </summary>
        /// <returns>
        /// The oAhuTestReport object which contains all the information.
        /// </returns>        
        [Authorize]
        [Route("reports/GetAhuTestReportBySystemReportID")]
        [HttpPost]
        public HttpResponseMessage GetAhuTestReportBySystemReportID([FromBody] string systemreportid)
        {
            try
            {
                //Get report details from the report table by providing systemreportid.
                var val = Guid.Parse(systemreportid);
                var oAhuTestReport = context.Rpt_AhuTest.Where(x => x.SystemReportID_fk == val).Select(x => new AhuTestReportViewModel
                {
                    ReportID = x.ReportID,
                    SystemReportID_fk = x.SystemReportID_fk.Value,
                    LocationAHU = x.LocationAHU,
                    ManufactureAHU = x.ManufactureAHU,
                    ModelNoAHU = x.ModelNoAHU,
                    SerialNoAHU = x.SerialNoAHU,
                    CapacityAHU = x.CapacityAHU,
                    DesignAirLsAHU = x.DesignAirLsAHU,
                    DesignAirPaAHU = x.DesignAirPaAHU,
                    MediaAHU = x.MediaAHU,
                    DesignWaterAHU = x.DesignWaterAHU,
                    FanTypeAHU = x.FanTypeAHU,
                    FanArrangementAHU = x.FanArrangementAHU,
                    MSSBNameAHU = x.MSSBNameAHU,
                    MSSBLocationAHU = x.MSSBLocationAHU,
                    MotorMakeMD = x.MotorMakeMD,
                    FrameMD = x.FrameMD,
                    ElectricalMD = x.ElectricalMD,
                    MotorPowerMD = x.MotorPowerMD,
                    FullLoadMD = x.FullLoadMD,
                    PoleMD = x.PoleMD,
                    PoleRPMMD = x.PoleRPMMD,
                    FuseDetailsMD = x.FuseDetailsMD,
                    FanPulleyFan = x.FanPulleyFan,
                    FanBushFan = x.FanBushFan,
                    FanBoreFan = x.FanBoreFan,
                    MotorPulleyFan = x.MotorPulleyFan,
                    MotorBushFan = x.MotorBushFan,
                    MotorBoreFan = x.MotorBoreFan,
                    BeltsMakeFan = x.BeltsMakeFan,
                    BeltsNoFan = x.BeltsNoFan,
                    BeltsSizeFan = x.BeltsSizeFan,
                    ShaftCentreFan = x.ShaftCentreFan,
                    OverloadMakeMS = x.OverloadMakeMS,
                    OverloadRangeMS = x.OverloadRangeMS,
                    VSDMakeMS = x.VSDMakeMS,
                    VSDRangeMS = x.VSDRangeMS,
                    ValveACT = x.ValveACT,
                    ModelACT = x.ModelACT,
                    SizeACT = x.SizeACT,
                    CoilACT = x.CoilACT,
                    ValveBAL = x.ValveBAL,
                    ModelBAL = x.ModelBAL,
                    SizeBAL = x.SizeBAL,
                    FilterType1 = x.FilterType1,
                    FilterSize1 = x.FilterSize1,
                    NoFilters1 = x.NoFilters1,
                    FilterType2 = x.FilterType2,
                    FilterSize2 = x.FilterSize2,
                    NoFilters2 = x.NoFilters2,
                    SupplyAirDES = x.SupplyAirDES,
                    ReturnAirDES = x.ReturnAirDES,
                    OutsideAirDES = x.OutsideAirDES,
                    PchwDES = x.PchwDES,
                    SchwDES = x.SchwDES,
                    TotalStaticDES = x.TotalStaticDES,
                    SucStaticDES = x.SucStaticDES,
                    DisStaticDES = x.DisStaticDES,
                    FilterDiffDES = x.FilterDiffDES,
                    PchwDiffDES = x.PchwDiffDES,
                    ScheDiffDES = x.ScheDiffDES,
                    VoltageDES = x.VoltageDES,
                    AmpsDES = x.AmpsDES,
                    SupplyAirFIN = x.SupplyAirFIN,
                    ReturnAirFIN = x.ReturnAirFIN,
                    OutsideAirFIN = x.OutsideAirFIN,
                    PchwFIN = x.PchwFIN,
                    SchwFIN = x.SchwFIN,
                    TotalStaticFIN = x.TotalStaticFIN,
                    SucStaticFIN = x.SucStaticFIN,
                    DisStaticFIN = x.DisStaticFIN,
                    FilterDiffFIN = x.FilterDiffFIN,
                    PchwDiffFIN = x.PchwDiffFIN,
                    ScheDiffFIN = x.ScheDiffFIN,
                    VoltageFIN = x.VoltageFIN,
                    AmpsFIN = x.AmpsFIN,
                    Instrument1 = x.Instrument1,
                    Model1 = x.Model1,
                    Serial1 = x.Serial1,
                    Instrument2 = x.Instrument2,
                    Model2 = x.Model2,
                    Serial2 = x.Serial2,
                    TechnicianComments = x.TechnicianComments,
                    ICAComments = x.ICAComments,
                    FuseDetailsMDPH = x.FuseDetailsMDPH,
                    OverloadRangeMS2 = x.OverloadRangeMS2,
                    OverloadRangeMS3 = x.OverloadRangeMS3,
                    FilterSize1B = x.FilterSize1B,
                    FilterSize1C = x.FilterSize1C,
                    FilterSize2B = x.FilterSize2B,
                    FilterSize2C = x.FilterSize2C,
                    KvACT = x.KvACT,
                    ShowSucStatic = x.ShowSucStatic,
                    ShowDisStatic = x.ShowDisStatic,
                    ShowFilterDiff = x.ShowFilterDiff,
                    ShowPchwDiff = x.ShowPchwDiff,
                    ScheDiff = x.ScheDiff,
                    HHWCoilDes = x.HHWCoilDes,
                    HHWCoilFin = x.HHWCoilFin,
                    AmpsHzDes = x.AmpsHzDes,
                    AmpsHzFin = x.AmpsHzFin,
                    HeadBuilding = x.HeadBuilding,
                    HeadService = x.HeadService,
                    HeadCustomer = x.HeadCustomer,
                    BMSFanSpeed = x.BMSFanSpeed,
                    BMSVelocity = x.BMSVelocity,
                    BMSPressure = x.BMSPressure,
                    BMSOther = x.BMSOther,
                    SupplyAirPER = x.SupplyAirPER,
                    ReturnAirPER = x.ReturnAirPER,
                    OutsideAirPER = x.OutsideAirPER,
                    PchwPER = x.PchwPER,
                    TotalStaticPER = x.TotalStaticPER,
                    WitnessName = x.WitnessName,
                    WitnessDate = x.WitnessDate,
                    WitnessSignature = x.WitnessSignature,
                    FilterSize3 = x.FilterSize3,
                    FilterSize3B = x.FilterSize3B,
                    FilterSize3C = x.FilterSize3C,
                    FilterSize4 = x.FilterSize4,
                    FilterSize4B = x.FilterSize4B,
                    FilterSize4C = x.FilterSize4C,
                    NoFilters3 = x.NoFilters3,
                    NoFilters4 = x.NoFilters4,
                    CapacityAHUHeat = x.CapacityAHUHeat,
                    CapacityAHUReclaim = x.CapacityAHUReclaim,
                    BMSFanSpeedType = x.BMSFanSpeedType,
                    BMSVelocityType = x.BMSVelocityType,
                }
                ).FirstOrDefault();

                //Get report header information and assign them to oAhuTestReport object.
                var oReportHeader = context.tblSystemReports.Where(x => x.SystemReportID == val).Select(x => new CommSystemReportViewModel
                {
                    SystemReportID = x.SystemReportID,
                    SystemID = x.SystemID_fk.Value,
                    TemplateID = x.TemplateID_fk.Value,
                    Name = x.Name,
                    TestReference = x.TestReference,
                    Status = x.Status.Value,
                    Date = x.DateTime.Value,
                }
                ).FirstOrDefault();
                oAhuTestReport.Header = oReportHeader;

                //Get report comments and assign them to the oAhuTestReport object.
                var oReportComments = context.tblSystemReportComments.Where(x => x.SysRepID_fk == val).Select(x => new CommCommentViewModel
                {
                    ID = x.ID,
                    SysRepID_fk = x.SysRepID_fk,
                    TechnicianID_fk = x.TechnicianID_fk,
                    DateCreated = x.DateCreated,
                    Comments = x.Comments,
                }
                ).ToList();
                oAhuTestReport.Comments = oReportComments;

                return Request.CreateResponse(HttpStatusCode.OK, oAhuTestReport);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }

        }

        /// <summary>
        /// Save Ahu Test report data into the relavant tables.
        /// It should provide systemid and templateid along with the report related data. 
        /// </summary>
        /// <returns>
        /// The Satatus based on the success or failed.
        /// </returns>
        [Authorize]
        [Route("reports/SaveAhuTestReport")]
        [HttpPost]
        public HttpResponseMessage SaveAhuTestReport([FromBody] AhuTestReportViewModel oAhuTestReport)
        {
            try
            {
                //Save report header information.
                tblSystemReport oSystemReport = new tblSystemReport();
                oSystemReport.SystemReportID = Guid.NewGuid();
                oSystemReport.SystemID_fk = oAhuTestReport.Header.SystemID;
                oSystemReport.TemplateID_fk = oAhuTestReport.Header.TemplateID;
                oSystemReport.Name = oAhuTestReport.Header.Name;
                oSystemReport.TestReference = oAhuTestReport.Header.TestReference;
                oSystemReport.Status = oAhuTestReport.Header.Status;
                oSystemReport.DateTime = oAhuTestReport.Header.Date;
                context.tblSystemReports.Add(oSystemReport);
                context.SaveChanges();
                var SysReportId = oSystemReport.SystemReportID;

                //Save report details information.
                Rpt_AhuTest oRptAhuTest = new Rpt_AhuTest();
                oRptAhuTest.ReportID = Guid.NewGuid();
                oRptAhuTest.SystemReportID_fk = SysReportId;
                oRptAhuTest.LocationAHU = oAhuTestReport.LocationAHU;
                oRptAhuTest.ManufactureAHU = oAhuTestReport.ManufactureAHU;
                oRptAhuTest.ModelNoAHU = oAhuTestReport.ModelNoAHU;
                oRptAhuTest.SerialNoAHU = oAhuTestReport.SerialNoAHU;
                oRptAhuTest.CapacityAHU = oAhuTestReport.CapacityAHU;
                oRptAhuTest.DesignAirLsAHU = oAhuTestReport.DesignAirLsAHU;
                oRptAhuTest.DesignAirPaAHU = oAhuTestReport.DesignAirPaAHU;
                oRptAhuTest.MediaAHU = oAhuTestReport.MediaAHU;
                oRptAhuTest.DesignWaterAHU = oAhuTestReport.DesignWaterAHU;
                oRptAhuTest.FanTypeAHU = oAhuTestReport.FanTypeAHU;
                oRptAhuTest.FanArrangementAHU = oAhuTestReport.FanArrangementAHU;
                oRptAhuTest.MSSBNameAHU = oAhuTestReport.MSSBNameAHU;
                oRptAhuTest.MSSBLocationAHU = oAhuTestReport.MSSBLocationAHU;
                oRptAhuTest.MotorMakeMD = oAhuTestReport.MotorMakeMD;
                oRptAhuTest.FrameMD = oAhuTestReport.FrameMD;
                oRptAhuTest.ElectricalMD = oAhuTestReport.ElectricalMD;
                oRptAhuTest.MotorPowerMD = oAhuTestReport.MotorPowerMD;
                oRptAhuTest.FullLoadMD = oAhuTestReport.FullLoadMD;
                oRptAhuTest.PoleMD = oAhuTestReport.PoleMD;
                oRptAhuTest.PoleRPMMD = oAhuTestReport.PoleRPMMD;
                oRptAhuTest.FuseDetailsMD = oAhuTestReport.FuseDetailsMD;
                oRptAhuTest.FanPulleyFan = oAhuTestReport.FanPulleyFan;
                oRptAhuTest.FanBushFan = oAhuTestReport.FanBushFan;
                oRptAhuTest.FanBoreFan = oAhuTestReport.FanBoreFan;
                oRptAhuTest.MotorPulleyFan = oAhuTestReport.MotorPulleyFan;
                oRptAhuTest.MotorBushFan = oAhuTestReport.MotorBushFan;
                oRptAhuTest.MotorBoreFan = oAhuTestReport.MotorBoreFan;
                oRptAhuTest.BeltsMakeFan = oAhuTestReport.BeltsMakeFan;
                oRptAhuTest.BeltsNoFan = oAhuTestReport.BeltsNoFan;
                oRptAhuTest.BeltsSizeFan = oAhuTestReport.BeltsSizeFan;
                oRptAhuTest.ShaftCentreFan = oAhuTestReport.ShaftCentreFan;
                oRptAhuTest.OverloadMakeMS = oAhuTestReport.OverloadMakeMS;
                oRptAhuTest.OverloadRangeMS = oAhuTestReport.OverloadRangeMS;
                oRptAhuTest.VSDMakeMS = oAhuTestReport.VSDMakeMS;
                oRptAhuTest.VSDRangeMS = oAhuTestReport.VSDRangeMS;
                oRptAhuTest.ValveACT = oAhuTestReport.ValveACT;
                oRptAhuTest.ModelACT = oAhuTestReport.ModelACT;
                oRptAhuTest.SizeACT = oAhuTestReport.SizeACT;
                oRptAhuTest.CoilACT = oAhuTestReport.CoilACT;
                oRptAhuTest.ValveBAL = oAhuTestReport.ValveBAL;
                oRptAhuTest.ModelBAL = oAhuTestReport.ModelBAL;
                oRptAhuTest.SizeBAL = oAhuTestReport.SizeBAL;
                oRptAhuTest.FilterType1 = oAhuTestReport.FilterType1;
                oRptAhuTest.FilterSize1 = oAhuTestReport.FilterSize1;
                oRptAhuTest.NoFilters1 = oAhuTestReport.NoFilters1;
                oRptAhuTest.FilterType2 = oAhuTestReport.FilterType2;
                oRptAhuTest.FilterSize2 = oAhuTestReport.FilterSize2;
                oRptAhuTest.NoFilters2 = oAhuTestReport.NoFilters2;
                oRptAhuTest.SupplyAirDES = oAhuTestReport.SupplyAirDES;
                oRptAhuTest.ReturnAirDES = oAhuTestReport.ReturnAirDES;
                oRptAhuTest.OutsideAirDES = oAhuTestReport.OutsideAirDES;
                oRptAhuTest.PchwDES = oAhuTestReport.PchwDES;
                oRptAhuTest.SchwDES = oAhuTestReport.SchwDES;
                oRptAhuTest.TotalStaticDES = oAhuTestReport.TotalStaticDES;
                oRptAhuTest.SucStaticDES = oAhuTestReport.SucStaticDES;
                oRptAhuTest.DisStaticDES = oAhuTestReport.DisStaticDES;
                oRptAhuTest.FilterDiffDES = oAhuTestReport.FilterDiffDES;
                oRptAhuTest.PchwDiffDES = oAhuTestReport.PchwDiffDES;
                oRptAhuTest.ScheDiffDES = oAhuTestReport.ScheDiffDES;
                oRptAhuTest.VoltageDES = oAhuTestReport.VoltageDES;
                oRptAhuTest.AmpsDES = oAhuTestReport.AmpsDES;
                oRptAhuTest.SupplyAirFIN = oAhuTestReport.SupplyAirFIN;
                oRptAhuTest.ReturnAirFIN = oAhuTestReport.ReturnAirFIN;
                oRptAhuTest.OutsideAirFIN = oAhuTestReport.OutsideAirFIN;
                oRptAhuTest.PchwFIN = oAhuTestReport.PchwFIN;
                oRptAhuTest.SchwFIN = oAhuTestReport.SchwFIN;
                oRptAhuTest.TotalStaticFIN = oAhuTestReport.TotalStaticFIN;
                oRptAhuTest.SucStaticFIN = oAhuTestReport.SucStaticFIN;
                oRptAhuTest.DisStaticFIN = oAhuTestReport.DisStaticFIN;
                oRptAhuTest.FilterDiffFIN = oAhuTestReport.FilterDiffFIN;
                oRptAhuTest.PchwDiffFIN = oAhuTestReport.PchwDiffFIN;
                oRptAhuTest.ScheDiffFIN = oAhuTestReport.ScheDiffFIN;
                oRptAhuTest.VoltageFIN = oAhuTestReport.VoltageFIN;
                oRptAhuTest.AmpsFIN = oAhuTestReport.AmpsFIN;
                oRptAhuTest.Instrument1 = oAhuTestReport.Instrument1;
                oRptAhuTest.Model1 = oAhuTestReport.Model1;
                oRptAhuTest.Serial1 = oAhuTestReport.Serial1;
                oRptAhuTest.Instrument2 = oAhuTestReport.Instrument2;
                oRptAhuTest.Model2 = oAhuTestReport.Model2;
                oRptAhuTest.Serial2 = oAhuTestReport.Serial2;
                oRptAhuTest.TechnicianComments = oAhuTestReport.TechnicianComments;
                oRptAhuTest.ICAComments = oAhuTestReport.ICAComments;
                oRptAhuTest.FuseDetailsMDPH = oAhuTestReport.FuseDetailsMDPH;
                oRptAhuTest.OverloadRangeMS2 = oAhuTestReport.OverloadRangeMS2;
                oRptAhuTest.OverloadRangeMS3 = oAhuTestReport.OverloadRangeMS3;
                oRptAhuTest.FilterSize1B = oAhuTestReport.FilterSize1B;
                oRptAhuTest.FilterSize1C = oAhuTestReport.FilterSize1C;
                oRptAhuTest.FilterSize2B = oAhuTestReport.FilterSize2B;
                oRptAhuTest.FilterSize2C = oAhuTestReport.FilterSize2C;
                oRptAhuTest.KvACT = oAhuTestReport.KvACT;
                oRptAhuTest.ShowSucStatic = oAhuTestReport.ShowSucStatic;
                oRptAhuTest.ShowDisStatic = oAhuTestReport.ShowDisStatic;
                oRptAhuTest.ShowFilterDiff = oAhuTestReport.ShowFilterDiff;
                oRptAhuTest.ShowPchwDiff = oAhuTestReport.ShowPchwDiff;
                oRptAhuTest.ScheDiff = oAhuTestReport.ScheDiff;
                oRptAhuTest.HHWCoilDes = oAhuTestReport.HHWCoilDes;
                oRptAhuTest.HHWCoilFin = oAhuTestReport.HHWCoilFin;
                oRptAhuTest.AmpsHzDes = oAhuTestReport.AmpsHzDes;
                oRptAhuTest.AmpsHzFin = oAhuTestReport.AmpsHzFin;
                oRptAhuTest.HeadBuilding = oAhuTestReport.HeadBuilding;
                oRptAhuTest.HeadService = oAhuTestReport.HeadService;
                oRptAhuTest.HeadCustomer = oAhuTestReport.HeadCustomer;
                oRptAhuTest.BMSFanSpeed = oAhuTestReport.BMSFanSpeed;
                oRptAhuTest.BMSVelocity = oAhuTestReport.BMSVelocity;
                oRptAhuTest.BMSPressure = oAhuTestReport.BMSPressure;
                oRptAhuTest.BMSOther = oAhuTestReport.BMSOther;
                oRptAhuTest.SupplyAirPER = oAhuTestReport.SupplyAirPER;
                oRptAhuTest.ReturnAirPER = oAhuTestReport.ReturnAirPER;
                oRptAhuTest.OutsideAirPER = oAhuTestReport.OutsideAirPER;
                oRptAhuTest.PchwPER = oAhuTestReport.PchwPER;
                oRptAhuTest.TotalStaticPER = oAhuTestReport.TotalStaticPER;
                oRptAhuTest.WitnessName = oAhuTestReport.WitnessName;
                oRptAhuTest.WitnessDate = oAhuTestReport.WitnessDate;
                oRptAhuTest.WitnessSignature = oAhuTestReport.WitnessSignature;
                oRptAhuTest.FilterSize3 = oAhuTestReport.FilterSize3;
                oRptAhuTest.FilterSize3B = oAhuTestReport.FilterSize3B;
                oRptAhuTest.FilterSize3C = oAhuTestReport.FilterSize3C;
                oRptAhuTest.FilterSize4 = oAhuTestReport.FilterSize4;
                oRptAhuTest.FilterSize4B = oAhuTestReport.FilterSize4B;
                oRptAhuTest.FilterSize4C = oAhuTestReport.FilterSize4C;
                oRptAhuTest.NoFilters3 = oAhuTestReport.NoFilters3;
                oRptAhuTest.NoFilters4 = oAhuTestReport.NoFilters4;
                oRptAhuTest.CapacityAHUHeat = oAhuTestReport.CapacityAHUHeat;
                oRptAhuTest.CapacityAHUReclaim = oAhuTestReport.CapacityAHUReclaim;
                oRptAhuTest.BMSFanSpeedType = oAhuTestReport.BMSFanSpeedType;
                oRptAhuTest.BMSVelocityType = oAhuTestReport.BMSVelocityType;
                context.Rpt_AhuTest.Add(oRptAhuTest);
                context.SaveChanges();

                //Save report comments information.
                List<CommCommentViewModel> lstComments = new List<CommCommentViewModel>();
                lstComments = oAhuTestReport.Comments;
                foreach (var item in lstComments)
                {
                    tblSystemReportComment oAhuTestComment = new tblSystemReportComment();
                    oAhuTestComment.ID = Guid.NewGuid();
                    oAhuTestComment.SysRepID_fk = SysReportId;
                    oAhuTestComment.TechnicianID_fk = item.TechnicianID_fk;
                    oAhuTestComment.DateCreated = item.DateCreated;
                    oAhuTestComment.Comments = item.Comments;
                    context.tblSystemReportComments.Add(oAhuTestComment);
                    context.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
        }

        /// <summary>
        /// Get Ahu Test Direct Drive report details for given system report id.
        /// </summary>
        /// <returns>
        /// The oAhuTestDirectDriveReport object which contains all the information.
        /// </returns>
        [Authorize]
        [Route("reports/GetAhuTestDirectDriveReportBySystemReportID")]
        [HttpPost]
        public HttpResponseMessage GetAhuTestDirectDriveReportBySystemReportID([FromBody] string systemreportid)
        {
            try
            {
                //Get report details information by providing systemreportid
                var val = Guid.Parse(systemreportid);
                var oAhuTestDirectDriveReport = context.Rpt_AhuTestDirectDrive.Where(x => x.SystemReportID_fk == val).Select(x => new AhuTestDirectDriveReportViewModel
                {
                    ReportID = x.ReportID,
                    SystemReportID_fk = x.SystemReportID_fk.Value,
                    LocationAHU = x.LocationAHU,
                    ManufactureAHU = x.ManufactureAHU,
                    ModelNoAHU = x.ModelNoAHU,
                    SerialNoAHU = x.SerialNoAHU,
                    CapacityAHU = x.CapacityAHU,
                    DesignAirLsAHU = x.DesignAirLsAHU,
                    DesignAirPaAHU = x.DesignAirPaAHU,
                    MediaAHU = x.MediaAHU,
                    FanTypeAHU = x.FanTypeAHU,
                    FanArrangementAHU = x.FanArrangementAHU,
                    MSSBNameAHU = x.MSSBNameAHU,
                    MSSBLocationAHU = x.MSSBLocationAHU,
                    MotorMakeMD = x.MotorMakeMD,
                    FrameMD = x.FrameMD,
                    ElectricalMD = x.ElectricalMD,
                    MotorPowerMD = x.MotorPowerMD,
                    FullLoadMD = x.FullLoadMD,
                    PoleMD = x.PoleMD,
                    PoleRPMMD = x.PoleRPMMD,
                    OverloadMakeMS = x.OverloadMakeMS,
                    OverloadRangeMS = x.OverloadRangeMS,
                    VSDMakeMS = x.VSDMakeMS,
                    VSDRangeMS = x.VSDRangeMS,
                    FilterType1 = x.FilterType1,
                    FilterSize1 = x.FilterSize1,
                    NoFilters1 = x.NoFilters1,
                    FilterType2 = x.FilterType2,
                    FilterSize2 = x.FilterSize2,
                    NoFilters2 = x.NoFilters2,
                    SupplyAirDES = x.SupplyAirDES,
                    ReturnAirDES = x.ReturnAirDES,
                    OutsideAirDES = x.OutsideAirDES,
                    SupplyAirFIN = x.SupplyAirFIN,
                    ReturnAirFIN = x.ReturnAirFIN,
                    OutsideAirFIN = x.OutsideAirFIN,
                    TotalStaticDES = x.TotalStaticDES,
                    TotalStaticFIN = x.TotalStaticFIN,
                    SucStaticFIN = x.SucStaticFIN,
                    DisStaticFIN = x.DisStaticFIN,
                    FilterDiffFIN = x.FilterDiffFIN,
                    PchwDiffFIN = x.PchwDiffFIN,
                    ScheDiffFIN = x.ScheDiffFIN,
                    VoltageDES = x.VoltageDES,
                    VoltageFIN = x.VoltageFIN,
                    AmpsDES = x.AmpsDES,                   
                    AmpsFIN = x.AmpsFIN,
                    Instrument1 = x.Instrument1,
                    Instrument2 = x.Instrument2,
                    Model1 = x.Model1,
                    Model2 = x.Model2,
                    Serial1 = x.Serial1,                    
                    Serial2 = x.Serial2,
                    FuseDetailsMDPH = x.FuseDetailsMDPH,
                    OverloadRangeMS2 = x.OverloadRangeMS2,
                    OverloadRangeMS3 = x.OverloadRangeMS3,
                    FilterSize1B = x.FilterSize1B,
                    FilterSize1C = x.FilterSize1C,
                    FilterSize2B = x.FilterSize2B,
                    FilterSize2C = x.FilterSize2C,
                    AmpsHzDes = x.AmpsHzDes,
                    AmpsHzFin = x.AmpsHzFin,
                    BMSFanSpeed = x.BMSFanSpeed,
                    BMSVelocity = x.BMSVelocity,
                    BMSPressure = x.BMSPressure,
                    BMSOther = x.BMSOther,
                    SupplyAirPER = x.SupplyAirPER,
                    ReturnAirPER = x.ReturnAirPER,
                    OutsideAirPER = x.OutsideAirPER,
                    HeadBuilding = x.HeadBuilding,
                    WitnessName = x.WitnessName,
                    WitnessDate = x.WitnessDate,
                    WitnessSignature = x.WitnessSignature,
                    FilterSize3 = x.FilterSize3,
                    FilterSize3B = x.FilterSize3B,
                    FilterSize3C = x.FilterSize3C,
                    FilterSize4 = x.FilterSize4,
                    FilterSize4B = x.FilterSize4B,
                    FilterSize4C = x.FilterSize4C,
                    NoFilters3 = x.NoFilters3,
                    NoFilters4 = x.NoFilters4,
                    BMSFanSpeedType = x.BMSFanSpeedType,
                    BMSVelocityType = x.BMSVelocityType,
                }
                ).FirstOrDefault();

                //Get report header information and add them to the oAhuTestDirectDriveReport object.
                var oReportHeader = context.tblSystemReports.Where(x => x.SystemReportID == val).Select(x => new CommSystemReportViewModel
                {
                    SystemReportID = x.SystemReportID,
                    SystemID = x.SystemID_fk.Value,
                    TemplateID = x.TemplateID_fk.Value,
                    Name = x.Name,
                    TestReference = x.TestReference,
                    Status = x.Status.Value,
                    Date = x.DateTime.Value,
                }
                ).FirstOrDefault();
                oAhuTestDirectDriveReport.Header = oReportHeader;

                //Get report comments information and add them to the oAhuTestDirectDriveReport object.
                var oReportComments = context.tblSystemReportComments.Where(x => x.SysRepID_fk == val).Select(x => new CommCommentViewModel
                {
                    ID = x.ID,
                    SysRepID_fk = x.SysRepID_fk,
                    TechnicianID_fk = x.TechnicianID_fk,
                    DateCreated = x.DateCreated,
                    Comments = x.Comments,
                }
                ).ToList();
                oAhuTestDirectDriveReport.Comments = oReportComments;

                return Request.CreateResponse(HttpStatusCode.OK, oAhuTestDirectDriveReport);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }

        }

        /// <summary>
        /// Save Ahu Test Direct Drive report data into the relavant tables. 
        /// It should provide systemid and templateid along with the report related data.
        /// </summary>
        /// <returns>
        /// The Satatus based on the success or failed.
        /// </returns>
        [Authorize]
        [Route("reports/SaveAhuTestDirectDriveReport")]
        [HttpPost]
        public HttpResponseMessage SaveAhuTestDirectDriveReport([FromBody] AhuTestDirectDriveReportViewModel oAhuTestDirectDriveReport)
        {
            try
            {
                //Save report header data and create systemreportid.
                tblSystemReport oSystemReport = new tblSystemReport();
                oSystemReport.SystemReportID = Guid.NewGuid();
                oSystemReport.SystemID_fk = oAhuTestDirectDriveReport.Header.SystemID;
                oSystemReport.TemplateID_fk = oAhuTestDirectDriveReport.Header.TemplateID;
                oSystemReport.Name = oAhuTestDirectDriveReport.Header.Name;
                oSystemReport.TestReference = oAhuTestDirectDriveReport.Header.TestReference;
                oSystemReport.Status = oAhuTestDirectDriveReport.Header.Status;
                oSystemReport.DateTime = oAhuTestDirectDriveReport.Header.Date;
                context.tblSystemReports.Add(oSystemReport);
                context.SaveChanges();
                var SysReportId = oSystemReport.SystemReportID;

                //Save report detail data along with the created systemreportid.
                Rpt_AhuTestDirectDrive oRptAhuTestDirectDrive = new Rpt_AhuTestDirectDrive();
                oRptAhuTestDirectDrive.ReportID = Guid.NewGuid();
                oRptAhuTestDirectDrive.SystemReportID_fk = SysReportId;
                oRptAhuTestDirectDrive.LocationAHU = oAhuTestDirectDriveReport.LocationAHU;
                oRptAhuTestDirectDrive.ManufactureAHU = oAhuTestDirectDriveReport.ManufactureAHU;
                oRptAhuTestDirectDrive.ModelNoAHU = oAhuTestDirectDriveReport.ModelNoAHU;
                oRptAhuTestDirectDrive.SerialNoAHU = oAhuTestDirectDriveReport.SerialNoAHU;
                oRptAhuTestDirectDrive.CapacityAHU = oAhuTestDirectDriveReport.CapacityAHU;
                oRptAhuTestDirectDrive.DesignAirLsAHU = oAhuTestDirectDriveReport.DesignAirLsAHU;
                oRptAhuTestDirectDrive.DesignAirPaAHU = oAhuTestDirectDriveReport.DesignAirPaAHU;
                oRptAhuTestDirectDrive.MediaAHU = oAhuTestDirectDriveReport.MediaAHU;
                oRptAhuTestDirectDrive.FanTypeAHU = oAhuTestDirectDriveReport.FanTypeAHU;
                oRptAhuTestDirectDrive.FanArrangementAHU = oAhuTestDirectDriveReport.FanArrangementAHU;
                oRptAhuTestDirectDrive.MSSBNameAHU = oAhuTestDirectDriveReport.MSSBNameAHU;
                oRptAhuTestDirectDrive.MSSBLocationAHU = oAhuTestDirectDriveReport.MSSBLocationAHU;
                oRptAhuTestDirectDrive.MotorMakeMD = oAhuTestDirectDriveReport.MotorMakeMD;
                oRptAhuTestDirectDrive.FrameMD = oAhuTestDirectDriveReport.FrameMD;
                oRptAhuTestDirectDrive.ElectricalMD = oAhuTestDirectDriveReport.ElectricalMD;
                oRptAhuTestDirectDrive.MotorPowerMD = oAhuTestDirectDriveReport.MotorPowerMD;
                oRptAhuTestDirectDrive.FullLoadMD = oAhuTestDirectDriveReport.FullLoadMD;
                oRptAhuTestDirectDrive.PoleMD = oAhuTestDirectDriveReport.PoleMD;
                oRptAhuTestDirectDrive.PoleRPMMD = oAhuTestDirectDriveReport.PoleRPMMD;
                oRptAhuTestDirectDrive.OverloadMakeMS = oAhuTestDirectDriveReport.OverloadMakeMS;
                oRptAhuTestDirectDrive.OverloadRangeMS = oAhuTestDirectDriveReport.OverloadRangeMS;
                oRptAhuTestDirectDrive.VSDMakeMS = oAhuTestDirectDriveReport.VSDMakeMS;
                oRptAhuTestDirectDrive.VSDRangeMS = oAhuTestDirectDriveReport.VSDRangeMS;
                oRptAhuTestDirectDrive.FilterType1 = oAhuTestDirectDriveReport.FilterType1;
                oRptAhuTestDirectDrive.FilterSize1 = oAhuTestDirectDriveReport.FilterSize1;
                oRptAhuTestDirectDrive.NoFilters1 = oAhuTestDirectDriveReport.NoFilters1;
                oRptAhuTestDirectDrive.FilterType2 = oAhuTestDirectDriveReport.FilterType2;
                oRptAhuTestDirectDrive.FilterSize2 = oAhuTestDirectDriveReport.FilterSize2;
                oRptAhuTestDirectDrive.NoFilters2 = oAhuTestDirectDriveReport.NoFilters2;
                oRptAhuTestDirectDrive.SupplyAirDES = oAhuTestDirectDriveReport.SupplyAirDES;
                oRptAhuTestDirectDrive.ReturnAirDES = oAhuTestDirectDriveReport.ReturnAirDES;
                oRptAhuTestDirectDrive.OutsideAirDES = oAhuTestDirectDriveReport.OutsideAirDES;
                oRptAhuTestDirectDrive.SupplyAirFIN = oAhuTestDirectDriveReport.SupplyAirFIN;
                oRptAhuTestDirectDrive.ReturnAirFIN = oAhuTestDirectDriveReport.ReturnAirFIN;
                oRptAhuTestDirectDrive.OutsideAirFIN = oAhuTestDirectDriveReport.OutsideAirFIN;
                oRptAhuTestDirectDrive.TotalStaticDES = oAhuTestDirectDriveReport.TotalStaticDES;
                oRptAhuTestDirectDrive.TotalStaticFIN = oAhuTestDirectDriveReport.TotalStaticFIN;
                oRptAhuTestDirectDrive.SucStaticFIN = oAhuTestDirectDriveReport.SucStaticFIN;
                oRptAhuTestDirectDrive.DisStaticFIN = oAhuTestDirectDriveReport.DisStaticFIN;
                oRptAhuTestDirectDrive.FilterDiffFIN = oAhuTestDirectDriveReport.FilterDiffFIN;
                oRptAhuTestDirectDrive.PchwDiffFIN = oAhuTestDirectDriveReport.PchwDiffFIN;
                oRptAhuTestDirectDrive.ScheDiffFIN = oAhuTestDirectDriveReport.ScheDiffFIN;
                oRptAhuTestDirectDrive.VoltageDES = oAhuTestDirectDriveReport.VoltageDES;
                oRptAhuTestDirectDrive.VoltageFIN = oAhuTestDirectDriveReport.VoltageFIN;
                oRptAhuTestDirectDrive.AmpsDES = oAhuTestDirectDriveReport.AmpsDES;
                oRptAhuTestDirectDrive.AmpsFIN = oAhuTestDirectDriveReport.AmpsFIN;
                oRptAhuTestDirectDrive.Instrument1 = oAhuTestDirectDriveReport.Instrument1;
                oRptAhuTestDirectDrive.Instrument2 = oAhuTestDirectDriveReport.Instrument2;
                oRptAhuTestDirectDrive.Model1 = oAhuTestDirectDriveReport.Model1;
                oRptAhuTestDirectDrive.Model2 = oAhuTestDirectDriveReport.Model2;
                oRptAhuTestDirectDrive.Serial1 = oAhuTestDirectDriveReport.Serial1;
                oRptAhuTestDirectDrive.Serial2 = oAhuTestDirectDriveReport.Serial2;
                oRptAhuTestDirectDrive.FuseDetailsMDPH = oAhuTestDirectDriveReport.FuseDetailsMDPH;
                oRptAhuTestDirectDrive.OverloadRangeMS2 = oAhuTestDirectDriveReport.OverloadRangeMS2;
                oRptAhuTestDirectDrive.OverloadRangeMS3 = oAhuTestDirectDriveReport.OverloadRangeMS3;
                oRptAhuTestDirectDrive.FilterSize1B = oAhuTestDirectDriveReport.FilterSize1B;
                oRptAhuTestDirectDrive.FilterSize1C = oAhuTestDirectDriveReport.FilterSize1C;
                oRptAhuTestDirectDrive.FilterSize2B = oAhuTestDirectDriveReport.FilterSize2B;
                oRptAhuTestDirectDrive.FilterSize2C = oAhuTestDirectDriveReport.FilterSize2C;
                oRptAhuTestDirectDrive.AmpsHzDes = oAhuTestDirectDriveReport.AmpsHzDes;
                oRptAhuTestDirectDrive.AmpsHzFin = oAhuTestDirectDriveReport.AmpsHzFin;
                oRptAhuTestDirectDrive.BMSFanSpeed = oAhuTestDirectDriveReport.BMSFanSpeed;
                oRptAhuTestDirectDrive.BMSVelocity = oAhuTestDirectDriveReport.BMSVelocity;
                oRptAhuTestDirectDrive.BMSPressure = oAhuTestDirectDriveReport.BMSPressure;
                oRptAhuTestDirectDrive.BMSOther = oAhuTestDirectDriveReport.BMSOther;
                oRptAhuTestDirectDrive.SupplyAirPER = oAhuTestDirectDriveReport.SupplyAirPER;
                oRptAhuTestDirectDrive.ReturnAirPER = oAhuTestDirectDriveReport.ReturnAirPER;
                oRptAhuTestDirectDrive.OutsideAirPER = oAhuTestDirectDriveReport.OutsideAirPER;
                oRptAhuTestDirectDrive.HeadBuilding = oAhuTestDirectDriveReport.HeadBuilding;
                oRptAhuTestDirectDrive.WitnessName = oAhuTestDirectDriveReport.WitnessName;
                oRptAhuTestDirectDrive.WitnessDate = oAhuTestDirectDriveReport.WitnessDate;
                oRptAhuTestDirectDrive.WitnessSignature = oAhuTestDirectDriveReport.WitnessSignature;
                oRptAhuTestDirectDrive.FilterSize3 = oAhuTestDirectDriveReport.FilterSize3;
                oRptAhuTestDirectDrive.FilterSize3B = oAhuTestDirectDriveReport.FilterSize3B;
                oRptAhuTestDirectDrive.FilterSize3C = oAhuTestDirectDriveReport.FilterSize3C;
                oRptAhuTestDirectDrive.FilterSize4 = oAhuTestDirectDriveReport.FilterSize4;
                oRptAhuTestDirectDrive.FilterSize4B = oAhuTestDirectDriveReport.FilterSize4B;
                oRptAhuTestDirectDrive.FilterSize4C = oAhuTestDirectDriveReport.FilterSize4C;
                oRptAhuTestDirectDrive.NoFilters3 = oAhuTestDirectDriveReport.NoFilters3;
                oRptAhuTestDirectDrive.NoFilters4 = oAhuTestDirectDriveReport.NoFilters4;
                oRptAhuTestDirectDrive.BMSFanSpeedType = oAhuTestDirectDriveReport.BMSFanSpeedType;
                oRptAhuTestDirectDrive.BMSVelocityType = oAhuTestDirectDriveReport.BMSVelocityType;
                context.Rpt_AhuTestDirectDrive.Add(oRptAhuTestDirectDrive);
                context.SaveChanges();

                //Save report comments informationn.
                List<CommCommentViewModel> lstComments = new List<CommCommentViewModel>();
                lstComments = oAhuTestDirectDriveReport.Comments;
                foreach (var item in lstComments)
                {
                    tblSystemReportComment oAhuTestDirectDriveComment = new tblSystemReportComment();
                    oAhuTestDirectDriveComment.ID = Guid.NewGuid();
                    oAhuTestDirectDriveComment.SysRepID_fk = SysReportId;
                    oAhuTestDirectDriveComment.TechnicianID_fk = item.TechnicianID_fk;
                    oAhuTestDirectDriveComment.DateCreated = item.DateCreated;
                    oAhuTestDirectDriveComment.Comments = item.Comments;
                    context.tblSystemReportComments.Add(oAhuTestDirectDriveComment);
                    context.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
        }


        /// <summary>
        /// Get Fan Test report details for given system report id.
        /// </summary>
        /// <returns>
        /// The oFanTestReport object which contains all the information.
        /// </returns>        
        [Authorize]
        [Route("reports/GetFanTestReportBySystemReportID")]
        [HttpPost]
        public HttpResponseMessage GetFanTestReportBySystemReportID([FromBody] string systemreportid)
        {
            try
            {
                //Get report details from the report table by providing systemreportid.
                var val = Guid.Parse(systemreportid);
                var oFanTestReport = context.Rpt_FanTest.Where(x => x.SystemReportID_fk == val).Select(x => new FanTestReportViewModel
                {
                    ReportID = x.ReportID,
                    SystemReportID_fk = x.SystemReportID_fk.Value,
                    LocationAHU = x.LocationAHU,
                    ManufactureAHU = x.ManufactureAHU,
                    ModelNoAHU = x.ModelNoAHU,
                    SerialNoAHU = x.SerialNoAHU,
                    DesignAirLsAHU = x.DesignAirLsAHU,
                    DesignAirPaAHU = x.DesignAirPaAHU,
                    FanControl = x.FanControl,
                    SupplyExhaust = x.SupplyExhaust,
                    MSSBNameAHU = x.MSSBNameAHU,
                    MSSBLocationAHU = x.MSSBLocationAHU,
                    MotorMakeMD = x.MotorMakeMD,
                    FrameMD = x.FrameMD,
                    ElectricalMD = x.ElectricalMD,
                    MotorPowerMD = x.MotorPowerMD,
                    FullLoadMD = x.FullLoadMD,
                    PoleMD = x.PoleMD,
                    PoleRPMMD = x.PoleRPMMD,
                    FuseDetailsMD = x.FuseDetailsMD,
                    FanPulleyFan = x.FanPulleyFan,
                    FanBushFan = x.FanBushFan,
                    FanBoreFan = x.FanBoreFan,
                    MotorPulleyFan = x.MotorPulleyFan,
                    MotorBushFan = x.MotorBushFan,
                    MotorBoreFan = x.MotorBoreFan,
                    BeltsMakeFan = x.BeltsMakeFan,
                    BeltsNoFan = x.BeltsNoFan,
                    BeltsSizeFan = x.BeltsSizeFan,
                    ShaftCentreFan = x.ShaftCentreFan,
                    OverloadMakeMS = x.OverloadMakeMS,
                    OverloadRangeMS = x.OverloadRangeMS,
                    VSDMakeMS = x.VSDMakeMS,
                    VSDRangeMS = x.VSDRangeMS,
                    FilterType1 = x.FilterType1,
                    FilterSize1 = x.FilterSize1,
                    NoFilters1 = x.NoFilters1,
                    FilterType2 = x.FilterType2,
                    FilterSize2 = x.FilterSize2,
                    NoFilters2 = x.NoFilters2,
                    TotalAirDES = x.TotalAirDES,
                    TotalStaticDES = x.TotalStaticDES,
                    SucStaticDES = x.SucStaticDES,
                    DisStaticDES = x.DisStaticDES,
                    FilterDiffDES = x.FilterDiffDES,
                    VoltageDES = x.VoltageDES,
                    AmpsDES = x.AmpsDES,
                    TotalAirFIN = x.TotalAirFIN,
                    TotalStaticFIN = x.TotalStaticFIN,
                    SucStaticFIN = x.SucStaticFIN,
                    DisStaticFIN = x.DisStaticFIN,
                    FilterDiffFIN = x.FilterDiffFIN,
                    VoltageFIN = x.VoltageFIN,
                    AmpsFIN = x.AmpsFIN,
                    VSDFinalHz = x.VSDFinalHz,
                    StaticPressureRef = x.StaticPressureRef,
                    Instrument1 = x.Instrument1,
                    Make1 = x.Make1,
                    Serial1 = x.Serial1,
                    Instrument2 = x.Instrument2,
                    Make2 = x.Make2,
                    Serial2 = x.Serial2,
                    TechnicianComments = x.TechnicianComments,
                    ICAComments = x.ICAComments,
                    FilterSize1B = x.FilterSize1B,
                    FilterSize1C = x.FilterSize1C,
                    FilterSize2B = x.FilterSize2B,
                    FilterSize2C = x.FilterSize2C,
                    FuseDetailsMDPH = x.FuseDetailsMDPH,
                    OverloadRangeMS2 = x.OverloadRangeMS2,
                    OverloadRangeMS3 = x.OverloadRangeMS3,
                    ShowSucStatic = x.ShowSucStatic,
                    ShowDisStatic = x.ShowDisStatic,
                    ShowFilterDiff = x.ShowFilterDiff,
                    HeadBuilding = x.HeadBuilding,
                    HeadService = x.HeadService,
                    HeadCustomer = x.HeadCustomer,
                    BMSFanSpeed = x.BMSFanSpeed,
                    BMSVelocity = x.BMSVelocity,
                    BMSPressure = x.BMSPressure,
                    BMSOther = x.BMSOther,
                    TotalAirPER = x.TotalAirPER,
                    WitnessName = x.WitnessName,
                    WitnessDate = x.WitnessDate,
                    WitnessSignature = x.WitnessSignature,
                    BMSFanSpeedType = x.BMSFanSpeedType,
                    BMSVelocityType = x.BMSVelocityType,
                }
                ).FirstOrDefault();

                //Get report header information and assign them to oAhuTestReport object.
                var oReportHeader = context.tblSystemReports.Where(x => x.SystemReportID == val).Select(x => new CommSystemReportViewModel
                {
                    SystemReportID = x.SystemReportID,
                    SystemID = x.SystemID_fk.Value,
                    TemplateID = x.TemplateID_fk.Value,
                    Name = x.Name,
                    TestReference = x.TestReference,
                    Status = x.Status.Value,
                    Date = x.DateTime.Value,
                }
                ).FirstOrDefault();
                oFanTestReport.Header = oReportHeader;

                //Get report comments and assign them to the oAhuTestReport object.
                var oReportComments = context.tblSystemReportComments.Where(x => x.SysRepID_fk == val).Select(x => new CommCommentViewModel
                {
                    ID = x.ID,
                    SysRepID_fk = x.SysRepID_fk,
                    TechnicianID_fk = x.TechnicianID_fk,
                    DateCreated = x.DateCreated,
                    Comments = x.Comments,
                }
                ).ToList();
                oFanTestReport.Comments = oReportComments;

                return Request.CreateResponse(HttpStatusCode.OK, oFanTestReport);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }

        }

        /// <summary>
        /// Save Fan Test report data into the relavant tables.
        /// It should provide systemid and templateid along with the report related data. 
        /// </summary>
        /// <returns>
        /// The Satatus based on the success or failed.
        /// </returns>
        [Authorize]
        [Route("reports/SaveFanTestReport")]
        [HttpPost]
        public HttpResponseMessage SaveFanTestReport([FromBody] FanTestReportViewModel oFanTestReport)
        {
            try
            {
                //Save report header information.
                tblSystemReport oSystemReport = new tblSystemReport();
                oSystemReport.SystemReportID = Guid.NewGuid();
                oSystemReport.SystemID_fk = oFanTestReport.Header.SystemID;
                oSystemReport.TemplateID_fk = oFanTestReport.Header.TemplateID;
                oSystemReport.Name = oFanTestReport.Header.Name;
                oSystemReport.TestReference = oFanTestReport.Header.TestReference;
                oSystemReport.Status = oFanTestReport.Header.Status;
                oSystemReport.DateTime = oFanTestReport.Header.Date;
                context.tblSystemReports.Add(oSystemReport);
                context.SaveChanges();
                var SysReportId = oSystemReport.SystemReportID;

                //Save report details information.
                Rpt_FanTest oRptFanTest = new Rpt_FanTest();
                oRptFanTest.ReportID = Guid.NewGuid();
                oRptFanTest.SystemReportID_fk = SysReportId;
                oRptFanTest.LocationAHU = oFanTestReport.LocationAHU;
                oRptFanTest.ManufactureAHU = oFanTestReport.ManufactureAHU;
                oRptFanTest.ModelNoAHU = oFanTestReport.ModelNoAHU;
                oRptFanTest.SerialNoAHU = oFanTestReport.SerialNoAHU;
                oRptFanTest.DesignAirLsAHU = oFanTestReport.DesignAirLsAHU;
                oRptFanTest.DesignAirPaAHU = oFanTestReport.DesignAirPaAHU;
                oRptFanTest.FanControl = oFanTestReport.FanControl;
                oRptFanTest.SupplyExhaust = oFanTestReport.SupplyExhaust;
                oRptFanTest.MSSBNameAHU = oFanTestReport.MSSBNameAHU;
                oRptFanTest.MSSBLocationAHU = oFanTestReport.MSSBLocationAHU;
                oRptFanTest.MotorMakeMD = oFanTestReport.MotorMakeMD;
                oRptFanTest.FrameMD = oFanTestReport.FrameMD;
                oRptFanTest.ElectricalMD = oFanTestReport.ElectricalMD;
                oRptFanTest.MotorPowerMD = oFanTestReport.MotorPowerMD;
                oRptFanTest.FullLoadMD = oFanTestReport.FullLoadMD;
                oRptFanTest.PoleMD = oFanTestReport.PoleMD;
                oRptFanTest.PoleRPMMD = oFanTestReport.PoleRPMMD;
                oRptFanTest.FuseDetailsMD = oFanTestReport.FuseDetailsMD;
                oRptFanTest.FanPulleyFan = oFanTestReport.FanPulleyFan;
                oRptFanTest.FanBushFan = oFanTestReport.FanBushFan;
                oRptFanTest.FanBoreFan = oFanTestReport.FanBoreFan;
                oRptFanTest.MotorPulleyFan = oFanTestReport.MotorPulleyFan;
                oRptFanTest.MotorBushFan = oFanTestReport.MotorBushFan;
                oRptFanTest.MotorBoreFan = oFanTestReport.MotorBoreFan;
                oRptFanTest.BeltsMakeFan = oFanTestReport.BeltsMakeFan;
                oRptFanTest.BeltsNoFan = oFanTestReport.BeltsNoFan;
                oRptFanTest.BeltsSizeFan = oFanTestReport.BeltsSizeFan;
                oRptFanTest.ShaftCentreFan = oFanTestReport.ShaftCentreFan;
                oRptFanTest.OverloadMakeMS = oFanTestReport.OverloadMakeMS;
                oRptFanTest.OverloadRangeMS = oFanTestReport.OverloadRangeMS;
                oRptFanTest.VSDMakeMS = oFanTestReport.VSDMakeMS;
                oRptFanTest.VSDRangeMS = oFanTestReport.VSDRangeMS;
                oRptFanTest.FilterType1 = oFanTestReport.FilterType1;
                oRptFanTest.FilterSize1 = oFanTestReport.FilterSize1;
                oRptFanTest.NoFilters1 = oFanTestReport.NoFilters1;
                oRptFanTest.FilterType2 = oFanTestReport.FilterType2;
                oRptFanTest.FilterSize2 = oFanTestReport.FilterSize2;
                oRptFanTest.NoFilters2 = oFanTestReport.NoFilters2;
                oRptFanTest.TotalAirDES = oFanTestReport.TotalAirDES;
                oRptFanTest.TotalStaticDES = oFanTestReport.TotalStaticDES;
                oRptFanTest.SucStaticDES = oFanTestReport.SucStaticDES;
                oRptFanTest.DisStaticDES = oFanTestReport.DisStaticDES;
                oRptFanTest.FilterDiffDES = oFanTestReport.FilterDiffDES;
                oRptFanTest.VoltageDES = oFanTestReport.VoltageDES;
                oRptFanTest.AmpsDES = oFanTestReport.AmpsDES;
                oRptFanTest.TotalAirFIN = oFanTestReport.TotalAirFIN;
                oRptFanTest.TotalStaticFIN = oFanTestReport.TotalStaticFIN;
                oRptFanTest.SucStaticFIN = oFanTestReport.SucStaticFIN;
                oRptFanTest.DisStaticFIN = oFanTestReport.DisStaticFIN;
                oRptFanTest.FilterDiffFIN = oFanTestReport.FilterDiffFIN;
                oRptFanTest.VoltageFIN = oFanTestReport.VoltageFIN;
                oRptFanTest.AmpsFIN = oFanTestReport.AmpsFIN;
                oRptFanTest.VSDFinalHz = oFanTestReport.VSDFinalHz;
                oRptFanTest.StaticPressureRef = oFanTestReport.StaticPressureRef;
                oRptFanTest.Instrument1 = oFanTestReport.Instrument1;
                oRptFanTest.Make1 = oFanTestReport.Make1;
                oRptFanTest.Serial1 = oFanTestReport.Serial1;
                oRptFanTest.Instrument2 = oFanTestReport.Instrument2;
                oRptFanTest.Make2 = oFanTestReport.Make2;
                oRptFanTest.Serial2 = oFanTestReport.Serial2;
                oRptFanTest.TechnicianComments = oFanTestReport.TechnicianComments;
                oRptFanTest.ICAComments = oFanTestReport.ICAComments;
                oRptFanTest.FilterSize1B = oFanTestReport.FilterSize1B;
                oRptFanTest.FilterSize1C = oFanTestReport.FilterSize1C;
                oRptFanTest.FilterSize2B = oFanTestReport.FilterSize2B;
                oRptFanTest.FilterSize2C = oFanTestReport.FilterSize2C;
                oRptFanTest.FuseDetailsMDPH = oFanTestReport.FuseDetailsMDPH;
                oRptFanTest.OverloadRangeMS2 = oFanTestReport.OverloadRangeMS2;
                oRptFanTest.OverloadRangeMS3 = oFanTestReport.OverloadRangeMS3;
                oRptFanTest.ShowSucStatic = oFanTestReport.ShowSucStatic;
                oRptFanTest.ShowDisStatic = oFanTestReport.ShowDisStatic;
                oRptFanTest.ShowFilterDiff = oFanTestReport.ShowFilterDiff;
                oRptFanTest.HeadBuilding = oFanTestReport.HeadBuilding;
                oRptFanTest.HeadService = oFanTestReport.HeadService;
                oRptFanTest.HeadCustomer = oFanTestReport.HeadCustomer;
                oRptFanTest.BMSFanSpeed = oFanTestReport.BMSFanSpeed;
                oRptFanTest.BMSVelocity = oFanTestReport.BMSVelocity;
                oRptFanTest.BMSPressure = oFanTestReport.BMSPressure;
                oRptFanTest.BMSOther = oFanTestReport.BMSOther;
                oRptFanTest.TotalAirPER = oFanTestReport.TotalAirPER;
                oRptFanTest.WitnessName = oFanTestReport.WitnessName;
                oRptFanTest.WitnessDate = oFanTestReport.WitnessDate;
                oRptFanTest.WitnessSignature = oFanTestReport.WitnessSignature;
                oRptFanTest.BMSFanSpeedType = oFanTestReport.BMSFanSpeedType;
                oRptFanTest.BMSVelocityType = oFanTestReport.BMSVelocityType;                
                context.Rpt_FanTest.Add(oRptFanTest);
                context.SaveChanges();

                //Save report comments information.
                List<CommCommentViewModel> lstComments = new List<CommCommentViewModel>();
                lstComments = oFanTestReport.Comments;
                foreach (var item in lstComments)
                {
                    tblSystemReportComment oAhuTestComment = new tblSystemReportComment();
                    oAhuTestComment.ID = Guid.NewGuid();
                    oAhuTestComment.SysRepID_fk = SysReportId;
                    oAhuTestComment.TechnicianID_fk = item.TechnicianID_fk;
                    oAhuTestComment.DateCreated = item.DateCreated;
                    oAhuTestComment.Comments = item.Comments;
                    context.tblSystemReportComments.Add(oAhuTestComment);
                    context.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
        }

        /// <summary>
        /// Get Fan Test Direct Drive report details for given system report id.
        /// </summary>
        /// <returns>
        /// The oFanTestDirectDriveReport object which contains all the information.
        /// </returns>
        [Authorize]
        [Route("reports/GetFanTestDirectDriveReportBySystemReportID")]
        [HttpPost]
        public HttpResponseMessage GetFanTestDirectDriveReportBySystemReportID([FromBody] string systemreportid)
        {
            try
            {
                //Get report details information by providing systemreportid
                var val = Guid.Parse(systemreportid);
                var oFanTestDirectDriveReport = context.Rpt_FanTestDirectDrive.Where(x => x.SystemReportID_fk == val).Select(x => new FanTestDirectDriveReportViewModel
                {
                    ReportID = x.ReportID,
                    SystemReportID_fk = x.SystemReportID_fk.Value,
                    LocationAHU = x.LocationAHU,
                    ManufactureAHU = x.ManufactureAHU,
                    ModelNoAHU = x.ModelNoAHU,
                    SerialNoAHU = x.SerialNoAHU,
                    DesignAirLsAHU = x.DesignAirLsAHU,
                    DesignAirPaAHU = x.DesignAirPaAHU,
                    FanControl = x.FanControl,
                    SupplyExhaust = x.SupplyExhaust,
                    MSSBNameAHU = x.MSSBNameAHU,
                    MSSBLocationAHU = x.MSSBLocationAHU,
                    MotorMakeMD = x.MotorMakeMD,
                    FrameMD = x.FrameMD,
                    ElectricalMD = x.ElectricalMD,
                    MotorPowerMD = x.MotorPowerMD,
                    FullLoadMD = x.FullLoadMD,
                    PoleMD = x.PoleMD,
                    PoleRPMMD = x.PoleRPMMD,
                    OverloadMakeMS = x.OverloadMakeMS,
                    OverloadRangeMS = x.OverloadRangeMS,
                    VSDMakeMS = x.VSDMakeMS,
                    VSDRangeMS = x.VSDRangeMS,
                    FilterType1 = x.FilterType1,
                    FilterSize1 = x.FilterSize1,
                    NoFilters1 = x.NoFilters1,
                    FilterType2 = x.FilterType2,
                    FilterSize2 = x.FilterSize2,
                    NoFilters2 = x.NoFilters2,
                    TotalAirDES = x.TotalAirDES,
                    TotalAirFIN = x.TotalAirFIN,
                    TotalStaticDES = x.TotalStaticDES,
                    TotalStaticFIN = x.TotalStaticFIN,
                    SucStaticDES = x.SucStaticDES,
                    SucStaticFIN = x.SucStaticFIN,
                    DisStaticFIN = x.DisStaticFIN,
                    FilterDiffFIN = x.FilterDiffFIN,
                    VoltageDES = x.VoltageDES,
                    VoltageFIN = x.VoltageFIN,
                    AmpsDES = x.AmpsDES,
                    AmpsFIN = x.AmpsFIN,
                    VSDFinalHz = x.VSDFinalHz,
                    StaticPressureRef = x.StaticPressureRef,
                    Instrument1 = x.Instrument1,
                    Instrument2 = x.Instrument2,
                    Make1 = x.Make1,
                    Make2 = x.Make2,
                    Serial1 = x.Serial1,
                    Serial2 = x.Serial2,
                    FilterSize1B = x.FilterSize1B,
                    FilterSize1C = x.FilterSize1C,
                    FilterSize2B = x.FilterSize2B,
                    FilterSize2C = x.FilterSize2C,
                    FuseDetailsMDPH = x.FuseDetailsMDPH,
                    OverloadRangeMS2 = x.OverloadRangeMS2,
                    OverloadRangeMS3 = x.OverloadRangeMS3,
                    BMSFanSpeed = x.BMSFanSpeed,
                    BMSVelocity = x.BMSVelocity,
                    BMSPressure = x.BMSPressure,
                    BMSOther = x.BMSOther,
                    TotalAirPER = x.TotalAirPER,
                    HeadBuilding = x.HeadBuilding,
                    WitnessName = x.WitnessName,
                    WitnessDate = x.WitnessDate,
                    WitnessSignature = x.WitnessSignature,
                    BMSFanSpeedType = x.BMSFanSpeedType,
                    BMSVelocityType = x.BMSVelocityType,
                }
                ).FirstOrDefault();

                //Get report header information and add them to the oFanTestDirectDriveReport object.
                var oReportHeader = context.tblSystemReports.Where(x => x.SystemReportID == val).Select(x => new CommSystemReportViewModel
                {
                    SystemReportID = x.SystemReportID,
                    SystemID = x.SystemID_fk.Value,
                    TemplateID = x.TemplateID_fk.Value,
                    Name = x.Name,
                    TestReference = x.TestReference,
                    Status = x.Status.Value,
                    Date = x.DateTime.Value,
                }
                ).FirstOrDefault();
                oFanTestDirectDriveReport.Header = oReportHeader;

                //Get report comments information and add them to the oFanTestDirectDriveReport object.
                var oReportComments = context.tblSystemReportComments.Where(x => x.SysRepID_fk == val).Select(x => new CommCommentViewModel
                {
                    ID = x.ID,
                    SysRepID_fk = x.SysRepID_fk,
                    TechnicianID_fk = x.TechnicianID_fk,
                    DateCreated = x.DateCreated,
                    Comments = x.Comments,
                }
                ).ToList();
                oFanTestDirectDriveReport.Comments = oReportComments;

                return Request.CreateResponse(HttpStatusCode.OK, oFanTestDirectDriveReport);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }

        }

        /// <summary>
        /// Save Fan Test Direct Drive report data into the relavant tables. 
        /// It should provide systemid and templateid along with the report related data.
        /// </summary>
        /// <returns>
        /// The Satatus based on the success or failed.
        /// </returns>
        [Authorize]
        [Route("reports/SaveFanTestDirectDriveReport")]
        [HttpPost]
        public HttpResponseMessage SaveFanTestDirectDriveReport([FromBody] FanTestDirectDriveReportViewModel oFanTestDirectDriveReport)
        {
            try
            {
                //Save report header data and create systemreportid.
                tblSystemReport oSystemReport = new tblSystemReport();
                oSystemReport.SystemReportID = Guid.NewGuid();
                oSystemReport.SystemID_fk = oFanTestDirectDriveReport.Header.SystemID;
                oSystemReport.TemplateID_fk = oFanTestDirectDriveReport.Header.TemplateID;
                oSystemReport.Name = oFanTestDirectDriveReport.Header.Name;
                oSystemReport.TestReference = oFanTestDirectDriveReport.Header.TestReference;
                oSystemReport.Status = oFanTestDirectDriveReport.Header.Status;
                oSystemReport.DateTime = oFanTestDirectDriveReport.Header.Date;
                context.tblSystemReports.Add(oSystemReport);
                context.SaveChanges();
                var SysReportId = oSystemReport.SystemReportID;

                //Save report detail data along with the created systemreportid.
                Rpt_FanTestDirectDrive oRptFanTestDirectDrive = new Rpt_FanTestDirectDrive();
                oRptFanTestDirectDrive.ReportID = Guid.NewGuid();
                oRptFanTestDirectDrive.SystemReportID_fk = SysReportId;
                oRptFanTestDirectDrive.LocationAHU = oFanTestDirectDriveReport.LocationAHU;
                oRptFanTestDirectDrive.ManufactureAHU = oFanTestDirectDriveReport.ManufactureAHU;
                oRptFanTestDirectDrive.ModelNoAHU = oFanTestDirectDriveReport.ModelNoAHU;
                oRptFanTestDirectDrive.SerialNoAHU = oFanTestDirectDriveReport.SerialNoAHU;
                oRptFanTestDirectDrive.DesignAirLsAHU = oFanTestDirectDriveReport.DesignAirLsAHU;
                oRptFanTestDirectDrive.DesignAirPaAHU = oFanTestDirectDriveReport.DesignAirPaAHU;
                oRptFanTestDirectDrive.FanControl = oFanTestDirectDriveReport.FanControl;
                oRptFanTestDirectDrive.SupplyExhaust = oFanTestDirectDriveReport.SupplyExhaust;
                oRptFanTestDirectDrive.MSSBNameAHU = oFanTestDirectDriveReport.MSSBNameAHU;
                oRptFanTestDirectDrive.MSSBLocationAHU = oFanTestDirectDriveReport.MSSBLocationAHU;
                oRptFanTestDirectDrive.MotorMakeMD = oFanTestDirectDriveReport.MotorMakeMD;
                oRptFanTestDirectDrive.FrameMD = oFanTestDirectDriveReport.FrameMD;
                oRptFanTestDirectDrive.ElectricalMD = oFanTestDirectDriveReport.ElectricalMD;
                oRptFanTestDirectDrive.MotorPowerMD = oFanTestDirectDriveReport.MotorPowerMD;
                oRptFanTestDirectDrive.FullLoadMD = oFanTestDirectDriveReport.FullLoadMD;
                oRptFanTestDirectDrive.PoleMD = oFanTestDirectDriveReport.PoleMD;
                oRptFanTestDirectDrive.PoleRPMMD = oFanTestDirectDriveReport.PoleRPMMD;
                oRptFanTestDirectDrive.OverloadMakeMS = oFanTestDirectDriveReport.OverloadMakeMS;
                oRptFanTestDirectDrive.OverloadRangeMS = oFanTestDirectDriveReport.OverloadRangeMS;
                oRptFanTestDirectDrive.VSDMakeMS = oFanTestDirectDriveReport.VSDMakeMS;
                oRptFanTestDirectDrive.VSDRangeMS = oFanTestDirectDriveReport.VSDRangeMS;
                oRptFanTestDirectDrive.FilterType1 = oFanTestDirectDriveReport.FilterType1;
                oRptFanTestDirectDrive.FilterSize1 = oFanTestDirectDriveReport.FilterSize1;
                oRptFanTestDirectDrive.NoFilters1 = oFanTestDirectDriveReport.NoFilters1;
                oRptFanTestDirectDrive.FilterType2 = oFanTestDirectDriveReport.FilterType2;
                oRptFanTestDirectDrive.FilterSize2 = oFanTestDirectDriveReport.FilterSize2;
                oRptFanTestDirectDrive.NoFilters2 = oFanTestDirectDriveReport.NoFilters2;
                oRptFanTestDirectDrive.TotalAirDES = oFanTestDirectDriveReport.TotalAirDES;
                oRptFanTestDirectDrive.TotalAirFIN = oFanTestDirectDriveReport.TotalAirFIN;
                oRptFanTestDirectDrive.TotalStaticDES = oFanTestDirectDriveReport.TotalStaticDES;
                oRptFanTestDirectDrive.TotalStaticFIN = oFanTestDirectDriveReport.TotalStaticFIN;
                oRptFanTestDirectDrive.SucStaticDES = oFanTestDirectDriveReport.SucStaticDES;
                oRptFanTestDirectDrive.SucStaticFIN = oFanTestDirectDriveReport.SucStaticFIN;
                oRptFanTestDirectDrive.DisStaticFIN = oFanTestDirectDriveReport.DisStaticFIN;
                oRptFanTestDirectDrive.FilterDiffFIN = oFanTestDirectDriveReport.FilterDiffFIN;
                oRptFanTestDirectDrive.VoltageDES = oFanTestDirectDriveReport.VoltageDES;
                oRptFanTestDirectDrive.VoltageFIN = oFanTestDirectDriveReport.VoltageFIN;
                oRptFanTestDirectDrive.AmpsDES = oFanTestDirectDriveReport.AmpsDES;
                oRptFanTestDirectDrive.AmpsFIN = oFanTestDirectDriveReport.AmpsFIN;
                oRptFanTestDirectDrive.VSDFinalHz = oFanTestDirectDriveReport.VSDFinalHz;
                oRptFanTestDirectDrive.StaticPressureRef = oFanTestDirectDriveReport.StaticPressureRef;
                oRptFanTestDirectDrive.Instrument1 = oFanTestDirectDriveReport.Instrument1;
                oRptFanTestDirectDrive.Instrument2 = oFanTestDirectDriveReport.Instrument2;
                oRptFanTestDirectDrive.Make1 = oFanTestDirectDriveReport.Make1;
                oRptFanTestDirectDrive.Make2 = oFanTestDirectDriveReport.Make2;
                oRptFanTestDirectDrive.Serial1 = oFanTestDirectDriveReport.Serial1;
                oRptFanTestDirectDrive.Serial2 = oFanTestDirectDriveReport.Serial2;
                oRptFanTestDirectDrive.FilterSize1B = oFanTestDirectDriveReport.FilterSize1B;
                oRptFanTestDirectDrive.FilterSize1C = oFanTestDirectDriveReport.FilterSize1C;
                oRptFanTestDirectDrive.FilterSize2B = oFanTestDirectDriveReport.FilterSize2B;
                oRptFanTestDirectDrive.FilterSize2C = oFanTestDirectDriveReport.FilterSize2C;
                oRptFanTestDirectDrive.FuseDetailsMDPH = oFanTestDirectDriveReport.FuseDetailsMDPH;
                oRptFanTestDirectDrive.OverloadRangeMS2 = oFanTestDirectDriveReport.OverloadRangeMS2;
                oRptFanTestDirectDrive.OverloadRangeMS3 = oFanTestDirectDriveReport.OverloadRangeMS3;
                oRptFanTestDirectDrive.BMSFanSpeed = oFanTestDirectDriveReport.BMSFanSpeed;
                oRptFanTestDirectDrive.BMSVelocity = oFanTestDirectDriveReport.BMSVelocity;
                oRptFanTestDirectDrive.BMSPressure = oFanTestDirectDriveReport.BMSPressure;
                oRptFanTestDirectDrive.BMSOther = oFanTestDirectDriveReport.BMSOther;
                oRptFanTestDirectDrive.TotalAirPER = oFanTestDirectDriveReport.TotalAirPER;
                oRptFanTestDirectDrive.HeadBuilding = oFanTestDirectDriveReport.HeadBuilding;
                oRptFanTestDirectDrive.WitnessName = oFanTestDirectDriveReport.WitnessName;
                oRptFanTestDirectDrive.WitnessDate = oFanTestDirectDriveReport.WitnessDate;
                oRptFanTestDirectDrive.WitnessSignature = oFanTestDirectDriveReport.WitnessSignature;
                oRptFanTestDirectDrive.BMSFanSpeedType = oFanTestDirectDriveReport.BMSFanSpeedType;
                oRptFanTestDirectDrive.BMSVelocityType = oFanTestDirectDriveReport.BMSVelocityType;
                context.Rpt_FanTestDirectDrive.Add(oRptFanTestDirectDrive);
                context.SaveChanges();

                //Save report comments informationn.
                List<CommCommentViewModel> lstComments = new List<CommCommentViewModel>();
                lstComments = oFanTestDirectDriveReport.Comments;
                foreach (var item in lstComments)
                {
                    tblSystemReportComment oAhuTestDirectDriveComment = new tblSystemReportComment();
                    oAhuTestDirectDriveComment.ID = Guid.NewGuid();
                    oAhuTestDirectDriveComment.SysRepID_fk = SysReportId;
                    oAhuTestDirectDriveComment.TechnicianID_fk = item.TechnicianID_fk;
                    oAhuTestDirectDriveComment.DateCreated = item.DateCreated;
                    oAhuTestDirectDriveComment.Comments = item.Comments;
                    context.tblSystemReportComments.Add(oAhuTestDirectDriveComment);
                    context.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception Ex)
            {
                return Request.CreateResponse(Ex.Message);
            }
        }


        // GET: api/Reports
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Reports/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Reports
        //[Authorize]
        [Route("reports/test")]
        public void Post([FromBody]string value)
        {
            int a = 2;
        }

        // PUT: api/Reports/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Reports/5
        public void Delete(int id)
        {
        }
    }
}
