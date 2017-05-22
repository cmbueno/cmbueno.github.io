//*********************************************************
//Application: XYZ
//Author:  MyNameHere
//Date: 05/2017
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data;
using System.Web.Mvc;

namespace BusinessLayer
{
    public class UpdateUser_BusinessLayer
    {

        public string submit_User_eMail_DataUpdate(User_Model_BusinessLayer user_Email_DataToUpdate, string mssgBackForEmail)
        {
            string mssg_Back, mssg_Back_eMail = string.Empty;

            mssg_Back = submitUpdatedUserData(user_Email_DataToUpdate);
            mssg_Back_eMail = submit_UpdatedUser_eMailData(user_Email_DataToUpdate);

            mssgBackForEmail = mssg_Back_eMail;
            return mssg_Back;

        }

        public string submitUpdatedUserData(User_Model_BusinessLayer userDataToUpdate)
        {
            string mssgBack = "Data of user " + userDataToUpdate.FIRST_NAME + " " + userDataToUpdate.LAST_NAME + " was saved successfully";


            string connectionString = ConfigurationManager.ConnectionStrings["DataConnectionStringMINE"].ConnectionString;

            User_Model_BusinessLayer selectedUserData = new User_Model_BusinessLayer();
            DataSet dataset = new DataSet();

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand("strk_admin_api.upd_tbl_fp_users", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_user_id",     OracleDbType.Varchar2).Value = (string)userDataToUpdate.USER_ID;
                cmd.Parameters.Add("p_last_name",   OracleDbType.Varchar2).Value = (string)userDataToUpdate.LAST_NAME;
                cmd.Parameters.Add("p_first_name",  OracleDbType.Varchar2).Value = (string)userDataToUpdate.FIRST_NAME;
                cmd.Parameters.Add("p_email",       OracleDbType.Varchar2).Value = (string)userDataToUpdate.EMAIL;
                cmd.Parameters.Add("p_role_id",     OracleDbType.Int32).Value =    (int)userDataToUpdate.ROLE_ID;
                cmd.Parameters.Add("p_contractor_id", OracleDbType.Int32).Value =  (int)userDataToUpdate.CONTRACTOR_ID;
                cmd.Parameters.Add("p_locked",      OracleDbType.Varchar2).Value = (string)userDataToUpdate.LOCKED;
                //cmd.Parameters.Add("V_OUTPUT",      OracleDbType.RefCursor, ParameterDirection.Output);

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
                    mssgBack = "Error in database: data of user " + userDataToUpdate.FIRST_NAME + " " + userDataToUpdate.LAST_NAME + " was not updated";
                }
                finally
                {
                    con.Close();
                }

            }


            return (mssgBack);
        }


