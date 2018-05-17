using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Device.Location;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


namespace PromocaoAgora.Models
{
    public class Suporte
    {
        public bool hasArroba(string input)
        {
            string specialChar = @"\@";
            foreach (var item in specialChar)
            {
                if (input.Contains(item)) return true;
            }

            return false;
        }
    }
    public class LoginInfo
    {
        public string email { get; set; }
        public string pass { get; set; }
        public string redirecturl { get; set; }
    }
    public class Retorno
    {
        public Guid guid { get; set; }
        public string responseStatus { get; set; }
        public string desc { get; set; }
        public string descConsole { get; set; }
        public string returnUrl { get; set; }
    }

    //Class Distance
    public class ListCard
    {
        public int Registros { get; set; }
        public decimal Paginas { get; set; }
        public List<PromoCard> PromoCards { get; set; }

    }


    public class PromoCard
    {
        public Guid PromoId { get; set; }
        public double Distancia { get; set; }
        public string Titulo { get; set; }
        public string EmpresaNome { get; set; }
        public string Bairro { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public decimal? PrecoNormal { get; set; }
        public decimal? PrecoComDesconto { get; set; }
        public decimal? Desconto { get; set; }
        public string TipoDesconto { get; set; }
        public string DescDesconto { get; set; }
        public string TipoAnuncio { get; set; }
        //public DateTime? DataFim { get; set; }
        public string DataFim { get; set; }
        public string Site { get; set; }
        public bool? Ativo { get; set; }



        public double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            GeoCoordinate gc = new GeoCoordinate(lat1, lon1);
            GeoCoordinate gt = new GeoCoordinate(lat2, lon2);
            double dist = gc.GetDistanceTo(gt);
            
            return (dist/1000);
        }

        
    }

    public class ValidaForm
    {
        public bool Valido { get; set; }
        public string Erro { get; set; }
    }

    public class EmailEstrutura
    {
        public string Assunto { get; set; }
        public string Descricao { get; set; }
        public string Para { get; set; }
        public string From { get; set; }
    }

    public class Statistic
    {
        public string Impressoes { get; set; }
        public string ViewsPagina { get; set; }
        public string ViewsCodigo { get; set; }

        private promocaoagoraEntities db = new promocaoagoraEntities();

        public void DoStat(Guid PromoId, bool imp, bool vp, bool vc)
        {
            Promocao p = new Promocao();
            p = db.Promocaos.Find(PromoId);
            if (p != null)
            {
                if (imp) { p.Impressoes++; }
                if (vp) { p.ViewsPagina++; }
                if (vc) { p.ViewsCodigo++; }

                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

    }

    public class SendEmailUsuarioReq
    {
        public string status { get; set; }
        public bool? ativo { get; set; }
        public string UF { get; set; }
        public string cid { get; set; }
        public string bai { get; set; }
        public string assunto { get; set; }
        public string template { get; set; }
        public string campanha { get; set; }
    }
}