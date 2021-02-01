using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoGetSettings
{


    public class tInput
    {
        public string SerialNumber { get; set; } = null;

    };


    public class tResult
    {
        public bool HasError { get; set; } = false;
        public string Info { get; set; } = "";

        // Entitlement
        public string EntitlementSpaceHeating { get; set; } = "";
        public string EntitlementHotWater { get; set; } = "";

        // StayWarm
        public bool StayWarmEnabled { get; set; } = false;
        public decimal? StayWarmTemperature { get; set; } = 0;
        public decimal? StayWarmHysteresis { get; set; } = 0;

        // SpaceHeating
        public decimal? SpaceHeatingFlowSetPoint { get; set; } = 0;
        public decimal? SpaceHeatingReturnSetPoint { get; set; } = 0;
        public decimal? SpaceHeatingPumpMinPercent { get; set; }
        public decimal? SpaceHeatingPumpMaxPercent { get; set; }

        // DomesticHw
        public decimal? DomesticHwSetPoint { get; set; } = 0;





        public int LastSeenMinutesAgo { get; set; } = -1;

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


    public class entitlementconfig
    {
        public int? chEnabled { get; set; }
        public int? hwEnabled { get; set; }
        public int? ctlEnabled { get; set; }
    }


    public class keepwarmconfig
    {
        public int? enabled { get; set; }
        public decimal? hysteresisOver { get; set; }
        public decimal? temperature { get; set; }

    }

    public class hwconfig
    {
        public Decimal? sp { get; set; }
    }

    public class chconfig
    {
        public decimal? fsp { get; set; }
        public decimal? rsp { get; set; }
        public decimal? pmax { get; set; }
        public decimal? pmin { get; set; }


    }

    public class CloudEcoGetSettings
    {



        const string strApiUrl = "https://ndygcmuadj.execute-api.eu-west-2.amazonaws.com/1/";
        const string strApiKey = "RhTHczPuO97m6D7E1uTg6ajnWKbN3zlhaaf6x2cv";
         public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {

            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();

            SqlConnection oSqlConnection = null;
            tResult oResult = new tResult();

            tCommand oCommandSend;
            entitlementconfig oentitlementconfig;
            keepwarmconfig okeepwarmconfig;
            hwconfig ohwconfig;
            chconfig ochconfig;

            string strIMEI;

            try
            {

                // All values are returned as strings, ready for display

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


                strIMEI = ecoCommon.GetDeviceIMEINumber(oInput.SerialNumber, context, ref oSqlConnection);


                // get the last seen

                oResult.LastSeenMinutesAgo = GetLastSeen(strIMEI, context);

                // End Get last seen




                // -- Send Command -----------------------------------------------------------------
                oCommandSend = new tCommand();

                oCommandSend.CommandName = "chconfig";
                oCommandSend.UrlPath = "chconfig/{devid}";
                oCommandSend.CommandJson = "";
                oCommandSend.ToEco = false;

                tCommand oChconfigReply = new tCommand();

                oChconfigReply = PostApi(oCommandSend, strIMEI);
                if (oChconfigReply.PostStatus != "ok")
                {
                    oResult.HasError = true;
                    oResult.Info = "chconfig " + oChconfigReply.PostStatus;
                    return oResult;
                };


                ochconfig = JsonSerializer.Deserialize<chconfig>(oChconfigReply.ReplyJson);

                if (ochconfig.fsp != null)
                {
                    oResult.SpaceHeatingFlowSetPoint = ochconfig.fsp;
                }

                if (ochconfig.rsp != null)
                {
                    oResult.SpaceHeatingReturnSetPoint = ochconfig.rsp;
                }

                if (ochconfig.pmax != null)
                {
                    oResult.SpaceHeatingPumpMaxPercent = ochconfig.pmax;
                }

                if (ochconfig.pmin != null)
                {
                    oResult.SpaceHeatingPumpMinPercent = ochconfig.pmin;
                }


                // -- End Command -----------------------------------------------------------------



                // -- Send Command -----------------------------------------------------------------
                oCommandSend = new tCommand();

                oCommandSend.CommandName = "hwconfig";
                oCommandSend.UrlPath = "hwconfig/{devid}";
                oCommandSend.CommandJson = "";
                oCommandSend.ToEco = false;

                tCommand oHwConfigReply = new tCommand();

                oHwConfigReply = PostApi(oCommandSend, strIMEI);
                if (oHwConfigReply.PostStatus != "ok")
                {
                    oResult.HasError = true;
                    oResult.Info = "hwconfig " + oHwConfigReply.PostStatus;
                    return oResult;
                };


                ohwconfig = JsonSerializer.Deserialize<hwconfig>(oHwConfigReply.ReplyJson);

                if (ohwconfig.sp != null)
                {
                    oResult.DomesticHwSetPoint = ohwconfig.sp;
                }


                // -- End Command -----------------------------------------------------------------






                // -- Send Command -----------------------------------------------------------------
                oCommandSend = new tCommand();

                oCommandSend.CommandName = "keepwarmconfig";
                oCommandSend.UrlPath = "keepwarmconfig/{devid}";
                oCommandSend.CommandJson = "";
                oCommandSend.ToEco = false;

                tCommand oKeepWarmConfigReply = new tCommand();

                oKeepWarmConfigReply = PostApi(oCommandSend, strIMEI);
                if (oKeepWarmConfigReply.PostStatus != "ok")
                {
                    oResult.HasError = true;
                    oResult.Info = "keepwarmconfig " + oKeepWarmConfigReply.PostStatus;
                    return oResult;
                };


                okeepwarmconfig = JsonSerializer.Deserialize<keepwarmconfig>(oKeepWarmConfigReply.ReplyJson);

                if (okeepwarmconfig.enabled != null)
                {
                    if (okeepwarmconfig.enabled == 0)
                    {
                        oResult.StayWarmEnabled = false;
                    }
                    if (okeepwarmconfig.enabled == 1)
                    {
                        oResult.StayWarmEnabled = true;
                    }
                }

                if (okeepwarmconfig.temperature != null)
                {
                    oResult.StayWarmTemperature = okeepwarmconfig.temperature;
                }

                if (okeepwarmconfig.hysteresisOver != null)
                {
                    oResult.StayWarmHysteresis = okeepwarmconfig.hysteresisOver;
                }

                // -- End Command -----------------------------------------------------------------









                // -- Send Command -----------------------------------------------------------------
                oCommandSend = new tCommand();

                oCommandSend.CommandName = "entitlementconfig";
                oCommandSend.UrlPath = "entitlementconfig/{devid}";
                oCommandSend.CommandJson = "";
                oCommandSend.ToEco = false;

                tCommand oEntitlementConfigReply = new tCommand();

                oEntitlementConfigReply = PostApi(oCommandSend, strIMEI);
                if (oEntitlementConfigReply.PostStatus != "ok")
                {
                    oResult.HasError = true;
                    oResult.Info = "entitlementconfig " + oEntitlementConfigReply.PostStatus;
                    return oResult;
                };


                oentitlementconfig = JsonSerializer.Deserialize<entitlementconfig>(oEntitlementConfigReply.ReplyJson);

                if (oentitlementconfig.chEnabled != null)
                {
                    if (oentitlementconfig.chEnabled == 0)
                    {
                        oResult.EntitlementSpaceHeating = "Forced Off";
                    }
                    if (oentitlementconfig.chEnabled == 1)
                    {
                        oResult.EntitlementSpaceHeating = "Enabled";
                    }
                    if (oentitlementconfig.chEnabled == 2)
                    {
                        oResult.EntitlementSpaceHeating = "Forced On";
                    }
                }



                if (oentitlementconfig.hwEnabled != null)
                {
                    if (oentitlementconfig.hwEnabled == 0)
                    {
                        oResult.EntitlementHotWater = "Forced Off";
                    }
                    if (oentitlementconfig.hwEnabled == 1)
                    {
                        oResult.EntitlementHotWater = "Enabled";
                    }
                }


                // -- End Command -----------------------------------------------------------------



            }




            catch (Exception ex)
            {
                context.Logger.LogLine("Ex in FunctionHandler " + ex.Message);
                oResult.HasError = false;
                oResult.Info = ex.Message;
            }

            return oResult;
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


        public class tLastSeen
        {
            public int timeStamp { get; set; }
        }

        private int GetLastSeen(string strIMEI, ILambdaContext context)
        {

            int intReturn = -1;
            string strUrl;
            Uri address;
            HttpWebRequest request;
            HttpWebResponse response = null;
            StreamReader reader;
            string appId;
            Stream postStream = null;
            string strResponse = "";
            tLastSeen oLastSeen;
            DateTime datLastSeen;


            strUrl = strApiUrl;

            strUrl = strUrl + "lastseen/";
            strUrl = strUrl + strIMEI;

            address = new Uri(strUrl);

            try
            {
                request = (HttpWebRequest)WebRequest.Create(address);

                request.Method = "GET";

                request.ContentType = "application/x-www-form-urlencoded";

                request.Headers.Add("x-api-key", strApiKey);

                appId = "ss-form";

                if (postStream != null)
                    postStream.Close();

                try
                {
                    // Get response  
                    response = (HttpWebResponse)request.GetResponse();

                    // Get the response stream into a reader  
                    reader = new StreamReader(response.GetResponseStream());

                    strResponse = reader.ReadToEnd();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        oLastSeen = JsonSerializer.Deserialize<tLastSeen>(strResponse);


                        datLastSeen = UnixTimeToDateTime(oLastSeen.timeStamp);
                        TimeSpan ts = DateTime.Now - datLastSeen;
                        intReturn = (int)ts.TotalMinutes;
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
                            //  oCommandReply.PostStatus = "ok";

                            if (response.StatusCode.ToString().IndexOf("504") != -1)
                                //  oCommandReply.PostStatus = "timeout";
                                response.Close();
                    }
                    else
                    {
                        // oCommandReply.PostStatus = "offline";
                    }
                }
            }

            catch (Exception ex)
            {
            }

            return intReturn;
        }

        private DateTime UnixTimeToDateTime(int intUnixDate)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return origin.AddSeconds(intUnixDate);
        }

    }
}

