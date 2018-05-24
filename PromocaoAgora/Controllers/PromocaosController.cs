using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using PromocaoAgora.Models;

namespace PromocaoAgora.Controllers
{
    public class PromocaosController : ApiController
    {
        private promocaoagoraEntities db = new promocaoagoraEntities();
        public Framework f = new Framework();
        public Suporte S = new Suporte();
        

        // GET: api/Promocaos
        [HttpGet]
        [Route("api/ListaPromo")]
        public ListCard ListaPromo(int? p, double ULat, double ULon, string tipo = "Local", string q = "")
        {
            ListCard lc = new ListCard();
            List<PromoCard> listPromo = new List<PromoCard>();
            int registroPorPagina = 8;
            int page = p ?? 1;
            int skipReg = 0;
            if (page > 1)
            {
                skipReg = (page - 1) * registroPorPagina;
            }

            var query = db.Promocaos.Where(x => x.Ativo == true && x.TipoAnuncio == tipo && x.DataFim >= DateTime.Now);
            //q => keyword
            if(!string.IsNullOrEmpty(q))
            {
                query = query.Where(x => x.Titulo.Contains(q) || x.Descricao.Contains(q) || x.Empresa.Nome.Contains(q) || x.Empresa.Descricao.Contains(q));
            }

            foreach (var promo in query)
            {
                PromoCard pc = new PromoCard();
                pc.Distancia = pc.Distance(ULat, ULon, Convert.ToDouble(promo.Empresa.Latitude), Convert.ToDouble(promo.Empresa.Longitude));
                
                pc.Titulo = promo.Titulo;
                pc.TipoAnuncio = promo.TipoAnuncio;
                pc.PromoId = promo.PromocaoId;
                pc.PrecoNormal = promo.PrecoNormal;
                pc.PrecoComDesconto = promo.PrecoComDesconto;
                pc.TipoDesconto = promo.TipoDesconto;
                pc.DescDesconto = promo.DescDesconto;
                pc.Cidade = promo.Empresa.Cidade;
                pc.EmpresaNome = promo.Empresa.Nome;
                pc.Bairro = promo.Empresa.Bairro;
                pc.Endereco = promo.Empresa.Logradouro + ", " + promo.Empresa.Numero + " " + promo.Empresa.Complemento;
                pc.Desconto = promo.Desconto;
                pc.DataFim = promo.DataFim.Value.ToString("dd/MM/yyyy");
                pc.Site = promo.LinkPromocao;
                listPromo.Add(pc);
                
            }
            lc.Registros = listPromo.Count;
            lc.PromoCards = listPromo.OrderBy(x => x.Distancia).Skip(skipReg).Take(registroPorPagina).ToList();
            
            if (lc.Registros > registroPorPagina)
            {
                Decimal pags= Convert.ToDecimal(lc.Registros)/ Convert.ToDecimal(registroPorPagina);
                //decimal d = pags;
                lc.Paginas = Math.Ceiling(pags);
            }
            else
            {
                lc.Paginas = 1;
            }
            return lc;
        }

        // GET: api/Promocaos
        [HttpGet]
        [Route("api/ListaPromoByUsuario")]
        public ListCard ListaPromoByUsuario(Guid uid)
        {
            ListCard lc = new ListCard();
            List<PromoCard> listPromo = new List<PromoCard>();
            

            var query = db.Promocaos.Where(x => x.Empresa.UsuarioEmpresas.Where(y => y.UsuarioId == uid).Any() == true);
            query = query.Where(x => x.Ativo != null);

            foreach (var promo in query)
            {
                PromoCard pc = new PromoCard();
                pc.Distancia = 0;//pc.Distance(ULat, ULon, Convert.ToDouble(promo.Empresa.Latitude), Convert.ToDouble(promo.Empresa.Longitude));

                pc.Titulo = promo.Titulo;
                pc.TipoAnuncio = promo.TipoAnuncio;
                pc.PromoId = promo.PromocaoId;
                pc.PrecoNormal = promo.PrecoNormal;
                pc.PrecoComDesconto = promo.PrecoComDesconto;
                pc.TipoDesconto = promo.TipoDesconto;
                pc.DescDesconto = promo.DescDesconto;
                pc.EmpresaNome = promo.Empresa.Nome;
                pc.Bairro = promo.Empresa.Bairro;
                pc.Endereco = promo.Empresa.Logradouro + ", " + promo.Empresa.Numero + " " + promo.Empresa.Complemento;
                pc.Desconto = promo.Desconto;
                pc.DataFim = promo.DataFim.Value.ToString("dd/MM/yyyy");
                pc.Site = promo.LinkPromocao;
                pc.Ativo = promo.Ativo;
                listPromo.Add(pc);

            }
            lc.Registros = listPromo.Count;
            lc.PromoCards = listPromo.OrderBy(x => x.EmpresaNome).ToList();

            
            return lc;
        }

