using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;

namespace QLCHTL.Controllers
{
    public class ComboSaleController : Controller
    {
        // GET: ComboSale
        QLCHTLEntities db = new QLCHTLEntities();
        public ActionResult Index()
        {
            var combo = db.KHUYENMAIs.ToList();
            return View(combo);
        }

        public ActionResult DanhsachCombo(string id)
        {

            KHUYENMAI km = db.KHUYENMAIs.Find(id);

            var chitiet = db.COMBOes.Where(d => d.MaDotKM.Equals(id));
            if (km == null)
            {
                return HttpNotFound();
            }
            return View(chitiet.ToList());
        }

        public ActionResult ChitietCombo(string id)
        {
            //==========================================
            List<COMBO> combo = db.COMBOes.ToList();
            List<CT_COMBO> ct_combo = db.CT_COMBO.ToList();
            var item = from p in combo
                       join g in ct_combo
                      on p.MaCombo equals g.MaCombo into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new AddCombo { combodetail = p, ct_combodetail = g };
            //List<HANG> productname = db.HANGs.ToList();
            //List<GIA> giabanmoi = db.GIAs.ToList();
            //var gia = from a in productname
            //          join b in giabanmoi
            //         on a.MaHang equals b.MaHang into tb1
            //          from b in tb1.DefaultIfEmpty()
            //          select new SanPham { productdetails = a, giabandetails = b };
            //CT_COMBO cb = db.CT_COMBO.Find(id);
            
            List<AddCombo> chitiet = item.Where(p => p.ct_combodetail.MaCombo.Trim().Equals(id)).ToList();
            List<GIA> giabanmoi = db.GIAs.ToList();
            var gia = from a in chitiet
                      join b in giabanmoi
                     on a.ct_combodetail.MaHang equals b.MaHang into tb1
                      from b in tb1.DefaultIfEmpty()
                      select new AddCombo { addgia = a, Giadetail = b };

            //var tam = item.Where(p => p.ct_combodetail.MaCombo.Trim().Equals(id));
            //foreach (var p in chitiet)
            //{
            //    //var temp = db.CT_COMBO.Where(_ => _.MaCombo.Trim().Equals(id));

            //    //var tam = gia.Where(a => a.giabandetails.MaHang.Trim().Equals(chitiet.Select(_=>_.ct_combodetail.MaHang.Trim())));
            //    var tem= gia.Where(a => a.giabandetails.MaHang.Trim().Equals(chitiet.Select(_ => _.ct_combodetail.MaHang.Trim())));
            //    p.Gia = tem.Select(p=>new {p.giabandetails.DonGia }).FirstOrDefault();

            //}
            if (chitiet == null)
            {
                return HttpNotFound();
            }
            return View(gia.ToList());
        }
    }
}