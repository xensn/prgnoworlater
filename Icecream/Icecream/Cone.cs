namespace Icecream;

public class Cone:IceCream
{
   public bool Dipped { get; set; }
   
   public Cone(){}

   public Cone(string option, int scoops, List<Flavour> flavours, List<Topping> toppings, bool dipped) : base(option,
      scoops, flavours, toppings)
   {
      Dipped = dipped;
   }

   public override double CalculatePrice()
   {
      double price = Scoops switch
      {
         1 => 4 + Toppings.Count,
         2 => 5.5 + Toppings.Count,
         _ => 6.5 + Toppings.Count
      };

      if (Dipped == true)
      {
         price += 2;
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