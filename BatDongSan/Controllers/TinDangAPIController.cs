using BatDongSan.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BatDongSan.Controllers
{
    public class TinDangAPIController : Controller
    {
        public JsonResult Index()
        {
            DataModel db = new DataModel();
            ArrayList a = db.getApi("exec XEMDANHSACHBATDONGSAN");
            return Json(a, JsonRequestBehavior.AllowGet);
        }
    }
}