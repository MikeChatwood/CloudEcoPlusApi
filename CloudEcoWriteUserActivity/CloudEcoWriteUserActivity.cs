using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoWriteUserActivity
{


    public class tInput
    {
        public string FormName { get; set; } = "";
        public string Button { get; set; } = "";
        public string ActionWho { get; set; } = "";

    };

    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";
    }


    public class CloudEcoWriteUserActivity
    {


        public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {
            string strQuery = "";
            SqlConnection oSqlConnection = null;
            tResult oResult = new tResult();

            try
            {


                try
                {
                    oSqlConnection = new SqlConnection(ecoCommon.GetSecret("CloudEcoPlus", context)); oSqlConnection.Open();
                    context.Logger.LogLine("FunctionHandler 2");
                }
                catch (Exception ex)
                {
                    context.Logger.LogLine("WriteRecord Ex  1" + ex.Message);
                }

                strQuery = " INSERT INTO UserActivity( FormName, Button, ActionWho) " +
                            " VALUES (@FormName, @Button, @ActionWho)";


                SqlCommand sqlInsert = new SqlCommand(strQuery, oSqlConnection);

                SqlParameter sqlParamFormName = new SqlParameter("@FormName", SqlDbType.NVarChar);
                sqlParamFormName.Value = oInput.FormName;
                sqlInsert.Parameters.Add(sqlParamFormName);

                SqlParameter sqlParamButton = new SqlParameter("@Button", SqlDbType.NVarChar);
                sqlParamButton.Value = oInput.Button;
                sqlInsert.Parameters.Add(sqlParamButton);

                SqlParameter sqlParamActionWho = new SqlParameter("@ActionWho", SqlDbType.NVarChar);
                sqlParamActionWho.Value = oInput.ActionWho;
                sqlInsert.Parameters.Add(sqlParamActionWho);

                try
                {
                    sqlInsert.Connection.Open();
                }
                catch (Exception)
                {
                }




                sqlInsert.ExecuteNonQuery();

                sqlInsert.Connection.Close();

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
