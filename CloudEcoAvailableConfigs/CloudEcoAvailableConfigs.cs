using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoAvailableConfigs
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
            public string CreatedBy { get; set; } = "";
            public DateTime? DateCreated { get; set; } = null;
            public bool Retired { get; set; } = false;
            public string Descr { get; set; } = "";
            public string Name { get; set; } = "";
            public int SiteID { get; set; } = -1;
            public int EhiuSiteConfigID { get; set; } = -1;
        }
    }
    public class CloudEcoAvailableConfigs
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

                strQuery = "SELECT EhiuSiteConfig.EhiuSiteConfigID, EhiuSiteConfig.SiteID, EhiuSiteConfig.Name, EhiuSiteConfig.Descr, " +
                            " EhiuSiteConfig.Retired, EhiuSiteConfig.DateCreated, EhiuSiteConfig.CreatedBy, Site.SiteName " +
                            " FROM EhiuSiteConfig INNER JOIN " +
                            " Site ON EhiuSiteConfig.SiteID = Site.SiteID " +
                            " WHERE (Site.SiteName LIKE @SiteName) " +
                            " ORDER BY EhiuSiteConfig.Name ";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamSiteName = new SqlParameter("@SiteName", SqlDbType.NVarChar);
                sqlParamSiteName.Value = oInput.Search;
                daCheck.SelectCommand.Parameters.Add(sqlParamSiteName);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {

                    tResult.tConfig oConfig = new tResult.tConfig();

                    oConfig.SiteID = (int)dsCheck.Tables[0].Rows[intIdx]["SiteID"];
                    oConfig.Descr = (string)dsCheck.Tables[0].Rows[intIdx]["Descr"];
                    oConfig.Name = (string)dsCheck.Tables[0].Rows[intIdx]["Name"];
                    oConfig.Retired = (bool)dsCheck.Tables[0].Rows[intIdx]["Retired"];
                    oConfig.DateCreated = (DateTime)dsCheck.Tables[0].Rows[intIdx]["DateCreated"];
                    oConfig.CreatedBy = (string)dsCheck.Tables[0].Rows[intIdx]["CreatedBy"];
                    oConfig.SiteName = (string)dsCheck.Tables[0].Rows[intIdx]["SiteName"];
                    oConfig.EhiuSiteConfigID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuSiteConfigID"];
                    
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
