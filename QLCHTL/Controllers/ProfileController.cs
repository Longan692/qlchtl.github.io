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
    public class ProfileController : Controller
    {
        // GET: Profile
        QLCHTLEntities db = new QLCHTLEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ThongTinCaNhan()
        {
            Account Kh = (Account)Session["user"];
            var tt = db.Accounts.Where(_ => _.Id == Kh.Id).SingleOrDefault();
            return View(tt);
        }
        [HttpPost]
        public ActionResult ThongTinCaNhan(Account account)
        {
            try
            {
                Account Kh = (Account)Session["user"];
                var check = db.Accounts.Find(Kh.Id);
                check.Fullname = account.Fullname;
                check.Phone = account.Phone;
                check.Address = account.Address;
                db.Entry(check).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Tb = "Cập nhật thông tin thành công! ";
                var tt = db.Accounts.Where(_ => _.Id == Kh.Id).SingleOrDefault();
                return View(tt);
            }
            catch
            {
                ViewBag.Tb = "Cập nhật thông tin không thành công! ";
                return View();
            }
        }
    }
}