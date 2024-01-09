using System.Collections;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Icecream
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Queue<Order> queueOrder = new Queue<Order>();
            Queue<Customer> customerQueue = new Queue<Customer>();
            
            AllCustomersInfo();
            RegisterCustomer();

            Random random = new Random();
            
            // Editing the Point Card File
            // Write new data to pointcard.csv
            void WriteToPointCard(int id, int points, string tier, int punchcard)
            {
                using (StreamWriter sw = File.AppendText("pointcard.csv"))
                {

                    sw.WriteLine($"{id},{points},{tier},{punchcard}");
                }
            }
            
            // Editing the pointcard.csv data
            void EditPointCard(int id, int points, string tier, int punchcard)
            {
                // Generates a temporary file
                string tempFile = Path.GetTempFileName();
                
                // Read the pointcard.csv and append all the lines is not changed to the temp file
                using(var sr = new StreamReader("pointcard.csv"))
                using(StreamWriter sw = File.AppendText(tempFile))
                {
                    string s;

                    while ((s=sr.ReadLine()) != null)
                    {
                        string[] lines = s.Split(',');
                        // Check which user's data to be changed
                        if (lines[0] != Convert.ToString(id))
                        {
                            sw.WriteLine(s);
                        }
                    }
                    
                    sw.WriteLine($"{id},{points},{tier},{punchcard}");
                }

                File.Delete("pointcard.csv");
                File.Move(tempFile, "pointcard.csv");
            }

            
            
            
            
            // Start of the Basic Features
            // 1) List all customers (Done)
            void AllCustomersInfo()
            {
                using (StreamReader sr = new StreamReader("customers.csv"))
                {
                    string? s = sr.ReadLine();
                    if (s != null)
                    {
                        string[] heading = s.Split(',');
                        Console.WriteLine("{0,-10}{1,-10}{2}", heading[0], heading[1], heading[2]);
                    }

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] lines = s.Split(',');
                        Console.WriteLine($"{lines[0],-10}{lines[1],-10}{lines[2],-10}");
                    }
                }
            }

            // 2) List all current orders
            void AllCurrentOrders(){}
            
            // 3) Register a new Customer
            void RegisterCustomer()
            {
                string? name = "";
                int id = 0;
                DateTime dob;
                
                // Data validation for Name
                while (true)
                {
                    try
                    {
                        Console.Write("Enter your Name: ");
                        name = Console.ReadLine();
                        
                        // Check if integers are in userinput
                        Regex numbersRegex = new Regex(@"\d");
                        if (numbersRegex.IsMatch(name))
                        {
                            throw new Exception("You cannot enter a number for your name.");
                        }
                        break;

                    }
                
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                
                // Data Validation for ID
                while (true)
                {
                    try
                    {
                        Console.Write("Enter your ID: ");
                        id = Convert.ToInt32(Console.ReadLine());
                        
                        // Check if letters are in user input
                        Regex lettersRegex = new Regex(@"[a-zA-Z]");
                        if (lettersRegex.IsMatch(Convert.ToString(id)))
                        {
                            throw new FormatException("You only can enter numbers for id");
                        }
                        
                        // To check if ID is already existing by comparing to the file.
                        using (StreamReader sr = new StreamReader("customers.csv"))
                        {
                            string? s = sr.ReadLine();

                            while ((s = sr.ReadLine()) != null)
                            {
                                string[] lines = s.Split(',');
                                if (lines[1] == Convert.ToString(id))
                                {
                                    throw new Exception("ID already exist, try again.");
                                }
                            }
                        }

                        break;
                    }

                    catch (FormatException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                
                // Data validation for date
                while (true)
                {
                    try
                    {
                        Console.Write("Enter your Date of birth(MMM,DD,YYYY): ");
                        string? stringDob = Console.ReadLine();
                        dob = DateTime.Parse(stringDob);
                        
                        // Ensure that birth date valid, and not after today's date
                        if (dob > DateTime.Now)
                        {
                            throw new Exception("Please enter a proper date of birth");
                        }

                        break;
                    }
                    // Check if they enter valid date
                    catch (FormatException)
                    {
                        Console.WriteLine("Enter a valid date");
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                

                Customer newCustomer = new Customer(name, id, dob);

                PointCard newPc = new PointCard(0, 0);
                Console.WriteLine(newPc.Tier);
                newCustomer.Rewards = newPc;
                
                // Append information into customer.csv
                using (StreamWriter sw = File.AppendText("customers.csv"))
                {
                    sw.WriteLine($"{name},{id},{dob.ToShortDateString()}");
                }
                
                using (StreamWriter sw = File.AppendText("pointcard.csv"))
                {
                    sw.WriteLine($"{newCustomer.Memberid},{newCustomer.Rewards.Points},{newCustomer.Rewards.Tier},{newCustomer.Rewards.PunchCard}");
                }
                
                Console.WriteLine("You have registered an account already");


            }
            
            // 5) Display order details of a customer
            void OrderDetails(){}
        }
    }
}