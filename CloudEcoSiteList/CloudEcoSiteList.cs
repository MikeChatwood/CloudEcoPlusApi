using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoSiteList
{
    public class tInput
    {
        public string Search { get; set; } = "%";

    };
    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";

        public List<tConfig> Configs { get; set; } = new List<tConfig>();

        public class tConfig
        {

            public string SiteName { get; set; } = "";
            public int SiteID { get; set; } = -1;

        }
    }


    public class CloudEcoSiteList
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

                context.Logger.LogLine("FunctionHandler 1 " );

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

                strQuery = "SELECT SiteName, SiteID " +
                            " FROM Site " +
                            " WHERE (SiteName LIKE dbo.PrepQry(@SiteName)) " +
                            " ORDER BY SiteName";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamSiteName = new SqlParameter("@SiteName", SqlDbType.NVarChar);
                sqlParamSiteName.Value = oInput.Search;
                daCheck.SelectCommand.Parameters.Add(sqlParamSiteName);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {

                    tResult.tConfig oConfig = new tResult.tConfig();

                    oConfig.SiteID = (int)dsCheck.Tables[0].Rows[intIdx]["SiteID"];
                    oConfig.SiteName = (string)dsCheck.Tables[0].Rows[intIdx]["SiteName"];

                    oResult.Configs.Add(oConfig);
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
