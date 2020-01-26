using Koncepto_webapp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Koncepto_webapp.Controllers
{
    public class HomeController : Controller
    {
        Koncept_dbEntities dbkoncepto = new Koncept_dbEntities();
        DB_KONCEPTOEntities SAPkoncepto = new DB_KONCEPTOEntities();

        //CLASS GENERAL
        private clsGeneral generalClass = new clsGeneral();

        public ActionResult Index(string fstartd, string fendd)
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
                List<BI_Facturas_Encabezado> lstInvoices = (from a in SAPkoncepto.BI_Facturas_Encabezado where ((a.Fecha >= filtrostartdate && a.Fecha <= filtroenddate) 
                                                            && puntosVenta.Contains(a.Id_Sucursal)) select a).ToList();

                List<BI_Facturas_Encabezado> earlier_lstInvoices = (from a in SAPkoncepto.BI_Facturas_Encabezado
                                                                    where ((a.Fecha >= anteriorSunday && a.Fecha <= anteriorSaturday) && puntosVenta.Contains(a.Id_Sucursal))
                                                                    select a).ToList();
                ViewBag.earlier_lstInvoices = earlier_lstInvoices;

                List<Tb_Invoices> lstInvoicesAzure = (from a in dbkoncepto.Tb_Invoices
                                                      where ((a.Fecha >= filtrostartdate && a.Fecha <= filtroenddate) && puntosVenta.Contains(a.ID_sucursal))
                                                      select a).ToList();
                ViewBag.lstinvoicesAzure = lstInvoicesAzure;


                return View(lstInvoices);

            }
            else
            {

                return RedirectToAction("Signin", "Home", new { access = false });

            }
        }

        public ActionResult Users()
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

                List<Sys_Users> lstusers = new List<Sys_Users>();
                lstusers = (from a in dbkoncepto.Sys_Users select a).ToList();


                var puntossventa = (from b in SAPkoncepto.C_SUCURSALES select b).ToList();
                ViewBag.puntosventa = puntossventa;
                return View(lstusers);

            }
            else
            {

                return RedirectToAction("Signin", "Home", new { access = false });

            }
        }


        public JsonResult EditUser(string id, string name, string email, string password, string telephone, string departments, string roles, string idsucursal, string sucursal, int idsap)
        {
            Sys_Users activeuser = Session["activeUser"] as Sys_Users;
            int idr = Convert.ToInt32(id);
            Sys_Users User = (from a in dbkoncepto.Sys_Users where (a.ID_User == idr) select a).FirstOrDefault();
            try
            {
                User.Name = name;
                User.Email = email;
                User.Password = password;
                User.Telephone = telephone;
                User.Departments = departments;
                User.Roles = roles;
                User.ID_SalesRep = idsap;
                User.ID_SalesPoint = idsucursal;
                User.Assigned_SalesPoint = sucursal;
                if (User.Departments == null)
                {
                    User.Departments = "";
                }
                if (User.Roles == null)
                {
                    User.Roles = "";
                }

                if (User.ID_SalesRep == null)
                {
                    User.ID_SalesRep = 0;
                }

                if (User.Telephone == null)
                {
                    User.Telephone = "";
                }

                dbkoncepto.Entry(User).State = EntityState.Modified;
                dbkoncepto.SaveChanges();

               
                return Json(new { Result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Something wrong happened, try again. " + ex.Message });
            }

        }


        public JsonResult CreateUser(string id, string name, string email, string password, string telephone, string departments, string roles, string idsucursal, string sucursal, int idsap)
        {
            Sys_Users activeuser = Session["activeUser"] as Sys_Users;
            Sys_Users User = new Sys_Users();
            try
            {
                User.Name = name;
                User.Email = email;
                User.Password = password;
                User.Telephone = telephone;
                User.Departments = departments;
                User.Roles = roles;
                User.ID_SalesRep = idsap;
                User.ID_SalesPoint = idsucursal;
                User.Assigned_SalesPoint = sucursal;

                if (User.Departments == null)
                {
                    User.Departments = "";
                }
                if (User.Roles == null)
                {
                    User.Roles = "";
                }

                if (User.ID_SalesRep == null)
                {
                    User.ID_SalesRep = 0;
                }

                if (User.Telephone == null)
                {
                    User.Telephone = "";
                }

                User.Active = true;
                User.Creation_date = DateTime.Now;
                dbkoncepto.Sys_Users.Add(User);
                dbkoncepto.SaveChanges();

                return Json(new { Result = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Something wrong happened, try again. " + ex.Message });
            }

        }

        public ActionResult Signin(bool access = true, int? logpage = 0)
        {
            if (access == false)
            {
                if (logpage == 0)
                {
                    TempData["advertencia"] = "Sesion expirada.";
                }
                else if (logpage == 1)
                {
                    TempData["advertencia"] = "Correo o contrasena incorrectos.";
                }
                else if (logpage == 3)
                {
                    TempData["advertencia"] = "Sesion finalizada.";
                }

            }

            HttpCookie aCookieCorreo = Request.Cookies["correo"];
            HttpCookie aCookiePassword = Request.Cookies["pass"];
            HttpCookie aCookieRememberme = Request.Cookies["rememberme"];

            try
            {
                var correo = Server.HtmlEncode(aCookieCorreo.Value).ToString();
                var pass = Server.HtmlEncode(aCookiePassword.Value).ToString();
                int remember = Convert.ToInt32(Server.HtmlEncode(aCookieRememberme.Value));

                if (remember == 1) { ViewBag.remember = true; } else { ViewBag.remember = false; }
                ViewBag.correo = correo;
                ViewBag.pass = pass;

            }
            catch
            {
                ViewBag.remember = false;

            }



            return View();
        }
        public ActionResult Log_out()
        {
            Session.RemoveAll();

            if (Request.Cookies["correo"] != null)
            {
                var c = new HttpCookie("correo");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
            if (Request.Cookies["pass"] != null)
            {
                var c = new HttpCookie("pass");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
            if (Request.Cookies["rememberme"] != null)
            {
                var c = new HttpCookie("rememberme");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }



            return RedirectToAction("Signin", "Home", new { access = false, logpage = 3 });
        }

        public ActionResult Sign_in(string email, string password, bool sesionactiva) 
        {
            Session["activeUser"] = (from a in dbkoncepto.Sys_Users where (a.Email == email && a.Password == password && a.Active == true) select a).FirstOrDefault();
           
            if (Session["activeUser"] != null)
            {
                Sys_Users activeuser = Session["activeUser"] as Sys_Users;

                ///PARA RECORDAR DATOS
                if (sesionactiva == true)
                {
                    if (Request.Cookies["correo"] != null)
                    {
                        //COMO YA EXISTE NO NECESITAMOS RECREARLA
                    }
                    else
                    {
                        HttpCookie aCookie = new HttpCookie("correo");
                        aCookie.Value = activeuser.Email.ToString();
                        aCookie.Expires = DateTime.Now.AddMonths(3);

                        HttpCookie aCookie2 = new HttpCookie("pass");
                        aCookie2.Value = activeuser.Password.ToString();
                        aCookie2.Expires = DateTime.Now.AddMonths(3);

                        HttpCookie aCookie3 = new HttpCookie("rememberme");
                        aCookie3.Value = "1";
                        aCookie3.Expires = DateTime.Now.AddMonths(3);


                        Response.Cookies.Add(aCookie);
                        Response.Cookies.Add(aCookie2);
                        Response.Cookies.Add(aCookie3);
                    }
                }
                //if (activeuser.Departments.Contains("Operations"))
                //{
                //    if (activeuser.Roles.Contains("Purchases"))
                //    {
                //        return RedirectToAction("Dashboard_OperationsPurchases", "Main", null);
                //    }
                //    else
                //    {
                //        return RedirectToAction("Dashboard_operations", "Main", null);
                //    }


                //}

                return RedirectToAction("Index", "Home", null);

            }

            return RedirectToAction("Signin", "Home", new { access = false, logpage = 1 });
        }

        public ActionResult getUser(string id_user)
        {

            try
            {

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var id = Convert.ToInt32(id_user);
               
                var user2 = (from a in dbkoncepto.Sys_Users
                             where (a.ID_User == id)
                             select a).ToArray();
                var result2 = javaScriptSerializer.Serialize(user2);
                var result = new { result =result2 };

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string result = "ERROR: " + ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult activeUser(int iduser, bool selected)
        {
            try
            {
                var usuario = dbkoncepto.Sys_Users.Find(iduser);
                if (usuario != null)
                {
                    usuario.Active = selected;
                    dbkoncepto.Entry(usuario).State = EntityState.Modified;
                    dbkoncepto.SaveChanges();
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json("warning", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            


        }

    }
}