

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace SofttrendsAddon.ViewModels
{
    public class Document
    {
        public string tags { get; set; }
        public string apptype { get; set; }

        public string appaccess { get; set; }

        [JsonIgnore]
        public string appName { get; set; }


    }
    public class FileDetails
    {
        [JsonIgnore]
        public string appName { get; set; }
        public string fileUrl { get; set; }

        public string filename { get; set; }

        public string description { get; set; }

        public DateTime date { get; set; }
    }
    
}
