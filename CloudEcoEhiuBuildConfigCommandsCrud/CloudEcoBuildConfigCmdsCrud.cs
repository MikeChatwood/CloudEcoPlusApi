using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoBuildConfigCmdsCrud
{


    public class tInput
    {
        public string Action { get; set; } = null;
        public string? UserName { get; set; }

        public int? EhiuBuildConfigCommandsID { get; set; }
        public string? CommandName { get; set; }
        public bool? ToEco { get; set; }
        public string? JsonConfigItem { get; set; }
        public string? CompanionName { get; set; }
        public string? JsonCompanionItem { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? MbusID { get; set; } 
        public string? UrlPath { get; set; } 



    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

    }
    public class CloudEcoBuildConfigCmdsCrud
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

                if (oInput.UserName == null)
                {
                    context.Logger.LogLine("No UserName " + oInput.ToString());

                    oResult.Ok = false;
                    oResult.Info = "No UserName supplied";

                    return oResult;
                }


                if (oInput.ToEco == null)
                {
                    context.Logger.LogLine("No ToEco " + oInput.ToString());

                    oResult.Ok = false;
                    oResult.Info = "No ToEco supplied";

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
                    if (oInput.EhiuBuildConfigCommandsID == null)
                    {
                        context.Logger.LogLine("No EhiuBuildConfigCommandsID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuBuildConfigCommandsID supplied";

                        return oResult;
                    }


                };

              

                if (oInput.Action == "insert")
                {
                    if (oInput.CommandName == null)
                    {
                        context.Logger.LogLine("No CommandName " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No CommandName supplied";

                        return oResult;
                    }


                    if (oInput.ToEco == null)
                    {
                        context.Logger.LogLine("No ToEco " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No ToEco supplied";

                        return oResult;
                    }





                }



                // Validated now, 

                // Build the sql statements

                if (oInput.Action == "insert")
                {
                    strQuery = "";
                    strQueryParams = "";


                    if (oInput.CommandName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "CommandName";
                        strQueryParams = strQueryParams + "@CommandName";
                    }

                    if (oInput.ToEco != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "ToEco";
                        strQueryParams = strQueryParams + "@ToEco";
                    }


                    if (oInput.JsonConfigItem != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "JsonConfigItem";
                        strQueryParams = strQueryParams + "@JsonConfigItem";
                    }


                    if (oInput.CompanionName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "CompanionName";
                        strQueryParams = strQueryParams + "@CompanionName";
                    }


                    if (oInput.JsonCompanionItem != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "JsonCompanionItem";
                        strQueryParams = strQueryParams + "@JsonCompanionItem";
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

                    if (oInput.UrlPath  != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "UrlPath";
                        strQueryParams = strQueryParams + "@UrlPath";
                    }

                    if (oInput.MbusID  != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "MbusID";
                        strQueryParams = strQueryParams + "@MbusID";
                    }




                    strQuery = "Insert Into EhiuBuildConfigCommands ( " + strQuery + ") Values " +
                                "( " + strQueryParams + ") " +
                                "SET @EhiuBuildConfigCommandsID = Scope_Identity()";




                }




                if (oInput.Action == "update")
                {
                    strQuery = "";
                    strQueryParams = "";




                    if (oInput.CommandName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "CommandName = @CommandName ";
                    }

                    if (oInput.ToEco != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "ToEco = @ToEco ";
                    }

                    if (oInput.JsonConfigItem != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "JsonConfigItem = @JsonConfigItem ";
                    }

                    if (oInput.CompanionName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "CompanionName = @CompanionName ";
                    }

                    if (oInput.JsonCompanionItem != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "JsonCompanionItem = @JsonCompanionItem ";
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


                    if (oInput.MbusID  != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "MbusID = @MbusID ";
                    }

                    if (oInput.UrlPath != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "UrlPath = @UrlPath ";
                    }






                    strQuery = "Update EhiuBuildConfigCommands Set " + strQuery + " Where " +
                                " EhiuBuildConfigCommandsID = @EhiuBuildConfigCommandsID ";

                }

                if (oInput.Action == "delete") // Build params
                {
                    strQuery = "Delete From  EhiuBuildConfigCommands  Where " +
                              " EhiuBuildConfigCommandsID = @EhiuBuildConfigCommandsID ";
                }



                SqlCommand sqlInsert = new SqlCommand(strQuery, oSqlConnection);



                if (oInput.Action == "insert" || oInput.Action == "update") // Build params
                {

                    if (oInput.CommandName != null)
                    {
                        SqlParameter sqlParamCommandName = new SqlParameter("@CommandName", SqlDbType.NVarChar);
                        sqlParamCommandName.Value = oInput.CommandName;
                        sqlInsert.Parameters.Add(sqlParamCommandName);
                    }


                    if (oInput.ToEco != null)
                    {
                        SqlParameter sqlParamToEco = new SqlParameter("@ToEco", SqlDbType.Bit);
                        sqlParamToEco.Value = oInput.ToEco;
                        sqlInsert.Parameters.Add(sqlParamToEco);
                    }

                    if (oInput.JsonConfigItem != null)
                    {
                        SqlParameter sqlParamJsonConfigItem = new SqlParameter("@JsonConfigItem", SqlDbType.NVarChar);
                        sqlParamJsonConfigItem.Value = oInput.JsonConfigItem;
                        sqlInsert.Parameters.Add(sqlParamJsonConfigItem);
                    }

                    if (oInput.CompanionName != null)
                    {
                        SqlParameter sqlParamCompanionName = new SqlParameter("@CompanionName", SqlDbType.NVarChar);
                        sqlParamCompanionName.Value = oInput.CompanionName;
                        sqlInsert.Parameters.Add(sqlParamCompanionName);
                    }


                    if (oInput.JsonCompanionItem != null)
                    {
                        SqlParameter sqlParamJsonCompanionItem = new SqlParameter("@JsonCompanionItem", SqlDbType.NVarChar);
                        sqlParamJsonCompanionItem.Value = oInput.JsonCompanionItem;
                        sqlInsert.Parameters.Add(sqlParamJsonCompanionItem);
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


                    if (oInput.MbusID != null)
                    {
                        SqlParameter sqlParamMbusID = new SqlParameter("@MbusID", SqlDbType.Int);
                        sqlParamMbusID.Value = oInput.MbusID;
                        sqlInsert.Parameters.Add(sqlParamMbusID);
                    }


                    if (oInput.UrlPath != null)
                    {
                        SqlParameter sqlParamUrlPath = new SqlParameter("@UrlPath", SqlDbType.NVarChar);
                        sqlParamUrlPath.Value = oInput.UrlPath;
                        sqlInsert.Parameters.Add(sqlParamUrlPath);
                    }



                }

                if (oInput.Action == "delete" || oInput.Action == "update") // Build params
                {
                    SqlParameter sqlParamEhiuBuildConfigCommandsID = new SqlParameter("@EhiuBuildConfigCommandsID", SqlDbType.Int);
                    sqlParamEhiuBuildConfigCommandsID.Value = oInput.EhiuBuildConfigCommandsID;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildConfigCommandsID);
                }

                if (oInput.Action == "insert")
                {
                    SqlParameter sqlParamEhiuBuildConfigCommandsID = new SqlParameter("@EhiuBuildConfigCommandsID", SqlDbType.Int);
                    sqlParamEhiuBuildConfigCommandsID.Direction = ParameterDirection.Output;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildConfigCommandsID);
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
                    oResult.Result = (int)sqlInsert.Parameters["@EhiuBuildConfigCommandsID"].Value;
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
