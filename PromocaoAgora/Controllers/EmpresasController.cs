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

namespace PromocaoAgora.Controllers
{
    public class EmpresasController : ApiController
    {
        private promocaoagoraEntities db = new promocaoagoraEntities();
        public Framework f = new Framework();

        // GET: api/Empresas
        public IQueryable<Empresa> GetEmpresas(int _take = 10, int _skip = 0, string _uf = "")
        {
            IQueryable<Empresa> IEmpresas = db.Empresas;
            if (_uf != "")
            {
                IEmpresas = IEmpresas.Where(x => x.UF == _uf);
            }
            
            return IEmpresas.OrderByDescending(x => x.CriadoEm).Skip(_skip).Take(_take);
        }

        // GET: api/Empresas
        [HttpGet]
        [Route("api/CountEmpresas")]
        public int CountEmpresas(int _take = 10, int _skip = 0, string _uf = "")
        {
            IQueryable<Empresa> IEmpresas = db.Empresas;
            if (_uf != "")
            {
                IEmpresas = IEmpresas.Where(x => x.UF == _uf);
            }

            return IEmpresas.Count();
        }

        // GET: api/Empresas/5
        [ResponseType(typeof(Empresa))]
        public IHttpActionResult GetEmpresa(Guid id)
        {
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return NotFound();
            }

            return Ok(empresa);
        }

        public ValidaForm ValidaEmpresa(Empresa empresa)
        {
            ValidaForm validaForm = new ValidaForm();
            validaForm.Valido = true;
            validaForm.Erro = "";
            if (empresa.Nome == null || empresa.Nome == "")
            {
                validaForm.Valido = false;
                validaForm.Erro = "O Título da promoção é obrigatório.";
            }
            if (empresa.CPFResponsavel == null || empresa.CPFResponsavel == "")
            {
                validaForm.Valido = false;
                validaForm.Erro = "O CPF é obrigatório. Digite o seu CPF.";
            }
            if (empresa.CEP == null || empresa.CEP == "")
            {
                validaForm.Valido = false;
                validaForm.Erro = "O Endereço tem que estar completo. Digite o CEP.";
            }
            if (empresa.Descricao.Trim().Length > 2000)
            {
                validaForm.Valido = false;
                validaForm.Erro = "A Descrição da empresa deve ter o máximo de 1500 caracteres.";
            }
            if (empresa.Latitude == null)
            {
                validaForm.Valido = false;
                validaForm.Erro = "O Endereço tem que estar completo.";
            }
            if (empresa.Longitude == null)
            {
                validaForm.Valido = false;
                validaForm.Erro = "O Endereço tem que estar completo.";
            }

            return validaForm;
        }

        // PUT: api/Empresas/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmpresa(Guid id, Empresa empresa)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != empresa.EmpresaId)
            {
                return BadRequest();
            }

            db.Entry(empresa).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpresaExists(empresa))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("api/UpdateEmpresa")]
        public Retorno UpdateEmpresa(Empresa empresa)
        {
            Retorno r = new Retorno();
            ValidaForm vlfrm = ValidaEmpresa(empresa);
            if (!vlfrm.Valido)
            {
                r.responseStatus = "NOK";
                r.desc = vlfrm.Erro;
                return r;
            }
            else
            {

                try
                {
                    Empresa _empresa = db.Empresas.Find(empresa.EmpresaId);
                    if (_empresa != null)
                    {
                        _empresa.Ativo = empresa.Ativo;
                        _empresa.AtualizadoEm = DateTime.Now;
                        _empresa.Bairro = empresa.Bairro;
                        _empresa.Celular = empresa.Celular;
                        _empresa.CEP = empresa.CEP;
                        _empresa.Cidade = empresa.Cidade;
                        _empresa.CNPJ = empresa.CNPJ;
                        _empresa.Complemento = empresa.Complemento;
                        _empresa.CPFResponsavel = empresa.CPFResponsavel;
                        _empresa.Descricao = empresa.Descricao;
                        _empresa.Email = empresa.Email;
                        _empresa.Latitude = empresa.Latitude;
                        _empresa.Longitude = empresa.Longitude;
                        _empresa.Logradouro = empresa.Logradouro;
                        _empresa.Nome = empresa.Nome;
                        _empresa.Numero = empresa.Numero;
                        _empresa.Responsavel = empresa.Responsavel;
                        _empresa.Site = empresa.Site;
                        _empresa.Telefone = empresa.Telefone;
                        _empresa.UF = empresa.UF;
                        

                        db.Entry(_empresa).State = EntityState.Modified;
                        db.SaveChanges();
                        r.responseStatus = "OK";
                        r.desc = "Empresa alterada com sucesso.";
                        return r;
                    }
                }
                catch (Exception e)
                {
                    if (!EmpresaExists(empresa))
                    {
                        r.responseStatus = "NOK";
                        r.desc = "Promoção não existe.";
                        return r;
                    }
                    else
                    {
                        r.responseStatus = "NOK";
                        r.desc = "Algum erro ocorreu. Tente mais tarde." + e;
                        return r;
                    }
                }
            }

            return r;
        }

        // POST: api/Empresas
        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("api/NovaEmpresa")]
        public Retorno PostEmpresa(Empresa empresa)
        {
            Retorno retorno = new Retorno();
            empresa.CriadoEm = DateTime.Now;
            empresa.AtualizadoEm = DateTime.Now;
            
            if (!ModelState.IsValid)
            {
                retorno.responseStatus = "NOK";
                retorno.desc = "Modelo Inválido";
                return retorno;
                //return BadRequest(ModelState);
            }
            
            db.Empresas.Add(empresa);
            if (!EmpresaExists(empresa))
            {
                try
                { 
                    db.SaveChanges();
                    f.write_cookie("eid", empresa.EmpresaId.ToString(), 30);

                    string userId = f.read_cookie("uid");
                    if (!string.IsNullOrEmpty(userId))
                    {
                        UsuarioEmpresa ue = new UsuarioEmpresa();
                        ue.EmpresaId = empresa.EmpresaId;
                        ue.UsuarioId = new Guid(userId);
                        ue.CriadoEm = DateTime.Now;
                        ue.AtualizadoEm = DateTime.Now;

                        db.UsuarioEmpresas.Add(ue);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    retorno.responseStatus = "NOK";
                    retorno.desc = "Houve uma falha de comunicação. Tente novamente, por favor.";
                    retorno.descConsole = "Mensagem:" + e.Message + " | InnerException: " + e.InnerException;
                    Console.Write(retorno.descConsole);
                    return retorno;
                }
                
            }
            else
            {
                retorno.responseStatus = "NOK";
                retorno.desc = "Já há uma empresa com esse nome no mesmo bairro. Por favor, verifique se a sua empresa já está cadastrada conosco.";
                return retorno;
            }

            retorno.responseStatus = "OK";
            retorno.desc = "Empresa cadastrada com sucesso.";
            return retorno;
        }

        // DELETE: api/Empresas/5
        [ResponseType(typeof(Empresa))]
        public IHttpActionResult DeleteEmpresa(Guid id)
        {
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return NotFound();
            }

            db.Empresas.Remove(empresa);
            db.SaveChanges();

            return Ok(empresa);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmpresaExists(Empresa emp)
        {
            bool resposta = false;
            resposta = db.Empresas.Count(e => e.EmpresaId == emp.EmpresaId) > 0;
            resposta = db.Empresas.Count(e => e.Nome == emp.Nome && e.Bairro == emp.Bairro) > 0;


            return resposta;
        }


    }
}