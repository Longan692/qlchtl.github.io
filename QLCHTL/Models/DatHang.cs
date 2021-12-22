using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLCHTL.Models
{
    public class DatHang
    {
        QLCHTLEntities db = new QLCHTLEntities();
        public int Id { get; set; }
        public string HoTenNguoiNhan { get; set; }
        public string SDT { get; set; }
        public string GhiChu { get; set; }
        public string DiaChi { get; set; }
     
       
    }
}