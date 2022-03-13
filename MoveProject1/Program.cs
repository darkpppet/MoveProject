using System.Text;
using System.Text.Json.Nodes;

public class Program
{
	public static void Main(string[] args)
	{
		JsonNode jsonNode = JsonNode.Parse(File.ReadAllText("pathinfo.json"))!;

		string[] lines = File.ReadAllLines((string)jsonNode["inputPath"]);
		
		Dictionary<string, StringBuilder> outputBuilders = new();
		
		for (int i = 0; i < lines.Length; i += 3)
		{
			string[] splitedLine = lines[i].Split(new[] { ' ', '\t' });
			string key = splitedLine[0] + splitedLine[1];
			
			if (!outputBuilders.ContainsKey(key))
				outputBuilders[key] = new StringBuilder("mv ");
			
			outputBuilders[key].Append(splitedLine[2]);
			outputBuilders[key].Append(".cs ");
		}

		if (!Directory.Exists((string)jsonNode["outputPath"]))
        	Directory.CreateDirectory((string)jsonNode["outputPath"]);
		File.WriteAllText((string)jsonNode["outputPath"] + "/order.sh", "#!/bin/bash\r\n");
		
		foreach (string key in outputBuilders.Keys)
		{
			outputBuilders[key].Append((string)jsonNode["sourcePath"] + $"/{key}");
			
			File.AppendAllText((string)jsonNode["outputPath"] + "/order.sh", outputBuilders[key].ToString() + "\r\n");
		}
	}
}
