using System.Collections;
using System.Drawing;
using System.Formats.Asn1;
using System.Text.RegularExpressions;

namespace Icecream
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Queue<Order> queueOrder = new Queue<Order>();
            Queue<Customer> goldQueue = new Queue<Customer>();

            //AllCustomersInfo();
            //RegisterCustomer();
            CreateOrder();
            CreateIceCream();


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
                using (var sr = new StreamReader("pointcard.csv"))
                using (StreamWriter sw = File.AppendText(tempFile))
                {
                    string s;

                    while ((s = sr.ReadLine()) != null)
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
                    sw.WriteLine(
                        $"{newCustomer.Memberid},{newCustomer.Rewards.Points},{newCustomer.Rewards.Tier},{newCustomer.Rewards.PunchCard}");
                }

                Console.WriteLine("You have registered an account already");


            }

            
            // 4) Create a customer's order
            Order CreateOrder()
            {
                // Display all customer info
                AllCustomersInfo();

                while (true)
                {
                    // Data validation for customer id
                    try
                    {
                        Console.Write("Enter customer id: ");
                        string? id = Console.ReadLine();

                        // Open file to find customers
                        bool idExist = false;
                        using (StreamReader sr = new StreamReader("customers.csv"))
                        {
                            string? s = sr.ReadLine();

                            while ((s = sr.ReadLine()) != null)
                            {
                                string[] lines = s.Split(',');
                                // Check if the id is existing
                                if (lines[1] == Convert.ToString(id))
                                {
                                    idExist = true;
                                    break;
                                }
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

                // Create new Order
                Random random = new Random();
                int randomId = random.Next();
                // I need to check if the id is already an existing order
                return (new Order(randomId, DateTime.Now));
            }

            // 4) Create the IceCream order
            IceCream CreateIceCream()
            {
                // Create new Flavours
                string? options = "";
                int scoops = 0;
                List<Flavour> flavourList = new List<Flavour>();
                List<Topping> toppingList = new List<Topping>();
                bool dippedChocolate = false;
                string? waffleFlavour = "";

                List<string> optionMenu = new List<string>() { "cup", "cone", "waffle" };
                List<string> waffleMenu = new List<string>() { "original", "red velvet", "charcoal", "pandan waffle" };
                while (true)
                {
                    // Data validation for options
                    try
                    {
                        Console.WriteLine($"Options: {string.Join(" | ", optionMenu)}");
                        Console.Write("Enter your options: ");
                        options = Console.ReadLine()?.ToLower();

                        // Check if the input is any of the provided option
                        if (!optionMenu.Contains(options))
                        {
                            throw new Exception("Please enter a valid option.");
                        }

                        // If Option is cone, Dipped chocolate is an option, so we need to ask the user if they want it or not
                        if (options == "cone")
                        {
                            Console.Write("Do you want dipped Chocolate(y/n): ");
                            string? chocolate = Console.ReadLine()?.ToLower();

                            if (chocolate != "y" && chocolate != "n")
                            {
                                throw new Exception("Please enter y(yes) or n(no)");
                            }

                            if (chocolate == "y")
                            {
                                dippedChocolate = true;
                            }
                        }

                        else if (options == "waffle")
                        {
                            Console.WriteLine($"Waffle Flavour: {string.Join(" | ", waffleMenu)}");
                            Console.Write("Choose a waffle flavour: ");
                            waffleFlavour = Console.ReadLine()?.ToLower();

                            if (!waffleMenu.Contains(waffleFlavour))
                            {
                                throw new Exception("Please choose a waffle flavour from the menu");
                            }

                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                while (true)
                {
                    // Data validation for scoops
                    try
                    {
                        Console.Write("Enter number of scoops(1-3): ");
                        scoops = Convert.ToInt32(Console.ReadLine());

                        // This is check if they put 1-3 scoops or not
                        if (scoops > 3 || scoops < 1)
                        {
                            throw new Exception("You can only choose 1 to 3 scoops.");
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                // Variables to use
                List<string> regularFlavourMenu = new List<string>() { "vanilla", "chocolate", "strawberry" };
                List<string> premiumFlavourMenu = new List<string>() { "durian", "ube", "sea salt" };
                int quantity = 0;
                string flavour = "";
                int totalQuantity = 0;
                // Data validation for Flavours
                Console.WriteLine(
                    $"Regular Flavours: {string.Join(", ", regularFlavourMenu)}  \nPremium Flavours: {string.Join(" | ", premiumFlavourMenu)}");
                while (true)
                {
                    try
                    {
                        for (int i = 0; i < scoops; i++)
                        {

                            Console.Write($"Choose flavours {i + 1}: ");
                            flavour = Console.ReadLine().ToLower();
                            if (!regularFlavourMenu.Contains(flavour) &&
                                !premiumFlavourMenu.Contains(flavour))
                            {
                                throw new Exception("Choose the existing flavours.");
                            }


                            Console.Write("Enter the amount: ");
                            quantity = Convert.ToInt32(Console.ReadLine());

                            Regex letterRegex = new Regex(@"[a-zA-Z]");
                            if (letterRegex.IsMatch(Convert.ToString(quantity)))
                            {
                                throw new Exception("It must be a integer.");
                            }

                            totalQuantity += quantity;
                            if (quantity > scoops || totalQuantity > scoops)
                            {
                                totalQuantity -= quantity;
                                throw new Exception(
                                    $"You only have {scoops} scoops. Please do not enter more than that");
                            }

                            // Create new flavour class
                            Flavour newFlavour = new Flavour(flavour,
                                premiumFlavourMenu.Contains(flavour.ToLower()),
                                quantity);
                            flavourList.Add(newFlavour);

                            if (quantity == scoops || scoops == totalQuantity)
                            {
                                break;
                            }
                        }

                        break;

                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }



                List<string> toppingMenu = new List<string>() { "sprinkles", "mochi", "sago", "oreos" };
                while (true)
                {
                    try
                    {
                        Console.Write("Number of toppings(0-4): ");
                        int numberToppings = Convert.ToInt32(Console.ReadLine());

                        if (numberToppings is > 4 or < 0)
                        {
                            throw new Exception("Only 0 to 4 toppings are allowed");
                        }


                        for (int i = 0; i < numberToppings; i++)
                        {
                            Console.WriteLine($"Toppings: {string.Join(", ", toppingMenu)}");
                            Console.Write($"Topping {i + 1}: ");
                            string? topping = Console.ReadLine()?.ToLower();

                            if (!toppingMenu.Contains(topping))
                            {
                                throw new Exception("Please enter a topping in the menu");
                            }

                            toppingMenu.Remove(topping);
                            Topping newTopping = new Topping(topping);
                            toppingList.Add(newTopping);

                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                // Create the IceCream
                if (options == "cone")
                {
                    return(new Cone(options, scoops, flavourList, toppingList, dippedChocolate));
                }

                else if (options == "cup")
                {
                    return(new Cup(options, scoops, flavourList, toppingList));
                }

                return(new Waffle(options, scoops, flavourList, toppingList, waffleFlavour));

            }
            
            // 4) Add IceCream into customer's order
            void AddIceCreamToOrder()
            {
                Order ordered = CreateOrder();
                IceCream iceCream = CreateIceCream();
                
                ordered.AddIceCream(iceCream);

                string anotherOrder = "";
                while (true)
                {
                    try
                    {
                        Console.Write("Would you like to add another ice cream to the order? (Y/N)");
                        anotherOrder = Console.ReadLine()?.ToLower();

                        if (anotherOrder != "y" || anotherOrder != "n")
                        {
                            throw new Exception("Please enter yes[Y] and no[N]");
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                while (true)
                {
                    if (anotherOrder == "y")
                    {
                        ordered = CreateOrder();
                        iceCream = CreateIceCream();
                
                        ordered.AddIceCream(iceCream);
                    }   
                }
            }
            

            // 5) Display order details of a customer
                void OrderDetails()
                {
                }
        }
    }
}