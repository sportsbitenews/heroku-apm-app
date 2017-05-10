

using System.Collections.Generic;

namespace SofttrendsAddon.ViewModels
{
    public class AppInfo
    {
        public string archived_at { get; set; }
        public string buildpack_provided_description { get; set; }
        public string created_at { get; set; }
        public string id { get; set; }
        public string git_url { get; set; }
        public string maintenance { get; set; }
        public string name { get; set; }
        public string space { get; set; }
        public string released_at { get; set; }
        public string repo_size { get; set; }
        public string slug_size { get; set; }
        public string updated_at { get; set; }
        public string web_url { get; set; }

        public AppOwner owner { get; set; }
        public AppRegion region { get; set; }
        public AppOrganization organization { get; set; }
        public AppStack stack { get; set; }

        public AppBuildStack build_stack { get; set; }

        public string tags{ get; set; }

        public string apptype { get; set; }

        public string appaccess { get; set; }

        public List<FileDetails> fileDetailsList { get; set; }
    }
}
