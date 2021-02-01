using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoBuildDetailCrud
{

    public class tInput
    {
        public string Action { get; set; } = null;
        public string? UserName { get; set; }
        public int? EhiuBuildDetailID { get; set; }  // PK
        public int? EhiuBuildID { get; set; }  // FK
        public int? EhiuBuildConfigDetailID { get; set; }

        public string? ConfigType { get; set; }
        public string? Prompt { get; set; }
        public string? TestTag { get; set; }
        public string? TextResponse { get; set; }
        public bool? OptionMultiChoice { get; set; }
        public string? OptionOne { get; set; }
        public string? OptionTwo { get; set; }
        public string? OptionThree { get; set; }
        public string? OptionOneResponse { get; set; }
        public string? OptionTwoResponse { get; set; }
        public string? OptionThreeResponse { get; set; }
        public decimal? NumericResponse { get; set; }
        public DateTime? DateCreated { get; set; }



    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

    }
    public class CloudEcoBuildDetailCrud
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
                    if (oInput.EhiuBuildConfigDetailID == null)
                    {
                        context.Logger.LogLine("No EhiuBuildConfigDetailID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuBuildConfigDetailID supplied";

                        return oResult;
                    }


                };

                if (oInput.Action == "insert")
                {
                    if (oInput.EhiuBuildID == null)
                    {
                        context.Logger.LogLine("No EhiuBuildID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuBuildID supplied";

                        return oResult;
                    }

                    if (oInput.EhiuBuildConfigDetailID == null)
                    {
                        context.Logger.LogLine("No EhiuBuildConfigDetailID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuBuildConfigDetailID supplied";

                        return oResult;
                    }


                    if (oInput.ConfigType == null)
                    {
                        context.Logger.LogLine("No ConfigType " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No ConfigType supplied";

                        return oResult;
                    }

                    if (oInput.Prompt == null)
                    {
                        context.Logger.LogLine("No Prompt " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No Prompt supplied";

                        return oResult;
                    }

                    if (oInput.TestTag == null)
                    {
                        context.Logger.LogLine("No TestTag " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No TestTag supplied";

                        return oResult;
                    }

                    if (ecoCommon.CodeExists(oInput.EhiuBuildConfigDetailID, "EhiuBuildConfigDetail", "EhiuBuildConfigDetailID", context, ref oSqlConnection) == false)
                    {
                        context.Logger.LogLine("EhiuBuildConfigID ref integrity " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "EhiuBuildConfigID references non existing ID";

                        return oResult;
                    }
                    if (ecoCommon.CodeExists(oInput.EhiuBuildID, "EhiuBuild", "EhiuBuildID", context, ref oSqlConnection) == false)
                    {
                        context.Logger.LogLine("EhiuBuildID ref integrity " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "EhiuBuildID references non existing ID";

                        return oResult;
                    }





                }



                // Validated now, 

                // Build the sql statements

                if (oInput.Action == "insert")
                {
                    strQuery = "";
                    strQueryParams = "";


                    if (oInput.EhiuBuildID != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "EhiuBuildID";
                        strQueryParams = strQueryParams + "@EhiuBuildID";
                    }

                    if (oInput.EhiuBuildConfigDetailID != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "EhiuBuildConfigDetailID";
                        strQueryParams = strQueryParams + "@EhiuBuildConfigDetailID";
                    }

                    if (oInput.ConfigType != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "ConfigType";
                        strQueryParams = strQueryParams + "@ConfigType";
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


                    if (oInput.TestTag != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "TestTag";
                        strQueryParams = strQueryParams + "@TestTag";
                    }


                    if (oInput.TextResponse != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "TextResponse";
                        strQueryParams = strQueryParams + "@TextResponse";
                    }

                    if (oInput.OptionMultiChoice != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "OptionMultiChoice";
                        strQueryParams = strQueryParams + "@OptionMultiChoice";
                    }

                    if (oInput.OptionOne != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "OptionOne";
                        strQueryParams = strQueryParams + "@OptionOne";
                    }

                    if (oInput.OptionTwo != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "OptionTwo";
                        strQueryParams = strQueryParams + "@OptionTwo";
                    }

                    if (oInput.OptionThree != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "OptionThree";
                        strQueryParams = strQueryParams + "@OptionThree";
                    }

                    if (oInput.OptionOneResponse != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "OptionOneResponse";
                        strQueryParams = strQueryParams + "@OptionOneResponse";
                    }


                    if (oInput.OptionTwoResponse != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "OptionTwoResponse";
                        strQueryParams = strQueryParams + "@OptionTwoResponse";
                    }

                    if (oInput.OptionThreeResponse != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "OptionThreeResponse";
                        strQueryParams = strQueryParams + "@OptionThreeResponse";
                    }


                    if (oInput.NumericResponse != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "NumericResponse";
                        strQueryParams = strQueryParams + "@NumericResponse";
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

                    strQuery = "Insert Into EhiuBuildDetail ( " + strQuery + ") Values " +
                                "( " + strQueryParams + ") " +
                                "SET @EhiuBuildDetailID = Scope_Identity()";




                }




                if (oInput.Action == "update")
                {
                    strQuery = "";
                    strQueryParams = "";




                    if (oInput.EhiuBuildID != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "EhiuBuildID = @EhiuBuildID ";
                    }


                    if (oInput.EhiuBuildConfigDetailID != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "EhiuBuildConfigDetailID = @EhiuBuildConfigDetailID ";
                    }

                    if (oInput.ConfigType != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "ConfigType = @ConfigType ";
                    }

                    if (oInput.Prompt != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "Prompt = @Prompt ";
                    }

                    if (oInput.TestTag != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "TestTag = @TestTag ";
                    }

                    if (oInput.TextResponse != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "TextResponse = @TextResponse ";
                    }

                    if (oInput.OptionMultiChoice != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "OptionMultiChoice = @OptionMultiChoice ";
                    }


                    if (oInput.OptionOne != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "OptionOne = @OptionOne ";
                    }


                    if (oInput.OptionTwo != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "OptionTwo = @OptionTwo ";
                    }


                    if (oInput.OptionThree != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "OptionThree = @OptionThree ";
                    }


                    if (oInput.OptionOneResponse != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "OptionOneResponse = @OptionOneResponse ";
                    }


                    if (oInput.OptionTwoResponse != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "OptionTwoResponse = @OptionTwoResponse ";
                    }

                    if (oInput.OptionThreeResponse != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "OptionThreeResponse = @OptionThreeResponse ";
                    }

                    if (oInput.NumericResponse != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "NumericResponse = @NumericResponse ";
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





                    strQuery = "Update EhiuBuildDetail Set " + strQuery + " Where " +
                                " EhiuBuildDetailID = @EhiuBuildDetailID ";

                }

                if (oInput.Action == "delete") // Build params
                {
                    strQuery = "Delete From  EhiuBuildDetail  Where " +
                              " EhiuBuildDetailID = @EhiuBuildDetailID ";
                }



                SqlCommand sqlInsert = new SqlCommand(strQuery, oSqlConnection);



                if (oInput.Action == "insert" || oInput.Action == "update") // Build params
                {

                    if (oInput.EhiuBuildID != null)
                    {
                        SqlParameter sqlParamEhiuID = new SqlParameter("@EhiuBuildID", SqlDbType.Int);
                        sqlParamEhiuID.Value = oInput.EhiuBuildID;
                        sqlInsert.Parameters.Add(sqlParamEhiuID);
                    }

                    if (oInput.EhiuBuildConfigDetailID != null)
                    {
                        SqlParameter sqlParamEhiuBuildConfigID = new SqlParameter("@EhiuBuildConfigDetailID", SqlDbType.Int);
                        sqlParamEhiuBuildConfigID.Value = oInput.EhiuBuildConfigDetailID;
                        sqlInsert.Parameters.Add(sqlParamEhiuBuildConfigID);
                    }



                    if (oInput.ConfigType != null)
                    {
                        SqlParameter sqlParamConfigType = new SqlParameter("@ConfigType", SqlDbType.NVarChar);
                        sqlParamConfigType.Value = oInput.ConfigType;
                        sqlInsert.Parameters.Add(sqlParamConfigType);
                    }



                    if (oInput.Prompt != null)
                    {
                        SqlParameter sqlParamPrompt = new SqlParameter("@Prompt", SqlDbType.NVarChar);
                        sqlParamPrompt.Value = oInput.Prompt;
                        sqlInsert.Parameters.Add(sqlParamPrompt);
                    }



                    if (oInput.TestTag != null)
                    {
                        SqlParameter sqlParamTestTag = new SqlParameter("@TestTag", SqlDbType.NVarChar);
                        sqlParamTestTag.Value = oInput.TestTag;
                        sqlInsert.Parameters.Add(sqlParamTestTag);
                    }



                    if (oInput.TextResponse != null)
                    {
                        SqlParameter sqlParamTextResponse = new SqlParameter("@TextResponse", SqlDbType.NVarChar);
                        sqlParamTextResponse.Value = oInput.TextResponse;
                        sqlInsert.Parameters.Add(sqlParamTextResponse);
                    }



                    if (oInput.OptionMultiChoice != null)
                    {
                        SqlParameter sqlParamOptionMultiChoice = new SqlParameter("@OptionMultiChoice", SqlDbType.Bit);
                        sqlParamOptionMultiChoice.Value = oInput.OptionMultiChoice;
                        sqlInsert.Parameters.Add(sqlParamOptionMultiChoice);
                    }




                    if (oInput.OptionOne != null)
                    {
                        SqlParameter sqlParamOptionOne = new SqlParameter("@OptionOne", SqlDbType.NVarChar);
                        sqlParamOptionOne.Value = oInput.OptionOne;
                        sqlInsert.Parameters.Add(sqlParamOptionOne);
                    }



                    if (oInput.OptionTwo != null)
                    {
                        SqlParameter sqlParamOptionTwo = new SqlParameter("@OptionTwo", SqlDbType.NVarChar);
                        sqlParamOptionTwo.Value = oInput.OptionTwo;
                        sqlInsert.Parameters.Add(sqlParamOptionTwo);
                    }



                    if (oInput.OptionThree != null)
                    {
                        SqlParameter sqlParamOptionThree = new SqlParameter("@OptionThree", SqlDbType.NVarChar);
                        sqlParamOptionThree.Value = oInput.OptionThree;
                        sqlInsert.Parameters.Add(sqlParamOptionThree);
                    }



                    if (oInput.OptionOneResponse != null)
                    {
                        SqlParameter sqlParamOptionOneResponse = new SqlParameter("@OptionOneResponse", SqlDbType.NVarChar);
                        sqlParamOptionOneResponse.Value = oInput.OptionOneResponse;
                        sqlInsert.Parameters.Add(sqlParamOptionOneResponse);
                    }



                    if (oInput.OptionTwoResponse != null)
                    {
                        SqlParameter sqlParamOptionTwoResponse = new SqlParameter("@OptionTwoResponse", SqlDbType.NVarChar);
                        sqlParamOptionTwoResponse.Value = oInput.OptionTwoResponse;
                        sqlInsert.Parameters.Add(sqlParamOptionTwoResponse);
                    }



                    if (oInput.OptionThreeResponse != null)
                    {
                        SqlParameter sqlParamOptionThreeResponse = new SqlParameter("@OptionThreeResponse", SqlDbType.NVarChar);
                        sqlParamOptionThreeResponse.Value = oInput.OptionThreeResponse;
                        sqlInsert.Parameters.Add(sqlParamOptionThreeResponse);
                    }



                    if (oInput.NumericResponse != null)
                    {
                        SqlParameter sqlParamNumericResponse = new SqlParameter("@NumericResponse", SqlDbType.Int);
                        sqlParamNumericResponse.Value = oInput.NumericResponse;
                        sqlInsert.Parameters.Add(sqlParamNumericResponse);
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
                    SqlParameter sqlParamEhiuBuildDetailID = new SqlParameter("@EhiuBuildDetailID", SqlDbType.Int);
                    sqlParamEhiuBuildDetailID.Value = oInput.EhiuBuildDetailID;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildDetailID);
                }

                if (oInput.Action == "insert")
                {
                    SqlParameter sqlParamEhiuBuildDetailID = new SqlParameter("@EhiuBuildDetailID", SqlDbType.Int);
                    sqlParamEhiuBuildDetailID.Direction = ParameterDirection.Output;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildDetailID);
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
                    oResult.Result = (int)sqlInsert.Parameters["@EhiuBuildDetailID"].Value;
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
