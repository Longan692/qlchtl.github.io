using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;
namespace QLCHTL.Areas.Admin.Controllers
{
    public class QuanLyTaiKhoanController : Controller
    {
        private QLCHTLEntities db = new QLCHTLEntities();
        // GET: Admin/QuanLyTaiKhoan
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DanhSachTaiKhoan()
        {
            if (Session["user"] == null || Session["user"].ToString() == "")
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
            var acc = db.Accounts.ToList();
            return View(acc);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account nguoidung = db.Accounts.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            return View(nguoidung);
        }
        [HttpPost]
        public ActionResult Edit(Account nguoidung)
        {
            try
            {
                var check = db.Accounts.Find(nguoidung.Id);
                check.Fullname = nguoidung.Fullname;
                check.Phone = nguoidung.Phone;
                check.Address = nguoidung.Address;
                check.Roles = nguoidung.Roles;
                check.Status = nguoidung.Status;
                db.Entry(check).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.ThanhCong = "Chỉnh sửa thông tin tài khoản thành công!";
                return View(check);
            }
            catch
            {
                return View(nguoidung);
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var acc = db.Accounts.Find(id);
            return View(acc);
        }
        // POST: Admin/Nguoidungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                TICHDIEM tichdiem = db.TICHDIEMs.Find(id);
                if (tichdiem != null)
                {
                    db.TICHDIEMs.Remove(tichdiem);
                    db.SaveChanges();
                }
                Account nguoidung = db.Accounts.Find(id);
                db.Accounts.Remove(nguoidung);
                db.SaveChanges();
                ViewBag.Tb = "Xóa người dùng thành công";
                return RedirectToAction("DanhSachTaiKhoan");
            }
            catch
            {
                Account nd = db.Accounts.Find(id);
                ViewBag.Tb = "Xóa người dùng không thành công";
                return View(nd);
            }
        }
    }
}