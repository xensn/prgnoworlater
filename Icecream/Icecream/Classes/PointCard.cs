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

    public void AddPoints(int points)
    {
        Points += points;
        
        if (Points >= 100 && Tier != "Gold")
        {
            Tier = "Gold";
        }
        
        else if (Points >= 50 && Tier == "Ordinary")
        {
            Tier = "Silver";
        }
    }

    public double? RedeemPoints(int pointsToRedeem)
    {
            if (Points > 0)
            {
                while (true)
                {
                    try
                    {
                        if (Points >= pointsToRedeem)
                        {
                            double discount = pointsToRedeem * 0.02;

                            Points -= pointsToRedeem;
                            
                            return discount;
                            break;
                        }
                        else
                        {
                            throw new Exception($"You can only redeem up to {Points} points!");
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.Write("Number of Points to redeemed: ");
                        pointsToRedeem = Convert.ToInt32(Console.ReadLine());
                    }
                }
            }
            else
            {
                return null;
            }
    }

    public void Punch()
    {
        PunchCard += 1;
    }

    public override string ToString()
    {
        return $"Points: {Points} | PunchCard: {PunchCard} | Tier: {Tier}";
    }
}