﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Koncepto_webapp.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DB_KONCEPTOEntities : DbContext
    {
        public DB_KONCEPTOEntities()
            : base("name=DB_KONCEPTOEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BI_Facturas_Detalle> BI_Facturas_Detalle { get; set; }
        public virtual DbSet<BI_Facturas_Encabezado> BI_Facturas_Encabezado { get; set; }
    }
}
