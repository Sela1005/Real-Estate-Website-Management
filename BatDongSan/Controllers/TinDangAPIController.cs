using BatDongSan.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace BatDongSan.Controllers
{
    public class TinDangAPIController : Controller
    {
        public JsonResult Index()
        {
            DataModel db = new DataModel();

            // Thực hiện truy vấn
            ArrayList rawData = db.getApi("exec XEMDANHSACHBATDONGSAN");

            // Danh sách key cho dữ liệu
            var keys = new string[]
            {
                "BatDongSanId","TieuDe", "MoTa", "Gia", "DienTich", "ViTri",
                "LoaiBDS", "TrangThai", "LoaiHinh", "TinhTrangPhapLy", "HinhAnh"
            };

            var formattedData = rawData.Cast<ArrayList>().Select(item =>
            {
                var obj = new Dictionary<string, object>();
                for (int i = 0; i < keys.Length; i++)
                {
                    obj[keys[i]] = item[i];
                }
                return obj;
            }).ToList();

            // Trả về JSON kết quả
            return Json(formattedData,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult XuLyDangNhap(string username, string password)
        {
            DataModel db = new DataModel();

            // Thực hiện truy vấn
            ArrayList rawData = db.getApi("exec KIEMTRADANGNHAP '" + username + "' ,'" + password + "'");

            // Nếu có dữ liệu (tức là đăng nhập thành công)
                var user = (ArrayList)rawData[0];

                var userData = new
                {
                    NguoiDungID = user[0],
                    TenNguoiDung = user[1],
                    Email = user[3],
                    SoDienThoai = user[4],
                    VaiTro = user[5],
                    Avatar = user[6],
                    NgayTao = user[7],
                };

                // Trả về dữ liệu người dùng dưới dạng JSON
                return Json(userData, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult XuLyDangKy(string username, string password, string Email, string confrimpassword)
        {
            DataModel db = new DataModel();

            // Kiểm tra độ dài và tính hợp lệ của username, password và email
            if (string.IsNullOrEmpty(username) || username.Length < 5 || username.Length > 30)
            {
                return Json(new { Status = "ERR", Message = "Tên người dùng phải từ 5 đến 30 ký tự và không được để trống!" });
            }

            if (string.IsNullOrEmpty(password) || password.Length < 6 || password.Length > 30)
            {
                return Json(new { Status = "ERR", Message = "Mật khẩu phải từ 6 đến 30 ký tự và không được để trống!" });
            }

            if (confrimpassword != password)
            {
                return Json(new { Status = "ERR", Message = "Mật khẩu và Nhập lại mật khẩu không khớp!" });
            }

            if (string.IsNullOrEmpty(Email) || Email.Length < 6 || Email.Length > 30 || !Email.Contains("@"))
            {
                return Json(new { Status = "ERR", Message = "Email phải chứa '@' và không được để trống!" });
            }

            // Thực hiện thủ tục lưu trữ
            ArrayList rawData = db.getApi($"EXEC SP_DangKy N'{username}', N'{password}', N'{Email}'");

            // Kiểm tra kết quả trả về
            if (rawData.Count > 0)
            {
                int result = Convert.ToInt32(((ArrayList)rawData[0])[0]);

                if (result == 1)
                {
                    // Đăng ký thành công
                    return Json(new { Status = "OK", Message = "Đăng ký tài khoản thành công!" });
                }
                else if (result == 0)
                {
                    // Email đã tồn tại
                    return Json(new { Status = "ERR", Message = "Email này đã tồn tại. Vui lòng thử lại với email khác." });
                }
            }

            // Lỗi không xác định
            return Json(new { Status = "ERR", Message = "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại." });
        }
        [HttpGet]
        public JsonResult Properties_Detail(int id)
        {
            // Kiểm tra dữ liệu đầu vào
            if (id <= 0)
            {
                return Json(new { Status = "ERR", Message = "Dữ liệu không hợp lệ!" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                DataModel db = new DataModel();

                // Thực hiện truy vấn để lấy chi tiết bất động sản
                ArrayList rawData = db.getApi($"exec XEMCHITIETTINDANG {id}");

                // Kiểm tra nếu không có dữ liệu
                if (rawData == null || rawData.Count == 0)
                {
                    return Json(new { Status = "ERR", Message = "Không tìm thấy thông tin." }, JsonRequestBehavior.AllowGet);
                }

                // Danh sách key cho dữ liệu trả về
                var keys = new string[]
                {
            "BatDongSanID", "TieuDe", "MoTa", "Gia", "DienTich",
            "ViTri", "LoaiBDS", "LoaiHinh", "TinhTrangPhapLy", "HinhAnh",
            "NguoiDungID", "TenNguoiDung", "Email", "SoDienThoai","Avatar"
                };

                // Lấy dữ liệu đầu tiên trong danh sách (vì chỉ có một bất động sản)
                var item = rawData.Cast<ArrayList>().FirstOrDefault();

                // Kiểm tra nếu item không có dữ liệu
                if (item == null)
                {
                    return Json(new { Status = "ERR", Message = "Không tìm thấy thông tin." }, JsonRequestBehavior.AllowGet);
                }

                // Định dạng lại dữ liệu theo key-value
                var formattedData = new Dictionary<string, object>();
                for (int i = 0; i < keys.Length; i++)
                {
                    formattedData[keys[i]] = item[i];
                }

                // Trả về dữ liệu theo cấu trúc yêu cầu
                return Json(new
                {
                    Status = "OK",
                    Message = "Lấy thông tin thành công",
                    BatDongSanID = formattedData["BatDongSanID"],
                    TieuDe = formattedData["TieuDe"],
                    MoTa = formattedData["MoTa"],
                    Gia = formattedData["Gia"],
                    DienTich = formattedData["DienTich"],
                    ViTri = formattedData["ViTri"],
                    LoaiBDS = formattedData["LoaiBDS"],
                    LoaiHinh = formattedData["LoaiHinh"],
                    TinhTrangPhapLy = formattedData["TinhTrangPhapLy"],
                    HinhAnh = formattedData["HinhAnh"],
                    NguoiDungID = formattedData["NguoiDungID"],
                    TenNguoiDung = formattedData["TenNguoiDung"],
                    Email = formattedData["Email"],
                    SoDienThoai = formattedData["SoDienThoai"],
                    Avatar = formattedData["Avatar"]
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có exception
                return Json(new { Status = "ERR", Message = "Đã xảy ra lỗi khi xử lý dữ liệu.", Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DangTinApi(string tieude,
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
                // Kiểm tra TIEUDE
                if (string.IsNullOrEmpty(tieude) || tieude.Length < 25 || tieude.Length > 100)
                {
                    return Json(new { Status = "ERR", message = "Tiêu đề phải từ 25 đến 100 ký tự và không được bỏ trống." });
                }

                // Kiểm tra các tham số khác (Giá, Diện tích, Mô tả, Vị trí...)
                if (string.IsNullOrEmpty(mota) || mota.Length > 1000) { return Json(new { Status = "ERR", message = "Mô tả không được bỏ trống và tối đa 1000 ký tự." }); }
                if (string.IsNullOrEmpty(gia) || !decimal.TryParse(gia, out decimal giaDecimal) || giaDecimal <= 0 || gia.Length > 30) { return Json(new { Status = "ERR", message = "Giá phải là số thập phân, lớn hơn 0 và tối đa 30 ký tự." }); }
                if (string.IsNullOrEmpty(dientich) || !decimal.TryParse(dientich, out decimal dientichDecimal) || dientichDecimal <= 0 || dientich.Length > 30) { return Json(new { Status = "ERR", message = "Diện tích phải là số thập phân, lớn hơn 0 và tối đa 30 ký tự." }); }
                if (string.IsNullOrEmpty(vitri) || vitri.Length > 255) { return Json(new { Status = "ERR", message = "Vị trí không được bỏ trống và tối đa 255 ký tự." }); }
                if (string.IsNullOrEmpty(idnguoidung) || !int.TryParse(idnguoidung, out int nguoidungId) || nguoidungId <= 0) { return Json(new { Status = "ERR", message = "ID người dùng phải là số nguyên dương hợp lệ." }); }
                if (string.IsNullOrEmpty(loaibds) || loaibds.Length > 50) { return Json(new { Status = "ERR", message = "Loại bất động sản không được bỏ trống và tối đa 50 ký tự." }); }
                if (string.IsNullOrEmpty(loaihinh) || loaihinh.Length > 50) { return Json(new { Status = "ERR", message = "Loại hình không được bỏ trống và tối đa 50 ký tự." }); }
                if (string.IsNullOrEmpty(tinhtrangphaply) || tinhtrangphaply.Length > 50) { return Json(new { Status = "ERR", message = "Tình trạng pháp lý không được bỏ trống và tối đa 50 ký tự." }); }

                // Kiểm tra HINHANH
                if (hinh == null || hinh.ContentLength == 0)
                {
                    return Json(new { Status = "ERR", message = "Ảnh không được bỏ trống." });
                }

                string fileExtension = Path.GetExtension(hinh.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { Status = "ERR", message = "Vui lòng chỉ chọn ảnh định dạng: \".jpg\", \".jpeg\", \".png\", \".gif\", \".bmp\"." });
                }

                // Lưu ảnh vào thư mục
                string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                string path = Path.Combine(Server.MapPath("~/images"), uniqueFileName);
                hinh.SaveAs(path);

                // Gọi stored procedure và kiểm tra kết quả trả về
                DataModel db = new DataModel();
                var result = db.get($"exec ThemBDS N'{tieude}', N'{mota}', {gia}, {dientich}, N'{vitri}', {nguoidungId}, N'{loaibds}', N'{loaihinh}', N'{tinhtrangphaply}', '{uniqueFileName}'");

                if (result != null && result.Count > 0)
                {
                    return Json(new { Status = "OK", message = "Đăng tin thành công!" });
                }
                else
                {
                    return Json(new { Status = "ERR", message = "Đã có lỗi xảy ra khi đăng tin." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = "ERR", message = "Đã có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}