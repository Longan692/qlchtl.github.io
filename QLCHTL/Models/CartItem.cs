using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLCHTL.Models
{
    public class CartItem
    {
        QLCHTLEntities db = new QLCHTLEntities();

        public string iMaHang { get; set; }
        public string iTenHang { get; set; }

        public string iDVT { get; set; }
        public string iHinhAnh { get; set; }
        public int soluong { get; set; }
        public double DonGia { get; set; }
        public int iStatus { get; set; }
        public string Status { get; set; }
        public string iMota { get; set; }
        public double DiemThuong { get; set; }
        public int Stock { get; set; }
        public double GiaCu { get; set; }

        public double ThanhTien
        {
            get { return soluong * DonGia; }
        }
        public CartItem(string MaHang)
        {
            iMaHang = MaHang;
            HANG hang = db.HANGs.Single(s => s.MaHang == iMaHang);
            GIA gia = db.GIAs.Single(p => p.MaHang == iMaHang);
            iTenHang = hang.TenHang;
            iHinhAnh = hang.HinhAnh;
            DonGia = double.Parse(gia.DonGia.ToString());
            //soluong = 1;
            iStatus = (int)hang.TrangThai;
            iMota = hang.MoTa;
            iDVT = hang.DVT;
            Stock = (int)hang.Soluong;
            GiaCu = 0;
            //if (iStatus == true)
            //{
            //    Status = "Còn hàng";
            //}
            //else
            //{
            //    Status = "Hết hàng";
            //}
        }
    }
}