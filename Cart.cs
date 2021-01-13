using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mojave
{
    public class Cart
    {
        public int CardID { get; set; }
        public List<Item> CartItems { get; set; }
    }
}
