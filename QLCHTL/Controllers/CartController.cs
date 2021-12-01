using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;
namespace QLCHTL.Controllers
{
    public class CartController : Controller
    {
        //private const string CartSession = "CartSession";
        QLCHTLEntities db = new QLCHTLEntities();

        public ActionResult Index()
        {
            //var cart = Session[CartSession];
            //var list = new List<CartItem>();
            //if(cart !=null)
            //{
            //    list = (List<CartItem>)cart;
            //}

            return View(/*list*/);
        }


        //Lay gio hang 
        public List<CartItem> LayGioHang()
        {
            List<CartItem> lstGioHang = Session["CartItem"] as List<CartItem>;
            if (lstGioHang == null)
            {
                //chua co list gio hang thi khoi tao
                lstGioHang = new List<CartItem>();
                Session["CartItem"] = lstGioHang;
            }
            return lstGioHang;
        }
        public ActionResult ThemGioHang(string ms,string strURL)
        {
            //lay gio hang
            List<CartItem> lstGioHang = LayGioHang();
            //kiem tra  ton tai 
            CartItem SanPham = lstGioHang.Find(sp => sp.iMaHang.Equals(ms));
            
            if(SanPham==null)
            {
                SanPham = new CartItem(ms);
                lstGioHang.Add(SanPham);
                return Redirect(strURL);
            }
            else
            {
                //Da co roi 
                SanPham.soluong++;
                return Redirect(strURL);
            }
            //return RedirectToAction("GioHang", "Cart");
        }
        private double TongThanhTien()
        {
            double ttt = 0;
            List<CartItem> lstGioHang = Session["CartItem"] as List<CartItem>;
            if (lstGioHang != null)
            {
                ttt += lstGioHang.Sum(p => p.ThanhTien);
            }
            return ttt;
        }
        //Tính tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<CartItem> lstGioHang = Session["CartItem"] as List<CartItem>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.soluong);
            }
            return iTongSoLuong;
        }
        public ActionResult GioHang()
        {
            if (Session["cartitem"] == null)
            {
                return RedirectToAction("Index", "Home");

            }
            List<CartItem> lstGioHang = LayGioHang();
            ViewBag.TongThanhTien = TongThanhTien();
            ViewBag.TongSoLuong = TongSoLuong();
            return View(lstGioHang);
        }
        //Partial View giỏ hàng:

        public ActionResult GioHangPartial()
        {
            if (TongSoLuong() == 0)
            {
                return PartialView();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongThanhTien();
            return PartialView();
        }
        [HttpPost]
        public ActionResult CapNhatGioHang(FormCollection f)
        {
            string[] quantities = f.GetValues("soluong");
          
            List<CartItem> lstCart = (List<CartItem>)Session["cartitem"];
            for (int i = 0; i < lstCart.Count(); i++)
            {
                lstCart[i].soluong = Convert.ToInt32(quantities[i]);
                

            }
            Session["cartitem"] = lstCart;
            return RedirectToAction("GioHang");

        }
        //Xóa giỏ hàng
        public ActionResult XoaGioHang(string iMaSP)
        {
            //Kiểm tra masp
            HANG sp = db.HANGs.SingleOrDefault(n => n.MaHang == iMaSP);
            //Nếu get sai masp thì sẽ trả về trang lỗi 404
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<CartItem> lstGioHang = LayGioHang();
            CartItem sanpham = lstGioHang.SingleOrDefault(n => n.iMaHang == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMaHang == iMaSP);

            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang");
        }
        [HttpPost]
        public ActionResult DatHang()
        {
            //Kiểm tra đăng đăng nhập
            if (Session["user"] == null || Session["user"].ToString() == "")
            {
                return RedirectToAction("Login", "Home");
            }
            //Kiểm tra giỏ hàng
            if (Session["cartitem"] == null)
            {
                RedirectToAction("Index", "Home");
            }
            //Thêm đơn hàng mới:
            DONDATHANG ddh = new DONDATHANG();
            Account Kh = (Account)Session["user"];
            List<CartItem> gh = LayGioHang();
            ddh.MaKH = Kh.Id;
            ddh.NgayDat = DateTime.Now;
            Console.WriteLine(ddh);
            db.DONDATHANGs.Add(ddh);
            db.SaveChanges();

            //TheM CHI TIET DON HANG
            foreach(var item in gh)
            {
                CT_DONDATHANG ctDDH = new CT_DONDATHANG();
                ctDDH.MaDonDatHang = ddh.MaDonDatHang;
                ctDDH.MaHang = item.iMaHang;
                ctDDH.TenHang = item.iTenHang;
                ctDDH.SoLuong = item.soluong;
                ctDDH.TinhTrang = "Đang chờ duyệt";
                //tinh trang =0 la chưa thanh toan
                ctDDH.TinhTrangThanhToan = 0;
                db.CT_DONDATHANG.Add(ctDDH);
            }
            db.SaveChanges();
            Session["cartitem"] = null;
            return RedirectToAction("Index","DonHang");
        }    

    }

}