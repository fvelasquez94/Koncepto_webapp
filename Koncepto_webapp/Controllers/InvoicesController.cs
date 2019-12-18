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

                var grupoCliente = SAPkoncepto.BI_Dim_Customers.Select(c => new grupo_clientes { id_grupo = c.Id_Grupo_Clientes, grupo = c.Grupo_Clientes }).Distinct().ToList();
                ViewBag.grupoCliente = grupoCliente;


               // var products = (from a in SAPkoncepto.BI_Dim_Productos where(a.Nombre_Producto != null && a.Linea != null && a.Marca != null) select a).ToList();


                var lstCategories = (from a in SAPkoncepto.BI_Dim_Productos where (a.Nombre_Producto != null && a.Linea != null && a.Marca != null && a.Id_Grupo_Productos != 100) select new grupo_productos { id_grupo = a.Id_Grupo_Productos, grupo = a.Grupo_Productos }).Distinct().OrderBy(c => c.grupo).ToList();
                ViewBag.lstCategories = lstCategories;

                //var lstSubCategories = (from a in SAPkoncepto.BI_Dim_Productos where (a.Nombre_Producto != null && a.Linea != null && a.Marca != null) select new Linea { id_linea = a.Id_Linea, linea = a.Linea, grupo = a.Grupo_Productos, id_grupo=a.Id_Grupo_Productos}).OrderBy(c => c.linea).ToList();
                //ViewBag.lstSubCategories = lstSubCategories;


                //var lstBrands = (from a in SAPkoncepto.BI_Dim_Productos where (a.Nombre_Producto != null && a.Linea != null && a.Marca != null) select new Marca { id_marca = a.Id_Marca, marca = a.Marca, linea = a.Linea,id_linea=a.Id_Linea, grupo=a.Grupo_Productos, id_grupo=a.Id_Grupo_Productos }).OrderBy(c => c.marca).ToList();
                //ViewBag.lstBrands = lstBrands;



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
    }
}