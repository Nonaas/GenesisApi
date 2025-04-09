using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GenesisApi.Models
{
    public class TableRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string TableCode { get; set; } = string.Empty;

        [SwaggerSchema(Nullable = true)]
        public string Area { get; set; } = "all";

        [SwaggerSchema(Nullable = true)]
        public string Language { get; set; } = "de";

        [SwaggerSchema(Nullable = true)]
        public string? StartYear { get; set; }

        [SwaggerSchema(Nullable = true)]
        public string? EndYear { get; set; }
    }
}
