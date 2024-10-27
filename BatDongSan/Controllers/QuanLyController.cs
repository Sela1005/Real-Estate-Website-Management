using BatDongSan.Filters;
using BatDongSan.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BatDongSan.Controllers
{
    [AdminAuthorize]
    public class QuanLyController : Controller
    {
        // GET: QuanLy
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult QuanLyBatDongSan()
        {
            DataModel db = new DataModel();
            ViewBag.listBDS = db.get("select * from BatDongSan");
            return View();
        }
        public ActionResult ThemBDS()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemBDSMoi(string tieude,
                                        string mota,
                                        string gia,
                                        HttpPostedFileBase hinh, /*HttpPostedFileBase: cho phep nhận dữ liệu "hinh" là file*/
                                        string dientich,
                                        string vitri,
                                        string idnguoidung,
                                        string loaibds,
                                        string loaihinh,
                                        string tinhtrangphaply)
        {
            try
            {
                if (hinh != null && hinh.ContentLength > 0)
                {
                    string filename = Path.GetFileName(hinh.FileName);
                    string path = Path.Combine(Server.MapPath("~/images"), filename);
                    hinh.SaveAs(path);
                    DataModel db = new DataModel();
                    db.get("exec ThemBDS N'"+ tieude + "', N'"+ mota + "', "+ gia + ", "+ dientich + ", N'"+ vitri + "', "+ idnguoidung + ", N'"+ loaibds + "', N'"+ loaihinh + "', N'"+ tinhtrangphaply + "', '"+ hinh + "'");
                }
            }
            catch (Exception) { }
            return RedirectToAction("QuanLyBatDongSan", "QuanLy");
        }
    }
}