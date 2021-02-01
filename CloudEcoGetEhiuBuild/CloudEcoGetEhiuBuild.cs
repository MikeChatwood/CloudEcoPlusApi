using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoGetEhiuBuild
{
    public class tInput
    {
        public int? EhiuID { get; set; } = null;
    }

    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";

        public List<tEhiuBuild> EhiuBuilds { get; set; } = new List<tEhiuBuild>();

        public class tEhiuBuild
        {

            public int? EhiuBuildID { get; set; }
            public int? EhiuID { get; set; }
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
            public decimal? NumericResponse { get; set; }
            public DateTime? DateCreated { get; set; }
            public string? CreatedBy { get; set; }


        };
    }
    public class CloudEcoGetEhiuBuild
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

                strQuery = "SELECT EhiuBuildID, EhiuID, EhiuBuildConfigID, ConfigType, Prompt, TestTag, TextResponse, OptionMultiChoice, OptionOne, " +
                           " OptionTwo, OptionThree, OptionOneResponse, OptionTwoResponse, OptionThreeResponse, " +
                           " NumericResponse, DateCreated, CreatedBy " +
                           " FROM EhiuBuild " +
                           " WHERE (EhiuID = @EhiuID)";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamEhiuID = new SqlParameter("@EhiuID", SqlDbType.NVarChar);
                sqlParamEhiuID.Value = oInput.EhiuID;
                daCheck.SelectCommand.Parameters.Add(sqlParamEhiuID);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {
                    tResult.tEhiuBuild oEhiuBuild = new tResult.tEhiuBuild();

                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuBuildID"] != DBNull.Value)
                    {
                        oEhiuBuild.EhiuBuildID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuBuildID"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuID"] != DBNull.Value)
                    {
                        oEhiuBuild.EhiuID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuID"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigID"] != DBNull.Value)
                    {
                        oEhiuBuild.EhiuBuildConfigID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigID"];
                    }



                    if (dsCheck.Tables[0].Rows[intIdx]["ConfigType"] != DBNull.Value)
                    {
                        oEhiuBuild.ConfigType = (string)dsCheck.Tables[0].Rows[intIdx]["ConfigType"];
                    }



                    if (dsCheck.Tables[0].Rows[intIdx]["Prompt"] != DBNull.Value)
                    {
                        oEhiuBuild.Prompt = (string)dsCheck.Tables[0].Rows[intIdx]["Prompt"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["TestTag"] != DBNull.Value)
                    {
                        oEhiuBuild.TestTag = (string)dsCheck.Tables[0].Rows[intIdx]["TestTag"];
                    }



                    if (dsCheck.Tables[0].Rows[intIdx]["TextResponse"] != DBNull.Value)
                    {
                        oEhiuBuild.TextResponse = (string)dsCheck.Tables[0].Rows[intIdx]["TextResponse"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionMultiChoice"] != DBNull.Value)
                    {
                        oEhiuBuild.OptionMultiChoice = (bool)dsCheck.Tables[0].Rows[intIdx]["OptionMultiChoice"];
                    }



                    if (dsCheck.Tables[0].Rows[intIdx]["OptionOne"] != DBNull.Value)
                    {
                        oEhiuBuild.OptionOne = (string)dsCheck.Tables[0].Rows[intIdx]["OptionOne"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionTwo"] != DBNull.Value)
                    {
                        oEhiuBuild.OptionTwo = (string)dsCheck.Tables[0].Rows[intIdx]["OptionTwo"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionThree"] != DBNull.Value)
                    {
                        oEhiuBuild.OptionThree = (string)dsCheck.Tables[0].Rows[intIdx]["OptionThree"];
                    }



                    if (dsCheck.Tables[0].Rows[intIdx]["OptionOneResponse"] != DBNull.Value)
                    {
                        oEhiuBuild.OptionOneResponse = (string)dsCheck.Tables[0].Rows[intIdx]["OptionOneResponse"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionTwoResponse"] != DBNull.Value)
                    {
                        oEhiuBuild.OptionTwoResponse = (string)dsCheck.Tables[0].Rows[intIdx]["OptionTwoResponse"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionThreeResponse"] != DBNull.Value)
                    {
                        oEhiuBuild.OptionThreeResponse = (string)dsCheck.Tables[0].Rows[intIdx]["OptionThreeResponse"];
                    }



                    if (dsCheck.Tables[0].Rows[intIdx]["NumericResponse"] != DBNull.Value)
                    {
                        oEhiuBuild.NumericResponse = (Decimal)dsCheck.Tables[0].Rows[intIdx]["NumericResponse"];
                    }



                    if (dsCheck.Tables[0].Rows[intIdx]["DateCreated"] != DBNull.Value)
                    {
                        oEhiuBuild.DateCreated = (DateTime)dsCheck.Tables[0].Rows[intIdx]["DateCreated"];
                    }



                    if (dsCheck.Tables[0].Rows[intIdx]["CreatedBy"] != DBNull.Value)
                    {
                        oEhiuBuild.CreatedBy = (string)dsCheck.Tables[0].Rows[intIdx]["CreatedBy"];
                    }



                    oResult.EhiuBuilds.Add(oEhiuBuild);
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
