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

            if (Session["idUser"] != null)
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
            else
            {
                return RedirectToAction("Login");
            }

        }
        //GET: Register

        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(TaiKhoang _user)
        {
            if (ModelState.IsValid)
            {
                var check = de.TaiKhoangs.FirstOrDefault(s => s.Email == _user.Email);
                if (check == null)
                {
                    _user.IdUsers = "003";
                    _user.MatKhau = GetMD5(_user.MatKhau);
                    de.Configuration.ValidateOnSaveEnabled = false;
                    de.TaiKhoangs.Add(_user);
                    de.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Email already exists";
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
        public ActionResult Login(string email, string Matkhau)
        {
            if (ModelState.IsValid)
            {


                var f_password = GetMD5(Matkhau);
                var data = de.TaiKhoangs.Where(s => s.Email.Equals(email) && s.MatKhau.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["idUser"] = data.FirstOrDefault().IdUsers;
                    return RedirectToAction("Index");
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


       
           

    }
}