using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;

namespace QLCHTL.Controllers
{
    public class DonHangController : Controller
    {
        // GET: DonHang
        private QLCHTLEntities db = new QLCHTLEntities();
        public ActionResult Index()
        {
            //Kiểm tra đang đăng nhập
            if (Session["user"] == null || Session["user"].ToString() == "")
            {
                return RedirectToAction("Login", "Home");
            }
            Account kh = (Account)Session["user"];
            int maND = kh.Id;
            var donhangs = db.DONDATHANGs.Include(d => d.Account).Where(d => d.MaKH == maND);
            return View(donhangs.ToList());
        }

        //Hiển thị chi tiết đơn hàng
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONDATHANG donhang = db.DONDATHANGs.Find(id);
            var chitiet = db.CT_DONDATHANG.Include(d => d.HANG).Where(d => d.MaDonDatHang == id).ToList();
            if (donhang == null)
            {
                return HttpNotFound();
            }
            return View(chitiet);
        }
    }
}