using GenesisApi.Models;

namespace GenesisApi.Interfaces
{
    public interface ITableParserService
    {
        string ParseTableContentToJson(string rawContent);
    }
}