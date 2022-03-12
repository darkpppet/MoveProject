using System.Text.Json;

class Program
{
    public static void Main(string[] args)
    {
        UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(File.ReadAllText("userinfo.json"))!;
        
        Classifier classifier = new()
        {
            SrcPath = userInfo.SrcPath!,
            DestPath = userInfo.DestPath!
        };

        Console.WriteLine("Start Classyfying");
        classifier.Classify();
        Console.WriteLine("Complete Classfying");
    }
}

