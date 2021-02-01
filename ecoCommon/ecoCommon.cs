using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System;
using System.IO;
using System.Text.Json;
using Amazon.Lambda.Core;
using System.Data.SqlClient;
using System.Data;

public class ecoCommon
{

    #region "Secret"

    public class tSecret
    {
        public string ConnectionString { get; set; }

    };


    public static string GetSecret(string strSecretName, ILambdaContext context)
    {
        string secretName = strSecretName;
        string region = "eu-west-2";
        string secret = "";
        tSecret oSecret = new tSecret();


        context.Logger.LogLine("GetSecret 1");

        MemoryStream memoryStream = new MemoryStream();


        IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

        context.Logger.LogLine("GetSecret 2" + RegionEndpoint.GetBySystemName(region));

        GetSecretValueRequest request = new GetSecretValueRequest();
        request.SecretId = secretName;
        request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.

        GetSecretValueResponse response = null;

        // In this sample we only handle the specific exceptions for the 'GetSecretValue' API.
        // See https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
        // We re throw the exception by default.

        try
        {
            context.Logger.LogLine("GetSecret 3");
            response = client.GetSecretValueAsync(request).Result;

            context.Logger.LogLine("GetSecret 4");
        }
        catch (DecryptionFailureException e)
        {
            // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
            // Deal with the exception here, and/or rethrow at your discretion.
            context.Logger.LogLine("GetSecret DecryptionFailureException");
            throw;
        }
        catch (InternalServiceErrorException e)
        {
            // An error occurred on the server side.
            // Deal with the exception here, and/or rethrow at your discretion.
            context.Logger.LogLine("GetSecret InternalServiceErrorException");
            throw;
        }
        catch (InvalidParameterException e)
        {
            // You provided an invalid value for a parameter.
            // Deal with the exception here, and/or rethrow at your discretion
            context.Logger.LogLine("GetSecret InvalidParameterException");
            throw;
        }
        catch (InvalidRequestException e)
        {
            // You provided a parameter value that is not valid for the current state of the resource.
            // Deal with the exception here, and/or rethrow at your discretion.
            context.Logger.LogLine("GetSecret InvalidRequestException");
            throw;
        }
        catch (ResourceNotFoundException e)
        {
            // We can't find the resource that you asked for.
            // Deal with the exception here, and/or rethrow at your discretion.
            context.Logger.LogLine("GetSecret ResourceNotFoundException");
            throw;
        }
        catch (System.AggregateException ae)
        {
            // More than one of the above exceptions were triggered.
            // Deal with the exception here, and/or rethrow at your discretion.
            context.Logger.LogLine("GetSecret AggregateException");
            throw;
        }

        // Decrypts secret using the associated KMS CMK.
        // Depending on whether the secret is a string or binary, one of these fields will be populated.
        if (response.SecretString != null)
        {

            context.Logger.LogLine("GetSecret 33");
            secret = response.SecretString;

            oSecret = JsonSerializer.Deserialize<tSecret>(response.SecretString);
        }
        else
        {
            memoryStream = response.SecretBinary;
            StreamReader reader = new StreamReader(memoryStream);
            string decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
        }


        context.Logger.LogLine("GetSecret 4a " + oSecret.ConnectionString);
        return oSecret.ConnectionString;

    }




    #endregion

    public static DateTime DateJson(string strDate)
    {

        // DD MM YYYY HH MM SS
        // 01 23 4567 89 11 1
        // 01 2
        // 

        DateTime datReturn;

        strDate = strDate.Replace("/", "");
        strDate = strDate.Replace(":", "");
        strDate = strDate.Replace(" ", "");

        if (strDate.Length == 8)
        {
            strDate = strDate + "000000";
        }

        datReturn = new DateTime(System.Convert.ToInt32(strDate.Substring(4, 4)), System.Convert.ToInt32(strDate.Substring(2, 2)), System.Convert.ToInt32(strDate.Substring(0, 2)), System.Convert.ToInt32(strDate.Substring(8, 2)), System.Convert.ToInt32(strDate.Substring(10, 2)), System.Convert.ToInt32(strDate.Substring(12, 2))
    );


        return datReturn;
    }

    public static string JsonDate(DateTime datConvert)
    {

        string strReturn;

        strReturn = datConvert.Day.ToString("00") + datConvert.Month.ToString("00") + datConvert.Year.ToString("0000") +
                    datConvert.Hour.ToString("00") + datConvert.Minute.ToString("00") + datConvert.Second.ToString("00");


        return strReturn;

    }



    public static bool CodeExists(int? intID, string strTable, string strField, ILambdaContext Context, ref SqlConnection oSqlConnection)
    {

        string strQuery = "";
        SqlDataAdapter daCheck = new SqlDataAdapter();
        DataSet dsCheck = new DataSet();
        bool bolReturn = false;
        int intIdx;

        try
        {
            oSqlConnection.Open();
        }
        catch (Exception)
        {
        }

        strQuery = "SELECT " + strField + " From " + strTable + " Where " + strField + " = @ID";

        daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

        SqlParameter sqlParamID = new SqlParameter("@ID", SqlDbType.Int);
        sqlParamID.Value = intID;
        daCheck.SelectCommand.Parameters.Add(sqlParamID);

        daCheck.Fill(dsCheck);

        for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
        {
            bolReturn = true;

        }

        return bolReturn;

    }

    public static string GetDeviceIMEINumber(string strSerialNumber, ILambdaContext Context, ref SqlConnection oSqlConnection)
    {

        string strQuery = "";
        SqlDataAdapter daCheck = new SqlDataAdapter();
        DataSet dsCheck = new DataSet();
        string strReturn = "";
        int intIdx;

        try
        {
            oSqlConnection.Open();
        }
        catch (Exception)
        {
        }

        strQuery = "SELECT IMEI From Ehiu Where SerialNumber = @SerialNumber";

        daCheck = new SqlDataAdapter(strQuery, oSqlConnection);

        SqlParameter sqlParamSerialNumber = new SqlParameter("@SerialNumber", SqlDbType.NVarChar);
        sqlParamSerialNumber.Value = strSerialNumber;
        daCheck.SelectCommand.Parameters.Add(sqlParamSerialNumber);

        daCheck.Fill(dsCheck);

        for (intIdx = 0; intIdx <= dsCheck.Tables[0].Rows.Count - 1; intIdx++)
        {
            strReturn = (string)dsCheck.Tables[0].Rows[intIdx]["IMEI"];

        }

        return strReturn;

    }
}

