using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoSiteConfigCrud
{
    public class tInput
    {
        public string Action { get; set; } = null;
        public string UserName { get; set; } = null;
        public int EhiuSiteConfigID { get; set; } = -1;
        public int SiteID { get; set; } = -1;
        public string Name { get; set; } = null;
        public string Descr { get; set; } = null;
        public bool? Retired { get; set; } = null;

    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

    }

    public class CloudEcoSiteConfigCrud
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
                    if (oInput.EhiuSiteConfigID == -1)
                    {
                        context.Logger.LogLine("No EhiuSiteConfigID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuSiteConfigID supplied";

                        return oResult;
                    }


                };

                if (oInput.Action == "insert")
                {
                    if (oInput.SiteID == -1)
                    {
                        context.Logger.LogLine("No SiteID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No SiteID supplied";

                        return oResult;
                    }

                    if (oInput.Name == null)
                    {
                        context.Logger.LogLine("No Name " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No Name supplied";

                        return oResult;
                    }

                    if (oInput.UserName == null)
                    {
                        context.Logger.LogLine("No UserName " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No UserName supplied";

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

                    if (oInput.Descr != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "Descr";
                        strQueryParams = strQueryParams + "@Descr";
                    }


                    if (oInput.Name != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "Name";
                        strQueryParams = strQueryParams + "@Name";
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

                    if (oInput.SiteID != -1)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "SiteID";
                        strQueryParams = strQueryParams + "@SiteID";
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



                   


                    strQuery = "Insert Into EhiuSiteConfig ( " + strQuery + ") Values " +
                                "( " + strQueryParams + ") " +
                                "SET @EhiuSiteConfigID = Scope_Identity()";




                }




                if (oInput.Action == "update")
                {
                    strQuery = "";
                    strQueryParams = "";

                    if (oInput.Descr != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "Descr = @Descr ";
                    }


                    if (oInput.Name != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "Name = @Name ";
                    }

                    if (oInput.Retired != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "Retired = @Retired ";
                    }

                    if (oInput.SiteID != -1)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "SiteID = @SiteID ";
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


                    strQuery = "Update EhiuSiteConfig Set " + strQuery + " Where " +
                                " EhiuSiteConfigID = @EhiuSiteConfigID ";

                }

                if (oInput.Action == "delete") // Build params
                {
                    strQuery = "Delete From  EhiuSiteConfig  Where " +
                              " EhiuSiteConfigID = @EhiuSiteConfigID ";
                }



                SqlCommand sqlInsert = new SqlCommand(strQuery, oSqlConnection);



                if (oInput.Action == "insert" || oInput.Action == "update") // Build params
                {
                    if (oInput.Descr != null)
                    {
                        SqlParameter sqlParamDescr = new SqlParameter("@Descr", SqlDbType.NVarChar);
                        sqlParamDescr.Value = oInput.Descr;
                        sqlInsert.Parameters.Add(sqlParamDescr);
                    }


                    if (oInput.Name != null)
                    {
                        SqlParameter sqlParamName = new SqlParameter("@Name", SqlDbType.NVarChar);
                        sqlParamName.Value = oInput.Name;
                        sqlInsert.Parameters.Add(sqlParamName);
                    }


                    if (oInput.Retired != null)
                    {
                        SqlParameter sqlParamRetired = new SqlParameter("@Retired", SqlDbType.Bit);
                        sqlParamRetired.Value = oInput.Retired;
                        sqlInsert.Parameters.Add(sqlParamRetired);
                    }



                    if (oInput.SiteID != -1)
                    {
                        SqlParameter sqlParamSiteID = new SqlParameter("@SiteID", SqlDbType.Int);
                        sqlParamSiteID.Value = oInput.SiteID;
                        sqlInsert.Parameters.Add(sqlParamSiteID);
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
                    SqlParameter sqlParamEhiuSiteConfigID = new SqlParameter("@EhiuSiteConfigID", SqlDbType.Int);
                    sqlParamEhiuSiteConfigID.Value = oInput.EhiuSiteConfigID;
                    sqlInsert.Parameters.Add(sqlParamEhiuSiteConfigID);
                }

                if (oInput.Action == "insert")
                {
                    SqlParameter sqlParamEhiuSiteConfigID = new SqlParameter("@EhiuSiteConfigID", SqlDbType.Int);
                    sqlParamEhiuSiteConfigID.Value = oInput.Retired;
                    sqlParamEhiuSiteConfigID.Direction = ParameterDirection.Output;
                    sqlInsert.Parameters.Add(sqlParamEhiuSiteConfigID);
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
                    oResult.Result = (int)sqlInsert.Parameters["@EhiuSiteConfigID"].Value;
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
