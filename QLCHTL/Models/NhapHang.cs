using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLCHTL.Models
{
    public class NhapHang
    {
        QLCHTLEntities db = new QLCHTLEntities();

        public string iMaHang { get; set; }
        public string iTenHang { get; set; }
        public string hinhanh { get; set; }
        public int soluong { get; set; }
        public double DonGia { get; set; }
        public NhapHang(string MaHang)
        {
            iMaHang = MaHang;
            HANG hang = db.HANGs.Single(s => s.MaHang == iMaHang);
            iTenHang = hang.TenHang;
            hinhanh = hang.HinhAnh;
        }

    }
}