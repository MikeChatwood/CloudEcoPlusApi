using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoSensorIntervalSet
{


    public class tInput
    {
        public string SerialNumber { get; set; } = null;
        public int SensorInterval { get; set; }
        public int RevertWindowMinutes { get; set; }
    };


    public class tResult
    {
        public bool Ok { get; set; } = false;
        public string Info { get; set; } = "";

    }



    public class tRevertCommand
    {
        public string MessageName { get; set; } = "diagintervalrevert";
        public int SensorInterval { get; set; }
        public string RevertAfter { get; set; }
        public string IMEI { get; set; }
    }

    public class tCommand
    {

        public string ReplyJson { get; set; } = null;
        public string PostStatus { get; set; } = null; //  Ok / OffLine / Timeout / NotSent

        public bool ToEco { get; set; } = true;
        public string UrlPath = "";

        public string PathElement { get; set; } = null;

        public DateTime? StartTime { get; set; } = null;
        public DateTime? EndTime { get; set; } = null;



        public string CommandName { get; set; } = null;
        public string CommandJson { get; set; } = null;

        public string CompanionName { get; set; } = null;
        public string CompanionJson { get; set; } = null;




    }


    public class diags
    {
        public int controlInterval { get; set; }
        public int sensorInterval { get; set; }
    }

    public class CloudEcoSensorIntervalSet
    {

        private static readonly AmazonKinesisClient kinesisClient = new AmazonKinesisClient(RegionEndpoint.EUWest2);

        const string strApiUrl = "https://ndygcmuadj.execute-api.eu-west-2.amazonaws.com/1/";
        const string strApiKey = "RhTHczPuO97m6D7E1uTg6ajnWKbN3zlhaaf6x2cv";

        public async Task<tResult> FunctionHandler(tInput oInput, ILambdaContext context)
        {

            SqlConnection oSqlConnection = null;
            string strIMEI;
            tCommand oCommandSend;
            tResult oResult = new tResult();
            diags oCurrentDiag;
            diags oNewDiag = new diags();
            tRevertCommand oRevertCommand = new tRevertCommand();
            DateTime datRevertAfter;

            /*
             *  Get the IMEI
             *  Get the current 
             *  Reconfigure to supplied values
             *  If the current is > 45 seconds 
             *      Queue a message on Kinesis, which will be ignored until the time is past. When time is passed the values are returned to the original values
             * 
             * 
             */

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


            strIMEI = ecoCommon.GetDeviceIMEINumber(oInput.SerialNumber, context, ref oSqlConnection);


            // --Get Current values (diags)  -----------------------------------------------------------------

            oCommandSend = new tCommand();

            oCommandSend.CommandName = "diags";
            oCommandSend.UrlPath = "diags/{devid}";
            oCommandSend.CommandJson = "";
            oCommandSend.ToEco = false;

            tCommand oControlreportGetReply = new tCommand();

            oControlreportGetReply = PostApi(oCommandSend, strIMEI);
            if (oControlreportGetReply.PostStatus != "ok")
            {
                oResult.Ok = false;
                oResult.Info = oControlreportGetReply.PostStatus;
                return oResult;
            };


            oCurrentDiag = JsonSerializer.Deserialize<diags>(oControlreportGetReply.ReplyJson);

            // -- End Command -----------------------------------------------------------------



            // --Send New values diags -----------------------------------------------------------------

            oCommandSend = new tCommand();

            oNewDiag.controlInterval = oInput.SensorInterval;
            oNewDiag.sensorInterval = oInput.SensorInterval;

            oCommandSend.CommandName = "diags";
            oCommandSend.UrlPath = "diags/{devid}";
            oCommandSend.CommandJson = JsonSerializer.Serialize(oNewDiag);
            oCommandSend.ToEco = true;

            tCommand oControlreportSetReply = new tCommand();

            PostApiAsync(oCommandSend, strIMEI);



            //oControlreportSetReply = PostApi(oCommandSend, strIMEI);
            //if (oControlreportSetReply.PostStatus != "ok")
            //{
            //    oResult.Ok = false;
            //    oResult.Info = oControlreportSetReply.PostStatus;
            //    return oResult;
            //};



            // -- End Command -----------------------------------------------------------------


            if (oCurrentDiag.sensorInterval > 45)  // Only revert back if > 45 seconds, less then is considered as already being in test
            {
                datRevertAfter = DateTime.Now.AddMinutes(oInput.RevertWindowMinutes);
                oRevertCommand.SensorInterval = oCurrentDiag.sensorInterval;

                oRevertCommand.RevertAfter = ecoCommon.JsonDate(datRevertAfter);
                oRevertCommand.IMEI = strIMEI;


                context.Logger.LogLine("FunctionHandler Set Revert " + oRevertCommand.RevertAfter + " From " + datRevertAfter.ToString());


                oResult = await WriteStream(oRevertCommand, context);  // Write to Kinesis

            }


            return oResult;

        }

        private async void PostApiAsync(tCommand oCommandSend, string strIMEI)
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
            tCommand oCommandReply = new tCommand();

            oCommandReply.PostStatus = "offline";





            strUrl = strApiUrl;

            strUrl = strUrl + oCommandSend.UrlPath;
            strUrl = strUrl.Replace("{devid}", strIMEI);
            strUrl = strUrl.Replace("{pathelement}", oCommandSend.PathElement);

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

                        //return oCommandReply;
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


            // return oCommandReply;
        }

        private tCommand PostApi(tCommand oCommandSend, string strIMEI)
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
            tCommand oCommandReply = new tCommand();

            oCommandReply.PostStatus = "offline";





            strUrl = strApiUrl;

            strUrl = strUrl + oCommandSend.UrlPath;
            strUrl = strUrl.Replace("{devid}", strIMEI);
            strUrl = strUrl.Replace("{pathelement}", oCommandSend.PathElement);

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


        private static async Task<tResult> WriteStream(tRevertCommand oRevertCommand, ILambdaContext context)
        {

            const string myStreamName = "CloudEcoPlus";
            string strInput;
            string strEncoded;
            tResult oResult = new tResult();

            try
            {

                context.Logger.LogLine("Putting records in stream : " + myStreamName);

                // Write 10 UTF-8 encoded records to the stream.

                PutRecordRequest requestRecord = new PutRecordRequest();

                requestRecord.StreamName = myStreamName;

                strInput = JsonSerializer.Serialize(oRevertCommand);
                requestRecord.Data = new MemoryStream(Encoding.UTF8.GetBytes(strInput));
                context.Logger.LogLine("Putting records in stream 1 : " + myStreamName);

                strEncoded = Base64Encode(strInput);  // just for debug base64 encoding

                //
                context.Logger.LogLine("Putting records in stream 2 : " + myStreamName);

                requestRecord.PartitionKey = "partitionKey";

                context.Logger.LogLine("Putting records in stream 3 : " + myStreamName);

                PutRecordResponse PutRecordResult = await kinesisClient.PutRecordAsync(requestRecord);
                context.Logger.LogLine("PutRecordResult ok" + PutRecordResult.HttpStatusCode);




            }

            catch (Exception ex)
            {

                oResult.Ok = false;
                oResult.Info = ex.Message;
                context.Logger.LogLine("Error WriteStream ");
            }

            context.Logger.LogLine("Putting records in stream 4");
            return oResult;

        }



        private static string Base64Encode(string input)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(data);
        }



    }
}
