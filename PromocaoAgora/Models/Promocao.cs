//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PromocaoAgora.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Promocao
    {
        public System.Guid PromocaoId { get; set; }
        public System.Guid EmpresaId { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public Nullable<decimal> PrecoNormal { get; set; }
        public Nullable<decimal> PrecoComDesconto { get; set; }
        public Nullable<decimal> Desconto { get; set; }
        public Nullable<System.DateTime> DataInicio { get; set; }
        public Nullable<System.DateTime> DataFim { get; set; }
        public string TipoAnuncio { get; set; }
        public string CodigoPromocao { get; set; }
        public string ModoUso { get; set; }
        public string LinkPromocao { get; set; }
        public string Tags { get; set; }
        public Nullable<int> CategoriaId { get; set; }
        public Nullable<bool> Ativo { get; set; }
        public Nullable<int> Impressoes { get; set; }
        public Nullable<int> ViewsPagina { get; set; }
        public Nullable<int> ViewsCodigo { get; set; }
        public Nullable<System.DateTime> CriadoEm { get; set; }
        public Nullable<System.DateTime> AtualizadoEm { get; set; }
        public string TipoDesconto { get; set; }
        public string DescDesconto { get; set; }
        public Nullable<bool> MostraEndereco { get; set; }
        public Nullable<bool> MostraTelefone { get; set; }
        public Nullable<bool> MostraSite { get; set; }
        public Nullable<bool> MostraEmail { get; set; }
    
        public virtual Categoria Categoria { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}