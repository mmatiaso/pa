using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PromocaoAgora.Models;
using System.Data.Entity;

namespace PromocaoAgora.Controllers
{
    public class SiteController : Controller
    {
        Framework F = new Framework();
        private promocaoagoraEntities db = new promocaoagoraEntities();
        // GET: /Site/
        public ActionResult Index()
        {
            return View();
        }
        // GET: /Site/Promocao
        [Route("p/{t}/{pid}")]
        public ActionResult Promocao(Guid pid, string t)
        {
            return View();
        }
        // GET: /Site/CadastroCliente
        public ActionResult CadastroCliente()
        {
            return View();
        }

        // GET: /Site/ConfirmaEmail
        public ActionResult ConfirmaEmail(string email)
        {
            string mensagem = "";
            string urlred = "/";
            if (db.Usuarios.Where(x => x.Email == email).Any())
            {
                Usuario u = db.Usuarios.Where(x => x.Email == email).First();
                u.Ativo = true;
                if (u.Status.Contains("CLIENTE"))
                {
                    urlred = "/site/cadastrocliente";
                }
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();
                mensagem = "Email <strong>" + email + "</strong> Confirmado!";
                
            }
            else
            {
                mensagem = "Email inexistente.";
            }
            ViewData["mens"] = mensagem;
            ViewData["redir"] = urlred;
            return View();
        }

        // GET: /Site/areacliente
        public ActionResult AreaCliente()
        {
            return View();
        }

        // GET: /Site/cadastropromocao
        public ActionResult CadastroPromocao(Guid uid)
        {
            Guid? p = db.UsuarioEmpresas.Where(x => x.Usuario.UsuarioId == uid).FirstOrDefault().EmpresaId;
            //Guid p = E.EmpresaId;
            return View(p);
        }

        // GET: /Site/EditaPromocao
        public ActionResult EditaPromocao(Guid pid)
        {
            Promocao p = db.Promocaos.Find(pid);
            return View(p);
        }

        // GET: /Site/EditaEmpresa
        public ActionResult EditaEmpresa(Guid eid)
        {
            Empresa e = db.Empresas.Find(eid);
            return View(e);
        }

        // GET: /Site/EditaUsuario
        public ActionResult EditaUsuario()
        {
            Guid uid = new Guid(F.read_cookie("uid"));
            Usuario u = db.Usuarios.Find(uid);
            return View(u);
        }

        // GET: /Site/CriaUsuario
        public ActionResult CriaUsuario(string returnUrl = "/")
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        // GET: /Site/login
        public ActionResult Login(string redirecturl = "", int popup = 0)
        {
            ViewData["pp"] = popup;
            return View();
        }

        

        public ActionResult Logout(string uid)
        {
            try
            {

                F.cookie_off("uid");
                F.cookie_off("eid");
                F.cookie_off("logged");
                F.cookie_off("loggedName");

            }
            catch (Exception e)
            {

            }
            Response.Redirect("/");
            return View();
        }

        public ActionResult Termos()
        {
            return View();
        }

        public ActionResult PoliticaPrivacidade()
        {
            return View();
        }

        public ActionResult QuemSomos()
        {
            return View();
        }

        public ActionResult FaleConosco()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult AdicionarTelaInicial()
        {
            return View();
        }
    }
}