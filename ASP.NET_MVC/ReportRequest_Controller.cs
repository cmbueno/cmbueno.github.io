using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIDA.Models;
//using FIDA.Helpers;

namespace FIDA.Controllers
{
    public class DetailsReportRequestController : Controller
    {
        //
        // GET: /DetailsReportRequest/
        //[HttpGet]
        public ActionResult Index(string pRequestID)
        {
            string ReqID = pRequestID.TrimStart('v', '_');  //Trims Request ID prefix

            @Session["transactionStatus"] = "";
                        
            string feedbackText = "";
            string userID = @System.Web.HttpContext.Current.User.Identity.Name.Split('\\')[1].ToUpper();

            DetailsReportRequest_Model detailsRepReqData = new DetailsReportRequest_Model();

            DetailsReportRequest_Model detRepReqDataProcess = new DetailsReportRequest_Model();

            if (ModelState.IsValid)
            {

                detailsRepReqData = detRepReqDataProcess.retrieveDetRepReqData(ReqID, userID, ref feedbackText);
                
                if (feedbackText == "1")
                {
                    @Session["transactionStatus"] = "";         // blanks the first instances                     
                }
                else
                {
                    @Session["transactionStatus"] = feedbackText; // pass the database o database transaction exception error
                }

                // User's data for modal dialog to display User's detail data
                @Session["userDataID"] = "  Users ID: " + detailsRepReqData.UserID; 
                @Session["userDataName"] = "  Users Name: " + detailsRepReqData.UserName;
                @Session["userDataEmail"] = "  Users Email: " + detailsRepReqData.UserEmail;
                @Session["userDataPhone"] = "  Users Phone: " + detailsRepReqData.UserPhoneNumber;
                @Session["userDataDateReg"] = "  Date Registered: " + detailsRepReqData.UserDateCreated;

                // Dropdown lists pre-selected values

                @Session["staffPrioLevel"] = detailsRepReqData.StaffPriorityLevel;
                @Session["reportStaff"] = detailsRepReqData.ReportStaff;
                @Session["admStatus"] = detailsRepReqData.AdminStatus;
                //@Session["repType"] = detailsRepReqData.ReportType_Adding;
                @Session["staffComm"] = detailsRepReqData.StaffComments;
                @Session["notesRequest"] = detailsRepReqData.NotesToRequester;
            }

            return View("DetailsReportRequest", detailsRepReqData);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminDetail([Bind(Include = "RequestID, StaffPriorityLevel, StaffPrioLevel_List, ReportStaff, ReportStaff_List, AdminStatus, AdminStatus_List, EstimCompletionTime, UpdateECTDays, UpdateECTHours, StaffComments, NotesToRequester")] DetailsReportRequest_Model dReportReqData)
        {

            string feedbackText = "", ReqID = "", userID ="";

            @Session["transactionStatus"] = "";

            ReqID = dReportReqData.RequestID;
            userID = dReportReqData.User_NM;
            
            DetailsReportRequest_Model dRepReqData = new DetailsReportRequest_Model();
            DetailsReportRequest_Model detRepReqDataProcess = new DetailsReportRequest_Model();

            // User's data for modal dialog to display User's detail data
            @Session["userDataID"] = "  Users ID: " + dReportReqData.UserID;
            @Session["userDataName"] = "  Users Name: " + dReportReqData.UserName;
            @Session["userDataEmail"] = "  Users Email: " + dReportReqData.UserEmail;
            @Session["userDataPhone"] = "  Users Phone: " + dReportReqData.UserPhoneNumber;
            @Session["userDataDateReg"] = "  Date Registered: " + dReportReqData.UserDateCreated;

            // Dropdown lists pre-selected values
            @Session["staffPrioLevel"] = dReportReqData.StaffPriorityLevel;
            @Session["reportStaff"] = dReportReqData.ReportStaff;
            @Session["admStatus"] = dReportReqData.AdminStatus;
            //@Session["repType"] = dReportReqData.ReportType_Adding;
            @Session["staffComm"] = dReportReqData.StaffComments;
            @Session["notesRequest"] = dReportReqData.NotesToRequester;
            
            
            //Server side validation

            var ModelVal = ModelState.IsValidField("StaffPriorityLevel") &&
                           ModelState.IsValidField("ReportStaff") &&
                           ModelState.IsValidField("AdminStatus");

            //if (ModelState.IsValid)

            if (ModelVal)
            {
                detRepReqDataProcess.updateAdmin(dReportReqData, ref feedbackText);

                @Session["transactionStatus"] = feedbackText;
            }
            else
            {
                @Session["transactionStatus"] = "You must select or fill the required fields";
            }

            ModelState.Clear();

            dRepReqData = detRepReqDataProcess.retrieveDetRepReqData(ReqID, userID, ref feedbackText);

                return View("DetailsReportRequest", dRepReqData);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportDetail([Bind(Include = "ReportName, ReportDescription, FOIA_yes, UserReportLocation, StaffPriorityLevel, ReportStaff, AdminStatus, ReportType_Adding, User_NM, ReportType, RequestID ")]DetailsReportRequest_Model dReportReqData)
        {


            string feedbackText = "", ReqID = "", userID = "";

            @Session["transactionStatus"] = "";


            ReqID = dReportReqData.RequestID;
            dReportReqData.User_NM = @System.Web.HttpContext.Current.User.Identity.Name.Split('\\')[1].ToUpper();

            DetailsReportRequest_Model dRepReqData = new DetailsReportRequest_Model();
            DetailsReportRequest_Model detRepReqDataProcess = new DetailsReportRequest_Model();

            // User's data for modal dialog to display User's detail data
            @Session["userDataID"] = "  Users ID: " + dReportReqData.UserID;
            @Session["userDataName"] = "  Users Name: " + dReportReqData.UserName;
            @Session["userDataEmail"] = "  Users Email: " + dReportReqData.UserEmail;
            @Session["userDataPhone"] = "  Users Phone: " + dReportReqData.UserPhoneNumber;
            @Session["userDataDateReg"] = "  Date Registered: " + dReportReqData.UserDateCreated;

            var ModelVal = ModelState.IsValidField("ReportName") &&
                           ModelState.IsValidField("ReportDescription") &&
                           ModelState.IsValidField("FOIA_yes") &&
                           ModelState.IsValidField("UserReportLocation");

            //Server side validation
            if (ModelVal)
            {
                detRepReqDataProcess.saveDetailRepReqData(dReportReqData, ref feedbackText);

                @Session["transactionStatus"] = feedbackText;

                if (feedbackText != "2")
                {
                    @Session["transactionStatus"] = feedbackText;  // pass the database o database transaction error 
                }                
            }
            else
            {
                @Session["transactionStatus"] = "You must select or fill the required fields";

                // regain pre-selected values
                @Session["reportName"] = dReportReqData.ReportName;
                @Session["reportDesc"] = dReportReqData.ReportDescription;                
                @Session["reportLoc"] = dReportReqData.UserReportLocation;
            }

            ModelState.Clear();

            dReportReqData = detRepReqDataProcess.retrieveDetRepReqData(ReqID, userID, ref feedbackText);

            if (feedbackText != "1")
            {
                @Session["transactionStatus"] = feedbackText;  // pass the database o database transaction error 
            }
            else
            {
                feedbackText = "2";  // resets the "detail's saving" status as ok, no database failures
            }

            return View("DetailsReportRequest", dReportReqData);
        }


        // Code for a double saving action with one button
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Index([Bind(Include = "RequestID, StaffPriorityLevel, StaffPrioLevel_List, ReportStaff, ReportStaff_List, AdminStatus, AdminStatus_List, EstimCompletionTime, UpdateECTDays, UpdateECTHours, StaffComments, NotesToRequester, ReportName, ReportDescription, FOIA_yes, ReportType, UserReportLocation")] DetailsReportRequest_Model dReportReqData)
        //{
        //    string pFeedBack ="", feedbackText = "";

        //    string ReqID = dReportReqData.RequestID;
        //    string userID = @System.Web.HttpContext.Current.User.Identity.Name.Split('\\')[1].ToUpper();


        //   // var errorsM = ModelState.Values.SelectMany(v => v.Errors);

           
        //   DetailsReportRequest_Model dRepReqData = new DetailsReportRequest_Model();
        //   DetailsReportRequest_Model detRepReqDataProcess = new DetailsReportRequest_Model();

        //   dReportReqData.User_NM = @System.Web.HttpContext.Current.User.Identity.Name.Split('\\')[1].ToUpper(); 

        //    if (ModelState.IsValid)
        //    {
        //        //DetailsReportRequest_Model.saveDetRepReqData(dReportReqData, pFeedBack);

        //        dReportReqData = detRepReqDataProcess.retrieveDetRepReqData(ReqID, userID, feedbackText);
            
        //    }
        //    else
        //    {

        //        dReportReqData = detRepReqDataProcess.retrieveDetRepReqData(ReqID, userID, feedbackText);

        //    }

        //    return View("DetailsReportRequest", dReportReqData);
        //}


        // Print Friendly version of the page
        public ActionResult printFriendly(string repReqNum)
        {
            string feedbackText = "";

            string userID = @System.Web.HttpContext.Current.User.Identity.Name.Split('\\')[1].ToUpper();


            DetailsReportRequest_Model dReportReqData = new DetailsReportRequest_Model();
            DetailsReportRequest_Model detRepReqDataProcess = new DetailsReportRequest_Model();

            dReportReqData = detRepReqDataProcess.retrieveDetRepReqData(repReqNum, userID, ref feedbackText);

            return View("DetailsReportReqPrint", dReportReqData);
        }

    }
}
