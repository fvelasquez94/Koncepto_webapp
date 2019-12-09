using Koncepto_webapp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Koncepto_webapp.Controllers
{
    public class InvoicesController : Controller
    {
        Koncept_dbEntities dbkoncepto = new Koncept_dbEntities();
        DB_KONCEPTOEntities SAPkoncepto = new DB_KONCEPTOEntities();

        //CLASS GENERAL
        private clsGeneral generalClass = new clsGeneral();
        // GET: Invoices
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




                return View();

            }
            else
            {

                return RedirectToAction("Signin", "Home", new { access = false });

            }
        }
    }
}