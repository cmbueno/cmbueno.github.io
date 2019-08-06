using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using APP1.Helpers;

namespace APP1.Models
{


    public class DetailsReportRequest_Model
#region Model's Properties
    {
        [Display(Name = "Request ID: ")]
        public string RequestID { get; set; }
        [Display(Name = "User's Name: ")]
        public string UserName { get; set; }
        [Display(Name = "Time Stamp: ")]
        public string TimeStamp { get; set; }
        [Display(Name = "Report Type: ")]
        public string ReportType { get; set; }
        [Display(Name = "Report Status: ")]
        public string ReportStatus { get; set; }
        [Display(Name = "Report Needed By: ")]
        public string RepNeededBy { get; set; }
        [Display(Name = "Current Duty Station: ")]
        public string CurrDutyStation { get; set; }
        [Display(Name = "Run Frequency: ")]
        public string RunFrequency { get; set; }
        [Display(Name = "What data do you need?: ")]
        public string WhatYouNeed { get; set; }
        [Display(Name = "What are you trying to accomplish with this data?: ")]
        public string WhyYouNeed { get; set; }
        [Display(Name = "Fields Required on Report: ")]
        public string FieldsRequired { get; set; }
        [Display(Name = "Sort/Display Criteria: ")]
        public string SortDispCriteria { get; set; }
        [Display(Name = "Disaster Type(s): ")]
        public string DisasterType { get; set; }
        [Display(Name = "Disaster Number(s): ")]
        public string DisasterNo { get; set; }
        [Display(Name = "Preferred Delivery Method: ")]
        public string PrefDelivery { get; set; }
        [Display(Name = "User Comments: ")]
        public string UserComments { get; set; }

        public string UserID { get; set; }
        public string User_NM { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserDateCreated { get; set; }
        public string UserEmail { get; set; }

        [Display(Name = "*Staff Priority Level: ")]
        [Required(ErrorMessage = "Staff Priority Level is a required field")]
        public string StaffPriorityLevel { get; set; }
        public IEnumerable<SelectListItem> StaffPrioLevel_List { get; set; }

        [Display(Name = "*Report Staff: ")]
        [Required(ErrorMessage = "Report Staff is a required field")]
        public string ReportStaff { get; set; }        
        public IEnumerable<SelectListItem> ReportStaff_List { get; set; }

        [Display(Name = "*Admin/ Report Status: ")]
        [Required(ErrorMessage = "Admin. Status is a required field")]
        public string AdminStatus { get; set; }        
        public IEnumerable<SelectListItem> AdminStatus_List { get; set; }

        [Display(Name = "Estimated Completion Time: ")]
        public string EstimCompletionTime { get; set; }

        [RegularExpression("^(?:[0-9]*)$", ErrorMessage = "ECT days must be a number")]
        [Display(Name = "Days: ")]
        public string UpdateECTDays { get; set; }

        [RegularExpression("^(?:[0-9]*)$", ErrorMessage = "ECT hours must be a number")]
        [Display(Name = "Hours: ")]
        public string UpdateECTHours { get; set; }

        //public int UpdateECTMinutes { get; set; }

        [Display(Name = "Staff Comments: ")]
        [StringLength(2000)]
        public string StaffComments { get; set; }
        [Display(Name = "Notes To Requester: ")]
        [StringLength(2000)]
        public string NotesToRequester { get; set; }

        // Fields for Admin. Report Details

        [Display(Name = "*Report Name: ")]
        [Required(ErrorMessage = "Report Name is a required field"), StringLength(50)]
        public string ReportName { get; set; }
        [Display(Name = "*Report Description: ")]
        [Required(ErrorMessage = "Report Description is a required field"), StringLength(2000)]
        public string ReportDescription { get; set; }

        [Display(Name = "*FOIA: ")]
        [Required(ErrorMessage = "FOIA is a required field")]
        public string FOIA_yes { get; set; }

        [Display(Name = "*Report Type: ")]
        //[Required(ErrorMessage = "Report Type is a required field")]
        public string ReportType_Adding { get; set; }
        public IEnumerable<SelectListItem> ReportType_List { get; set; }

        [Display(Name = "*User Report Location: "), StringLength(100)]
        [Required(ErrorMessage = "User Report Location is a required field")]
        public string UserReportLocation { get; set; }

        // Database Transaction Fields

        public string StatusIDReportType { get; set; }
        public string StaffPrioLevelDesc { get; set; }
        public string ReportStaffName { get; set; }

//    }  // end of the class
#endregion


#region Model's Methods
        public DetailsReportRequest_Model retrieveDetRepReqData(string requestID, string userNM, ref string feedbackMssg)
            #region Retrieves Report Details
        {
            feedbackMssg = "1";         // no exception present in the transaction

            DetailsReportRequest_Model detailsRepReqData = new DetailsReportRequest_Model();

            detailsRepReqData.RequestID = requestID;    // loads request ID
            detailsRepReqData.User_NM = userNM;         // loads ID of the logged in User 

            DBOps dbManager = new DBOps();


            List<OracleParameter> Params = new List<OracleParameter>()
        {
                    new OracleParameter("p_request_id", OracleDbType.Int64, int.Parse(requestID), ParameterDirection.Input),
                    new OracleParameter("V_OUTPUT", OracleDbType.RefCursor, ParameterDirection.Output), 
        };

            try
            {
                using (OracleDataReader dr = dbManager.ExecSP("APP1.APP1_REQUEST_DETAILS_API.view_request_details", Params))
                {
                    while (dr.Read())
                    {

                        detailsRepReqData.UserName =                dr["users_firstname"].ToString() +" "+ dr["users_lastname"].ToString();
                        detailsRepReqData.TimeStamp =               dr["time_stamp"].ToString();
                        detailsRepReqData.ReportType =              dr["report_type_description"].ToString();
                        detailsRepReqData.ReportStatus =            dr["status_description"].ToString();
                        detailsRepReqData.RepNeededBy =             dr["needed_by"].ToString();

                        detailsRepReqData.CurrDutyStation =         dr["location_description"].ToString();
                        detailsRepReqData.RunFrequency =            dr["frequency_description"].ToString();
                        detailsRepReqData.WhatYouNeed =             dr["report_criteria"].ToString();
                        detailsRepReqData.WhyYouNeed =              dr["report_objective"].ToString();
                        detailsRepReqData.FieldsRequired =          dr["required_fields"].ToString();

                        detailsRepReqData.SortDispCriteria =        dr["sort_criteria"].ToString();
                        //detailsRepReqData.DisasterType =            dr[""].ToString();
                        detailsRepReqData.DisasterNo =              dr["disaster_no"].ToString();
                        //detailsRepReqData.PrefDelivery =          dr["delivery_option_note"].ToString();
                        detailsRepReqData.PrefDelivery =            dr["delivery_option"].ToString();
                        detailsRepReqData.UserComments =            dr["user_comments"].ToString();
                        detailsRepReqData.StaffPriorityLevel =      dr["staff_prority"].ToString();

                        detailsRepReqData.ReportStaff =             dr["staff_id"].ToString();
                        detailsRepReqData.AdminStatus =             dr["status_id"].ToString();
                        detailsRepReqData.EstimCompletionTime =     dr["est_compl_time"].ToString();
                        detailsRepReqData.StaffComments =           dr["staff_comments"].ToString();
                        detailsRepReqData.NotesToRequester =        dr["notes_to_user"].ToString();

                        detailsRepReqData.UserID =                  dr["user_id"].ToString();
                        detailsRepReqData.UserEmail =               dr["users_email"].ToString();
                        detailsRepReqData.UserPhoneNumber =         dr["user_phone_area_code"].ToString() + "-" + dr["user_prefix"].ToString() + "-" + dr["user_phone_number"].ToString() + "Ext." + dr["user_phone_ext"].ToString();
                        detailsRepReqData.UserDateCreated =         dr["user_date_created"].ToString();
                        
                        detailsRepReqData.StatusIDReportType =      dr["status_id"].ToString();
                        detailsRepReqData.StaffPrioLevelDesc =      dr["staff_priority_description"].ToString();
                        detailsRepReqData.ReportStaffName =         dr["assigned_to"].ToString();

                    }
                }
            }
            catch (Exception ex)
            {

                feedbackMssg = "Database transaction or connection was unsuccessful, please contact your administrator";

                throw new CustomExceptions.CustomException(ex.Message + " " + ex.StackTrace);

                //string exceptionEmailText = "Error retrieving details of Report Requested data using APP1.APP1_REQUEST_DETAILS_API.view_request_details, error: " + err.Message;
                //sendExceptionEmail(exceptionEmailText);

            }
            finally
            {

            }


            retrieveListsDetRepReqData(detailsRepReqData, requestID, userNM, ref feedbackMssg);

            return (detailsRepReqData);

        }   // end of retrieveDetRepReqData            
#endregion

        private static void retrieveListsDetRepReqData( DetailsReportRequest_Model detailsRepReqData, string requestID, string userNM, ref string feedbackMssg)
            #region Retrieves List
        {
            // Retrieves data to populate Staff Priority Level
            List<SelectListItem> staffPriorList = new List<SelectListItem>();

            //itemSelected = detailsRepReqData.    .ToString();

            DBOps dbManagerSP = new DBOps();
            List<OracleParameter> ParamsSP = new List<OracleParameter>()
        {
                    new OracleParameter("V_OUTPUT", OracleDbType.RefCursor, ParameterDirection.Output), 
        };


            try
            {
                using (OracleDataReader dr = dbManagerSP.ExecSP("SCH1.APP1_API.sel_priority", ParamsSP))
                {
                    while (dr.Read())
                    {
                        staffPriorList.Add(new SelectListItem()
                        {
                            Value = dr["ID"].ToString(),
                            Text = dr["DESCRIPTION"].ToString(),
                            // Selected = dr["ID"].ToString() == itemSelected ? true : false
                            Selected = dr["ID"].ToString() == "L" ? true : false
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Database transaction or connection was unsuccessful for Staff Priority Level, please contact your administrator");
                //string exceptionEmailText = "Error retrieving User's data using APP1_READ_API.sel_user_info, error: " + err.Message;
                //sendExceptionEmail(exceptionEmailText);
            }
            finally
            {

            }

            // Adds the default text for null selection

            detailsRepReqData.StaffPrioLevel_List = staffPriorList;

            //=========================

            // Retrieves data to populate Report Staff
            List<SelectListItem> reportStaffList = new List<SelectListItem>();

            var itemSelectedUserID = detailsRepReqData.UserID;

            DBOps dbManager_RS = new DBOps();
            List<OracleParameter> Params_RS = new List<OracleParameter>()
        {
            //new OracleParameter("p_user_nm", OracleDbType.Varchar2, userNM, ParameterDirection.Input),
            new OracleParameter("V_OUTPUT", OracleDbType.RefCursor, ParameterDirection.Output) 
        };

            try
            {
                //using (OracleDataReader dr = dbManager_RS.ExecSP("APP1.APP1_USERINFO_API.sel_all_user_info", Params_RS))

                using (OracleDataReader dr = dbManager_RS.ExecSP("SCH1.APP1_API.sel_report_staff", Params_RS))
                {
                    while (dr.Read())
                    {
                        reportStaffList.Add(new SelectListItem()
                        {
                            Value = dr["ID"].ToString(),
                            Text = dr["FIRSTNAME"].ToString() + " " + dr["LASTNAME"].ToString(),
                            //Selected = dr["ID"].ToString() == itemSelectedUserID ? true : false
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Database transaction or connection was unsuccessful for Report Staff, please contact your administrator");
                //string exceptionEmailText = "Error retrieving User's data using APP1_READ_API.sel_user_info, error: " + err.Message;
                //sendExceptionEmail(exceptionEmailText);
            }
            finally
            {

            }

            detailsRepReqData.ReportStaff_List = reportStaffList;

            //=========================            
            // Retrieves data to populate Admin Status
            List<SelectListItem> admStatusList = new List<SelectListItem>();

            //itemSelected = detailsRepReqData.  .ToString();

            DBOps dbManager_AmS = new DBOps();
            List<OracleParameter> Params_AmS = new List<OracleParameter>()
                    {
                        new OracleParameter("p_status_id", OracleDbType.Varchar2, detailsRepReqData.StatusIDReportType , ParameterDirection.Input),
                        new OracleParameter("V_OUTPUT", OracleDbType.RefCursor, ParameterDirection.Output) 
                    };

            try
            {
                using (OracleDataReader dr = dbManager_AmS.ExecSP("APP1.APP1_REQUEST_DETAILS_API.sel_admin_status", Params_AmS))
                {
                    while (dr.Read())
                    {
                        admStatusList.Add(new SelectListItem()
                        {
                            Value = dr["status_id_sel"].ToString(),
                            Text = dr["description"].ToString(),
                            // Selected = dr["ID"].ToString() == itemSelected ? true : false
                            //Selected = dr["status_id_sel"].ToString() == "A" ? true : false
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Database transaction or connection was unsuccessful for Admin Status, please contact your administrator");
                //string exceptionEmailText = "Error retrieving User's data using APP1_READ_API.sel_user_info, error: " + err.Message;
                //sendExceptionEmail(exceptionEmailText);
            }
            finally
            {

            }

            detailsRepReqData.AdminStatus_List = admStatusList;

            #region Report Type - Retieves
            //=========================
            // Retrieves data to populate Report Type
            // List<SelectListItem> reportTypeList = new List<SelectListItem>();

            //itemSelected = detailsRepReqData.  .ToString();

            //    DBOps dbManager_RT = new DBOps();
            //    List<OracleParameter> Params_RT = new List<OracleParameter>()
            //{            
            //    new OracleParameter("V_OUTPUT", OracleDbType.RefCursor, ParameterDirection.Output) 
            //};

            //    try
            //    {
            //        using (OracleDataReader dr = dbManager_RT.ExecSP("SCH1.APP1_API.sel_report_type", Params_RT))
            //        {
            //            while (dr.Read())
            //            {
            //                reportTypeList.Add(new SelectListItem()
            //                {
            //                    Value = dr["ID"].ToString(),
            //                    Text = dr["DESCRIPTION"].ToString()
            //                    // Selected = dr["ID"].ToString() == itemSelected ? true : false
            //                });
            //            }
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        throw new Exception("Database transaction or connection was unsuccessful for Report Type, please contact your administrator");
            //        //string exceptionEmailText = "Error retrieving User's data using APP1_READ_API.sel_user_info, error: " + err.Message;
            //        //sendExceptionEmail(exceptionEmailText);
            //    }
            //    finally
            //    {

            //    }

            //    detailsRepReqData.ReportType_List = reportTypeList;
            #endregion

            //return detailsRepReqData;

        }
#endregion Retrieve List

        public DetailsReportRequest_Model retrieveRepDetailData(DetailsReportRequest_Model detailsRepReqData, string feedbackMssg)
            #region Retrieves Report Detail Data
        {

            //=========================
            // Retrieves data to populate Add Report Details for update            
           
            DBOps dbManager_AddRD = new DBOps();
            List<OracleParameter> Params_AddRD = new List<OracleParameter>()
        {   
            new OracleParameter("p_id", OracleDbType.Int64, int.Parse(detailsRepReqData.RequestID), ParameterDirection.Input),
            new OracleParameter("V_OUTPUT", OracleDbType.RefCursor, ParameterDirection.Output) 
        };

            try
            {
                using (OracleDataReader dr = dbManager_AddRD.ExecSP("APP1.APP1_SEND_REQUEST_API.sel_report", Params_AddRD))
                {
                    while (dr.Read())
                    {
                        detailsRepReqData.ReportName =          dr["report_name"].ToString();
                        detailsRepReqData.ReportDescription =   dr["report_desc"].ToString();
                        detailsRepReqData.FOIA_yes =            dr["foia"].ToString(); 
                        detailsRepReqData.ReportType_Adding =   dr["report_type_id"].ToString();
                        detailsRepReqData.UserReportLocation =  dr["user_report_loc"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Database transaction or connection was unsuccessful for Add Report Details, please contact your administrator");
                //string exceptionEmailText = "Error retrieving User's data using APP1.APP1_SEND_REQUEST_API.sel_report, error: " + err.Message;
                //sendExceptionEmail(exceptionEmailText);
            }
            finally
            {

            }

            //detailsRepReqData.ReportType_List = reportTypeList;


            return (detailsRepReqData);

        }
#endregion

        //=========================
        // Saves data related to Admin Related Details section of the Report Request Details

        public void updateAdmin(DetailsReportRequest_Model DetRepReqDataIn, ref string feedbackMssg)
            #region Saving Details
        {
            feedbackMssg = "1";

            string nullValidator = string.IsNullOrEmpty(DetRepReqDataIn.UpdateECTDays) ? "0" : DetRepReqDataIn.UpdateECTDays;
            DetRepReqDataIn.UpdateECTDays = nullValidator;
            nullValidator = string.IsNullOrEmpty(DetRepReqDataIn.UpdateECTHours) ? "0" : DetRepReqDataIn.UpdateECTHours;
            DetRepReqDataIn.UpdateECTHours = nullValidator;

                DBOps dbManager_sARD = new DBOps();

                List<OracleParameter> Params_sARD = new List<OracleParameter>()
            {
                new OracleParameter("p_id",             OracleDbType.Varchar2, (string)DetRepReqDataIn.RequestID            ,ParameterDirection.Input),
                new OracleParameter("p_staff_priority", OracleDbType.Varchar2, (string)DetRepReqDataIn.StaffPriorityLevel   ,ParameterDirection.Input),
                new OracleParameter("p_staff_id",       OracleDbType.Varchar2, (string)DetRepReqDataIn.ReportStaff          ,ParameterDirection.Input),     
                new OracleParameter("p_status_id",      OracleDbType.Varchar2, (string)DetRepReqDataIn.AdminStatus          ,ParameterDirection.Input),

                new OracleParameter("p_est_day",        OracleDbType.Int64,    int.Parse(DetRepReqDataIn.UpdateECTDays)     ,ParameterDirection.Input),
                new OracleParameter("p_est_hr",         OracleDbType.Int64,    int.Parse(DetRepReqDataIn.UpdateECTHours)    ,ParameterDirection.Input),
                //new OracleParameter("p_est_comp_time",  OracleDbType.Varchar2, (string)DetRepReqDataIn.EstimCompletionTime  ,ParameterDirection.Input),
                new OracleParameter("p_staff_comments", OracleDbType.Varchar2, (string)DetRepReqDataIn.StaffComments        ,ParameterDirection.Input),
                new OracleParameter("p_notes_to_user",  OracleDbType.Varchar2, (string)DetRepReqDataIn.NotesToRequester     ,ParameterDirection.Input),
                new OracleParameter("p_modified_by",    OracleDbType.Varchar2, (string)DetRepReqDataIn.UserID               ,ParameterDirection.Input)

                //new OracleParameter("p_output",         OracleDbType.RefCursor,ParameterDirection.Output)
            };

            try
            {
                //using (OracleDataReader dr = dbManager_sARD.ExecSP("APP1.APP1_SEND_REQUEST_API.upd_admin_section", Params_sARD));
                dbManager_sARD.ExecSPNonQuery("APP1.APP1_SEND_REQUEST_API.upd_admin_section", Params_sARD);

            }
            catch (Exception ex)
            {
                feedbackMssg = "Error associated with the database while saving Requested Report Details, please contact your administrator";
                throw new CustomExceptions.CustomException(ex.Message + " " + ex.StackTrace);

                //string exceptionEmailText = "Error retrieving User's APP1.APP1_SEND_REQUEST_API.upd_admin_section, error: " + ex.Message;
                //sendExceptionEmail(exceptionEmailText);

            }
        }
        
            //=========================
            // Saves data related to Report Details section of the Report Request Details

        public void saveDetailRepReqData(DetailsReportRequest_Model DetRepReqDataIn, ref string feedbackMssg)
        {

            feedbackMssg = "2";

            DBOps dbManager_sD = new DBOps();

            List<OracleParameter> Params_sD = new List<OracleParameter>()
            {                
                new OracleParameter("p_report_desc",    OracleDbType.Varchar2,8000, (string)DetRepReqDataIn.ReportDescription   ,ParameterDirection.Input),
                new OracleParameter("p_created_by_nm",  OracleDbType.Varchar2,120, (string)DetRepReqDataIn.User_NM              ,ParameterDirection.Input),
                new OracleParameter("p_foia",           OracleDbType.Varchar2,4, (string)DetRepReqDataIn.FOIA_yes               ,ParameterDirection.Input),        
                new OracleParameter("p_user_report_loc",OracleDbType.Varchar2,400, (string)DetRepReqDataIn.UserReportLocation   ,ParameterDirection.Input),

                new OracleParameter("p_report_name",    OracleDbType.Varchar2,200, (string)DetRepReqDataIn.ReportName           ,ParameterDirection.Input),
                new OracleParameter("p_report_type_id", OracleDbType.Varchar2,80, (string)DetRepReqDataIn.ReportType            ,ParameterDirection.Input),  // left as is since Stored Procedure signature and report type doesn't change
                new OracleParameter("p_request_id",     OracleDbType.Int64, int.Parse(DetRepReqDataIn.RequestID)                ,ParameterDirection.Input)
            };

            try
            {
                dbManager_sD.ExecSPNonQuery("APP1.APP1_SEND_REQUEST_API.ins_report", Params_sD);

            }
            catch (Exception ex)
            {
                feedbackMssg = "Error associated with the database while saving Requested Report Details, please contact your administrator";
                throw new CustomExceptions.CustomException(ex.Message + " " + ex.StackTrace);

                //string exceptionEmailText = "Error Saving Report Details using APP1.APP1_SEND_REQUEST_API.ins_report, error: " + ex.Message;
                //sendExceptionEmail(exceptionEmailText);

            } 



        }

        //public void updateDetailsRepReqData(DetailsReportRequest_Model DetRepReqDataIn, ref string feedbackMssg)
        //{

        //    //=========================
        //    // Updates data related to Report Details section of the Report Request Details

        //    feedbackMssg = "1";

        //    string nullValidator = string.IsNullOrEmpty(DetRepReqDataIn.UpdateECTDays) ? "0" : DetRepReqDataIn.UpdateECTDays;
        //    DetRepReqDataIn.UpdateECTDays = nullValidator;
        //    nullValidator = string.IsNullOrEmpty(DetRepReqDataIn.UpdateECTHours) ? "0" : DetRepReqDataIn.UpdateECTHours;
        //    DetRepReqDataIn.UpdateECTHours = nullValidator;

        //    DBOps dbManager_D = new DBOps();

        //    List<OracleParameter> Params_D = new List<OracleParameter>()
        //    {
        //        new OracleParameter("p_id",             OracleDbType.Varchar2, (string)DetRepReqDataIn.RequestID            ,ParameterDirection.Input),
        //        new OracleParameter("p_staff_priority", OracleDbType.Varchar2, (string)DetRepReqDataIn.StaffPriorityLevel   ,ParameterDirection.Input),
        //        new OracleParameter("p_staff_id",       OracleDbType.Varchar2, (string)DetRepReqDataIn.ReportStaff          ,ParameterDirection.Input),
        //        new OracleParameter("p_status_id",      OracleDbType.Varchar2, (string)DetRepReqDataIn.AdminStatus         ,ParameterDirection.Input),        
        //        new OracleParameter("p_est_day",        OracleDbType.Int64, int.Parse(DetRepReqDataIn.UpdateECTDays)        ,ParameterDirection.Input),

        //        new OracleParameter("p_est_hr",         OracleDbType.Int64, int.Parse(DetRepReqDataIn.UpdateECTHours)       ,ParameterDirection.Input),
        //        new OracleParameter("p_staff_comments", OracleDbType.Varchar2, (string)DetRepReqDataIn.StaffComments        ,ParameterDirection.Input),
        //        new OracleParameter("p_notes_to_user",  OracleDbType.Varchar2, (string)DetRepReqDataIn.NotesToRequester     ,ParameterDirection.Input),
        //        new OracleParameter("p_modified_by",    OracleDbType.Varchar2, (string)DetRepReqDataIn.UserID               ,ParameterDirection.Input)

        //        //new OracleParameter("p_output",         OracleDbType.RefCursor,ParameterDirection.Output)
        //    };

        //    try
        //    {
        //        //using (OracleDataReader dr = dbManager_D.ExecSP("APP1.APP1_SEND_REQUEST_API.upd_admin_section", Params_D));

        //        dbManager_D.ExecSPNonQuery("APP1.APP1_SEND_REQUEST_API.upd_admin_section", Params_D);

        //    }
        //    catch (Exception ex)
        //    {
        //        feedbackMssg = "Error associated with the database while updating Requested Report Details, please contact your administrator";
        //        throw new CustomExceptions.CustomException(ex.Message + " " + ex.StackTrace);

        //        //string exceptionEmailText = "Error retrieving User's APP1.APP1_REGISTER_USER_API.ins_user_profile, error: " + ex.Message;
        //        //sendExceptionEmail(exceptionEmailText);

        //    }
        //}

#endregion

#endregion

    }  // end of the class

}