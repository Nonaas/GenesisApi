using System.ComponentModel.DataAnnotations;

namespace GenesisApi.Models
{
    public class TableRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string TableCode { get; set; } = string.Empty;

        public string Area { get; set; } = "all";

        public string Language { get; set; } = "de";

        public string? StartYear { get; set; }

        public string? EndYear { get; set; }
    }
}
