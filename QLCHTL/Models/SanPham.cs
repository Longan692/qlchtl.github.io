using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLCHTL.Models
{
    public class SanPham
    {

        public GIA giabandetails { get; set; }

        public HANG productdetails { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }


    //public class SanPhamV2
    //{
    //    public string MaHang { get; set; }
    //    public System.DateTime Ngay { get; set; }
    //    public string DonGia { get; set; }

    //    public virtual HANG HANG { get; set; }

    //    public string TenHang { get; set; }
    //    public string MaLoaiHang { get; set; }
    //    public string DVT { get; set; }
    //    public string MaNCC { get; set; }
    //    public string MoTa { get; set; }
    //    public string HinhAnh { get; set; }
    //    public virtual ICollection<GIA> GIAs { get; set; }
    //    public virtual LOAIHANG LOAIHANG { get; set; }
    //    public virtual NHACUNGCAP NHACUNGCAP { get; set; }

    //}
}