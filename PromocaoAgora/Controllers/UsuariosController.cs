using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PromocaoAgora.Models;
using System.Configuration;

namespace PromocaoAgora.Controllers
{
    public class UsuariosController : ApiController
    {
        private promocaoagoraEntities db = new promocaoagoraEntities();
        public Framework F = new Framework();
        public Modelos M = new Modelos();
        string ActualHost = ConfigurationManager.AppSettings["actualhost"];

        

        // GET: api/Usuarios
        public IQueryable<Usuario> GetUsuarios(int _take = 10, int _skip = 0, string _status = "", bool? _ativo = null, string _UF = "", string _cid = "", string _bai = "")
        {
            IQueryable<Usuario> IUsuarios = db.Usuarios;
            if(!string.IsNullOrEmpty(_status))
            {
                IUsuarios = IUsuarios.Where(x => x.Status == _status);
            }
            if (_ativo != null)
            {
                IUsuarios = IUsuarios.Where(x => x.Ativo == _ativo);
            }
            if (!string.IsNullOrEmpty(_UF))
            {
                IUsuarios = IUsuarios.Where(x => x.UF == _UF);
            }
            if (!string.IsNullOrEmpty(_cid))
            {
                IUsuarios = IUsuarios.Where(x => x.Cidade == _cid);
            }
            if (!string.IsNullOrEmpty(_bai))
            {
                IUsuarios = IUsuarios.Where(x => x.Bairro == _bai);
            }
            int iu = IUsuarios.Count();
            return IUsuarios.OrderByDescending(x => x.CriadoEm).Skip(_skip).Take(_take);
        }

        // GET: api/CountUsuarios
        [HttpGet]
        [Route("api/CountUsuarios")]
        public int CountUsuarios(string _status = "", bool? _ativo = null, string _UF = "", string _cid = "", string _bai = "")
        {
            IQueryable<Usuario> IUsuarios = db.Usuarios;
            if (!string.IsNullOrEmpty(_status))
            {
                IUsuarios = IUsuarios.Where(x => x.Status == _status);
            }
            if (_ativo != null)
            {
                IUsuarios = IUsuarios.Where(x => x.Ativo == _ativo);
            }
            if (!string.IsNullOrEmpty(_UF))
            {
                IUsuarios = IUsuarios.Where(x => x.UF == _UF);
            }
            if (!string.IsNullOrEmpty(_cid))
            {
                IUsuarios = IUsuarios.Where(x => x.Cidade == _cid);
            }
            if (!string.IsNullOrEmpty(_bai))
            {
                IUsuarios = IUsuarios.Where(x => x.Bairro == _bai);
            }
            return IUsuarios.Count();
        }

        // GET: api/SendEmailUsuarios
        [HttpPost]
        [Route("api/SendEmailUsuarios")]
        public int SendEmailUsuarios(SendEmailUsuarioReq Sr)
        {
            int totalEnvios = 0;

            //cria a camapnha
            Campanha C = new Campanha();
            C.Nome = Sr.campanha;
            C = M.MantemCampanha(C);

            string urltemplate = ActualHost + "/emailtemplate/" + Sr.template;
            IQueryable<Usuario> IUsuarios = GetUsuarios(100, 0, Sr.status, null, Sr.UF, Sr.cid, Sr.bai);
            foreach(Usuario u in IUsuarios.ToList())
            {
                try
                {
                    if (F.SendMail("", u.Email, Sr.assunto, urltemplate, true, u.Nome) == "OK")
                    {
                        totalEnvios++;
                        //atualiza CamapanhasEnviadas
                        Usuario U = new Usuario();
                        U = u;
                        U.CampanhasEnviadas += Sr.campanha + "|";
                        U = M.MantemUsuario(U);
                        

                    }
                }
                catch(Exception e)
                {
                    //
                }
                
            }
            //atualiza camapnha
            try
            {

                C.EmailEnvios = totalEnvios;
                C = M.MantemCampanha(C);
            }
            catch (Exception e)
            {
                //
            }
            return totalEnvios;
        }

        // GET:api/UFUsuarios
        [HttpGet]
        [Route("api/UFUsuarios")]
        public IEnumerable<String> UFUsuarios()
        {
            IQueryable<String> Ufs = db.Usuarios.GroupBy(c => c.UF, (key, c) => c.FirstOrDefault()).Select(x => x.UF).Distinct();
            //List<string> IUF = db.Usuarios.GroupBy(x => x.UF).Distinct();
            Ufs = Ufs.Where(x => x != "" && x != null);
            return Ufs;
        }

