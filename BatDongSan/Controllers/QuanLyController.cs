using BatDongSan.Filters;
using BatDongSan.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public ActionResult QuanLyNguoiDung()
        {
            DataModel db = new DataModel();
            ViewBag.listUser = db.get("select * from NguoiDung");
            return View();
        }
        public ActionResult ThemBDS()
        {
            return View();
        }
        /*[HttpPost]
        public ActionResult ThemBDSMoi(string tieude,
                                        string mota,
                                        string gia,
                                        HttpPostedFileBase hinh, *//*HttpPostedFileBase: cho phep nhận dữ liệu "hinh" là file*//*
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
                    db.get("exec ThemBDS N'"+ tieude + "', N'"+ mota + "', "+ gia + ", "+ dientich + ", N'"+ vitri + "', "+ idnguoidung + ", N'"+ loaibds + "', N'"+ loaihinh + "', N'"+ tinhtrangphaply + "', '"+ hinh.FileName + "'");
                }
            }
            catch (Exception) { }
            return RedirectToAction("QuanLyBatDongSan", "QuanLy");
        }*/
        [HttpPost]
        public ActionResult ThemBDSMoi(string tieude,
                                string mota,
                                string gia,
                                HttpPostedFileBase hinh,
                                string dientich,
                                string vitri,
                                string idnguoidung,
                                string loaibds,
                                string loaihinh,
                                string tinhtrangphaply)
        {
            try
            {
                // Kiểm tra các trường dữ liệu
                if (string.IsNullOrEmpty(tieude) || tieude.Length < 25 || tieude.Length > 100)
                {
                    TempData["ErrorMessage"] = "tên dự án phải từ 25 đến 100 ký tự và không được bỏ trống.";
                    return RedirectToAction("ThemBDS", "Quanly");
                }
                if (string.IsNullOrEmpty(gia) || !decimal.TryParse(gia, out decimal giaDecimal) || giaDecimal.ToString().Length < 7 || giaDecimal.ToString().Length > 13)
                {
                    TempData["ErrorMessage"] = "Giá phải là một số từ 7 đến 13 ký tự.";
                    return RedirectToAction("ThemBDS", "Quanly");
                }
                if (string.IsNullOrEmpty(dientich) || !decimal.TryParse(dientich, out decimal dientichDecimal) || dientichDecimal.ToString().Length < 2 || dientichDecimal.ToString().Length > 10)
                {
                    TempData["ErrorMessage"] = "Diện tích phải là một số có độ dài từ 2 đến 10 ký tự.";
                    return RedirectToAction("ThemBDS", "Quanly");
                }
                if (string.IsNullOrEmpty(vitri) || vitri.Length < 30 || vitri.Length > 200)
                {
                    TempData["ErrorMessage"] = "Vị trí phải từ 30 đến 200 ký tự và không được bỏ trống.";
                    return RedirectToAction("ThemBDS", "Quanly");
                }
                if (string.IsNullOrEmpty(loaibds))
                {
                    TempData["ErrorMessage"] = "Loại bất động sản không được bỏ trống.";
                    return RedirectToAction("ThemBDS", "Quanly");
                }
                if (string.IsNullOrEmpty(loaihinh) || loaihinh.Length < 5 || loaihinh.Length > 20)
                {
                    TempData["ErrorMessage"] = "Loại hình phải từ 5 đến 20 ký tự và không được bỏ trống.";
                    return RedirectToAction("ThemBDS", "Quanly");
                }
                if (string.IsNullOrEmpty(tinhtrangphaply) || tinhtrangphaply.Length < 5 || tinhtrangphaply.Length > 50)
                {
                    TempData["ErrorMessage"] = "Tình trạng pháp lý phải từ 5 đến 50 ký tự và không được bỏ trống.";
                    return RedirectToAction("ThemBDS", "Quanly");
                }
                if (string.IsNullOrEmpty(idnguoidung))
                {
                    TempData["ErrorMessage"] = "ID người dùng không hợp lệ.";
                    return RedirectToAction("ThemBDS", "Quanly");
                }

                // Kiểm tra hình ảnh
                if (hinh == null || hinh.ContentLength == 0)
                {
                    return RedirectToAction("ThemBDS", "QuanLy");
                }

                // Kiểm tra định dạng ảnh
                string fileExtension = Path.GetExtension(hinh.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return RedirectToAction("ThemBDS", "QuanLy");
                }

                // Xử lý ảnh
                string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                string path = Path.Combine(Server.MapPath("~/images"), uniqueFileName);

                // Mở ảnh từ HttpPostedFileBase
                using (var img = Image.FromStream(hinh.InputStream))
                {
                    int newWidth = img.Width;
                    int newHeight = img.Height;

                    // Nếu ảnh có kích thước lớn hơn 500x500, thay đổi kích thước
                    if (img.Width > 500 || img.Height > 500)
                    {
                        if (img.Width > img.Height)
                        {
                            newWidth = 500;
                            newHeight = (int)(img.Height * (500.0 / img.Width));
                        }
                        else
                        {
                            newHeight = 500;
                            newWidth = (int)(img.Width * (500.0 / img.Height));
                        }
                    }

                    // Resize ảnh để phù hợp với 500x500 mà không làm biến dạng ảnh
                    var resizedImg = new Bitmap(img, new Size(newWidth, newHeight));
                    resizedImg.Save(path);
                }

                // Lưu thông tin vào cơ sở dữ liệu
                DataModel db = new DataModel();
                db.get("exec ThemBDS N'" + tieude + "', N'" + mota + "', " + gia + ", " + dientich + ", N'" + vitri + "', " + idnguoidung + ", N'" + loaibds + "', N'" + loaihinh + "', N'" + tinhtrangphaply + "', '" + uniqueFileName + "'");

                return RedirectToAction("QuanLyBatDongSan", "QuanLy");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("ThemBDS", "QuanLy");
            }
        }
        [HttpPost]
        public ActionResult XoaBDS(string id)//cần chỉnh lại điều kiện
        {
            try
            {
                if (string.IsNullOrEmpty(id)|| !int.TryParse(id, out _))
                {
                    TempData["ErrorMessage"] = "Không tìm thấy id";
                    return RedirectToAction("QuanLyBatDongSan", "Quanly");
                }
                DataModel db = new DataModel(); 
                db.get("exec XOABDS "+id+"");
                TempData["SuccessMessage"] = "Đã xóa thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction("QuanLyBatDongSan", "Quanly");
        }
        public ActionResult SuaBDS(string id, string idNguoiDung)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(idNguoiDung) || !int.TryParse(id, out _) || !int.TryParse(idNguoiDung, out _))
            {
                TempData["ErrorMessage"] = "Không tìm thấy id";
                return RedirectToAction("QuanLyBatDongSan", "QuanLy"); // Điều hướng về trang chủ nếu thiếu dữ liệu hoặc dữ liệu không hợp lệ
            }
            try
            {
                DataModel db = new DataModel();
                ViewBag.list = db.get("exec TIMKIEMBDSTHEOID " + id + ";");
                ViewBag.listUser = db.get("exec TIMKIEMNguoiDungTHEOID " + idNguoiDung + ";");
            }
            catch (Exception) { return RedirectToAction("QuanLyBatDongSan", "QuanLy"); }
            return View();
        }
        [HttpPost]
        public ActionResult SuaBDS(string tieude,
                            string mota,
                            string gia,
                            HttpPostedFileBase hinh,
                            string dientich,
                            string vitri,
                            string idnguoidung,
                            string loaibds,
                            string loaihinh,
                            string tinhtrangphaply,
                            string idbatdongsan,
                            string existingImage)
        {
            try
            {
                // Kiểm tra các trường dữ liệu
                if (string.IsNullOrEmpty(tieude) || tieude.Length < 25 || tieude.Length > 100)
                {
                    TempData["ErrorMessage"] = "tên dự án phải từ 25 đến 100 ký tự và không được bỏ trống.";
                    return RedirectToAction("QuanLyBatDongSan", "Quanly");
                }
                if (string.IsNullOrEmpty(gia) || !decimal.TryParse(gia, out decimal giaDecimal) || giaDecimal.ToString().Length < 7 || giaDecimal.ToString().Length > 13)
                {
                    TempData["ErrorMessage"] = "Giá phải là một số từ 7 đến 13 ký tự.";
                    return RedirectToAction("QuanLyBatDongSan", "Quanly");
                }
                if (string.IsNullOrEmpty(dientich) || !decimal.TryParse(dientich, out decimal dientichDecimal) || dientichDecimal.ToString().Length < 2 || dientichDecimal.ToString().Length > 10)
                {
                    TempData["ErrorMessage"] = "Diện tích phải là một số có độ dài từ 2 đến 10 ký tự.";
                    return RedirectToAction("QuanLyBatDongSan", "Quanly");
                }
                if (string.IsNullOrEmpty(vitri) || vitri.Length < 30 || vitri.Length > 200)
                {
                    TempData["ErrorMessage"] = "Vị trí phải từ 30 đến 200 ký tự và không được bỏ trống.";
                    return RedirectToAction("QuanLyBatDongSan", "Quanly");
                }
                if (string.IsNullOrEmpty(loaibds))
                {
                    TempData["ErrorMessage"] = "Loại bất động sản không được bỏ trống.";
                    return RedirectToAction("QuanLyBatDongSan", "Quanly");
                }
                if (string.IsNullOrEmpty(loaihinh) || loaihinh.Length < 5 || loaihinh.Length > 20)
                {
                    TempData["ErrorMessage"] = "Loại hình phải từ 5 đến 20 ký tự và không được bỏ trống.";
                    return RedirectToAction("QuanLyBatDongSan", "Quanly");
                }
                if (string.IsNullOrEmpty(tinhtrangphaply) || tinhtrangphaply.Length < 5 || tinhtrangphaply.Length > 50)
                {
                    TempData["ErrorMessage"] = "Tình trạng pháp lý phải từ 5 đến 50 ký tự và không được bỏ trống.";
                    return RedirectToAction("QuanLyBatDongSan", "Quanly");
                }
                if (string.IsNullOrEmpty(idnguoidung))
                {
                    TempData["ErrorMessage"] = "ID người dùng không hợp lệ.";
                    return RedirectToAction("QuanLyBatDongSan", "Quanly");
                }

                // Kiểm tra nếu người dùng không thay đổi hình ảnh
                string imageFileName = existingImage;  // Dùng ảnh cũ nếu người dùng không chọn ảnh mới

                // Kiểm tra hình ảnh nếu người dùng đã tải lên ảnh mới
                if (hinh != null && hinh.ContentLength > 0)
                {
                    // Kiểm tra định dạng ảnh
                    string fileExtension = Path.GetExtension(hinh.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return RedirectToAction("ThemBDS", "QuanLy");
                    }

                    // Xử lý ảnh
                    string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                    string path = Path.Combine(Server.MapPath("~/images"), uniqueFileName);

                    // Mở ảnh từ HttpPostedFileBase
                    using (var img = Image.FromStream(hinh.InputStream))
                    {
                        int newWidth = img.Width;
                        int newHeight = img.Height;

                        // Nếu ảnh có kích thước lớn hơn 500x500, thay đổi kích thước
                        if (img.Width > 500 || img.Height > 500)
                        {
                            if (img.Width > img.Height)
                            {
                                newWidth = 500;
                                newHeight = (int)(img.Height * (500.0 / img.Width));
                            }
                            else
                            {
                                newHeight = 500;
                                newWidth = (int)(img.Width * (500.0 / img.Height));
                            }
                        }

                        // Resize ảnh để phù hợp với 500x500 mà không làm biến dạng ảnh
                        var resizedImg = new Bitmap(img, new Size(newWidth, newHeight));
                        resizedImg.Save(path);
                    }

                    // Cập nhật tên ảnh mới
                    imageFileName = uniqueFileName;
                }

                // Lưu thông tin vào cơ sở dữ liệu
                DataModel db = new DataModel();
                db.get("exec SUABDS N'" + tieude + "', N'" + mota + "', " + gia + ", " + dientich + ", N'" + vitri + "', " + idnguoidung + ", N'" + loaibds + "', N'" + loaihinh + "', N'" + tinhtrangphaply + "', '" + imageFileName + "', " + idbatdongsan);
                TempData["SuccessMessage"] = "Chỉnh sửa Bất Động Sản Thành Công";
                return RedirectToAction("QuanLyBatDongSan", "QuanLy");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("SuaBDS", "QuanLy");
            }
        }
        [HttpPost]
        public ActionResult XoaNguoiDung(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || !int.TryParse(id, out _))
                {
                    TempData["ErrorMessage"] = "Không tìm thấy id";
                    return RedirectToAction("QuanLyNguoiDung", "Quanly");
                }
                DataModel db = new DataModel();
                db.get("exec XOANGUOIDUNG "+id+"");
                TempData["SuccessMessage"] = "Đã xóa thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("QuanLyNguoiDung", "Quanly");
            }

            return RedirectToAction("QuanLyNguoiDung", "Quanly");
        }
        public ActionResult SuaNguoiDung(string idNguoiDung)
        {
            if ( string.IsNullOrEmpty(idNguoiDung)|| !int.TryParse(idNguoiDung, out _))
            {
                TempData["ErrorMessage"] = "Không tìm thấy id";
                return RedirectToAction("QuanLyNguoiDung", "QuanLy"); // Điều hướng về trang chủ nếu thiếu dữ liệu hoặc dữ liệu không hợp lệ
            }
            try
            {
                DataModel db = new DataModel();
                ViewBag.listUser = db.get("exec TIMKIEMNguoiDungTHEOID " + idNguoiDung + ";");
            }
            catch (Exception) { return RedirectToAction("QuanLyNguoiDung", "QuanLy"); }
            return View();
        }
        [HttpPost]
        public ActionResult SUANGUOIDUNG(string tennguoidung,
                            HttpPostedFileBase hinh,
                            string vaitro,
                            string email,
                            string sodienthoai,
                            string existingImage,
                            string idnguoidung)
        {
            try
            {
                // Kiểm tra các trường dữ liệu
                if (string.IsNullOrEmpty(tennguoidung) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(sodienthoai) || string.IsNullOrEmpty(idnguoidung))
                {
                    TempData["ErrorMessage"] = "Vui lòng không bỏ trống các trường!";
                    return RedirectToAction("SuaNguoiDung", "Quanly", new { idNguoiDung = idnguoidung });
                }

                // Kiểm tra độ dài và tính hợp lệ của các trường
                if (tennguoidung.Length < 5 || tennguoidung.Length > 30)
                {
                    TempData["ErrorMessage"] = "Tên người dùng phải từ 5 đến 30 ký tự!";
                    return RedirectToAction("SuaNguoiDung", "Quanly", new { idNguoiDung = idnguoidung });
                }

                if (sodienthoai.Length < 7 || sodienthoai.Length > 15 || !sodienthoai.All(char.IsDigit))
                {
                    TempData["ErrorMessage"] = "Số điện thoại phải từ 7 đến 15 ký tự và chỉ chứa số!";
                    return RedirectToAction("SuaNguoiDung", "Quanly", new { idNguoiDung = idnguoidung });
                }

                if (email.Length < 10 || email.Length > 30 || !email.Contains("@"))
                {
                    TempData["ErrorMessage"] = "Email phải có ít nhất 10 ký tự, tối đa 30 ký tự và chứa ký tự '@'!";
                    return RedirectToAction("SuaNguoiDung", "Quanly", new { idNguoiDung = idnguoidung });
                }

                // Kiểm tra nếu người dùng không thay đổi hình ảnh
                string imageFileName = existingImage;

                // Kiểm tra hình ảnh nếu người dùng đã tải lên ảnh mới
                if (hinh != null && hinh.ContentLength > 0)
                {
                    // Kiểm tra định dạng ảnh
                    string fileExtension = Path.GetExtension(hinh.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        TempData["ErrorMessage"] = "Định dạng hình không hợp lệ!";
                        return RedirectToAction("SuaNguoiDung", "Quanly", new { idNguoiDung = idnguoidung });
                    }
                    // Xử lý ảnh
                    string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                    string path = Path.Combine(Server.MapPath("~/images"), uniqueFileName);

                    // Mở ảnh từ HttpPostedFileBase
                    using (var img = Image.FromStream(hinh.InputStream))
                    {
                        int newWidth = img.Width;
                        int newHeight = img.Height;

                        // Nếu ảnh có kích thước lớn hơn 500x500, thay đổi kích thước
                        if (img.Width > 500 || img.Height > 500)
                        {
                            if (img.Width > img.Height)
                            {
                                newWidth = 500;
                                newHeight = (int)(img.Height * (500.0 / img.Width));
                            }
                            else
                            {
                                newHeight = 500;
                                newWidth = (int)(img.Width * (500.0 / img.Height));
                            }
                        }
                        // Resize ảnh để phù hợp với 500x500 mà không làm biến dạng ảnh
                        var resizedImg = new Bitmap(img, new Size(newWidth, newHeight));
                        resizedImg.Save(path);
                    }
                    // Cập nhật tên ảnh mới
                    imageFileName = uniqueFileName;
                }
                // Lưu thông tin vào cơ sở dữ liệu
                DataModel db = new DataModel();
                db.get("exec SUANGUOIDUNGADMIN N'" + tennguoidung + "', '" + email + "', '" + sodienthoai + "',N'"+vaitro+"', N'" + imageFileName + "', " + idnguoidung + "");
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("QuanLyNguoiDung", "QuanLy");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("SuaNguoiDung", "Quanly", new { idNguoiDung = idnguoidung });
            }
        }

    }
}