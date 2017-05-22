//*********************************************************
//Application: XYZ
//Author:  MyNameHere
//Date: 05/2017
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using System.Web.Routing;

namespace XYZ.Controllers
{
    public class UserMaintenanceController : Controller
    {
        //private IView userData_Model;

        /// <summary>
        /// Displays Add User interface, retrieving contractors and roles data from database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddUser()
        {
            User_Model_BusinessLayer userData_Model = new User_Model_BusinessLayer();

            AddUserBusinessLayer userData_BusinessLayer = new AddUserBusinessLayer();
            try
            {
                userData_Model = userData_BusinessLayer.User_Data_Get(null, null, false);   //Retrieves user data from database
            }
            catch (Exception err)
            {
                string errMssg = "Error in database trying to use strk_read_api.sel_tbl_fp_roles or .sel_tbl_fp_contractor " + err.Message.ToString();

                return View();
                //throw;
            }
            

            return View(userData_Model);                        
        }


        /// <summary>
        /// Retrieves data from Add User interface, activating SP strk_read_api.ins_tbl_fp_users to store it
        /// </summary>
        /// <param name="AddedUserData"></param>
        /// <returns></returns>
        [HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult AddUser(User_Model_BusinessLayer AddedUserData)
        {
            string mssgBack_User = string.Empty;
            string mssgBack_Email = string.Empty;

            AddUserBusinessLayer insertNewUser = new AddUserBusinessLayer();

            if (ModelState.IsValid)
            {
                try
                {
                    mssgBack_User = insertNewUser.storeUserData(AddedUserData); 
                    mssgBack_Email = insertNewUser.storeUserEmailData(AddedUserData);

                    return RedirectToAction("AddUser");                                             //Add User's interface is displayed again
                }
                catch (Exception err)
                {
                    string errorMessg = "Error saving new user's data" + err;
                    mssgBack_User = "Error in the application, please contact technical support with this error message: " + err;
                    return View();

                   // throw;
                }                
            }

            ViewBag.mssgBack_User = mssgBack_User;
            ViewBag.mssgBack_Email = mssgBack_Email;
            return View();
        }

        [HttpGet]
        public ActionResult UserEdit()
        {
            EditUser_BusinessLayer editUserListBusinessLayer = new EditUser_BusinessLayer();
            EditUser_Model_BusinessLayer editUserData_Model = editUserListBusinessLayer.editUser_DataGet();

            ViewBag.DisplayAddUser = false;  // Displays Partial View for Edit User functionality
            return View (editUserData_Model);
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult UserEdit(EditUser_Model_BusinessLayer SelectedUserData)
        {
            Session["mssgCont"] = null;
            if (ModelState.IsValid)
            {
                string selected_userID = (SelectedUserData.USER_ID == null) ? SelectedUserData.FULLNAME : SelectedUserData.USER_ID;  // user ID is retrieved as the Value of the selected dropdownlist (FULLNAME or USER_ID)
                return RedirectToAction("UpdateUser", new { selectedUser_ID = selected_userID });
            }

            return RedirectToAction("UserEdit");        //application redisplay the interface anew since no ID or User was selected

        }

        public ActionResult UpdateUser(string selecteduser_ID)
        {

            User_Model_BusinessLayer selectedUserRetrievedData = new User_Model_BusinessLayer();
            EditUser_BusinessLayer getSelectedUserData = new EditUser_BusinessLayer();        // Will hold the personal data of the user to be edited

            List<Junc_Email_Dest_Model_BusinessLayer> selectedUserRetrievedJuncEmailDataList = new List<Junc_Email_Dest_Model_BusinessLayer>();
            EditUser_BusinessLayer getSelectedUserJuncEmailDataList = new EditUser_BusinessLayer();        // Will hold the email data of the user to be edited

            User_Model_BusinessLayer modelWithSelectedUserProfile = new User_Model_BusinessLayer();
            EditUser_BusinessLayer getDataForModelWithUserProfile = new EditUser_BusinessLayer();        // Will be the model used to build the Edit User interface

            try
            {
                selectedUserRetrievedData = getSelectedUserData.Get_User_Data_Cursor(selecteduser_ID);                              // Will retrieve the personal data of the user to be edited
                selectedUserRetrievedJuncEmailDataList = getSelectedUserJuncEmailDataList.Get_User_Email_Cursor(selecteduser_ID);   // Will retrieve the email data of the user to be edited

                if (!(selectedUserRetrievedJuncEmailDataList.Count == 0))                                                           //There is email's data in the table
                {         
                    var emailCode_Dictionary = new Dictionary<string, string>();
                    string e_mailCondition = "N";
                    foreach (Junc_Email_Dest_Model_BusinessLayer emailItem in selectedUserRetrievedJuncEmailDataList)
                    {
                        if (emailItem.P_EMAIL == "Y") { e_mailCondition = "P"; }
                        else { e_mailCondition = "N"; }

                        emailCode_Dictionary.Add(emailItem.EMAIL_REASON_CODE, e_mailCondition);
                    }

                    ViewData["emailDict"] = emailCode_Dictionary;
                }
                else
                {

                    Session["mssgCont"] = "No email records were found for user ID " + selecteduser_ID.ToString() +". Information could be deleted or corrupted";
                    
                    return RedirectToAction("UserEdit");
                }


                string userSelected_RoleID = selectedUserRetrievedData.ROLE_ID.ToString();
                string userSelected_ContractorID = selectedUserRetrievedData.CONTRACTOR_ID.ToString();
                modelWithSelectedUserProfile = getDataForModelWithUserProfile.editUserForUpdate_DataGet(userSelected_RoleID, userSelected_ContractorID); // Retrieves user's role and contractor as selected in the corresponding dropdownlist

                modelWithSelectedUserProfile.USER_ID = selectedUserRetrievedData.USER_ID;                                           //populates model for rendering user data
                modelWithSelectedUserProfile.LAST_NAME = selectedUserRetrievedData.LAST_NAME;
                
                return View(modelWithSelectedUserProfile);

            }
            catch (Exception err)
            {
                string errMssg = "Error reading juncEmail table (using SP: sel_junc_email_dest) at UserMaintenanceController level: " + err.Message;

                //return View("Error",)

                ViewBag.mssgBack_Email = "Error reading data from the database, check connection is up and try again ";

                return RedirectToAction("UserEdit");
                // throw;
            }

            //return RedirectToAction("UserEdit");
        }


        [HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult UpdateUser(User_Model_BusinessLayer userUIUpdatedData)
        {
            string mssg_back = string.Empty;
            string messageBackEmail = string.Empty;
            UpdateUser_BusinessLayer submitUser_update = new UpdateUser_BusinessLayer();

            if (ModelState.IsValid)
            {
                try
                {
                    mssg_back = submitUser_update.submit_User_eMail_DataUpdate(userUIUpdatedData, messageBackEmail);
                }
                catch (Exception err)
                {
                    string errormssg = "Error while updating users data" + err.Message;
                    mssg_back = "Error in the connection to database while updating data of user" + userUIUpdatedData.FIRST_NAME.ToString() + " " + userUIUpdatedData.LAST_NAME.ToString() + "check connection and try again";
                    return View();
                }
            }

            ViewBag.mssgBack_User = mssg_back;
            ViewBag.mssgBack_Email = messageBackEmail;
            return RedirectToAction("UserEdit");
        }
    }
}
