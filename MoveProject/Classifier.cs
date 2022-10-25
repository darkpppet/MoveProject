using System.Text.RegularExpressions;
using System.Text.Json.Nodes;
using System.Net;

class Classifier
{
    private readonly HttpClient httpClient = new(); //instanace which get response from api

    private const string apiUrlBody = @"https://solved.ac/api/v3/problem/show?problemId="; //appending problem ID to this is real api url; ex) ApiUrlBody + "1000";
    private const string logFileName = $"movinglog.txt";

    public string Path { get; set; } = "."; //sources folder path

    private readonly Regex fileNameRegex = new(@"^\d+(_\w*)?\.\w+$"); //digit[_word].word; ex) 13705_Binary.cs, 1000.cs, ...
    private readonly Regex fileNameAdditionalRegex = new(@"(_\w*)?\.\w+"); //[_word].word; ex) _Newton.cs

    public void Classify()
    {
        WriteLog($"{DateTime.Now:G}");

        int logCount = 1;
        int moveCount = 0;
        int errorCount = 0;
        int readCount = 0;

        DirectoryInfo directoryInfo = new(Path);

        foreach (var rankDirectory in directoryInfo.GetDirectories()) //rank folders
        {
            string originRank = rankDirectory.Name; //rank folder name

            foreach (var file in rankDirectory.GetFiles()) //source files
            {
                if (fileNameRegex.IsMatch(file.Name)) //digit[_word].word; ex) 13705_Binary.cs, 1000.cs, ...
                {
                    readCount++;
                    do
                    {
                        string problemID = fileNameAdditionalRegex.Replace(file.Name, ""); //ex) "1000"
                        Console.Write($"  {problemID, 6}...");

                        try
                        {
                            string response = httpClient.GetStringAsync(apiUrlBody + problemID).Result;
                            JsonNode responseNode = JsonNode.Parse(response)!;

                            int level = (int)responseNode["level"]!;
                            string nowRank = RankLevelToRankString(level);

                            if (nowRank != originRank)
                            {
                                moveCount++;
                                if (!Directory.Exists(Path + '/' + nowRank))
                                {
                                    Directory.CreateDirectory(Path + '/' + nowRank);
                                }
                                File.Move(file.FullName, Path + '/' + nowRank + '/' + file.Name);
                                WriteLog(logCount++, problemID, $"{originRank} → {nowRank}");
                                Console.WriteLine("ok(move)");
                            }
                            else
                            {
                                Console.WriteLine("ok(not move)");
                            }
                        }
                        catch (Exception)
                        {
                            errorCount++;
                            WriteLog(logCount++, problemID, "error");
                            Console.Write($"error: wating 1 minutes...");
                            Thread.Sleep(60 * 1000);
                            continue;
                        }
                        break;
                    } while (true);
                }
            }
        }

        WriteLog($"read {readCount} files, move {moveCount} files, occur {errorCount} errors");
        Console.WriteLine($"read {readCount} files, move {moveCount} files, occur {errorCount} errors");
    }

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
            4 => "I",
            _ => ""
        };
        
        return result;
    }

    private void WriteLog(string text)
    {
        File.AppendAllText(logFileName, $"{text}\r\n\r\n");
    }

    private void WriteLog(int index, string id, string text)
    {
        File.AppendAllText(logFileName, $"#{index, 4} [{id, 6}]: {text}\r\n");
    }
}
