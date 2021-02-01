using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoInstallStatus
{


    public class tInput
    {
        public string SerialNumber { get; set; } = null;

    };


    public class tResult
    {
        public int PropertyId { get; set; } = -1;
        public bool HasError { get; set; } = false;
        public string Info { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Address { get; set; } = "";

    }

    public class CloudEcoInstallStatus
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

                strQuery = "SELECT EhiuInstall.SerialNumber, EhiuInstall.ToDate, EhiuInstall.PropertyID, LTRIM(RTRIM(ISNULL(Property.PropertyNumber, '') + ' ' + ISNULL(Property.BlockName, ''))) AS Addr " +
                            " FROM EhiuInstall INNER JOIN " +
                            " Property ON EhiuInstall.PropertyID = Property.PropertyID " +
                            " WHERE (EhiuInstall.SerialNumber = @SerialNumber) AND (EhiuInstall.ToDate IS NULL)";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamSerialNumber = new SqlParameter("@SerialNumber", SqlDbType.NVarChar);
                sqlParamSerialNumber.Value = oInput.SerialNumber;
                daCheck.SelectCommand.Parameters.Add(sqlParamSerialNumber);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {
                    oResult.Address = (string)dsCheck.Tables[0].Rows[intIdx]["Addr"];
                    oResult.PropertyId = (int)dsCheck.Tables[0].Rows[intIdx]["PropertyID"];
                    oResult.SerialNumber = (string)dsCheck.Tables[0].Rows[intIdx]["SerialNumber"];
                }


            }

            catch (Exception ex)
            {
                context.Logger.LogLine("Ex in FunctionHandler " + ex.Message);
                oResult.HasError = false;
                oResult.Info = ex.Message;
            }

            return oResult;
        }
    }
}
