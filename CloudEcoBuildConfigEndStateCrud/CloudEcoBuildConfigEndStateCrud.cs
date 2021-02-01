using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoBuildConfigEndStateCrud
{

    public class tInput
    {
        public string Action { get; set; } = null;
        public string? UserName { get; set; }
        public int? EhiuBuildConfigEndStateID { get; set; }
        public int? EhiuBuildConfigID { get; set; }
        public int? DisplaySequence { get; set; }
        public bool? Retired { get; set; }
        public string? Prompt { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;


    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

    }
    public class CloudEcoBuildConfigEndStateCrud
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

                if (oInput.Action == "update" || oInput.Action == "delete")
                {
                    if (oInput.EhiuBuildConfigEndStateID == null)
                    {
                        context.Logger.LogLine("No EhiuBuildConfigEndStateID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuBuildConfigEndStateID supplied";

                        return oResult;
                    }
                    else
                    {
                        if (ecoCommon.CodeExists(oInput.EhiuBuildConfigEndStateID, "EhiuBuildConfigEndState", "EhiuBuildConfigEndStateID", context, ref oSqlConnection) == false)
                        {
                            context.Logger.LogLine("EhiuBuildConfigEndStateID does not exist " + oInput.ToString());

                            oResult.Ok = false;
                            oResult.Info = "EhiuBuildConfigEndStateID does not exist";

                            return oResult;
                        }
                    }



                };

                if (oInput.EhiuBuildConfigEndStateID == null)
                {
                    if (ecoCommon.CodeExists(oInput.EhiuBuildConfigID, "EhiuBuildConfig", "EhiuBuildConfigID", context, ref oSqlConnection) == false)
                    {
                        context.Logger.LogLine("EhiuBuildConfigID does not exist " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "EhiuBuildConfigID does not exist";

                        return oResult;
                    }

                }


                if (oInput.Action == "delete")
                {


              /*      if (ecoCommon.CodeExists(oInput.EhiuBuildConfigEndStateID, "EhiuBuild", "EhiuBuildConfigEndStateID", context, ref oSqlConnection) == false)
                    {
                        context.Logger.LogLine("EhiuBuildConfigID Used " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "This code has been used, retire the code instead";

                        return oResult;
                    }
              */ 

                };

                if (oInput.Action == "insert")
                {

                    if (oInput.EhiuBuildConfigID == null)
                    {
                        context.Logger.LogLine("No EhiuBuildConfigID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuBuildConfigID supplied";

                        return oResult;
                    }
                    if (oInput.DisplaySequence == null)
                    {
                        context.Logger.LogLine("No DisplaySequence " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No DisplaySequence supplied";

                        return oResult;
                    }

                    if (oInput.Prompt == null)
                    {
                        context.Logger.LogLine("No Prompt " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No Prompt supplied";

                        return oResult;
                    }



                    if (oInput.UserName == null)
                    {
                        context.Logger.LogLine("No UserName " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No UserName supplied";

                        return oResult;
                    }



                }



                // Validated now, 

                // Build the sql statements

                if (oInput.Action == "insert")
                {
                    strQuery = "";
                    strQueryParams = "";

                    if (oInput.EhiuBuildConfigID != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "EhiuBuildConfigID";
                        strQueryParams = strQueryParams + "@EhiuBuildConfigID";
                    }

                    if (oInput.DisplaySequence != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "DisplaySequence";
                        strQueryParams = strQueryParams + "@DisplaySequence";
                    }

                    if (oInput.Retired != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "Retired";
                        strQueryParams = strQueryParams + "@Retired";
                    }


                    if (oInput.Prompt != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "Prompt";
                        strQueryParams = strQueryParams + "@Prompt";
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


                    strQuery = "Insert Into EhiuBuildConfigEndState ( " + strQuery + ") Values " +
                                "( " + strQueryParams + ") " +
                                "SET @EhiuBuildConfigEndStateID = Scope_Identity()";




                }




                if (oInput.Action == "update")
                {
                    strQuery = "";
                    strQueryParams = "";



                    if (oInput.EhiuBuildConfigID != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "EhiuBuildConfigID = @EhiuBuildConfigID ";
                    }

                    if (oInput.DisplaySequence != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "DisplaySequence = @DisplaySequence ";
                    }



                    if (oInput.Retired != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "Retired = @Retired ";
                    }

                    if (oInput.Prompt != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "Prompt = @Prompt ";
                    }

                    if (oInput.UserName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "CreatedBy = @CreatedBy ";
                    }




                    strQuery = "Update EhiuBuildConfigEndState Set " + strQuery + " Where " +
                                " EhiuBuildConfigEndStateID = @EhiuBuildConfigEndStateID ";

                }

                if (oInput.Action == "delete") // Build params
                {
                    strQuery = "Delete From  EhiuBuildConfigEndState  Where " +
                               " EhiuBuildConfigEndStateID = @EhiuBuildConfigEndStateID ";
                }



                SqlCommand sqlInsert = new SqlCommand(strQuery, oSqlConnection);



                if (oInput.Action == "insert" || oInput.Action == "update") // Build params
                {

                    if (oInput.EhiuBuildConfigID != null)
                    {
                        SqlParameter sqlParamEhiuBuildConfigID = new SqlParameter("@EhiuBuildConfigID", SqlDbType.Int);
                        sqlParamEhiuBuildConfigID.Value = oInput.EhiuBuildConfigID;
                        sqlInsert.Parameters.Add(sqlParamEhiuBuildConfigID);
                    }


                    if (oInput.DisplaySequence != null)
                    {
                        SqlParameter sqlParamDisplaySequence = new SqlParameter("@DisplaySequence", SqlDbType.Int);
                        sqlParamDisplaySequence.Value = oInput.DisplaySequence;
                        sqlInsert.Parameters.Add(sqlParamDisplaySequence);
                    }


                    if (oInput.Retired != null)
                    {
                        SqlParameter sqlParamRetired = new SqlParameter("@Retired", SqlDbType.Bit);
                        sqlParamRetired.Value = oInput.Retired;
                        sqlInsert.Parameters.Add(sqlParamRetired);
                    }

                    if (oInput.Prompt != null)
                    {
                        SqlParameter sqlParamPrompt = new SqlParameter("@Prompt", SqlDbType.NVarChar);
                        sqlParamPrompt.Value = oInput.Prompt;
                        sqlInsert.Parameters.Add(sqlParamPrompt);
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
                    SqlParameter sqlParamEhiuBuildConfigEndStateID = new SqlParameter("@EhiuBuildConfigEndStateID", SqlDbType.Int);
                    sqlParamEhiuBuildConfigEndStateID.Value = oInput.EhiuBuildConfigEndStateID;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildConfigEndStateID);
                }

                if (oInput.Action == "insert")
                {
                    SqlParameter sqlParamEhiuBuildConfigEndStateID = new SqlParameter("@EhiuBuildConfigEndStateID", SqlDbType.Int);
                    sqlParamEhiuBuildConfigEndStateID.Direction = ParameterDirection.Output;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildConfigEndStateID);
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
                    oResult.Result = (int)sqlInsert.Parameters["@EhiuBuildConfigEndStateID"].Value;
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
