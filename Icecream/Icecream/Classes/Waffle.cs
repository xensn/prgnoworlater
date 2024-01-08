namespace Icecream;

public class Waffle:IceCream
{
    public string WaffleFlavour { get; set; }
    
    public Waffle(){}

    public Waffle(string option, int scoops, List<Flavour> flavours, List<Topping> toppings, string waffleFlavour):base(option, scoops, flavours, toppings)
    {
        WaffleFlavour = waffleFlavour;
    }

    public override double CalculatePrice()
    {
        double price = Scoops switch
        {
            1 => 7 + Toppings.Count,
            2 => 8.5 + Toppings.Count,
            _ => 9.5 + Toppings.Count
        };

        if (WaffleFlavour != "Original")
        {
            price += 3;
        }
        
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