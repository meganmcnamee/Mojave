using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mojave
{
    public class Order
    {
        public int OrderID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public int Zipcode { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set;}
        public string EmailAddress { get; set; }
        List<Item> OrderItem { get; set; }
    }
}
