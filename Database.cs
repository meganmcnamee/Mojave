using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Data;
using Newtonsoft.Json;

namespace Mojave
{
    public class Database
    {
        private bool m_open = false;
        private SqlConnection m_sqlConnection;
        public Database()
        {

        }

        public bool OpenConnection(string path)
        {
            if (!m_open)
            {
                m_sqlConnection = new SqlConnection(string.Format(Properties.Resources.dbConnectionString, path));
                m_sqlConnection.Open();
                if (m_sqlConnection.State == System.Data.ConnectionState.Open)
                    m_open = true;
            }
            return m_open;
        }

        public bool CloseConnection()
        {
            m_sqlConnection?.Close();
            return m_open = false;
        }

        #region Products

        public List<Product> GetProducts(string clause = "")
        {
            if (!m_open)
                return null;

            var products = new List<Product>();
            var command = new SqlCommand(string.Format("SELECT * FROM Products{0};", clause), m_sqlConnection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var product = new Product()
                {
                    ProductID = (int)reader["ProductID"],
                    ProductName = (string)reader["ProductName"],
                    ProductPrice = (double)reader["ProductPrice"],
                    ProductDescription = (string)reader["ProductDescription"],
                    ProductImage = (string)reader["ProductImage"],
                    ProductCategory = (string)reader["ProductCategory"],
                    ProductQuantity = (int)reader["ProductQuantity"]
                };
                products.Add(product);
            }
            return products;
        }

        public bool AddProduct(Product product)
        {
            bool result = false;
            if (!m_open)
                return result;

            var products = new List<Product>();
            var command = new SqlCommand("INSERT INTO Products (ProductName, ProductPrice, ProductDescription, ProductImage, ProductCategory, ProductQuantity) VALUES (@ProductName, @ProductPrice, @ProductDescription, @ProductImage. @ProductCategory, @ProductQuantity);", m_sqlConnection);
            command.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = product.ProductName;
            command.Parameters.Add("@ProductPrice", SqlDbType.Float).Value = product.ProductPrice;
            command.Parameters.Add("@ProductDescription", SqlDbType.VarChar).Value = product.ProductDescription;
            command.Parameters.Add("@ProductImage", SqlDbType.VarChar).Value = product.ProductImage;
            command.Parameters.Add("@ProductCategory", SqlDbType.VarChar).Value = product.ProductCategory;
            command.Parameters.Add("@ProductQuantity", SqlDbType.VarChar).Value = product.ProductQuantity;

            result = command.ExecuteNonQuery() > 0;

            return result;
        }

        public bool RemoveProduct(Product product)
        {
            bool result = false;
            if (!m_open)
                return result;

            var products = new List<Product>();
            var command = new SqlCommand("DELETE FROM Products WHERE ProductName = @ProductName;", m_sqlConnection);
            command.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = product.ProductName;

            result = command.ExecuteNonQuery() > 0;

            return result;
        }

        public Product FindProduct(string name)
        {
            if (!m_open)
                return null;

            var product = new Product();
            var command = new SqlCommand("SELECT * FROM Products WHERE ProductName = @Name;", m_sqlConnection);
            command.Parameters.AddWithValue("@Name", name);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                product = new Product()
                {
                    ProductID = (int)reader["ProductID"],
                    ProductName = (string)reader["ProductName"],
                    ProductPrice = (double)reader["ProductPrice"],
                    ProductDescription = (string)reader["ProductDescription"],
                    ProductImage = (string)reader["ProductImage"],
                    ProductCategory = (string)reader["ProductCategory"],
                    ProductQuantity = (int)reader["ProductQuantity"]
                };
            }

            return product;
        }

        public Product GetProductByID(int ID)
        {
            if (!m_open)
                return null;

            var product = new Product();
            var command = new SqlCommand("SELECT * FROM Products WHERE ProductID = @ID;", m_sqlConnection);
            command.Parameters.AddWithValue("@ID", ID);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                product = new Product()
                {
                    ProductID = (int)reader["ProductID"],
                    ProductName = (string)reader["ProductName"],
                    ProductPrice = (double)reader["ProductPrice"],
                    ProductDescription = (string)reader["ProductDescription"],
                    ProductImage = (string)reader["ProductImage"],
                    ProductCategory = (string)reader["ProductCategory"],
                    ProductQuantity = (int)reader["ProductQuantity"]
                };
            }

            return product;
        }

        public List<string> GetCategories()
        {
            if (!m_open)
                return null;

            var categories = new List<string>();
            var command = new SqlCommand("SELECT DISTINCT ProductCategory FROM Products", m_sqlConnection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                categories.Add((string)reader["ProductCategory"]);
            }

            return categories;
        }

        #endregion

        public Cart GetCart()
        {
            if (!m_open)
                return null;

            Cart cart = null;
            var command = new SqlCommand("SELECT * FROM Carts WHERE CartID=1;", m_sqlConnection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                cart = JsonConvert.DeserializeObject<Cart>((string)reader["CartItems"]);
            }

            return cart;
        }

        public bool AddItem(int product, int count)
        {
            bool result = false;
            bool exists = true;
            if (!m_open)
                return result;

            var cart = GetCart();
            if (cart == null)
            {
                cart = new Cart();
                cart.CartItems = new List<Item>();
                exists = false;
            }
            int index = -1;
            if((index = cart.CartItems.FindIndex(x => x.ProductID == product)) > -1)
            {
                cart.CartItems[index].ProductQuantity += count;
            }
            else
            {
                var item = new Item() { ProductID = product,  ProductQuantity = count};
                cart.CartItems.Add(item);
            }

            var cartJson = JsonConvert.SerializeObject(cart);

            var command = new SqlCommand((!exists) ? "INSERT INTO Carts (CartItems) VALUES (@CartItem);" : "UPDATE Carts set CartItems = @CartItem WHERE CartID = 1;", m_sqlConnection);
            command.Parameters.Add("@CartItem", SqlDbType.VarChar).Value = cartJson;

            result = command.ExecuteNonQuery() > 0;

            return result;
        }

        public bool RemoveFromCart(int ProductID)
        {
            bool result = false;
            if (!m_open)
                return result;

            Cart cart = GetCart();
            if (cart == null)
                return result;

            cart.CartItems.RemoveAll(x => x.ProductID == ProductID);

            var cartJson = JsonConvert.SerializeObject(cart);

            var command = new SqlCommand("UPDATE Carts set CartItems = @CartItems WHERE CartID=1;", m_sqlConnection);
            command.Parameters.Add("@CartItems", SqlDbType.VarChar).Value = cartJson;
            result = command.ExecuteNonQuery() > 0;

            return result;
        }
    }
}
