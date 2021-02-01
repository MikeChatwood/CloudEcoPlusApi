using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoDeRegisterUnit
{

    public class tInput
    {
        public string SerialNumber { get; set; } = null;
        

    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";
    }

    public class CloudEcoDeRegisterUnit
    {

        public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {

            SqlConnection oSqlConnection = null;
            tResult oResult = new tResult();
            string strQuery;

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
            try
            {
                oSqlConnection.Open();
            }
            catch (Exception)
            {
            }

            try
            {
                strQuery = "Update EhiuInstall Set  ToDate = GetDate() Where SerialNumber = @SerialNumber ";

                SqlCommand sqlCommInsert = new SqlCommand(strQuery, oSqlConnection);



                SqlParameter aParamSerialNumber = new SqlParameter("@SerialNumber", SqlDbType.NVarChar);
                aParamSerialNumber.Value = oInput .SerialNumber;
                sqlCommInsert.Parameters.Add(aParamSerialNumber);

                sqlCommInsert.ExecuteNonQuery();

                oSqlConnection.Close();
            }
            catch (Exception ex)
            {

                oResult.Ok = false;
                oResult.Info = ex.Message;

                context.Logger.LogLine("FunctionHandler Ex 2" + ex.Message);
            }

            return oResult;

        }
    }
}
