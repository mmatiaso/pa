using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PromocaoAgora.Controllers
{
    public class SiteController : Controller
    {
        // GET: /Site/
        public ActionResult Index()
        {
            return View();
        }
        // GET: /Site/Promocao
        public ActionResult Promocao()
        {
            return View();
        }
        // GET: /Site/CadastroCliente
        public ActionResult CadastroCliente()
        {
            return View();
        }
    }
}