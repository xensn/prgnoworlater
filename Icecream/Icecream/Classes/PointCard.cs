using System.Drawing;


// Need to check and redo
namespace Icecream;

public class PointCard
{
    public int Points { get; set; }
    public int PunchCard { get; set; }
    public string Tier { get; set; } = "Ordinary";
    
    public PointCard(){}

    public PointCard(int points, int punchCard)
    {
        Points = points;
        PunchCard = punchCard;
    }

    public void AddPoints(int price)
    {
        Points += Convert.ToInt32(Math.Floor(price * 0.72));
        
        if (Points >= 100 && Tier != "Gold" )
        {
            Tier = "Gold";
        }
        
        else if (Points >= 50 && Tier == "Ordinary")
        {
            Tier = "Silver";
        }
    }

    public void RedeemPoints(int price)
    {
        if (Tier != "Ordinary")
        {
            if (Points > 0)
            {
                Console.Write("Number of Points to redeemed: ");
                int pointsToRedeem = Convert.ToInt32(Console.ReadLine());
                double discount = pointsToRedeem * 0.02;

                Points = -pointsToRedeem;   
                
                Console.WriteLine("Price Deducted: {0}",discount);
                Console.WriteLine("After discount price: {0}",price-discount );
                Console.WriteLine("Remaining Points: {0}", Points);
            }
        }
    }

    public void Punch()
    {
        if (PunchCard == 10)
        {
            Console.WriteLine("Your 11th Ice-cream is free.");
            PunchCard = 10; 
        }

        else
        {
            PunchCard += 1;
        }
    }

    public override string ToString()
    {
        return $"Points: {Points} | PunchCard: {PunchCard} | Tier: {Tier}";
    }
}