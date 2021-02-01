using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoEhiuSiteConfigDetailCrud
{
    public class tInput
    {
        public string Action { get; set; } = null;
        public string UserName { get; set; } = null;
        public int EhiuSiteConfigDetailID { get; set; } = -1;
        public int EhiuSiteConfigID { get; set; } = -1;


        public string CommandName { get; set; } = null;
        public bool? ToEco { get; set; } = null;
        public string JsonConfigItem { get; set; } = null;
        public string CompanionName { get; set; } = null;
        public string JsonCompanionItem { get; set; } = null;
        public int? MbusID { get; set; } = -1;
        public string UrlPath { get; set; } = null;


    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

    }


    public class CloudEcoEhiuSiteConfigDetailCrud
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
                    if (oInput.EhiuSiteConfigDetailID == -1)
                    {
                        context.Logger.LogLine("No EhiuSiteConfigDetailID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuSiteConfigDetailID supplied";

                        return oResult;
                    }


                };

                if (oInput.Action == "insert")
                {
                    if (oInput.EhiuSiteConfigID == -1)
                    {
                        context.Logger.LogLine("No EhiuSiteConfigID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuSiteConfigID supplied";

                        return oResult;
                    }



                    if (oInput.CommandName == null)
                    {
                        context.Logger.LogLine("No CommandName " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No CommandName supplied";

                        return oResult;
                    }

                    if (oInput.JsonConfigItem == null)
                    {
                        context.Logger.LogLine("No JsonConfigItem " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No JsonConfigItem supplied";

                        return oResult;
                    }

                };


                if (oInput.UserName == null)
                {
                    context.Logger.LogLine("No UserName " + oInput.ToString());

                    oResult.Ok = false;
                    oResult.Info = "No UserName supplied";

                    return oResult;
                }

                // Validated now, 

                // Build the sql statements

                if (oInput.Action == "insert")
                {
                    strQuery = "";
                    strQueryParams = "";


                    if (oInput.EhiuSiteConfigID != -1)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "EhiuSiteConfigID";
                        strQueryParams = strQueryParams + "@EhiuSiteConfigID";
                    }

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

                    if (oInput.MbusID != -1)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "MbusID";
                        strQueryParams = strQueryParams + "@MbusID";
                    }

                    if (oInput.UrlPath != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "UrlPath";
                        strQueryParams = strQueryParams + "@UrlPath";
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






                    strQuery = "Insert Into EhiuSiteConfigDetail ( " + strQuery + ") Values " +
                                "( " + strQueryParams + ") " +
                                "SET @EhiuSiteConfigDetailID = Scope_Identity()";




                }




                if (oInput.Action == "update")
                {
                    strQuery = "";
                    strQueryParams = "";

                    if (oInput.EhiuSiteConfigID != -1)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "EhiuSiteConfigID = @EhiuSiteConfigID ";
                    }

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



                    if (oInput.UserName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "CreatedBy = @CreatedBy ";
                    }


                    if (oInput.MbusID != -1)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "MbusID = @MbusID ";
                    }

                    if (oInput.UrlPath != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "UrlPath = @UrlPath ";
                    }


                    strQuery = "Update EhiuSiteConfigDetail Set " + strQuery + " Where " +
                                " EhiuSiteConfigDetailID = @EhiuSiteConfigDetailID ";

                }

                if (oInput.Action == "delete") // Build params
                {
                    strQuery = "Delete From  EhiuSiteConfigDetail  Where " +
                              " EhiuSiteConfigDetailID = @EhiuSiteConfigDetailID ";
                }



                SqlCommand sqlInsert = new SqlCommand(strQuery, oSqlConnection);



                if (oInput.Action == "insert" || oInput.Action == "update") // Build params
                {

                    if (oInput.EhiuSiteConfigID != -1)
                    {
                        SqlParameter sqlParamEhiuSiteConfigID = new SqlParameter("@EhiuSiteConfigID", SqlDbType.Int);
                        sqlParamEhiuSiteConfigID.Value = oInput.EhiuSiteConfigID;
                        sqlInsert.Parameters.Add(sqlParamEhiuSiteConfigID);
                    }

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




                    if (oInput.UserName != null)
                    {
                        SqlParameter sqlParamCreatedBy = new SqlParameter("@CreatedBy", SqlDbType.NVarChar);
                        sqlParamCreatedBy.Value = oInput.UserName;
                        sqlInsert.Parameters.Add(sqlParamCreatedBy);

                    }

                    if (oInput.MbusID != -1)
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
                    SqlParameter sqlParamEhiuSiteConfigDetailID = new SqlParameter("@EhiuSiteConfigDetailID", SqlDbType.Int);
                    sqlParamEhiuSiteConfigDetailID.Value = oInput.EhiuSiteConfigDetailID;
                    sqlInsert.Parameters.Add(sqlParamEhiuSiteConfigDetailID);
                }

                if (oInput.Action == "insert")
                {
                    SqlParameter sqlParamEhiuSiteConfigDetailID = new SqlParameter("@EhiuSiteConfigDetailID", SqlDbType.Int);
                    sqlParamEhiuSiteConfigDetailID.Direction = ParameterDirection.Output;
                    sqlInsert.Parameters.Add(sqlParamEhiuSiteConfigDetailID);
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
                    oResult.Result = (int)sqlInsert.Parameters["@EhiuSiteConfigDetailID"].Value;
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
