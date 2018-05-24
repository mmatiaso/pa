using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PromocaoAgora.Models;

namespace PromocaoAgora.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.Title = "Home Page";
            Response.Redirect("/site/");
            return View();
        }

        //----/home/stat?p=000&imp=0&vp=1&vc=1
        public ActionResult Stat(Guid p, bool imp, bool vp, bool vc)
        {
            //ViewBag.Title = "Home Page";
            //Response.Redirect("/site/");
            Statistic S = new Statistic();
            S.DoStat(p, imp, vp, vc);
            return View();
        }
    }
}
