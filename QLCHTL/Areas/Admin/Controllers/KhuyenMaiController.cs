using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Constants;
using QLCHTL.Models;
namespace QLCHTL.Areas.Admin.Controllers
{
    public class KhuyenMaiController : Controller
    {
        private QLCHTLEntities db = new QLCHTLEntities();
        // GET: Admin/KhuyenMai
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DanhSachKhuyenMai()
        {
            var km = db.KHUYENMAIs.ToList();
            return View(km);
        }
        public ActionResult CreateKM()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateKM(KHUYENMAI km)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    km.MaDotKM = "";
                    db.KHUYENMAIs.Add(km);
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
        public ActionResult EditKM(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var nguoidung = db.KHUYENMAIs.FirstOrDefault(_ => _.MaDotKM.Trim().Equals(id));
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            return View(nguoidung);
        }
        [HttpPost]
        public ActionResult EditKM(KHUYENMAI kn)
        {
            try
            {
                var check = db.KHUYENMAIs.FirstOrDefault(_ => _.MaDotKM.Equals(kn.MaDotKM));
                check.TenDotKM = kn.TenDotKM;
                check.NgayBatDau = kn.NgayBatDau;
                check.NgayKetThuc = kn.NgayKetThuc;
                check.TrangThai = kn.TrangThai;
                db.Entry(check).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.ThanhCong = "Chỉnh sửa thông tin thành công";
                return View(check);
            }
            catch
            {
                return View(kn);
            }
        }
        public ActionResult DeleteKM(string id)
        {
            try
            {
                var kh = db.KHUYENMAIs.FirstOrDefault(_ => _.MaDotKM.Trim().Equals(id));
                db.KHUYENMAIs.Remove(kh);
                db.SaveChanges();
                return RedirectToAction("DanhSachKhuyenMai");
            }
            catch
            {
                return RedirectToAction("DanhSachKhuyenMai");
            }
        }

        public ActionResult DanhSachCombo(string MaDKM)
        {
            var km = db.COMBOes.Include(p => p.KHUYENMAI).Where(_=>_.MaDotKM.Equals(MaDKM)).ToList();
            return View(km);
        }
        public ActionResult CreateCB()
        {
            ViewBag.KhuyenMai = db.KHUYENMAIs.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCB(ComboKM km)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string filename = Path.GetFileNameWithoutExtension(km.ImageFile.FileName);
                    string extension = Path.GetExtension(km.ImageFile.FileName);
                    filename = filename + extension;
                    km.cbkm.AnhBia = "assets/img/anh_combo/" + filename;
                    filename = Path.Combine(Server.MapPath("~/assets/img/anh_combo/"), filename);
                    km.ImageFile.SaveAs(filename);
                    COMBO cb = new COMBO();
                    cb.MaCombo = "";
                    cb.MaDotKM = km.cbkm.MaDotKM;
                    cb.TyLeKM = km.cbkm.TyLeKM;
                    cb.TenCB = km.cbkm.TenCB;
                    cb.AnhBia = km.cbkm.AnhBia;
                    db.COMBOes.Add(cb);
                    db.SaveChanges();
                    ViewBag.Success = "Thêm thành công combo mới";
                    return RedirectToAction("DanhSachCombo");
                }
                catch
                {
                    ViewBag.Success = "Thêm không thành công combo mới";
                    return View();
                }
            }
            return View();
        }

        #region THÊM SẢN PHẨM VÀO COMBO
        public ActionResult ChiTietCB(string MaCB)
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
            ViewBag.Hang = item.OrderByDescending(p => p.productdetails.MaHang)
                .Where(p => p.productdetails.TrangThai == StatusProduct.ConHang).ToList();


            List<CartItem> lstNhapHang = LayNhapHang();
            if (lstNhapHang.Select(_ => _.MaCB).FirstOrDefault() != MaCB)
            {
                lstNhapHang.Clear();
                Session["NhapCB"] = null;
            }
            var combo = db.CT_COMBO.Where(_ => _.MaCombo.Equals(MaCB)).ToList();
            foreach (var icb in combo)
            {
                CartItem SanPham = lstNhapHang.Find(sp => sp.iMaHang.Equals(icb.MaHang));
                if (SanPham == null)
                {
                    SanPham = new CartItem(icb.MaHang);
                    SanPham.MaCB = MaCB;
                    lstNhapHang.Add(SanPham);
                }
            }
            return View(lstNhapHang);
        }
        public ActionResult TaoSP_InCB_New()
        {
            ViewBag.Combo = db.COMBOes.ToList();
            return View();
        }

        //Lay gio hang 
        public List<CartItem> LayNhapHang()
        {
            List<CartItem> lstNhapHang = Session["NhapCB"] as List<CartItem>;
            if (lstNhapHang == null)
            {
                //chua co list gio hang thi khoi tao
                lstNhapHang = new List<CartItem>();
                Session["NhapCB"] = lstNhapHang;
            }
            return lstNhapHang;
        }
        public ActionResult ThemSP_CB(string id, string strURL, string MaCB)
        {
            //lay gio hang
            List<CartItem> lstNhapHang = LayNhapHang();
            //kiem tra  ton tai 
            CartItem SanPham = lstNhapHang.Find(sp => sp.iMaHang.Equals(id));
            if (SanPham == null)
            {
                SanPham = new CartItem(id);
                SanPham.MaCB = MaCB;
                lstNhapHang.Add(SanPham);
                ViewBag.ThanhCong = "Thành công";
            }
            return Redirect(strURL);

        }
        #endregion THÊM SẢN PHẨM VÀO COMBO

        public ActionResult ThemCT_CB(string strURL)
        {
            List<CartItem> lstNhapHang = LayNhapHang();
            if (lstNhapHang.Count() == 0)
            {
                return Redirect(strURL);
            }
            else
            {
                foreach (var item in lstNhapHang)
                {

                    var ctcb = db.CT_COMBO.Where(_ => _.MaCombo.Equals(item.MaCB)).FirstOrDefault();
                    CT_COMBO cT_COMBO = new CT_COMBO();
                    if (ctcb == null)
                    {
                        cT_COMBO.MaCombo = item.MaCB;
                        cT_COMBO.MaHang = item.iMaHang;
                        cT_COMBO.IsKey = 1;
                        db.CT_COMBO.Add(cT_COMBO);
                    }
                    else
                    {
                        var mahang = db.CT_COMBO.Where(_ => _.MaHang.Equals(item.iMaHang)).FirstOrDefault();
                        if (mahang == null)
                        {
                            cT_COMBO.MaCombo = item.MaCB;
                            cT_COMBO.MaHang = item.iMaHang;
                            cT_COMBO.IsKey = 1;
                            db.CT_COMBO.Add(cT_COMBO);
                        }
                    }
                }
            }
            db.SaveChanges();
            Session["NhapCB"] = null;
            return Redirect(strURL);
        }

        #region Xóa item in ds nhap hang
        public ActionResult XoaHang(string iMaSP, string strURL)
        {

            //Lấy giỏ hàng ra từ session
            List<CartItem> lstNhapHang = LayNhapHang();
            CartItem sanpham = lstNhapHang.SingleOrDefault(n => n.iMaHang == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                lstNhapHang.RemoveAll(n => n.iMaHang == iMaSP);

            }
            if (lstNhapHang.Count == 0)
            {
                return Redirect(strURL);
            }
            return Redirect(strURL);
        }
        #endregion
    }
}