using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace PromocaoAgora.Models
{
    public class Modelos
    {
        private promocaoagoraEntities db = new promocaoagoraEntities();
        public Framework F = new Framework();

        //Campanhas
        public Campanha MantemCampanha(Campanha c)
        {
            Campanha C = new Campanha();
            var q = db.Campanhas.Where(x => x.Nome == c.Nome);
            if (q.Any())
            {
                //update
                try
                {
                    C = q.First();
                    db.Entry(C).State = EntityState.Modified;
                    db.SaveChanges();
                    return C;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    C = c;
                    C.EmailAberturas = 0;
                    C.EmailClicks = 0;
                    C.CriadoEm = DateTime.Now;
                    C.AtualizadoEm = DateTime.Now;
                    db.Campanhas.Add(C);
                    db.SaveChanges();
                    return C;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }


        //Campanhas
        public Usuario MantemUsuario(Usuario u)
        {
            Usuario U = new Usuario();
            var q = db.Usuarios.Where(x => x.UsuarioId == u.UsuarioId);
            if (q.Any())
            {
                //update
                try
                {
                    U = q.First();
                    U.Ativo = u.Ativo;
                    U.AtualizadoEm = DateTime.Now;
                    U.Bairro = u.Bairro;
                    U.CampanhasAbertas = u.CampanhasAbertas;
                    U.CampanhasClicadas = u.CampanhasClicadas;
                    U.CampanhasEnviadas = u.CampanhasEnviadas;
                    U.Celular = u.Celular;
                    U.Cidade = u.Cidade;
                    U.CPF = u.CPF;
                    U.CriadoEm = u.CriadoEm;
                    U.Email = u.Email;
                    U.Nome = u.Nome;
                    U.Senha = u.Senha;
                    U.Status = u.Status;
                    U.TagAcao = u.TagAcao;
                    U.UF = u.UF;
                    db.Entry(U).State = EntityState.Modified;
                    db.SaveChanges();
                    return U;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    U = u;
                    db.Usuarios.Add(U);
                    db.SaveChanges();
                    return U;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}