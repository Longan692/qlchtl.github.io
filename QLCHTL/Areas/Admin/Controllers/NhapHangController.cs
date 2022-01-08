using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinqToExcel;
using OfficeOpenXml;
using QLCHTL.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace QLCHTL.Areas.Admin.Controllers
{
    public class NhapHangController : Controller
    {
        private QLCHTLEntities db = new QLCHTLEntities();
        // GET: Admin/NhapHang
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NhapHang()
        {
            List<CartItem> lstNhapHang = LayNhapHang();
            //Kiểm tra đăng đăng nhập
            if (Session["user"] == null || Session["user"].ToString() == "")
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
            return View(lstNhapHang);
        }
        public ActionResult NhapHangMoi()
        {
            ViewBag.MaHang = db.HANGs.ToList();
            return View();
        }
        #region Tạo đơn đặt hàng
        //Lay gio hang 
        public List<CartItem> LayNhapHang()
        {
            List<CartItem> lstNhapHang = Session["NhapHang"] as List<CartItem>;
            if (lstNhapHang == null)
            {
                //chua co list gio hang thi khoi tao
                lstNhapHang = new List<CartItem>();
                Session["NhapHang"] = lstNhapHang;
            }
            return lstNhapHang;
        }
        #endregion
        #region Thêm sản phẩm mới vào giỏ hàng
        [HttpPost]
        public ActionResult ThemDatHang(string iMaHang, string strURL, int soluong, double DonGia)
        {
            //lay gio hang
            List<CartItem> lstNhapHang = LayNhapHang();
            //kiem tra  ton tai 
            CartItem SanPham = lstNhapHang.Find(sp => sp.iMaHang.Equals(iMaHang));
            if (SanPham == null)
            {
                SanPham = new CartItem(iMaHang);
                SanPham.DonGia = DonGia;
                SanPham.soluong = soluong;
                lstNhapHang.Add(SanPham);
                return Redirect(strURL);
            }
            else
            {
                //Da co roi 
                //SanPham.soluong++;
                SanPham.soluong = SanPham.soluong + soluong;
                return Redirect(strURL);
            }
        }
        #endregion
        public ActionResult DanhSachNhapHang()
        {
            List<CartItem> lstNhapHang = LayNhapHang();
            //Kiểm tra đăng đăng nhập
            if (Session["user"] == null || Session["user"].ToString() == "")
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }
            return View(lstNhapHang);
        }
        #region Xóa item in ds nhap hang
        public ActionResult XoaGioHang(string iMaSP)
        {

            //Lấy giỏ hàng ra từ session
            List<CartItem> lstNhapHang = LayNhapHang();
            CartItem sanpham = lstNhapHang.SingleOrDefault(n => n.iMaHang == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                lstNhapHang.RemoveAll(n => n.iMaHang == iMaSP);

            }
            if (lstNhapHang.Count == 0)
            {
                return RedirectToAction("NhapHang", "NhapHang");
            }

            return RedirectToAction("NhapHang");
        }
        #endregion

        public ActionResult NhapHangTay()
        {
            if (ModelState.IsValid)
            {
                PHIEUNHAPHANG pnh = new PHIEUNHAPHANG();
                Account Nv = (Account)Session["user"];
                List<CartItem> gh = LayNhapHang();
                if (gh.Count() == 0)
                {
                    return RedirectToAction("NhapHang", "NhapHang");
                }
                //Thêm phiếu nhập hàng mới
                pnh.NgayNhap = DateTime.Now;
                pnh.TenNhanVien = Nv.Fullname;
                Console.WriteLine(pnh);
                db.PHIEUNHAPHANGs.Add(pnh);
                db.SaveChanges();

                //Them chi tiết phiếu nhập
                foreach (var item in gh)
                {
                    //Thêm chi tiết đơn hàng
                    CT_PHIEUNHAPHANG ctPNH = new CT_PHIEUNHAPHANG();
                    ctPNH.MaPhieuNH = pnh.MaPhieuNH;
                    ctPNH.MaHang = item.iMaHang;
                    ctPNH.GiaNhap = item.DonGia;
                    ctPNH.SoLuongNhap = item.soluong;
                    db.CT_PHIEUNHAPHANG.Add(ctPNH);
                    //Update số lượng hàng.
                    HANG hang = db.HANGs.Find(item.iMaHang);
                    hang.Soluong = hang.Soluong + ctPNH.SoLuongNhap;
                    db.Entry(hang).State = EntityState.Modified;
                }
                db.SaveChanges();
                Session["NhapHang"] = null;
                return RedirectToAction("NhapHang", "NhapHang");
            }
            return View();
        }

        public FileResult DownloadExcel()
        {
            string path = "/Doc/Template.xlsx";
            return File(path, "application/vnd.ms-excel", "Template.xlsx");
        }
        [HttpGet]
        public ActionResult UploadExcel()
        {
            return View();
        }

        //#region Nhập hàng bằng excel
        //[HttpPost]
        //public JsonResult UploadExcel(HttpPostedFileBase FileUpload)
        //{
        //    List<string> data = new List<string>();
        //    if (FileUpload != null)
        //    {
        //        // tdata.ExecuteCommand("truncate table OtherCompanyAssets");
        //        if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //        {
        //            string filename = FileUpload.FileName;
        //            string targetpath = Server.MapPath("~/Doc/");
        //            FileUpload.SaveAs(targetpath + filename);
        //            string pathToExcelFile = targetpath + filename;
        //            var connectionString = "";
        //            if (filename.EndsWith(".xls"))
        //            {
        //                connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
        //            }
        //            else if (filename.EndsWith(".xlsx"))
        //            {
        //                connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0 Xml;HDR=YES;IMEX=1;", pathToExcelFile);
        //            }

        //            var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1]", connectionString);
        //            var ds = new DataSet();
        //            adapter.Fill(ds, "ExcelTable");
        //            DataTable dtable = ds.Tables["ExcelTable"];
        //            string sheetName = "Sheet1";
        //            var excelFile = new ExcelQueryFactory(pathToExcelFile);
        //            var artistAlbums = from a in excelFile.Worksheet<CartItem>(sheetName) select a;

        //            //Lặp tất cả
        //            foreach (var a in artistAlbums)
        //            {
        //                try
        //                {
        //                    var check = db.HANGs.Where(_ => _.MaHang.Trim().Equals(a.iMaHang.Trim())).FirstOrDefault();
        //                    if (check == null)
        //                    {
        //                        ViewBag.Loi = "Không tìm thấy mã hàng cần nhập. Vui lòng kiểm tra lại!";
        //                    }
        //                    if (a.iMaHang != "" && a.iTenHang != "" && a.soluong != 0 && a.DonGia != 0)
        //                    {
        //                        //lay gio hang
        //                        List<CartItem> lstNhapHang = LayNhapHang();
        //                        //kiem tra  ton tai 
        //                        CartItem SanPham = lstNhapHang.Find(sp => sp.iMaHang.Equals(a.iMaHang));
        //                        if (SanPham == null)
        //                        {
        //                            SanPham = new CartItem(a.iMaHang);
        //                            SanPham.DonGia = a.DonGia;
        //                            SanPham.soluong = a.soluong;
        //                            lstNhapHang.Add(SanPham);

        //                        }
        //                        else
        //                        {
        //                            //Da co roi 
        //                            //SanPham.soluong++;
        //                            SanPham.soluong = SanPham.soluong + a.soluong;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        data.Add("<ul>");
        //                        if (a.iMaHang == "" || a.iMaHang == null) data.Add("<li> Mã hàng </li>");
        //                        if (a.iTenHang == "" || a.iTenHang == null) data.Add("<li> Tên hàng</li>");
        //                        if (a.soluong == 0 || a.soluong < 0) data.Add("<li> Số lượng </li>");
        //                        if (a.DonGia == 0 || a.DonGia < 0) data.Add("<li>Đơn giá</li>");
        //                        data.Add("</ul>");
        //                        data.ToArray();
        //                        return Json(data, JsonRequestBehavior.AllowGet);
        //                    }
        //                }
        //                catch (DbEntityValidationException ex)
        //                {
        //                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
        //                    {
        //                        foreach (var validationError in entityValidationErrors.ValidationErrors)
        //                        {
        //                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
        //                        }
        //                    }
        //                }
        //            }
        //            //deleting excel file from folder
        //            if ((System.IO.File.Exists(pathToExcelFile)))
        //            {
        //                System.IO.File.Delete(pathToExcelFile);
        //            }
        //            return Json("success", JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            //alert message for invalid file format
        //            data.Add("<ul>");
        //            data.Add("<li>Only Excel file format is allowed</li>");
        //            data.Add("</ul>");
        //            data.ToArray();
        //            return Json(data, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else
        //    {
        //        data.Add("<ul>");
        //        if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
        //        data.Add("</ul>");
        //        data.ToArray();
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //#endregion 

        [HttpPost]
        public ActionResult ImportEx(HttpPostedFileBase excelFile)
        {
            if (excelFile == null || excelFile.ContentLength == 0 )
            {
              
                return View();
            }
            else
            {
                if (excelFile.FileName.EndsWith("xls") || excelFile.FileName.EndsWith("xlsx"))
                {
                    var package = new ExcelPackage(excelFile.InputStream);
                    ExcelWorksheet ws = package.Workbook.Worksheets[1];

                    for (int rw = 2; rw <= ws.Dimension.End.Row; rw++)
                    {
                        var itemID = ws.Cells[rw, 1].Value;
                        var tenHANG = ws.Cells[rw, 2].Value;
                        double itemGia = (double)ws.Cells[rw, 3].Value;
                        int itemSL =Convert.ToInt32(ws.Cells[rw, 4].Value);
                        var check = db.HANGs.Where(_ => _.MaHang.Equals(itemID.ToString())).FirstOrDefault();
                        if (check == null)
                        {
                            ViewBag.Loi = "Không tìm thấy mã hàng cần nhập. Vui lòng kiểm tra lại!";
                        }
                        if (itemID.ToString() != "" && tenHANG.ToString() != "" && itemSL != 0 && itemGia != 0)
                        {
                            //lay gio hang
                            List<CartItem> lstNhapHang = LayNhapHang();
                            //kiem tra  ton tai 
                            CartItem SanPham = lstNhapHang.Find(sp => sp.iMaHang.Equals(itemID.ToString()));
                            if (SanPham == null)
                            {
                                SanPham = new CartItem(itemID.ToString());
                                SanPham.DonGia = itemGia;
                                SanPham.soluong = itemSL;
                                lstNhapHang.Add(SanPham);
                            }
                            else
                            {
                                //Da co roi 
                                //SanPham.soluong++;
                                SanPham.soluong = SanPham.soluong + itemSL;
                            }
                        }
                    }
                    return RedirectToAction("NhapHang");
                }
            }
            return View();
        }
    }
}