using BatDongSan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;

namespace BatDongSan.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("Select * from BatDongSan");
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult XulyDangNhap(string username, string password)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("EXEC KIEMTRADANGNHAP '" + username + "','" + password + "'");
            if (ViewBag.list.Count > 0)
            {
                Session["taikhoan"] = ViewBag.list[0];
                return RedirectToAction("Index", "Quanly");
            }
            else
                return RedirectToAction("Index", "Home");
        }
        public ActionResult Services()
        {
            return View();
        }
        
        public ActionResult Properties()
        {
            return View();
        }
        
        public ActionResult Properties_Detail(string id, string idNguoiDung)
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("exec TIMKIEMBDSTHEOID " + id + ";");
            ViewBag.listUser = db.get("exec TIMKIEMNguoiDungTHEOID " + idNguoiDung + ";");
            return View();
        }
    }
}