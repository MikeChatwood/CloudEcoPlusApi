using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoGetBuildConfigs
{


    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";

        public List<tBuildConfig> BuildConfigs { get; set; } = new List<tBuildConfig>();


        public class tBuildConfig
        {

            public int? EhiuBuildConfigID { get; set; }
            public bool? DefaultConfig { get; set; }
            public string? ConfigName { get; set; }
            public DateTime? DateCreated { get; set; } = DateTime.Now;
            public string? CreatedBy { get; set; }

        };
    }




    public class CloudEcoGetBuildConfigs
    {

        public tResult FunctionHandler(ILambdaContext context)
        {

            string strQuery = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();

            SqlConnection oSqlConnection = null;
            tResult oResult = new tResult();
            int intIdx;

            try
            {

                context.Logger.LogLine("FunctionHandler 1706 ");

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

                strQuery = "SELECT EhiuBuildConfigID, DefaultConfig, ConfigName, DateCreated, CreatedBy " +
                                  " FROM  EhiuBuildConfig " +
                                  " ORDER BY DateCreated DESC";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);
                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {

                    tResult.tBuildConfig oConfig = new tResult.tBuildConfig();


                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigID"] != DBNull.Value)
                    {
                        oConfig.EhiuBuildConfigID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigID"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["DefaultConfig"] != DBNull.Value)
                    {
                        oConfig.DefaultConfig = (bool)dsCheck.Tables[0].Rows[intIdx]["DefaultConfig"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["ConfigName"] != DBNull.Value)
                    {
                        oConfig.ConfigName = (string)dsCheck.Tables[0].Rows[intIdx]["ConfigName"];
                    }




                    oConfig.DateCreated = (DateTime)dsCheck.Tables[0].Rows[intIdx]["DateCreated"];
                    oConfig.CreatedBy = (string)dsCheck.Tables[0].Rows[intIdx]["CreatedBy"];

                    oResult.BuildConfigs.Add(oConfig);
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
