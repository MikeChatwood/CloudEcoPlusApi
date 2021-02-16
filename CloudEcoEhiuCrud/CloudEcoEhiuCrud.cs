using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoEhiuCrud
{

    public class tInput
    {
        public string Action { get; set; } = null;
        public string? UserName { get; set; }
        public int? EhiuID { get; set; }
        public string? SerialNumber { get; set; }
        public string? IMEI { get; set; }
        public bool? TestsPassed { get; set; }
        public DateTime? DateCreated { get; set; }


    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

    }


    public class CloudEcoEhiuCrud
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

              

                if ("insertupdatedelete".IndexOf(oInput.Action) == -1)
                {
                    oResult.Ok = false;
                    oResult.Info = "Action needs to be either INSERT DELETE or UPDATE";

                    return oResult;

                };

                if (oInput.Action == "update" || oInput.Action == "delete")
                {
                    if (oInput.EhiuID == null)
                    {
                        context.Logger.LogLine("No EhiuID " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No EhiuID supplied";

                        return oResult;
                    }

                                    };

                if (oInput.Action == "insert")
                {
                    if (oInput.SerialNumber == null)
                    {
                        context.Logger.LogLine("No SerialNumber " + oInput.ToString());

                        oResult.Ok = false;
                        oResult.Info = "No SerialNumber supplied";

                        return oResult;
                    }

    


                }



                // Validated now, 

                // Build the sql statements

                if (oInput.Action == "insert")
                {
                    strQuery = "";
                    strQueryParams = "";


                    if (oInput.SerialNumber != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "SerialNumber";
                        strQueryParams = strQueryParams + "@SerialNumber";
                    }


                    if (oInput.IMEI != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "IMEI";
                        strQueryParams = strQueryParams + "@IMEI";
                    }


                    if (oInput.TestsPassed != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                            strQueryParams = strQueryParams + ", ";
                        };
                        strQuery = strQuery + "TestsPassed";
                        strQueryParams = strQueryParams + "@TestsPassed";
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




                    strQuery = "Insert Into Ehiu ( " + strQuery + ") Values " +
                                "( " + strQueryParams + ") " +
                                "SET @EhiuID = Scope_Identity()";




                }




                if (oInput.Action == "update")
                {
                    strQuery = "";
                    strQueryParams = "";


                    if (oInput.SerialNumber != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "SerialNumber = @SerialNumber ";
                    }


                    if (oInput.IMEI != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "IMEI = @IMEI ";
                    }



                    if (oInput.TestsPassed != null)
                    {
                        if (strQuery != "")
                        {
                            strQuery = strQuery + ", ";
                        };
                        strQuery = strQuery + "TestsPassed = @TestsPassed ";
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




                    strQuery = "Update Ehiu Set " + strQuery + " Where " +
                                " EhiuID = @EhiuID ";

                }

                if (oInput.Action == "delete") // Build params
                {
                    strQuery = "Delete From  Ehiu  Where " +
                              " EhiuID = @EhiuID ";
                }



                SqlCommand sqlInsert = new SqlCommand(strQuery, oSqlConnection);



                if (oInput.Action == "insert" || oInput.Action == "update") // Build params
                {



                    if (oInput.SerialNumber != null)
                    {
                        SqlParameter sqlParamSerialNumber = new SqlParameter("@SerialNumber", SqlDbType.NVarChar);
                        sqlParamSerialNumber.Value = oInput.SerialNumber;
                        sqlInsert.Parameters.Add(sqlParamSerialNumber);
                    }



                    if (oInput.IMEI != null)
                    {
                        SqlParameter sqlParamIMEI = new SqlParameter("@IMEI", SqlDbType.NVarChar);
                        sqlParamIMEI.Value = oInput.IMEI;
                        sqlInsert.Parameters.Add(sqlParamIMEI);
                    }



                    if (oInput.TestsPassed != null)
                    {
                        SqlParameter sqlParamTestsPassed = new SqlParameter("@TestsPassed", SqlDbType.Bit);
                        sqlParamTestsPassed.Value = oInput.TestsPassed;
                        sqlInsert.Parameters.Add(sqlParamTestsPassed);
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
                    SqlParameter sqlParamEhiuID = new SqlParameter("@EhiuID", SqlDbType.Int);
                    sqlParamEhiuID.Value = oInput.EhiuID;
                    sqlInsert.Parameters.Add(sqlParamEhiuID);
                }

                if (oInput.Action == "insert")
                {
                    SqlParameter sqlParamEhiuID = new SqlParameter("@EhiuID", SqlDbType.Int);
                    sqlParamEhiuID.Direction = ParameterDirection.Output;
                    sqlInsert.Parameters.Add(sqlParamEhiuID);
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
                    oResult.Result = (int)sqlInsert.Parameters["@EhiuID"].Value;
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
