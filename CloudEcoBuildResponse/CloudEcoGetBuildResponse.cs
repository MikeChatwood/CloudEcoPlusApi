using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoGetBuildResponse
{
    public class tInput
    {
        public string SerialNumber { get; set; } = null;
    }

    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";

        public List<tResponse> Responses { get; set; } = new List<tResponse>();

        public class tResponse
        {

            public int? DisplaySequence { get; set; }
            public int? EhiuID { get; set; }
            public string? SerialNumber { get; set; }
            public string? IMEI { get; set; }
            public bool? TestsPassed { get; set; }
            public DateTime? DateCreated { get; set; }
            public string? CreatedBy { get; set; }
            public int? EhiuBuildID { get; set; }
            public int? EhiuBuildConfigID { get; set; }
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
            public Decimal? NumericResponse { get; set; }


        };
    }
    public class CloudEcoGetBuildResponse
    {

        public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {

            string strQuery = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();

            SqlConnection oSqlConnection = null;
            tResult oResult = new tResult();
            int intIdx;

            try
            {

                context.Logger.LogLine("FunctionHandler 1 ");

                try
                {
                    oSqlConnection = new SqlConnection(ecoCommon.GetSecret("CloudEcoPlus", context)); oSqlConnection.Open();
                    context.Logger.LogLine("FunctionHandler 2");
                }
                catch (Exception ex)
                {
                    context.Logger.LogLine("WriteRecord Ex  1" + ex.Message);
                }

                try
                {
                    oSqlConnection.Open();
                }
                catch (Exception)
                {
                }

                strQuery = " SELECT EhiuBuildConfig.DisplaySequence, Ehiu.EhiuID, Ehiu.SerialNumber, Ehiu.IMEI, Ehiu.TestsPassed, Ehiu.DateCreated, " +
                            " Ehiu.CreatedBy, EhiuBuild.EhiuBuildID, EhiuBuild.EhiuBuildConfigID, EhiuBuild.ConfigType,  " +
                            " EhiuBuild.Prompt, EhiuBuild.TestTag, EhiuBuild.TextResponse, EhiuBuild.OptionMultiChoice, EhiuBuild.OptionOne,  " +
                            " EhiuBuild.OptionTwo, EhiuBuild.OptionThree, EhiuBuild.OptionOneResponse, EhiuBuild.OptionTwoResponse,  " +
                            " EhiuBuild.OptionThreeResponse, EhiuBuild.NumericResponse " +
                            " FROM Ehiu INNER JOIN " +
                            " EhiuBuild ON Ehiu.EhiuID = EhiuBuild.EhiuID INNER JOIN " +
                            " EhiuBuildConfig ON EhiuBuild.EhiuBuildConfigID = EhiuBuildConfig.EhiuBuildConfigID " +
                            " WHERE (Ehiu.SerialNumber = @SerialNumber)";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamSerialNumber = new SqlParameter("@SerialNumber", SqlDbType.NVarChar);
                sqlParamSerialNumber.Value = oInput.SerialNumber;
                daCheck.SelectCommand.Parameters.Add(sqlParamSerialNumber);
                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {
                    tResult.tResponse oConfig = new tResult.tResponse();

                    if (dsCheck.Tables[0].Rows[intIdx]["DisplaySequence"] != DBNull.Value)
                    {
                        oConfig.DisplaySequence = (int)dsCheck.Tables[0].Rows[intIdx]["DisplaySequence"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuID"] != DBNull.Value)
                    {
                        oConfig.EhiuID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuID"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["SerialNumber"] != DBNull.Value)
                    {
                        oConfig.SerialNumber = (string)dsCheck.Tables[0].Rows[intIdx]["SerialNumber"];
                    }



                    if (dsCheck.Tables[0].Rows[intIdx]["TestsPassed"] != DBNull.Value)
                    {
                        oConfig.TestsPassed = (bool)dsCheck.Tables[0].Rows[intIdx]["TestsPassed"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["DateCreated"] != DBNull.Value)
                    {
                        oConfig.DateCreated = (DateTime)dsCheck.Tables[0].Rows[intIdx]["DateCreated"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["CreatedBy"] != DBNull.Value)
                    {
                        oConfig.CreatedBy = (string)dsCheck.Tables[0].Rows[intIdx]["CreatedBy"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuBuildID"] != DBNull.Value)
                    {
                        oConfig.EhiuBuildID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuBuildID"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigID"] != DBNull.Value)
                    {
                        oConfig.EhiuBuildConfigID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigID"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["ConfigType"] != DBNull.Value)
                    {
                        oConfig.ConfigType = (string)dsCheck.Tables[0].Rows[intIdx]["ConfigType"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["Prompt"] != DBNull.Value)
                    {
                        oConfig.Prompt = (string)dsCheck.Tables[0].Rows[intIdx]["Prompt"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["TestTag"] != DBNull.Value)
                    {
                        oConfig.TestTag = (string)dsCheck.Tables[0].Rows[intIdx]["TestTag"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["TextResponse"] != DBNull.Value)
                    {
                        oConfig.TextResponse = (string)dsCheck.Tables[0].Rows[intIdx]["TextResponse"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionMultiChoice"] != DBNull.Value)
                    {
                        oConfig.OptionMultiChoice = (bool)dsCheck.Tables[0].Rows[intIdx]["OptionMultiChoice"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionOne"] != DBNull.Value)
                    {
                        oConfig.OptionOne = (string)dsCheck.Tables[0].Rows[intIdx]["OptionOne"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionTwo"] != DBNull.Value)
                    {
                        oConfig.OptionTwo = (string)dsCheck.Tables[0].Rows[intIdx]["OptionTwo"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionThree"] != DBNull.Value)
                    {
                        oConfig.OptionThree = (string)dsCheck.Tables[0].Rows[intIdx]["OptionThree"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionOneResponse"] != DBNull.Value)
                    {
                        oConfig.OptionOneResponse = (string)dsCheck.Tables[0].Rows[intIdx]["OptionOneResponse"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionTwoResponse"] != DBNull.Value)
                    {
                        oConfig.OptionTwoResponse = (string)dsCheck.Tables[0].Rows[intIdx]["OptionTwoResponse"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionThreeResponse"] != DBNull.Value)
                    {
                        oConfig.OptionThreeResponse = (string)dsCheck.Tables[0].Rows[intIdx]["OptionThreeResponse"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["NumericResponse"] != DBNull.Value)
                    {
                        oConfig.NumericResponse = (Decimal)dsCheck.Tables[0].Rows[intIdx]["NumericResponse"];
                    }




                    oResult.Responses.Add(oConfig);
                }


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
