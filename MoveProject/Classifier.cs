using System.Text.RegularExpressions;
using System.Text.Json.Nodes;

class Classifier
{
    private readonly HttpClient httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(60)
    }; //instanace which get response from api

    private const string ApiUrlBody = @"https://solved.ac/api/v3/problem/lookup?problemIds="; //appending problem ID to this is real api url; ex) ApiUrlBody + "1000";
    private const string LogFileName = $"movinglog.txt";

    public string Path { get; set; } = "."; //sources folder path

    private readonly Regex FileNameRegex = new(@"^\d+(_\w*)?\.\w+$"); //digit[_word].word; ex) 13705_Binary.cs, 1000.cs, ...
    private readonly Regex FileNameAdditionalRegex = new(@"(_\w*)?\.\w+"); //[_word].word; ex) _Newton.cs

    private Dictionary<string, string> ProblemsOrigin = new();
    private Dictionary<string, string> ProblemsNew = new();
    
    public Classifier()
    {
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mover");
    }
    
    public void Classify()
    {
        WriteLog($"{DateTime.Now:G}");
        
        int logCount = 1;
        int errorCount = 0;
        int readCount = 0;
        int moveCount = 0;
        
        DirectoryInfo directoryInfo = new(Path);
        foreach (var rankDirectory in directoryInfo.GetDirectories()) //rank folders
        {
            string originRank = rankDirectory.Name; //rank folder name

            foreach (var file in rankDirectory.GetFiles()) //source files
            {
                if (FileNameRegex.IsMatch(file.Name)) //digit[_word].word; ex) 13705_Binary.cs, 1000.cs, ...
                {
                    readCount++;
                    ProblemsOrigin[file.Name] = originRank;
                }
            }
        }
        
        var queryStrings = ProblemsOrigin.Keys
            .Select((value, index) => (value, index))
            .GroupBy(x => x.index / 100)
            .Select(group => string.Join("%2C", group.Select(x => FileNameToProblemId(x.value))))
            .ToList();

        foreach (var queryString in queryStrings)
        {
            string response = httpClient.GetStringAsync(ApiUrlBody + queryString).Result;
                
            JsonNode responseNode = JsonNode.Parse(response)!;
            JsonArray items = responseNode.AsArray();

            foreach (var item in items)
            {
                string problemId = item["problemId"].ToString();
                string rank = RankLevelToRankString((int)item["level"]);
                
                ProblemsNew[problemId] = rank;
            }
        }

        foreach (var fileName in ProblemsOrigin.Keys)
        {
            string problemId = FileNameToProblemId(fileName);
            try
            {
                if (ProblemsOrigin[fileName] != ProblemsNew[problemId])
                {
                    moveCount++;
                
                    if (!Directory.Exists($"{Path}/{ProblemsNew[problemId]}"))
                        Directory.CreateDirectory($"{Path}/{ProblemsNew[problemId]}");
                
                    string srcPath = $"{Path}/{ProblemsOrigin[fileName]}/{fileName}";
                    string destPath = $"{Path}/{ProblemsNew[problemId]}/{fileName}";
                    File.Move(srcPath, destPath);
                
                    WriteLog(logCount++, problemId, $"{ProblemsOrigin[fileName]} → {ProblemsNew[problemId]}");
                    Console.WriteLine($"{problemId}: {ProblemsOrigin[fileName]} → {ProblemsNew[problemId]}");
                }
            }
            catch (KeyNotFoundException)
            {
                errorCount++;
                WriteLog(logCount++, problemId, "no problemId error");
                Console.Write($"{problemId}: no problemId error");
            }
        }

        WriteLog($"\r\nread {readCount} files, move {moveCount} files, occur {errorCount} errors");
        Console.WriteLine($"read {readCount} files, move {moveCount} files, occur {errorCount} errors");
    }

    private string FileNameToProblemId(string filename) => FileNameAdditionalRegex.Replace(filename, "");
    
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
        File.AppendAllText(LogFileName, $"{text}\r\n\r\n");
    }

    private void WriteLog(int index, string id, string text)
    {
        File.AppendAllText(LogFileName, $"#{index, 4} [{id, 6}]: {text}\r\n");
    }
}