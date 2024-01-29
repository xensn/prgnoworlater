﻿using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Formats.Asn1;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace Icecream
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            /*// list for the customer objects from the .csv file 
            List<Customer> customers = new List<Customer>();
            // creating the customer objects from the .csv file 
            CreateCustomerObjects("customers.csv", customers);
            
            // list for the previous orders from the .csv file 
            List<Order> orders = new List<Order>();*/
            
            Queue<Order> orderQueue = new Queue<Order>();
            Queue<Order> goldOrderQueue = new Queue<Order>();
            Dictionary<int, Order> customerOrder = new Dictionary<int, Order>();
            Customer? chosenCustomer;
            
                // Select them option
                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine(" ---------------------------------------------------");
                    Console.WriteLine(" |            Ice Cream Shop Option Menu           |");
                    Console.WriteLine(" ---------------------------------------------------");
                    Console.WriteLine(" | [1] Display all customers information           |");
                    Console.WriteLine(" | [2] Display regular and gold member queue       |");
                    Console.WriteLine(" | [3] Register a new customer                     |");
                    Console.WriteLine(" | [4] Create a new order                          |");
                    Console.WriteLine(" | [5] Display order details of a customer         |");
                    Console.WriteLine(" | [6] Modify Order Details                        |");
                    Console.WriteLine(" | [7] Process an order and checkout               |");
                    Console.WriteLine(" | [8] Display Month & Yearly total charged amount |");
                    Console.WriteLine(" | [0] Exit                                        |");
                    Console.WriteLine(" ---------------------------------------------------");
                    int options = CheckIntInput("Enter an option: ", 0, 8);
                    Console.WriteLine();
                    
                    if (options == 1)
                    {
                        AllCustomersInfo();
                    }
                    
                    else if (options == 2)
                    {
                        AllCurrentOrders();
                    }
                    
                    else if (options == 3)
                    {
                        RegisterCustomer();
                    }
                    
                    else if (options == 4)
                    {
                        AddIceCreamToOrder();
                    }
                    
                    else if (options == 5)
                    {
                        OrderDetails();
                    }
                    
                    else if (options == 6)
                    {
                        ModifyOrderDetails();
                    }
                    
                    else if (options == 7)
                    {
                        
                    }
                    
                    else if (options == 8)
                    {
                        TotalPriceBreakdown();
                    }

                    else
                    {
                        break;
                    }
                }
            

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
                // print out the current orders in the normal order queue
                Console.WriteLine("Orders from Normal Order Queue" +
                                  "\n-------------------------------------------------------------------------------------------------");
                ListOrder(orderQueue);
                Console.WriteLine("-------------------------------------------------------------------------------------------------");
                // print out the current orders in the gold queue
                Console.WriteLine("Orders from Gold Order Queue" +
                                  "\n-------------------------------------------------------------------------------------------------");
                ListOrder(goldOrderQueue);

                Console.WriteLine("-------------------------------------------------------------------------------------------------");
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
                chosenCustomer = ChooseCustomer(null);

                // Create the order for the customer
                return chosenCustomer?.MakeOrder();
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
                    if (Convert.ToString(chosenCustomer.Memberid) != lines[1]) continue;
                    
                    if (lines[3] == "Gold")
                    {
                        goldOrderQueue.Enqueue(chosenCustomer.CurrentOrder);
                        break;
                    }
                    
                    orderQueue.Enqueue(chosenCustomer.CurrentOrder);
                    customerOrder.Add(chosenCustomer.Memberid, ordered);
                }
            }


            // 5) Display order details of a customer
            void OrderDetails()
            {
                // List the customers 
                AllCustomersInfo();

                // Prompt user to select a customer and retrieve the selected customer 
                Customer chosencustomer = ChooseCustomer(null);

                // Retrieve all the order objects of the customer, pass and current
                Console.WriteLine("Current Orders" +
                                  "\n-------------------------------------------------------------------------------------------------");
                Console.WriteLine(chosencustomer.CurrentOrder.ToString());
                Console.WriteLine(
                    "-------------------------------------------------------------------------------------------------");

                // For each order, display all the details of the order including datetime recieved, datetime fulfilled(if applicable) and all icecream details associated with the order
                Console.WriteLine("Past Orders" +
                                  "\n-------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("{0, -9} {1, -17} {2, -17} {3, -7} {4,-7} {5, -7} {6,-15} {7, -11} {8, -11} {9,-11} {10, -10} {11, -10} {12, -10} {13, -10}",
                    "Order Id", "Time Received", "Time Fulfilled", "Option", "Scoops", "Dipped", "Waffle Flavour", "Flavour1", "Flavour2", "Flavour3", "Toppings1", "Toppings2", "Toppings3", "Toppings4");
                foreach (string[] elements in ReadFile("orders.csv"))
                {
                    if (chosencustomer.Memberid == Convert.ToInt32(elements[1]))
                    {
                        Console.WriteLine("{0, -9} {1, -17} {2, -17} {3, -7} {4,-7} {5, -7} {6,-15} {7, -11} {8, -11} {9,-11} {10, -10} {11, -10} {12, -10} {13, -10}",
                            elements[0] , elements[2] , elements[3], elements[4], elements[5], elements[6], elements[7], elements[8], elements[9], elements[10], elements[11], elements[12], elements[13], elements[14]);
                    }
                }
                ListOrder(chosencustomer.OrderHistory);
                Console.WriteLine(
                    "---------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            }

            // 6) Modify order details
            void ModifyOrderDetails()
            {
                Order chosenOrder = null;
                // List the customers
                AllCustomersInfo();
                // Prompt the user to select a customer and retrieve the selected customer's current order 
                Customer? chosencustomer = ChooseCustomer(null);

                foreach (KeyValuePair<int, Order> kvp in customerOrder)
                {
                    if (chosencustomer.Memberid == kvp.Key)
                    {
                        chosenOrder = kvp.Value;
                    }
                }
                
                // input validation for the options
                bool whileloop = true;
                while (whileloop)
                {
                    // List all the ice cream objects contained in the order 
                    Console.WriteLine("Current Orders" +
                                      "\n-------------------------------------------------------------------------------------------------");
                    List<IceCream> chosenicecreamlist = chosenOrder.IceCreamList;
                    foreach (IceCream icecream in chosenicecreamlist)
                    {
                        Console.WriteLine($"{chosenicecreamlist.IndexOf(icecream) + 1}. {icecream.ToString()}");
                    }
                    Console.WriteLine(
                        "-------------------------------------------------------------------------------------------------");
                    try
                    {
                        int opt = CheckIntInput("Option 1 - Choose an existing ice cream to modify" +
                                                "\nOption 2 - Add an entirely new ice cream to your order" +
                                                "\nOption 3 - Choose an existing ice cream to delete from your order" +
                                                "\nOption 4 - Exit" +
                                                "\nPlease select an option: ", 1, 4);
                        switch (opt)
                        {
                            case 1:
                                chosenOrder.ModifyIceCream();
                                break;
                            case 2:
                                IceCream icecream = CreateIceCream();
                                chosenOrder.AddIceCream(icecream);
                                break;
                            case 3:
                                chosenOrder.DeleteIceCream();
                                break;
                            case 4:
                                whileloop = false;
                                break;
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    // Option 1 - Choose an existing ice cream to modify
                    // Let the user select which ice cream to modify 
                    // Prompt the new information for the modifications they wish to make 
                    // Option 2 - Add on entirely new ice cream to the order 
                    // Create a new ice cream and add it to the order
                    // Option 3 - Choose an existing ice cream to delete from the order 
                    // Select the ice cream they want to remove but if there is only one in the order then got to display a message 
                }

            }
            
            // Advanced Feature (a)
            void Payment()
            {
                Order chosenOrder;
                Order dequeueOrder;
                
                // Check if there are any any gold queue members.
                if (goldOrderQueue.Count > 0)
                {
                    // Dequeue and store the order we are handling in dequeueOrder
                    dequeueOrder = goldOrderQueue.Dequeue();
                    
                    // Find the correct order and the customer who ordered in the store dictionary.
                    foreach (KeyValuePair<int, Order> kvp in customerOrder)
                    {
                        if (dequeueOrder.Id == kvp.Value.Id)
                        {
                            chosenOrder = kvp.Value;
                            chosenCustomer = ChooseCustomer(Convert.ToString(kvp.Key));
                            break;
                        }
                    }
                }

                else
                {
                    dequeueOrder = orderQueue.Dequeue();
                }
                
                // Have fun dhava
            }
            
            
            
            // Advanced Feature (b)
            void TotalPriceBreakdown()
            {
                double total = 0;
                // Prompt user for the year.
                int year = CheckIntInput("Enter the year: ", 1, DateTime.Now.Year);
                Console.WriteLine();
                
                // List of Months and keep track of the total of each month
                List<string> monthList = new List<string>(){"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};
                List<double> monthTotalList = new List<double>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                
                // Read File
                foreach (string[] elements in ReadFile("orders.csv"))
                {
                    // Find the date of the line
                    DateTime dateFulfilled = DateTime.Parse(elements[3]);
                    
                    // Check if Year matches
                    if (dateFulfilled.Year == year)
                    {
                        // Convert the Line of text to IceCream 
                        IceCream tempIceCream = ConvertOrderCsv(elements);
                        
                        // Loop through 
                        for (int i = 0; i < 12; i++)
                        {
                            if (dateFulfilled.Month == i+1)
                            {
                                monthTotalList[i] += tempIceCream.CalculatePrice();
                            }
                        }
                        total += tempIceCream.CalculatePrice();
                    }
                }
                
                for (int i = 0; i < 12; i++)
                {
                    Console.WriteLine($"{monthList[i]} {year}:   ${monthTotalList[i]}");
                }
                Console.WriteLine();
                Console.WriteLine($"Total:     ${total}");
            }

            // Additional Methods
            // Choose which customer to append data inside.
            Customer? ChooseCustomer(string id)
            {
                if (id != null)
                {
                    foreach (string[] lines in ReadFile("customers.csv"))
                    {
                        if (lines[1] == Convert.ToString(id))
                        {
                            Customer tempCustomer = new Customer(lines[0], Convert.ToInt32(lines[1]), DateTime.Parse(lines[2]));
                            PointCard pointCard = new PointCard(Convert.ToInt32(lines[4]), Convert.ToInt32(lines[5]));
                            pointCard.Tier = lines[3];
                            tempCustomer.Rewards = pointCard;
                            return tempCustomer;
                        }
                    }
                }
                
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
            
            // Listing the orders in both queue and list
            void ListOrder(IEnumerable<Order> orderqueue)
            {
                foreach (Order order in orderqueue)
                {
                    Console.WriteLine(order.ToString());
                }
            }

            IceCream ConvertOrderCsv(string[] line)
            {
                bool isPremium = false;
                List<Flavour> flavourList = new List<Flavour>();
                List<Topping> toppingList = new List<Topping>();
                // Create Flavour Object
                for (int i = 8; i < 11; i++)
                {
                    if (line[i] == "Ube" || line[i] == "Sea Salt" || line[i] == "Durian")
                    {
                        isPremium = true;
                    }
                    
                    if (line[i] != null)
                    {
                        Flavour tempFlavour = new Flavour(line[i], isPremium, 1);
                        flavourList.Add(tempFlavour);
                    }

                }
                
                // Create Topping Object
                for (int i = 11; i < 15; i++)
                {
                    if (line[i] != null)
                    {
                        Topping tempTopping = new Topping(line[i]);
                        toppingList.Add(tempTopping);
                    }
                }
                
                if (line[4] == "Cup")
                {
                    IceCream tempIceCream = new Cup(line[4], Convert.ToInt32(line[5]), flavourList, toppingList);
                    return tempIceCream;
                }
                
                else if (line[4] == "Cone")
                {
                    bool.TryParse(line[6], out bool result);
                    IceCream tempIceCream = new Cone(line[4], Convert.ToInt32(line[5]), flavourList, toppingList, result);
                    return tempIceCream;
                }
                
                else if (line[4] == "Waffle")
                {
                    Waffle tempIceCream = new Waffle(line[4],Convert.ToInt32(line[5]), flavourList, toppingList, line[7]);
                    return tempIceCream;
                }

                return null;
            }

            /*void CreateCustomerObjects(string filename, List<Customer> customers)
            {
                foreach (string[] elements in ReadFile("customers.csv"))
                {
                    Customer customer = new Customer(elements[0], Convert.ToInt32(elements[1]), Convert.ToDateTime(elements[2]));
                    customer.Rewards.Points = Convert.ToInt32(elements[4]);
                    
                    customers.Add(customer);
                }
            }*/
        }
        
         // Public static methods 
         // 4) Create the IceCream order
            public static IceCream CreateIceCream()
            {
                string options = CheckUserInput(new List<string>() { "cup", "cone", "waffle" }, "Enter your options: ");
                int scoops = CheckIntInput("Enter number of scoops(1-3): ", 1, 3);

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
            public static List<Flavour> GetFlavours(int scoops)
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
            public static List<Topping> GetToppings()
            {
                List<string> toppingMenu = new List<string>() { "sprinkles", "mochi", "sago", "oreos" };
                List<Topping> toppings = new List<Topping>();

                int numToppings = CheckIntInput("Enter number of toppings (0-4): ", 0, 4);
                for (int i = 0; i < numToppings; i++)
                {
                    string toppingChoice = CheckUserInput(toppingMenu, $"Choose topping {i + 1}: ");
                    Topping topping = new Topping(toppingChoice);
                    toppings.Add(topping);
                }

                return toppings;
            }
            
            // Additional Features
            
            // Check if user input is an existing item on the list (For Option 4)
            public static string CheckUserInput(List<string> menu, string prompt)
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
            public static int CheckIntInput(string prompt, int min, int max)
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
            public static bool checkYesNoInput(string prompt)
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