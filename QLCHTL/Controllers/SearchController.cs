using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;
namespace QLCHTL.Controllers
{
    public class SearchController : Controller
    {
        private QLCHTLEntities db = new QLCHTLEntities();
        //// GET: Search
        //[HttpPost]
        //public ActionResult Index(string searchString)
        //{
        //    List<HANG> productname = db.HANGs.ToList();
        //    List<GIA> giabanmoi = db.GIAs.ToList();
        //    var item = from p in productname
        //               join g in giabanmoi
        //              on p.MaHang equals g.MaHang into tb1
        //               from g in tb1.DefaultIfEmpty()
        //               select new SanPham { productdetails = p, giabandetails = g };

        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        searchString = searchString.ToLower();
        //        item = item.Where(b => b.productdetails.TenHang.ToLower().Contains(searchString)
        //        || b.giabandetails.DonGia.ToString().ToLower().Contains(searchString)
        //        || b.productdetails.LOAIHANG.TenLoaiHang.ToLower().Contains(searchString));
        //    }

        //    return View(item.ToList());
        //} 
        public ActionResult SearchItem()
        {
            List<HANG> productname = db.HANGs.ToList();
            List<GIA> giabanmoi = db.GIAs.ToList();
            var item = from p in productname
                       join g in giabanmoi
                      on p.MaHang equals g.MaHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new SanPham { productdetails = p, giabandetails = g };
            return View(item.ToList());
        }

        [HttpPost]
        public ActionResult SearchItem(string searchString)
        {
            try
            {
                List<HANG> productname = db.HANGs.ToList();
                List<GIA> giabanmoi = db.GIAs.ToList();
                var item = from p in productname
                           join g in giabanmoi
                          on p.MaHang equals g.MaHang into tb1
                           from g in tb1.DefaultIfEmpty()
                           select new SanPham { productdetails = p, giabandetails = g };
                var list = item.Where(_ => _.productdetails.TrangThai != 0);
                if (!String.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToUpper();
                    //list = list.Where(s => s.productdetails.TenHang.ToUpper().Contains(searchString.ToUpper()));

                    list = list.Where(p => p.productdetails.TenHang.ToUpper().Contains(searchString)
                    || p.giabandetails.DonGia.ToString().ToUpper().Contains(searchString)
                    || p.productdetails.LOAIHANG.TenLoaiHang.ToUpper().Contains(searchString));
                }
                else
                {
                    return RedirectToAction("KhongTimThay");
                }

                return View(list.ToList());
            }
            catch
            {
                return RedirectToAction("KhongTimThay");
            }
        }
        public ActionResult KhongTimThay()
        {
            return View();
        }
    }
}