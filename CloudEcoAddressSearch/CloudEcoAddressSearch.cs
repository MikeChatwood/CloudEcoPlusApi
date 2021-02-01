using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoAddressSearch
{


    public class tInput
    {
        public string? SearchString { get; set; }
        public int? MaxRecords { get; set; }
    }

    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";

        public List<tAddress> AddressList { get; set; } = new List<tAddress>();

        public class tAddress
        {

            public int? PropertyID { get; set; }
            public string? Address { get; set; }
            public string? BlockPostCode { get; set; }
            public string? PropertyReference { get; set; }
            public string? SiteName { get; set; }
            public int? SiteID { get; set; }

        };
    }





    public class CloudEcoAddressSearch
    {

        public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {

            string strQuery = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();

            SqlConnection oSqlConnection = null;
            tResult oResult = new tResult();
            int intIdx;

            try
            {

                context.Logger.LogLine("FunctionHandler 1 ");

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



                strQuery = "SELECT {TOPN}  Property.PropertyID, LTRIM(RTRIM(Property.PropertyNumber + ' ' + Property.BlockName)) AS Address, Property.PropertyReference, " +
                            " Property.BlockPostCode, Site.SiteName, Property.SiteID " +
                            " FROM Property INNER JOIN " +
                            "       Site ON Property.SiteID = Site.SiteID " +
                            " WHERE (LTRIM(RTRIM(Property.PropertyNumber + ' ' + Property.BlockName)) LIKE @Search) AND (Property.ArchivedProperty = 0) AND (ISNULL(Property.Deleted,0) = 0) OR " +
                            "       (Property.ArchivedProperty = 0) AND (ISNULL(Property.Deleted,0) = 0) AND (Property.PropertyReference LIKE @Search) " +
                            " ORDER BY Address ";

                if (oInput.MaxRecords == null)
                {
                    strQuery = strQuery.Replace("{TOPN}", "");
                }
                else
                {
                    strQuery = strQuery.Replace("{TOPN}", " TOP " + oInput.MaxRecords.ToString());
                }




                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamSearch = new SqlParameter("@Search", SqlDbType.NVarChar);
                sqlParamSearch.Value = oInput.SearchString;
                daCheck.SelectCommand.Parameters.Add(sqlParamSearch);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {
                    tResult.tAddress oAddress = new tResult.tAddress();

                    if (dsCheck.Tables[0].Rows[intIdx]["Address"] != DBNull.Value)
                    {
                        oAddress.Address = (string)dsCheck.Tables[0].Rows[intIdx]["Address"];
                    }

                    if (dsCheck.Tables[0].Rows[intIdx]["PropertyID"] != DBNull.Value)
                    {
                        oAddress.PropertyID = (int)dsCheck.Tables[0].Rows[intIdx]["PropertyID"];
                    }

                    if (dsCheck.Tables[0].Rows[intIdx]["PropertyReference"] != DBNull.Value)
                    {
                        oAddress.PropertyReference = (string)dsCheck.Tables[0].Rows[intIdx]["PropertyReference"];
                    }

                    if (dsCheck.Tables[0].Rows[intIdx]["BlockPostCode"] != DBNull.Value)
                    {
                        oAddress.BlockPostCode = (string)dsCheck.Tables[0].Rows[intIdx]["BlockPostCode"];
                    }

                    if (dsCheck.Tables[0].Rows[intIdx]["SiteName"] != DBNull.Value)
                    {
                        oAddress.SiteName = (string)dsCheck.Tables[0].Rows[intIdx]["SiteName"];
                    }

                    if (dsCheck.Tables[0].Rows[intIdx]["SiteID"] != DBNull.Value)
                    {
                        oAddress.SiteID = (int)dsCheck.Tables[0].Rows[intIdx]["SiteID"];
                    }




                    oResult.AddressList.Add(oAddress);
                }


            }

            catch (Exception ex)
            {
                context.Logger.LogLine("Ex in FunctionHandler " + ex.Message);
                oResult.Ok = false;
                oResult.Info = ex.Message;
            }

            return oResult;
        }
    }
}
