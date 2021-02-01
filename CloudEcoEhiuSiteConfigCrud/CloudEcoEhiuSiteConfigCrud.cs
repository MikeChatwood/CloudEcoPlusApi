using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CloudEcoEhiuSiteConfigCrud
{

    public class tInput
    {
        public string Action { get; set; } = null;
        public string UserName { get; set; } = null;
        public int EhiuSiteConfigID { get; set; } = -1;
        public int SiteID { get; set; } = -1;
        public string Name { get; set; } = null;
        public string Descr { get; set; } = null;
        public bool? Retired { get; set; } = null;

    };

    public class tResult
    {
        public bool Ok { get; set; } = true;
        public int Result { get; set; } = -1;
        public string Info { get; set; } = "";

    }
    public class CloudEcoEhiuSiteConfigCrud
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            return input?.ToUpper();
        }
    }
}
