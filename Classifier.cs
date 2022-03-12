using System.Text.RegularExpressions;
using System.Text.Json.Nodes;
using System.Net;

class Classifier
{
    private WebClient webClient = new();

    private string ApiUrlBody { get => @"https://solved.ac/api/v3/problem/show?problemId="; } //append problem ID is real api url. 

    public string SrcPath { get; set; } = "./src";
    public string DestPath { get; set; } = "./dest";

    private Regex fileNameRegex = new(@"^\d+(_\w*)?\.\w+$"); //digit[_word].word
    private Regex fileNameAdditionalRegex = new(@"(_\w*)?\.\w+"); //[_word].word

    private string RankLevelToRankString(int level)
    {
        if (level == 0)
            return "Unrated";

        level -= 1;

        string result = (level / 5) switch
        {
            0 => "Bronze",
            1 => "Silver",
            2 => "Gold",
            3 => "Platinum",
            4 => "Diamond",
            5 => "Ruby",
            _ => ""
        };

        result += (level % 5) switch
        {
            0 => "V",
            1 => "IV",
            2 => "III",
            3 => "II",
            4 => "II",
            _ => ""
        };
        
        return result;
    }
    public void Classify()
    {
        DirectoryInfo srcDirectoryInfo = new(SrcPath);

        foreach (var srcRankDirectory in srcDirectoryInfo.GetDirectories())
        {
            string originRank = srcRankDirectory.Name;

            foreach (var file in srcRankDirectory.GetFiles())
            {
                if (fileNameRegex.IsMatch(file.Name))
                {
                    string problemID = fileNameAdditionalRegex.Replace(file.Name, "");

                    Console.Write($"  {problemID, 6}...");
                    
                    string response = webClient.DownloadString(ApiUrlBody + problemID);
                    JsonNode responseNode = JsonNode.Parse(response)!;

                    int level = (int)responseNode["level"]!;

                    string nowRank = RankLevelToRankString(level);

                    if (nowRank != originRank)
                        File.AppendAllText("movelog.txt", $"[{problemID, 6}]: {originRank} → {nowRank}\r\n");

                    if (!Directory.Exists(DestPath + '/' + nowRank))
                            Directory.CreateDirectory(DestPath + '/' + nowRank);

                    File.Copy(file.FullName, DestPath + '/' + nowRank + '/' + file.Name);

                    Console.WriteLine("ok");
                }
            }
        }
    }
}