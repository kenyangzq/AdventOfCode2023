
using System.Data;

public class Program {

    const int min = 1;
    const int max = 4000;

    public static void Main(string[] args) {
        ParseInput("input.txt", out var workflows, out var parts);
        
        // var answer = SolvePart1(workflows, parts);
        // Console.WriteLine($"Answer: {answer}");

        var answer2 = SolvePart2(workflows);
        Console.WriteLine($"Answer: {answer2}");
    }

#region Part 1

    public static int SolvePart1(Dictionary<string, Workflow> workflows, List<Part> parts) {

        var curWorkflowName = "";
        var answer = 0;

        foreach (var part in parts) {
            curWorkflowName = "in";
            Console.WriteLine($"\nWorking on Part {part}");

            while (curWorkflowName != "A" && curWorkflowName != "R") {
                var workflow = workflows[curWorkflowName];
                Console.WriteLine($"Part {part} is in workflow {workflow.Name}");
                curWorkflowName = workflow.GetNextWorkflow(part);
            }

            if (curWorkflowName == "A") {
                Console.WriteLine($"Part {part} is accepted");
                answer += part.PartValue;
            } else {
                Console.WriteLine($"Part {part} is rejected");
            }
        }

        return answer;
    }

#endregion

#region Part 2
    public class FourTuple {
        public List<long> values = new List<long>();

        public FourTuple(long x, long m, long a, long s) {
            this.values.Add(x);
            this.values.Add(m);
            this.values.Add(a);
            this.values.Add(s);
        }

        public FourTuple(FourTuple other) {
            values = new List<long>(other.values);
        }

        public override string ToString() {
            return string.Join(",", values);
        }

        public long Product() {
            return values[0] * values[1] * values[2] * values[3];
        }

        public FourTuple Diff(FourTuple other) {
            return new FourTuple(
                values[0] - other.values[0] + 1,
                values[1] - other.values[1] + 1,
                values[2] - other.values[2] + 1,
                values[3] - other.values[3] + 1
            );
        }

        public void Set(FourTuple other) {
            values[0] = other.values[0];
            values[1] = other.values[1];
            values[2] = other.values[2];
            values[3] = other.values[3];
        }
    }

    public static Dictionary<string, int> LetterToIndex = new Dictionary<string, int> {
        { "x", 0 },
        { "m", 1 },
        { "a", 2 },
        { "s", 3 },
    };

    public static long SolvePart2(Dictionary<string, Workflow> workflows) {

        long result = 0;
        FourTuple low = new FourTuple(min, min, min, min);
        FourTuple high = new FourTuple(max, max, max, max);

        ProcessRule("in");

        return result;

        void ProcessRule(string curWorkflowName) {
            Console.WriteLine($"Processing rule {curWorkflowName}. Low: {low}, High: {high}");

            if (curWorkflowName == "A") {
                // Add result
                var product = high.Diff(low).Product();
                Console.WriteLine($"low: {low}, high: {high}, Product: {product}\n");
                result += product;
                return;
            }

            if (curWorkflowName == "R") {
                return;
            }

            var curWorkflow = workflows[curWorkflowName];

            // The value when start processing the workflow.
            var startLow = new FourTuple(low);
            var startHigh = new FourTuple(high);
            foreach (var rule in curWorkflow.Rules) {
                if (rule.ruleType == RuleType.Direct) {
                    ProcessRule(rule.workflow);
                    continue;
                }
                
                var index = LetterToIndex[rule.source];
                if (rule.ruleType == RuleType.GreaterThan) {
                    var original = new FourTuple(low);
                    
                    if (rule.against >= high.values[index]) {
                        continue;
                    }
                    if (rule.against > low.values[index]) {
                        low.values[index] = rule.against + 1;
                        ProcessRule(rule.workflow);
                        low.Set(original);
                        high.values[index] = rule.against;
                    }
                    else {
                        ProcessRule(rule.workflow);
                    }
                }
                else if (rule.ruleType == RuleType.LessThan) {
                    var original = new FourTuple(high);

                    if (rule.against <= low.values[index]) {
                        continue;
                    }
                    if (rule.against < high.values[index]) {
                        high.values[index] = rule.against - 1;
                        ProcessRule(rule.workflow);
                        high.Set(original);
                        low.values[index] = rule.against;
                    }
                    else {
                        ProcessRule(rule.workflow);
                    }
                }
            }

            // Reset the value after processing the workflow.
            low.Set(startLow);
            high.Set(startHigh);
        }
    }
#endregion

#region Helper
    private static void ParseInput(string fileName, out Dictionary<string, Workflow> workflows, out List<Part> parts) {
        var input = File.ReadAllLines(fileName);

        int i = 0;

        workflows = new Dictionary<string, Workflow>();
        parts = new List<Part>();

        // parse workflows
        while (i < input.Length && !string.IsNullOrEmpty(input[i])) {
            var workflow = new Workflow(input[i]);
            workflows.Add(workflow.Name, workflow);
            i++;

            // Console.WriteLine(workflow);
        }

        // skip blank line
        i++;

        // parse parts
        while (i < input.Length) {
            var part = new Part(input[i]);
            i++;

            // Console.WriteLine(part);
            parts.Add(part);
        }

    }

