using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;

namespace QLCHTL.Areas.Admin.Controllers
{
    public class NhapHangController : Controller
    {
        private QLCHTLEntities db = new QLCHTLEntities();
        // GET: Admin/NhapHang
        public ActionResult Index()
        {
            return View();
        } 
        public ActionResult NhapHang()
        {
            return View();
        } 
        public ActionResult NhapHangMoi()
        {
            ViewBag.MaHang = db.HANGs.ToList();
            return View();
        }
        #region Tạo đơn đặt hàng
        //Lay gio hang 
        public List<NhapHang> LayNhapHang()
        {
            List<NhapHang> lstNhapHang = Session["NhapHang"] as List<NhapHang>;
            if (lstNhapHang == null)
            {
                //chua co list gio hang thi khoi tao
                lstNhapHang = new List<NhapHang>();
                Session["NhapHang"] = lstNhapHang;
            }
            return lstNhapHang;
        }
        #endregion
        #region Thêm sản phẩm mới vào giỏ hàng
        [HttpPost]
        public ActionResult ThemDatHang(string iMaHang, string strURL, int soluong, double DonGia)
        {
            //lay gio hang
            List<NhapHang> lstNhapHang = LayNhapHang();
            //kiem tra  ton tai 
            NhapHang SanPham = lstNhapHang.Find(sp => sp.iMaHang.Equals(iMaHang));
            if (SanPham == null)
            {
                SanPham = new NhapHang(iMaHang);
                SanPham.DonGia = DonGia;
                SanPham.soluong = soluong;
                lstNhapHang.Add(SanPham);
                return Redirect(strURL);
            }
            else
            {
                //Da co roi 
                //SanPham.soluong++;
                SanPham.soluong = SanPham.soluong + soluong;
                return Redirect(strURL);
            }
            //return RedirectToAction("GioHang", "Cart");
        }
    }
    #endregion
}