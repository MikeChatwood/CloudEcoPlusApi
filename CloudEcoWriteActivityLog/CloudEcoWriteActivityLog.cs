using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoWriteActivityLog
{


    public class tInput
    {

        public string? UserName { get; set; }
        public List<tActivity> Activities { get; set; } = new List<tActivity>();

        public class tActivity
        {

            public string? SerialNumber { get; set; } = "";
            public string? EcoCommandSend { get; set; } = "";
            public string? EcoCommandReply { get; set; } = "";
            public string? CommandName { get; set; } = "";
            public bool? IsGet { get; set; } = false;
            public string? PostStatus { get; set; } = "";
            public string? PathElement { get; set; } = "";
            public DateTime? DateCreated { get; set; } = DateTime.Now;
            public string? CreatedBy { get; set; } = "";
        }


    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

    }

    public class CloudEcoWriteActivityLog
    {

        public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {
            SqlConnection oSqlConnection = null;
            tResult oResult = new tResult();
            int intIdx;

            try
            {

                try
                {
                    oSqlConnection = new SqlConnection(ecoCommon.GetSecret("CloudEcoPlus", context)); oSqlConnection.Open();
                    context.Logger.LogLine("FunctionHandler 2");
                }
                catch (Exception ex)
                {
                    context.Logger.LogLine("WriteRecord Ex  1" + ex.Message);
                }

                for (intIdx = 0; intIdx <= oInput.Activities.Count - 1; intIdx++)
                {

                    tInput.tActivity oActivity;

                    oActivity = oInput.Activities[intIdx];
                    WriteActivity(oActivity, oInput.UserName, context, ref oSqlConnection);

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


        bool WriteActivity(tInput.tActivity oActivity, string strCreatedBy, ILambdaContext context, ref SqlConnection oSqlConnection)
        {

            string strQuery;
            bool bolReturn = true;

            try

            {

                strQuery = " INSERT INTO EhiuActivityLog ( SerialNumber, EcoCommandSend, EcoCommandReply, CommandName, IsGet, PostStatus, PathElement, CreatedBy ) Values " +
                           " ( @SerialNumber, @EcoCommandSend, @EcoCommandReply, @CommandName, @IsGet, @PostStatus, @PathElement, @CreatedBy )  ";


                SqlCommand sqlInsert = new SqlCommand(strQuery, oSqlConnection);

                SqlParameter sqlParamSerialNumber = new SqlParameter("@SerialNumber", SqlDbType.NVarChar);
                sqlParamSerialNumber.Value = oActivity.SerialNumber;
                sqlInsert.Parameters.Add(sqlParamSerialNumber);

                SqlParameter sqlParamEcoCommandSend = new SqlParameter("@EcoCommandSend", SqlDbType.NVarChar);
                sqlParamEcoCommandSend.Value = oActivity.EcoCommandSend;
                sqlInsert.Parameters.Add(sqlParamEcoCommandSend);

                SqlParameter sqlParamEcoCommandReply = new SqlParameter("@EcoCommandReply", SqlDbType.NVarChar);
                sqlParamEcoCommandReply.Value = oActivity.EcoCommandReply;
                sqlInsert.Parameters.Add(sqlParamEcoCommandReply);

                SqlParameter sqlParamCommandName = new SqlParameter("@CommandName", SqlDbType.NVarChar);
                sqlParamCommandName.Value = oActivity.CommandName;
                sqlInsert.Parameters.Add(sqlParamCommandName);

                SqlParameter sqlParamIsGet = new SqlParameter("@IsGet", SqlDbType.Bit);
                sqlParamIsGet.Value = oActivity.IsGet;
                sqlInsert.Parameters.Add(sqlParamIsGet);

                SqlParameter sqlParamPostStatus = new SqlParameter("@PostStatus", SqlDbType.NVarChar);
                sqlParamPostStatus.Value = oActivity.PostStatus;
                sqlInsert.Parameters.Add(sqlParamPostStatus);


                SqlParameter sqlParamPathElement = new SqlParameter("@PathElement", SqlDbType.NVarChar);
                sqlParamPathElement.Value = oActivity.PathElement;
                sqlInsert.Parameters.Add(sqlParamPathElement);


                SqlParameter sqlParamCreatedBy = new SqlParameter("@CreatedBy", SqlDbType.NVarChar);
                sqlParamCreatedBy.Value = strCreatedBy;
                sqlInsert.Parameters.Add(sqlParamCreatedBy);



                try
                {
                    sqlInsert.Connection.Open();
                }
                catch (Exception)
                {
                }




                sqlInsert.ExecuteNonQuery();

                sqlInsert.Connection.Close();

            }

            catch (Exception ex)
            {
                context.Logger.LogLine("Ex in FunctionHandler " + ex.Message);
                bolReturn = false;
            }

            return bolReturn;
        }
    }
}
