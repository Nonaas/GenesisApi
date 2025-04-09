using GenesisApi.Models;

namespace GenesisApi.Interfaces
{
    public interface ITableParser
    {
        List<ParsedTableRow> ParseTableContent(string rawContent);
    }
}