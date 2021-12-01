using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;
namespace QLCHTL.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        QLCHTLEntities db = new QLCHTLEntities();
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
        public ActionResult AddProductDetail(string ms)
        {
            //lay gio hang
            List<CartItem> lstGioHang = LayGioHang();
            //kiem tra  ton tai 
            CartItem SanPham = lstGioHang.Find(sp => sp.iMaHang.Equals(ms));

            if (SanPham == null)
            {
                SanPham = new CartItem(ms);
                lstGioHang.Add(SanPham);
               
            }
            else
            {
                //Da co roi 
                SanPham.soluong++;
            }
            return RedirectToAction("Detail", "Product");
        }
        public ActionResult Detail()
        {
            List<CartItem> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }
    }
}