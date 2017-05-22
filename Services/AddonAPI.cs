using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Extensions.MonoHttp;
using SofttrendsAddon.Extensions;
using SofttrendsAddon.Helpers;
using SofttrendsAddon.Models;
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
    public static class AddonAPI
    {
        private static IRequestFactory mFactory { get; set; }

        static AddonAPI()
        {
            mFactory = new RequestFactory();
        }

        public static List<Categories> GetCategoryInfo(string id)
        {
            var request = mFactory.CreateRequest();
            request.Resource = "/getpubliccategories/" + id;
            request.Method = Method.GET;
            request.AddHeader("content-type", "application/json");
            var blocker = new AutoResetEvent(false);

            IRestResponse httpResponse = null;
            var client = mFactory.CreateClient();
            client.BaseUrl = new Uri(ConfigVars.Instance.AddonAPIUrl + "home");
            client.ExecuteAsync(request, response =>
            {
                httpResponse = response;
                blocker.Set();
            });
            blocker.WaitOne();

            if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
            {
                return (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<List<Categories>>(httpResponse);
            }
            return null;
        }

        public static SofttrendsAddon.Models.Resources GetResource(string id)
        {
            var request = mFactory.CreateRequest();
            request.Resource = "/getresource/" + id;
            request.Method = Method.GET;
            request.AddHeader("content-type", "application/json");
            var blocker = new AutoResetEvent(false);

            IRestResponse httpResponse = null;
            var client = mFactory.CreateClient();
            client.BaseUrl = new Uri(ConfigVars.Instance.AddonAPIUrl + "home");
            client.ExecuteAsync(request, response =>
            {
                httpResponse = response;
                blocker.Set();
            });
            blocker.WaitOne();

            if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
            {
                return (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<SofttrendsAddon.Models.Resources>(httpResponse);
            }
            return null;
        }

        public static List<Applications> GetAppInfo(string id)
        {
            var request = mFactory.CreateRequest();
            request.Resource = "/getpublicapps/" + id;
            request.Method = Method.GET;
            request.AddHeader("content-type", "application/json");
            var blocker = new AutoResetEvent(false);

            IRestResponse httpResponse = null;
            var client = mFactory.CreateClient();
            client.BaseUrl = new Uri(ConfigVars.Instance.AddonAPIUrl + "home");
            client.ExecuteAsync(request, response =>
            {
                httpResponse = response;
                blocker.Set();
            });
            blocker.WaitOne();

            if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
            {
                return (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<List<Applications>>(httpResponse);
            }
            return null;
        }

        public static Applications GetAppDetail(string id)
        {
            var request = mFactory.CreateRequest();
            request.Resource = "/getappdetail/" + id;
            request.Method = Method.GET;
            request.AddHeader("content-type", "application/json");
            var blocker = new AutoResetEvent(false);

            IRestResponse httpResponse = null;
            var client = mFactory.CreateClient();
            client.BaseUrl = new Uri(ConfigVars.Instance.AddonAPIUrl + "home");
            client.ExecuteAsync(request, response =>
            {
                httpResponse = response;
                blocker.Set();
            });
            blocker.WaitOne();

            if (httpResponse != null && (httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Created))
            {
                return (new RestSharp.Deserializers.JsonDeserializer()).Deserialize<Applications>(httpResponse);
            }
            return null;
        }

    }
}
