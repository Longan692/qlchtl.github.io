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

            COMBO cb = db.COMBOes.Find(id);

            var chitiet = db.CT_COMBO.Include(p => p.HANG).Where(p => p.MaCombo.Equals(id));
            if (chitiet == null)
            {
                return HttpNotFound();
            }
            return View(chitiet.ToList());
        }
    }
}