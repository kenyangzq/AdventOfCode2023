using System.IO;
using System.Collections.Generic;


public class Card
{
    public int Id { get; set;}
    public List<int> WinningNumbers { get; set; }

    public List<int> MyNumbers { get; set; }

    public int MatchCount { get; set; }

    public override string ToString()
    {
        return $"Card id: {Id}, WinningNumbers: {string.Join(",", WinningNumbers)}, MyNumbers: {string.Join(",", MyNumbers)} MatchCount: {MatchCount}";
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var cards = new List<Card>();
        var points = 0;
        
        using (var reader = new StreamReader("input.txt"))
        {
            while (!reader.EndOfStream)
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(':');
                    var id = int.Parse(parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
                    var numbersParts = parts[1].Split('|');
                    var numbers1 = numbersParts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    var numbers2 = numbersParts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                    var card = new Card {
                            Id = id, 
                            WinningNumbers = numbers1, 
                            MyNumbers = numbers2,
                            MatchCount = numbers1.Intersect(numbers2).Count() 
                        };
                    cards.Add(card);

                    Console.WriteLine(card);
                    // Console.WriteLine(card.MatchCount > 0 ? Math.Pow(2, (card.MatchCount-1)) : 0);
                }
                
            }
        }

        points = cards.Select(x => x.MatchCount > 0 ? (int)(Math.Pow(2, (x.MatchCount-1))) : 0).Sum();

        Console.WriteLine($"Points: {points}");
    }
}