        // GET: api/Promocaos
        public IQueryable<Promocao> GetPromocaos(int _take = 10, int _skip = 0, bool? _ativo = null)
        {
            db.Configuration.LazyLoadingEnabled = false;

            IQueryable<Promocao> IPromocoes = db.Promocaos;
            if (_ativo == true)
            {
                IPromocoes = IPromocoes.Where(x => x.DataFim >= DateTime.Now);
            }

            return IPromocoes.OrderByDescending(x => x.CriadoEm).Skip(_skip).Take(_take);
        }

        // GET: api/Promocaos
        [HttpGet]
        [Route("api/CountPromocoes")]
        public int CountPromocoes(bool? _ativo = null)
        {
            db.Configuration.LazyLoadingEnabled = false;

            IQueryable<Promocao> IPromocoes = db.Promocaos;
            if (_ativo == true)
            {
                IPromocoes = IPromocoes.Where(x => x.DataFim >= DateTime.Now);
            }

            return IPromocoes.Count();
        }

        // GET: api/Promocaos/5
        [ResponseType(typeof(Promocao))]
        public IHttpActionResult GetPromocao(Guid id)
        {
            Promocao promocao = db.Promocaos.Find(id);
            
            if (promocao == null)
            {
                return NotFound();
            }
            
            return Ok(promocao);
        }

        
        public ValidaForm ValidaPromocao(Promocao promocao)
        {
            ValidaForm validaFormPromo = new ValidaForm();
            validaFormPromo.Valido = true;
            validaFormPromo.Erro = "";
            if(promocao.EmpresaId == null)
            {
                validaFormPromo.Valido = false;
                validaFormPromo.Erro = "A Promoção deve estar vinculada a uma empresa. Caso não tenha ainda feito, crie a empresa.";
            }
            if (promocao.Titulo == null || promocao.Titulo == "")
            {
                validaFormPromo.Valido = false;
                validaFormPromo.Erro = "O Título da promoção é obrigatório.";
            }
            if (promocao.Descricao == null || promocao.Descricao == "")
            {
                validaFormPromo.Valido = false;
                validaFormPromo.Erro = "A Descrição da promoção é obrigatória.";
            }
            if (promocao.Descricao.Length > 2000)
            {
                validaFormPromo.Valido = false;
                validaFormPromo.Erro = "A Descrição da promoção deve ter o máximo de 1500 caracteres.";
            }
            if (promocao.ModoUso == null || promocao.ModoUso == "")
            {
                validaFormPromo.Valido = false;
                validaFormPromo.Erro = "As regras da promoção é um item obrigatório.";
            }
            else
            {
                if (promocao.ModoUso.Length > 500)
                {
                    validaFormPromo.Valido = false;
                    validaFormPromo.Erro = "A Descrição das regras deve ter o máximo de 500 caracteres.";
                }
            }
            
            if (promocao.DataInicio == null)
            {
                validaFormPromo.Valido = false;
                validaFormPromo.Erro = "A data de início da promoção é de preenchimento obrigatório.";
            }
            if (promocao.DataFim == null)
            {
                validaFormPromo.Valido = false;
                validaFormPromo.Erro = "A data de início da promoção é de preenchimento obrigatório.";
            }
            if (promocao.DataFim <= DateTime.Today)
            {
                validaFormPromo.Valido = false;
                validaFormPromo.Erro = "A data de fim da promoção precisa ser maior ou igual a data de hoje.";
            }
            if (promocao.PrecoNormal == null && promocao.TipoDesconto == "P")
            {
                validaFormPromo.Valido = false;
                validaFormPromo.Erro = "O preço normal da promoção é de preenchimento obrigatório com o tipo de desconto de preço.";
            }
            if (promocao.PrecoComDesconto == null && promocao.TipoDesconto == "P")
            {
                validaFormPromo.Valido = false;
                validaFormPromo.Erro = "A preço de desconto da promoção é de preenchimento obrigatório com o tipo de desconto de preço.";
            }
            if (promocao.Desconto == null || promocao.Desconto < 0)
            {
                validaFormPromo.Valido = false;
                validaFormPromo.Erro = "O desconto da promoção é de preenchimento obrigatório.";
            }
            if (promocao.DescDesconto == null && promocao.TipoDesconto == "C")
            {
                validaFormPromo.Valido = false;
                validaFormPromo.Erro = "A descrição do desconto da promoção é de preenchimento obrigatório.";
            }
            return validaFormPromo;
        }
        
