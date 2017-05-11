using System;

namespace SofttrendsAddon
{
    public sealed class ConfigVars
    {
        public string AddonAPIUrl = string.Empty;
        public string ResourceId = string.Empty;
        public string ClientId = string.Empty;
        public string ClientSecret = string.Empty;
        public Uri AuthServerBaseUrl = null;
        public string RedirectUri = string.Empty;
        public string HerokuApiUrl = string.Empty;
        public string AWSBucketName = string.Empty;
        public string AWSServiceUrl = string.Empty;
        public string AWSAccessKey = string.Empty;
        public string AWSSecretKey = string.Empty;
        public string addon_plan = string.Empty;
        public string heroku_username = string.Empty;

        private ConfigVars()
        {
            ResourceId = Environment.GetEnvironmentVariable("ResourceId");
            AddonAPIUrl = Environment.GetEnvironmentVariable("AddonAPIUrl");
            //ClientId = Environment.GetEnvironmentVariable("ClientId");
            //ClientSecret = Environment.GetEnvironmentVariable("ClientSecret");
            //if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AuthServerBaseUrl")))
            //    AuthServerBaseUrl = new Uri(Environment.GetEnvironmentVariable("AuthServerBaseUrl"));
            //RedirectUri = Environment.GetEnvironmentVariable("RedirectUri");
            //HerokuApiUrl = Environment.GetEnvironmentVariable("HerokuApiUrl");
            //AWSBucketName = Environment.GetEnvironmentVariable("AWSBucketName");
            //AWSServiceUrl = Environment.GetEnvironmentVariable("AWSServiceUrl");
            //AWSAccessKey = Environment.GetEnvironmentVariable("AWSAccessKey");
            //AWSSecretKey = Environment.GetEnvironmentVariable("AWSSecretKey");
            //addon_plan = Environment.GetEnvironmentVariable("Addon_Plan");
            //heroku_username = Environment.GetEnvironmentVariable("HEROKU_ADDON_ID");

            //for local testing
            //AddonAPIUrl = "https://portfoliomanager-addon.herokuapp.com/";
            //ResourceId = "7f1d3c09-0356-489d-b50a-4503ecd5dc4d";
            //ClientId = "2730a19c-951e-4681-bfa8-215f8488dfc5";
            //ClientSecret = "177204e6-f6b3-4251-9bdc-4b94b0eca5a3";
            //AuthServerBaseUrl = new Uri("https://id.heroku.com");
            //RedirectUri = "http://localhost:50786/oauthcallback";
            //HerokuApiUrl = "https://api.heroku.com";
            //AWSBucketName = "herokuapm";
            //AWSServiceUrl = "https://s3.amazonaws.com/";
            //AWSAccessKey = "AKIAJB6KT2EGZQ76Q7BA";
            //AWSSecretKey = "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";
            //addon_plan = "[{'plan_name':'test','expire_in_days':0,'expiry_message':'Using FREE Plan, upgrade to get more ','grace_period':0,'send_mail':true,'categories_limit':5,'upload_document':false,'application_limit_for_each_category':10},{'plan_name':'personal','expire_in_days':0,'expiry_message':'Using FREE Plan, upgrade to get more','grace_period':0,'send_mail':true,'categories_limit':5,'upload_document':true,'application_limit_for_each_category':10},{'plan_name':'professional','expire_in_days':30,'expiry_message':'Professional Plan expires','grace_period':3,'send_mail':true,'categories_limit':'','upload_document':true,'application_limit_for_each_category':''}]";
            //heroku_username = "portfoliomanager";
        }

        public static ConfigVars Instance { get { return ConfigVarInstance.Instance; } }

        private class ConfigVarInstance
        {
            static ConfigVarInstance()
            {
            }

            internal static readonly ConfigVars Instance = new ConfigVars();
        }
    }
}
