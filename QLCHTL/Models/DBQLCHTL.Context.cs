﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QLCHTLEntities : DbContext
    {
        public QLCHTLEntities()
            : base("name=QLCHTLEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<HANG> HANGs { get; set; }
        public virtual DbSet<LOAIHANG> LOAIHANGs { get; set; }
        public virtual DbSet<NHACUNGCAP> NHACUNGCAPs { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<GIA> GIAs { get; set; }
        public virtual DbSet<CT_DONDATHANG> CT_DONDATHANG { get; set; }
        public virtual DbSet<DONDATHANG> DONDATHANGs { get; set; }
        public virtual DbSet<COMBO> COMBOes { get; set; }
        public virtual DbSet<CT_COMBO> CT_COMBO { get; set; }
        public virtual DbSet<DANHSACHHANGGIAM> DANHSACHHANGGIAMs { get; set; }
        public virtual DbSet<KHUYENMAI> KHUYENMAIs { get; set; }
        public virtual DbSet<TICHDIEM> TICHDIEMs { get; set; }
        public virtual DbSet<VOUCHER> VOUCHERs { get; set; }
        public virtual DbSet<DANGNHAP> DANGNHAPs { get; set; }
        public virtual DbSet<NHANVIEN> NHANVIENs { get; set; }
        public virtual DbSet<TONKHO> TONKHOes { get; set; }
        public virtual DbSet<CT_PHIEUNHAPHANG> CT_PHIEUNHAPHANG { get; set; }
        public virtual DbSet<PHIEUNHAPHANG> PHIEUNHAPHANGs { get; set; }
    }
}
