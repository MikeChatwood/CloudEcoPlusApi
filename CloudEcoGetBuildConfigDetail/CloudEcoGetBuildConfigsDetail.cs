using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoGetBuildConfigsDetail
{
  public class tInput
    {
        public int EhiuBuildConfigID { get; set; } = -1;
        public bool JustLive { get; set; } = false;
    }
    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";

        public List<tBuildConfig> BuildConfigs { get; set; } = new List<tBuildConfig>();
        public List<tBuildCommand> BuildCommands { get; set; } = new List<tBuildCommand>();




        public class tBuildCommand
        {
            public int? EhiuBuildConfigCommandsID { get; set; }
            public int? EhiuBuildConfigID { get; set; }
            public string? CommandName { get; set; }
            public bool? ToEco { get; set; }
            public string? JsonConfigItem { get; set; }
            public string? CompanionName { get; set; }
            public string? JsonCompanionItem { get; set; }
            public string? UrlPath { get; set; }
            public int? MbusID { get; set; }
            public DateTime? DateCreated { get; set; }
            public string? CreatedBy { get; set; }

        }



        public class tBuildConfig
        {
            public int? EhiuBuildConfigDetailID { get; set; }
            public int? EhiuBuildConfigID { get; set; }
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
            public string? CreatedBy { get; set; }
            public bool? Retired { get; set; }




        };
    }

  
    public class CloudEcoGetBuildConfigsDetail
    {

        public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {

            string strQuery = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();
            DataSet dsCommands;

            SqlConnection oSqlConnection = null;
            tResult oResult = new tResult();
            int intIdx;

            try
            {

                context.Logger.LogLine("FunctionHandler 1706 " + oInput.JustLive.ToString());

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



                if (oInput.JustLive == true)
                {
                    strQuery = "SELECT EhiuBuildConfigDetailID, EhiuBuildConfigID, DisplaySequence, Mandatory, ConfigType, Prompt, Tag, TextDefault, " +
                                      " TextMaxLength, OptionOne, OptionTwo, OptionThree, OptionOneTag, OptionTwoTag, OptionThreeTag, OptionMultiChoice, " +
                                      " NumericMinimum, NumericMaximum, NumericDecimals, NumericDefault, ImageFileName, DateCreated, CreatedBy ,Retired " +
                                      " FROM  EhiuBuildConfigDetail " +
                                      " Where Retired = 0 And " +
                                      " EhiuBuildConfigID = @EhiuBuildConfigID " +
                                      " ORDER BY DisplaySequence, DateCreated";
                }
                else
                {
                    strQuery = "SELECT EhiuBuildConfigDetailID, EhiuBuildConfigID, DisplaySequence, Mandatory, ConfigType, Prompt, Tag, TextDefault, " +
                                      " TextMaxLength, OptionOne, OptionTwo, OptionThree, OptionOneTag, OptionTwoTag, OptionThreeTag, OptionMultiChoice, " +
                                      " NumericMinimum, NumericMaximum, NumericDecimals, NumericDefault, ImageFileName, DateCreated, CreatedBy, Retired " +
                                      " FROM  EhiuBuildConfigDetail " +
                                      " Where EhiuBuildConfigID = @EhiuBuildConfigID " +
                                      " ORDER BY DisplaySequence, DateCreated";
                }


                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);


                SqlParameter sqlParamEhiuBuildConfigID = new SqlParameter("@EhiuBuildConfigID", SqlDbType.Int);
                sqlParamEhiuBuildConfigID.Value = oInput.EhiuBuildConfigID;
                daCheck.SelectCommand.Parameters.Add(sqlParamEhiuBuildConfigID);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {

                    tResult.tBuildConfig oConfig = new tResult.tBuildConfig();

                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigDetailID"] != DBNull.Value)
                    {
                        oConfig.EhiuBuildConfigDetailID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigDetailID"];
                    }



                    if (dsCheck.Tables[0].Rows[intIdx]["ConfigType"] != DBNull.Value)
                    {
                        oConfig.ConfigType = (string)dsCheck.Tables[0].Rows[intIdx]["ConfigType"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigID"] != DBNull.Value)
                    {
                        oConfig.EhiuBuildConfigID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigID"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["DisplaySequence"] != DBNull.Value)
                    {
                        oConfig.DisplaySequence = (int)dsCheck.Tables[0].Rows[intIdx]["DisplaySequence"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["Mandatory"] != DBNull.Value)
                    {
                        oConfig.Mandatory = (bool)dsCheck.Tables[0].Rows[intIdx]["Mandatory"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["Prompt"] != DBNull.Value)
                    {
                        oConfig.Prompt = (string)dsCheck.Tables[0].Rows[intIdx]["Prompt"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["Tag"] != DBNull.Value)
                    {
                        oConfig.Tag = (string)dsCheck.Tables[0].Rows[intIdx]["Tag"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["TextDefault"] != DBNull.Value)
                    {
                        oConfig.TextDefault = (string)dsCheck.Tables[0].Rows[intIdx]["TextDefault"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["TextMaxLength"] != DBNull.Value)
                    {
                        oConfig.TextMaxLength = (int)dsCheck.Tables[0].Rows[intIdx]["TextMaxLength"];
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


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionOneTag"] != DBNull.Value)
                    {
                        oConfig.OptionOneTag = (string)dsCheck.Tables[0].Rows[intIdx]["OptionOneTag"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionTwoTag"] != DBNull.Value)
                    {
                        oConfig.OptionTwoTag = (string)dsCheck.Tables[0].Rows[intIdx]["OptionTwoTag"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionThreeTag"] != DBNull.Value)
                    {
                        oConfig.OptionThreeTag = (string)dsCheck.Tables[0].Rows[intIdx]["OptionThreeTag"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["OptionMultiChoice"] != DBNull.Value)
                    {
                        oConfig.OptionMultiChoice = (bool)dsCheck.Tables[0].Rows[intIdx]["OptionMultiChoice"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["NumericMinimum"] != DBNull.Value)
                    {
                        oConfig.NumericMinimum = (decimal)dsCheck.Tables[0].Rows[intIdx]["NumericMinimum"];
                    }



                    if (dsCheck.Tables[0].Rows[intIdx]["NumericMaximum"] != DBNull.Value)
                    {
                        oConfig.NumericMaximum = (decimal)dsCheck.Tables[0].Rows[intIdx]["NumericMaximum"];
                    }



                    if (dsCheck.Tables[0].Rows[intIdx]["NumericDecimals"] != DBNull.Value)
                    {
                        oConfig.NumericDecimals = (int)dsCheck.Tables[0].Rows[intIdx]["NumericDecimals"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["NumericDefault"] != DBNull.Value)
                    {
                        oConfig.NumericDefault = (decimal)dsCheck.Tables[0].Rows[intIdx]["NumericDefault"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["ImageFileName"] != DBNull.Value)
                    {
                        oConfig.ImageFileName = (string)dsCheck.Tables[0].Rows[intIdx]["ImageFileName"];
                    }

                    if (dsCheck.Tables[0].Rows[intIdx]["Retired"] != DBNull.Value)
                    {
                        oConfig.Retired = (bool)dsCheck.Tables[0].Rows[intIdx]["Retired"];
                    }

                    oConfig.DateCreated = (DateTime)dsCheck.Tables[0].Rows[intIdx]["DateCreated"];
                    oConfig.CreatedBy = (string)dsCheck.Tables[0].Rows[intIdx]["CreatedBy"];

                    oResult.BuildConfigs.Add(oConfig);
                }



                dsCommands = getCommands(context, ref oSqlConnection);
                for (intIdx = 0; intIdx <= dsCommands.Tables[0].Rows.Count - 1; intIdx++)
                {

                    tResult.tBuildCommand oBuildCommand = new tResult.tBuildCommand();

                    if (dsCommands.Tables[0].Rows[intIdx]["CommandName"] != DBNull.Value)
                    {
                        oBuildCommand.CommandName = (string)dsCommands.Tables[0].Rows[intIdx]["CommandName"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["CompanionName"] != DBNull.Value)
                    {
                        oBuildCommand.CompanionName = (string)dsCommands.Tables[0].Rows[intIdx]["CompanionName"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["CreatedBy"] != DBNull.Value)
                    {
                        oBuildCommand.CreatedBy = (string)dsCommands.Tables[0].Rows[intIdx]["CreatedBy"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["EhiuBuildConfigCommandsID"] != DBNull.Value)
                    {
                        oBuildCommand.EhiuBuildConfigCommandsID = (int)dsCommands.Tables[0].Rows[intIdx]["EhiuBuildConfigCommandsID"];
                    }


                    if (dsCommands.Tables[0].Rows[intIdx]["JsonCompanionItem"] != DBNull.Value)
                    {
                        oBuildCommand.JsonCompanionItem = (string)dsCommands.Tables[0].Rows[intIdx]["JsonCompanionItem"];
                    }


                    if (dsCommands.Tables[0].Rows[intIdx]["JsonConfigItem"] != DBNull.Value)
                    {
                        oBuildCommand.JsonConfigItem = (string)dsCommands.Tables[0].Rows[intIdx]["JsonConfigItem"];
                    }


                    if (dsCommands.Tables[0].Rows[intIdx]["ToEco"] != DBNull.Value)
                    {
                        oBuildCommand.ToEco = (bool)dsCommands.Tables[0].Rows[intIdx]["ToEco"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["MbusID"] != DBNull.Value)
                    {
                        oBuildCommand.MbusID = (int)dsCommands.Tables[0].Rows[intIdx]["MbusID"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["UrlPath"] != DBNull.Value)
                    {
                        oBuildCommand.UrlPath = (string)dsCommands.Tables[0].Rows[intIdx]["UrlPath"];
                    }


                    oResult.BuildCommands.Add(oBuildCommand);

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



        public DataSet getCommands(ILambdaContext context, ref SqlConnection oSqlConnection)
        {

            string strQuery = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();

            try
            {

                context.Logger.LogLine("getCommands 1 ");

                try
                {
                    oSqlConnection.Open();
                }
                catch (Exception)
                {
                }

                strQuery = "SELECT EhiuBuildConfigCommandsID, CommandName, ToEco, JsonConfigItem, CompanionName, JsonCompanionItem, DateCreated, CreatedBy, MbusID, UrlPath " +
                            " FROM EhiuBuildConfigCommands Order By CommandName";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);
                daCheck.Fill(dsCheck);

            }
            catch (Exception)
            {
            }

            return dsCheck;
        }
    }
}
