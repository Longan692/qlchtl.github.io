using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;

namespace QLCHTL.Areas.Admin.Controllers
{
    public class QuanLyHangHoaController : Controller
    {
        private QLCHTLEntities db = new QLCHTLEntities();
        // GET: Admin/QuanLyHangHoa
        public ActionResult Index()
        {
            //Kiểm tra đăng đăng nhập
            if (Session["user"] == null || Session["user"].ToString() == "")
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
            List<HANG> productname = db.HANGs.ToList();
            List<GIA> giabanmoi = db.GIAs.ToList();
            var item = from p in productname
                       join g in giabanmoi
                      on p.MaHang equals g.MaHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new SanPham { productdetails = p, giabandetails = g };
            item = item.OrderBy(s => s.productdetails.LOAIHANG.MaLoaiHang);
            return View(item.ToList());
        }
        public ActionResult Create()
        {
            ViewBag.Maloaihang = db.LOAIHANGs.ToList();
            ViewBag.MaNCC = db.NHACUNGCAPs.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SanPham sp)
        {
            ViewBag.Maloaihang = db.LOAIHANGs.ToList();
            ViewBag.MaNCC = db.NHACUNGCAPs.ToList();
            if (sp.productdetails.TenHang == null)
            {
                ViewBag.ERTen = "Vui lòng nhập tên sản phẩm";
                return View();
            }
            if (sp.productdetails.DVT == null)
            {
                ViewBag.ERDVT = "Vui lòng nhập đơn vị tính";
                return View();
            }
            string filename = Path.GetFileNameWithoutExtension(sp.ImageFile.FileName);
            string extension = Path.GetExtension(sp.ImageFile.FileName);
            filename = filename + extension;
            sp.productdetails.HinhAnh = "assets/img/product/" + filename;
            filename = Path.Combine(Server.MapPath("~/assets/img/product/"), filename);
            sp.ImageFile.SaveAs(filename);

            if (ModelState.IsValid)
            {
                try
                {
                    HANG item = new HANG();
                    item.MaHang = "";
                    item.TenHang = sp.productdetails.TenHang;
                    item.MaLoaiHang = sp.productdetails.MaLoaiHang;
                    item.DVT = sp.productdetails.DVT;
                    item.MaNCC = sp.productdetails.MaNCC;
                    item.MoTa = sp.productdetails.MoTa;
                    item.HinhAnh = sp.productdetails.HinhAnh;
                    item.Soluong = sp.productdetails.Soluong;
                    item.TrangThai = sp.productdetails.TrangThai;
                    db.HANGs.Add(item);
                    db.SaveChanges();
                    var temp = db.HANGs.FirstOrDefault(_ => _.TenHang == item.TenHang && _.MaLoaiHang == item.MaLoaiHang
                    && _.DVT == item.DVT && _.MoTa == item.MoTa && _.HinhAnh == item.HinhAnh && _.Soluong == item.Soluong && _.TrangThai == item.TrangThai);
                    //Thêm vào giá
                    GIA gia = new GIA();
                    gia.MaHang = temp.MaHang;
                    gia.Ngay = DateTime.Now;
                    gia.DonGia = 0;
                    db.GIAs.Add(gia);
                    db.SaveChanges();
                    ViewBag.Success = "Thêm thành công sản phẩm mới";
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.Success = "Thêm không thành công sản phẩm mới";
                    return View();
                }
            }
            return View();
        }
        public ActionResult Edit(string id)
        {
            List<HANG> productname = db.HANGs.ToList();
            List<GIA> giabanmoi = db.GIAs.ToList();
            var item = from p in productname
                       join g in giabanmoi
                      on p.MaHang equals g.MaHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new SanPham { productdetails = p, giabandetails = g };
            // Hiển thị dropdownlist
            var dt = item.Where(_ => _.productdetails.MaHang.Trim().Equals(id.Trim())).FirstOrDefault();
            ViewBag.Maloaihang = db.LOAIHANGs.ToList();
            ViewBag.MaloaihangChon = db.LOAIHANGs.Where(_ => _.MaLoaiHang.Trim().Equals(dt.productdetails.MaLoaiHang)).Select(_ => _.TenLoaiHang);
            ViewBag.MaNCC = db.NHACUNGCAPs.ToList();
            ViewBag.MaNCCChon = db.NHACUNGCAPs.Where(_ => _.MaNCC.Trim().Equals(dt.productdetails.MaNCC)).Select(_ => _.TenNCC);
            return View(dt);

        }
        [HttpPost]
        public ActionResult Edit(SanPham sp)
        {
            ViewBag.Maloaihang = db.LOAIHANGs.ToList();
            ViewBag.MaloaihangChon = db.LOAIHANGs.Where(_ => _.MaLoaiHang.Trim().Equals(sp.productdetails.MaLoaiHang)).Select(_ => _.TenLoaiHang);
            ViewBag.MaNCC = db.NHACUNGCAPs.ToList();
            ViewBag.MaNCCChon = db.NHACUNGCAPs.Where(_ => _.MaNCC.Trim().Equals(sp.productdetails.MaNCC)).Select(_ => _.TenNCC);
            try
            {
                var item = db.HANGs.Find(sp.productdetails.MaHang);
                if (sp.ImageFile != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(sp.ImageFile.FileName);
                    string extension = Path.GetExtension(sp.ImageFile.FileName);
                    filename = filename + extension;
                    sp.productdetails.HinhAnh = "assets/img/product/" + filename;
                    filename = Path.Combine(Server.MapPath("~/assets/img/product/"), filename);
                    sp.ImageFile.SaveAs(filename);
                    item.HinhAnh = sp.productdetails.HinhAnh;
                }
                // Sửa sản phẩm theo mã sản phẩm
                item.TenHang = sp.productdetails.TenHang;
                item.MaLoaiHang = sp.productdetails.MaLoaiHang;
                item.DVT = sp.productdetails.DVT;
                item.MaNCC = sp.productdetails.MaNCC;
                item.MoTa = sp.productdetails.MoTa;
                item.Soluong = sp.productdetails.Soluong;
                item.TrangThai = sp.productdetails.TrangThai;
                // lưu lại

                var gia = db.GIAs.FirstOrDefault(_ => _.MaHang.Trim().Equals(sp.productdetails.MaHang.Trim()));
                if (sp.giabandetails.DonGia != gia.DonGia)
                {
                    //gia.Ngay = DateTime.Now;
                    gia.DonGia = sp.giabandetails.DonGia;
                }
                db.SaveChanges();
                // xong chuyển qua index
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }
        public ActionResult Details(string id)
        {
            List<HANG> productname = db.HANGs.ToList();
            List<GIA> giabanmoi = db.GIAs.ToList();
            var item = from p in productname
                       join g in giabanmoi
                      on p.MaHang equals g.MaHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new SanPham { productdetails = p, giabandetails = g };
            var dt = item.SingleOrDefault(_ => _.productdetails.MaHang.Trim().Equals(id.Trim()));
            return View(dt);
        }
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<HANG> productname = db.HANGs.ToList();
            List<GIA> giabanmoi = db.GIAs.ToList();
            var item = from p in productname
                       join g in giabanmoi
                      on p.MaHang equals g.MaHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new SanPham { productdetails = p, giabandetails = g };
            var dt = item.SingleOrDefault(_ => _.productdetails.MaHang.Trim().Equals(id.Trim()));
            return View(dt);
           
        }
        // POST: Admin/Hedieuhanhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                //Xóa sản phẩm trong bảng giá
                GIA gia = db.GIAs.SingleOrDefault(_ => _.MaHang.Trim().Equals(id));
                db.GIAs.Remove(gia);
                db.SaveChanges();
                //Xóa sản phẩm trong bảng hàng
                HANG hang = db.HANGs.SingleOrDefault(_ => _.MaHang.Trim().Equals(id));
                db.HANGs.Remove(hang);
                db.SaveChanges();
                ViewBag.Success = "Xóa thành công";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Success = "Xóa không thành công";
                return RedirectToAction("Index");
            }
        }

    }
}