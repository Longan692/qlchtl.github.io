using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QLCHTL.Models;
namespace QLCHTL.Models
{
    public class ProductDetail
    {
        QLCHTLEntities db = new QLCHTLEntities();
        public string iMaHang  { get; set; }
        public string iTenHang  { get; set; }
        public string iDVT  { get; set; }
        public string MoTa  { get; set; }   
        public bool iStatus  { get; set; }   
        public string iHinhAnh  { get; set; }
        public double DonGia { get; set; }
        public int Soluong  { get; set; }

        public ProductDetail(string MaHang)
        {
            
            HANG hang = db.HANGs.Single(s => s.MaHang == iMaHang);
            GIA gia = db.GIAs.Single(p => p.MaHang == iMaHang);
            iTenHang = hang.TenHang;
            iHinhAnh = hang.HinhAnh;
            DonGia = double.Parse(gia.DonGia.ToString());
            Soluong = 1;
            
          
        }
    }
}