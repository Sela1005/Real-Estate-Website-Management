using BatDongSan.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
                // Lưu thông tin đăng nhập vào session
                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                Session["taikhoan"] = ViewBag.list[0];

                // Kiểm tra vai trò (role) tại vị trí thứ 5
                string role = ViewBag.list[0][5];

                // Nếu vai trò là admin, điều hướng đến trang quản lý
                if (role == "Admin")
                {
                    return RedirectToAction("Index", "Quanly");
                }
                else
                {
                    // Nếu không phải admin, điều hướng đến trang chủ
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Thông tin đăng nhập không hợp lệ, hãy kiểm tra email và mật khẩu.";
                return RedirectToAction("DangNhap", "Home");
            }
        }
        [HttpPost]
        public ActionResult XulyDangXuat()
        {
            try
            {
                // Xóa thông tin người dùng trong session
                Session["taikhoan"] = null;

                // Xóa tất cả các session khác nếu cần (tùy vào cấu trúc ứng dụng của bạn)
                Session.Clear(); // Dùng nếu bạn muốn xóa tất cả session.

                // Sau khi đăng xuất, chuyển hướng người dùng về trang đăng nhập hoặc trang chủ
                TempData["SuccessMessage"] = "Đăng xuất thành công!";
                return RedirectToAction("Index", "Home"); // Điều hướng về trang đăng nhập
            }catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
            
        }
        public ActionResult DangTin()
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            System.Collections.ArrayList tenDaDangNhap = (System.Collections.ArrayList)Session["taikhoan"];
            if (tenDaDangNhap == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đăng tin.";
                return RedirectToAction("DangNhap", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult DangTin(string tieude,
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
                if (string.IsNullOrEmpty(tieude) || tieude.Length < 25 || tieude.Length > 100)
                {
                    TempData["ErrorMessage"] = "tên dự án phải từ 25 đến 100 ký tự và không được bỏ trống.";
                    return RedirectToAction("DangTin", "Home");
                }
                if (string.IsNullOrEmpty(gia) || !decimal.TryParse(gia, out decimal giaDecimal) || giaDecimal.ToString().Length < 7 || giaDecimal.ToString().Length > 13)
                {
                    TempData["ErrorMessage"] = "Giá phải là một số từ 7 đến 13 ký tự.";
                    return RedirectToAction("DangTin", "Home");
                }
                if (string.IsNullOrEmpty(dientich) || !decimal.TryParse(dientich, out decimal dientichDecimal) || dientichDecimal.ToString().Length < 2 || dientichDecimal.ToString().Length > 10)
                {
                    TempData["ErrorMessage"] = "Diện tích phải là một số có độ dài từ 2 đến 10 ký tự.";
                    return RedirectToAction("DangTin", "Home");
                }
                if (string.IsNullOrEmpty(vitri) || vitri.Length < 30 || vitri.Length > 200)
                {
                    TempData["ErrorMessage"] = "Vị trí phải từ 30 đến 200 ký tự và không được bỏ trống.";
                    return RedirectToAction("DangTin", "Home");
                }
                if (string.IsNullOrEmpty(loaibds))
                {
                    TempData["ErrorMessage"] = "Loại bất động sản không được bỏ trống.";
                    return RedirectToAction("DangTin", "Home");
                }
                if (string.IsNullOrEmpty(loaihinh) || loaihinh.Length < 5 || loaihinh.Length > 20)
                {
                    TempData["ErrorMessage"] = "Loại hình phải từ 5 đến 20 ký tự và không được bỏ trống.";
                    return RedirectToAction("DangTin", "Home");
                }
                if (string.IsNullOrEmpty(tinhtrangphaply) || tinhtrangphaply.Length < 5 || tinhtrangphaply.Length > 50)
                {
                    TempData["ErrorMessage"] = "Tình trạng pháp lý phải từ 5 đến 50 ký tự và không được bỏ trống.";
                    return RedirectToAction("DangTin", "Home");
                }
                if (string.IsNullOrEmpty(idnguoidung))
                {
                    TempData["ErrorMessage"] = "ID người dùng không hợp lệ.";
                    return RedirectToAction("DangTin", "Home");
                }
                // Kiểm tra hình ảnh
                if (hinh == null || hinh.ContentLength == 0)
                {
                    TempData["ErrorMessage"] = "Ảnh không được bỏ trống.";
                    return RedirectToAction("DangTin", "Home");
                }

                // Kiểm tra định dạng ảnh
                string fileExtension = Path.GetExtension(hinh.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessage"] = "Vui lòng chỉ chọn ảnh định dạng: \".jpg\", \".jpeg\", \".png\", \".gif\", \".bmp\" .";
                    return RedirectToAction("DangTin", "Home");
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
                TempData["SuccessMessage"] = "Đăng tin thành công!";
                return RedirectToAction("QuanLyTinDang", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("DangTin", "Home");
            }
        }

        public ActionResult QuanLyTinDang()
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            System.Collections.ArrayList tenDaDangNhap = (System.Collections.ArrayList)Session["taikhoan"];
            if (tenDaDangNhap == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để vào trang quản lý tin đăng.";
                return RedirectToAction("DangNhap", "Home");
            }
            DataModel db = new DataModel();
            ViewBag.listBDS = db.get("exec TIMKIEMBDSTHEONGUOIDUNG "+ tenDaDangNhap[0]+ "");
            if (ViewBag.listBDS == null || ViewBag.listBDS.Count == 0)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                TempData["ErrorMessage"] = "Bạn chưa có tin đăng nào cả.";
            }
            return View();
        }
        [HttpPost]
        public ActionResult XoaBDS(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || !int.TryParse(id, out _))
                {
                    TempData["ErrorMessage"] = "Không tìm thấy id";
                    return RedirectToAction("QuanLyTinDang", "Home");
                }
                DataModel db = new DataModel();
                db.get("exec XOABDS " + id + "");
                TempData["SuccessMessage"] = "Đã xóa thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra: " + ex.Message;
            }
            return RedirectToAction("QuanLyTinDang", "Home");
        }
        public ActionResult SuaBDS(string id)
        {
            System.Collections.ArrayList tenDaDangNhap = (System.Collections.ArrayList)Session["taikhoan"];
            if (tenDaDangNhap == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để sửa tin.";
                return RedirectToAction("DangNhap", "Home");
            }
            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out _))
            {
                TempData["ErrorMessage"] = "Không tìm thấy id";
                return RedirectToAction("QuanLyTinDang", "Home"); // Điều hướng về trang chủ nếu thiếu dữ liệu hoặc dữ liệu không hợp lệ
            }
            try
            {
                DataModel db = new DataModel();
                ViewBag.list = db.get("exec TIMKIEMBDSTHEOID " + id + ";");
            }
            catch (Exception) { return RedirectToAction("QuanLyTinDang", "Home"); }
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
                    return RedirectToAction("SuaBDS", "Home", new { id = idnguoidung });
                }
                if (string.IsNullOrEmpty(gia) || !decimal.TryParse(gia, out decimal giaDecimal) || giaDecimal.ToString().Length < 7 || giaDecimal.ToString().Length > 13)
                {
                    TempData["ErrorMessage"] = "Giá phải là một số từ 7 đến 13 ký tự.";
                    return RedirectToAction("SuaBDS", "Home", new { id = idnguoidung });
                }
                if (string.IsNullOrEmpty(dientich) || !decimal.TryParse(dientich, out decimal dientichDecimal) || dientichDecimal.ToString().Length < 2 || dientichDecimal.ToString().Length > 10)
                {
                    TempData["ErrorMessage"] = "Diện tích phải là một số có độ dài từ 2 đến 10 ký tự.";
                    return RedirectToAction("SuaBDS", "Home", new { id = idnguoidung });
                }
                if (string.IsNullOrEmpty(vitri) || vitri.Length < 30 || vitri.Length > 200)
                {
                    TempData["ErrorMessage"] = "Vị trí phải từ 30 đến 200 ký tự và không được bỏ trống.";
                    return RedirectToAction("SuaBDS", "Home", new { id = idnguoidung });
                }
                if (string.IsNullOrEmpty(loaibds))
                {
                    TempData["ErrorMessage"] = "Loại bất động sản không được bỏ trống.";
                    return RedirectToAction("SuaBDS", "Home", new { id = idnguoidung });
                }
                if (string.IsNullOrEmpty(loaihinh) || loaihinh.Length < 5 || loaihinh.Length > 20)
                {
                    TempData["ErrorMessage"] = "Loại hình phải từ 5 đến 20 ký tự và không được bỏ trống.";
                    return RedirectToAction("SuaBDS", "Home", new { id = idnguoidung });
                }
                if (string.IsNullOrEmpty(tinhtrangphaply) || tinhtrangphaply.Length < 5 || tinhtrangphaply.Length > 50)
                {
                    TempData["ErrorMessage"] = "Tình trạng pháp lý phải từ 5 đến 50 ký tự và không được bỏ trống.";
                    return RedirectToAction("SuaBDS", "Home", new { id = idnguoidung });
                }
                if (string.IsNullOrEmpty(idnguoidung))
                {
                    TempData["ErrorMessage"] = "ID người dùng không hợp lệ.";
                    return RedirectToAction("SuaBDS", "Home", new { id = idnguoidung });
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
                        TempData["ErrorMessage"] = "Định dạng hình không hợp lệ.";
                        return RedirectToAction("SuaBDS", "Home", new { id = idnguoidung });
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
                return RedirectToAction("Quanlytindang", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("SuaBDS", "Home", new { id = idnguoidung });
            }
        }
        public ActionResult Properties()
        {
            DataModel db = new DataModel();
            ViewBag.list = db.get("Select * from BatDongSan");
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
                System.Collections.ArrayList tenDaDangNhap = Session["taikhoan"] as System.Collections.ArrayList;
                if (tenDaDangNhap == null)
                {
                    ViewBag.list = db.get("exec TIMKIEMBDSTHEOID " + id + ";");
                    ViewBag.listUser = db.get("exec TIMKIEMNguoiDungTHEOID " + idNguoiDung + ";");

                }
                else
                {
                    ViewBag.list = db.get("exec TIMKIEMBDSTHEOID " + id + ";");
                    ViewBag.listUser = db.get("exec TIMKIEMNguoiDungTHEOID " + idNguoiDung + ";");
                    ViewBag.listKiemTraYeuThich = db.get("exec KIEMTRATINYEUTHICH " + tenDaDangNhap[0] + ", " + id + "");
                }
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

            // Kiểm tra độ dài và tính hợp lệ của username, password và email
            if (string.IsNullOrEmpty(username) || username.Length < 5 || username.Length > 30)
            {
                TempData["ErrorMessage"] = "Tên người dùng phải từ 5 đến 30 ký tự và không được để trống!";
                return RedirectToAction("Dangky", "Home"); 
            }

            if (string.IsNullOrEmpty(password) || password.Length < 6 || password.Length > 30)
            {
                TempData["ErrorMessage"] = "Mật khẩu phải từ 6 đến 30 ký tự và không được để trống!";
                return RedirectToAction("Dangky", "Home");
            }

            if (confrimpassword != password)
            {
                TempData["ErrorMessage"] = "Mật khẩu và Nhập lại mật khẩu không khớp!";
                return RedirectToAction("Dangky", "Home");
            }

            if (string.IsNullOrEmpty(email) || email.Length < 6 || email.Length > 30 || !email.Contains("@"))
            {
                TempData["ErrorMessage"] = "Email phải chứa '@' và không được để trống!";
                return RedirectToAction("Dangky", "Home");
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
                    TempData["SuccessMessage"] = "Đăng ký tài khoản thành công!";
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
                TempData["ErrorMessage"] = "Bạn chưa nhập thông tin tìm kiếm.";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                DataModel db = new DataModel();
                ViewBag.list = db.get("EXEC TIMKIEMBDSTHEOTIEUDE N'" + TieuDe + "'");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi:"+ ex.Message;
                RedirectToAction("Index", "Home"); 
            }
            
            return View();
        }
        public ActionResult ThongTinNguoiDung(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Lỗi! Không thể tìm thấy người dùng.";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                DataModel db = new DataModel();
                ViewBag.listUser = db.get("exec TIMKIEMNguoiDungTHEOID " + id + ";");
            }
            catch (Exception)
            {
                RedirectToAction("Index", "Home");
            }

            return View();
        }
        public ActionResult DanhSachYeuThichNguoiDung(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Lỗi! Không thể tìm thấy người dùng.";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                DataModel db = new DataModel();
                ViewBag.listBDS = db.get("exec TIMYEUTHICHNGUOIDUNG "+ id + "");
            }
            catch (Exception)
            {
                return RedirectToAction("ThongTinNguoiDung", "Home", new { id });
            }
            return View();
        }

        [HttpPost]
        public ActionResult SUANGUOIDUNG(string tennguoidung,
                            HttpPostedFileBase hinh,
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
                    return RedirectToAction("ThongTinNguoiDung", "Home", new { id = idnguoidung });
                }

                // Kiểm tra độ dài và tính hợp lệ của các trường
                if (tennguoidung.Length < 5 || tennguoidung.Length > 30)
                {
                    TempData["ErrorMessage"] = "Tên người dùng phải từ 5 đến 30 ký tự!";
                    return RedirectToAction("ThongTinNguoiDung", "Home", new { id = idnguoidung });
                }

                if (sodienthoai.Length < 7 || sodienthoai.Length > 15 || !sodienthoai.All(char.IsDigit))
                {
                    TempData["ErrorMessage"] = "Số điện thoại phải từ 7 đến 15 ký tự và chỉ chứa số!";
                    return RedirectToAction("ThongTinNguoiDung", "Home", new { id = idnguoidung });
                }

                if (email.Length < 10 || email.Length > 30 || !email.Contains("@"))
                {
                    TempData["ErrorMessage"] = "Email phải có ít nhất 10 ký tự, tối đa 30 ký tự và chứa ký tự '@'!";
                    return RedirectToAction("ThongTinNguoiDung", "Home", new { id = idnguoidung });
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
                        return RedirectToAction("ThongTinNguoiDung", "Home", new { id = idnguoidung });
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
                db.get("exec SUANGUOIDUNG N'"+ tennguoidung + "', '"+ email + "', '"+ sodienthoai + "', N'"+ imageFileName + "', "+idnguoidung+"");
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("ThongTinNguoiDung", "Home", new { id = idnguoidung });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("ThongTinNguoiDung", "Home");
            }
        }
        [HttpPost]
        public ActionResult YeuTichTin(string id,string idbatdongsan, string idchubds)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || !int.TryParse(id, out _)|| string.IsNullOrEmpty(idbatdongsan) || !int.TryParse(idbatdongsan, out _) || string.IsNullOrEmpty(idchubds) || !int.TryParse(idchubds, out _))
                {
                    TempData["ErrorMessage"] = "Không thể yêu thích tin";
                    return RedirectToAction("Properties_Detail", "Home",new { id = idbatdongsan, idNguoiDung = idchubds });
                }
                DataModel db = new DataModel();
                db.get("exec YEUTHICHTINDANG "+id+", "+idbatdongsan+"");
                TempData["SuccessMessage"] = "Yêu thích tin thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra: " + ex.Message;
            }
            return RedirectToAction("Properties_Detail", "Home", new { id = idbatdongsan, idNguoiDung = idchubds });
        }
        [HttpPost]
        public ActionResult HuyYeuThichTin(string id, string idbatdongsan,string idchubds)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || !int.TryParse(id, out _) || string.IsNullOrEmpty(idbatdongsan) || !int.TryParse(idbatdongsan, out _))
                {
                    TempData["ErrorMessage"] = "Không thể hủy yêu thích tin";
                    return RedirectToAction("Properties_Detail", "Home", new { id = idbatdongsan, idNguoiDung = idchubds });
                }
                DataModel db = new DataModel();
                db.get("exec HUYYEUTICHTINDANG "+ id + "");
                TempData["SuccessMessage"] = "Hủy yêu thích tin thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra: " + ex.Message;
            }
            return RedirectToAction("Properties_Detail", "Home", new { id = idbatdongsan, idNguoiDung = idchubds });
        }

    }
}