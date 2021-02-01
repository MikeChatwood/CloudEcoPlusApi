using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoGetConfigEndState
{

    public class tInput
    {
        public int EhiuBuildConfigID { get; set; } = -1;
        public bool NonRetiredOnly { get; set; } = false;
    }
    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";

        public List<tEndState> EndStates { get; set; } = new List<tEndState>();

        public class tEndState
        {
            public int? EhiuBuildConfigEndStateID { get; set; }
            public int? EhiuBuildConfigID { get; set; }
            public int? DisplaySequence { get; set; }
            public bool? Retired { get; set; }
            public string? Prompt { get; set; }
            public DateTime? DateCreated { get; set; }
            public string? CreatedBy { get; set; }



        };
    }


    public class CloudEcoGetConfigEndState
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

                context.Logger.LogLine("FunctionHandler 1706 " + oInput.NonRetiredOnly.ToString());

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


                if (oInput.NonRetiredOnly == true)
                {
                    strQuery = "SELECT EhiuBuildConfigEndStateID, EhiuBuildConfigID, DisplaySequence, Retired, Prompt, DateCreated, CreatedBy " +
                                      " FROM  EhiuBuildConfigEndState " +
                                      " Where Retired = 0 And " +
                                      " EhiuBuildConfigID = @EhiuBuildConfigID " +
                                      " ORDER BY DisplaySequence, DateCreated";
                }
                else
                {
                    strQuery = "SELECT EhiuBuildConfigEndStateID, EhiuBuildConfigID, DisplaySequence, Retired, Prompt, DateCreated, CreatedBy " +
                                      " FROM  EhiuBuildConfigEndState " +
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

                    tResult.tEndState oEndState = new tResult.tEndState();

                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigEndStateID"] != DBNull.Value)
                    {
                        oEndState.EhiuBuildConfigEndStateID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigEndStateID"];
                    }

                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigID"] != DBNull.Value)
                    {
                        oEndState.EhiuBuildConfigID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuBuildConfigID"];
                    }

                    if (dsCheck.Tables[0].Rows[intIdx]["DisplaySequence"] != DBNull.Value)
                    {
                        oEndState.DisplaySequence = (int)dsCheck.Tables[0].Rows[intIdx]["DisplaySequence"];
                    }

                    if (dsCheck.Tables[0].Rows[intIdx]["Retired"] != DBNull.Value)
                    {
                        oEndState.Retired = (bool)dsCheck.Tables[0].Rows[intIdx]["Retired"];
                    }

                    if (dsCheck.Tables[0].Rows[intIdx]["Prompt"] != DBNull.Value)
                    {
                        oEndState.Prompt = (string)dsCheck.Tables[0].Rows[intIdx]["Prompt"];
                    }

                    if (dsCheck.Tables[0].Rows[intIdx]["DateCreated"] != DBNull.Value)
                    {
                        oEndState.DateCreated = (DateTime)dsCheck.Tables[0].Rows[intIdx]["DateCreated"];
                    }

                    if (dsCheck.Tables[0].Rows[intIdx]["CreatedBy"] != DBNull.Value)
                    {
                        oEndState.CreatedBy = (string)dsCheck.Tables[0].Rows[intIdx]["CreatedBy"];
                    }


                    oResult.EndStates.Add(oEndState);
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
