using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoUnitSearch
{

    public class tInput
    {
        public string SearchOptions { get; set; } = "All";
        public string AddressSearch { get; set; } = "";
        public string SiteSearch { get; set; } = "";
        public string SerialSearch { get; set; } = "";

    };


    public class tResult
    {

        public bool Ok { get; set; } = true;
        public string Info { get; set; } = "";

        public List<tProperty> Propertys { get; set; } = new List<tProperty>();

        public class tProperty
        {


            public int? EhiuInstallID { get; set; } = null;
            public int? PropertyID { get; set; } = null;
            public string SerialNumber { get; set; } = null;
            public string FromDate { get; set; } = null;
            public string DateCreated { get; set; } = null;
            public string CreatedBy { get; set; } = null;
            public string PropertyNumber { get; set; } = null;
            public string BlockName { get; set; } = null;
            public string BlockAddress { get; set; } = null;
            public string PropertyReference { get; set; } = null;
            public string BlockPostCode { get; set; } = null;
            public string SiteName { get; set; } = null;
            public string Addr { get; set; } = null;
            public int? SiteID { get; set; } = null;
        }
    }


    public class CloudEcoUnitSearch
    {

        public tResult FunctionHandler(tInput oInput, ILambdaContext context)
        {

            string strQuery = "";
            SqlDataAdapter daCheck = new SqlDataAdapter();
            DataSet dsCheck = new DataSet();

            SqlConnection oSqlConnection = null;
            tResult oResult = new tResult();
            int intIdx;
            bool bolValidated = false;
            DateTime datFrom;
            DateTime datDateCreated;

            oInput.SearchOptions = oInput.SearchOptions.ToLower();

            if (oInput.SearchOptions == "all" || oInput.SearchOptions == "withecoplus" || oInput.SearchOptions == "noecoplus")
            {
                bolValidated = true;
            };

            if (bolValidated == false)
            {
                oResult.Ok = false;
                oResult.Info = "Search options must be either WithEcoPlus or NoEcoPlus or All";

                return oResult;
            }

            try
            {

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

                if (oInput.SearchOptions == "all")
                {
                    strQuery = " SELECT  Top 1000      EhiuInstall_2.EhiuInstallID, Property_2.PropertyID,  EhiuInstall_2.SerialNumber, EhiuInstall_2.FromDate,  EhiuInstall_2.DateCreated, EhiuInstall_2.CreatedBy, " +
                               " Property_2.PropertyNumber, Property_2.BlockName, Property_2.BlockAddress, Property_2.PropertyReference, Property_2.BlockPostCode, Site_2.SiteKey, Site_2.SiteName, " +
                               " LTRIM(RTRIM(Property_2.PropertyNumber + ' ' + Property_2.BlockName)) AS Addr, Site_2.SiteID" +
                               " FROM            Site AS Site_2 INNER JOIN" +
                               "  Property AS Property_2 ON Site_2.SiteID = Property_2.SiteID LEFT OUTER JOIN" +
                               " EhiuInstall AS EhiuInstall_2 ON Property_2.PropertyID = EhiuInstall_2.PropertyID" +
                               " WHERE        (Property_2.PropertyNumber + Property_2.BlockName + Property_2.BlockAddress + Property_2.PropertyReference + Property_2.BlockPostCode LIKE DBO.PrepQry(@AddrSearch)) AND " +
                               " (Site_2.SiteName LIKE DBO.PrepQry(@SiteSearch))" +
                               " ORDER BY SerialNumber, PropertyReference";
                };

                if (oInput.SearchOptions == "withecoplus")
                {

                    strQuery = "SELECT  Top 1000   EhiuInstallID, EhiuInstall.PropertyID, EhiuInstall.SerialNumber, EhiuInstall.FromDate,  EhiuInstall.DateCreated, " +
                            " EhiuInstall.CreatedBy, Property.PropertyNumber, Property.BlockName, Property.BlockAddress, Property.PropertyReference, Property.BlockPostCode, Site.SiteKey, Site.SiteName, " +
                            " LTRIM(RTRIM(Property.PropertyNumber + ' ' + Property.BlockName)) AS Addr, Site.SiteID" +
                            " FROM            EhiuInstall INNER JOIN" +
                            "  Property ON EhiuInstall.PropertyID = Property.PropertyID INNER JOIN" +
                            " Site ON Property.SiteID = Site.SiteID" +
                            " WHERE        (Property.PropertyNumber + Property.BlockName + Property.BlockAddress + Property.PropertyReference + Property.BlockPostCode LIKE DBO.PrepQry(@AddrSearch)) AND (Site.SiteName LIKE DBO.PrepQry(@SiteSearch)) AND " +
                            " (EhiuInstall.SerialNumber LIKE DBO.PrepQry(@SerialSearch)) AND (EhiuInstall.ToDate IS NULL)" +
                            " ORDER BY SerialNumber, PropertyReference";

                };

                if (oInput.SearchOptions == "noecoplus")
                {


                    strQuery = " SELECT  Top 1000      NULL AS EhiuInstallID, Property_1.PropertyID,  NULL AS SerialNumber, NULL AS FromDate,  NULL AS DateCreated, NULL AS CreatedBy, Property_1.PropertyNumber, Property_1.BlockName, " +
                                " Property_1.BlockAddress, Property_1.PropertyReference, Property_1.BlockPostCode, Site_1.SiteKey, Site_1.SiteName, LTRIM(RTRIM(Property_1.PropertyNumber + ' ' + Property_1.BlockName)) AS Addr," +
                                " Site_1.SiteID " +
                                " FROM            Property AS Property_1 INNER JOIN" +
                                " Site AS Site_1 ON Property_1.SiteID = Site_1.SiteID" +
                                " WHERE        (Property_1.PropertyNumber + Property_1.BlockName + Property_1.BlockAddress + Property_1.PropertyReference + Property_1.BlockPostCode LIKE DBO.PrepQry(@AddrSearch)) AND " +
                                " (Site_1.SiteName LIKE DBO.PrepQry(@SiteSearch)) AND (NOT (Property_1.PropertyID IN" +
                                "  (SELECT        PropertyID" +
                                "  FROM            EhiuInstall AS EhiuInstall_1" +
                                "  WHERE        (ToDate IS NULL)))) " +
                                " ORDER BY SerialNumber, PropertyReference";
                };

                daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

                SqlParameter sqlParamSerialSearch = new SqlParameter("@SerialSearch", SqlDbType.NVarChar);
                sqlParamSerialSearch.Value = oInput.SerialSearch;
                daCheck.SelectCommand.Parameters.Add(sqlParamSerialSearch);

                SqlParameter sqlParamSiteSearch = new SqlParameter("@SiteSearch", SqlDbType.NVarChar);
                sqlParamSiteSearch.Value = oInput.SiteSearch;
                daCheck.SelectCommand.Parameters.Add(sqlParamSiteSearch);

                SqlParameter sqlParamAddrSearch = new SqlParameter("@AddrSearch", SqlDbType.NVarChar);
                sqlParamAddrSearch.Value = oInput.AddressSearch;
                daCheck.SelectCommand.Parameters.Add(sqlParamAddrSearch);

                daCheck.Fill(dsCheck);

                for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
                {

                    tResult.tProperty oProperty = new tResult.tProperty();

                    if (dsCheck.Tables[0].Rows[intIdx]["EhiuInstallID"] != DBNull.Value)
                    {
                        oProperty.EhiuInstallID = (int)dsCheck.Tables[0].Rows[intIdx]["EhiuInstallID"];
                    };

                    if (dsCheck.Tables[0].Rows[intIdx]["PropertyID"] != DBNull.Value)
                    {
                        oProperty.PropertyID = (int)dsCheck.Tables[0].Rows[intIdx]["PropertyID"];
                    };

                    if (dsCheck.Tables[0].Rows[intIdx]["SerialNumber"] != DBNull.Value)
                    {
                        oProperty.SerialNumber = (string)dsCheck.Tables[0].Rows[intIdx]["SerialNumber"];
                    };

                    if (dsCheck.Tables[0].Rows[intIdx]["FromDate"] != DBNull.Value)
                    {
                        datFrom = (DateTime)dsCheck.Tables[0].Rows[intIdx]["FromDate"];
                        oProperty.FromDate = datFrom.ToString();
                    };

                    if (dsCheck.Tables[0].Rows[intIdx]["DateCreated"] != DBNull.Value)
                    {
                        datDateCreated = (DateTime)dsCheck.Tables[0].Rows[intIdx]["DateCreated"];
                        oProperty.FromDate = datDateCreated.ToString();
                    };

                    if (dsCheck.Tables[0].Rows[intIdx]["CreatedBy"] != DBNull.Value)
                    {
                        oProperty.CreatedBy = (string)dsCheck.Tables[0].Rows[intIdx]["CreatedBy"];
                    };

                    if (dsCheck.Tables[0].Rows[intIdx]["PropertyNumber"] != DBNull.Value)
                    {
                        oProperty.PropertyNumber = (string)dsCheck.Tables[0].Rows[intIdx]["PropertyNumber"];
                    };

                    if (dsCheck.Tables[0].Rows[intIdx]["BlockName"] != DBNull.Value)
                    {
                        oProperty.BlockName = (string)dsCheck.Tables[0].Rows[intIdx]["BlockName"];
                    };

                    if (dsCheck.Tables[0].Rows[intIdx]["BlockAddress"] != DBNull.Value)
                    {
                        oProperty.BlockAddress = (string)dsCheck.Tables[0].Rows[intIdx]["BlockAddress"];
                    };

                    if (dsCheck.Tables[0].Rows[intIdx]["PropertyReference"] != DBNull.Value)
                    {
                        oProperty.PropertyReference = (string)dsCheck.Tables[0].Rows[intIdx]["PropertyReference"];
                    };

                    if (dsCheck.Tables[0].Rows[intIdx]["BlockPostCode"] != DBNull.Value)
                    {
                        oProperty.BlockPostCode = (string)dsCheck.Tables[0].Rows[intIdx]["BlockPostCode"];
                    };

                    if (dsCheck.Tables[0].Rows[intIdx]["SiteName"] != DBNull.Value)
                    {
                        oProperty.SiteName = (string)dsCheck.Tables[0].Rows[intIdx]["SiteName"];
                    };

                    if (dsCheck.Tables[0].Rows[intIdx]["Addr"] != DBNull.Value)
                    {
                        oProperty.Addr = (string)dsCheck.Tables[0].Rows[intIdx]["Addr"];
                    };
                    if (dsCheck.Tables[0].Rows[intIdx]["SiteID"] != DBNull.Value)
                    {
                        oProperty.SiteID = (int)dsCheck.Tables[0].Rows[intIdx]["SiteID"];
                    };



                    oResult.Propertys.Add(oProperty);

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

