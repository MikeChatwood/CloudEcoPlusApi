using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoSendCommandList
{

    public class tInput
    {

        public string SerialNumber { get; set; } = null;
        public string IMEI { get; set; } = null;
        public List<tCommand> Commands { get; set; } = new List<tCommand>();

        public class tCommand
        {

            public bool ToEco { get; set; } = true;

            public string UrlPath { get; set; } = null;
            public string PathElement { get; set; } = null;

            public DateTime? StartTime { get; set; } = null;
            public DateTime? EndTime { get; set; } = null;



            public string CommandName { get; set; } = null;
            public string CommandJson { get; set; } = null;

            public string CompanionName { get; set; } = null;
            public string CompanionJson { get; set; } = null;


        }
    };
    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

        public string SerialNumber { get; set; } = null;
        public List<tCommand> Commands { get; set; } = new List<tCommand>();

        public class tCommand
        {

            public string ReplyJson { get; set; } = null;
            public string PostStatus { get; set; } = null; //  Ok / OffLine / Timeout / NotSent

            public bool ToEco { get; set; } = true;

            public string PathElement { get; set; } = null;

            public DateTime? StartTime { get; set; } = null;
            public DateTime? EndTime { get; set; } = null;



            public string CommandName { get; set; } = null;
            public string CommandJson { get; set; } = null;

            public string CompanionName { get; set; } = null;
            public string CompanionJson { get; set; } = null;




        }


    }


    public class CloudEcoSendCommandList
    {

        const string strApiUrl = "https://ndygcmuadj.execute-api.eu-west-2.amazonaws.com/1/";
        const string strApiKey = "RhTHczPuO97m6D7E1uTg6ajnWKbN3zlhaaf6x2cv";

        public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {
            tResult.tCommand oCommandReply;
            tResult oResult = new tResult();
            string strIMEI;
            SqlConnection oSqlConnection = null;

            try
            {

                context.Logger.LogLine("FunctionHandler 1 " + oInput.ToString());

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

                if (oInput.IMEI == null)
                {
                    strIMEI = ecoCommon.GetDeviceIMEINumber(oInput.SerialNumber, context, ref oSqlConnection);
                }
                else
                {
                    strIMEI = oInput.IMEI;
                };



                foreach (tInput.tCommand oCommand in oInput.Commands)
                {

                    oCommandReply = PostApi(oCommand, strIMEI);
                    oResult.Commands.Add(oCommandReply);

                    if (oCommandReply.PostStatus != "ok")
                    {
                        oResult.Ok = false;
                        oResult.Info = "A command failed";

                    }
                }


            }
            catch (Exception ex)
            {
                context.Logger.LogLine("WriteRecord Ex  1" + ex.Message);
            }

            oSqlConnection.Close(); ;

            return oResult;

        }


        private tResult.tCommand PostApi(tInput.tCommand oCommandSend, string strSerialNo)
        {
            HttpWebRequest request;
            HttpWebResponse response = null/* TODO Change to default(_) if this is not a reference type */;
            StreamReader reader;
            Uri address;
            string appId;
            string strResponse = "";
            UInt32 uInt32StartDatetime;
            UInt32 uInt32EndDatetime;

            Stream postStream = null;
            string strUrl;
            byte[] byteData;
            StringBuilder data;
            tResult.tCommand oCommandReply = new tResult.tCommand();

            oCommandReply.PostStatus = "offline";





            strUrl = strApiUrl;

            strUrl = strUrl + oCommandSend.UrlPath;
            strUrl = strUrl.Replace("{devid}", strSerialNo);
            strUrl = strUrl.Replace("{pathelement}", oCommandSend.PathElement);

            if (oCommandSend.StartTime != null && oCommandSend.EndTime != null)
            {
                uInt32StartDatetime = DateTimeToUnixTime(oCommandSend.StartTime);
                uInt32EndDatetime = DateTimeToUnixTime(oCommandSend.EndTime);
                strUrl = strUrl + "?startTime=" + uInt32StartDatetime.ToString() + "&endTime=" + uInt32EndDatetime.ToString();
            }



            address = new Uri(strUrl);

            try
            {

                // The reply is mainly the same as the send, for easier processing by the client, ie send and reply together

                oCommandReply.CommandJson = oCommandSend.CommandJson;
                oCommandReply.CommandName = oCommandSend.CommandName;
                oCommandReply.CompanionJson = oCommandSend.CompanionJson;
                oCommandReply.CompanionName = oCommandSend.CompanionName;
                oCommandReply.EndTime = oCommandSend.EndTime;
                oCommandReply.PathElement = oCommandSend.PathElement;
                oCommandReply.StartTime = oCommandSend.StartTime;
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

                appId = "ss-form";
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
        private static UInt32 DateTimeToUnixTime(DateTime? aDate)
        {
            UInt32 intReturn;

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = (DateTime)aDate - origin;

            intReturn = (UInt32)diff.TotalSeconds;

            return intReturn;
        }




    }
}
