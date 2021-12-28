using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;
namespace QLCHTL.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {
        private QLCHTLEntities db = new QLCHTLEntities();
        // GET: Admin/DonHang
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DonHang()
        {
            if (Session["user"] == null || Session["user"].ToString() == "")
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
            var donhang = db.DONDATHANGs.Where(_ => _.TinhTrang == "Đang chờ duyệt").ToList();
            return View(donhang);
        }
        public ActionResult DonHangDaDuyet()
        {
            var donhang = db.DONDATHANGs.Where(_ => _.TinhTrang == "Đang giao").ToList();
            return View(donhang);
        }
        public ActionResult DonHangDaGiao()
        {
            var donhang = db.DONDATHANGs.Where(_ => _.TinhTrang == "Đã giao").ToList();
            return View(donhang);
        }
        public ActionResult DonHangDaHuy()
        {
            var donhang = db.DONDATHANGs.Where(_ => _.TinhTrang == "Hủy đơn").ToList();
            return View(donhang);
        }
        public ActionResult EditDonHang(int? id)
        {
            var donhang = db.DONDATHANGs.Find(id);
            return View(donhang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDonHang(DONDATHANG donhang, int tinhTrangTT)
        {
            //try
            //{
            var dh = db.DONDATHANGs.Find(donhang.MaDonDatHang);
            dh.HoTenNguoiNhan = donhang.HoTenNguoiNhan;
            dh.SDT = donhang.SDT;
            dh.DiaChi = donhang.DiaChi;
            if (donhang.GhiChu != null)
            {
                dh.GhiChu = donhang.GhiChu;
            }
            dh.TinhTrang = donhang.TinhTrang;
            db.Entry(dh).State = EntityState.Modified;
            db.SaveChanges();
            Account Kh = (Account)Session["user"];
            //Cập nhật chi tiết đơn hàng
            var ctdh = db.CT_DONDATHANG.Where(_ => _.MaDonDatHang == dh.MaDonDatHang).ToList();
            foreach (var item in ctdh)
            {
                //var ct = db.CT_DONDATHANG.FirstOrDefault(_ => _.MaHang.Equals(item.MaHang));
                if (item.DateDuyet == null && item.MaNVDuyet == null)
                {
                    item.DateDuyet = DateTime.Now;
                    item.MaNVDuyet = Kh.Id;
                    item.NgayGiao = DateTime.Now;

                }
                else
                {
                    item.TinhTrangThanhToan = tinhTrangTT;

                }
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            ViewBag.Success = "Cập nhật thông tin đơn hàng thành công";
            return View(dh);
            //}
            //catch
            //{
            //    var dh = db.DONDATHANGs.Find(donhang.MaDonDatHang);
            //    ViewBag.Success = "Cập nhật thông tin đơn hàng không thành công";
            //    return View(dh);
            //}
        }
        public ActionResult ChiTietDonHang(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONDATHANG donhang = db.DONDATHANGs.Find(id);
            var chitiet = db.CT_DONDATHANG.Include(d => d.HANG).Where(d => d.MaDonDatHang == id).ToList();
            if (donhang == null)
            {
                return HttpNotFound();
            }
            return View(chitiet);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var donhang = db.DONDATHANGs.Find(id);
            return View(donhang);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                //Xóa sản phẩm trong bảng CT_DDH
                List<CT_DONDATHANG> ctdh = db.CT_DONDATHANG.Where(_ => _.MaDonDatHang == id).ToList();
                foreach (var item in ctdh)
                {
                    db.CT_DONDATHANG.Remove(item);
                    db.SaveChanges();
                }
                //Xóa sản phẩm trong bảng DONĐATHANG
                DONDATHANG ddhang = db.DONDATHANGs.Find(id);
                db.DONDATHANGs.Remove(ddhang);
                db.SaveChanges();
                ViewBag.Success = "Xóa thành công";
                return RedirectToAction("DonHang");
            }
            catch
            {
                ViewBag.Success = "Xóa không thành công";
                var donhang = db.DONDATHANGs.Find(id);
                return View(donhang);
            }
        }

    }
}