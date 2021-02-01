using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoBuildConfigCrud
{

    public class tInput
    {
        public string Action { get; set; } = null;
        public string? UserName { get; set; }
        public int? EhiuBuildConfigID { get; set; }
        public bool? DefaultConfig { get; set; }
        public string? ConfigName { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;


    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

    }

    public class CloudEcoBuildConfigCrud
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
                    if (oInput.EhiuBuildConfigID == null)
                    {
                        context.Logger.LogLine("No EhiuBuildConfigID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuBuildConfigID supplied";

                        return oResult;
                    }



                };


                if (oInput.Action == "delete")
                {


                    if (ecoCommon.CodeExists(oInput.EhiuBuildConfigID, "EhiuBuild", "EhiuBuildConfigID", context, ref oSqlConnection) == false)
                    {
                        context.Logger.LogLine("EhiuBuildConfigID Used " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "This code has been used, retire the code instead";

                        return oResult;
                    }


                };

                if (oInput.Action == "insert")
                {
                    if (oInput.DefaultConfig == null)
                    {
                        context.Logger.LogLine("No DefaultConfig " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No DefaultConfig supplied";

                        return oResult;
                    }

                    if (oInput.ConfigName == null)
                    {
                        context.Logger.LogLine("No ConfigName " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No ConfigName supplied";

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

                    if (oInput.DefaultConfig != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "DefaultConfig";
                        strQueryParams = strQueryParams + "@DefaultConfig";
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


                    strQuery = "Insert Into EhiuBuildConfig ( " + strQuery + ") Values " +
                                "( " + strQueryParams + ") " +
                                "SET @EhiuBuildConfigID = Scope_Identity()";




                }




                if (oInput.Action == "update")
                {
                    strQuery = "";
                    strQueryParams = "";



                    if (oInput.DefaultConfig != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "DefaultConfig = @DefaultConfig ";
                    }

                    if (oInput.ConfigName != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "ConfigName = @ConfigName ";
                    }

 

                    if (oInput.DateCreated != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "DateCreated = @DateCreated ";
                    }

                    if (oInput.UserName  != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "CreatedBy = @CreatedBy ";
                    }




                    strQuery = "Update EhiuBuildConfig Set " + strQuery + " Where " +
                                " EhiuBuildConfigID = @EhiuBuildConfigID ";

                }

                if (oInput.Action == "delete") // Build params
                {
                    strQuery = "Delete From  EhiuBuildConfig  Where " +
                              " EhiuBuildConfigID = @EhiuBuildConfigID ";
                }



                SqlCommand sqlInsert = new SqlCommand(strQuery, oSqlConnection);



                if (oInput.Action == "insert" || oInput.Action == "update") // Build params
                {

                    if (oInput.DefaultConfig != null)
                    {
                        SqlParameter sqlParamDefaultConfig = new SqlParameter("@DefaultConfig", SqlDbType.Bit);
                        sqlParamDefaultConfig.Value = oInput.DefaultConfig;
                        sqlInsert.Parameters.Add(sqlParamDefaultConfig);
                    }

                    if (oInput.ConfigName != null)
                    {
                        SqlParameter sqlParamConfigName = new SqlParameter("@ConfigName", SqlDbType.NVarChar );
                        sqlParamConfigName.Value = oInput.ConfigName;
                        sqlInsert.Parameters.Add(sqlParamConfigName);
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
                    SqlParameter sqlParamEhiuBuildConfigID = new SqlParameter("@EhiuBuildConfigID", SqlDbType.Int);
                    sqlParamEhiuBuildConfigID.Value = oInput.EhiuBuildConfigID;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildConfigID);
                }

                if (oInput.Action == "insert")
                {
                    SqlParameter sqlParamEhiuBuildConfigID = new SqlParameter("@EhiuBuildConfigID", SqlDbType.Int);
                    sqlParamEhiuBuildConfigID.Direction = ParameterDirection.Output;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildConfigID);
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
                    oResult.Result = (int)sqlInsert.Parameters["@EhiuBuildConfigID"].Value;
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