        // PUT: api/UpdatePromocao
        //[ResponseType(typeof(void))]
        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("api/UpdatePromocao")]
        public Retorno UpdatePromocao(Promocao promocao)
        {
            Retorno r = new Retorno();
            ValidaForm vlpromo = ValidaPromocao(promocao);
            if (!vlpromo.Valido)
            {
                r.responseStatus = "NOK";
                r.desc = vlpromo.Erro;
                
                return r;
            }
            else
            {

                try
                {
                    Promocao _promocao = db.Promocaos.Find(promocao.PromocaoId);
                    if(promocao.TipoDesconto == "C")
                    {
                        promocao.PrecoComDesconto = null;
                        promocao.PrecoNormal = null;
                    }
                    else
                    {
                        promocao.DescDesconto = "";
                    }
                    if(promocao.TipoAnuncio == "Local")
                    {
                        promocao.LinkPromocao = null;
                    }
                    if (promocao.LinkPromocao != null)
                    {
                        if (!promocao.LinkPromocao.Contains("http") && !promocao.LinkPromocao.Contains("@"))
                        {
                            promocao.LinkPromocao = "http://" + promocao.LinkPromocao;
                        }
                    }

                    if (_promocao != null)
                    {
                        _promocao.Ativo = promocao.Ativo;
                        _promocao.AtualizadoEm = DateTime.Now;
                        _promocao.CategoriaId = promocao.CategoriaId;
                        _promocao.CodigoPromocao = promocao.CodigoPromocao;
                        _promocao.DataFim = promocao.DataFim;
                        _promocao.DataInicio = promocao.DataInicio;
                        _promocao.TipoDesconto = promocao.TipoDesconto;
                        _promocao.Desconto = promocao.Desconto;
                        _promocao.DescDesconto = promocao.DescDesconto;
                        _promocao.Descricao = promocao.Descricao;
                        _promocao.LinkPromocao = promocao.LinkPromocao;
                        _promocao.ModoUso = promocao.ModoUso;
                        _promocao.PrecoComDesconto = promocao.PrecoComDesconto;
                        _promocao.PrecoNormal = promocao.PrecoNormal;
                        _promocao.TipoAnuncio = promocao.TipoAnuncio;
                        _promocao.Titulo = promocao.Titulo;
                        _promocao.MostraEndereco = promocao.MostraEndereco;
                        _promocao.MostraTelefone = promocao.MostraTelefone;
                        _promocao.MostraSite = promocao.MostraSite;
                        _promocao.MostraEmail = promocao.MostraEmail;
                        db.Entry(_promocao).State = EntityState.Modified;
                        db.SaveChanges();
                        r.responseStatus = "OK";
                        r.desc = "Promoção alterada com sucesso.";
                        return r;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    if (!PromocaoExists(promocao))
                    {
                        r.responseStatus = "NOK";
                        r.desc = "Promoção não existe." + e.ToString();
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

        // POST: api/Promocaos
        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("api/CadastraPromocao")]
        public Retorno PostPromocao(Promocao promocao)
        {
            Retorno retorno = new Retorno();
            ValidaForm vlpromo = ValidaPromocao(promocao);
            if (!vlpromo.Valido)
            {
                retorno.responseStatus = "NOK";
                retorno.desc = vlpromo.Erro;
                return retorno;
            }
            promocao.PromocaoId = Guid.NewGuid();
            promocao.CriadoEm = DateTime.Now;
            promocao.AtualizadoEm = DateTime.Now;
            if (f.read_cookie("eid") != "")
            {
                promocao.EmpresaId = new Guid(f.read_cookie("eid")); //new Guid("16C28901-DABB-73A9-8FC2-C9CB6AE4AA7A");
            }
            else
            {
                retorno.responseStatus = "NOK";
                retorno.desc = "Empresa não encontrada.";
                return retorno;
            }
            if (promocao.LinkPromocao != null)
            {
                if (!promocao.LinkPromocao.Contains("http") && !promocao.LinkPromocao.Contains("@"))
                {
                    promocao.LinkPromocao = "http://" + promocao.LinkPromocao;
                }
            }
            if (!ModelState.IsValid)
            {
                retorno.responseStatus = "NOK";
                retorno.desc = "Modelo Inválido";
                return retorno;
                //return BadRequest(ModelState);
            }

            db.Promocaos.Add(promocao);
            if (!PromocaoExists(promocao))
            {
                try
                {
                    db.SaveChanges();
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
                retorno.desc = "Já há uma promoção com esse título para essa empresa. Por favor, verifique se a promoção já está cadastrada conosco.";
                return retorno;
            }


            retorno.responseStatus = "OK";
            retorno.desc = "Promoção cadastrada com sucesso.";
            string avisoAdmin = "Nova Promoção: <br/>Titulo: " + promocao.Titulo + "<br/>Descrição:" + promocao.Descricao;
            f.SendMail("", "mmatiaso@gmail.com", "Nova Promoção cadastrada", avisoAdmin);
            return retorno;
        }

        // DELETE: api/Promocaos/5
        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("api/ApagaPromocao")]
        public Retorno ApagaPromocao(Guid id)
        {
            Retorno retorno = new Retorno();
            Promocao promocao = db.Promocaos.Find(id);
            if (promocao == null)
            {
                retorno.responseStatus = "NOK";
                retorno.desc = "Promoção não encontrada no sistema.";
                return retorno;
            }

            promocao.Ativo = null;
            db.Entry(promocao).State = EntityState.Modified;
            db.SaveChanges();

            retorno.responseStatus = "OK";
            retorno.desc = "Promoção excluída.";
            return retorno;
        }

        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("api/AtivaPromocao")]
        public Retorno AtivaPromocao(Guid id)
        {
            Retorno retorno = new Retorno();
            Promocao promocao = db.Promocaos.Find(id);
            if (promocao == null)
            {
                retorno.responseStatus = "NOK";
                retorno.desc = "Promoção não encontrada no sistema.";
                return retorno;
            }
            if (promocao.Ativo.HasValue)
            {
                if (promocao.Ativo.Value)
                {
                    promocao.Ativo = false;
                    promocao.DataFim = DateTime.Now;
                }
                else if (!promocao.Ativo.Value)
                {
                    promocao.Ativo = true;
                    promocao.DataFim = DateTime.Now.AddDays(7);
                }
            }
            db.Entry(promocao).State = EntityState.Modified;
            db.SaveChanges();

            retorno.responseStatus = "OK";
            retorno.desc = "Promoção excluída.";
            return retorno;
        }

        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("api/DeletaPromocao")]
        public Retorno DeletePromocao(Guid id)
        {
            Retorno retorno = new Retorno();
            Promocao promocao = db.Promocaos.Find(id);
            if (promocao == null)
            {
                retorno.responseStatus = "NOK";
                retorno.desc = "Promoção não encontrada no sistema.";
                return retorno;
            }

            db.Promocaos.Remove(promocao);
            db.SaveChanges();

            retorno.responseStatus = "OK";
            retorno.desc = "Promoção apagada do sistema.";
            return retorno;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PromocaoExists(Promocao promocao)
        {
            bool resposta = false;
            resposta = db.Promocaos.Count(e => e.PromocaoId == e.PromocaoId) > 0;
            resposta = db.Promocaos.Count(e => e.Titulo == promocao.Titulo && e.EmpresaId == promocao.EmpresaId) > 0;


            return resposta;
            
        }

        
    }
}