        public string submit_UpdatedUser_eMailData(User_Model_BusinessLayer userEmail_DataToUpdate)
        {


            {
                string mssgBack = "The email data of user " + userEmail_DataToUpdate.FIRST_NAME + " " + userEmail_DataToUpdate.LAST_NAME + " was updated succesfully";

                string P_EMAIL = "N"; string CC_EMAIL = "N";
                string BC_EMAIL = "N"; string NO_EMAIL = "N";

                /*
                DESCRIPTION
                The following assignments are establishing the EMAIL_REASON_CODE code for each EMAIL_REASON_CODE text. Example: code 41 is for DbError etc. -  codes established in previous Classic ASP version -
            
                TABLE OF EMAIL_REASON_CODE values     
            
                CODE            DESCRIPTION    |    CODE            DESCRIPTION           |      CODE            DESCRIPTION
                41 =            DbError        |    42 =            InspDetermChange      |      43 =            InspDeleted
                44 =            DepListReport  |    45 =            AppAdmins             |      46 =            TechSolutions
                47 =            ContactUs.     |                                          |

                The value stored in userEmail_Data.DbError (and the others like 'InspDetermChange', etc.)  can be P, C, B or N  */

                string _userID = userEmail_DataToUpdate.USER_ID.ToString();
                string[] p_emailUserID_Array = new string[] { _userID, _userID, _userID, _userID, _userID, _userID, _userID };
                string[] p_emailReasonCode_Array = new string[] { "41", "42", "43", "44", "45", "46", "47" }; //  NOTE: the length of this array is used to calculate cmd.ArrayBindCount used in the Oracle instruction below


                string[] emailConditionSelected = new string[] {userEmail_DataToUpdate.DbError.ToString(),                                                  // Can have the values "P", "C", "B", "N" indicating DbError is going 
                                                            userEmail_DataToUpdate.InspDetermChange.ToString(), userEmail_DataToUpdate.InspDeleted.ToString(),  // to Primary Email ("P") or CCEmail ("C")... etc. Same for the 
                                                            userEmail_DataToUpdate.DepListReport.ToString(),    userEmail_DataToUpdate.AppAdmins.ToString(),    //  others userEmail_Data.InspDetermChange.ToString() ... etc
                                                            userEmail_DataToUpdate.TechSolutions.ToString(),    userEmail_DataToUpdate.ContactUs.ToString()};

                string[] emailConditions_Array = new string[] { "P", "C", "B", "N" };      // array used just to count amount of conditions for emaiCX matix


                string[,] emailCX = new string[p_emailReasonCode_Array.Length, emailConditions_Array.Length];

                for (int indexEmailReasonCode = 0; indexEmailReasonCode <= (int)p_emailReasonCode_Array.Length - 1; indexEmailReasonCode++)
                {
                    P_EMAIL = "N"; CC_EMAIL = "N";
                    BC_EMAIL = "N"; NO_EMAIL = "N";

                    switch (emailConditionSelected[indexEmailReasonCode])
                    {
                        case "P": P_EMAIL = "Y"; break;                // set the value "Y" or "N" for 
                        case "C": CC_EMAIL = "Y"; break;
                        case "B": BC_EMAIL = "Y"; break;
                        case "N": NO_EMAIL = "Y"; break;
                    }

                    emailCX[indexEmailReasonCode, 0] = P_EMAIL;
                    emailCX[indexEmailReasonCode, 1] = CC_EMAIL;
                    emailCX[indexEmailReasonCode, 2] = BC_EMAIL;
                    emailCX[indexEmailReasonCode, 3] = NO_EMAIL;
                }

                // in emailCX[x, y]  x = emailReasonCode ("41", "42", "43" ... etc.)  established by the instruction: for ( int indexEmailReasonCode = 0; indexEmailReasonCode <= (int)p_emai... etc. as seen above
                // and               y = emailCondition  ("P", "C", "B"  ... etc.) established by the instruction: switch (emailConditionSelected[indexEmailReasonCode]) { case "P":  P_EMAIL = "Y"; break; ... etc. as seen above
                // because Oracle Array Binded parameters are implemented, the matrix emailCX is transposed (interchanging rows for columns, and columns for rows) with respect of the matrix (table) of the UI

                string[] p_p_email_Array = new string[] { emailCX[0, 0], emailCX[1, 0], emailCX[2, 0], emailCX[3, 0], emailCX[4, 0], emailCX[5, 0], emailCX[6, 0] }; // P_EMAIL's yes or not for each condition (Database Error or Inspector Determ... etc.)
                string[] p_cc_email_Array = new string[] { emailCX[0, 1], emailCX[1, 1], emailCX[2, 1], emailCX[3, 1], emailCX[4, 1], emailCX[5, 1], emailCX[6, 1] }; //CC_EMAIL's yes or not for each condition (Database Error or Inspector Determ... etc.)
                string[] p_bc_email_Array = new string[] { emailCX[0, 2], emailCX[1, 2], emailCX[2, 2], emailCX[3, 2], emailCX[4, 2], emailCX[5, 2], emailCX[6, 2] }; //BC_EMAIL's yes or not for each condition (Database Error or Inspector Determ... etc.)
                string[] p_no_email_Array = new string[] { emailCX[0, 3], emailCX[1, 3], emailCX[2, 3], emailCX[3, 3], emailCX[4, 3], emailCX[5, 3], emailCX[6, 3] }; //NO_EMAIL's yes or not for each condition (Database Error or Inspector Determ... etc.)

                string connectionString = ConfigurationManager.ConnectionStrings["DataConnectionStringMINE"].ConnectionString;

                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    con.Open();

                    OracleCommand cmd = new OracleCommand("strk_admin_api.upd_junc_email_dest", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ArrayBindCount = p_emailReasonCode_Array.Length;
                    OracleTransaction oracle_Transaction;

                    cmd.Connection = con;
                    oracle_Transaction = con.BeginTransaction(); //(IsolationLevel.ReadCommitted);    //starts local transaction
                    cmd.Transaction = oracle_Transaction;           //assign transaction object for a pending local transaction

                    try
                    {
                        cmd.Parameters.Add("p_user_id", OracleDbType.Varchar2).Value = p_emailUserID_Array;
                        cmd.Parameters.Add("p_email_reason_code", OracleDbType.Varchar2).Value = p_emailReasonCode_Array;
                        cmd.Parameters.Add("p_p_email", OracleDbType.Varchar2).Value = p_p_email_Array;
                        cmd.Parameters.Add("p_cc_email", OracleDbType.Varchar2).Value = p_cc_email_Array;
                        cmd.Parameters.Add("p_bc_email", OracleDbType.Varchar2).Value = p_bc_email_Array;
                        cmd.Parameters.Add("p_no_email", OracleDbType.Varchar2).Value = p_no_email_Array;
                        cmd.Parameters.Add("V_OUTPUT", OracleDbType.RefCursor, ParameterDirection.Output);

                        cmd.ExecuteNonQuery();
                        oracle_Transaction.Commit();

                    }
                    catch (Exception err)
                    {
                        oracle_Transaction.Rollback();
                        string errMssg = "Error inserting emails in table: " + err.Message;
                        mssgBack = "Error saving emails' data, database was reset, check if connection is up and try again";
                        //throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
                return mssgBack;
            }
        }
    }
}
