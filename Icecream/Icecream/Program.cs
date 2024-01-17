using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Formats.Asn1;
using System.Text.RegularExpressions;

namespace Icecream
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Queue<Order> orderQueue = new Queue<Order>();
            Queue<Order> goldOrderQueue = new Queue<Order>();
            Customer? chosenCustomer;
            
            // Option 1:
            //AllCustomersInfo();
            
            
            //RegisterCustomer();
            AddIceCreamToOrder();
            

            // Start of the Basic Features  
            // 1) List all customers (Done)
            void AllCustomersInfo()
            {
                // Header
                Console.WriteLine("All Members Information\n----------------------");
                Console.WriteLine($"{"Name",-10}{"MemberId",-10}DOB");
                foreach (string[] elements in ReadFile("customers.csv"))
                {
                    Console.WriteLine($"{elements[0],-10}{elements[1],-10}{elements[2]}");
                }
            }

            // 2) List all current orders
            void AllCurrentOrders()
            {
            }

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

                        // Check if integers are in user input
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
                        foreach (string[] lines in ReadFile("customers.csv"))
                        {
                            if (lines[1] == Convert.ToString(id))
                            {
                                throw new Exception("ID already exist, try again.");
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
                    sw.WriteLine($"{name},{id},{dob.ToShortDateString()},{newCustomer.Rewards.Tier},{newCustomer.Rewards.Points},{newCustomer.Rewards.PunchCard}");
                }

                Console.WriteLine("You have registered an account.");


            }


            // 4) Create a customer's order
            Order CreateOrder()
            {
                // Display all customer info
                AllCustomersInfo();

                // Choose which customer to add order
                chosenCustomer = ChooseCustomer();

                // Create the order for the customer
                return chosenCustomer?.MakeOrder();
            }

            // 4) Create the IceCream order
            IceCream CreateIceCream()
            {
                string options = CheckUserInput(new List<string>() { "cup", "cone", "waffle" }, "Enter your options: ");
                int scoops = checkIntInput("Enter number of scoops(1-3): ", 1, 3);

                List<Flavour> flavourList = GetFlavours(scoops);
                List<Topping> toppingList = GetToppings();
                
                bool dipppedChocolate = options == "cone" && checkYesNoInput("Do you want dipped chocolate(y/n): ");
                string waffleFlavour = options == "waffle" ? CheckUserInput(new List<string>() { "original", "red velvet", "charcoal", "pandan" }, "Choose a waffle flavour: ") : "";

                return (options switch
                {
                    "cone" => new Cone(options,scoops,flavourList,toppingList,dipppedChocolate),
                    "cup" => new Cup(options, scoops, flavourList, toppingList),
                    "waffle" => new Waffle(options, scoops, flavourList, toppingList, waffleFlavour),
                    _ => null
                })!;
            }
            
            // Get the flavour for the scoops that the user wants
            List<Flavour> GetFlavours(int scoops)
            {
                List<string> regularFlavours = new List<string>() { "vanilla", "chocolate", "strawberry" };
                List<string> premiumFlavours = new List<string>() { "durian", "ube", "sea salt" };

                List<Flavour> flavours = new List<Flavour>();

                for (int i = 0; i < scoops; i++)
                {
                    string flavourChoice = CheckUserInput(regularFlavours.Concat(premiumFlavours).ToList(), $"Choose flavour for scoop {i + 1}: ");
                    bool isPremium = premiumFlavours.Contains(flavourChoice);
                    Flavour flavour = new Flavour(flavourChoice, isPremium, 1); // Assuming 1 quantity per scoop
                    flavours.Add(flavour);
                }

                return flavours;
            }
            
            // Get the topping that the user wants
            List<Topping> GetToppings()
            {
                List<string> toppingMenu = new List<string>() { "sprinkles", "mochi", "sago", "oreos" };
                List<Topping> toppings = new List<Topping>();

                int numToppings = checkIntInput("Enter number of toppings (0-4): ", 0, 4);
                for (int i = 0; i < numToppings; i++)
                {
                    string toppingChoice = CheckUserInput(toppingMenu, $"Choose topping {i + 1}: ");
                    Topping topping = new Topping(toppingChoice);
                    toppings.Add(topping);
                }

                return toppings;
            }
            
            // 4) Add IceCream into customer's order
            void AddIceCreamToOrder()
            {
                Order ordered = CreateOrder();
                IceCream iceCream = CreateIceCream();

                ordered.AddIceCream(iceCream);

                while (true)
                {

                    bool anotherO = checkYesNoInput("Would you like to add another ice cream to the order?(y/n)");
                    if (anotherO)
                    {
                        iceCream = CreateIceCream();

                        ordered.AddIceCream(iceCream);
                    }

                    break;
                }

                chosenCustomer!.CurrentOrder = ordered;
                chosenCustomer.OrderHistory.Add(ordered);
                foreach (string[] lines in ReadFile("customers.csv"))
                {
                    if (Convert.ToString(chosenCustomer.Memberid) != lines[0]) continue;
                    if (lines[3] == "Gold")
                    {
                        goldOrderQueue.Enqueue(chosenCustomer.CurrentOrder);
                        break;
                    }
                    
                    orderQueue.Enqueue(ordered);
                }
                Console.WriteLine(chosenCustomer);
            }


            // 5) Display order details of a customer
            void OrderDetails()
            {
            }

            
            
            // Additional Methods
            // Choose which customer to append data inside.
            Customer? ChooseCustomer()
            {
                string? id = "";
                while (true)
                {
                    // Data validation for customer id
                    try
                    {
                        Console.Write("Enter customer id: ");
                        id = Console.ReadLine();

                        // Open file to find customers
                        bool idExist = false;
                        foreach (string[] lines in ReadFile("customers.csv"))
                        {
                            if (lines[1] == Convert.ToString(id))
                            {
                                idExist = true;
                                Customer tempCustomer = new Customer(lines[0], Convert.ToInt32(lines[1]), DateTime.Parse(lines[2]));
                                PointCard pointCard = new PointCard(Convert.ToInt32(lines[4]), Convert.ToInt32(lines[5]));
                                pointCard.Tier = lines[3];
                                tempCustomer.Rewards = pointCard;
                                return tempCustomer;
                            }
                        }

                        // Not existing, it will give user an error
                        if (!idExist)
                        {
                            throw new Exception("Customer entered does not valid.");
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                
                

                return null;
            }

            // Editing the customer.csv data
            void EditCustomerFile(Customer c)
            {
                // Generates a temporary file
                string tempFile = Path.GetTempFileName();

                // Read the pointcard.csv and append all the lines is not changed to the temp file
                using (var sr = new StreamReader("customers.csv"))
                using (StreamWriter sw = File.AppendText(tempFile))
                {
                    string s;

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] lines = s.Split(',');
                        // Check which user's data to be changed
                        if (lines[0] != Convert.ToString(c.Memberid))
                        {
                            sw.WriteLine(s);
                        }
                    }

                    sw.WriteLine($"{c.Name},{c.Memberid},{c.Dob},{c.Rewards.Tier},{c.Rewards.Points},{c.Rewards.PunchCard}");
                }

                File.Delete("customers.csv");
                File.Move(tempFile, "customers.csv");
            }
            
            IEnumerable<string[]> ReadFile(string filename)
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string? s = sr.ReadLine();

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] lines = s.Split(',');

                        yield return lines;
                    }
                }
            }
            
            // Check if user input is an existing item on the list (For Option 4)
            string CheckUserInput(List<string> menu, string prompt)
            {
                while (true)
                {
                    try
                    {
                        Console.WriteLine(string.Join(" | ", menu));
                        Console.Write(prompt);
                        string? choice = Console.ReadLine()?.ToLower();
                        if (!menu.Contains(choice))
                        {
                            throw new Exception("Invalid Choice, Please try again");
                        }

                        return choice;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            
            // Check if the user input only contains integer (For option 4)
            int checkIntInput(string prompt, int min, int max)
            {
                while (true)
                {
                    try
                    {
                        Console.Write(prompt);
                        if (!int.TryParse(Console.ReadLine(), out int result) || result < min || result > max)
                        {
                            throw new Exception($"Please enter a number between {min} to {max}.");
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            
            // Check if user input is y(yes) or n(no)
            bool checkYesNoInput(string prompt)
            {
                while (true)
                {
                    Console.Write(prompt);
                    string? input = Console.ReadLine()?.ToLower();
                    if (input == "y") return true;
                    if (input == "n") return false;
                    Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                }
            }
        }
    }
}