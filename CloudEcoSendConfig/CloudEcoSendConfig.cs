using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Amazon.Lambda.Core;
using System.Data.SqlClient;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoSendConfig
{

    public class tInput
    {
        public string SerialNumber { get; set; } = null;
        public int PropertyID { get; set; } = -1;
        public int EhiuSiteConfigID { get; set; } = -1;
        public string UserName { get; set; } = null;

    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";
        public string InfoDetail { get; set; } = "";
    }

    public class CloudEcoSendConfig
    {

        const string strApiUrl = "https://ndygcmuadj.execute-api.eu-west-2.amazonaws.com/1/";
        const string strApiKey = "RhTHczPuO97m6D7E1uTg6ajnWKbN3zlhaaf6x2cv";

        //  public List<tCommandSend> CommandsSend { get; set; } = new List<tCommandSend>();

        public class tCommandSend
        {

            public bool ToEco { get; set; } = true;
            public string UrlPath { get; set; } = null;
            public string PathElement { get; set; } = null;
            public int MbusID { get; set; } = -1;
            public string CommandName { get; set; } = null;
            public string CommandJson { get; set; } = null;
            public string CompanionName { get; set; } = null;
            public string CompanionJson { get; set; } = null;


        }

        public class tCommandReply
        {

            public string ReplyJson { get; set; } = null;
            public int MbusID { get; set; } = -1;
            public bool ToEco { get; set; } = true;
            public string CommandName { get; set; } = null;
            public string CommandJson { get; set; } = null;
            public string CompanionName { get; set; } = null;
            public string CompanionJson { get; set; } = null;
            public string PostStatus { get; set; } = null; //  Ok / OffLine / Timeout / NotSent
            public string PostStatusDetail { get; set; } = null;
        }
        public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {

            DataSet dsCommands;
            SqlConnection oSqlConnection = null;
            int intIdx;
            tResult oResult = new tResult();
            int intEhiuInstallID;
            int intEhiuInstallDetailID;
            string strConfigName;
            int intMbusID = -1;
            string strIMEI;

            // Get profile commands
            // Call ehiu api to send
            // register property/history



            try
            {
                oSqlConnection = new SqlConnection(ecoCommon.GetSecret("CloudEcoPlus", context)); oSqlConnection.Open();
                context.Logger.LogLine("FunctionHandler 2");
            }
            catch (Exception ex)
            {
                context.Logger.LogLine("WriteRecord Ex  1" + ex.Message);
            }

            try
            {
                oSqlConnection.Open();
            }
            catch (Exception)
            {
            }

            try
            {
                strIMEI = ecoCommon.GetDeviceIMEINumber(oInput.SerialNumber, context, ref oSqlConnection);
                if (strIMEI == "")
                {

                    context.Logger.LogLine("Not recognised serial number " + oInput.SerialNumber);
                    oResult.Ok = false;
                    oResult.Info = "Not recognised serial number";
                    return oResult;
                };

                dsCommands = getCommands(oInput.EhiuSiteConfigID, context, ref oSqlConnection);

                strConfigName = (string)dsCommands.Tables[0].Rows[0]["Name"];

                intEhiuInstallID = getExistingEcoInstallThisSerialNumber(oInput.SerialNumber, oInput.PropertyID, context, ref oSqlConnection);
                if (intEhiuInstallID != -1)
                {
                    // This unit as already installed in another property, cant procede

                    oResult.Ok = false;
                    oResult.Info = "This Eco+ is already installed in another property";

                    return oResult;
                }



                //intEhiuInstallID = getExistingEcoInstallThisProperty(oInput.SerialNumber, oInput.PropertyID, context, ref oSqlConnection);
                //if (intEhiuInstallID == -1)
                //{
                //    intEhiuInstallID = RegisterUnit(oInput.PropertyID, oInput.SerialNumber, oInput.EhiuSiteConfigID, strConfigName, oInput.UserName, context, ref oSqlConnection); // Add 
                //};

                //intEhiuInstallDetailID = RegisterConfigApplication(intEhiuInstallID, oInput.EhiuSiteConfigID, DateTime.Now, oInput.UserName, context, ref oSqlConnection);


                for (intIdx = 0; intIdx <= dsCommands.Tables[0].Rows.Count - 1; intIdx++)
                {

                    tCommandSend oCommandSend = new tCommandSend();

                    oCommandSend.CommandJson = (string)dsCommands.Tables[0].Rows[intIdx]["JsonConfigItem"];
                    oCommandSend.CommandName = (string)dsCommands.Tables[0].Rows[intIdx]["CommandName"];

                    if ((string)dsCommands.Tables[0].Rows[intIdx]["JsonCompanionItem"] != "")
                    {
                        oCommandSend.CompanionJson = (string)dsCommands.Tables[0].Rows[intIdx]["JsonCompanionItem"];
                    };

                    if ((string)dsCommands.Tables[0].Rows[intIdx]["CompanionName"] != "")
                    {
                        oCommandSend.CompanionName = (string)dsCommands.Tables[0].Rows[intIdx]["CompanionName"];
                    };


                    oCommandSend.MbusID = (int)dsCommands.Tables[0].Rows[intIdx]["MbusID"];



                    oCommandSend.ToEco = (bool)dsCommands.Tables[0].Rows[intIdx]["ToEco"];

                    oCommandSend.UrlPath = (string)dsCommands.Tables[0].Rows[intIdx]["UrlPath"];

                    oCommandSend.PathElement = "";
                    if (oCommandSend.MbusID != -1)
                    {
                        intMbusID = oCommandSend.MbusID;
                        oCommandSend.PathElement = intMbusID.ToString();
                    }


                    tCommandReply oCommandReply;
                    oCommandReply = PostApi(oCommandSend, strIMEI);


                    if (oCommandReply.PostStatus != "ok")
                    {

                        oResult.Ok = false;
                        oResult.Info = oCommandReply.PostStatus;
                        if (oCommandReply.PostStatusDetail != null)
                        {
                            oResult.InfoDetail = oCommandReply.PostStatusDetail;
                        }

                        break;
                    }
                    else
                    {

                        intEhiuInstallID = getExistingEcoInstallThisProperty(oInput.SerialNumber, oInput.PropertyID, context, ref oSqlConnection);
                        if (intEhiuInstallID == -1)
                        {
                            intEhiuInstallID = RegisterUnit(oInput.PropertyID, oInput.SerialNumber, oInput.EhiuSiteConfigID, strConfigName, oInput.UserName, context, ref oSqlConnection); // Add 
                        };

                        intEhiuInstallDetailID = RegisterConfigApplication(intEhiuInstallID, oInput.EhiuSiteConfigID, DateTime.Now, oInput.UserName, context, ref oSqlConnection);


                        WriteRegisterEcoDetailCommands(intEhiuInstallDetailID,
                                                                   oCommandSend.CompanionName,
                                                                   oCommandSend.CompanionJson,
                                                                   oCommandSend.CommandName,
                                                                   oCommandSend.CommandJson,
                                                                   oCommandReply.ReplyJson,
                                                                   oInput.UserName,
                                                                   oCommandSend.MbusID,
                                                                   context, ref oSqlConnection);
                    }


                };

            }
            catch (Exception ex)
            {

                oResult.Ok = false;
                oResult.Info = "Internal error";
                context.Logger.LogLine("WriteRecord Ex 2" + ex.Message);
            }
            return oResult;

        }



        private void WriteRegisterEcoDetailCommands(int intEhiuInstallDetailID, string strCompanionName, string strCompanionItemJson, string strCommandName,
                            string strSendJson, string strReplyJson, string strEnginerName, int intMBusID, ILambdaContext context, ref SqlConnection oSqlConnection)
        {
            string strQuery;

            try
            {
                oSqlConnection.Open();
            }
            catch (Exception)
            {
            }

            try
            {
                strQuery = "Insert Into EhiuInstallDetailCommands( EhiuInstallDetailID, CompanionName, CompanionItemJson, DateCreated, CreatedBy, CommandName, SendJson, ReplyJson, MbusID ) " +
                            "                       Values (@EhiuInstallDetailID, @CompanionName, @CompanionItemJson, @DateCreated, @CreatedBy, @CommandName, @SendJson, @ReplyJson, @MBusID ) ";

                SqlCommand sqlCommInsert = new SqlCommand(strQuery, oSqlConnection);

                SqlParameter aParamMBusID = new SqlParameter("@MBusID", SqlDbType.Int);
                if (intMBusID != -1)
                {
                    aParamMBusID.Value = intMBusID;
                }
                else
                {
                    aParamMBusID.Value = DBNull.Value;
                };
                sqlCommInsert.Parameters.Add(aParamMBusID);

                SqlParameter aParamCommandName = new SqlParameter("@CommandName", SqlDbType.NVarChar);
                aParamCommandName.Value = strCommandName;
                sqlCommInsert.Parameters.Add(aParamCommandName);

                SqlParameter aParamSendJson = new SqlParameter("@SendJson", SqlDbType.NVarChar);
                aParamSendJson.Value = strSendJson;
                sqlCommInsert.Parameters.Add(aParamSendJson);

                SqlParameter aParamReplyJson = new SqlParameter("@ReplyJson", SqlDbType.NVarChar);
                aParamReplyJson.Value = strReplyJson;
                sqlCommInsert.Parameters.Add(aParamReplyJson);


                SqlParameter aParamEhiuInstallDetailID = new SqlParameter("@EhiuInstallDetailID", SqlDbType.Int);
                aParamEhiuInstallDetailID.Value = intEhiuInstallDetailID;
                sqlCommInsert.Parameters.Add(aParamEhiuInstallDetailID);

                SqlParameter aParamCompanionName = new SqlParameter("@CompanionName", SqlDbType.NVarChar);
                if (strCompanionName is null)
                {
                    aParamCompanionName.Value = DBNull.Value;
                }
                else
                {
                    aParamCompanionName.Value = strCompanionName;
                }
                sqlCommInsert.Parameters.Add(aParamCompanionName);

                SqlParameter aParamCompanionItemJson = new SqlParameter("@CompanionItemJson", SqlDbType.NVarChar);
                aParamCompanionItemJson.Value = strCompanionItemJson;
                sqlCommInsert.Parameters.Add(aParamCompanionItemJson);

                SqlParameter aParamDateCreated = new SqlParameter("@DateCreated", SqlDbType.DateTime);
                aParamDateCreated.Value = DateTime.Now;
                sqlCommInsert.Parameters.Add(aParamDateCreated);

                SqlParameter aParamCreatedBy = new SqlParameter("@CreatedBy", SqlDbType.NVarChar);
                aParamCreatedBy.Value = strEnginerName;
                sqlCommInsert.Parameters.Add(aParamCreatedBy);


                sqlCommInsert.ExecuteNonQuery();

                oSqlConnection.Close();
            }
            catch (Exception ex)
            {
                context.Logger.LogLine("WriteRegisterEcoDetailCommands Ex 2" + ex.Message);
            }
        }




        private DataSet getCommands(int intEhiuSiteConfigID, ILambdaContext context, ref SqlConnection oSqlConnection)
        {

            string strQuery = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();
            tResult oResult = new tResult();

            try
            {

                context.Logger.LogLine("getCommands 1 " + intEhiuSiteConfigID.ToString());

                strQuery = "SELECT EhiuSiteConfigDetail.EhiuSiteConfigID, EhiuSiteConfigDetail.CommandName, EhiuSiteConfigDetail.ToEco, EhiuSiteConfigDetail.JsonConfigItem, " +
                            " ISNULL(EhiuSiteConfigDetail.CompanionName, '') AS CompanionName, EhiuSiteConfigDetail.UrlPath, " +
                            " ISNULL(EhiuSiteConfigDetail.JsonCompanionItem, '') AS JsonCompanionItem, ISNULL(EhiuSiteConfigDetail.MbusID, - 1) AS MbusID, EhiuSiteConfig.Name " +
                            " FROM EhiuSiteConfigDetail INNER JOIN " +
                            " EhiuSiteConfig ON EhiuSiteConfigDetail.EhiuSiteConfigID = EhiuSiteConfig.EhiuSiteConfigID " +
                            " WHERE (EhiuSiteConfigDetail.EhiuSiteConfigID = @EhiuSiteConfigID)";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamEhiuSiteConfigID = new SqlParameter("@EhiuSiteConfigID", SqlDbType.Int);
                sqlParamEhiuSiteConfigID.Value = intEhiuSiteConfigID;
                daCheck.SelectCommand.Parameters.Add(sqlParamEhiuSiteConfigID);

                daCheck.Fill(dsCheck);


            }
            catch (Exception ex)
            {
                context.Logger.LogLine("getCommands ex " + ex.Message);
            }
            return dsCheck;
        }


        private int RegisterConfigApplication(int intEhiuInstallID, int intEhiuSiteConfigID, DateTime datDateCreated, string strCreatedBy, ILambdaContext context, ref SqlConnection oSqlConnection)
        {
            int intReturn = -1;

            string strQuery;

            try
            {
                strQuery = "USP_CreateEhiuInstallDetail";
                SqlCommand sqlCommTmp = new SqlCommand(strQuery, oSqlConnection);
                try
                {
                    sqlCommTmp.Connection.Open();
                }
                catch (Exception)
                {
                }

                sqlCommTmp.CommandType = CommandType.StoredProcedure;

                sqlCommTmp.Parameters.AddWithValue("@EhiuInstallDetailID", 0);
                sqlCommTmp.Parameters[0].SqlDbType = SqlDbType.Int;
                sqlCommTmp.Parameters[0].Direction = ParameterDirection.Output;

                sqlCommTmp.Parameters.AddWithValue("@EhiuInstallID", intEhiuInstallID);
                sqlCommTmp.Parameters.AddWithValue("@EhiuSiteConfigID", intEhiuSiteConfigID);
                sqlCommTmp.Parameters.AddWithValue("@DateCreated", datDateCreated);
                sqlCommTmp.Parameters.AddWithValue("@CreatedBy", strCreatedBy);

                sqlCommTmp.ExecuteNonQuery();
                intReturn = (int)sqlCommTmp.Parameters[0].Value;
            }

            catch (Exception ex)
            {
            }

            return intReturn;
        }


        private static int RegisterUnit(int intPropertyID, string strSerialNumber, int intEhiuSiteConfigID, string strConfigName, string strUserName, ILambdaContext context, ref SqlConnection oSqlConnection)
        {
            int intInvoiceLineID = -1;
            DateTime DatReturn = DateTime.Now;
            string strQuery;

            try
            {

                strQuery = "USP_CreateEhiuInstall";
                SqlCommand sqlCommTmp = new SqlCommand(strQuery, oSqlConnection);

                try
                {
                    sqlCommTmp.Connection.Open();
                }
                catch (Exception)
                {
                }

                sqlCommTmp.CommandType = CommandType.StoredProcedure;
                sqlCommTmp.Parameters.AddWithValue("@EhiuInstallID", 0);
                sqlCommTmp.Parameters[0].SqlDbType = SqlDbType.Int;
                sqlCommTmp.Parameters[0].Direction = ParameterDirection.Output;

                sqlCommTmp.Parameters.AddWithValue("@PropertyId", intPropertyID);
                sqlCommTmp.Parameters.AddWithValue("@SerialNumber", strSerialNumber);
                sqlCommTmp.Parameters.AddWithValue("@FromDate", DateTime.Now.Date);
                sqlCommTmp.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                sqlCommTmp.Parameters.AddWithValue("@CreatedBy", strUserName);


                sqlCommTmp.ExecuteNonQuery();
                intInvoiceLineID = (int)sqlCommTmp.Parameters[0].Value;

                sqlCommTmp.Connection.Close();


            }
            catch (Exception ex)
            {
                context.Logger.LogLine("RegisterUnit Ex " + ex.Message);
            }
            return intInvoiceLineID;
        }



        private int getExistingEcoInstallThisProperty(string strSerialNumber, int intPropertyID, ILambdaContext context, ref SqlConnection oSqlConnection)
        {

            string strQuery = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();
            tResult oResult = new tResult();
            int intIdx;
            int intReturn = -1;

            try
            {

                context.Logger.LogLine("getExistingEcoInstallThisProperty 1 " + strSerialNumber.ToString());

                strQuery = "SELECT TOP (1) EhiuInstallID " +
                            " FROM EhiuInstall  " +
                            " WHERE (PropertyID = @PropertyID) AND  " +
                            " ToDate Is Null And  " +
                            " SerialNumber = @SerialNumber";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamPropertyID = new SqlParameter("@PropertyID", SqlDbType.Int);
                sqlParamPropertyID.Value = intPropertyID;
                daCheck.SelectCommand.Parameters.Add(sqlParamPropertyID);

                SqlParameter sqlParamSerialNumber = new SqlParameter("@SerialNumber", SqlDbType.NVarChar);
                sqlParamSerialNumber.Value = strSerialNumber;
                daCheck.SelectCommand.Parameters.Add(sqlParamSerialNumber);

                daCheck.Fill(dsCheck);


                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {
                    intReturn = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuInstallID"];
                }

            }
            catch (Exception)
            {
            }
            return intReturn;
        }

        private int getExistingEcoInstallThisSerialNumber(string strSerialNumber, int intPropertyID, ILambdaContext context, ref SqlConnection oSqlConnection)
        {

            string strQuery = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();
            tResult oResult = new tResult();
            int intIdx;
            int intReturn = -1;

            try
            {

                context.Logger.LogLine("getExistingEcoInstallThisProperty 1 " + strSerialNumber.ToString());

                strQuery = "SELECT TOP (1) EhiuInstallID " +
                            " FROM EhiuInstall  " +
                            " WHERE (PropertyID <> @PropertyID) AND  " +
                            " ToDate Is Null And  " +
                            " SerialNumber = @SerialNumber";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamPropertyID = new SqlParameter("@PropertyID", SqlDbType.Int);
                sqlParamPropertyID.Value = intPropertyID;
                daCheck.SelectCommand.Parameters.Add(sqlParamPropertyID);

                SqlParameter sqlParamSerialNumber = new SqlParameter("@SerialNumber", SqlDbType.NVarChar);
                sqlParamSerialNumber.Value = strSerialNumber;
                daCheck.SelectCommand.Parameters.Add(sqlParamSerialNumber);

                daCheck.Fill(dsCheck);


                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {
                    intReturn = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuInstallID"];
                }

            }
            catch (Exception)
            {
            }
            return intReturn;
        }





        private tCommandReply PostApi(tCommandSend oCommandSend, string strSerialNo)
        {
            HttpWebRequest request;
            HttpWebResponse response = null/* TODO Change to default(_) if this is not a reference type */;
            StreamReader reader;
            Uri address;
            string strResponse = "";

            Stream postStream = null;
            string strUrl;
            byte[] byteData;
            StringBuilder data;
            tCommandReply oCommandReply = new tCommandReply();

            oCommandReply.PostStatus = "offline";





            strUrl = strApiUrl;

            strUrl = strUrl + oCommandSend.UrlPath;
            strUrl = strUrl.Replace("{devid}", strSerialNo);
            strUrl = strUrl.Replace("{pathelement}", oCommandSend.PathElement);




            address = new Uri(strUrl);

            try
            {

                // The reply is mainly the same as the send, for easier processing by the client, ie send and reply together

                oCommandReply.CommandJson = oCommandSend.CommandJson;
                oCommandReply.CommandName = oCommandSend.CommandName;
                oCommandReply.CompanionJson = oCommandSend.CompanionJson;
                oCommandReply.CompanionName = oCommandSend.CompanionName;
                oCommandReply.MbusID = oCommandSend.MbusID;
                oCommandReply.ToEco = oCommandSend.ToEco;

                request = (HttpWebRequest)WebRequest.Create(address);
                if (oCommandSend.ToEco == true)
                {
                    request.Method = "PATCH";
                }
                else
                {
                    request.Method = "GET";
                }

                request.ContentType = "application/x-www-form-urlencoded";

                request.Headers.Add("x-api-key", strApiKey);

                if (oCommandSend.ToEco == true)
                {
                    data = new StringBuilder();

                    data.Append(oCommandSend.CommandJson);

                    byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                    // Set the content length in the request headers  
                    request.ContentLength = byteData.Length;

                    // Write data  
                    try
                    {
                        postStream = request.GetRequestStream();
                        postStream.Write(byteData, 0, byteData.Length);
                    }
                    finally
                    {
                        if (postStream != null)
                            postStream.Close();
                    }
                }
                else
                {

                    if (postStream != null)
                        postStream.Close();
                }
                try
                {
                    // Get response  
                    response = (HttpWebResponse)request.GetResponse();

                    // Get the response stream into a reader  
                    reader = new StreamReader(response.GetResponseStream());

                    strResponse = reader.ReadToEnd();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        oCommandReply.PostStatus = "ok";
                        oCommandReply.ReplyJson = strResponse;

                        return oCommandReply;
                    }
                    else
                    {
                    }
                }

                catch (Exception ex)
                {

                    oCommandReply.PostStatusDetail = ex.Message;
                }
                finally
                {
                    if (response != null)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                            oCommandReply.PostStatus = "ok";

                        if (response.StatusCode.ToString().IndexOf("504") != -1)
                            oCommandReply.PostStatus = "timeout";
                        response.Close();
                    }
                    else
                        oCommandReply.PostStatus = "offline";
                }
            }
            catch (Exception ex)
            {
            }


            return oCommandReply;
        }
    }
}
