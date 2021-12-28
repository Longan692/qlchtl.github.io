using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Constants;
using QLCHTL.Models;
namespace QLCHTL.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private QLCHTLEntities db = new QLCHTLEntities();
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            //Kiểm tra đăng đăng nhập
            if (Session["user"] == null || Session["user"].ToString() == "")
            {
                return RedirectToAction("Login", "Home",new {area="" });
            }
            ViewBag.TongSanPham = db.HANGs.Where(_=>_.TrangThai==StatusProduct.ConHang).Count();
            ViewBag.TongUser = db.Accounts.Where(_=>_.Roles==Role.KhachHang).Count();
            ViewBag.TongUserNew = db.Accounts.Where(_=>_.Roles==Role.KhachHang && _.NgayTao==DateTime.Today).Count();
            ViewBag.TongDonHang = db.DONDATHANGs.Where(_=>_.TinhTrang== "Đang chờ duyệt").Count();
            return View();
        }
    }
}