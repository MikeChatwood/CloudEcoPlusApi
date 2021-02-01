using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoSerialHistory
{

    public class tInput
    {
        public string SerialNumber { get; set; } = "";

    };

    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";

        public List<tConfig> Configs { get; set; } = new List<tConfig>();

        public class tConfig
        {


            public DateTime AppliedDate { get; set; } = DateTime.Now ;
            public string Name { get; set; } = "";
            public string PropertyNumber { get; set; } = "";
            public string BlockName { get; set; } = "";

        }
    }

    public class CloudEcoSerialHistory
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

                strQuery = "SELECT Property.PropertyNumber, Property.BlockName, EhiuInstall.FromDate, EhiuInstall.ToDate, EhiuSiteConfig.Name, EhiuInstallDetail.DateCreated " +
                            " FROM EhiuInstall INNER JOIN " +
                            " EhiuInstallDetail ON EhiuInstall.EhiuInstallID = EhiuInstallDetail.EhiuInstallID INNER JOIN " +
                            " Property ON EhiuInstall.PropertyID = Property.PropertyID INNER JOIN " +
                            " EhiuSiteConfig ON EhiuInstallDetail.EhiuSiteConfigID = EhiuSiteConfig.EhiuSiteConfigID " +
                            " WHERE (EhiuInstall.SerialNumber = @SerialNumber) " +
                            " ORDER BY EhiuInstallDetail.DateCreated Desc";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamSerialNumber = new SqlParameter("@SerialNumber", SqlDbType.NVarChar);
                sqlParamSerialNumber.Value = oInput.SerialNumber ;
                daCheck.SelectCommand.Parameters.Add(sqlParamSerialNumber);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {

                    tResult.tConfig oConfig = new tResult.tConfig();

                    oConfig.AppliedDate = (DateTime)dsCheck.Tables[0].Rows[intIdx]["DateCreated"];
                    oConfig.Name = (string)dsCheck.Tables[0].Rows[intIdx]["Name"];
                    oConfig.PropertyNumber = (string)dsCheck.Tables[0].Rows[intIdx]["PropertyNumber"];
                    oConfig.BlockName = (string)dsCheck.Tables[0].Rows[intIdx]["BlockName"];

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
