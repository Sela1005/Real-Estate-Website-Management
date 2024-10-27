using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BatDongSan.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ép kiểu `Session["taikhoan"]` sang `dynamic` để truy cập `VaiTro`
            var session = filterContext.HttpContext.Session["taikhoan"] as dynamic;

            // Kiểm tra nếu `session` là null hoặc không có `VaiTro` là Admin
            if (session == null || session.Count < 2 || session[5].ToString() != "Admin")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Home" },
                        { "action", "Index" }
                    });
            }

            base.OnActionExecuting(filterContext);
        }
    }
}