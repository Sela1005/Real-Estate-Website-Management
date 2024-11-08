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
            ViewBag.list = db.get("exec KIEMTRADANGNHAP '"+ username + "' ,'"+ password + "'");
            
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
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(idNguoiDung) || !int.TryParse(id, out _) || !int.TryParse(idNguoiDung, out _))
            {
                return RedirectToAction("Index", "Home"); // Điều hướng về trang chủ nếu thiếu dữ liệu hoặc dữ liệu không hợp lệ
            }
            try
            {
                DataModel db = new DataModel();
                ViewBag.list = db.get("exec TIMKIEMBDSTHEOID " + id + ";");
                ViewBag.listUser = db.get("exec TIMKIEMNguoiDungTHEOID " + idNguoiDung + ";");
            }
            catch(Exception) { return RedirectToAction("Index", "Home"); }
            return View();
        }
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult XulyDangKy(string username, string password, string email, string confrimpassword)
        {
            DataModel db = new DataModel();

            if(confrimpassword != password)
            {
                TempData["ErrorMessage"] = "Vui lòng nhập mật khẩu và Nhập lại mật khẩu giống nhau";
                return RedirectToAction("Dangky", "Home"); // Chuyển đến action đăng ký
            }

            // Thực hiện thủ tục lưu trữ
            ViewBag.list = db.get("EXEC SP_DangKy N'" + username + "', N'" + password + "', N'" + email + "'");

            // Kiểm tra kết quả trả về
            if (ViewBag.list.Count > 0)
            {
                int result = Convert.ToInt32(ViewBag.list[0][0]);

                if (result == 1)
                {
                    // Đăng ký thành công
                    return RedirectToAction("Dangnhap", "home");
                }
                else if (result == 0)
                {
                    TempData["ErrorMessage"] = "Email này đã tồn tại. Vui lòng thử lại với email khác.";
                    return RedirectToAction("Dangky", "Home"); // Chuyển đến action đăng ký
                }
                else
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại.";
                    return RedirectToAction("Dangky", "Home");
                }
            }

            TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại.";
            return RedirectToAction("Dangky", "Home");
        }
        public ActionResult TimKiemTieuDeBDS(string TieuDe)
        {
            if (string.IsNullOrEmpty(TieuDe))
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                DataModel db = new DataModel();
                ViewBag.list = db.get("EXEC TIMKIEMBDSTHEOTIEUDE N'" + TieuDe + "'");
            }
            catch (Exception)
            {
                RedirectToAction("Index", "Home"); 
            }
            
            return View();
        }


    }
}