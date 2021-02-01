using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoGetLiveBuildConfigs
{


    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";
        public int EhiuBuildConfigID { get; set; } = -1;
        public string ConfigName { get; set; } = "";

        public List<tBuildConfig> BuildConfigs { get; set; } = new List<tBuildConfig>();
        public List<tBuildCommand> BuildCommands { get; set; } = new List<tBuildCommand>();

        public List<tEhiuBuildConfigEndState> EndStates { get; set; } = new List<tEhiuBuildConfigEndState>();




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


        public class tEhiuBuildConfigEndState
        {
            public int? EhiuBuildConfigEndStateID { get; set; }
            public int? EhiuBuildConfigID { get; set; }
            public string? Prompt { get; set; }


        }
    }
    public class CloudEcoGetLiveBuildConfigs
    {

        public tResult FunctionHandler(ILambdaContext context)
        {

            string strQuery = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();
            DataSet dsCommands;
            DataSet dsEndStates;

            SqlConnection oSqlConnection = null;
            tResult oResult = new tResult();
            int intIdx;

            try
            {

                context.Logger.LogLine("FunctionHandler 1706 ");

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




                strQuery = "SELECT TOP 1 ConfigName, EhiuBuildConfigID " +
                            " FROM  EhiuBuildConfig " +
                            " WHERE (DefaultConfig = 1)";




                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {

                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigID"] != DBNull.Value)
                    {
                        oResult.EhiuBuildConfigID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigID"];
                    }


                    if (dsCheck.Tables[0].Rows[intIdx]["ConfigName"] != DBNull.Value)
                    {
                        oResult.ConfigName = (string)dsCheck.Tables[0].Rows[intIdx]["ConfigName"];
                    }





                }

                dsCommands = getBuildConfigDetail(oResult.EhiuBuildConfigID, context, ref oSqlConnection);
                for (intIdx = 0; intIdx <= dsCommands.Tables[0].Rows.Count - 1; intIdx++)
                {
                    tResult.tBuildConfig oBuildConfig = new tResult.tBuildConfig();

                    if (dsCommands.Tables[0].Rows[intIdx]["EhiuBuildConfigDetailID"] != DBNull.Value)
                    {
                        oBuildConfig.EhiuBuildConfigDetailID = (int)dsCommands.Tables[0].Rows[intIdx]["EhiuBuildConfigDetailID"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["EhiuBuildConfigID"] != DBNull.Value)
                    {
                        oBuildConfig.EhiuBuildConfigID = (int)dsCommands.Tables[0].Rows[intIdx]["EhiuBuildConfigID"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["DisplaySequence"] != DBNull.Value)
                    {
                        oBuildConfig.DisplaySequence = (int)dsCommands.Tables[0].Rows[intIdx]["DisplaySequence"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["Mandatory"] != DBNull.Value)
                    {
                        oBuildConfig.Mandatory = (bool)dsCommands.Tables[0].Rows[intIdx]["Mandatory"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["ConfigType"] != DBNull.Value)
                    {
                        oBuildConfig.ConfigType = (string)dsCommands.Tables[0].Rows[intIdx]["ConfigType"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["Prompt"] != DBNull.Value)
                    {
                        oBuildConfig.Prompt = (string)dsCommands.Tables[0].Rows[intIdx]["Prompt"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["Tag"] != DBNull.Value)
                    {
                        oBuildConfig.Tag = (string)dsCommands.Tables[0].Rows[intIdx]["Tag"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["TextDefault"] != DBNull.Value)
                    {
                        oBuildConfig.TextDefault = (string)dsCommands.Tables[0].Rows[intIdx]["TextDefault"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["TextMaxLength"] != DBNull.Value)
                    {
                        oBuildConfig.TextMaxLength = (int)dsCommands.Tables[0].Rows[intIdx]["TextMaxLength"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["OptionOne"] != DBNull.Value)
                    {
                        oBuildConfig.OptionOne = (string)dsCommands.Tables[0].Rows[intIdx]["OptionOne"];
                    }


                    if (dsCommands.Tables[0].Rows[intIdx]["OptionTwo"] != DBNull.Value)
                    {
                        oBuildConfig.OptionTwo = (string)dsCommands.Tables[0].Rows[intIdx]["OptionTwo"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["OptionThree"] != DBNull.Value)
                    {
                        oBuildConfig.OptionThree = (string)dsCommands.Tables[0].Rows[intIdx]["OptionThree"];
                    }


                    if (dsCommands.Tables[0].Rows[intIdx]["OptionOneTag"] != DBNull.Value)
                    {
                        oBuildConfig.OptionOneTag = (string)dsCommands.Tables[0].Rows[intIdx]["OptionOneTag"];
                    }


                    if (dsCommands.Tables[0].Rows[intIdx]["OptionTwoTag"] != DBNull.Value)
                    {
                        oBuildConfig.OptionTwoTag = (string)dsCommands.Tables[0].Rows[intIdx]["OptionTwoTag"];
                    }


                    if (dsCommands.Tables[0].Rows[intIdx]["OptionThreeTag"] != DBNull.Value)
                    {
                        oBuildConfig.OptionThreeTag = (string)dsCommands.Tables[0].Rows[intIdx]["OptionThreeTag"];
                    }


                    if (dsCommands.Tables[0].Rows[intIdx]["OptionMultiChoice"] != DBNull.Value)
                    {
                        oBuildConfig.OptionMultiChoice = (bool)dsCommands.Tables[0].Rows[intIdx]["OptionMultiChoice"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["NumericMinimum"] != DBNull.Value)
                    {
                        oBuildConfig.NumericMinimum = (decimal)dsCommands.Tables[0].Rows[intIdx]["NumericMinimum"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["NumericMaximum"] != DBNull.Value)
                    {
                        oBuildConfig.NumericMaximum = (decimal)dsCommands.Tables[0].Rows[intIdx]["NumericMaximum"];
                    }


                    if (dsCommands.Tables[0].Rows[intIdx]["NumericDecimals"] != DBNull.Value)
                    {
                        oBuildConfig.NumericDecimals = (int)dsCommands.Tables[0].Rows[intIdx]["NumericDecimals"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["NumericDefault"] != DBNull.Value)
                    {
                        oBuildConfig.NumericDefault = (decimal)dsCommands.Tables[0].Rows[intIdx]["NumericDefault"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["ImageFileName"] != DBNull.Value)
                    {
                        oBuildConfig.ImageFileName = (string)dsCommands.Tables[0].Rows[intIdx]["ImageFileName"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["Retired"] != DBNull.Value)
                    {
                        oBuildConfig.Retired = (bool)dsCommands.Tables[0].Rows[intIdx]["Retired"];
                    }


                    if (dsCommands.Tables[0].Rows[intIdx]["DateCreated"] != DBNull.Value)
                    {
                        oBuildConfig.DateCreated = (DateTime)dsCommands.Tables[0].Rows[intIdx]["DateCreated"];
                    }

                    if (dsCommands.Tables[0].Rows[intIdx]["CreatedBy"] != DBNull.Value)
                    {
                        oBuildConfig.CreatedBy = (string)dsCommands.Tables[0].Rows[intIdx]["CreatedBy"];
                    }



                    oResult.BuildConfigs.Add(oBuildConfig);
                }


                dsEndStates = getEndStates(oResult.EhiuBuildConfigID, context, ref oSqlConnection);
                for (intIdx = 0; intIdx <= dsEndStates.Tables[0].Rows.Count - 1; intIdx++)
                {

                    tResult.tEhiuBuildConfigEndState oEhiuBuildConfigEndState = new tResult.tEhiuBuildConfigEndState();

                    if (dsEndStates.Tables[0].Rows[intIdx]["EhiuBuildConfigEndStateID"] != DBNull.Value)
                    {
                        oEhiuBuildConfigEndState.EhiuBuildConfigEndStateID = (int)dsEndStates.Tables[0].Rows[intIdx]["EhiuBuildConfigEndStateID"];
                    }

                    if (dsEndStates.Tables[0].Rows[intIdx]["EhiuBuildConfigID"] != DBNull.Value)
                    {
                        oEhiuBuildConfigEndState.EhiuBuildConfigID = (int)dsEndStates.Tables[0].Rows[intIdx]["EhiuBuildConfigID"];
                    }

                    if (dsEndStates.Tables[0].Rows[intIdx]["Prompt"] != DBNull.Value)
                    {
                        oEhiuBuildConfigEndState.Prompt = (string)dsEndStates.Tables[0].Rows[intIdx]["Prompt"];
                    }


                    oResult.EndStates.Add(oEhiuBuildConfigEndState);

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

        public DataSet getEndStates(int intEhiuBuildConfigID, ILambdaContext context, ref SqlConnection oSqlConnection)
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

                strQuery = "SELECT EhiuBuildConfigEndStateID, EhiuBuildConfigID, Prompt " +
                            " FROM EhiuBuildConfigEndState " +
                            " WHERE (Retired = 0) AND (EhiuBuildConfigID = @EhiuBuildConfigID) " +
                            " ORDER BY DisplaySequence";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamEhiuBuildConfigID = new SqlParameter("@EhiuBuildConfigID", SqlDbType.Int);
                sqlParamEhiuBuildConfigID.Value = intEhiuBuildConfigID;
                daCheck.SelectCommand.Parameters.Add(sqlParamEhiuBuildConfigID);

                daCheck.Fill(dsCheck);

            }
            catch (Exception)
            {
            }

            return dsCheck;
        }


        public DataSet getBuildConfigDetail(int intEhiuBuildConfigID, ILambdaContext context, ref SqlConnection oSqlConnection)
        {

            string strQuery = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();

            try
            {

                context.Logger.LogLine("getBuildConfigDetail 1 ");

                try
                {
                    oSqlConnection.Open();
                }
                catch (Exception)
                {
                }

                strQuery = "SELECT EhiuBuildConfigDetailID, EhiuBuildConfigID, DisplaySequence, Mandatory, ConfigType, Prompt, Tag, TextDefault, " +
                            " TextMaxLength, OptionOne, OptionTwo, OptionThree, OptionOneTag, OptionTwoTag, OptionThreeTag,  " +
                            " OptionMultiChoice, NumericMinimum, NumericMaximum, NumericDecimals, NumericDefault, " +
                            " ImageFileName, Retired, DateCreated, CreatedBy " +
                            " FROM EhiuBuildConfigDetail " +
                            " WHERE (EhiuBuildConfigID = @EhiuBuildConfigID) AND (Retired = 0) " +
                            " ORDER BY DisplaySequence";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamEhiuBuildConfigID = new SqlParameter("@EhiuBuildConfigID", SqlDbType.Int);
                sqlParamEhiuBuildConfigID.Value = intEhiuBuildConfigID;
                daCheck.SelectCommand.Parameters.Add(sqlParamEhiuBuildConfigID);

                daCheck.Fill(dsCheck);

            }
            catch (Exception)
            {
            }

            return dsCheck;
        }


    }
}
