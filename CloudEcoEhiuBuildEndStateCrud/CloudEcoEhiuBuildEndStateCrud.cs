using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoEhiuBuildEndStateCrud
{

    public class tInput
    {
        public string Action { get; set; } = null;
        public string? UserName { get; set; }
        public int? EhiuBuildEndStateID { get; set; }
        public int? EhiuBuildID { get; set; }
        public int? EhiuBuildConfigEndStateID { get; set; }
        public bool? Selected { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;


    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

    }
    public class CloudEcoEhiuBuildEndStateCrud
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
                    if (oInput.EhiuBuildEndStateID == null)
                    {
                        context.Logger.LogLine("No EhiuBuildEndStateID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuBuildEndStateID supplied";

                        return oResult;
                    }
 



                };

                if (oInput.EhiuBuildConfigEndStateID != null)
                {
                    if (ecoCommon.CodeExists(oInput.EhiuBuildID, "EhiuBuild", "EhiuBuildID", context, ref oSqlConnection) == false)
                    {
                        context.Logger.LogLine("EhiuBuildID does not exist " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "EhiuBuildID does not exist";

                        return oResult;
                    }

                }

                if (oInput.EhiuBuildConfigEndStateID != null)
                {
                    if (ecoCommon.CodeExists(oInput.EhiuBuildConfigEndStateID, "EhiuBuildConfigEndState", "EhiuBuildConfigEndStateID", context, ref oSqlConnection) == false)
                    {
                        context.Logger.LogLine("EhiuBuildConfigEndStateID does not exist " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "EhiuBuildConfigEndStateID does not exist";

                        return oResult;
                    }

                }


                if (oInput.Action == "insert" || oInput.Action == "update")
                {

                    if (oInput.EhiuBuildID == null)
                    {
                        context.Logger.LogLine("No EhiuBuildID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuBuildID supplied";

                        return oResult;
                    }


                    if (oInput.EhiuBuildConfigEndStateID == null)
                    {
                        context.Logger.LogLine("No EhiuBuildConfigEndStateID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuBuildConfigEndStateID supplied";

                        return oResult;
                    }

                    if (oInput.Selected == null)
                    {
                        context.Logger.LogLine("No Selected " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No Selected supplied";

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

                   if (oInput.EhiuBuildConfigEndStateID != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "EhiuBuildConfigEndStateID";
                        strQueryParams = strQueryParams + "@EhiuBuildConfigEndStateID";
                    }


                   if (oInput.Selected != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "Selected";
                        strQueryParams = strQueryParams + "@Selected";
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


                    strQuery = "Insert Into EhiuBuildEndState ( " + strQuery + ") Values " +
                                "( " + strQueryParams + ") " +
                                "SET @EhiuBuildEndStateID = Scope_Identity()";




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

 
                    if (oInput.EhiuBuildConfigEndStateID != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "EhiuBuildConfigEndStateID = @EhiuBuildConfigEndStateID ";
                    }


                    if (oInput.Selected != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "Selected = @Selected ";
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


                    strQuery = "Update EhiuBuildEndState Set " + strQuery + " Where " +
                                " EhiuBuildEndStateID = @EhiuBuildEndStateID ";

                }

                if (oInput.Action == "delete") // Build params
                {
                    strQuery = "Delete From  EhiuBuildEndState  Where " +
                               " EhiuBuildEndStateID = @EhiuBuildEndStateID ";
                }



                SqlCommand sqlInsert = new SqlCommand(strQuery, oSqlConnection);



                if (oInput.Action == "insert" || oInput.Action == "update") // Build params
                {


                    if (oInput.EhiuBuildID != null)
                    {
                        SqlParameter sqlParamEhiuBuildID = new SqlParameter("@EhiuBuildID", SqlDbType.Int);
                        sqlParamEhiuBuildID.Value = oInput.EhiuBuildID;
                        sqlInsert.Parameters.Add(sqlParamEhiuBuildID);
                    }


                    if (oInput.EhiuBuildConfigEndStateID != null)
                    {
                        SqlParameter sqlParamEhiuBuildConfigEndStateID = new SqlParameter("@EhiuBuildConfigEndStateID", SqlDbType.Int);
                        sqlParamEhiuBuildConfigEndStateID.Value = oInput.EhiuBuildConfigEndStateID;
                        sqlInsert.Parameters.Add(sqlParamEhiuBuildConfigEndStateID);
                    }


                    if (oInput.Selected != null)
                    {
                        SqlParameter sqlParamSelected = new SqlParameter("@Selected", SqlDbType.Bit);
                        sqlParamSelected.Value = oInput.Selected;
                        sqlInsert.Parameters.Add(sqlParamSelected);
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
                    SqlParameter sqlParamEhiuBuildEndStateID = new SqlParameter("@EhiuBuildEndStateID", SqlDbType.Int);
                    sqlParamEhiuBuildEndStateID.Value = oInput.EhiuBuildEndStateID;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildEndStateID);
                }

                if (oInput.Action == "insert")
                {
                    SqlParameter sqlParamEhiuBuildEndStateID = new SqlParameter("@EhiuBuildEndStateID", SqlDbType.Int);
                    sqlParamEhiuBuildEndStateID.Direction = ParameterDirection.Output;
                    sqlInsert.Parameters.Add(sqlParamEhiuBuildEndStateID);
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
                    oResult.Result = (int)sqlInsert.Parameters["@EhiuBuildEndStateID"].Value;
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
