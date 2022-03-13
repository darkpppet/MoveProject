using System.Text.RegularExpressions;
using System.Text.Json.Nodes;
using System.Net;

class Classifier
{
    private WebClient webClient = new(); //instanace which get response from api

    private string ApiUrlBody { get => @"https://solved.ac/api/v3/problem/show?problemId="; } //appending problem ID to this is real api url; ex) ApiUrlBody + "1000";

    public string Path { get; set; } = "."; //sources folder path

    private Regex fileNameRegex = new(@"^\d+(_\w*)?\.\w+$"); //digit[_word].word; ex) 13705_Binary.cs, 1000.cs, ...
    private Regex fileNameAdditionalRegex = new(@"(_\w*)?\.\w+"); //[_word].word; ex) _Newton.cs

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
        string logFileName = $"movelog.txt";
        File.AppendAllText(logFileName, $"{DateTime.Now.ToString("G")}\r\n\r\n");

        int moveCount = 1;

        DirectoryInfo directoryInfo = new(Path);

        foreach (var rankDirectory in directoryInfo.GetDirectories()) //rank folders
        {
            string originRank = rankDirectory.Name; //rank folder name

            foreach (var file in rankDirectory.GetFiles()) //source files
            {
                if (fileNameRegex.IsMatch(file.Name))
                {
                    string problemID = fileNameAdditionalRegex.Replace(file.Name, "");

                    Console.Write($"  {problemID, 6}...");
                    
                    try
                    {
                        string response = webClient.DownloadString(ApiUrlBody + problemID);
                        JsonNode responseNode = JsonNode.Parse(response)!;

                        int level = (int)responseNode["level"]!;
                        string nowRank = RankLevelToRankString(level);

                        try
                        {
                            if (nowRank != originRank)
                            {
                                if (!Directory.Exists(Path + '/' + nowRank))
                                    Directory.CreateDirectory(Path + '/' + nowRank);
                                File.Move(file.FullName, Path + '/' + nowRank + '/' + file.Name);
                                File.AppendAllText(logFileName, $"  #{moveCount++} [{problemID, 6}]: {originRank} → {nowRank}\r\n");
                                Console.WriteLine("ok(move)");
                            }
                            else
                            {
                                Console.WriteLine("ok(not move)");
                            }
                        }
                        catch(IOException)
                        {
                            File.AppendAllText(logFileName, $"  #{moveCount++} [{problemID, 6}]: io error\r\n");
                            Console.WriteLine("io error");
                        }
                    }
                    catch (WebException)
                    {
                        Console.WriteLine("api error");
                        return;
                    }
                }
            }
        }

        if (moveCount == 1)
            File.AppendAllText(logFileName, "Nothing Moves\r\n\r\n");
        else
            File.AppendAllText(logFileName, "\r\n\r\n");
    }
}