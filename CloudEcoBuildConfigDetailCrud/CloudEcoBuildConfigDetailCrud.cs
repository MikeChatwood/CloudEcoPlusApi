using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoBuildConfigDetailCrud
{
    public class tInput
    {
        public string Action { get; set; } = null;
        public string? UserName { get; set; }
        public int? EhiuBuildConfigDetailID { get; set; }
        public int? EhiuBuildConfigID { get; set; }
        public bool? Retired { get; set; }
        public int? DisplaySequence { get; set; }
        public bool? Mandatory { get; set; }
        public string? ConfigType { get; set; }
        public string? Prompt { get; set; }
        public string? Tag { get; set; }
        public string? TextDefault { get; set; }
        public int? TextMaxLength { get; set; }
        public string? OptionOne { get; set; }
        public string? OptionTwo { get; set; }
        public string? OptionThree { get; set; }
        public string? OptionOneTag { get; set; }
        public string? OptionTwoTag { get; set; }
        public string? OptionThreeTag { get; set; }
        public bool? OptionMultiChoice { get; set; }
        public decimal? NumericMinimum { get; set; }
        public decimal? NumericMaximum { get; set; }
        public int? NumericDecimals { get; set; }
        public decimal? NumericDefault { get; set; }
        public string? ImageFileName { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;


    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

    }
    public class CloudEcoBuildConfigDetailCrud
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


                if (oInput.Action == "delete")
                {


                    if (ecoCommon.CodeExists(oInput.EhiuBuildConfigDetailID, "EhiuBuildDetail", "EhiuBuildConfigDetailID", context, ref oSqlConnection) == false)
                    {
                        context.Logger.LogLine("EhiuBuildConfigDetailID Used " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "This code has been used, retire the code instead";

                        return oResult;
                    }


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

                    if (oInput.Mandatory == null)
                    {
                        context.Logger.LogLine("No Mandatory " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No Mandatory supplied";

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

                    if (oInput.Tag == null)
                    {
                        context.Logger.LogLine("No Tag " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No Tag supplied";

                        return oResult;
                    }

                    if (oInput.UserName == null)
                    {
                        context.Logger.LogLine("No CreatedBy " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No CreatedBy supplied";

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

                    if (oInput.Mandatory != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "Mandatory";
                        strQueryParams = strQueryParams + "@Mandatory";
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


                    if (oInput.Tag != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "Tag";
                        strQueryParams = strQueryParams + "@Tag";
                    }

                    if (oInput.TextDefault != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "TextDefault";
                        strQueryParams = strQueryParams + "@TextDefault";
                    }


                    if (oInput.TextMaxLength != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "TextMaxLength";
                        strQueryParams = strQueryParams + "@TextMaxLength";
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




                    if (oInput.OptionOneTag != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "OptionOneTag";
                        strQueryParams = strQueryParams + "@OptionOneTag";
                    }




                    if (oInput.OptionTwoTag != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "OptionTwoTag";
                        strQueryParams = strQueryParams + "@OptionTwoTag";
                    }




                    if (oInput.OptionThreeTag != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "OptionThreeTag";
                        strQueryParams = strQueryParams + "@OptionThreeTag";
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





                    if (oInput.NumericMinimum != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "NumericMinimum";
                        strQueryParams = strQueryParams + "@NumericMinimum";
                    }




                    if (oInput.NumericMaximum != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "NumericMaximum";
                        strQueryParams = strQueryParams + "@NumericMaximum";
                    }





                    if (oInput.NumericDecimals != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "NumericDecimals";
                        strQueryParams = strQueryParams + "@NumericDecimals";
                    }





                    if (oInput.NumericDefault != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "NumericDefault";
                        strQueryParams = strQueryParams + "@NumericDefault";
                    }






                    if (oInput.ImageFileName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "ImageFileName";
                        strQueryParams = strQueryParams + "@ImageFileName";
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


                    strQuery = "Insert Into EhiuBuildConfigDetail ( " + strQuery + ") Values " +
                                "( " + strQueryParams + ") " +
                                "SET @EhiuBuildConfigDetailID = Scope_Identity()";




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


                    if (oInput.Retired != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "Retired = @Retired ";
                    }

                    if (oInput.DisplaySequence != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "DisplaySequence = @DisplaySequence ";
                    }

                    if (oInput.Mandatory != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "Mandatory = @Mandatory ";
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


                    if (oInput.Tag != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "Tag = @Tag ";
                    }

                    if (oInput.TextDefault != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "TextDefault = @TextDefault ";
                    }

                    if (oInput.TextMaxLength != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "TextMaxLength = @TextMaxLength ";
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

                    if (oInput.OptionOneTag != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "OptionOneTag = @OptionOneTag ";
                    }


                    if (oInput.OptionTwoTag != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "OptionTwoTag = @OptionTwoTag ";
                    }



                    if (oInput.OptionThreeTag != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "OptionThreeTag = @OptionThreeTag ";
                    }


                    if (oInput.OptionMultiChoice != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "OptionMultiChoice = @OptionMultiChoice ";
                    }


                    if (oInput.NumericMinimum != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "NumericMinimum = @NumericMinimum ";
                    }


                    if (oInput.NumericMaximum != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "NumericMaximum = @NumericMaximum ";
                    }


                    if (oInput.NumericDecimals != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "NumericDecimals = @NumericDecimals ";
                    }


                    if (oInput.NumericDefault != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "NumericDefault = @NumericDefault ";
                    }


                    if (oInput.ImageFileName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "ImageFileName = @ImageFileName ";
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




                    strQuery = "Update EhiuBuildConfigDetail Set " + strQuery + " Where " +
                                " EhiuBuildConfigDetailID = @EhiuBuildConfigDetailID ";

                }

                if (oInput.Action == "delete") // Build params
                {
                    strQuery = "Delete From  EhiuBuildConfigDetail  Where " +
                              " EhiuBuildConfigDetailID = @EhiuBuildConfigDetailID ";
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

                    if (oInput.Retired != null)
                    {
                        SqlParameter sqlParamRetired = new SqlParameter("@Retired", SqlDbType.Bit);
                        sqlParamRetired.Value = oInput.Retired;
                        sqlInsert.Parameters.Add(sqlParamRetired);
                    }

                    if (oInput.DisplaySequence != null)
                    {
                        SqlParameter sqlParamDisplaySequence = new SqlParameter("@DisplaySequence", SqlDbType.Int);
                        sqlParamDisplaySequence.Value = oInput.DisplaySequence;
                        sqlInsert.Parameters.Add(sqlParamDisplaySequence);
                    }

                    if (oInput.Mandatory != null)
                    {
                        SqlParameter sqlParamMandatory = new SqlParameter("@Mandatory", SqlDbType.Bit);
                        sqlParamMandatory.Value = oInput.Mandatory;
                        sqlInsert.Parameters.Add(sqlParamMandatory);
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


                    if (oInput.Tag != null)
                    {
                        SqlParameter sqlParamTag = new SqlParameter("@Tag", SqlDbType.NVarChar);
                        sqlParamTag.Value = oInput.Tag;
                        sqlInsert.Parameters.Add(sqlParamTag);
                    }

                    if (oInput.TextDefault != null)
                    {
                        SqlParameter sqlParamTextDefault = new SqlParameter("@TextDefault", SqlDbType.NVarChar);
                        sqlParamTextDefault.Value = oInput.TextDefault;
                        sqlInsert.Parameters.Add(sqlParamTextDefault);
                    }

                    if (oInput.TextMaxLength != null)
                    {
                        SqlParameter sqlParamTextMaxLength = new SqlParameter("@TextMaxLength", SqlDbType.Int);
                        sqlParamTextMaxLength.Value = oInput.TextMaxLength;
                        sqlInsert.Parameters.Add(sqlParamTextMaxLength);
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

                    if (oInput.OptionOneTag != null)
                    {
                        SqlParameter sqlParamOptionOneTag = new SqlParameter("@OptionOneTag", SqlDbType.NVarChar);
                        sqlParamOptionOneTag.Value = oInput.OptionOneTag;
                        sqlInsert.Parameters.Add(sqlParamOptionOneTag);
                    }

                    if (oInput.OptionTwoTag != null)
                    {
                        SqlParameter sqlParamOptionTwoTag = new SqlParameter("@OptionTwoTag", SqlDbType.NVarChar);
                        sqlParamOptionTwoTag.Value = oInput.OptionTwoTag;
                        sqlInsert.Parameters.Add(sqlParamOptionTwoTag);
                    }

                    if (oInput.OptionThreeTag != null)
                    {
                        SqlParameter sqlParamOptionThreeTag = new SqlParameter("@OptionThreeTag", SqlDbType.NVarChar);
                        sqlParamOptionThreeTag.Value = oInput.OptionThreeTag;
                        sqlInsert.Parameters.Add(sqlParamOptionThreeTag);
                    }

                    if (oInput.OptionMultiChoice != null)
                    {
                        SqlParameter sqlParamOptionMultiChoice = new SqlParameter("@OptionMultiChoice", SqlDbType.Bit);
                        sqlParamOptionMultiChoice.Value = oInput.OptionMultiChoice;
                        sqlInsert.Parameters.Add(sqlParamOptionMultiChoice);
                    }

                    if (oInput.NumericMinimum != null)
                    {
                        SqlParameter sqlParamNumericMinimum = new SqlParameter("@NumericMinimum", SqlDbType.Decimal);
                        sqlParamNumericMinimum.Value = oInput.NumericMinimum;
                        sqlInsert.Parameters.Add(sqlParamNumericMinimum);
                    }


                    if (oInput.NumericMaximum != null)
                    {
                        SqlParameter sqlParamNumericMaximum = new SqlParameter("@NumericMaximum", SqlDbType.Decimal);
                        sqlParamNumericMaximum.Value = oInput.NumericMaximum;
                        sqlInsert.Parameters.Add(sqlParamNumericMaximum);
                    }

                    if (oInput.NumericDecimals != null)
                    {
                        SqlParameter sqlParamNumericDecimals = new SqlParameter("@NumericDecimals", SqlDbType.Int);
                        sqlParamNumericDecimals.Value = oInput.NumericDecimals;
                        sqlInsert.Parameters.Add(sqlParamNumericDecimals);
                    }

                    if (oInput.NumericDefault != null)
                    {
                        SqlParameter sqlParamNumericDefault = new SqlParameter("@NumericDefault", SqlDbType.Decimal);
                        sqlParamNumericDefault.Value = oInput.NumericDefault;
                        sqlInsert.Parameters.Add(sqlParamNumericDefault);
                    }

                    if (oInput.ImageFileName != null)
                    {
                        SqlParameter sqlParamImageFileName = new SqlParameter("@ImageFileName", SqlDbType.NVarChar);
                        sqlParamImageFileName.Value = oInput.ImageFileName;
                        sqlInsert.Parameters.Add(sqlParamImageFileName);
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
                    SqlParameter sqlParamEhiuBuildConfigDetailID = new SqlParameter("@EhiuBuildConfigDetailID", SqlDbType.Int);
                    sqlParamEhiuBuildConfigDetailID.Value = oInput.EhiuBuildConfigDetailID;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildConfigDetailID);
                }

                if (oInput.Action == "insert")
                {
                    SqlParameter sqlParamEhiuBuildConfigDetailID = new SqlParameter("@EhiuBuildConfigDetailID", SqlDbType.Int);
                    sqlParamEhiuBuildConfigDetailID.Direction = ParameterDirection.Output;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildConfigDetailID);
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
                    oResult.Result = (int)sqlInsert.Parameters["@EhiuBuildConfigDetailID"].Value;
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
