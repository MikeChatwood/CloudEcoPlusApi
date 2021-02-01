using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoGetDiagValues
{

    public class tInput
    {
        public string SerialNumber { get; set; } = null;

    };


    public class tResult
    {
        public bool HasError { get; set; } = false;
        public bool IsInstalled { get; set; } = false;
        public string Info { get; set; } = "";
        public string PrimaryFlowTemp { get; set; } = "";
        public string PrimaryFlowReturnTemp { get; set; } = "";
        public string PrimaryFlowRate { get; set; } = "";
        public string SpaceHeatFlowTemp { get; set; } = "";
        public string SpaceHeatFlowReturnTemp { get; set; } = "";
        public string SpaceHeatActuatorPosition { get; set; } = "";
        public string PumpSpeed { get; set; } = "";
        public string SpaceHeatDemandPresent { get; set; } = "";
        public string DhwFlow { get; set; } = "";
        public string DhwReturn { get; set; } = "";
        public string DhwRate { get; set; } = "";
        public string ActuatorPositionDhw { get; set; } = "";
        public string DemandPresentDhw { get; set; } = "";
        public string PaygoStatus { get; set; } = "";
        public int LastSeenMinutesAgo { get; set; } = -1;

    }

    public class tMbusconfig_Companion
    {
        public int FlowTempID { get; set; } = -1;
        public int ReturnTempID { get; set; } = -1;
        public int FlowRate { get; set; } = -1;
    }


    public class tCommand
    {

        public string ReplyJson { get; set; } = null;
        public string PostStatus { get; set; } = null; //  Ok / OffLine / Timeout / NotSent

        public bool ToEco { get; set; } = true;
        public string UrlPath = "";

        public string PathElement { get; set; } = null;

        public DateTime? StartTime { get; set; } = null;
        public DateTime? EndTime { get; set; } = null;



        public string CommandName { get; set; } = null;
        public string CommandJson { get; set; } = null;

        public string CompanionName { get; set; } = null;
        public string CompanionJson { get; set; } = null;




    }



    public class controlreport
    {
        private int _hwValve = 0;
        private int _chPump = 0;
        private int _chValve = 0;


        public int hwValve
        {
            get
            {
                return _hwValve;
            }
            set
            {
                _hwValve = value;
            }
        }



        public int chPump
        {
            get
            {
                return _chPump;
            }
            set
            {
                _chPump = value;
            }
        }



        public int chValve
        {
            get
            {
                return _chValve;
            }
            set
            {
                _chValve = value;
            }
        }


    }



    public class sensorreport
    {
        private decimal _chPressure = 0;
        private decimal _hwInTemp = 0;
        private decimal _priInTemp = 0;
        private int _chDemand = 0;
        private decimal _hwFlowRate = 0;
        private decimal _priOutTemp = 0;
        private decimal _hwOutTemp = 0;
        private decimal _chOutTemp = 0;
        private decimal _chInTemp = 0;
        private int _payg = 0;


        public decimal chPressure
        {
            get
            {
                return _chPressure;
            }
            set
            {
                _chPressure = value;
            }
        }

        public decimal hwInTemp
        {
            get
            {
                return _hwInTemp;
            }
            set
            {
                _hwInTemp = value;
            }
        }


        public decimal priInTemp
        {
            get
            {
                return _priInTemp;
            }
            set
            {
                _priInTemp = value;
            }
        }

        public int chDemand
        {
            get
            {
                return _chDemand;
            }
            set
            {
                _chDemand = value;
            }
        }


        public decimal hwFlowRate
        {
            get
            {
                return _hwFlowRate;
            }
            set
            {
                _hwFlowRate = value;
            }
        }


        public decimal priOutTemp
        {
            get
            {
                return _priOutTemp;
            }
            set
            {
                _priOutTemp = value;
            }
        }



        public decimal hwOutTemp
        {
            get
            {
                return _hwOutTemp;
            }
            set
            {
                _hwOutTemp = value;
            }
        }



        public decimal chOutTemp
        {
            get
            {
                return _chOutTemp;
            }
            set
            {
                _chOutTemp = value;
            }
        }


        public decimal chInTemp
        {
            get
            {
                return _chInTemp;
            }
            set
            {
                _chInTemp = value;
            }
        }


        public int payg
        {
            get
            {
                return _payg;
            }
            set
            {
                _payg = value;
            }
        }

    }



    public class mbusreport
    {
        private UInt32 _mbusTimestamp = 0;
        private byte _mbusAddress = 0;

        private int _mbusMain = 0;
        private double _mbusSub1 = 0;
        private double _mbusSub2 = 0;
        private double _mbusSub3 = 0;
        private double _mbusSub4 = 0;
        private double _mbusSub5 = 0;
        private int _mbusError = 0;
        private UInt64 _mbusSerial;  // the value read by the bus

        public int mbusMain
        {
            get
            {
                return _mbusMain;
            }
            set
            {
                _mbusMain = value;
            }
        }

        public double mbusSub1
        {
            get
            {
                return _mbusSub1;
            }
            set
            {
                _mbusSub1 = value;
            }
        }

        public double mbusSub2
        {
            get
            {
                return _mbusSub2;
            }
            set
            {
                _mbusSub2 = value;
            }
        }

        public double mbusSub3
        {
            get
            {
                return _mbusSub3;
            }
            set
            {
                _mbusSub3 = value;
            }
        }


        public double mbusSub4
        {
            get
            {
                return _mbusSub4;
            }
            set
            {
                _mbusSub4 = value;
            }
        }

        public double mbusSub5
        {
            get
            {
                return _mbusSub5;
            }
            set
            {
                _mbusSub5 = value;
            }
        }

        public UInt64 mbusSerial
        {
            get
            {
                return _mbusSerial;
            }
            set
            {
                _mbusSerial = value;
            }
        }

        public int mbusError
        {
            get
            {
                return _mbusError;
            }
            set
            {
                _mbusError = value;
            }
        }

        public UInt32 mbusTimestamp
        {
            get
            {
                return _mbusTimestamp;
            }
            set
            {
                _mbusTimestamp = value;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return UnixTimeToDateTime(_mbusTimestamp);
            }
        }

        public byte mbusAddress
        {
            get
            {
                return _mbusAddress;
            }
            set
            {
                _mbusAddress = value;
            }
        }


        private static DateTime UnixTimeToDateTime(UInt32 uInt32UnixDate)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return origin.AddSeconds(uInt32UnixDate);
        }


    }




    public class CloudEcoGetDiagValues
    {

        const string strApiUrl = "https://ndygcmuadj.execute-api.eu-west-2.amazonaws.com/1/";
        const string strApiKey = "RhTHczPuO97m6D7E1uTg6ajnWKbN3zlhaaf6x2cv";
        public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {

            string strQuery = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();

            SqlConnection oSqlConnection = null;
            tResult oResult = new tResult();
            int intIdx;

            int intDhwFlowTempID = -1;
            int intDhwReturnTempID = -1;
            int intDhwRate = -1;
            string strJson;
            tCommand oCommandSend;
            controlreport oControlReport;
            sensorreport oSensorReport;
            mbusreport oMbusReport;
            string strIMEI;

            try
            {

                // All values are returned as strings, ready for display

                context.Logger.LogLine("FunctionHandler 1 " + oInput.ToString());

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


                strIMEI = ecoCommon.GetDeviceIMEINumber(oInput.SerialNumber, context, ref oSqlConnection);


                // get the last seen



                oResult.LastSeenMinutesAgo = GetLastSeen(strIMEI, context);



                // End Get last seen





                // Get the configuration for the current property
                strQuery = "SELECT TOP 1 EhiuInstallDetailCommands.MbusId, EhiuInstallDetailCommands.SendJson, EhiuInstallDetailCommands.ReplyJson, EhiuInstallDetailCommands.CompanionName, " +
                            " EhiuInstallDetailCommands.CompanionItemJson,  " +
                            " EhiuInstallDetailCommands.DateCreated " +
                            " FROM EhiuInstall INNER JOIN " +
                            " EhiuInstallDetail ON EhiuInstall.EhiuInstallID = EhiuInstallDetail.EhiuInstallID INNER JOIN " +
                            " EhiuInstallDetailCommands ON EhiuInstallDetail.EhiuInstallDetailID = EhiuInstallDetailCommands.EhiuInstallDetailID " +
                            " WHERE (EhiuInstallDetailCommands.CommandName = N'mbusconfig') AND (EhiuInstallDetailCommands.MbusId IS NOT NULL) AND (EhiuInstall.SerialNumber = @SerialNumber) And " +
                            " EhiuInstallDetailCommands.MbusId = 1 " +
                            " ORDER BY EhiuInstallDetailCommands.DateCreated DESC";

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamSerialNumber = new SqlParameter("@SerialNumber", SqlDbType.NVarChar);
                sqlParamSerialNumber.Value = oInput.SerialNumber;
                daCheck.SelectCommand.Parameters.Add(sqlParamSerialNumber);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {

                    strJson = (string)dsCheck.Tables[0].Rows[intIdx]["CompanionItemJson"];

                    tMbusconfig_Companion mbusconfig_Companion;
                    mbusconfig_Companion = JsonSerializer.Deserialize<tMbusconfig_Companion>(strJson);

                    intDhwFlowTempID = mbusconfig_Companion.FlowTempID;
                    intDhwReturnTempID = mbusconfig_Companion.ReturnTempID;
                    intDhwRate = mbusconfig_Companion.FlowRate;

                    oResult.IsInstalled = true;



                }
                // Build list to send
                if (oResult.IsInstalled == false)
                {
                    return oResult;
                }


                // -- Send Command -----------------------------------------------------------------
                oCommandSend = new tCommand();

                oCommandSend.CommandName = "controlreport";
                oCommandSend.UrlPath = "controlreport/{devid}";
                oCommandSend.CommandJson = "";
                oCommandSend.ToEco = false;

                tCommand oControlreportReply = new tCommand();

                oControlreportReply = PostApi(oCommandSend, strIMEI);
                if (oControlreportReply.PostStatus != "ok")
                {
                    oResult.HasError = true;
                    oResult.Info = "Controlreport " + oControlreportReply.PostStatus;
                    return oResult;
                };


                oControlReport = JsonSerializer.Deserialize<controlreport>(oControlreportReply.ReplyJson);
                // -- End Command -----------------------------------------------------------------



                // -- Send Command -----------------------------------------------------------------
                oCommandSend = new tCommand();

                oCommandSend.CommandName = "sensorreport";
                oCommandSend.UrlPath = "sensorreport/{devid}";
                oCommandSend.CommandJson = "";
                oCommandSend.ToEco = false;

                tCommand oSensorreportReply = new tCommand();

                oSensorreportReply = PostApi(oCommandSend, strIMEI);
                if (oSensorreportReply.PostStatus != "ok")
                {
                    oResult.HasError = true;
                    oResult.Info = "SensorReport " + oSensorreportReply.PostStatus;
                    return oResult;
                };


                oSensorReport = JsonSerializer.Deserialize<sensorreport>(oSensorreportReply.ReplyJson);
                // -- End Command -----------------------------------------------------------------



                // -- Send Command -----------------------------------------------------------------
                oCommandSend = new tCommand();

                oCommandSend.CommandName = "mbusreport";
                oCommandSend.UrlPath = "mbusreport/{devid}/{pathelement}";
                oCommandSend.CommandJson = "";
                oCommandSend.PathElement = "1";
                oCommandSend.ToEco = false;

                tCommand oMbusReportReply = new tCommand();

                oMbusReportReply = PostApi(oCommandSend, strIMEI);
                if (oMbusReportReply.PostStatus != "ok")
                {
                    oResult.HasError = true;
                    oResult.Info = "SensorReport " + oMbusReportReply.PostStatus;
                    return oResult;
                };


                oMbusReport = JsonSerializer.Deserialize<mbusreport>(oMbusReportReply.ReplyJson);
                // -- End Command -----------------------------------------------------------------


                if (oSensorReport.payg == 0)
                {
                    oResult.PaygoStatus = "Off";
                }
                else
                {
                    oResult.PaygoStatus = "On";
                };


                oResult.PrimaryFlowRate = GetSubValue(oMbusReport, intDhwRate).ToString("0.0") + " l/m";
                oResult.PrimaryFlowReturnTemp = GetSubValue(oMbusReport, intDhwReturnTempID).ToString("0.0") + " c";
                oResult.PrimaryFlowTemp = GetSubValue(oMbusReport, intDhwFlowTempID).ToString("0.0") + " c";

                oResult.PumpSpeed = oControlReport.chPump.ToString();
                oResult.SpaceHeatActuatorPosition = oControlReport.chValve.ToString();
                oResult.SpaceHeatDemandPresent = oSensorReport.chDemand.ToString("0.0");
                oResult.SpaceHeatFlowReturnTemp = oSensorReport.chInTemp.ToString("0.0") + " c";

                oResult.SpaceHeatFlowTemp = oSensorReport.chOutTemp.ToString("0.0") + " c";

                oResult.DhwFlow = oSensorReport.hwInTemp.ToString("0.0") + " c";
                oResult.DhwReturn = oSensorReport.hwOutTemp.ToString("0.0") + " c";
                oResult.DhwRate = oSensorReport.hwFlowRate.ToString("0.0") + " l/m";

                oResult.ActuatorPositionDhw = oControlReport.hwValve.ToString();

                if (oSensorReport.hwFlowRate == 0)
                {
                    oResult.DemandPresentDhw = "Yes";
                }
                else
                {
                    oResult.DemandPresentDhw = "No";
                }

            }








            catch (Exception ex)
            {
                context.Logger.LogLine("Ex in FunctionHandler " + ex.Message);
                oResult.HasError = false;
                oResult.Info = ex.Message;
            }

            return oResult;
        }

        private double GetSubValue(mbusreport oMbusReport, int intIndex)
        {

            double dblReturn = 0;

            if (intIndex == 1)
            {
                dblReturn = oMbusReport.mbusSub1;
            }

            if (intIndex == 2)
            {
                dblReturn = oMbusReport.mbusSub2;
            }

            if (intIndex == 3)
            {
                dblReturn = oMbusReport.mbusSub3;
            }

            if (intIndex == 4)
            {
                dblReturn = oMbusReport.mbusSub4;
            }

            if (intIndex == 5)
            {
                dblReturn = oMbusReport.mbusSub5;
            };


            return dblReturn;
        }

        private tCommand PostApi(tCommand oCommandSend, string strIMEI)
        {
            HttpWebRequest request;
            HttpWebResponse response = null/* TODO Change to default(_) if this is not a reference type */;
            StreamReader reader;
            Uri address;
            string appId;
            string strResponse = "";
            UInt32 uInt32StartDatetime;
            UInt32 uInt32EndDatetime;

            Stream postStream = null;
            string strUrl;
            byte[] byteData;
            StringBuilder data;
            tCommand oCommandReply = new tCommand();

            oCommandReply.PostStatus = "offline";





            strUrl = strApiUrl;

            strUrl = strUrl + oCommandSend.UrlPath;
            strUrl = strUrl.Replace("{devid}", strIMEI);
            strUrl = strUrl.Replace("{pathelement}", oCommandSend.PathElement);

            if (oCommandSend.StartTime != null && oCommandSend.EndTime != null)
            {
                uInt32StartDatetime = DateTimeToUnixTime(oCommandSend.StartTime);
                uInt32EndDatetime = DateTimeToUnixTime(oCommandSend.EndTime);
                strUrl = strUrl + "?startTime=" + uInt32StartDatetime.ToString() + "&endTime=" + uInt32EndDatetime.ToString();
            }



            address = new Uri(strUrl);

            try
            {

                // The reply is mainly the same as the send, for easier processing by the client, ie send and reply together

                oCommandReply.CommandJson = oCommandSend.CommandJson;
                oCommandReply.CommandName = oCommandSend.CommandName;
                oCommandReply.CompanionJson = oCommandSend.CompanionJson;
                oCommandReply.CompanionName = oCommandSend.CompanionName;
                oCommandReply.EndTime = oCommandSend.EndTime;
                oCommandReply.PathElement = oCommandSend.PathElement;
                oCommandReply.StartTime = oCommandSend.StartTime;
                oCommandReply.ToEco = oCommandSend.ToEco;

                request = (HttpWebRequest)WebRequest.Create(address);
                if (oCommandSend.ToEco == true)
                {
                    request.Method = "PATCH";
                }
                else
                {
                    request.Method = "GET";
                }

                request.ContentType = "application/x-www-form-urlencoded";

                request.Headers.Add("x-api-key", strApiKey);

                appId = "ss-form";
                if (oCommandSend.ToEco == true)
                {
                    data = new StringBuilder();

                    data.Append(oCommandSend.CommandJson);

                    byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                    // Set the content length in the request headers  
                    request.ContentLength = byteData.Length;

                    // Write data  
                    try
                    {
                        postStream = request.GetRequestStream();
                        postStream.Write(byteData, 0, byteData.Length);
                    }
                    finally
                    {
                        if (postStream != null)
                            postStream.Close();
                    }
                }
                else
                {

                    if (postStream != null)
                        postStream.Close();
                }
                try
                {
                    // Get response  
                    response = (HttpWebResponse)request.GetResponse();

                    // Get the response stream into a reader  
                    reader = new StreamReader(response.GetResponseStream());

                    strResponse = reader.ReadToEnd();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        oCommandReply.PostStatus = "ok";
                        oCommandReply.ReplyJson = strResponse;

                        return oCommandReply;
                    }
                    else
                    {
                    }
                }
                finally
                {
                    if (response != null)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                            oCommandReply.PostStatus = "ok";

                        if (response.StatusCode.ToString().IndexOf("504") != -1)
                            oCommandReply.PostStatus = "timeout";
                        response.Close();
                    }
                    else
                        oCommandReply.PostStatus = "offline";
                }
            }
            catch (Exception ex)
            {
            }


            return oCommandReply;
        }

        private static UInt32 DateTimeToUnixTime(DateTime? aDate)
        {
            UInt32 intReturn;

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = (DateTime)aDate - origin;

            intReturn = (UInt32)diff.TotalSeconds;

            return intReturn;
        }


        public class tLastSeen
        {
            public int timeStamp { get; set; }
        }

        private int GetLastSeen(string strIMEI, ILambdaContext context)
        {

            int intReturn = -1;
            string strUrl;
            Uri address;
            HttpWebRequest request;
            HttpWebResponse response = null;
            StreamReader reader;
            string appId;
            Stream postStream = null;
            string strResponse = "";
            tLastSeen oLastSeen;
            DateTime datLastSeen;


            strUrl = strApiUrl;

            strUrl = strUrl + "lastseen/";
            strUrl = strUrl + strIMEI;

            address = new Uri(strUrl);

            try
            {
                request = (HttpWebRequest)WebRequest.Create(address);

                request.Method = "GET";

                request.ContentType = "application/x-www-form-urlencoded";

                request.Headers.Add("x-api-key", strApiKey);

                appId = "ss-form";

                if (postStream != null)
                    postStream.Close();

                try
                {
                    // Get response  
                    response = (HttpWebResponse)request.GetResponse();

                    // Get the response stream into a reader  
                    reader = new StreamReader(response.GetResponseStream());

                    strResponse = reader.ReadToEnd();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        oLastSeen = JsonSerializer.Deserialize<tLastSeen>(strResponse);


                        datLastSeen = UnixTimeToDateTime(oLastSeen.timeStamp);
                        TimeSpan ts = DateTime.Now - datLastSeen;
                        intReturn  = (int)ts.TotalMinutes;
                    }
                    else
                    {
                    }
                }
                finally
                {
                    if (response != null)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                            //  oCommandReply.PostStatus = "ok";

                            if (response.StatusCode.ToString().IndexOf("504") != -1)
                                //  oCommandReply.PostStatus = "timeout";
                                response.Close();
                    }
                    else
                    {
                        // oCommandReply.PostStatus = "offline";
                    }
                }
            }

            catch (Exception ex)
            {
            }

            return intReturn;
        }

        private DateTime UnixTimeToDateTime(int intUnixDate)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return origin.AddSeconds(intUnixDate);
        }

    }
}
