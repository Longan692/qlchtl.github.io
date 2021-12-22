using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;
using System.Data.Entity;
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
        public ActionResult Category(string maLoai, string sortOrder, string searchString)
        {
            List<HANG> productname = db.HANGs.ToList();
            List<GIA> giabanmoi = db.GIAs.ToList();
            var item = from p in productname
                       join g in giabanmoi
                      on p.MaHang equals g.MaHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new SanPham { productdetails = p, giabandetails = g };
            var list = item.Where(_ => _.productdetails.MaLoaiHang.Trim().Equals(maLoai) && _.productdetails.TrangThai!=0);
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(s => s.productdetails.TenHang.ToUpper().Contains(searchString.ToUpper()));
                
            }
            switch (sortOrder)
            {
                case "name_desc":
                    list = list.OrderByDescending(s => s.productdetails.TenHang);
                    break;
                case "Gia":
                    list = list.OrderBy(s => s.giabandetails.DonGia);
                    break;
                case "Gia_desc":
                    list = list.OrderByDescending(s => s.giabandetails.DonGia);
                    break;
                default:
                    list = list.OrderBy(s => s.productdetails.TenHang);
                    break;
            }
            if (list == null)
            {
                ViewBag.Loi = "Không tìm thấy sản phẩm nào!";
                return View(list.ToList());
            }
            return View(list.ToList());
        }
       
        public ActionResult DanhMucPartial()
        {
            var list = db.LOAIHANGs.ToList();
            return View(list);
        }
    }
}