        // GET:api/CidadeUsuarios
        [HttpGet]
        [Route("api/CidadeUsuarios")]
        public IEnumerable<String> CidadeUsuarios(string _uf)
        {
            IQueryable<String> Cids = db.Usuarios.Where(x => x.UF == _uf).GroupBy(c => c.Cidade, (key, c) => c.FirstOrDefault()).Select(x => x.Cidade).Distinct();

            Cids = Cids.Where(x => x != "" && x != null).OrderBy(x => x);
            return Cids;
        }

        // GET:api/CidadeUsuarios
        [HttpGet]
        [Route("api/BairroUsuarios")]
        public IEnumerable<String> BairroUsuarios(string _cid)
        {
            IQueryable<String> Bai = db.Usuarios.Where(x => x.Cidade == _cid).GroupBy(c => c.Bairro, (key, c) => c.FirstOrDefault()).Select(x => x.Bairro).Distinct();

            Bai = Bai.Where(x => x != "" && x != null);
            return Bai;
        }

        // GET: api/Usuarios/5
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult GetUsuario(Guid id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        public ValidaForm ValidaUsuario(Usuario usuario)
        {
            ValidaForm validaForm = new ValidaForm();
            validaForm.Valido = true;
            validaForm.Erro = "";
            if (string.IsNullOrEmpty(usuario.Nome))
            {
                validaForm.Valido = false;
                validaForm.Erro = "O preenchimento do Nome é obrigatório.";
            }
            if (string.IsNullOrEmpty(usuario.Email))
            {
                validaForm.Valido = false;
                validaForm.Erro = "O preenchimento do Email é obrigatório.";
            }
            
            return validaForm;
        }

        // PUT: api/Usuarios/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUsuario(Guid id, Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != usuario.UsuarioId)
            {
                return BadRequest();
            }

            db.Entry(usuario).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            return Ok("OK");//StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Usuarios
        
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult PostUsuario(Usuario usuario)
        {
            //Usuario usuario = new Usuario();
            //usuario.Nome = formext.Nome;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            usuario.CriadoEm = DateTime.Now;
            usuario.AtualizadoEm = DateTime.Now;
            db.Usuarios.Add(usuario);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (UsuarioExists(usuario.UsuarioId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            string envio = F.SendMail("", usuario.Email, "Confirme seu Cadastro", "Olá " + usuario.Nome + ", para confirmar o seu cadastro, clique no link abaixo: <a href='http://localhost:61602/site/confirmaemail?email=" + usuario.Email + "'> CONFIRMAR MEU CADASTRO</a>");
            return Ok(envio);//CreatedAtRoute("DefaultApi", new { id = usuario.UsuarioId }, usuario);
        }

        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("api/CriaUsuario")]
        public Retorno CriaUsuario(Usuario usuario)
        {
            Retorno r = new Retorno();
            ValidaForm vlform = ValidaUsuario(usuario);
            bool emailexiste = false;
            emailexiste = db.Usuarios.Where(x => x.Email == usuario.Email).Any();
            if (emailexiste)
            {
                r.responseStatus = "NOK";
                r.desc = "Email já existente.";
                return r;
            }
            if (!vlform.Valido)
            {
                r.responseStatus = "NOK";
                r.desc = vlform.Erro;
                return r;
            }
            else
            {

                usuario.CriadoEm = DateTime.Now;
                usuario.AtualizadoEm = DateTime.Now;
                db.Usuarios.Add(usuario);

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (!UsuarioExists(usuario.UsuarioId))
                    {
                        r.responseStatus = "NOK";
                        r.desc = "Usuário não existe.";
                        return r;
                    }
                    else
                    {
                        r.responseStatus = "NOK";
                        r.desc = "Algum erro ocorreu. Tente mais tarde.";
                        return r;
                    }
                }
                try
                {
                    string urltemplate = ActualHost + "/emailtemplate/bemvindo";
                    string envio = F.SendMail("", usuario.Email, "Confirme seu Cadastro", urltemplate, true, usuario.Nome);
                    if (envio == "OK")
                    {
                        r.responseStatus = "OK";
                    }
                    else
                    {
                        r.responseStatus = "NOK";
                        r.desc = envio;
                    }
                }
                catch (Exception e)
                {
                    r.responseStatus = "NOK";
                    r.desc = e.ToString();
                }
                return r;
            }
         }

        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("api/UpdateUsuario")]
        public Retorno UpdateUsuario(Usuario usuario)
        {
            Retorno r = new Retorno();
            ValidaForm vlform = ValidaUsuario(usuario);
            if (!vlform.Valido)
            {
                r.responseStatus = "NOK";
                r.desc = vlform.Erro;
                return r;
            }
            else
            {

                try
                {
                    usuario.UsuarioId = new Guid(F.read_cookie("uid"));
                    Usuario _usuario = db.Usuarios.Find(usuario.UsuarioId);
                    
                    if (_usuario != null)
                    {
                        //_usuario.Ativo = usuario.Ativo;
                        _usuario.AtualizadoEm = DateTime.Now;
                        _usuario.Nome = usuario.Nome;
                        _usuario.Email = usuario.Email;
                        if (!string.IsNullOrEmpty(usuario.Senha))
                        {
                            _usuario.Senha = usuario.Senha;
                        }

                        db.Entry(_usuario).State = EntityState.Modified;
                        db.SaveChanges();
                        r.responseStatus = "OK";
                        r.desc = "Usuário alterado com sucesso.";
                        F.write_cookie("loggedName", _usuario.Nome, 360);
                        return r;
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioId))
                    {
                        r.responseStatus = "NOK";
                        r.desc = "Usuário não existe.";
                        return r;
                    }
                    else
                    {
                        r.responseStatus = "NOK";
                        r.desc = "Algum erro ocorreu. Tente mais tarde.";
                        return r;
                    }
                }
            }

