//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLCHTL.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VOUCHER
    {
        public string MaVoucher { get; set; }
        public string TenVoucher { get; set; }
        public string MaDotKM { get; set; }
        public Nullable<double> TyLeGiam { get; set; }
    
        public virtual KHUYENMAI KHUYENMAI { get; set; }
    }
}
