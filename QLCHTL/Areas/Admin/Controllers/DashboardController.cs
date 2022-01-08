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
            ViewBag.TongSanPham = db.HANGs.Count();
            ViewBag.TongUser = db.Accounts.Where(_=>_.Roles==Role.KhachHang).Count();

            //ViewBag.TongUserNew = db.Accounts.Where(_=>_.Roles==Role.KhachHang && _.NgayTao==DateTime.Today).Count();
            ViewBag.TongDonHang = db.DONDATHANGs.Where(_=>_.TinhTrang== "Đang chờ duyệt").Count();
            ViewBag.TongDoanhThu = db.CT_DONDATHANG.Where(_ => _.NgayGiao.Value.Month.Equals(DateTime.Now.Month)).Count();
            ViewBag.TongKhuyenMai = db.KHUYENMAIs.Where(_ => _.TrangThai==StatusProduct.ConHang).Count();

           //Hang mới
            List<HANG> productname = db.HANGs.ToList();
            List<GIA> giabanmoi = db.GIAs.ToList();
            var item = from p in productname
                       join g in giabanmoi
                      on p.MaHang equals g.MaHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new SanPham { productdetails = p, giabandetails = g };
            ViewBag.TongSanPhamMoi = item.OrderByDescending(p => p.productdetails.MaHang)
                .Where(p => p.productdetails.TenHang != null).Take(10).ToList();
            //Người dùng mới đăng kí
            ViewBag.TongUserNew = db.Accounts.OrderByDescending(p => p.Id).Where(_ => _.Roles == Role.KhachHang).Take(5).ToList() ;

            List<DONDATHANG> ddh = db.DONDATHANGs.ToList();
            List<CT_DONDATHANG> ct_ddh = db.CT_DONDATHANG.ToList();
            var temp = from p in ddh
                       join g in ct_ddh
                      on p.MaDonDatHang equals g.MaDonDatHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new ThongKe { dh = p, ct_ddh = g };
            //ViewBag.TongDoanhThuThang = item.Where(_ => _.dh.TinhTrang == "Đã giao" && _.ct_ddh.NgayGiao.Value.ToString("MM").Equals(DateTime.Now.Month)).Count()/*.Sum(_ => _.dh.TongTien)*/;
            return View();
        }
    }
}