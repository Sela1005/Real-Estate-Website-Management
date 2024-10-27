using BatDongSan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BatDongSan.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult XulyDangNhapAdmin(string email, string password)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC KIEMTRADANGNHAP '" + email + "','" + password + "'");
            if (ViewBag.list.Count > 0)
            {
                Session["taikhoan"] = ViewBag.list[0];
                return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Login");
        }
    }
}