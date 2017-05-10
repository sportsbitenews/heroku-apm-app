using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Extensions.MonoHttp;
using SofttrendsAddon.Extensions;
using SofttrendsAddon.Helpers;
using SofttrendsAddon.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SofttrendsAddon.Services
{
    public static class HerokuApi
    {
        private static IRequestFactory mFactory { get; set; }

        static HerokuApi()
        {
            mFactory = new RequestFactory();
        }

        public static AccountInfo GetAccountInfo(string herokuAuthToken)
        {
            AccountInfo accountInfo = null;
            var request = mFactory.CreateRequest();
            request.Resource = "/account";
            request.Method = Method.GET;
            request.AddHeader("Authorization", "Bearer " + herokuAuthToken);
            request.AddHeader("Accept", "application/vnd.heroku+json;version=3");
            var blocker = new AutoResetEvent(false);

            IRestResponse httpResponse = null;
            var client = mFactory.CreateClient();
            client.BaseUrl = new Uri(ConfigVars.Instance.HerokuApiUrl);
            client.ExecuteAsync(request, response =>
            {
                httpResponse = response;
                blocker.Set();
            });
            blocker.WaitOne();

            if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
            {
                accountInfo = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<AccountInfo>(httpResponse);
            }
            return accountInfo;
        }
        public static dynamic GetAddonInfo(string appName, string herokuAuthToken, string addonName)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.Unauthorized;
            HerokuAddon addonInfo = null;

            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("apps/{0}/addons", appName);
                request.Method = Method.GET;
                request.AddHeader("Authorization", "Bearer " + herokuAuthToken);
                request.AddHeader("Accept", "application/vnd.heroku+json;version=3");
                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri(ConfigVars.Instance.HerokuApiUrl);
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created)
                    {
                        var addons = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<List<HerokuAddon>>(httpResponse);
                        if (addons != null && addons.Where(p => p.name.ToLower().Contains(ConfigVars.Instance.heroku_username.ToLower())).Count() > 0)
                        {
                            httpStatusCode = HttpStatusCode.Found;
                            addonInfo = addons.Where(p => p.name.ToLower().Contains(ConfigVars.Instance.heroku_username.ToLower())).FirstOrDefault();
                        }
                    }
                    else
                    {
                        httpStatusCode = httpResponse.StatusCode;
                    }
                }
                else
                {
                    httpStatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new { HttpStatusCode = (int)httpStatusCode, Addon = addonInfo };
        }
        public static AppInfo GetAppInfo(string name, string herokuAuthToken)
        {
            AppInfo appInfo = null;

            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("apps/{0}", name);
                request.Method = Method.GET;
                request.AddHeader("Authorization", "Bearer " + herokuAuthToken);
                request.AddHeader("Accept", "application/vnd.heroku+json;version=3");
                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri(ConfigVars.Instance.HerokuApiUrl);
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    appInfo = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<AppInfo>(httpResponse);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return appInfo;
        }
        public static List<AppInfo> GetAppList(string herokuAuthToken)
        {
            List<AppInfo> appList = null;
            var request = mFactory.CreateRequest();
            request.Resource = "/apps";
            request.Method = Method.GET;
            request.AddHeader("Authorization", "Bearer " + herokuAuthToken);
            request.AddHeader("Accept", "application/vnd.heroku+json;version=3");
            var blocker = new AutoResetEvent(false);

            IRestResponse httpResponse = null;
            var client = mFactory.CreateClient();
            client.BaseUrl = new Uri(ConfigVars.Instance.HerokuApiUrl);
            client.ExecuteAsync(request, response =>
            {
                httpResponse = response;
                blocker.Set();
            });
            blocker.WaitOne();
            if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
            {
                appList = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<List<AppInfo>>(httpResponse);
            }

            return appList;
        }

        public static string GetToken(HttpContext context)
        {
            mFactory = new RequestFactory();
            var request = mFactory.CreateRequest();
            request.Resource = "/oauth/authorize";
            ConfigVars.Instance.RedirectUri = string.Format("http://localhost:{0}/login/oauthcallback", context.Request.Host.Port);

            request.AddObject(new
            {
                response_type = "code",
                client_id = ConfigVars.Instance.ClientId,
                redirect_uri = ConfigVars.Instance.RedirectUri,
            });

            var client = mFactory.CreateClient();
            client.BaseUrl = ConfigVars.Instance.AuthServerBaseUrl;
            return client.BuildUri(request).ToString();
        }

        public static async Task<HerokuAuthToken> QueryAccessToken(IQueryCollection parameters)
        {
            HerokuAuthToken authToken = null;
            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = "/oauth/token";
                request.Method = Method.POST;
                request.AddObject(new
                {
                    code = parameters["code"],
                    client_id = ConfigVars.Instance.ClientId,
                    client_secret = ConfigVars.Instance.ClientSecret,
                    redirect_uri = ConfigVars.Instance.RedirectUri,
                    grant_type = "authorization_code"
                });

                var client = mFactory.CreateClient();
                client.BaseUrl = ConfigVars.Instance.AuthServerBaseUrl;
                var httpResponse = await client.RestExecuteAsync(request);
                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    authToken = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<HerokuAuthToken>(httpResponse);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return authToken;
        }

        private static string ParseTokenResponse(string content, string key)
        {
            if (String.IsNullOrEmpty(content) || String.IsNullOrEmpty(key))
                return null;

            try
            {
                // response can be sent in JSON format
                var token = JObject.Parse(content).SelectToken(key);
                return token != null ? token.ToString() : null;
            }
            catch (JsonReaderException)
            {
                // or it can be in "query string" format (param1=val1&param2=val2)
                var collection = HttpUtility.ParseQueryString(content);
                return collection[key];
            }
        }

        public static string GetHerokuAppLogUrl(bool isconnectorlog, string appName, string herokuAuthToken)
        {
            IDictionary<string, string> appLogInfo = null;
            string appLogs = string.Empty;
            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("apps/{0}/log-sessions", appName);
                request.Method = Method.POST;
                request.AddHeader("Authorization", "Bearer " + herokuAuthToken);
                request.AddHeader("Accept", "application/vnd.heroku+json;version=3");
                if (isconnectorlog)
                {
                    request.RequestFormat = DataFormat.Json;
                    var reqBody = new
                    {
                        dyno = "web",
                        lines = 100,
                        source = "app",
                        tail = false
                    };
                    request.AddBody(reqBody);
                }
                //request.AddBody(JsonConvert.SerializeObject(reqBody));
                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri(ConfigVars.Instance.HerokuApiUrl);
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    appLogInfo = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<Dictionary<string, string>>(httpResponse);
                    if (appLogInfo != null && appLogInfo.Count > 0 && !string.IsNullOrEmpty(appLogInfo["logplex_url"]))
                    {
                        appLogs = appLogInfo["logplex_url"];

                        //var requestlog = mFactory.CreateRequest();
                        //requestlog.Method = Method.GET;
                        //var blockerlog = new AutoResetEvent(false);

                        //IRestResponse httpResponselog = null;
                        //var clientlog = mFactory.CreateClient();
                        //clientlog.BaseUrl = new Uri(appLogInfo["logplex_url"].ToString());
                        //clientlog.ExecuteAsync(requestlog, response =>
                        //{
                        //    httpResponselog = response;
                        //    blocker.Set();
                        //});
                        //blocker.WaitOne();
                        //if (httpResponselog != null && (httpResponselog.StatusCode == HttpStatusCode.OK || httpResponselog.StatusCode == HttpStatusCode.Created))
                        //{
                        //    appLogs = httpResponselog.Content.ToString();
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return appLogs;
        }

        public static string GetAccessCollabrators(string appName, string herokuAuthToken)
        {
            List<Dictionary<string, string>> appInfo = null;
            string appCollabratorInfo = string.Empty;
            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("apps/{0}/collaborators ", appName);
                request.Method = Method.GET;
                request.AddHeader("Authorization", "Bearer " + herokuAuthToken);
                request.AddHeader("Accept", "application/vnd.heroku+json;version=3");

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri(ConfigVars.Instance.HerokuApiUrl);
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    appInfo = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<List<Dictionary<string, string>>>(httpResponse);
                    if (appInfo != null && appInfo.Count > 0)
                    {
                        //appCollabratorInfo = ;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return appCollabratorInfo;
        }

        public static List<Dictionary<string, string>> GetReleases(string appName, string herokuAuthToken)
        {
            List<Dictionary<string, string>> releaseInfo = null;

            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("apps/{0}/releases ", appName);
                request.Method = Method.GET;
                request.AddHeader("Authorization", "Bearer " + herokuAuthToken);
                request.AddHeader("Accept", "application/vnd.heroku+json;version=3");

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri(ConfigVars.Instance.HerokuApiUrl);
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    releaseInfo = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<List<Dictionary<string, string>>>(httpResponse);


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return releaseInfo;
        }

        public static List<Dictionary<string, string>> GetDynos(string appName, string herokuAuthToken)
        {
            List<Dictionary<string, string>> dynoInfo = null;

            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("apps/{0}/dynos ", appName);
                request.Method = Method.GET;
                request.AddHeader("Authorization", "Bearer " + herokuAuthToken);
                request.AddHeader("Accept", "application/vnd.heroku+json;version=3");

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri(ConfigVars.Instance.HerokuApiUrl);
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    dynoInfo = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<List<Dictionary<string, string>>>(httpResponse);


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dynoInfo;
        }

        public static List<Dictionary<string, string>> GetMembers(string organizationId, string herokuAuthToken)
        {
            List<Dictionary<string, string>> membersInfo = null;

            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("organizations/{0}/members ", organizationId);
                request.Method = Method.GET;
                request.AddHeader("Authorization", "Bearer " + herokuAuthToken);
                request.AddHeader("Accept", "application/vnd.heroku+json;version=3");

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri(ConfigVars.Instance.HerokuApiUrl);
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    membersInfo = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<List<Dictionary<string, string>>>(httpResponse);


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return membersInfo;
        }

        public static List<Dictionary<string, string>> GetCollabrators(string appname, string herokuAuthToken)
        {
            List<Dictionary<string, string>> membersInfo = null;

            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("apps/{0}/collaborators ", appname);
                request.Method = Method.GET;
                request.AddHeader("Authorization", "Bearer " + herokuAuthToken);
                request.AddHeader("Accept", "application/vnd.heroku+json;version=3");

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri(ConfigVars.Instance.HerokuApiUrl);
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    membersInfo = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<List<Dictionary<string, string>>>(httpResponse);


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return membersInfo;
        }

        public static List<Dictionary<string, string>> GetAddons(string appName, string herokuAuthToken)
        {
            List<Dictionary<string, string>> appInfo = null;

            try
            {
                var request = mFactory.CreateRequest();
                request.Resource = string.Format("apps/{0}/addons ", appName);
                request.Method = Method.GET;
                request.AddHeader("Authorization", "Bearer " + herokuAuthToken);
                request.AddHeader("Accept", "application/vnd.heroku+json;version=3");

                var blocker = new AutoResetEvent(false);

                IRestResponse httpResponse = null;
                var client = mFactory.CreateClient();
                client.BaseUrl = new Uri(ConfigVars.Instance.HerokuApiUrl);
                client.ExecuteAsync(request, response =>
                {
                    httpResponse = response;
                    blocker.Set();
                });
                blocker.WaitOne();

                if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
                {
                    appInfo = (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<List<Dictionary<string, string>>>(httpResponse);


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return appInfo;
        }
        public static string GetHerokuAppLogs(string log_plex_url)
        {
            string appLogs = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(log_plex_url))
                {
                    var requestlog = mFactory.CreateRequest();
                    requestlog.Method = Method.GET;
                    var blockerlog = new AutoResetEvent(false);

                    IRestResponse httpResponselog = null;
                    var clientlog = mFactory.CreateClient();
                    clientlog.BaseUrl = new Uri(log_plex_url);
                    clientlog.ExecuteAsync(requestlog, response =>
                    {
                        httpResponselog = response;
                        blockerlog.Set();
                    });
                    blockerlog.WaitOne();
                    if (httpResponselog != null && (httpResponselog.StatusCode == HttpStatusCode.OK || httpResponselog.StatusCode == HttpStatusCode.Created))
                    {
                        appLogs = httpResponselog.Content.ToString();
                        appLogs = Regex.Replace(appLogs, @"\r\n?|\n", "<br>");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return appLogs;
        }

       


    }
}
