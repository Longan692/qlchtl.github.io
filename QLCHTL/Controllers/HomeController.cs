using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;
namespace QLCHTL.Controllers
{
    public class HomeController : Controller
    {
        private QLCHTLEntities de = new QLCHTLEntities();
        public ActionResult Index()
        {
            //if (Session["Id"] != null)
            //{
            //    List<HANG> productname = de.HANGs.ToList();
            //    List<GIA> giabanmoi = de.GIAs.ToList();
            //    var item = from p in productname
            //               join g in giabanmoi
            //              on p.MaHang equals g.MaHang into tb1
            //               from g in tb1.DefaultIfEmpty()
            //               select new SanPham { productdetails = p, giabandetails = g };
            //    return View(item);
            //}
            //else
            //{
            //    return RedirectToAction("Login");
            //}
            List<HANG> productname = de.HANGs.ToList();
            List<GIA> giabanmoi = de.GIAs.ToList();
            var item = from p in productname
                       join g in giabanmoi
                      on p.MaHang equals g.MaHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new SanPham { productdetails = p, giabandetails = g };
            return View(item);
        }
        //public ActionResult ThucAn(string maloai)
        //{
        //    List<HANG> productname = de.HANGs.Where(p => p.MaLoaiHang==maloai).ToList();
        //    List<GIA> giabanmoi = de.GIAs.ToList();
        //    var item = from p in productname
        //               join g in giabanmoi
        //              on p.MaHang equals g.MaHang into tb1
        //               from g in tb1.DefaultIfEmpty()
        //               select new SanPham { productdetails = p, giabandetails = g };
        //    return View(item);
        //}
        //GET: Register

        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Account _user)
        {
            if (ModelState.IsValid)
            {
                var check = de.Accounts.FirstOrDefault(s => s.Email == _user.Email || s.Username==_user.Username);
                if (check == null)
                {
                    _user.Password = GetMD5(_user.Password);
                    _user.IsAdmin =false;
                   
                    _user.Status = true;
                    de.Configuration.ValidateOnSaveEnabled = false;
                    de.Accounts.Add(_user);
                    de.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Account already exists";
                    return View();
                }


            }
            return View();


        }
        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserName, string PassWord)
        {
            if (ModelState.IsValid)
            {

                var f_password = GetMD5(PassWord);
                //var checkAdmin = de.Accounts.Where(_ => _.IsAdmin==false).ToList();
                var data = de.Accounts.SingleOrDefault(s => s.Username.Equals(UserName) && s.Password.Equals(f_password));

                //if (data.Count() > 0)
                //{
                //    if (data.FirstOrDefault().IsAdmin!=false)
                //    {
                //        //add session admin
                //        Session["UserName"] = data.FirstOrDefault().Username;
                //        Session["FullName"] = data.FirstOrDefault().Fullname;
                //        Session["Email"] = data.FirstOrDefault().Email;
                //        Session["id"] = data.FirstOrDefault().Id;
                //        return RedirectToAction("Index","Admin/QuanLyHang");

                //    }
                //    else
                //    {
                //        //add session
                //        Session["UserName"] = data.FirstOrDefault().Username;
                //        Session["FullName"] = data.FirstOrDefault().Fullname;
                //        Session["Email"] = data.FirstOrDefault().Email;
                //        Session["id"] = data.FirstOrDefault().Id;
                //        return RedirectToAction("Index");
                //    }
                if (data!=null)
                {
                    if (data.IsAdmin != false)
                    {
                        //add session admin
                        Session["user"] = data;
                        return RedirectToAction("Index", "Admin/QuanLyHang");

                    }
                    else
                    {
                        //add session
                        Session["user"] = data;
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }



        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
        public ActionResult ThucAn()
        {
        
                List<HANG> productname = de.HANGs.ToList();
                List<GIA> giabanmoi = de.GIAs.ToList();
                var item = from p in productname
                           join g in giabanmoi
                          on p.MaHang equals g.MaHang into tb1
                           from g in tb1.DefaultIfEmpty()
                           select new SanPham { productdetails = p, giabandetails = g };
                return View(item);
          
           
        }
        public ActionResult DangXuat()
        {
            Session["user"] = null;
            return RedirectToAction("Index", "Home");

        }
        //public ActionResult DangNhapPartial()
        //{
        //    if (TongSoLuong() == 0)
        //    {
        //        return PartialView();
        //    }
        //    ViewBag.TongSoLuong = TongSoLuong();
        //    ViewBag.TongTien = TongThanhTien();
        //    return PartialView();
        //}




    }
}