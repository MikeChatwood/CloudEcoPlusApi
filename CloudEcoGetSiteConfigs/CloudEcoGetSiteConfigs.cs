using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoGetSiteConfigs
{

    public class tInput
    {
        public int SiteID { get; set; } = -1;

    };

    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";

        public List<tConfig> Configs { get; set; } = new List<tConfig>();

        public class tConfig
        {


            public int EhiuSiteConfigID { get; set; } = -1;
            public string Name { get; set; } = "";
            public string Descr { get; set; } = "";
        }
    }

    public class CloudEcoGetSiteConfigs
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

                strQuery = "SELECT EhiuSiteConfigID, Name, Descr " +
                            " FROM EhiuSiteConfig " +
                            " WHERE (Retired = 0) AND (SiteID = @SiteID) " +
                            " ORDER BY Name";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamSiteID = new SqlParameter("@SiteID", SqlDbType.Int);
                sqlParamSiteID.Value = oInput.SiteID;
                daCheck.SelectCommand.Parameters.Add(sqlParamSiteID);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {

                    tResult.tConfig oConfig = new tResult.tConfig();

                    oConfig.EhiuSiteConfigID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuSiteConfigID"];
                    oConfig.Name = (string)dsCheck.Tables[0].Rows[intIdx]["Name"];
                    oConfig.Descr = (string)dsCheck.Tables[0].Rows[intIdx]["Descr"];

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
