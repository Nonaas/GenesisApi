using GenesisApi.Models;

namespace GenesisApi.Interfaces
{
    public interface ITableParserService
    {
        List<ParsedTableRow> ParseTableContent(string rawContent);
    }
}