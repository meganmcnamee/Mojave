using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mojave
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImage { get; set; }
        public string ProductCategory { get; set; }
        public int ProductQuantity { get; set; }
    }
}
