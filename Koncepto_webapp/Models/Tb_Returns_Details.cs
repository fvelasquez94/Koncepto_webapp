//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Tb_Returns_Details
    {
        public int ID_detalle { get; set; }
        public int Docnum { get; set; }
        public string CodigoProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioDescuento { get; set; }
        public decimal TotalFila { get; set; }
        public string DocEntryDevolucion { get; set; }
        public int EstadoFila { get; set; }
        public string MensajeError { get; set; }
        public int Error { get; set; }
        public System.DateTime Fecha { get; set; }
    }
}
