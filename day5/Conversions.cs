using System;

namespace Day5{

    public class ConversionRule {
        public long DestStart { get; set; }

        public long SourceStart { get; set; }

        public long Length { get; set; }

        public ConversionRule(long destStart, long sourceStart, long length) {
            DestStart = destStart;
            SourceStart = sourceStart;
            Length = length;
        }

        public bool TryConvert(long value, out long result) {
            result = 0;
            if (value >= SourceStart && value < SourceStart + Length) {
                result = DestStart + (value - SourceStart);
                return true;
            }
            return false;
        }

        public bool TryReverseConvert(long value, out long result) {
            result = 0;
            if (value >= DestStart && value < DestStart + Length) {
                result = SourceStart + (value - DestStart);
                return true;
            }
            return false;
        }
    }

    public class ConversionMap {

        public string SourceType { get; set;}

        public string DestType { get; set; }

        public List<ConversionRule> Rules { get; set; }

        public ConversionMap() {
            Rules = new List<ConversionRule>();
            DestType = "";
            SourceType = "";
        }

        public long Convert(long value) {
            foreach (var rule in Rules) {
                if (rule.TryConvert(value, out var result)) {
                    return result;
                }
            }
            return value;
        }

        public long ReverseConvert(long value) {
            foreach (var rule in Rules) {
                if (rule.TryReverseConvert(value, out var result)) {
                    return result;
                }
            }
            return value;
        }
    }

    public class ConversionPipeline {
        private Dictionary<string, ConversionMap> SourceToMaps { get; set; }
        
        private Dictionary<string, ConversionMap> DestToMaps { get; set; }

        public ConversionPipeline() {
            SourceToMaps = new Dictionary<string, ConversionMap>();
            DestToMaps = new Dictionary<string, ConversionMap>();
        }

        public void AddMap(string sourceType, string destType, ConversionMap map) {
            SourceToMaps.Add(sourceType, map);
            DestToMaps.Add(destType, map);
        }

        public long Convert(string sourceType, long value) {
            string curType = sourceType;
            while (curType != "location") {
                SourceToMaps.TryGetValue(curType, out var map);
                if (map == null) {
                    throw new Exception($"No map found for type {curType}");
                }

                value = map.Convert(value);
                curType = map.DestType;
            }

            return value;
        }

        public long ReverseConvert(string destType, long value) {
            string curType = destType;
            while (curType != "seed") {
                DestToMaps.TryGetValue(curType, out var map);
                if (map == null) {
                    throw new Exception($"No map found for type {curType}");
                }

                value = map.ReverseConvert(value);
                curType = map.SourceType;
            }

            return value;
        }
        
    }

}
