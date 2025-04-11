namespace GenesisApi.Models
{
    public class TableData
    {
        public string? TableId { get; set; }
        public string? Title { get; set; }
        public List<Column>? Columns { get; set; }
        public List<Dictionary<string, object>>? Data { get; set; }
        public string? Source { get; set; }
        public string? DateStand { get; set; }
    }
}
