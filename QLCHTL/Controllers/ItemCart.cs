using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QLCHTL.Models;
namespace QLCHTL.Controllers
{
    public class ItemCart
    {
        private HANG product = new HANG();
        private int quantity;

        public ItemCart()
        {

        }
         public ItemCart(HANG product,int quantity)
        {
            this.product = product;
            this.quantity = quantity;

        }

        public HANG Product { get => product; set => product = value; }
        public int Quantity { get => quantity; set => quantity = value; }
    }
}