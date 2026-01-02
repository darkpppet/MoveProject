using System.Text.Json.Nodes;

JsonNode jsonNode = JsonNode.Parse(File.ReadAllText("pathinfo.json"))!;

string[] lines = File.ReadAllLines((string)jsonNode["inputPath"]!);

Dictionary<string, string> dictionary = new();

for (int i = 0; i < lines.Length; i += 3)
{
    string[] splitedLine = lines[i].Split(new[] { ' ', '\t' });
    
    dictionary[splitedLine[2]] = splitedLine[0] + splitedLine[1];
}

DirectoryInfo directoryInfo = new((string)jsonNode["sourcePath"]!);
        
foreach (var folder in directoryInfo.GetDirectories())
{
    string name = folder.Name;
    string tier = dictionary[name];
    
    if (!Directory.Exists((string)jsonNode["outputPath"]! + $"/{tier}"))
        Directory.CreateDirectory((string)jsonNode["outputPath"]! + $"/{tier}");
    
    File.Copy((string)jsonNode["sourcePath"]! + $"/{name}/{name}/Program.cs", (string)jsonNode["outputPath"]! + $"/{tier}/{name}.cs");
}
