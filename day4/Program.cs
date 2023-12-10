using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;


public class Card
{
    public int Id { get; set;}
    public List<int> WinningNumbers { get; set; }

    public List<int> MyNumbers { get; set; }

    public int MatchCount { get; set; }

    public int InstanceCount { get; set; }

    public override string ToString()
    {
        return $"Card id: {Id}, WinningNumbers: {string.Join(",", WinningNumbers)}, MyNumbers: {string.Join(",", MyNumbers)} MatchCount: {MatchCount}";
    }

    public Card(int id){
        Id = id;

        WinningNumbers = new List<int>();
        MyNumbers = new List<int>();
        
        MatchCount = 0;
        InstanceCount = 1;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var answer = ReadCardsAndGetNumberOfCopies();

        Console.WriteLine($"Answer: {answer}");
    }

    private static int ReadCardsAndGetWinningPoints(){
        var cards = new List<Card>();
        var points = 0;

        string line;
        using (var reader = new StreamReader("input.txt"))
        {
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(':');
                var id = int.Parse(parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
                var numbersParts = parts[1].Split('|');
                var numbers1 = numbersParts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                var numbers2 = numbersParts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                var card = new Card(id) {
                        WinningNumbers = numbers1, 
                        MyNumbers = numbers2,
                        MatchCount = numbers1.Intersect(numbers2).Count() 
                    };
                cards.Add(card);

                Console.WriteLine(card);
                // Console.WriteLine(card.MatchCount > 0 ? Math.Pow(2, (card.MatchCount-1)) : 0);
            }
        }


        points = cards.Select(x => x.MatchCount > 0 ? (int)(Math.Pow(2, (x.MatchCount-1))) : 0).Sum();

        return points;
    }

    private static int ReadCardsAndGetNumberOfCopies(){
        var cards = new List<Card>();
        var points = 0;

        string line;
        using (var reader = new StreamReader("input.txt"))
        {
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(':');
                var id = int.Parse(parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
                var numbersParts = parts[1].Split('|');
                var numbers1 = numbersParts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                var numbers2 = numbersParts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                var card = new Card(id) {
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
        int copyCounts = 0;

        for (int i = 0; i < cards.Count(); i++) {
            for (int j = 0; j < cards[i].MatchCount; j++) {
                cards[i+j+1].InstanceCount += cards[i].InstanceCount;
            }

            copyCounts += cards[i].InstanceCount;    
        }

        return copyCounts;
    }
}