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
            CartItem SanPham = lstGioHang.Find(sp => sp.iMaHang == ms.Trim());
            if(SanPham==null)
            {
                SanPham = new CartItem(ms);
                lstGioHang.Add(SanPham);
                //return Redirect(strURL);
            }
            else
            {
                //Da co roi 
                SanPham.soluong++;
                //return Redirect(strURL);
            }
            return RedirectToAction("GioHang", "Cart");
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

        public ActionResult GioHang()
        {

            //if (Session["CartItem"] == null)
            //{
            //    return RedirectToAction("Index", "Home");

            //}
            List<CartItem> lstGioHang = LayGioHang();
            ViewBag.TongThanhTien = TongThanhTien();
            return View(lstGioHang);
        }
        //public ActionResult AddItem(string productId, int quantity)
        //{
        //    var cart = Session[CartSession];
        //    if(cart!=null)
        //    {
        //        var list = (List<CartItem>)cart;
        //        if(list.Exists(x=>x.Product.MaHang==productId))
        //        {
        //            foreach(var item in list)
        //            {
        //                if(item.Product.MaHang==productId)
        //                {
        //                    item.Quantity += quantity;
        //                }

        //            }

        //        }
        //        else
        //        {
        //            //tao moi
        //            var item = new CartItem();
        //            item.Product.MaHang = productId;
        //            item.Quantity = quantity;
        //            list.Add(item);
        //        }
        //        //Gan Session
        //        Session[CartSession] = list;

        //    }
        //    else
        //    {
        //        //Tao moi
        //        var item = new CartItem();
        //        item.Product.MaHang = productId;
        //        item.Quantity = quantity;
        //        var list = new List<CartItem>();

        //        Session[CartSession] = list;
        //    }
        //    return RedirectToAction("Index");
        //}

        public ActionResult Test()
        {
            //var cart = Session[CartSession];
            //var list = new List<CartItem>();
            //if(cart !=null)
            //{
            //    list = (List<CartItem>)cart;
            //}

            return View();
        }

    }

}