using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SofttrendsAddon.Services;
using SofttrendsAddon.ViewModels;
using System.Threading.Tasks;

namespace SofttrendsAddon.ViewComponents
{
    public class AccountViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            AccountInfo accInfo = null;
            var herokuAuthToken = HttpContext.Session.GetString("herokuAuthToken");
            if (!string.IsNullOrEmpty(herokuAuthToken))
            {
                accInfo = HerokuApi.GetAccountInfo(herokuAuthToken);
            }

            return View("~/Components/Account/Index.cshtml", accInfo);
        }
    }
}
