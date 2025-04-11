using GenesisApi.Interfaces;
using GenesisApi.Models;
using Newtonsoft.Json;

namespace GenesisApi.Services
{
    public class TableParserService : ITableParserService
    {
        public string ParseTableContentToJson(string rawContent)
        {
            var lines = rawContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(line => line.Trim())
                            .Where(line => !string.IsNullOrEmpty(line))
                            .ToArray();

            var result = new TableData();
            int lineIndex = 0;

            // Parse Table ID
            if (lineIndex < lines.Length && lines[lineIndex].StartsWith("Tabelle:"))
            {
                result.TableId = lines[lineIndex++].Split(new[] { ':' }, 2)[1].Trim();
            }

            // Parse Title
            var titleLines = new List<string>();
            while (lineIndex < lines.Length && lines[lineIndex].Contains(";;"))
            {
                titleLines.Add(lines[lineIndex++].TrimEnd(';'));
            }
            result.Title = string.Join(" ", titleLines);

            // Parse Column Headers
            if (lineIndex < lines.Length && lines[lineIndex].StartsWith(";"))
            {
                var headerLines = new List<string[]>();

                // Capture both header rows
                for (int i = 0; i < 2; i++)
                {
                    headerLines.Add(lines[lineIndex++].Split(';').Skip(1).ToArray());
                }

                result.Columns = new List<Column>();
                for (int i = 0; i < headerLines[0].Length; i++)
                {
                    var mainHeader = headerLines[0][i].Trim();
                    var subHeader = headerLines[1][i].Trim();

                    result.Columns.Add(new Column
                    {
                        Name = $"{mainHeader}",
                        Unit = subHeader.Contains("(") ?
                               subHeader.Split('(')[1].Trim(')') :
                               subHeader,
                        Measurement = subHeader.Contains(")") ?
                                    subHeader.Split(')')[0].Trim() + ")" :
                                    null
                    });
                }
            }

            // Parse Data Rows
            result.Data = new List<Dictionary<string, object>>();
            while (lineIndex < lines.Length && !lines[lineIndex].StartsWith("__________"))
            {
                var parts = lines[lineIndex++].Split(';');
                if (parts.Length < result.Columns.Count + 1) continue;

                var entry = new Dictionary<string, object>();
                entry["Year"] = parts[0].Trim();

                for (int i = 0; i < result.Columns.Count; i++)
                {
                    var col = result.Columns[i];
                    var value = parts[i + 1].Trim().TrimStart('+');

                    entry[col.Name] = Utilities.Extensions.ParseDouble(value) ?? (object)value;
                }

                result.Data.Add(entry);
            }

            // Parse Footer
            if (lineIndex < lines.Length && lines[lineIndex].StartsWith("__________"))
            {
                lineIndex++;
            }

            if (lineIndex < lines.Length)
            {
                result.Source = lines[lineIndex++];
            }

            if (lineIndex < lines.Length && lines[lineIndex].StartsWith("Stand:"))
            {
                var standParts = lines[lineIndex].Split(new[] { "Stand: " }, StringSplitOptions.None);
                if (standParts.Length > 1)
                {
                    result.DateStand = standParts[1].Trim();
                }
            }

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        

    }
}
