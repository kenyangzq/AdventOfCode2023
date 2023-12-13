using System;
using System.IO;
using System.Net.NetworkInformation;
using Day5;

public class Program
{
    public static void Main()
    {
        Part2();
    }

    private static void Part1()
    {
        
        List<long> seeds;
        ConversionPipeline pipeline = new ConversionPipeline();

        using (var reader = new StreamReader("input.txt"))
        {
            var line = reader.ReadLine();

            if (line == null) {
                throw new Exception("Invalid input");
            }

            seeds = line.Split(":")[1].Trim().Split(" ").Select(long.Parse).ToList();

            Console.WriteLine($"Seeds: {string.Join(", ", seeds)}");

            while ((line = reader.ReadLine()) != null)
            {
                var sourceName = "";
                var destName = "";
                if (line.EndsWith("map:"))
                {
                    var mapName = line.Substring(0, line.Length - 5).Split("-");
                    sourceName = mapName[0];
                    destName = mapName[2];

                    ConversionMap map = new ConversionMap();
                    map.DestType = destName;
                    map.SourceType = sourceName;

                    while ((line = reader.ReadLine()) != null && line != "")
                    {
                        var parts = line.Split(" ").Select(long.Parse).ToList();
                        ConversionRule rule = new ConversionRule(parts[0], parts[1], parts[2]);
                        map.Rules.Add(rule);
                    }

                    pipeline.AddMap(sourceName, destName, map);
                }
            }
        }

        var locations = seeds.Select(s => pipeline.Convert("seed", s)).ToList();
        
        Console.WriteLine($"Result: {locations.Min()}");
    }

    private static void Part2()
    {
        List<long> seeds;
        ConversionPipeline pipeline = new ConversionPipeline();

        using (var reader = new StreamReader("input.txt"))
        {
            var line = reader.ReadLine();

            if (line == null) {
                throw new Exception("Invalid input");
            }

            seeds = line.Split(":")[1].Trim().Split(" ").Select(long.Parse).ToList();

            Console.WriteLine($"Seeds: {string.Join(", ", seeds)}");

            while ((line = reader.ReadLine()) != null)
            {
                var sourceName = "";
                var destName = "";
                if (line.EndsWith("map:"))
                {
                    var mapName = line.Substring(0, line.Length - 5).Split("-");
                    sourceName = mapName[0];
                    destName = mapName[2];

                    ConversionMap map = new ConversionMap();
                    map.DestType = destName;
                    map.SourceType = sourceName;

                    while ((line = reader.ReadLine()) != null && line != "")
                    {
                        var parts = line.Split(" ").Select(long.Parse).ToList();
                        ConversionRule rule = new ConversionRule(parts[0], parts[1], parts[2]);
                        map.Rules.Add(rule);
                    }

                    pipeline.AddMap(sourceName, destName, map);
                }
            }
        }

        for (long i = 0; i < long.MaxValue; i++)
        {
            var seed = pipeline.ReverseConvert("location", i);
            if (IsValidSeed(seed, seeds))
            {
                Console.WriteLine($"Result: {i}");
                break;
            }
        }
    }

    private static bool IsValidSeed(long seed, List<long> seeds) 
    {
        for (int i = 0; i < seeds.Count(); i += 2) {
            if (seed >= seeds[i] && seed < seeds[i] + seeds[i+1])
            {
                return true;
            }
        }
        return false;
    }  
}