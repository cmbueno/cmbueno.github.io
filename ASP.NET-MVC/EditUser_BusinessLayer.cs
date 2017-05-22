//*********************************************************
//Application: XYZ
//Author:  MyNameHere
//Date: 05/2017
//*********************************************************

using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Mvc;

namespace BusinessLayer
{
    public class EditUser_BusinessLayer
    {

        /////////////////////////////////////  RETRIEVING DATA /////////////////////////////////////

        public User_Model_BusinessLayer editUserForUpdate_DataGet(string userRoleID_selected, string userContractorID_selected)
        {

            AddUserBusinessLayer rolesListBusinessLayer = new AddUserBusinessLayer();           //Retrieves roles from database without set a selected role in the output
            List<SelectListItem> rolesSelectList = rolesListBusinessLayer.Roles_List_Cursor(userRoleID_selected, false);

            AddUserBusinessLayer contractorsListBusinessLayer = new AddUserBusinessLayer();     //Retrieves contractors from database  without set a selected contractor in the output
            List<SelectListItem> contractorsSelectList = contractorsListBusinessLayer.Contractors_List_Cursor(userContractorID_selected, false);

            var editUserForUpdate_Model = new User_Model_BusinessLayer
            {
                ROLE_ID_LIST = rolesSelectList,
                CONTRACTOR_ID_LIST = contractorsSelectList

            };
            return editUserForUpdate_Model;

        }



        /// <summary>
        /// Populates model for the rendering of the view, or other applications
        /// </summary>
        /// <returns></returns>
        public EditUser_Model_BusinessLayer editUser_DataGet()
        {

            List<SelectListItem> ID_usersSelectlist = new List<SelectListItem>();

            List<SelectListItem> usersSelectList = Get_User_List_Cursor(ID_usersSelectlist);

            var editUser_Model = new EditUser_Model_BusinessLayer
            {
                EDIT_FULLNAME_LIST = usersSelectList,
                EDIT_USER_ID_LIST = ID_usersSelectlist
            };
            return editUser_Model;

        }


