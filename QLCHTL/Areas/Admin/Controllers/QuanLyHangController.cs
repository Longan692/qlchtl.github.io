using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;
namespace QLCHTL.Areas.Admin.Controllers
{
    public class QuanLyHangController : Controller
    {
        // GET: Admin/QuanLyHang
        QLCHTLEntities db = new QLCHTLEntities();
        public ActionResult Index()
        {
            List<HANG> productname = db.HANGs.ToList();
            List<GIA> giabanmoi = db.GIAs.ToList();
            var item = from p in productname
                       join g in giabanmoi
                      on p.MaHang equals g.MaHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new SanPham { productdetails = p, giabandetails = g };
            return View(item);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(HANG model )
        {

            //ViewBag.LoadLoaiHang = db.LOAIHANGs.ToList();
            //ViewBag.LoadNCC = new SelectList(GetAllNCC(),"MaNCC","TenNCC");
            if (ModelState.IsValid)
            {
                var check = db.HANGs.FirstOrDefault(s => s.MaHang == model.MaHang);
                if (check == null)
                {
                    
                        db.HANGs.Add(model);
                        db.SaveChanges();
                   
                    string message = "Created the record successfully";
                    ViewBag.Message = message;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Mã hàng đã tồn tại";
                    return View();
                }


            }
            
            return View();
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var data = db.HANGs.Where(x => x.MaHang == id).FirstOrDefault();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(HANG Model)
        {
            var data = db.HANGs.Where(x => x.MaHang == Model.MaHang).FirstOrDefault();
            if (data != null)
            {
                data.MaHang = Model.MaHang;
                data.TenHang = Model.TenHang;
                data.MaLoaiHang = Model.MaLoaiHang;
                data.DVT = Model.DVT;
                data.MaNCC = Model.MaNCC;
                data.MoTa = Model.MoTa;
                data.HinhAnh = Model.HinhAnh;
                db.SaveChanges();
            }

            return RedirectToAction("index");
        }

        public ActionResult Delete(string id)
        {
          
                var data = db.HANGs.Where(x => x.MaHang == id).FirstOrDefault();
                db.HANGs.Remove(data);
                db.SaveChanges();
                ViewBag.Messsage = "Record Delete Successfully";
                return RedirectToAction("index");
            
            
        }
        public List<LOAIHANG> GetAllLoaiHang()
        {
            var categories = db.LOAIHANGs.ToList();
            return categories;
           
        }
        public List<NHACUNGCAP> GetAllNCC()
        {
            var categories = db.NHACUNGCAPs.ToList();
            return categories;

        }
    }
}