    public class Workflow {
        public string Name { get; set; }

        public List<Rule> Rules { get; set; } = new List<Rule>();
        
        public Workflow(string workflowString) {
            var parts = workflowString.Replace("}", "").Split("{");
            Name = parts[0].Trim();
            
            var rules = parts[1].Split(",");

            foreach (var rule in rules) {
                Rules.Add(new Rule(rule));
            }
        }
        
        public override string ToString() {
            return $"{Name} {{{string.Join("\n", Rules)}}}\n\n";
        }

        public string GetNextWorkflow(Part p) {
            foreach (var rule in Rules) {
                if (rule.Evaluate(p)) {
                    return rule.workflow;
                }
            }

            throw new Exception("No matching rule found");
        }
    }

    public enum RuleType {
        GreaterThan,
        LessThan,
        Direct,
    }

    public class Rule {
        public string source;

        public int against;

        public string workflow;

        public RuleType ruleType;

        public Rule(string ruleString) {
            if (ruleString.Contains(">")) {
                ruleType = RuleType.GreaterThan;

                var parts = ruleString.Split(":");

                var condition = parts[0].Trim();
                workflow = parts[1].Trim();
                
                
                var conditionParts = condition.Split(">");
                source = conditionParts[0].Trim();
                against = int.Parse(conditionParts[1].Trim());
            }
            else if (ruleString.Contains("<"))
            {
                ruleType = RuleType.LessThan;

                var parts = ruleString.Split(":");

                var condition = parts[0].Trim();
                workflow = parts[1].Trim();
                
                var conditionParts = condition.Split("<");
                source = conditionParts[0].Trim();
                against = int.Parse(conditionParts[1].Trim());
            } else {
                ruleType = RuleType.Direct;
                source = "";
                against = 0;
                workflow = ruleString;
            }
        }

        public bool Evaluate(Part p) {
            switch (ruleType) {
                case RuleType.GreaterThan:
                    return p.GetValue(source) > against;
                case RuleType.LessThan:
                    return p.GetValue(source) < against;
                case RuleType.Direct:
                    return true;
            }

            return false;
        }

        public override string ToString() {
            return $"{source} {ruleType.ToString()} {against} {workflow}";
        }
    }

    public class Part {
        int x;
        int m;
        int a;
        int s;

        public Part(string partsString) {
            var parts = partsString.Replace("{", "").Replace("}", "").Split(",");
            x = int.Parse(parts[0].Split("=")[1].Trim());
            m = int.Parse(parts[1].Split("=")[1].Trim());
            a = int.Parse(parts[2].Split("=")[1].Trim());
            s = int.Parse(parts[3].Split("=")[1].Trim());
        }

        public override string ToString()
        {
            return "x=" + x + ", m=" + m + ", a=" + a + ", s=" + s;
        }

        public int PartValue => x + m + a + s;

        
        public int GetValue(string source) {
            switch (source) {
                case "x":
                    return x;
                case "m":
                    return m;
                case "a":
                    return a;
                case "s":
                    return s;
                default:
                    throw new Exception("Invalid source");
            }
        }
    }

#endregion

}