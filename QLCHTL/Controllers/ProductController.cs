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



        public ActionResult Detail(string ms)
        {
            List<HANG> productname = db.HANGs.Where(_ => _.MaHang.Equals(ms)).ToList();
            List<GIA> giabanmoi = db.GIAs.Where(_ => _.MaHang.Equals(ms)).ToList();
            var item = from p in productname
                       join g in giabanmoi
                      on p.MaHang equals g.MaHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new SanPham { productdetails = p, giabandetails = g };
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);

        }
        public ActionResult Category(string maLoai)
        {
            List<HANG> productname = db.HANGs.ToList();
            List<GIA> giabanmoi = db.GIAs.ToList();
            var item = from p in productname
                       join g in giabanmoi
                      on p.MaHang equals g.MaHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new SanPham { productdetails = p, giabandetails = g };
            var list = item.Where(_ => _.productdetails.MaLoaiHang.Equals(maLoai)).ToList();
            return View(list);
        }
        //public ActionResult ThucAnNhanh()
        //{
        //    List<HANG> productname = db.HANGs.ToList();
        //    List<GIA> giabanmoi = db.GIAs.ToList();
        //    var item = from p in productname
        //               join g in giabanmoi
        //              on p.MaHang equals g.MaHang into tb1
        //               from g in tb1.DefaultIfEmpty()
        //               select new SanPham { productdetails = p, giabandetails = g };
        //    var list = item.Where(_ => _.productdetails.MaLoaiHang.Trim().Equals("LH001"));
        //    return View(list);
        //}
        public ActionResult DanhMucPartial()
        {
            ViewBag.danhmuc = db.LOAIHANGs.ToList();
            return View();
        }
        
    }
}