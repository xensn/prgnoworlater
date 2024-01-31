namespace Icecream;
//========================================================== 
// Student Number : S10258427D
// Student Name : Senthilkumar Dhavasre
// Partner Name : Chia Eason
//========================================================== 

public class Cup:IceCream
{
    public Cup(){}
    
    public Cup(string option, int scoops, List<Flavour> flavours, List<Topping> toppings):base(option, scoops, flavours, toppings){}

    public override double CalculatePrice()
    {
        double price = Scoops switch
        {
            1 => 4 + Toppings.Count,
            2 => 5.50 + Toppings.Count,
            _ => 6.50 + Toppings.Count
        };

        foreach (Flavour f in Flavours)
        {
            if (f.Premium == true)
            {
                price += 2;
            }
        }

        return price;
    }

    public override string ToString()
    {
        return $"{base.ToString()} | Price: {CalculatePrice()}";
    }
}