using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using PromocaoAgora.Models;


namespace PromocaoAgora.Controllers
{
    public class EmailTemplateController : Controller
    {
        string ActualHost = ConfigurationManager.AppSettings["actualhost"];
        private promocaoagoraEntities db = new promocaoagoraEntities();
        Framework F = new Framework();

        // /EmailTemplate/EnviaEmail?_template=EmktBasico&_email=mmatiaso@gmail.com&_assunto=Teste%20de%20Envio
        public ActionResult EnviaEmail(string _template, string _email, string _assunto, string _nome)
        {
            string urltemplate = ActualHost + "/emailtemplate/"+ _template;
            string envio = F.SendMail("", _email, _assunto, urltemplate, true, _nome);
            ViewData["resposta"] = envio;
            return View();
        }

        

        //---------------------------------TEMPLATES----------------------------------------------------

        // GET: EmailTemplate
        public ActionResult BemVindo()
        {
            ViewData["actualhost"] = "https://www.promocaoagora.com.br";// ActualHost;
            return View();
        }

        public ActionResult EmktBasico(string campanha = "")
        {
            ViewData["actualhost"] = "https://www.promocaoagora.com.br";// ActualHost;
            ViewData["campanha"] = campanha;
            return View();
        }
    }
}