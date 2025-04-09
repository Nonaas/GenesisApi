using GenesisApi.Interfaces;
using GenesisApi.Models;

namespace GenesisApi.Services
{
    public class TableParserService : ITableParserService
    {
        public List<ParsedTableRow> ParseTableContent(string rawContent)
        {
            var lines = rawContent
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .ToList();

            var headerLineIndex = lines.FindIndex(l => l.StartsWith(";"));
            if (headerLineIndex < 0 || lines.Count <= headerLineIndex + 1)
            {
                throw new InvalidDataException("Table content could not be parsed: No headers found.");
            }

            var headers = lines[headerLineIndex]
                .Split(';')
                .Skip(1)
                .ToList();

            var dataLines = lines.Skip(headerLineIndex + 2)
                                 .TakeWhile(l => !l.StartsWith("__________"));

            var rows = new List<ParsedTableRow>();

            foreach (var line in dataLines)
            {
                var parts = line.Split(';');
                if (parts.Length < 2)
                {
                    continue;
                }

                var row = new ParsedTableRow { ["Label"] = parts[0] };

                for (int i = 1; i < parts.Length && i - 1 < headers.Count; i++)
                {
                    row[headers[i - 1]] = parts[i];
                }

                rows.Add(row);
            }

            return rows;
        }
    }
}
