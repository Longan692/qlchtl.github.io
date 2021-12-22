using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using QLCHTL.Constants;
using QLCHTL.Models;
namespace QLCHTL.Controllers
{
    public class HomeController : Controller
    {
        private QLCHTLEntities de = new QLCHTLEntities();
        public ActionResult Index()
        {
            List<HANG> productname = de.HANGs.ToList();
            List<GIA> giabanmoi = de.GIAs.ToList();
            var item = from p in productname
                       join g in giabanmoi
                      on p.MaHang equals g.MaHang into tb1
                       from g in tb1.DefaultIfEmpty()
                       select new SanPham { productdetails = p, giabandetails = g };
            ViewBag.MoiNhat = item.OrderByDescending(p =>p.productdetails.MaHang /*p.productdetails.MaLoaiHang.Trim().Equals("LH001") &&*/).Where(p=>p.productdetails.TrangThai == StatusProduct.ConHang).Take(10).ToList();
            ViewBag.SapBan = item.OrderByDescending(p => p.productdetails.MaHang /*p.productdetails.MaLoaiHang.Trim().Equals("LH001") &&*/).Where(p => p.productdetails.TrangThai == StatusProduct.HangSapBan).Take(10).ToList();
            return View();
        }

        //GET: Register

        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Account _user,string ConfirmPassword)
        {
            if (ModelState.IsValid)
            {
                var checkmail = de.Accounts.FirstOrDefault(s => s.Email == _user.Email);
                var checkname = de.Accounts.FirstOrDefault(s => s.Username == _user.Username);
                
                if (checkmail != null)
                {
                    ViewBag.TrungMail = "Email đã tồn tại vui lòng, xin vui lòng thử lại 1 tài khoảng khác";
                    return View();
                }
                if (checkname != null)
                {
                    ViewBag.TrungName = "UserName đã tồn tại vui lòng, xin vui lòng thử lại 1 username khác";
                    return View();
                }
               
                if (_user.Username != null || _user.Email != null || _user.Fullname != null || _user.Address != null || _user.Phone != null)
                {
                    if (_user.Password != ConfirmPassword)
                    {
                        ViewBag.SaiPass = "Password không trùng khớp nhau, vui lòng kiểm tra lại";
                        return View();
                    }
                    _user.Password = GetMD5(_user.Password);
                    _user.Roles = Role.KhachHang;
                    _user.Status = true;
                    de.Configuration.ValidateOnSaveEnabled = false;
                    de.Accounts.Add(_user);
                    de.SaveChanges();
                    //Them khách hàng vào bảng tích điểm
                    TICHDIEM td = new TICHDIEM();
                    td.Id = _user.Id;
                    td.TenKH = _user.Fullname;
                    td.DiemThuong = 0;
                    de.TICHDIEMs.Add(td);
                    de.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Vui lòng nhập đầy đủ thông tin";
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
                var checkName = de.Accounts.SingleOrDefault(s => s.Username.Equals(UserName));
                var checkPass = de.Accounts.FirstOrDefault(s => s.Password.Equals(f_password));
                if (checkName == null)
                {
                    ViewBag.SaiName = "UserName không tồn tại, vui lòng kiểm tra lại";
                    return View();
                }
                if (checkPass == null)
                {
                    ViewBag.SaiPass = "Sai password, vui lòng kiểm tra lại";
                    return View();
                }
                if (data != null)
                {
                    if (data.Roles == Role.Admin)
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
                    ViewBag.error = "Đăng nhập không thành công, hãy thử lại";
                    return View();
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
        #region Tự động tạo mật khẩu
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder sb = new StringBuilder();
            char c;
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                c = Convert.ToChar(Convert.ToInt32(rand.Next(65, 87)));
                sb.Append(c);
            }
            if (lowerCase)
            {
                return sb.ToString().ToLower();
            }
            return sb.ToString();
        }
        #endregion
        #region Gửi mail xác nhận lại mật khẩu
        public bool sendMail(string Emailto, string subject, string body)
        {
            var Email = System.Configuration.ConfigurationManager.AppSettings["Gmail"].ToString();
            MailMessage obj = new MailMessage(Email, Emailto);
            obj.Subject = subject;
            obj.Body = body;
            obj.IsBodyHtml = true;
            string Message = null;

            MailAddress bcc = new MailAddress("devddv123@gmail.com");
            obj.Bcc.Add(bcc);
            using (SmtpClient smtp = new SmtpClient())
            {
                var Password = System.Configuration.ConfigurationManager.AppSettings["PasswordGmail"].ToString();

                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential(Email, Password);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                try
                {
                    smtp.Send(obj);
                    Message = "Gửi mail thành công";
                    return true;
                }
                catch (Exception ex)
                {
                    Message = "Err:" + ex.ToString();
                    return false;
                }
            }
        }
        #endregion
        #region VIew Quên mật khẩu
        [HttpGet]
        public ActionResult QuenMatKhau()
        {
            return View();
        }
        #endregion
        #region Chức năng quên mật khẩu
        [HttpPost]
        public ActionResult QuenMatKhau(FormCollection col)
        {
            string email = col["Email"];
            string subject = "YÊU CẦU CẤP LẠI MẬT KHẨU?";
            try
            {
                var password = RandomString(8, true);
                Account account = de.Accounts.Where(s => s.Email == email).FirstOrDefault();
                if(account==null)
                {
                    ViewBag.tb = "Email không chính xác, vui lòng kiểm tra lại";
                    return View();
                }    
                account.Password = GetMD5(password);
                de.SaveChanges();
                Account acc = de.Accounts.Where(s => s.Email == email).FirstOrDefault();
                string body = "Xin chào!  "+acc.Email+" Đây là mật khẩu mới của bạn sau khi yêu cầu thay đổi: " + password + ". Vui lòng thay đổi mật khẩu sau khi đăng nhập lại!";
                sendMail(acc.Email, subject, body);
                ViewBag.tb = "Yêu cầu lấy lại mật khẩu thành công, vui lòng quay lại trang đăng nhập";
            }
            catch
            {
                ViewBag.tb = "Yêu cầu cấp lại mật khẩu không thành công, vui lòng kiểm tra lại";
            }
            return View();
        }
        #endregion
        #region View Đổi mật khẩu
        public ActionResult DoiMatKhau()
        {
            return View();
        }
        #endregion
        #region Đổi mật khẩu
        [HttpPost]
        public ActionResult DoiMatKhau(FormCollection col)
        {
            string MatKhauCu = GetMD5(col["MatKhauCu"]);
            string MatKhauMoi = col["MatKhauMoi"];
            string XacNhanMK = col["XacNhanMatKhau"];
            Account Kh = (Account)Session["user"];
            try
            {
                Account account = de.Accounts.Where(s => s.Email == Kh.Email && s.Password == MatKhauCu).FirstOrDefault();
                if (account == null)
                {
                    ViewBag.tbSaiPassCu = "Mật khẩu cũ không chính xác, vui lòng kiểm tra lại!";
                    return View();
                }
                if (MatKhauMoi != XacNhanMK)
                {
                    ViewBag.tbPassNotMer = "Mật khẩu mới vừa nhập không trùng khớp nhau, vui lòng kiểm tra lại!";
                    return View();
                }
                account.Password = GetMD5(MatKhauMoi);
                de.SaveChanges();
                ViewBag.tb = "Đã thay đổi mật khẩu thành công, vui lòng đăng nhập lại bằng mật khẩu mới!";
                return RedirectToAction("Login", "Home");
            }
            catch
            {
                ViewBag.tb = "Không thể thay đổi mật khẩu!";
            }
            return View();
        }
        #endregion
        public ActionResult MenuCategoryPartial()
        {
            ViewBag.MenuDanhMuc = de.LOAIHANGs.ToList();
            return View();
        }
       
    }
}