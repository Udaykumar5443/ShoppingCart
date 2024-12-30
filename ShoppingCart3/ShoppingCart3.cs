using System;
using System.Data.SqlClient;


namespace ShoppingCart2
{
    class _2Shoppingcart
    {
        string connectionString = "server=.;integrated security=true;database=udayprac";

        public void RegisterUser()
        {
            Console.WriteLine("Register New User");

            Console.Write("Enter Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Username: ");
            string username = Console.ReadLine();

            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            Console.Write("Confirm Password: ");
            string confirmPassword = Console.ReadLine();

            if (password != confirmPassword)
            {
                Console.WriteLine("Passwords do not match.");
                return;
            }

            Console.Write("Enter Mobile Number: ");
            string mobileNumber = Console.ReadLine();

            SqlConnection connection = new SqlConnection(connectionString);
            string query = "INSERT INTO Registration (Name, Username, Password, MobileNumber) VALUES ('" +
                           name + "', '" + username + "', '" + password + "', '" + mobileNumber + "')";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            Console.WriteLine("Registration Successful!");
        }

        public void DisplayProducts()
        {
            Console.WriteLine("Available Products:");

            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT ProductID, ProductName, Price, Qty FROM Product";

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(reader["ProductID"] + " - " +
                                  reader["ProductName"] + " - " +
                                  reader["Price"] + " - Qty: " +
                                  reader["Qty"]);
            }

            reader.Close();
            connection.Close();
        }

        public void AddToCart(string username)
        {
            int totalCost = 0;

            while (true)
            {
                Console.Write("Enter ProductID to add to cart: ");
                string productId = Console.ReadLine();

                SqlConnection connection = new SqlConnection(connectionString);
                string query = "SELECT ProductName, Price, Qty FROM Product WHERE ProductID = " + productId;

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string productName = reader["ProductName"].ToString();
                    int price = Convert.ToInt32(reader["Price"]);
                    int qty = Convert.ToInt32(reader["Qty"]);

                    if (qty > 0)
                    {
                        totalCost += price;

                        Console.WriteLine(productName + " added to cart. Price: " + price);

                        reader.Close();

                        string updateQuery = "UPDATE Product SET Qty = Qty - 1 WHERE ProductID = " + productId;
                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.ExecuteNonQuery();

                        string insertQuery = "INSERT INTO Cart (ProductID, ProductName, Username, FinalPrice) " +
                                             "VALUES (" + productId + ", '" + productName + "', '" + username + "', " + price + ")";
                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                        insertCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        Console.WriteLine("Product out of stock.");
                    }
                }
                else
                {
                    Console.WriteLine("Product not found.");
                }

                connection.Close();

                Console.Write("Do you want to add another product? (yes/no): ");
                string answer = Console.ReadLine();
                if (answer.ToLower() != "yes")
                {
                    break;
                }
            }

            Console.WriteLine("Total cost: " + totalCost);
            Console.WriteLine("Thanks For Shopping With Us.");
        }

        public void Start()
        {
            Console.Write("Do you need to register? (yes/no): ");
            string answer = Console.ReadLine();

            if (answer.ToLower() == "yes")
            {
                RegisterUser();
            }

            Console.Write("Enter Username to Login: ");
            string username = Console.ReadLine();

            DisplayProducts();
            AddToCart(username);
        }


    }
}





