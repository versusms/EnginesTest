using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnginesTest.Base;
using EnginesTest.Models;
using System.Net;
using System.Xml;
using Newtonsoft.Json;
using System.IO;

namespace EnginesTest.Controllers
{
    [Authorize(Roles = "user")]
    public class HomeController : BaseController
    {
        /**
         * Отображение главной страницы
         */
        public ActionResult Index()
        {
            User CurrentUser = SecurityManager.GetUserInfo(HttpContext.User.Identity.Name);
            ViewBag.UserIdentity = CurrentUser.FirstName + " "  + (CurrentUser.MiddleName != null ? " " + CurrentUser.MiddleName : "") + CurrentUser.LastName;
            ViewBag.CurrentUser = JsonConvert.SerializeObject(new {
                Id = CurrentUser.Id,
                UserIdentity = ViewBag.UserIdentity,
                JobTitle = CurrentUser.JobTitle.Title,
                STV = CurrentUser.SecurityTV,
                STM = CurrentUser.SecurityTM,
                SUM = CurrentUser.SecurityUM
            });
            ViewBag.STV = CurrentUser.SecurityTV;
            ViewBag.SUM = CurrentUser.SecurityUM;
            ViewBag.UserProfileImage = CurrentUser.ProfileImage;
            ViewBag.EnginesCount = DataBase.Engines.Count();
            ViewBag.TestsCount = DataBase.Tests.Count();
            return View();
        }

        /**
         * Прокси для получения RSS со стороннего сервера
         */
        public ApiResult RSSProxy(String url)
        {
            WebClient client = new WebClient();
            ApiResult result = new ApiResult(); 
            client.Encoding = System.Text.Encoding.UTF8;
            XmlDocument doc = new XmlDocument();
            string content = "";

            try
            {
                content = client.DownloadString(url);
                doc.LoadXml(content);
                result.Success = true;
                result.DataSet = JsonConvert.SerializeXmlNode(doc);
                StreamWriter rssfile = new StreamWriter(HttpRuntime.AppDomainAppPath + "/App_Data/rss.xml", true);
                rssfile.Write(content);
                rssfile.Close();
            }
            catch (Exception re)
            {
                try
                {
                    StreamReader rssfile = new StreamReader(HttpRuntime.AppDomainAppPath + "/App_Data/rss.xml", true);
                    content = rssfile.ReadToEnd();
                    doc.LoadXml(content);
                    result.Success = true;
                    result.DataSet = JsonConvert.SerializeXmlNode(doc);
                }
                catch (Exception fe)
                {
                    result.Success = false;
                    result.Message = "Can not load data from " + url;
                }
            }

            return result;
        }
    }
}
