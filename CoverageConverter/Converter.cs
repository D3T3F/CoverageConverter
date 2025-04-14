using System.Text;
using System.Text.RegularExpressions;

namespace CoverageConverter;

internal class Converter
{
	private class Coverage
	{
		internal int Id { get; set; } = 0;
		internal string Name { get; set; } = "";
		internal string Description { get; set; } = "";
		internal string RegexCoberturas { get; set; } = "";
	}

	private List<Coverage> CoverageList()
	{
		var path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory.ToString()) + "\\banco.txt";

		var text = File.ReadAllText(path ?? "");

		var lines = text.Split(['\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		var coverages = new List<Coverage>();

		foreach (var line in lines)
		{
			if (line.Contains("|"))
			{
				var data = line.Split(['|'], StringSplitOptions.TrimEntries);

				if (data.Length > 1)
				{
					var coverage = new Coverage();

					if (int.TryParse(data[0].Trim(), out int id)) coverage.Id = id;

					coverage.Description = data[1].Trim();

					var auxRegex = Regex.Replace(data[1], @"e\/ou|\s-\s|,\s|\s–\s|\s\(.+\)", ".+").Replace("/", @".?\s?");

					auxRegex = Regex.Replace(auxRegex, @"[^\x00-\x7F]|-|–", ".").Trim();

					coverage.RegexCoberturas = auxRegex;

					var nameAux = data[1].Split([' ', '/', '-', '–', ',', '.'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

					var name = nameAux.ToList();

					name.RemoveAll(x => x.Length < 3);

					if (name.Count == 0) name = nameAux.ToList();

					for (int i = 0; i < name.Count; i++)
					{
						if (i == 3) break;

						var auxString = name[i].ToLower().Trim();

						auxString = auxString.Substring(0, 1).ToUpper() + auxString.Substring(1);

						coverage.Name += auxString;
					}

					coverages.Add(coverage);
				}
			}
		}

		return coverages;
	}

	private string ReturnString()
	{
		var coverages = CoverageList();

		var output = "";

		foreach (var coverage in coverages)
		{
			output += $"\n[Description(\"{coverage.Description}\")]\n[RegexCobertura(@\"{coverage.RegexCoberturas}\")]\n{coverage.Name} = {coverage.Id},\n";
		}

		return output;
	}

	private void SaveCoverages(string coverages)
	{
		var path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory.ToString()) + "\\coverages.txt";

		File.WriteAllText(path, coverages, Encoding.UTF8);
	}

	internal void ConvertCoverages()
	{
		Console.WriteLine("Convertendo coberturas...");

		var coverages = ReturnString();

		Console.WriteLine("\nCoberturas convertidas com sucesso!");

		Console.WriteLine("\nSalvando coberturas...");

		SaveCoverages(coverages);

		Console.WriteLine("\nCoberturas salvas com sucesso no arquivo coverages.txt!");

		Console.WriteLine("");

		Console.WriteLine(coverages);
	}
}

