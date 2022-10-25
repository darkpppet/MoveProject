using System.Text.Json.Nodes;

class Program
{
    public static void Main(string[] args)
    {
        JsonNode userInfo = JsonNode.Parse(File.ReadAllText("userinfo.json"))!;
        
        Classifier classifier = new()
        {
            Path = (string)userInfo["path"]!
        };

        Console.WriteLine("Start Classyfying");
        classifier.Classify();
        Console.WriteLine("Complete Classfying");
    }
}
