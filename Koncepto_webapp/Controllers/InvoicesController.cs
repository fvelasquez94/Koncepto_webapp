﻿using Koncepto_webapp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Koncepto_webapp.Controllers
{
    public class InvoicesController : Controller
    {
        Koncept_dbEntities dbkoncepto = new Koncept_dbEntities();
        DB_KONCEPTOEntities SAPkoncepto = new DB_KONCEPTOEntities();

        //CLASS GENERAL
        private clsGeneral generalClass = new clsGeneral();
        // GET: Invoices


        public class grupo_clientes
        {
            public int? id_grupo { get; set; }
            public string grupo { get; set; }
        }
        public class grupo_productos
        {
            public int? id_grupo { get; set; }
            public string grupo { get; set; }
        }
        public class Linea

        {
            public string id_linea { get; set; }
            public string linea { get; set; }
            public int? id_grupo { get; set; }
            public string grupo { get; set; }


        }

        public class Marca

        {
            public string id_marca { get; set; }
            public string marca { get; set; }
            public string id_linea { get; set; }
            public string linea { get; set; }
            public int? id_grupo { get; set; }
            public string grupo { get; set; }

        }

        public ActionResult Invoice_process()
        {
            if (generalClass.checkSession())
            {
                Sys_Users activeuser = Session["activeUser"] as Sys_Users;
                //PERMISOS
                List<string> s = new List<string>(activeuser.Departments.Split(new string[] { "," }, StringSplitOptions.None));
                ViewBag.lstDepartments = JsonConvert.SerializeObject(s);
                List<string> r = new List<string>(activeuser.Roles.Split(new string[] { "," }, StringSplitOptions.None));
                ViewBag.lstRoles = JsonConvert.SerializeObject(r);
                //PUNTOS DE VENTA
                List<string> puntosVenta = new List<string>(activeuser.ID_SalesPoint.Split(new string[] { "," }, StringSplitOptions.None));


                var lstCustomers = SAPkoncepto.BI_Dim_Customers.ToList();
                ViewBag.lstCustomers = lstCustomers;


                var grupoCliente = SAPkoncepto.BI_Dim_Customers.Select(c => new grupo_clientes { id_grupo = c.Id_Grupo_Clientes, grupo = c.Grupo_Clientes }).Distinct().ToList();
                ViewBag.grupoCliente = grupoCliente;


                var lstCategories = (from a in SAPkoncepto.BI_Dim_Productos where (a.Nombre_Producto != null && a.Linea != null && a.Marca != null && a.Id_Grupo_Productos != 100) select new grupo_productos { id_grupo = a.Id_Grupo_Productos, grupo = a.Grupo_Productos }).Distinct().OrderBy(c => c.grupo).ToList();
                ViewBag.lstCategories = lstCategories;

                List<BI_Dim_Empleados> vendedores = new List<BI_Dim_Empleados>();
                if (activeuser.Roles.Contains("Vendedor"))
                {
                    vendedores = (from a in SAPkoncepto.BI_Dim_Empleados where (a.Tipo_Empleado.Contains("Vendedor") && a.Activo == "Y" && a.Id_Vendedor == activeuser.ID_SalesRep) select a).ToList();
                }
                else
                {
                    vendedores = (from a in SAPkoncepto.BI_Dim_Empleados where (a.Tipo_Empleado.Contains("Vendedor") && a.Activo == "Y") select a).ToList();
                }


                ViewBag.lstEmpleados = vendedores;



                return View();

            }
            else
            {

                return RedirectToAction("Signin", "Home", new { access = false });

            }
        }

        public ActionResult Invoice_processEdit(int factura)
        {
            if (generalClass.checkSession())
            {
                Sys_Users activeuser = Session["activeUser"] as Sys_Users;
                //PERMISOS
                List<string> s = new List<string>(activeuser.Departments.Split(new string[] { "," }, StringSplitOptions.None));
                ViewBag.lstDepartments = JsonConvert.SerializeObject(s);
                List<string> r = new List<string>(activeuser.Roles.Split(new string[] { "," }, StringSplitOptions.None));
                ViewBag.lstRoles = JsonConvert.SerializeObject(r);
                //PUNTOS DE VENTA
                List<string> puntosVenta = new List<string>(activeuser.ID_SalesPoint.Split(new string[] { "," }, StringSplitOptions.None));



                //Necesario

                //factura
                //header
                var header = (from a in dbkoncepto.Tb_Invoices where (a.ID_factura == factura) select a).FirstOrDefault();
                //detalles
                var detalles = (from a in dbkoncepto.Tb_Invoices_Details where (a.ID_factura == factura) select a).ToList();
                //Enviamos cliente seleccionado

                ViewBag.headerfactura = header;

                //

                var lstCustomers = SAPkoncepto.BI_Dim_Customers.ToList();
                ViewBag.lstCustomers = lstCustomers;


                var grupoCliente = SAPkoncepto.BI_Dim_Customers.Select(c => new grupo_clientes { id_grupo = c.Id_Grupo_Clientes, grupo = c.Grupo_Clientes }).Distinct().ToList();
                ViewBag.grupoCliente = grupoCliente;


                var lstCategories = (from a in SAPkoncepto.BI_Dim_Productos where (a.Nombre_Producto != null && a.Linea != null && a.Marca != null && a.Id_Grupo_Productos != 100) select new grupo_productos { id_grupo = a.Id_Grupo_Productos, grupo = a.Grupo_Productos }).Distinct().OrderBy(c => c.grupo).ToList();
                ViewBag.lstCategories = lstCategories;

                List<BI_Dim_Empleados> vendedores = new List<BI_Dim_Empleados>();

                    vendedores = (from a in SAPkoncepto.BI_Dim_Empleados where (a.Id_Vendedor==header.ID_Vendedor) select a).ToList();
                


                ViewBag.lstEmpleados = vendedores;


                ViewBag.ideliminar = header.ID_factura;

                return View(detalles);

            }
            else
            {

                return RedirectToAction("Signin", "Home", new { access = false });

            }
        }

        public ActionResult Invoice_process2()
        {
            if (generalClass.checkSession())
            {
                Sys_Users activeuser = Session["activeUser"] as Sys_Users;
                //PERMISOS
                List<string> s = new List<string>(activeuser.Departments.Split(new string[] { "," }, StringSplitOptions.None));
                ViewBag.lstDepartments = JsonConvert.SerializeObject(s);
                List<string> r = new List<string>(activeuser.Roles.Split(new string[] { "," }, StringSplitOptions.None));
                ViewBag.lstRoles = JsonConvert.SerializeObject(r);
                //PUNTOS DE VENTA
                List<string> puntosVenta = new List<string>(activeuser.ID_SalesPoint.Split(new string[] { "," }, StringSplitOptions.None));


                var lstCustomers = SAPkoncepto.BI_Dim_Customers.ToList();
                ViewBag.lstCustomers = lstCustomers;


                var grupoCliente = SAPkoncepto.BI_Dim_Customers.Select(c => new grupo_clientes { id_grupo = c.Id_Grupo_Clientes, grupo = c.Grupo_Clientes }).Distinct().ToList();
                ViewBag.grupoCliente = grupoCliente;


                var lstCategories = (from a in SAPkoncepto.BI_Dim_Productos where (a.Nombre_Producto != null && a.Linea != null && a.Marca != null && a.Id_Grupo_Productos != 100) select new grupo_productos { id_grupo = a.Id_Grupo_Productos, grupo = a.Grupo_Productos }).Distinct().OrderBy(c => c.grupo).ToList();
                ViewBag.lstCategories = lstCategories;

                List<BI_Dim_Empleados> vendedores = new List<BI_Dim_Empleados>();
                if (activeuser.Roles.Contains("Vendedor"))
                {
                    vendedores = (from a in SAPkoncepto.BI_Dim_Empleados where (a.Tipo_Empleado.Contains("Vendedor") && a.Activo == "Y" && a.Id_Vendedor==activeuser.ID_SalesRep) select a).ToList();
                }
                else {
                    vendedores = (from a in SAPkoncepto.BI_Dim_Empleados where (a.Tipo_Empleado.Contains("Vendedor") && a.Activo == "Y") select a).ToList();
                }

      
                ViewBag.lstEmpleados = vendedores;



                return View();

            }
            else
            {

                return RedirectToAction("Signin", "Home", new { access = false });

            }
        }


        public ActionResult Save_newCustomer(string nombre, string correo, string telefono, string direccion, string departamento, int tipocontribuyente, string dui, string nrc, string nit, string contribuyente)
        {
            Tb_newCustomers newCustomer = new Tb_newCustomers();
            newCustomer.Nombre = nombre;
            newCustomer.Correo = correo;
            newCustomer.Telefono = telefono;
            newCustomer.Direccion = direccion;
            newCustomer.Departamento = departamento;
            newCustomer.Tipo_contribuyente = tipocontribuyente;
            newCustomer.DUI = dui;
            newCustomer.NRC = nrc;
            newCustomer.NIT = nit;
            newCustomer.Contribuyente = contribuyente;
            newCustomer.Creation_date = DateTime.UtcNow;
            newCustomer.ID_SAP = "";






            try
            {
                //guardamos modelo
                dbkoncepto.Tb_newCustomers.Add(newCustomer);
                dbkoncepto.SaveChanges();

                List<Tb_newCustomers> lstCustomers = new List<Tb_newCustomers>();
                lstCustomers.Add(newCustomer);
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string lstnuevocliente = javaScriptSerializer.Serialize(lstCustomers);

                var result = new { flag = "Success", lstNuevoCliente = lstnuevocliente };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex){
                var result = new { flag = ex };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            

        }

        public ActionResult Search_customer(string idcustomer)
        {
            

            try
            {
                BI_Dim_Customers newCustomer = SAPkoncepto.BI_Dim_Customers.Where(a=>a.Id_Cliente==idcustomer).FirstOrDefault();

                List<BI_Dim_Customers> lstCustomers = new List<BI_Dim_Customers>();
                lstCustomers.Add(newCustomer);

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string lstnuevocliente = javaScriptSerializer.Serialize(lstCustomers);

                var result = new { flag = "Success", lstNuevoCliente = lstnuevocliente };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result = new { flag = ex };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }


        public ActionResult product_searchsku(string sku)
        {

            try
            {
                var product = SAPkoncepto.BI_Dim_Productos.Where(a => a.Id_Producto == sku).FirstOrDefault();

                if (product == null)
                {
                    var result = new { flag = "Producto no encontrado."};
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else {

                    List<BI_Dim_Productos> lstProducts = new List<BI_Dim_Productos>();
                    lstProducts.Add(product);
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string lstProductos = javaScriptSerializer.Serialize(lstProducts);

                    var result = new { flag = "Success", lstProducts = lstProductos };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                var result = new { flag = ex };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult product_searchdescription(string description)
        {

            try
            {
                var product = SAPkoncepto.BI_Dim_Productos.Where(a => a.Nombre_Producto.Contains(description)).ToList();

                if (product.Count <=0)
                {
                    var result = new { flag = "Producto no encontrado."};
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else {


                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string lstProductos = javaScriptSerializer.Serialize(product);

                    var result = new { flag = "Success", lstProducts = lstProductos };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                var result = new { flag = ex };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult product_searchcodigobarras(string codigobarras)
        {

            try
            {
                var product = SAPkoncepto.BI_Dim_Productos.Where(a => a.Codigo_Barras ==codigobarras).FirstOrDefault();

                if (product == null)
                {
                    var result = new { flag = "Producto no encontrado."};
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else {

                    List<BI_Dim_Productos> lstProducts = new List<BI_Dim_Productos>();
                    lstProducts.Add(product);
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string lstProductos = javaScriptSerializer.Serialize(lstProducts);

                    var result = new { flag = "Success", lstProducts = lstProductos };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                var result = new { flag = ex };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult product_searchasistente(int categoria, string subcategoria, string marca)
        {

            try
            {
                var product = SAPkoncepto.BI_Dim_Productos.Where(a => a.Id_Grupo_Productos ==categoria && a.Id_Linea.Contains(subcategoria) && a.Marca ==marca && a.Activo=="Y").ToList();

                if (product.Count() <= 0)
                {
                    var result = new { flag = "Producto(s) no encontrado(s)." };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string lstProductos = javaScriptSerializer.Serialize(product);

                    var result = new { flag = "Success", lstProducts = lstProductos };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                var result = new { flag = ex };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult Get_linea(int? id_grupo)
        {
            try
            {
                    var lst_linea = SAPkoncepto.BI_Dim_Productos
                            .Where(i => i.Id_Grupo_Productos == id_grupo && i.Id_Linea !=null)
                            .Select(i => new Linea { id_linea = i.Id_Linea, linea = i.Linea, grupo=i.Grupo_Productos, id_grupo=i.Id_Grupo_Productos})
                            .Distinct()
                            .OrderBy(i => i.linea)
                            .ToList();
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    string result = javaScriptSerializer.Serialize(lst_linea);
                    return Json(result, JsonRequestBehavior.AllowGet);
                
            }
            catch
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Get_marca(string id_linea, int? id_grupo)
        {
            try
            {
                var lst_linea = SAPkoncepto.BI_Dim_Productos
                        .Where(i => i.Id_Linea == id_linea && i.Id_Grupo_Productos==id_grupo && i.Id_Marca !=null)
                        .Select(i => new Marca {id_marca=i.Marca, marca=i.Marca, id_linea = i.Id_Linea, linea = i.Linea, grupo = i.Grupo_Productos, id_grupo = i.Id_Grupo_Productos })
                        .Distinct()
                        .OrderBy(i => i.marca)
                        .ToList();
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string result = javaScriptSerializer.Serialize(lst_linea);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult Save_InvoiceData(List<Tb_Invoices_Details> detailsInvoice, List<Tb_Invoices> headerInvoice)
        {
            string ttresult = "";
            try
            {
                Sys_Users activeuser = Session["activeUser"] as Sys_Users;
                if (detailsInvoice.Count > 0 && headerInvoice.Count>0) {


                    Tb_Invoices header = new Tb_Invoices();
                    //guardamos cabecera
                    header = headerInvoice.FirstOrDefault();


                    if (header.BancoCheque == null) { header.BancoCheque = ""; }
                    if (header.NumeroCheque == null) { header.NumeroCheque = ""; }
                    if (header.TitularTarjeta == null) { header.TitularTarjeta = ""; }
                    if (header.DocumentoTarjeta == null) { header.DocumentoTarjeta = ""; }
                    if (header.NumeroTarjeta == null) { header.NumeroTarjeta = ""; }
                    if (header.VoucherTarjeta == null) { header.VoucherTarjeta = ""; }

                    header.Docentry = "";
                    header.MensajeError = "";
                    header.ID_sucursal = activeuser.ID_SalesPoint;
                    header.Sucursal = activeuser.Assigned_SalesPoint;
                    header.Estado = 1;
                    dbkoncepto.Tb_Invoices.Add(header);
                    dbkoncepto.SaveChanges();

                    //guardamos detalles

                    foreach (var item in detailsInvoice) {

                        Tb_Invoices_Details newdetail = new Tb_Invoices_Details();
                        newdetail = item;
                        newdetail.MensajeError = "";
                        newdetail.DocEntryDevolucion = "";

                        newdetail.ID_factura = header.ID_factura;

                    }

                    dbkoncepto.Tb_Invoices_Details.AddRange(detailsInvoice);
                    dbkoncepto.SaveChanges();

                    ttresult = "SUCCESS";
                    return Json(ttresult, JsonRequestBehavior.AllowGet);
                }



                ttresult = "No data";
                return Json(ttresult, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ttresult = "ERROR: " + ex.Message;
                return Json(ttresult, JsonRequestBehavior.AllowGet);
            }




        }


        [HttpPost]
        public ActionResult Save_InvoiceDatafromPedido(int ideliminar,List<Tb_Invoices_Details> detailsInvoice, List<Tb_Invoices> headerInvoice)
        {
            string ttresult = "";
            try
            {

                Sys_Users activeuser = Session["activeUser"] as Sys_Users;




                if (detailsInvoice.Count > 0 && headerInvoice.Count > 0)
                {

                    //ELIMINAMOS ANTERIOR

                     Tb_Invoices headereliminar = dbkoncepto.Tb_Invoices.Find(ideliminar);
                    dbkoncepto.Tb_Invoices.Remove(headereliminar);
                    dbkoncepto.SaveChanges();


                    var listtodelete = dbkoncepto.Tb_Invoices_Details.Where(a => a.ID_factura == ideliminar).ToList();
                    dbkoncepto.Tb_Invoices_Details.RemoveRange(listtodelete);
                    dbkoncepto.SaveChanges();






                    //

                    Tb_Invoices header = new Tb_Invoices();
                    //guardamos cabecera
                    header = headerInvoice.FirstOrDefault();


                    if (header.BancoCheque == null) { header.BancoCheque = ""; }
                    if (header.NumeroCheque == null) { header.NumeroCheque = ""; }
                    if (header.TitularTarjeta == null) { header.TitularTarjeta = ""; }
                    if (header.DocumentoTarjeta == null) { header.DocumentoTarjeta = ""; }
                    if (header.NumeroTarjeta == null) { header.NumeroTarjeta = ""; }
                    if (header.VoucherTarjeta == null) { header.VoucherTarjeta = ""; }

                    header.Docentry = "";
                    header.MensajeError = "";
                    header.ID_sucursal = activeuser.ID_SalesPoint;
                    header.Sucursal = activeuser.Assigned_SalesPoint;
                    header.Estado = 1;
                    dbkoncepto.Tb_Invoices.Add(header);
                    dbkoncepto.SaveChanges();

                    //guardamos detalles

                    foreach (var item in detailsInvoice)
                    {

                        Tb_Invoices_Details newdetail = new Tb_Invoices_Details();
                        newdetail = item;
                        newdetail.MensajeError = "";
                        newdetail.DocEntryDevolucion = "";

                        newdetail.ID_factura = header.ID_factura;

                    }

                    dbkoncepto.Tb_Invoices_Details.AddRange(detailsInvoice);
                    dbkoncepto.SaveChanges();

                    ttresult = "SUCCESS";
                    return Json(ttresult, JsonRequestBehavior.AllowGet);
                }



                ttresult = "No data";
                return Json(ttresult, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ttresult = "ERROR: " + ex.Message;
                return Json(ttresult, JsonRequestBehavior.AllowGet);
            }




        }




        [HttpPost]
        public ActionResult Save_Pedido(List<Tb_Invoices_Details> detailsInvoice, List<Tb_Invoices> headerInvoice)
        {
            string ttresult = "";
            try
            {
                Sys_Users activeuser = Session["activeUser"] as Sys_Users;
                if (detailsInvoice.Count > 0 && headerInvoice.Count > 0)
                {


                    Tb_Invoices header = new Tb_Invoices();
                    //guardamos cabecera
                    header = headerInvoice.FirstOrDefault();


                    if (header.BancoCheque == null) { header.BancoCheque = ""; }
                    if (header.NumeroCheque == null) { header.NumeroCheque = ""; }
                    if (header.TitularTarjeta == null) { header.TitularTarjeta = ""; }
                    if (header.DocumentoTarjeta == null) { header.DocumentoTarjeta = ""; }
                    if (header.NumeroTarjeta == null) { header.NumeroTarjeta = ""; }
                    if (header.VoucherTarjeta == null) { header.VoucherTarjeta = ""; }

                    header.TipoDocumento = "";
                    header.Cod_tipoDocumento = "";
                    header.TipoPago = "";
                    header.Cod_tipoPago = "";
                    header.Docentry = "";
                    header.MensajeError = "";
                    header.ID_sucursal = activeuser.ID_SalesPoint;
                    header.Sucursal = activeuser.Assigned_SalesPoint;
                    header.Estado = 0;//pedido
                    dbkoncepto.Tb_Invoices.Add(header);
                    dbkoncepto.SaveChanges();

                    //guardamos detalles

                    foreach (var item in detailsInvoice)
                    {

                        Tb_Invoices_Details newdetail = new Tb_Invoices_Details();
                        newdetail = item;
                        newdetail.MensajeError = "";
                        newdetail.DocEntryDevolucion = "";

                        newdetail.ID_factura = header.ID_factura;

                    }

                    dbkoncepto.Tb_Invoices_Details.AddRange(detailsInvoice);
                    dbkoncepto.SaveChanges();

                    ttresult = "SUCCESS";
                    return Json(ttresult, JsonRequestBehavior.AllowGet);
                }



                ttresult = "No data";
                return Json(ttresult, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ttresult = "ERROR: " + ex.Message;
                return Json(ttresult, JsonRequestBehavior.AllowGet);
            }




        }

        public ActionResult Invoices_history(string fstartd, string fendd)
        {
            if (generalClass.checkSession())
            {
                Sys_Users activeuser = Session["activeUser"] as Sys_Users;
                //PERMISOS
                List<string> s = new List<string>(activeuser.Departments.Split(new string[] { "," }, StringSplitOptions.None));
                ViewBag.lstDepartments = JsonConvert.SerializeObject(s);
                List<string> r = new List<string>(activeuser.Roles.Split(new string[] { "," }, StringSplitOptions.None));
                ViewBag.lstRoles = JsonConvert.SerializeObject(r);
                //PUNTOS DE VENTA
                List<string> puntosVenta = new List<string>(activeuser.ID_SalesPoint.Split(new string[] { "," }, StringSplitOptions.None));

                ///
                DateTime filtrostartdate;
                DateTime filtroenddate;
                //filtros de fecha (DIARIO)
                //var sunday = DateTime.Today;
                //var saturday = sunday.AddHours(23);
                ////filtros de fecha (MENSUAL)
                var sunday = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var saturday = sunday.AddMonths(1).AddDays(-1);

                var anteriorSunday = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);

                var anteriorSaturday = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);

                if (fstartd == null || fstartd == "") { filtrostartdate = sunday; } else { filtrostartdate = Convert.ToDateTime(fstartd); }
                if (fendd == null || fendd == "") { filtroenddate = saturday; } else { filtroenddate = Convert.ToDateTime(fendd).AddHours(23).AddMinutes(59); }

                ViewBag.filtrofechastart = filtrostartdate.ToShortDateString();
                ViewBag.filtrofechaend = filtroenddate.ToShortDateString();

                //
                var fechaactual = DateTime.Today;
                fechaactual = fechaactual.AddDays(-365);
                List<BI_Facturas_Encabezado> lstInvoices = (from d in SAPkoncepto.BI_Facturas_Encabezado
                                                            where (puntosVenta.Contains(d.Id_Sucursal) && d.Tipo=="FAC" && d.Fecha >= fechaactual)
                                                            select d).ToList();
                return View(lstInvoices);

            }
            else
            {

                return RedirectToAction("Signin", "Home", new { access = false });

            }
        }
    }
}