        /// <summary>
        /// /Retrieves the list of users listed in Edit User from User Maintenance in STRK
        /// </summary>
        public List<SelectListItem> Get_User_List_Cursor(List<SelectListItem> users_ID_Selectlist)
        {
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DataConnectionStringMINE"].ConnectionString;
                List<SelectListItem> users_List = new List<SelectListItem>();
                DataTable datatable = new DataTable();

                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    OracleCommand cmd = new OracleCommand("strk_read_api.sel_tbl_fp_users", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_user_id", OracleDbType.Varchar2).Value = DBNull.Value;
                    cmd.Parameters.Add("v_output", OracleDbType.RefCursor, ParameterDirection.Output);

                    try
                    {
                        con.Open();
                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        cmd.ExecuteNonQuery();
                        da.Fill(datatable);

                            foreach (DataRow row in datatable.Rows)
                            {
                                users_List.Add(new SelectListItem()
                                {
                                    Value = row[0].ToString(),
                                    Text = row[1].ToString()        
                                });

                                users_ID_Selectlist.Add(new SelectListItem()
                                {
                                    Value = row[0].ToString(),
                                    Text = row[0].ToString()
                                });
                            }
                    }
                    catch (Exception err)
                    {
                        string errMssg = "Error reading users table for edit user data: " + err.Message;
                        con.Close();
                    }
                    con.Close();
                }
                return users_List;
            }
        }

 
        public User_Model_BusinessLayer Get_User_Data_Cursor(string selectedUser_ID)
        {
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DataConnectionStringMINE"].ConnectionString;

                User_Model_BusinessLayer selectedUserData = new User_Model_BusinessLayer();
                DataSet dataset = new DataSet();

                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    OracleCommand cmd = new OracleCommand("strk_read_api.sel_tbl_fp_users_2", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_user_id", OracleDbType.Varchar2).Value = selectedUser_ID;
                    cmd.Parameters.Add("v_output", OracleDbType.RefCursor, ParameterDirection.Output);

                    try
                    {
                        con.Open();
                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        cmd.ExecuteNonQuery();
                        da.Fill(dataset);
                    }
                    catch (Exception err)
                    {
                        string errMssg = "Error reading contractors table: " + err.Message;
                    }
                    finally
                    {
                        con.Close();
                    }
                    
                }
                selectedUserData.USER_ID = (string)dataset.Tables[0].Rows[0]["user_id"];
                selectedUserData.LAST_NAME = (string)dataset.Tables[0].Rows[0]["last_name"];
                selectedUserData.FIRST_NAME = (string)dataset.Tables[0].Rows[0]["first_name"];
                selectedUserData.EMAIL = (string)dataset.Tables[0].Rows[0]["email"];
                selectedUserData.ROLE_ID = int.Parse(dataset.Tables[0].Rows[0]["role_id"].ToString());
                selectedUserData.CONTRACTOR_ID = int.Parse(dataset.Tables[0].Rows[0]["contractor_id"].ToString());
                selectedUserData.LOCKED = (string)dataset.Tables[0].Rows[0]["locked"];

                return selectedUserData;
            }
        }

        public List<Junc_Email_Dest_Model_BusinessLayer> Get_User_Email_Cursor(string selectedUser_ID)
        {

            {
                string connectionString = ConfigurationManager.ConnectionStrings["DataConnectionStringMINE"].ConnectionString;
                List<Junc_Email_Dest_Model_BusinessLayer> junc_Email_Dest_List = new List<Junc_Email_Dest_Model_BusinessLayer>();
                DataSet ds = new DataSet();
                DataTable datatable = new DataTable();

                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    OracleCommand cmd = new OracleCommand("strk_read_api.sel_junc_email_dest", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_user_id", OracleDbType.Varchar2).Value = selectedUser_ID;
                    cmd.Parameters.Add("v_output", OracleDbType.RefCursor, ParameterDirection.Output);

                    try
                    {
                        con.Open();
                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        cmd.ExecuteNonQuery();
                        da.Fill(ds, "JUNC_EMAIL_DEST");
                        datatable = ds.Tables["JUNC_EMAIL_DEST"];

                        foreach ( DataRow row in datatable.Rows)
                        {
                            Junc_Email_Dest_Model_BusinessLayer jEmail = new Junc_Email_Dest_Model_BusinessLayer();
                            jEmail.EMAIL_REASON_CODE = row["EMAIL_REASON_CODE"].ToString();
                            jEmail.P_EMAIL = row["P_EMAIL"].ToString();
                            jEmail.CC_EMAIL = row["CC_EMAIL"].ToString();
                            jEmail.BC_EMAIL = row["BC_EMAIL"].ToString();
                            jEmail.NO_EMAIL = row["NO_EMAIL"].ToString();

                            junc_Email_Dest_List.Add(jEmail);
                        }
                    }
                    catch (Exception err)
                    {
                        string errMssg = "Error reading juncEmail table (using SP: sel_junc_email_dest) at EditUser_BusinessLayer/Get_User_Email_Cursor: " + err.Message;

                    }
                    finally
                    {
                        con.Close();
                    }
                    
                }
                return junc_Email_Dest_List;
            }


        }


        public string storeUserData(User_Model_BusinessLayer userData)
        {

            string connectionString = ConfigurationManager.ConnectionStrings["DataConnectionStringMINE"].ConnectionString;
            string mssgBack = "Data for the added user was stored successfully";

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand("strk_admin_api.ins_tbl_fp_users", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_user_id", OracleDbType.Varchar2).Value = (string)userData.USER_ID;
                cmd.Parameters.Add("p_last_name", OracleDbType.Varchar2).Value = (string)userData.LAST_NAME;
                cmd.Parameters.Add("p_first_name", OracleDbType.Varchar2).Value = (string)userData.FIRST_NAME;
                cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = (string)userData.EMAIL;
                cmd.Parameters.Add("p_role_id", OracleDbType.Int32).Value = (int)userData.ROLE_ID;
                cmd.Parameters.Add("p_contractor_id", OracleDbType.Int32).Value = (int)userData.CONTRACTOR_ID;
                cmd.Parameters.Add("p_locked", OracleDbType.Varchar2).Value = (string)userData.LOCKED;
                cmd.Parameters.Add("V_OUTPUT", OracleDbType.RefCursor, ParameterDirection.Output);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                }
                catch (Exception err)
                {
                    string errMssg = "Error inserting users in table: " + err.Message;
                    mssgBack = "Error saving added user's data in database, check if connection is up and try again";
                }
                finally
                {
                    con.Close();
                }                
            }
            return mssgBack;
        }

        /// <summary>
        /// Creates records in jn_junc_email_dest with the info about emails necessary for a new user created in Add User interface
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        public string storeUserEmailData(User_Model_BusinessLayer userData)
        {
            const int first_email_reason_code = 41;
            const int last_email_reason_code = 47;

            string connectionString = ConfigurationManager.ConnectionStrings["DataConnectionStringMINE"].ConnectionString;

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand("strk_admin_api.ins_multiple_junc_email_dest", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_user_id", OracleDbType.Varchar2).Value = (string)userData.USER_ID;
                cmd.Parameters.Add("p_first_email_reason_code", OracleDbType.Int32).Value = (int)first_email_reason_code;
                cmd.Parameters.Add("p_last_email_reason_code", OracleDbType.Int32).Value = (int)last_email_reason_code;
                cmd.Parameters.Add("p_DbError", OracleDbType.Varchar2).Value = (string)userData.DbError;
                cmd.Parameters.Add("p_InspDetermChange", OracleDbType.Varchar2).Value = (string)userData.InspDetermChange;
                cmd.Parameters.Add("p_InspDeleted", OracleDbType.Varchar2).Value = (string)userData.InspDeleted;
                cmd.Parameters.Add("p_DepListReport", OracleDbType.Varchar2).Value = (string)userData.DepListReport;
                cmd.Parameters.Add("p_AppAdmins", OracleDbType.Varchar2).Value = (string)userData.AppAdmins;
                cmd.Parameters.Add("p_TechSolutions", OracleDbType.Varchar2).Value = (string)userData.TechSolutions;
                cmd.Parameters.Add("p_ContactUs", OracleDbType.Varchar2).Value = (string)userData.ContactUs;
                cmd.Parameters.Add("V_OUTPUT", OracleDbType.RefCursor, ParameterDirection.Output);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                }
                catch (Exception err)
                {
                    string errMssg = "Error inserting emails in table: " + err.Message;
                    con.Close();
                }
                con.Close();
            }
            return "Data of emails stored succesfully";
        }

    }
}
