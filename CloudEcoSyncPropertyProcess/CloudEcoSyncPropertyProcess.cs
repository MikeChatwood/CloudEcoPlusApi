using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoSyncPropertyProcess
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

        public int PropertyID { get; set; }
        public int SiteID { get; set; }
        public string PropertyNumber { get; set; } = "";
        public string BlockName { get; set; } = "";
        public string BlockKey { get; set; } = "";
        public string PropertyReference { get; set; } = "";
        public string BlockAddress { get; set; } = "";
        public string BlockPostCode { get; set; } = "";
        public bool ArchivedProperty { get; set; } = false;
        public string ArchivedReason { get; set; } = "";
        public string ArchivedWho { get; set; } = "";
        public string ArchivedWhen { get; set; } = "";


    }

    public class CloudEcoSyncPropertyProcess
    {

        public void FunctionHandler(KinesisEvent kinesisEvent, ILambdaContext context)
        {

            // Insert / Delete Site table

            SqlConnection oSqlConnection = null;
            int intPropertyID;
            int intSiteID;
            tInput oInput;

            context.Logger.LogLine("FunctionHandler 1 1446");

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
                
                if (oInput.MessageName.ToLower() != "property")
                {
                    continue;
                }

                if (oInput.Action == tInput.tAction.UPSERT)
                {
                    intPropertyID = oInput.PropertyID;
                    intSiteID = oInput.SiteID;

                    if (SiteIdExists(intSiteID, context, ref oSqlConnection) == false)
                    {
                        InsertSiteID(intSiteID, context, ref oSqlConnection);
                    };

                    if (PropertyRecordExists(intPropertyID , context, ref oSqlConnection) == true)
                    {
                        UpdateProperty(oInput, context, ref oSqlConnection);
                    }
                    else
                    {
                        InsertProperty(oInput, context, ref oSqlConnection);
                    }
                }

                if (oInput.Action == tInput.tAction.DELETE)
                {
                    DeleteProperty(oInput, context, ref oSqlConnection);
                };
                
                context.Logger.LogLine("Stream processing complete.");
            }

            try
            {
                oSqlConnection.Close();
            }
            catch
            {

            }
          
        }
        private string GetRecordContents(KinesisEvent.Record streamRecord)
        {
            using (var reader = new StreamReader(streamRecord.Data, Encoding.ASCII))
            {
                return reader.ReadToEnd();
            }
        }


        private bool InsertSiteID(int intSiteID, ILambdaContext context, ref SqlConnection oSqlConnection)
        {
            // Write a placeholder record, this maintains referential integrity and later syncs will provide the details
            SqlCommand sqlComm;
            string strQuery;


            try
            {
                strQuery = " INSERT INTO Site (SiteID, SiteKey, SiteName) Values (@SiteID, 'HOLD', 'Place Holder') ";

                sqlComm = oSqlConnection.CreateCommand();

                sqlComm.CommandText = strQuery;

                SqlParameter sqlParamSiteID = new SqlParameter("@SiteID", SqlDbType.Int);
                sqlParamSiteID.Value = intSiteID;
                sqlComm.Parameters.Add(sqlParamSiteID);

                sqlComm.ExecuteNonQuery();
                sqlComm.Dispose();
            }


            catch (Exception ex)
            {
                return false;

            }


            return true;

        }

        private bool SiteIdExists(int intSiteID, ILambdaContext context, ref SqlConnection oSqlConnection)
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
                context.Logger.LogLine("SiteIDExists Ex " + intSiteID.ToString() + " " + ex.Message);
            }

            return bolReturn;

        }

        private bool InsertProperty(tInput oInput, ILambdaContext context, ref SqlConnection oSqlConnection)
        {
            SqlCommand sqlComm;
            string strQuery;

            try
            {
                strQuery = " INSERT INTO Property " +
                           " (  PropertyID, SiteID, PropertyNumber, BlockName, BlockKey, PropertyReference, BlockAddress, BlockPostCode, ArchivedProperty, ArchivedReason, ArchivedWho, ArchivedWhen ) VALUES " +
                           " (  @PropertyID, @SiteID, @PropertyNumber, @BlockName, @BlockKey, @PropertyReference, @BlockAddress, @BlockPostCode, @ArchivedProperty, @ArchivedReason, @ArchivedWho, @ArchivedWhen ) ";

                sqlComm = oSqlConnection.CreateCommand();

                sqlComm.CommandText = strQuery;

                SqlParameter sqlParamPropertyID = new SqlParameter("@PropertyID", SqlDbType.Int);
                sqlParamPropertyID.Value = oInput.PropertyID;
                sqlComm.Parameters.Add(sqlParamPropertyID);

                SqlParameter sqlParamSiteID = new SqlParameter("@SiteID", SqlDbType.Int);
                sqlParamSiteID.Value = oInput.SiteID;
                sqlComm.Parameters.Add(sqlParamSiteID);

                SqlParameter sqlParamPropertyNumber = new SqlParameter("@PropertyNumber", SqlDbType.NVarChar);
                sqlParamPropertyNumber.Value = oInput.PropertyNumber;
                sqlComm.Parameters.Add(sqlParamPropertyNumber);

                SqlParameter sqlParamBlockName = new SqlParameter("@BlockName", SqlDbType.NVarChar);
                sqlParamBlockName.Value = oInput.BlockName;
                sqlComm.Parameters.Add(sqlParamBlockName);

                SqlParameter sqlParamBlockKey = new SqlParameter("@BlockKey", SqlDbType.NVarChar);
                sqlParamBlockKey.Value = oInput.BlockKey;
                sqlComm.Parameters.Add(sqlParamBlockKey);

                SqlParameter sqlParamPropertyReference = new SqlParameter("@PropertyReference", SqlDbType.NVarChar);
                sqlParamPropertyReference.Value = oInput.PropertyReference;
                sqlComm.Parameters.Add(sqlParamPropertyReference);


                SqlParameter sqlParamBlockAddress = new SqlParameter("@BlockAddress", SqlDbType.NVarChar);
                sqlParamBlockAddress.Value = oInput.BlockAddress;
                sqlComm.Parameters.Add(sqlParamBlockAddress);

                SqlParameter sqlParamBlockPostCode = new SqlParameter("@BlockPostCode", SqlDbType.NVarChar);
                sqlParamBlockPostCode.Value = oInput.BlockPostCode;
                sqlComm.Parameters.Add(sqlParamBlockPostCode);

                SqlParameter sqlParamArchivedProperty = new SqlParameter("@ArchivedProperty", SqlDbType.Bit);
                sqlParamArchivedProperty.Value = oInput.ArchivedProperty;
                sqlComm.Parameters.Add(sqlParamArchivedProperty);

                SqlParameter sqlParamArchivedReason = new SqlParameter("@ArchivedReason", SqlDbType.NVarChar);
                sqlParamArchivedReason.Value = oInput.ArchivedReason;
                sqlComm.Parameters.Add(sqlParamArchivedReason);

                SqlParameter sqlParamArchivedWho = new SqlParameter("@ArchivedWho", SqlDbType.NVarChar);
                sqlParamArchivedWho.Value = oInput.ArchivedWho;
                sqlComm.Parameters.Add(sqlParamArchivedWho);


                SqlParameter sqlParamArchivedWhen = new SqlParameter("@ArchivedWhen", SqlDbType.DateTime);
                if (oInput.ArchivedWhen == "")
                {
                    sqlParamArchivedWhen.Value = DBNull.Value;
                }
                else
                {
                    sqlParamArchivedWhen.Value = ecoCommon.DateJson(oInput.ArchivedWhen);
                }
                sqlComm.Parameters.Add(sqlParamArchivedWhen);



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

        private bool UpdateProperty(tInput oInput, ILambdaContext context, ref SqlConnection oSqlConnection)
        {
            SqlCommand sqlComm;
            string strQuery;

            try
            {
                strQuery = " Update Property Set " +
                           " SiteID = @SiteID, " +
                           " PropertyNumber = @PropertyNumber, " +
                           " BlockName = @BlockName, " +
                           " BlockKey = @BlockKey, " +
                           " PropertyReference = @PropertyReference, " +
                           " BlockAddress = @BlockAddress, " +
                           " BlockPostCode = @BlockPostCode, " +
                           " ArchivedProperty = @ArchivedProperty, " +
                           " ArchivedReason = @ArchivedReason, " +
                           " ArchivedWho = @ArchivedWho, " +
                           " ArchivedWhen = @ArchivedWhen, " +
                           " Deleted = 0 " +
                           " Where " +
                           " PropertyID = @PropertyID ";

                sqlComm = oSqlConnection.CreateCommand();

                sqlComm.CommandText = strQuery;


                SqlParameter sqlParamPropertyID = new SqlParameter("@PropertyID", SqlDbType.Int);
                sqlParamPropertyID.Value = oInput.PropertyID;
                sqlComm.Parameters.Add(sqlParamPropertyID);

                SqlParameter sqlParamSiteID = new SqlParameter("@SiteID", SqlDbType.Int);
                sqlParamSiteID.Value = oInput.SiteID;
                sqlComm.Parameters.Add(sqlParamSiteID);

                SqlParameter sqlParamPropertyNumber = new SqlParameter("@PropertyNumber", SqlDbType.NVarChar);
                sqlParamPropertyNumber.Value = oInput.PropertyNumber;
                sqlComm.Parameters.Add(sqlParamPropertyNumber);

                SqlParameter sqlParamBlockName = new SqlParameter("@BlockName", SqlDbType.NVarChar);
                sqlParamBlockName.Value = oInput.BlockName;
                sqlComm.Parameters.Add(sqlParamBlockName);

                SqlParameter sqlParamBlockKey = new SqlParameter("@BlockKey", SqlDbType.NVarChar);
                sqlParamBlockKey.Value = oInput.BlockKey;
                sqlComm.Parameters.Add(sqlParamBlockKey);

                SqlParameter sqlParamPropertyReference = new SqlParameter("@PropertyReference", SqlDbType.NVarChar);
                sqlParamPropertyReference.Value = oInput.PropertyReference;
                sqlComm.Parameters.Add(sqlParamPropertyReference);


                SqlParameter sqlParamBlockAddress = new SqlParameter("@BlockAddress", SqlDbType.NVarChar);
                sqlParamBlockAddress.Value = oInput.BlockAddress;
                sqlComm.Parameters.Add(sqlParamBlockAddress);

                SqlParameter sqlParamBlockPostCode = new SqlParameter("@BlockPostCode", SqlDbType.NVarChar);
                sqlParamBlockPostCode.Value = oInput.BlockPostCode;
                sqlComm.Parameters.Add(sqlParamBlockPostCode);

                SqlParameter sqlParamArchivedProperty = new SqlParameter("@ArchivedProperty", SqlDbType.Bit);
                sqlParamArchivedProperty.Value = oInput.ArchivedProperty;
                sqlComm.Parameters.Add(sqlParamArchivedProperty);

                SqlParameter sqlParamArchivedReason = new SqlParameter("@ArchivedReason", SqlDbType.NVarChar);
                sqlParamArchivedReason.Value = oInput.ArchivedReason;
                sqlComm.Parameters.Add(sqlParamArchivedReason);

                SqlParameter sqlParamArchivedWho = new SqlParameter("@ArchivedWho", SqlDbType.NVarChar);
                sqlParamArchivedWho.Value = oInput.ArchivedWho;
                sqlComm.Parameters.Add(sqlParamArchivedWho);


                SqlParameter sqlParamArchivedWhen = new SqlParameter("@ArchivedWhen", SqlDbType.DateTime);
                if (oInput.ArchivedWhen == "")
                {
                    sqlParamArchivedWhen.Value = DBNull.Value;
                }
                else
                {
                    sqlParamArchivedWhen.Value = ecoCommon.DateJson(oInput.ArchivedWhen);
                }
                sqlComm.Parameters.Add(sqlParamArchivedWhen);


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

        private bool DeleteProperty(tInput oInput, ILambdaContext context, ref SqlConnection oSqlConnection)
        {
            SqlCommand sqlComm;
            string strQuery;

            try
            {
                strQuery = " Update Property Set " +
                           " Deleted = 1 " +
                           " Where " +
                           " PropertyID = @PropertyID ";

                sqlComm = oSqlConnection.CreateCommand();

                sqlComm.CommandText = strQuery;

                SqlParameter sqlParamPropertyID = new SqlParameter("@PropertyID", SqlDbType.Int);
                sqlParamPropertyID.Value = oInput.PropertyID;
                sqlComm.Parameters.Add(sqlParamPropertyID);

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


        private bool PropertyRecordExists(int intPropertyID, ILambdaContext context, ref SqlConnection oSqlConnection)
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

                strQuery = "SELECT TOP (1) PropertyID " + " FROM Property " + " WHERE (PropertyID = @PropertyID)";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamPropertyID = new SqlParameter("@PropertyID", SqlDbType.Int);
                sqlParamPropertyID.Value = intPropertyID;
                daCheck.SelectCommand.Parameters.Add(sqlParamPropertyID);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {
                    bolReturn = true;
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine("PropertyRecordExists Ex " + intPropertyID.ToString() + " " + ex.Message);
            }

            return bolReturn;

        }
    }
}
