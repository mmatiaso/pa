using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using PromocaoAgora.Models;
using System.Data.Entity;
using System.Linq.Dynamic;

namespace PromocaoAgora.Controllers
{
    public class CRMController : Controller
    {
        private promocaoagoraEntities db = new promocaoagoraEntities();
        public Modelos M = new Modelos();
        string ActualHost = ConfigurationManager.AppSettings["actualhost"];
        // GET: CRM
        public ActionResult Index()
        {
            return View();
        }

        // GET: CRM
        public ActionResult Mailing()
        {
            return View();
        }

        // GET: CRM/usuarios
        public ActionResult usuarios()
        {
            List<Usuario> Lu = new List<Usuario>();
            Lu = db.Usuarios.OrderByDescending(x => x.CriadoEm).Take(50).ToList();

            return View(Lu);
        }

        // GET: CRM/Track
        public void Track(string _email = "", string _campanha = "", string _acao = "", string qurl = "", string _template = "")
        {
            if (_email != "" && _campanha != "" && _acao != "")
            {
                Usuario U = new Usuario();
                //atualizar campanha
                Campanha C = new Campanha();
                C.Nome = _campanha;
                //C = M.MantemCampanha(C);
                try
                {
                    U = db.Usuarios.Where(x => x.Email == _email).FirstOrDefault();
                    if (_acao == "OPEN")
                    {
                        U.CampanhasAbertas += " " + _campanha;
                        C.EmailAberturas += 1;
                    }
                    if (_acao == "CLICK")
                    {
                        U.CampanhasClicadas += " " + _campanha;
                        C.EmailClicks += 1;
                    }
                    db.Entry(U).State = EntityState.Modified;
                    db.SaveChanges();

                    
                    

                    C = M.MantemCampanha(C);


                    if (_acao == "CLICK")
                    {
                        string redir = ActualHost;
                        if (!string.IsNullOrEmpty(qurl)) { redir += "/" + qurl;  }
                        if (!string.IsNullOrEmpty(_template)) { redir += "?utm_source=" + _template; }
                        if (!string.IsNullOrEmpty(_campanha)) { redir += "&utm_campaign=" + _campanha; }
                        Response.Redirect(redir);
                    }

                }
                catch (Exception e)
                {
                    //
                }
            }
            //return View();
        }

        public ActionResult Campaign()
        {
            //List<Campanha> L = new List<Campanha>();
            //L = db.Campanhas.OrderByDescending(x => x.CriadoEm).Take(50).ToList();

            return View();
        }

        public ActionResult CampaignData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var busca = Request.Form.GetValues("search[value]").FirstOrDefault();
            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            db.Configuration.ProxyCreationEnabled = false;

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            //dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
            var v = db.Campanhas.Select(n => new {
                CampanhaId = n.CampanhaId,
                Nome = n.Nome,
                EmailAberturas = n.EmailAberturas,
                EmailClicks = n.EmailClicks,
                EmailEnvios = n.EmailEnvios,
                CriadoEm = n.CriadoEm,
                AtualizadoEm = n.AtualizadoEm
                
            });

            // SEARCH
            if (!(string.IsNullOrEmpty(busca)))
            {
                v = v.Where(a => a.CampanhaId.ToString() == busca || a.Nome.Contains(busca) ||
                    a.CriadoEm.ToString().Contains(busca) || a.AtualizadoEm.ToString().Contains(busca)
                );
            }

            //SORT

            if (!(string.IsNullOrEmpty(sortColumn)) && !(string.IsNullOrEmpty(sortColumnDir)))
            {
                string hlp = sortColumn + " " + sortColumnDir;
                v = v.OrderBy(hlp);
            }
            else
            {
                v = v.OrderBy("0 desc");
            }

            //List<clusters> ld = v.ToList();

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            //return data.ToString();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            //return View(db.clusters.ToList());
        }


    }
}