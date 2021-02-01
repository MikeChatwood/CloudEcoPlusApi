using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoSyncSiteAPI
{


    public class tInput
    {

        public enum tAction
        {
            UPSERT,
            DELETE
        };
        public tAction Action { get; set; }
        public string MessageName { get; set; }
        public int SiteID { get; set; }
        public string SiteKey { get; set; }
        public string SiteName { get; set; }

    }

    public class tResult
    {
        private bool _bolOk = true;
        private string _strInfo = "";
        private int _intReturnVal = 0;

        public bool Ok
        {
            get
            {
                return _bolOk;
            }
            set
            {
                _bolOk = value;
            }
        }

        public string Info
        {
            get
            {
                return _strInfo;
            }
            set
            {
                _strInfo = value;
            }
        }

        public int ReturnVal
        {
            get
            {
                return _intReturnVal;
            }
            set
            {
                _intReturnVal = value;
            }
        }
    }

    public class CloudEcoSyncSiteAPI
    {

        private static readonly AmazonKinesisClient kinesisClient = new AmazonKinesisClient(RegionEndpoint.EUWest2);

        //  public async Task<tResult> FunctionHandler(tInput oInput, ILambdaContext context)
        public async Task<tResult> FunctionHandler(tInput oInput, ILambdaContext context)
        {
            tResult oResult = new tResult();

            context.Logger.LogLine($"Input string:{JsonSerializer.Serialize<tInput>(oInput) }");

            try
            {

                // Validate here
                oResult = await WriteStream(oInput, context);  // Write to Kinesis

                context.Logger.LogLine("State 1");

            }
            catch (Exception ex)
            {
                oResult.Ok = false;
                oResult.Info = ex.Message;
                context.Logger.LogLine("Ex in WriteRecord " + ex.Message);
            }

            return oResult;
        }




        private static async Task<tResult> WriteStream(tInput oInput, ILambdaContext context)
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

                strInput = JsonSerializer.Serialize(oInput);
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
