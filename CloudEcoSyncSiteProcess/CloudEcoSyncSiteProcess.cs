using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoSyncSiteProcess
{
    public class CloudEcoSyncSiteProcess
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


        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void FunctionHandler(KinesisEvent kinesisEvent, ILambdaContext context)
        {

            // Insert / Delete Site table

            SqlConnection oSqlConnection = null;
            int intSiteID;
            tInput oInput;

            context.Logger.LogLine("FunctionHandler 1 1725");

            try
            {
                 oSqlConnection = new SqlConnection(ecoCommon.GetSecret("CloudEcoPlus", context)); oSqlConnection.Open();
                context.Logger.LogLine("FunctionHandler 2");
            }
            catch (Exception ex)
            {
                context.Logger.LogLine("WriteRecord Ex  1" + ex.Message);
            }

            context.Logger.LogLine("FunctionHandler 3");

            context.Logger.LogLine($"Beginning to process {kinesisEvent.Records.Count} records...");

            foreach (var record in kinesisEvent.Records)
            {

                string recordData = GetRecordContents(record.Kinesis);

                context.Logger.LogLine("FunctionHandler rd  >" + recordData);

                oInput = JsonSerializer.Deserialize<tInput>(recordData);

                if (oInput.MessageName.ToLower() != "site")
                {
                    continue;
                }

                if (oInput.Action == tInput.tAction.UPSERT)
                {
                    intSiteID = oInput.SiteID;

                    if (SiteRecordExists(intSiteID, context, ref oSqlConnection) == true)
                    {
                        UpdateSite(oInput, context, ref oSqlConnection);
                    }
                    else
                    {
                        InsertSite(oInput, context, ref oSqlConnection);
                    }
                }

                if (oInput.Action == tInput.tAction.DELETE)
                {
                    DeleteSite(oInput, context, ref oSqlConnection);
                };

                context.Logger.LogLine("Stream processing complete.");
            }
        }

        private string GetRecordContents(KinesisEvent.Record streamRecord)
        {
            using (var reader = new StreamReader(streamRecord.Data, Encoding.ASCII))
            {
                return reader.ReadToEnd();
            }
        }


        private bool InsertSite(tInput oInput, ILambdaContext context, ref SqlConnection oSqlConnection)
        {
            SqlCommand sqlComm;
            string strQuery;

            try
            {
                strQuery = " INSERT INTO Site (SiteID, SiteKey, SiteName ) Values (@SiteID, @SiteKey, @SiteName ) ";

                sqlComm = oSqlConnection.CreateCommand();

                sqlComm.CommandText = strQuery;

                SqlParameter sqlParamSiteID = new SqlParameter("@SiteID", SqlDbType.Int);
                sqlParamSiteID.Value = oInput.SiteID;
                sqlComm.Parameters.Add(sqlParamSiteID);

                SqlParameter sqlParamSiteKey = new SqlParameter("@SiteKey", SqlDbType.NVarChar);
                sqlParamSiteKey.Value = oInput.SiteKey;
                sqlComm.Parameters.Add(sqlParamSiteKey);

                SqlParameter sqlParamSiteName = new SqlParameter("@SiteName", SqlDbType.NVarChar);
                sqlParamSiteName.Value = oInput.SiteName;
                sqlComm.Parameters.Add(sqlParamSiteName);
                
                sqlComm.ExecuteNonQuery();
                sqlComm.Dispose();
            }


            catch (Exception ex)
            {

                context.Logger.LogLine("InsertSite Ex " + ex.Message);
                return false;

            }


            return true;

        }

        private bool UpdateSite(tInput oInput, ILambdaContext context, ref SqlConnection oSqlConnection)
        {
            SqlCommand sqlComm;
            string strQuery;

            try
            {
                strQuery = " Update Site Set " +
                           " SiteKey = @SiteKey, " +
                           " SiteName = @SiteName, " +
                           " Deleted = 0 " +
                           " Where " +
                           " SiteID = @SiteID ";

                sqlComm = oSqlConnection.CreateCommand();

                sqlComm.CommandText = strQuery;

                SqlParameter sqlParamSiteID = new SqlParameter("@SiteID", SqlDbType.Int);
                sqlParamSiteID.Value = oInput.SiteID;
                sqlComm.Parameters.Add(sqlParamSiteID);

                SqlParameter sqlParamSiteKey = new SqlParameter("@SiteKey", SqlDbType.NVarChar);
                sqlParamSiteKey.Value = oInput.SiteKey;
                sqlComm.Parameters.Add(sqlParamSiteKey);

                SqlParameter sqlParamSiteName = new SqlParameter("@SiteName", SqlDbType.NVarChar);
                sqlParamSiteName.Value = oInput.SiteName;
                sqlComm.Parameters.Add(sqlParamSiteName);

                sqlComm.ExecuteNonQuery();
                sqlComm.Dispose();
            }

            catch (Exception ex)
            {

                context.Logger.LogLine("UpdateSite Ex " + ex.Message);
                return false;

            }


            return true;

        }

        private bool DeleteSite(tInput oInput, ILambdaContext context, ref SqlConnection oSqlConnection)
        {
            SqlCommand sqlComm;
            string strQuery;

            try
            {
                strQuery = " Update Site Set " +
                           " Deleted = 1 " +
                           " Where " +
                           " SiteID = @SiteID ";

                sqlComm = oSqlConnection.CreateCommand();

                sqlComm.CommandText = strQuery;

                SqlParameter sqlParamSiteID = new SqlParameter("@SiteID", SqlDbType.Int);
                sqlParamSiteID.Value = oInput.SiteID;
                sqlComm.Parameters.Add(sqlParamSiteID);

                sqlComm.ExecuteNonQuery();
                sqlComm.Dispose();
            }

            catch (Exception ex)
            {

                context.Logger.LogLine("UpdateSite Ex " + ex.Message);
                return false;

            }


            return true;

        }


        private bool SiteRecordExists(int intSiteID, ILambdaContext context, ref SqlConnection oSqlConnection)
        {

            string strQuery;
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();
            bool bolReturn = false;
            int intIdx;

            try
            {

                try
                {
                    oSqlConnection.Open();
                }
                catch (Exception)
                {
                }

                strQuery = "SELECT TOP (1) SiteID " + " FROM Site " + " WHERE (SiteID = @SiteID)";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamSiteID = new SqlParameter("@SiteID", SqlDbType.Int);
                sqlParamSiteID.Value = intSiteID;
                daCheck.SelectCommand.Parameters.Add(sqlParamSiteID);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {
                    bolReturn = true;
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine("SiteRecordExists Ex " + intSiteID.ToString() + " " + ex.Message);
            }

            return bolReturn;

        }





    }
}
