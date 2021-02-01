using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoBuildCrud
{
    public class tInput
    {
        public string Action { get; set; } = null;
        public string? UserName { get; set; }
        public int? EhiuBuildID { get; set; }
        public int? EhiuID { get; set; }
        public string? ConfigName { get; set; }
        public string? ScannedIMEI { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? TestResult { get; set; }
        public string? TestResultNote { get; set; }
        public DateTime? DateCreated { get; set; }



    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

    }

    public class CloudEcoBuildCrud
    {

        public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {

            string strQuery = "";
            string strQueryParams = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();

            SqlConnection oSqlConnection = null;
            tResult oResult = new tResult();




            try
            {

                context.Logger.LogLine("FunctionHandler 1 " + oInput.ToString());

                if (oInput.Action == null)
                {
                    context.Logger.LogLine("No Action " + oInput.ToString());

                    oResult.Ok = false;
                    oResult.Info = "No Action supplied";

                    return oResult;
                }

                oInput.Action = oInput.Action.ToLower();

                try
                {
                    oSqlConnection = new SqlConnection(ecoCommon.GetSecret("CloudEcoPlus", context)); oSqlConnection.Open();
                    context.Logger.LogLine("FunctionHandler 2");
                }
                catch (Exception ex)
                {
                    context.Logger.LogLine("WriteRecord Ex  1" + ex.Message);
                }



                // Validate passed 

                if ("insertupdatedelete".IndexOf(oInput.Action) == -1)
                {
                    oResult.Ok = false;
                    oResult.Info = "Action needs to be either INSERT DELETE or UPDATE";

                    return oResult;

                };


                    if (oInput.TestResult != null)
                    {
                        if ("passfailabandon".IndexOf(oInput.TestResult.ToLower()) == -1)
                        {
                            oResult.Ok = false;
                            oResult.Info = "Action needs to be either PASS FAIL or ABANDON";

                            return oResult;

                        };
                    }


                if (oInput.Action == "update" || oInput.Action == "delete")
                {
                    if (oInput.EhiuBuildID == null)
                    {
                        context.Logger.LogLine("No EhiuBuildID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuBuildID supplied";

                        return oResult;
                    }


                };

                if (oInput.Action == "insert")
                {
                    if (oInput.EhiuID == null)
                    {
                        context.Logger.LogLine("No EhiuID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuID supplied";

                        return oResult;
                    }


                    if (oInput.ConfigName == null)
                    {
                        context.Logger.LogLine("No ConfigName " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No ConfigName supplied";

                        return oResult;
                    }

                    if (oInput.ScannedIMEI == null)
                    {
                        context.Logger.LogLine("No ScannedIMEI " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No ScannedIMEI supplied";

                        return oResult;
                    }



                    if (ecoCommon.CodeExists(oInput.EhiuID, "Ehiu", "EhiuID", context, ref oSqlConnection) == false)
                    {
                        context.Logger.LogLine("EhiuID ref integrity " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "EhiuID references non existing ID";

                        return oResult;
                    }





                }



                // Validated now, 

                // Build the sql statements

                if (oInput.Action == "insert")
                {
                    strQuery = "";
                    strQueryParams = "";


                    if (oInput.EhiuID != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "EhiuID";
                        strQueryParams = strQueryParams + "@EhiuID";
                    }

                    if (oInput.ConfigName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "ConfigName";
                        strQueryParams = strQueryParams + "@ConfigName";
                    }

                    if (oInput.ScannedIMEI != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "ScannedIMEI";
                        strQueryParams = strQueryParams + "@ScannedIMEI";
                    }


                    if (oInput.StartTime != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "StartTime";
                        strQueryParams = strQueryParams + "@StartTime";
                    }

                    if (oInput.EndTime != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "EndTime";
                        strQueryParams = strQueryParams + "@EndTime";
                    }

                    if (oInput.TestResult != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "TestResult";
                        strQueryParams = strQueryParams + "@TestResult";
                    }

                    if (oInput.TestResultNote != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "TestResultNote";
                        strQueryParams = strQueryParams + "@TestResultNote";
                    }

                    if (oInput.DateCreated != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "DateCreated";
                        strQueryParams = strQueryParams + "@DateCreated";
                    }

                    if (oInput.UserName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "CreatedBy";
                        strQueryParams = strQueryParams + "@CreatedBy";
                    }

                    strQuery = "Insert Into EhiuBuild ( " + strQuery + ") Values " +
                                "( " + strQueryParams + ") " +
                                "SET @EhiuBuildID = Scope_Identity()";




                }




                if (oInput.Action == "update")
                {
                    strQuery = "";
                    strQueryParams = "";




                    if (oInput.EhiuID != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "EhiuID = @EhiuID ";
                    }

                    if (oInput.ConfigName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "ConfigName = @ConfigName ";
                    }

                    if (oInput.ScannedIMEI != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "ScannedIMEI = @ScannedIMEI ";
                    }


                    if (oInput.StartTime != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "StartTime = @StartTime ";
                    }

                    if (oInput.EndTime != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "EndTime = @EndTime ";
                    }

                    if (oInput.TestResult != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "TestResult = @TestResult ";
                    }

                    if (oInput.TestResultNote != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "TestResultNote = @TestResultNote ";
                    }



                    if (oInput.DateCreated != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "DateCreated = @DateCreated ";
                    }

                    if (oInput.UserName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "CreatedBy = @CreatedBy ";
                    }


                    strQuery = "Update EhiuBuild Set " + strQuery + " Where " +
                                " EhiuBuildID = @EhiuBuildID ";

                }

                if (oInput.Action == "delete") // Build params
                {
                    strQuery = "Delete From  EhiuBuild  Where " +
                              " EhiuBuildID = @EhiuBuildID ";
                }



                SqlCommand sqlInsert = new SqlCommand(strQuery, oSqlConnection);



                if (oInput.Action == "insert" || oInput.Action == "update") // Build params
                {

                    if (oInput.EhiuID != null)
                    {
                        SqlParameter sqlParamEhiuID = new SqlParameter("@EhiuID", SqlDbType.Int);
                        sqlParamEhiuID.Value = oInput.EhiuID;
                        sqlInsert.Parameters.Add(sqlParamEhiuID);
                    }

                    if (oInput.ConfigName != null)
                    {
                        SqlParameter sqlParamConfigName = new SqlParameter("@ConfigName", SqlDbType.NVarChar);
                        sqlParamConfigName.Value = oInput.ConfigName;
                        sqlInsert.Parameters.Add(sqlParamConfigName);
                    }

                    if (oInput.ScannedIMEI != null)
                    {
                        SqlParameter sqlParamScannedIMEI = new SqlParameter("@ScannedIMEI", SqlDbType.NVarChar);
                        sqlParamScannedIMEI.Value = oInput.ScannedIMEI;
                        sqlInsert.Parameters.Add(sqlParamScannedIMEI);
                    }

                    if (oInput.StartTime != null)
                    {
                        SqlParameter sqlParamStartTime = new SqlParameter("@StartTime", SqlDbType.DateTime);
                        sqlParamStartTime.Value = ecoCommon.DateJson(oInput.StartTime);
                        sqlInsert.Parameters.Add(sqlParamStartTime);
                    }



                    if (oInput.EndTime != null)
                    {
                        SqlParameter sqlParamEndTime = new SqlParameter("@EndTime", SqlDbType.DateTime);
                        sqlParamEndTime.Value = ecoCommon.DateJson(oInput.EndTime);
                        sqlInsert.Parameters.Add(sqlParamEndTime);
                    }



                    if (oInput.TestResult != null)
                    {
                        SqlParameter sqlParamTestResult = new SqlParameter("@TestResult", SqlDbType.NVarChar);
                        sqlParamTestResult.Value = oInput.TestResult.ToUpper();
                        sqlInsert.Parameters.Add(sqlParamTestResult);
                    }



                    if (oInput.TestResultNote != null)
                    {
                        SqlParameter sqlParamTestResultNote = new SqlParameter("@TestResultNote", SqlDbType.NVarChar);
                        sqlParamTestResultNote.Value = oInput.TestResultNote.ToUpper();
                        sqlInsert.Parameters.Add(sqlParamTestResultNote);
                    }





                    if (oInput.DateCreated != null)
                    {
                        SqlParameter sqlParamDateCreated = new SqlParameter("@DateCreated", SqlDbType.DateTime);
                        sqlParamDateCreated.Value = oInput.DateCreated;
                        sqlInsert.Parameters.Add(sqlParamDateCreated);
                    }



                    if (oInput.UserName != null)
                    {
                        SqlParameter sqlParamCreatedBy = new SqlParameter("@CreatedBy", SqlDbType.NVarChar);
                        sqlParamCreatedBy.Value = oInput.UserName;
                        sqlInsert.Parameters.Add(sqlParamCreatedBy);
                    }



                }

                if (oInput.Action == "delete" || oInput.Action == "update") // Build params
                {
                    SqlParameter sqlParamEhiuBuildID = new SqlParameter("@EhiuBuildID", SqlDbType.Int);
                    sqlParamEhiuBuildID.Value = oInput.EhiuBuildID;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildID);
                }

                if (oInput.Action == "insert")
                {
                    SqlParameter sqlParamEhiuBuildID = new SqlParameter("@EhiuBuildID", SqlDbType.Int);
                    sqlParamEhiuBuildID.Direction = ParameterDirection.Output;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildID);
                }




                try
                {
                    sqlInsert.Connection.Open();
                }
                catch (Exception)
                {
                }




                sqlInsert.ExecuteNonQuery();

                if (oInput.Action == "insert")
                {
                    oResult.Result = (int)sqlInsert.Parameters["@EhiuBuildID"].Value;
                }


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
