using System;
using System.Collections.Generic;
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

namespace CloudEcoGetUnitStatus
{

    public class tInput
    {
        public string SerialNumber { get; set; }
    }

    public class tResult
    {

        public bool Ok { get; set; } = true;
        public int LastSeenMinutesAgo { get; set; }
    }


    public class tLastSeen
    {
        public int timeStamp { get; set; }
    }

    public class CloudEcoGetUnitStatus
    {

        const string strApiUrl = "https://ndygcmuadj.execute-api.eu-west-2.amazonaws.com/1/";
        const string strApiKey = "RhTHczPuO97m6D7E1uTg6ajnWKbN3zlhaaf6x2cv";

        public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {
            tResult oResult = new tResult();
            tLastSeen oLastSeen;

            HttpWebRequest request;
            HttpWebResponse response = null;
            StreamReader reader;
            Uri address;
            string appId;
            string strResponse = "";

            Stream postStream = null;
            string strUrl;
            byte[] byteData;
            SqlConnection oSqlConnection = null;
            string strIMEI;
            DateTime datLastSeen;


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

                        oResult.Ok = true;

                        datLastSeen = UnixTimeToDateTime(oLastSeen.timeStamp);
                        TimeSpan ts = DateTime.Now - datLastSeen;
                        oResult.LastSeenMinutesAgo = (int)ts.TotalMinutes;
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





            return oResult;


        }

        private DateTime UnixTimeToDateTime(int intUnixDate)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return origin.AddSeconds(intUnixDate);
        }


    }
}
