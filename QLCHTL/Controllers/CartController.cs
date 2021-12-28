using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using QLCHTL.Models;
using QLCHTL.Constants;
namespace QLCHTL.Controllers
{
    public class CartController : Controller
    {
        //private const string CartSession = "CartSession";
        private QLCHTLEntities db = new QLCHTLEntities();

        public ActionResult Index()
        {
            return View();
        }
        #region Lấy giỏ hàng, nếu không có tạo mới
        //Lay gio hang 
        public List<CartItem> LayGioHang()
        {
            List<CartItem> lstGioHang = Session["CartItem"] as List<CartItem>;
            if (lstGioHang == null)
            {
                //chua co list gio hang thi khoi tao
                lstGioHang = new List<CartItem>();
                Session["CartItem"] = lstGioHang;
            }
            return lstGioHang;
        }
        #endregion Lấy giỏ hàng, nếu không có tạo mới
        #region Thêm sản phẩm mới vào giỏ hàng
        public ActionResult ThemGioHang(string ms, string strURL, int sl)
        {
            //lay gio hang
            List<CartItem> lstGioHang = LayGioHang();
            //kiem tra  ton tai 
            CartItem SanPham = lstGioHang.Find(sp => sp.iMaHang.Equals(ms));
            if (SanPham == null)
            {
                SanPham = new CartItem(ms);
                SanPham.soluong = sl;
                lstGioHang.Add(SanPham);
                return Redirect(strURL);
            }
            else
            {
                //Da co roi 
                //SanPham.soluong++;
                SanPham.soluong = SanPham.soluong + sl;
                return Redirect(strURL);
            }
            //return RedirectToAction("GioHang", "Cart");
        }
        #endregion Thêm sản phẩm mới vào giỏ hàng
        #region Tính tổng tien các sản phẩm
        private double TongThanhTien()
        {
            double ttt = 0;
            List<CartItem> lstGioHang = Session["CartItem"] as List<CartItem>;
            if (lstGioHang != null)
            {
                ttt += lstGioHang.Sum(p => p.ThanhTien);
            }
            return ttt;
        }
        #endregion Tính tổng tien các sản phẩm
        #region Tính tổng số lượng các sản phẩm
        //Tính tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<CartItem> lstGioHang = Session["CartItem"] as List<CartItem>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.soluong);
            }
            return iTongSoLuong;
        }
        #endregion  Tính tổng số lượng các sản phẩm
        #region Hiển thị giỏ hàng
        public ActionResult GioHang()
        {
            List<CartItem> lstGioHang = LayGioHang();
            if (Session["cartitem"] == null)
            {
                return RedirectToAction("Index", "Home");

            }
            if (Session["Voucher"] == null)
            {
                //Kiểm tra đăng đăng nhập
                if (Session["user"] == null || Session["user"].ToString() == "")
                {
                    ViewBag.ThongBao = "Đăng nhập để nhận được nhiều khuyến mãi từ cửa hàng";
                    ViewBag.TamTinh = TongThanhTien();
                    if (TongThanhTien() > 200000)
                    {
                        ViewBag.TongThanhTien = TongThanhTien();
                        ViewBag.PhiShip = "Miễn phí";
                        ViewBag.KM = 0;
                    }
                    else
                    {
                        ViewBag.TongThanhTien = TongThanhTien() + 28000;
                        ViewBag.PhiShip = "28.000 VNĐ";
                        ViewBag.KM = 0;
                    }
                }
                else
                {
                    Account Kh = (Account)Session["user"];
                    var diem = db.TICHDIEMs.Where(_ => _.Id == Kh.Id).FirstOrDefault();
                    ViewBag.TamTinh = TongThanhTien();
                    if (diem.DiemThuong >= RankMember.KimCuong)
                    {
                        if (TongThanhTien() > 200000)
                        {
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.KimCuong) / 100);
                            ViewBag.PhiShip = "Miễn phí";
                            ViewBag.KM = TiLeGiam.KimCuong;
                        }
                        else
                        {
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.KimCuong) / 100) + 28000;
                            ViewBag.PhiShip = "28.000 VNĐ";
                            ViewBag.KM = TiLeGiam.KimCuong;
                        }
                    }
                    else if (diem.DiemThuong < RankMember.KimCuong && diem.DiemThuong >= RankMember.Vang)
                    {
                        if (TongThanhTien() > 200000)
                        {
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.Vang) / 100);
                            ViewBag.PhiShip = "Miễn phí";
                            ViewBag.KM = TiLeGiam.Vang;
                        }
                        else
                        {
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.Vang) / 100) + 28000;
                            ViewBag.PhiShip = "28.000 VNĐ";
                            ViewBag.KM = TiLeGiam.Vang;
                        }
                    }
                    else if (diem.DiemThuong < RankMember.Vang && diem.DiemThuong >= RankMember.Bac)
                    {
                        if (TongThanhTien() > 200000)
                        {
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.Bac) / 100);
                            ViewBag.PhiShip = "Miễn phí";
                            ViewBag.KM = TiLeGiam.Bac;
                        }
                        else
                        {
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.Bac) / 100) + 28000;
                            ViewBag.PhiShip = "28.000 VNĐ";
                            ViewBag.KM = TiLeGiam.Bac;
                        }
                    }
                    else if (diem.DiemThuong < RankMember.Bac)
                    {
                        if (TongThanhTien() > 200000)
                        {
                            ViewBag.TongThanhTien = TongThanhTien();
                            ViewBag.PhiShip = "Miễn phí";
                            ViewBag.KM = 0;
                        }
                        else
                        {
                            ViewBag.TongThanhTien = TongThanhTien() + 28000;
                            ViewBag.PhiShip = "28.000 VNĐ";
                            ViewBag.KM = 0;
                        }
                    }
                }
            }
            else
            {
                VOUCHER vc = (VOUCHER)Session["Voucher"];
                var giamgia = ((100 - vc.TyLeGiam) / 100);
                ViewBag.TamTinh = TongThanhTien();
                if (TongThanhTien() > 200000)
                {
                    ViewBag.TongThanhTien = TongThanhTien() * giamgia;
                    ViewBag.PhiShip = "Miễn phí";
                    ViewBag.KM = vc.TyLeGiam;
                }
                else
                {
                    ViewBag.TongThanhTien = TongThanhTien() * giamgia + 28000;
                    ViewBag.PhiShip = "28.000 VNĐ";
                    ViewBag.KM = vc.TyLeGiam;
                }
                ViewBag.DungVoucher = "*Mã hợp lệ bạn được giảm " + vc.TyLeGiam + "% trên tổng đơn hàng.";
            }

            ViewBag.TongSoLuong = TongSoLuong();
            return View(lstGioHang);
        }
        #endregion Hiển thị giỏ hàng

        #region Partial View giỏ hàng:
        public ActionResult GioHangPartial()
        {
            if (TongSoLuong() == 0)
            {
                ViewBag.TongSoLuong = TongSoLuong();
                return PartialView();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongThanhTien();
            return PartialView();
        }
        #endregion
        #region Cập nhật giỏ hàng
        [HttpPost]
        public ActionResult CapNhatGioHang(FormCollection f)
        {
            string[] quantities = f.GetValues("soluong");
            List<CartItem> lstCart = (List<CartItem>)Session["cartitem"];
            for (int i = 0; i < lstCart.Count(); i++)
            {
                lstCart[i].soluong = Convert.ToInt32(quantities[i]);
            }
            Session["cartitem"] = lstCart;
            return RedirectToAction("GioHang");

        }
        #endregion  Cập nhật giỏ hàng

        #region Xóa giỏ hàng
        public ActionResult XoaGioHang(string iMaSP)
        {
            //Kiểm tra masp
            HANG sp = db.HANGs.SingleOrDefault(n => n.MaHang == iMaSP);
            //Nếu get sai masp thì sẽ trả về trang lỗi 404
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<CartItem> lstGioHang = LayGioHang();
            CartItem sanpham = lstGioHang.SingleOrDefault(n => n.iMaHang == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMaHang == iMaSP);

            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("GioHang");
        }
        #endregion
        #region View đặt hàng
        public ActionResult DatHang()
        {
            //Kiểm tra đăng đăng nhập
            if (Session["user"] == null || Session["user"].ToString() == "")
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (Session["Voucher"] == null)
                {
                    Account Kh = (Account)Session["user"];
                    TICHDIEM diem = db.TICHDIEMs.Find(Kh.Id);
                    if (diem.DiemThuong >= RankMember.KimCuong)
                    {
                        if (TongThanhTien() > 200000)
                        {

                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.KimCuong) / 100);
                        }
                        else
                        {

                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.KimCuong) / 100) + 28000;
                        }
                    }
                    else if (diem.DiemThuong < RankMember.KimCuong && diem.DiemThuong >= RankMember.Vang)
                    {
                        if (TongThanhTien() > 200000)
                        {

                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.Vang) / 100);
                        }
                        else
                        {

                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.Vang) / 100) + 28000;
                        }
                    }
                    else if (diem.DiemThuong < RankMember.Vang && diem.DiemThuong >= RankMember.Bac)
                    {
                        if (TongThanhTien() > 200000)
                        {
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.Bac) / 100);
                        }
                        else
                        {
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.Bac) / 100) + 28000;
                        }
                    }
                    else if (diem.DiemThuong < RankMember.Bac)
                    {
                        if (TongThanhTien() > 200000)
                        {
                            ViewBag.TongThanhTien = TongThanhTien();
                        }
                        else
                        {
                            ViewBag.TongThanhTien = TongThanhTien();
                        }
                    }
                }
                else
                {
                    VOUCHER vc = (VOUCHER)Session["Voucher"];
                    var giamgia = ((100 - vc.TyLeGiam) / 100);
                    ViewBag.TamTinh = TongThanhTien();
                    if (TongThanhTien() > 200000)
                    {
                        ViewBag.TongThanhTien = TongThanhTien() * giamgia;
                    }
                    else
                    {
                        ViewBag.TongThanhTien = TongThanhTien() * giamgia + 28000;
                    }
                }
            }
            //Kiểm tra giỏ hàng
            if (Session["cartitem"] == null)
            {
                RedirectToAction("Index", "Home");
            }
            Account acc = (Account)Session["user"];
            DatHang item = new DatHang();
            item.HoTenNguoiNhan = acc.Fullname;
            item.SDT = acc.Phone;
            item.DiaChi = acc.Address;
            ViewBag.TongSoLuong = TongSoLuong();
            return View(item);
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatHang(DatHang datHang)
        {
            if (ModelState.IsValid)
            {

                DONDATHANG ddh = new DONDATHANG();
                Account Kh = (Account)Session["user"];
                TICHDIEM diem = db.TICHDIEMs.Find(Kh.Id);
                List<CartItem> gh = LayGioHang();
                //Thêm đơn hàng mới:
                ddh.Email = Kh.Email;
                ddh.MaKH = Kh.Id;
                ddh.NgayDat = DateTime.Now;
                ddh.HoTenNguoiNhan = datHang.HoTenNguoiNhan;
                ddh.SDT = datHang.SDT;
                ddh.DiaChi = datHang.DiaChi;
                ddh.GhiChu = datHang.GhiChu;
                ddh.TinhTrang = "Đang chờ duyệt";
                if (Session["Voucher"] == null)
                {
                    if (diem.DiemThuong >= RankMember.KimCuong)
                    {
                        if (TongThanhTien() > 200000)
                        {
                            ddh.TongTien = TongThanhTien() * ((100 - TiLeGiam.KimCuong) / 100);
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.KimCuong) / 100);
                        }
                        else
                        {
                            ddh.TongTien = TongThanhTien() * ((100 - TiLeGiam.KimCuong) / 100) + 28000;
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.KimCuong) / 100) + 28000;
                        }
                    }
                    else if (diem.DiemThuong < RankMember.KimCuong && diem.DiemThuong >= RankMember.Vang)
                    {
                        if (TongThanhTien() > 200000)
                        {
                            ddh.TongTien = TongThanhTien() * ((100 - TiLeGiam.Vang) / 100);
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.Vang) / 100);
                        }
                        else
                        {
                            ddh.TongTien = TongThanhTien() * ((100 - TiLeGiam.Vang) / 100) + 28000;
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.Vang) / 100) + 28000;
                        }
                    }
                    else if (diem.DiemThuong < RankMember.Vang && diem.DiemThuong >= RankMember.Bac)
                    {
                        if (TongThanhTien() > 200000)
                        {
                            ddh.TongTien = TongThanhTien() * ((100 - TiLeGiam.Bac) / 100);
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.Bac) / 100);

                        }
                        else
                        {
                            ddh.TongTien = TongThanhTien() * ((100 - TiLeGiam.Bac) / 100) + 28000;
                            ViewBag.TongThanhTien = TongThanhTien() * ((100 - TiLeGiam.Bac) / 100) + 28000;
                        }
                    }
                    else if (diem.DiemThuong < RankMember.Bac)
                    {
                        if (TongThanhTien() > 200000)
                        {
                            ddh.TongTien = TongThanhTien();
                            ViewBag.TongThanhTien = TongThanhTien();
                        }
                        else
                        {
                            ddh.TongTien = TongThanhTien() + 28000;
                            ViewBag.TongThanhTien = TongThanhTien();
                        }
                    }
                }
                else
                {
                    VOUCHER vc = (VOUCHER)Session["Voucher"];
                    var giamgia = ((100 - vc.TyLeGiam) / 100);
                    ViewBag.TamTinh = TongThanhTien();
                    if (TongThanhTien() > 200000)
                    {
                        ddh.TongTien = TongThanhTien() * giamgia;
                        ViewBag.TongThanhTien = TongThanhTien() * giamgia;
                    }
                    else
                    {
                        ddh.TongTien = TongThanhTien() * giamgia + 28000;
                        ViewBag.TongThanhTien = TongThanhTien() * giamgia + 28000;
                    }
                }
                ViewBag.TongSoLuong = TongSoLuong();
                if (datHang.HoTenNguoiNhan == null && datHang.SDT == null && datHang.DiaChi == null)
                {
                    ViewBag.Erro = "Vui lòng nhập đầy đủ thông tin";
                    return View();
                }
                if (datHang.HoTenNguoiNhan == null)
                {
                    ViewBag.ErroName = "Vui lòng nhập tên người nhận";
                    return View();

                }
                if (datHang.SDT == null)
                {
                    ViewBag.ErroSDT = "Vui lòng nhập số điện thoại";
                    return View();

                }
                if (datHang.DiaChi == null)
                {
                    ViewBag.ErroDiaChi = "Vui lòng nhập địa chỉ";
                    return View();

                }
                Console.WriteLine(ddh);
                db.DONDATHANGs.Add(ddh);
                db.SaveChanges();

                //TTCH DIEM CHO THANH VIEN
                diem.Id = Kh.Id;
                diem.TenKH = Kh.Fullname;
                diem.DiemThuong = diem.DiemThuong + (TongThanhTien() / 10000);
                db.Entry(diem).State = EntityState.Modified;
                db.SaveChanges();
                //TheM CHI TIET DON HANG
                foreach (var item in gh)
                {
                    //Update số lượng hàng.
                    HANG hang = db.HANGs.Find(item.iMaHang);
                    hang.Soluong = hang.Soluong - item.soluong;
                    db.Entry(hang).State = EntityState.Modified;
                    //Thêm chi tiết đơn hàng
                    CT_DONDATHANG ctDDH = new CT_DONDATHANG();
                    ctDDH.MaDonDatHang = ddh.MaDonDatHang;
                    ctDDH.MaHang = item.iMaHang;
                    ctDDH.TenHang = item.iTenHang;
                    ctDDH.SoLuong = item.soluong;
                    //tinh trang =0 la chưa thanh toan
                    ctDDH.TinhTrangThanhToan = 0;
                    db.CT_DONDATHANG.Add(ctDDH);
                }
                db.SaveChanges();
                var tongsl = TongSoLuong();
                Session["cartitem"] = null;
                
                string subject = "THÔNG TIN BÁO XÁC NHẬN ĐƠN HÀNG DOUBLE A";
                string content = System.IO.File.ReadAllText(Server.MapPath("/assets/template/MailXacNhanDonHang.html"));
                content = content.Replace("{{Fullname}}", Kh.Fullname);
                content = content.Replace("{{TongSanPham}}", tongsl.ToString());
                content = content.Replace("{{TongTien}}", ddh.TongTien.Value.ToString("#,##0").Replace(',', '.'));

                content = content.Replace("{{Email}}", ddh.Email);
                content = content.Replace("{{NgayDat}}", ddh.NgayDat.ToString());
                content = content.Replace("{{Hotennguoinhan}}", ddh.HoTenNguoiNhan.ToString());
                content = content.Replace("{{SDT}}", ddh.SDT.ToString());
                content = content.Replace("{{DiaChi}}", ddh.DiaChi.ToString());
               
                //string body = "Cảm ơn bạn đã đặt hàng tại cửa hàng chúng tôi. " +
                //    " Tổng đơn hàng của bạn là " + ddh.TongTien.Value.ToString("#,##0").Replace(',', '.') + "VNĐ. " +
                //    "Nhân viên sẽ gọi xác nhận đơn hàng của bạn trong thời gian sớm nhất. Mọi thông tin thắc mắc xin vui lòng liên hệ đến hotline: 0845.475.575";
                sendMail(Kh.Email, subject, content);
                ViewBag.tb = "Thông tin đơn hàng đã được gửi đến mail của bạn vui lòng kiểm tra để bạn nhé!";
                return RedirectToAction("Index", "DonHang");
            }

            return View();
        }

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
        [HttpPost]
        public ActionResult Voucher(string maVoucher)
        {
            var check = db.VOUCHERs.FirstOrDefault(_ => _.MaVoucher.Equals(maVoucher));
            //Kiểm tra đăng đăng nhập
            if (Session["user"] == null || Session["user"].ToString() == "")
            {
                return RedirectToAction("Login", "Home");
            }
            if (check != null)
            {
                Session["Voucher"] = check;
            }
            else
            {
                ViewBag.SaiVoucher = "*Mã COUPON không hợp lệ, khách hàng vui lòng sử dụng mã khác";
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaVoucher()
        {
            Session["Voucher"] = null;
            return RedirectToAction("GioHang");
        }
        #region MUA combo vào giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemCombo(List<AddCombo> combo)
        {
            foreach (var item in combo)
            {
                //lay gio hang
                List<CartItem> lstGioHang = LayGioHang();
                //kiem tra  ton tai 
                CartItem SanPham = lstGioHang.Find(sp => sp.iMaHang.Equals(item.addgia.ct_combodetail.MaHang));
                if (SanPham == null)
                {
                    SanPham = new CartItem(item.addgia.ct_combodetail.MaHang);
                    SanPham.GiaCu = SanPham.DonGia;
                    SanPham.DonGia = (double)(SanPham.DonGia * ((100 - item.addgia.combodetail.TyLeKM) / 100));
                    SanPham.soluong = 1;
                    lstGioHang.Add(SanPham);
                    //return Redirect(item.strURL);
                }
                else
                {
                    //Da co roi 
                    SanPham.soluong++;
                    //SanPham.soluong = SanPham.soluong + sl;

                }
            }
            //return Redirect(strURL);
            return RedirectToAction("GioHang", "Cart");
        }
        #endregion

    }

}