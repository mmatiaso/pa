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
    
    public partial class UsuarioEmpresa
    {
        public int UsuarioEmpresaId { get; set; }
        public Nullable<System.Guid> UsuarioId { get; set; }
        public Nullable<System.Guid> EmpresaId { get; set; }
        public Nullable<System.DateTime> CriadoEm { get; set; }
        public Nullable<System.DateTime> AtualizadoEm { get; set; }
    
        public virtual Empresa Empresa { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
