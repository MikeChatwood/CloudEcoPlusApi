using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoSensorIntervalRevert
{


    public class tRevertCommand
    {
        public string MessageName { get; set; } = "diagintervalrevert";
        public int SensorInterval { get; set; }
        public string RevertAfter { get; set; }
        public string IMEI { get; set; }
        public string MakeItFail { get; set; }
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

    public class CloudEcoSensorIntervalRevert
    {


        const string strApiUrl = "https://ndygcmuadj.execute-api.eu-west-2.amazonaws.com/1/";
        const string strApiKey = "RhTHczPuO97m6D7E1uTg6ajnWKbN3zlhaaf6x2cv";

        public void FunctionHandler(KinesisEvent kinesisEvent, ILambdaContext context)
        {

            tRevertCommand oRevertCommand;
            DateTime datRevert;
            diags oNewDiag = new diags();
            tCommand oCommandSend;

            context.Logger.LogLine("FunctionHandler 1 0945");



           // context.Logger.LogLine("FunctionHandler 3");

          //  context.Logger.LogLine($"Beginning to process {kinesisEvent.Records.Count} records...");

            foreach (var record in kinesisEvent.Records)
            {

                string recordData = GetRecordContents(record.Kinesis);


                oRevertCommand = JsonSerializer.Deserialize<tRevertCommand>(recordData);

                if (oRevertCommand.MessageName.ToLower() != "diagintervalrevert")
                {
                    continue;
                }

                context.Logger.LogLine("FunctionHandler rd  >" + recordData);

                datRevert = ecoCommon.DateJson(oRevertCommand.RevertAfter);
                if (DateTime.Compare(DateTime.Now, datRevert) < 0)
                {

                    context.Logger.LogLine("FunctionHandler Not my time " + datRevert.ToString () + " " + DateTime.Now.ToString());

                    context.Logger.LogLine(oRevertCommand.MakeItFail.ToString());

                    continue;  // Not my time, let the stream represent until ready
                }


                // --Send New values diags -----------------------------------------------------------------

                oCommandSend = new tCommand();

                oNewDiag.controlInterval = oRevertCommand.SensorInterval;
                oNewDiag.sensorInterval = oRevertCommand.SensorInterval;

                oCommandSend.CommandName = "diags";
                oCommandSend.UrlPath = "diags/{devid}";
                oCommandSend.CommandJson = JsonSerializer.Serialize(oNewDiag);
                oCommandSend.ToEco = true;

                tCommand oControlreportSetReply = new tCommand();

                oControlreportSetReply = PostApi(oCommandSend, oRevertCommand.IMEI);
                //if (oControlreportSetReply.PostStatus != "ok")
                //{
                //    oResult. = false;
                //    return oResult;
                //};



                // -- End Command -----------------------------------------------------------------



               // context.Logger.LogLine("Stream processing complete.");
            }
        }
        private string GetRecordContents(KinesisEvent.Record streamRecord)
        {
            using (var reader = new StreamReader(streamRecord.Data, Encoding.ASCII))
            {
                return reader.ReadToEnd();
            }
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







    }
}