            return r;
        }

        // DELETE: api/Usuarios/5
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult DeleteUsuario(Guid id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            db.Usuarios.Remove(usuario);
            db.SaveChanges();

            return Ok(usuario);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        private bool UsuarioExists(Guid id)
        {
            return db.Usuarios.Count(e => e.UsuarioId == id) > 0;
        }

        [Route("usuarios/confirmaemail")]
        [HttpGet]
        public string ConfirmaEmail(string email)
        {
            if(db.Usuarios.Where(x => x.Email == email).Any())
            {
                Usuario u = db.Usuarios.Where(x => x.Email == email).First();
                u.Ativo = true;
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();
                return "Email " + email + " Confirmado!";
            }
            else
            {
                return "Email inexistente.";
            }
        }

        [HttpPost]
        [Route("api/login")]
        [AcceptVerbs("POST")]
        public Retorno Postlogin(LoginInfo login)
        {
            Retorno retorno = new Retorno();
            if (db.Usuarios.Where(x => x.Email == login.email && x.Senha == login.pass).Any())
            { 
                if (db.Usuarios.Where(x => x.Email == login.email && x.Senha == login.pass && x.Ativo == true).Any())
                {
                    Usuario u = new Usuario();
                    u = db.Usuarios.Where(x => x.Email == login.email && x.Senha == login.pass).First();
                    retorno.guid = u.UsuarioId;
                    retorno.responseStatus = "OK";
                    //retorno.desc = login.redirecturl;
                    F.write_cookie("uid", u.UsuarioId.ToString(), 360);
                    F.write_cookie("logged", "true", 360);
                    F.write_cookie("loggedName", u.Nome, 360);
                    if (u.UsuarioEmpresas.Any())
                    {
                        F.write_cookie("eid", u.UsuarioEmpresas.First().EmpresaId.ToString(), 360);
                    }

                    return retorno;
                }
                else
                {
                    retorno.responseStatus = "NOTCONFIRMED";
                    return retorno;
                    
                }
                
            }
            else
            {
                retorno.responseStatus = "Email ou senha inválidos";
                return retorno;
                
                
            }

            
        }

        [HttpGet]
        [Route("api/islogged")]
        public bool Islogged(string uid)
        {
            try
            {
                Guid u = new Guid(uid);
                if (db.Usuarios.Where(x => x.UsuarioId == u).Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }catch(Exception e)
            {
                return false;
            }
        }

        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("api/EnviaEmail")]
        public Retorno EnviaEmail(EmailEstrutura ems)
        {
            Retorno r = new Retorno();
            ems.Descricao = "<p>Email do usuário: " + ems.From + "</p>" + ems.Descricao;
            string ret_sendmail = F.SendMail(ems.From, ems.Para, ems.Assunto, ems.Descricao);
            if (ret_sendmail == "OK")
            {
                r.responseStatus = "OK";
                r.desc = "Email enviado com sucesso.";
            }
            else
            {
                r.responseStatus = "NOK";
                r.desc = ret_sendmail;
            }
            return r;
        }




        }
}