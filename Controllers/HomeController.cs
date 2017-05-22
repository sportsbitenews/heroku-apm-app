using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SofttrendsAddon.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using SofttrendsAddon.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading;
using SofttrendsAddon.Helpers;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using SofttrendsAddon.Models;

namespace SofttrendsAddon.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index(string id)
        {
            try
            {
                string resourceId = ConfigVars.Instance.ResourceId;
                Console.WriteLine("resource id :" + id);
                if (!string.IsNullOrEmpty(resourceId))
                {
                    Resources resource = AddonAPI.GetResource(resourceId);
                    Console.WriteLine("resource  :" + JsonConvert.SerializeObject(resource));
                    if (resource != null && !string.IsNullOrEmpty(resource.uuid))
                    {
                        List<Categories> result = AddonAPI.GetCategoryInfo(resourceId);
                        Console.WriteLine("Categories :" + JsonConvert.SerializeObject(result));
                        if (result != null && result.Count > 0)
                        {
                            ViewBag.Categories = result;
                            ViewBag.AlertMessage = null;
                        }
                    }
                    else
                    {
                        ViewBag.Categories = null;
                        ViewBag.AlertMessage = "Portfolio Manager is De-provisioned from your Heroku account. Please delete this App from your account as it will not work when it is not linked to a Portfolio Manager add-on.";
                    }
                    return View();
                }
                else
                {
                    ViewBag.Categories = null;
                    ViewBag.AlertMessage = "Portfolio Manager is De-provisioned from your Heroku account. Please delete this App from your account as it will not work when it is not linked to a Portfolio Manager add-on.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Categories = null;
                ViewBag.AlertMessage = null;
                _logger.LogError(ex.Message, ex);
            }
            return View();
        }

        public JsonResult GetPublicApps(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError });
                }
                else
                {
                    List<Applications> applications = AddonAPI.GetAppInfo(id);
                    if (applications != null && applications.Count > 0)
                    {
                        return Json(new { Status = (int)HttpStatusCode.OK, apps = applications });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.NoContent });
                    }

                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError });
            }

        }

        public JsonResult GetAppDetail(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError });
                }
                else
                {
                    Applications app = AddonAPI.GetAppDetail(id);
                    if (app != null)
                    {
                        return Json(new { Status = (int)HttpStatusCode.OK, apps = app });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.NoContent });
                    }

                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError });
            }

        }


        [AllowAnonymous]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = feature?.Error;
            return View(new ErrorViewModel() { Code = HttpStatusCode.InternalServerError, Message = exception.Message });
        }


    }
}
