using System;
using System.Data.SqlClient;
using System.Configuration;

namespace ShoppingCart4
{
    class ShoppingCart4
    {
        string connectionString = ConfigurationManager.ConnectionStrings["Conn"].ConnectionString;

        public void RegisterUser()
        {
            try
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
                    SqlCommand command = new SqlCommand("RegisterUser", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@MobileNumber", mobileNumber);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                Console.WriteLine("Registration Successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public void DisplayProducts()
        {
            try
            {
                Console.WriteLine("Available Products:");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("GetProducts", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine(reader["ProductID"] + " - " +
                                          reader["ProductName"] + " - " +
                                          reader["Price"] + " - Qty: " +
                                          reader["Qty"]);
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public void AddToCart(string username)
        {
            try
            {
                int totalCost = 0;

                while (true)
                {
                    Console.Write("Enter ProductID to add to cart: ");
                    string productId = Console.ReadLine();

                    Console.Write("How much Quantity You want to add to cart: ");
                    int qty = Convert.ToInt32(Console.ReadLine());

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand("SELECT ProductName, Price, Qty FROM Product WHERE ProductID = @ProductID", connection);
                        command.Parameters.AddWithValue("@ProductID", productId);

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            string productName = reader["ProductName"].ToString();
                            int price = Convert.ToInt32(reader["Price"]);
                            int stockQty = Convert.ToInt32(reader["Qty"]);

                            if (stockQty >= qty)
                            {
                                totalCost += price * qty;

                                Console.WriteLine($"{productName} added to cart. Price per unit: {price}, Total Price for {qty} unit(s): {price * qty}");

                                reader.Close();

                                SqlCommand insertCommand = new SqlCommand("AddToCart", connection);
                                insertCommand.CommandType = System.Data.CommandType.StoredProcedure;

                                insertCommand.Parameters.AddWithValue("@ProductID", productId);
                                insertCommand.Parameters.AddWithValue("@Username", username);
                                insertCommand.Parameters.AddWithValue("@ProductName", productName);
                                insertCommand.Parameters.AddWithValue("@Price", price);
                                insertCommand.Parameters.AddWithValue("@Qty", qty);

                                insertCommand.ExecuteNonQuery();
                            }
                            else
                            {
                                Console.WriteLine("Not enough stock available.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Product not found.");
                        }

                        connection.Close();
                    }

                    Console.Write("Do you want to add another product? (yes/no): ");
                    string answer = Console.ReadLine();
                    if (answer.ToLower() != "yes")
                    {
                        break;
                    }
                }

                Console.WriteLine("Total cost: " + totalCost);
                DisplayPayment();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public void DisplayPayment()
        {
            try
            {
                Console.WriteLine("Which Mode Do You Need To Make Payment?");
                Console.WriteLine("1. Cash On Delivery\n2. Card");
                int opt = Convert.ToInt32(Console.ReadLine());
                if (opt == 2)
                {
                    Console.WriteLine("Enter Card Number:");
                    string cardNumber = Console.ReadLine();
                    Console.WriteLine("Enter CVV:");
                    string cvv = Console.ReadLine();
                    Console.WriteLine("Online Payment is Successful!!");
                }
                else
                {
                    Console.WriteLine("Cash on Delivery is Successful!!");
                }
                Console.WriteLine("Order is Successfully Placed!\nOrder will be Delivered in 2 Days!!");
                Console.WriteLine("Thanks for shopping with us.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
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
            Console.WriteLine("Enter Password:");
            string passkey = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("CheckUserLogin", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", passkey);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int loginStatus = Convert.ToInt32(reader["LoginStatus"]);
                    if (loginStatus == 1)
                    {
                        Console.WriteLine("Login Successful!");
                        connection.Close();
                        Console.WriteLine("Welcome To Amazon....!");

                        Console.WriteLine("Do you want to see Product List? (yes/no)");
                        string option = Console.ReadLine();
                        if (option.ToLower() == "yes")
                        {
                            DisplayProducts();
                            AddToCart(username);
                        }
                        else
                        {
                            Console.WriteLine("Thank you! Please login again for shopping.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect Username or Password. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Error during login check.");
                }

                connection.Close();
            }
        }
    }
}
