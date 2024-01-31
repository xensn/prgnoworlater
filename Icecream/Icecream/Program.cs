﻿using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Formats.Asn1;
using System.Reflection;
using System.Text;
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
            
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelKeyPress_Handler);
            
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
                        Payment();
                    }
                    
                    else if (options == 8)
                    {
                        TotalPriceBreakdown();
                    }

                    else
                    {
                        WriteDataToOrders();
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

                bool anotherOrder = true;
                while (true)
                {

                    anotherOrder = checkYesNoInput("Would you like to add another ice cream to the order?(y/n): ");
                    if (anotherOrder)
                    {
                        iceCream = CreateIceCream();

                        ordered.AddIceCream(iceCream);
                    }

                    else if (!anotherOrder)
                    {
                        break;
                    }
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
                    try
                    {
                        customerOrder.Add(chosenCustomer.Memberid, ordered);
                    }
                    catch (System.ArgumentException e)
                    {
                        Console.WriteLine("You have already ordered.Please use option 6 to add ice cream.");
                    }
                }
            }


            // 5) Display order details of a customer
            void OrderDetails()
            {
                // list all the customers
                AllCustomersInfo();

                // prompt the user for the customer id and get the chosen customer
                Order chosenOrder = null;
                Customer chosencustomer = ChooseCustomer(null);
                
                // get the chosen customer's current order from customerorder dict 
                foreach (KeyValuePair<int, Order> kvp in customerOrder)
                {
                    if (chosencustomer.Memberid == kvp.Key)
                    {
                        chosenOrder = kvp.Value;
                    }
                }
                
                // check if the customer's current order even exists, and print the details accordingly
                if (chosenOrder != null)
                {
                    Console.WriteLine("Current Orders" +
                                      "\n-------------------------------------------------------------------------------------------------");
                    foreach (IceCream icecream in chosenOrder.IceCreamList)
                    {
                        Console.WriteLine($"{chosenOrder.IceCreamList.IndexOf(icecream) + 1}. {icecream.ToString()}");
                    }
                    Console.WriteLine(
                        "-------------------------------------------------------------------------------------------------");
                }
                else
                {
                    Console.WriteLine("There are no current orders for the given Member ID.");
                }

                // list to store currrent matching order info from order.csv
                List<string[]> matchingorderinfo = new List<string[]>();
                
                //reading the orders.csv file to check if there are any past orders matching the member id
                foreach (string[] orders in ReadFile("orders.csv"))
                {
                    if (chosencustomer.Memberid == Convert.ToInt32(orders[1]))
                    {
                        matchingorderinfo.Add(orders);
                    }
                }
                
                // if there is a matching member id then print out the past orders, if not then dont print out the past orders
                if (matchingorderinfo.Count > 0)
                {
                    Console.WriteLine("Past Orders" +
                                      "\n-------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("{0, -9} {1, -17} {2, -17} {3, -7} {4,-7} {5, -7} {6,-15} {7, -11} {8, -11} {9,-11} {10, -10} {11, -10} {12, -10} {13, -10}",
                        "Order Id", "Time Received", "Time Fulfilled", "Option", "Scoops", "Dipped", "Waffle Flavour", "Flavour1", "Flavour2", "Flavour3", "Toppings1", "Toppings2", "Toppings3", "Toppings4");

                    foreach (var elements in matchingorderinfo)
                    {
                        Console.WriteLine("{0, -9} {1, -17} {2, -17} {3, -7} {4,-7} {5, -7} {6,-15} {7, -11} {8, -11} {9,-11} {10, -10} {11, -10} {12, -10} {13, -10}",
                            elements[0] , elements[2] , elements[3], elements[4], elements[5], elements[6], elements[7], elements[8], elements[9], elements[10], elements[11], elements[12], elements[13], elements[14]);
                    }
                    Console.Write(
                        "---------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                }
                else
                {
                    Console.WriteLine("There are no past orders for the given Member ID.");
                }
            }

            // 6) Modify order details
            void ModifyOrderDetails()
            {
                // list the customers
                AllCustomersInfo();
                
                // prompt the user for their member id and get their current order so that they can make modifications
                Order chosenOrder = null;
                Customer? chosencustomer = ChooseCustomer(null);

                foreach (KeyValuePair<int, Order> kvp in customerOrder)
                {
                    if (chosencustomer.Memberid == kvp.Key)
                    {
                        chosenOrder = kvp.Value;
                    }
                }
                
                // loop to print out the options and the current order for the modifications [doing this only if current order exits for the chosencustomer]
                if (chosenOrder != null)
                {
                    bool whileloop = true;
                    while (whileloop)
                    {
                        // List all the ice cream objects contained in the order 
                        Console.WriteLine("Current Orders" +
                                          "\n-------------------------------------------------------------------------------------------------");
                        foreach (IceCream icecream in chosenOrder.IceCreamList)
                        {
                            Console.WriteLine($"{chosenOrder.IceCreamList.IndexOf(icecream) + 1}. {icecream.ToString()}");
                        }
                        Console.WriteLine(
                            "-------------------------------------------------------------------------------------------------");
                        int opt = CheckIntInput("Option 1 - Choose an existing ice cream to modify" +
                                                "\nOption 2 - Add an entirely new ice cream to your order" +
                                                "\nOption 3 - Choose an existing ice cream to delete from your order" +
                                                "\nOption 4 - Exit" +
                                                "\nPlease select an option: ", 1, 4);
                        switch (opt)
                        {
                            case 1:
                                Console.Write("Please select an ice cream to modify: ");
                                int icecreamopt = Convert.ToInt32(Console.ReadLine());
                                chosenOrder.ModifyIceCream(icecreamopt);
                                break;
                            case 2:
                                IceCream icecream = CreateIceCream();
                                chosenOrder.AddIceCream(icecream);
                                break;
                            case 3:
                                // not allowing them to delete the ice cream if there is only one ice cream left in the current order
                                if (chosenOrder.IceCreamList.Count != 1)
                                {
                                    Console.Write("Please select an ice cream that you want to delete: ");
                                    int icecreamtodelete = Convert.ToInt32(Console.ReadLine());
                                    chosenOrder.DeleteIceCream(icecreamtodelete);
                                }
                                else
                                {
                                    Console.WriteLine("You cannot remove this ice cream as there is only one ice cream in this order!");
                                }
                                break;
                            case 4:
                                whileloop = false;
                                break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("There are no current orders for the given Member ID.");
                }
            }
            
            // Advanced Feature (a)
            void Payment()
            {
                Order? chosenOrder = null;
                Order dequeueOrder;
                Customer? chosenCustomer = null;
                
                // Check if there are any any gold queue members.
                if (goldOrderQueue.Count > 0)
                {
                    // Dequeue and store the order we are handling in dequeueOrder
                    dequeueOrder = goldOrderQueue.Dequeue();
                }
                else
                {
                    dequeueOrder = orderQueue.Dequeue();
                }
                
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

                Console.WriteLine($"Member name:{chosenCustomer.Name}");
                // displaying all the ice creams within the order
                Console.WriteLine("Total Bill Amount" +
                                  "\n-------------------------------------------------------------------------------------------------");
                double totalprice = 0;
                int noofpunches = 0;
                IceCream expIceCream = dequeueOrder.IceCreamList[0];
                foreach (IceCream icecream in dequeueOrder.IceCreamList)
                {
                    totalprice += icecream.CalculatePrice();
                    noofpunches += 1;
                    Console.WriteLine($"{dequeueOrder.IceCreamList.IndexOf(icecream) + 1}. {icecream.ToString()}");
                    
                    // checking for the most expensive icecream in the current order
                    if (expIceCream.CalculatePrice() < icecream.CalculatePrice())
                    {
                        expIceCream = icecream;
                    }
                }
                Console.WriteLine(
                    "-------------------------------------------------------------------------------------------------");
                Console.WriteLine($"Total Price: {totalprice:C2}"); // displaying the total bill 
                Console.WriteLine($"Membership status: {chosenCustomer.Rewards.Tier} | Points: {chosenCustomer.Rewards.Points}"); // displaying the membership status and points
                
                //checking if it's the customer's birthday 
                if (chosenCustomer.isBirthday() == true)
                {
                    dequeueOrder.IceCreamList.Remove(expIceCream); //removing the expensive icecream from the order so that it will not be included in the final bill
                }
                
                // checking if the punchcard is completed and setting it to 0 if it is 
                if (chosenCustomer.Rewards.PunchCard == 10)
                {
                    dequeueOrder.IceCreamList.RemoveAt(0); // removing the first icecream from the order so that it will not be included in the final bill
                    chosenCustomer.Rewards.PunchCard = 0;
                }
                
                // calculate the final price 
                double finalbill = 0;
                foreach (IceCream icecream in dequeueOrder.IceCreamList)
                {
                    finalbill += icecream.CalculatePrice();
                }
                
                // checking pointcard status and checking if they can redeem points
                if (chosenCustomer.Rewards.Tier != "Ordinary")
                {
                    Console.Write("Number of Points to redeemed: ");
                    int pointsToRedeem = Convert.ToInt32(Console.ReadLine());
                    double? discount = chosenCustomer.Rewards.RedeemPoints(pointsToRedeem);
                    if (discount != null)
                    {
                        finalbill -= discount.Value;
                        Console.WriteLine("Price Deducted: {0}", discount);
                        Console.WriteLine("Remaining Points: {0}", chosenCustomer.Rewards.Points);
                    }
                    else
                    {
                        Console.WriteLine("You have no points left to be redeemed!");
                    }
                }
                Console.WriteLine($"The final bill is {finalbill:C2}"); // final bill displayed 
                Console.WriteLine("You can press any key to make payment");
                
                Console.ReadKey(); // waiting for the customer to press any key they want to make payment
                
                for(int i = 1; i<= noofpunches; i++)
                {
                    chosenCustomer.Rewards.Punch(); // punching the card for every ice cream in the order
                }

                if (chosenCustomer.Rewards.PunchCard >= 10)
                {
                    chosenCustomer.Rewards.PunchCard = 10; // setting the punch card back to 10 if the number of punches beyond 10
                }
                
                int pointstoadd = Convert.ToInt32(Math.Floor(finalbill * 0.72)); // calculating the points
                chosenCustomer.Rewards.AddPoints(pointstoadd); // adding the points and upgrading the member status accordingly
                
                chosenOrder.TimeFulfilled = DateTime.Now; //marking the order fulfilled
                
                // Append ordered item into the file
                using (StreamWriter sw = File.AppendText("orders.csv"))
                {
                    foreach (IceCream ic in chosenOrder.IceCreamList)
                    {
                        var line = new StringBuilder();
                        line.Append($"{chosenOrder.Id},{chosenCustomer.Memberid},{chosenOrder.TimeReceived.ToString("dd/MM/yyyy hh:MM")},{chosenOrder.TimeFulfilled.ToString("dd/MM/yyyy hh:MM")},{ic.Option},{ic.Scoops}");
                            
                        switch (ic.Option.ToLower())
                        {
                            case "cup":
                            {
                                Cup tempCup = (Cup)ic;
                                line.Append(",,");
                                break;
                            }
                            case "waffle":
                            {
                                Waffle tempWaffle = (Waffle)ic;
                                line.Append($",,{tempWaffle.WaffleFlavour}");
                                break;
                            }
                            case "cone":
                            {
                                Cone tempCone = (Cone)ic;
                                line.Append($",{tempCone.Dipped.ToString().ToUpper()},");
                                break;
                            }
                        }
                            
                        // Adding Flavours now
                        for (int i = 0; i < 3; i++)
                        {
                            if (i < ic.Flavours.Count)
                            {
                                line.Append($",{ic.Flavours[i].Type}");  
                            }
                            else
                            {
                                line.Append(",,");
                            }
                        }
                            
                        //  Adding Toppings now
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < ic.Toppings.Count)
                            {
                                line.Append($",{ic.Toppings[i].Type}");
                            }
                            else
                            {
                                line.Append(",,");
                            }
                        }
                        sw.WriteLine(line);
                    }
                }

                customerOrder.Remove(chosenCustomer.Memberid);
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
                        if (elements[3] == "")
                        {
                            continue;
                        }
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
                // If user terminate process in the middle, it will save whatever in customerOrder dictionary into the orders.csv
                void CancelKeyPress_Handler(object sender, ConsoleCancelEventArgs e)
                {
                    Console.WriteLine("Logging all data.");
                    e.Cancel = true;
                
                    WriteDataToOrders();
                
                    Environment.Exit(0);
                }
            
            
                // Format IceCream Object to append into orders.csv
                void WriteDataToOrders()
                {
                    using (StreamWriter sw = File.AppendText("orders.csv"))
                    {
                        foreach (KeyValuePair<int, Order> kvp in customerOrder)
                        {
                            foreach (IceCream ic in kvp.Value.IceCreamList)
                            {
                                var line = new StringBuilder();
                                line.Append($"{kvp.Value.Id},{kvp.Key},{kvp.Value.TimeReceived.ToString("dd/MM/yyyy hh:MM")}");

                                if (kvp.Value.TimeFulfilled == null)
                                {
                                    line.Append(",");
                                }
                                else
                                {
                                    line.Append($",{kvp.Value.TimeFulfilled}");
                                }

                                line.Append($",{ic.Option},{ic.Scoops}");
                            
                                switch (ic.Option.ToLower())
                                {
                                    case "cup":
                                    {
                                        Cup tempCup = (Cup)ic;
                                        line.Append(",,");
                                        break;
                                    }
                                    case "waffle":
                                    {
                                        Waffle tempWaffle = (Waffle)ic;
                                        line.Append($",,{tempWaffle.WaffleFlavour}");
                                        break;
                                    }
                                    case "cone":
                                    {
                                        Cone tempCone = (Cone)ic;
                                        line.Append($",{tempCone.Dipped.ToString().ToUpper()},");
                                        break;
                                    }
                                }
                            
                                // Adding Flavours now
                                for (int i = 0; i < 3; i++)
                                {
                                    if (i < ic.Flavours.Count)
                                    {
                                        line.Append($",{ic.Flavours[i].Type}");  
                                    }
                                    else
                                    {
                                        line.Append(",,");
                                    }
                                }
                            
                                //  Adding Toppings now
                                for (int i = 0; i < 4; i++)
                                {
                                    if (i < ic.Toppings.Count)
                                    {
                                        line.Append($",{ic.Toppings[i].Type}");
                                    }
                                    else
                                    {
                                        line.Append(",,");
                                    }
                                }
                                sw.WriteLine(line);
                            }
                        }
                    }
                }
            
            
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
            
                // This is a method to Read all kind of files.
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
            
                // This is to convert the data in order.csv to a IceCream Class
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
                    
                        if (line[i] != "")
                        {
                            Flavour tempFlavour = new Flavour(line[i], isPremium, 1);
                        
                            flavourList.Add(tempFlavour);
                        }

                    }
                
                    // Create Topping Object
                    for (int i = 11; i < 15; i++)
                    {
                        if (line[i] != "")
                        {
                            Topping tempTopping = new Topping(line[i]);
                            toppingList.Add(tempTopping);
                        }
                    }
                
                    if (line[4].ToLower() == "cup")
                    {
                        IceCream tempIceCream = new Cup(line[4], Convert.ToInt32(line[5]), flavourList, toppingList);
                        return tempIceCream;
                    }
                
                    else if (line[4].ToLower() == "cone")
                    {
                        bool.TryParse(line[6], out bool result);
                        IceCream tempIceCream = new Cone(line[4], Convert.ToInt32(line[5]), flavourList, toppingList, result);
                        return tempIceCream;
                    }
                
                    else if (line[4].ToLower() == "waffle")
                    {
                        Waffle tempIceCream = new Waffle(line[4],Convert.ToInt32(line[5]), flavourList, toppingList, line[7]);
                        return tempIceCream;
                    }

                    return null;
                }
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