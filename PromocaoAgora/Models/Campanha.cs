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
    
    public partial class Campanha
    {
        public int CampanhaId { get; set; }
        public string Nome { get; set; }
        public Nullable<int> EmailEnvios { get; set; }
        public Nullable<int> EmailAberturas { get; set; }
        public Nullable<int> EmailClicks { get; set; }
        public Nullable<System.DateTime> CriadoEm { get; set; }
        public Nullable<System.DateTime> AtualizadoEm { get; set; }
    }
}