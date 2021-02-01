using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoGetEhiu
{

    public class tInput
    {
        public string SerialNumber { get; set; } = null;
    }

    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";

        public List<tEhiuTest> EhiuTests { get; set; } = new List<tEhiuTest>();

        public class tEhiuTest
        {

            public int? EhiuID { get; set; }
            public string? SerialNumber { get; set; }
            public string? IMEI { get; set; }
            public bool? TestsPassed { get; set; }
            public DateTime? DateCreated { get; set; }
            public string? CreatedBy { get; set; }


        };
    }
    public class CloudEcoGetEhiu
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

                strQuery = "SELECT EhiuID, SerialNumber, IMEI, TestsPassed, DateCreated, CreatedBy " +
                           " FROM Ehiu " +
                           " WHERE (SerialNumber = @SerialNumber)";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamSerialNumber = new SqlParameter("@SerialNumber", SqlDbType.NVarChar);
                sqlParamSerialNumber.Value = oInput.SerialNumber;
                daCheck.SelectCommand.Parameters.Add(sqlParamSerialNumber);
                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {
                    tResult.tEhiuTest oEhiuTest = new tResult.tEhiuTest();

                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuID"] != DBNull.Value)
                    {
                        oEhiuTest.EhiuID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuID"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["SerialNumber"] != DBNull.Value)
                    {
                        oEhiuTest.SerialNumber = (string)dsCheck.Tables[0].Rows[intIdx]["SerialNumber"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["IMEI"] != DBNull.Value)
                    {
                        oEhiuTest.IMEI = (string)dsCheck.Tables[0].Rows[intIdx]["IMEI"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["TestsPassed"] != DBNull.Value)
                    {
                        oEhiuTest.TestsPassed = (bool)dsCheck.Tables[0].Rows[intIdx]["TestsPassed"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["DateCreated"] != DBNull.Value)
                    {
                        oEhiuTest.DateCreated = (DateTime)dsCheck.Tables[0].Rows[intIdx]["DateCreated"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["CreatedBy"] != DBNull.Value)
                    {
                        oEhiuTest.CreatedBy = (string)dsCheck.Tables[0].Rows[intIdx]["CreatedBy"];
                    }


                    oResult.EhiuTests.Add(oEhiuTest);
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
