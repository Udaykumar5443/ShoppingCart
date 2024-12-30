using System;
using System.Data.SqlClient;

namespace ShoppingCartProject1
{
    class ShoppingCart
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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Registration (Name, Username, Password, MobileNumber) " +
                               "VALUES (@Name, @Username, @Password, @MobileNumber)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@MobileNumber", mobileNumber);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.WriteLine("Registration Successful!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during registration: " + ex.Message);
                }
            }
        }

        public void DisplayProducts()
        {
            Console.WriteLine("Available Products:");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductID, ProductName, Price, Qty FROM Product";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    using (SqlDataReader sdr = command.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            Console.WriteLine($"{sdr["ProductID"]} - {sdr["ProductName"]} - {sdr["Price"]} - Qty: {sdr["Qty"]}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error fetching products: " + ex.Message);
                }
            }
        }

        public void AddToCart(string username)
        {
            int totalCost = 0;

            while (true)
            {
                Console.Write("Enter ProductID to add to cart (or 'exit' to stop): ");
                string productId = Console.ReadLine();

                if (productId.ToLower() == "exit") break; // Allow user to exit

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT ProductName, Price, Qty FROM Product WHERE ProductID = @ProductID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductID", productId);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string productName = reader["ProductName"].ToString();
                                int price = Convert.ToInt32(reader["Price"]);
                                int qty = Convert.ToInt32(reader["Qty"]);

                                if (qty > 0)
                                {
                                    totalCost += price;
                                    Console.WriteLine($"{productName} added to cart. Price: {price}");

                                    // Close the reader before running any other commands
                                    reader.Close();

                                    // Update stock
                                    string updateQuery = "UPDATE Product SET Qty = Qty - 1 WHERE ProductID = @ProductID";
                                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                                    updateCommand.Parameters.AddWithValue("@ProductID", productId);
                                    updateCommand.ExecuteNonQuery();

                                    // Add to cart
                                    string insertQuery = "INSERT INTO Cart (ProductID, ProductName, Username, FinalPrice) " +
                                                         "VALUES (@ProductID, @ProductName, @Username, @FinalPrice)";
                                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                                    insertCommand.Parameters.AddWithValue("@ProductID", productId);
                                    insertCommand.Parameters.AddWithValue("@ProductName", productName);
                                    insertCommand.Parameters.AddWithValue("@Username", username);
                                    insertCommand.Parameters.AddWithValue("@FinalPrice", price);
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
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during cart operation: " + ex.Message);
                    }
                }
            }
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
