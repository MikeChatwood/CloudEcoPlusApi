using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoConfigDetail
{

    public class tInput
    {
        public int EhiuSiteConfigID { get; set; } = -1;

    };
    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";

        public List<tConfig> Configs { get; set; } = new List<tConfig>();

        public class tConfig
        {

            public int EhiuSiteConfigDetailID { get; set; } = -1;
            public string JsonConfigItem { get; set; } = "";
            public DateTime DateCreated { get; set; } = DateTime.Now;
            public string CreatedBy { get; set; } = "";
            public string CommandName { get; set; } = "";
            public bool ToEco { get; set; } = false;
            public string JsonCompanionItem { get; set; } = "";
            public string CompanionName { get; set; } = "";

        }
    }



    public class CloudEcoConfigDetail
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

                strQuery = "SELECT EhiuSiteConfigDetailID, JsonConfigItem, DateCreated, CreatedBy, CommandName, ToEco, ISNULL(JsonCompanionItem,'') As JsonCompanionItem, ISNULL(CompanionName,'') As CompanionName " +
                            " FROM EhiuSiteConfigDetail " +
                            " WHERE (EhiuSiteConfigID = @EhiuSiteConfigID)";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamEhiuSiteConfigID = new SqlParameter("@EhiuSiteConfigID", SqlDbType.Int);
                sqlParamEhiuSiteConfigID.Value = oInput.EhiuSiteConfigID;
                daCheck.SelectCommand.Parameters.Add(sqlParamEhiuSiteConfigID);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {

                    tResult.tConfig oConfig = new tResult.tConfig();

                    oConfig.EhiuSiteConfigDetailID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuSiteConfigDetailID"];

                    oConfig.JsonConfigItem = (string)dsCheck.Tables[0].Rows[intIdx]["JsonConfigItem"];
                    oConfig.CommandName = (string)dsCheck.Tables[0].Rows[intIdx]["CommandName"];

                    oConfig.DateCreated = (DateTime)dsCheck.Tables[0].Rows[intIdx]["DateCreated"];
                    oConfig.CreatedBy = (string)dsCheck.Tables[0].Rows[intIdx]["CreatedBy"];
                    oConfig.ToEco = (bool)dsCheck.Tables[0].Rows[intIdx]["ToEco"];

                    oConfig.JsonCompanionItem = (string)dsCheck.Tables[0].Rows[intIdx]["JsonCompanionItem"];
                    oConfig.CompanionName = (string)dsCheck.Tables[0].Rows[intIdx]["CompanionName"];

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
