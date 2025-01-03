using System.Text.Json.Serialization;

namespace ClinicalTrial.Domain.Entities
{
    public class ClinicalTrial : BaseEntity
    {
        public string TrialId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Participants { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public string Status { get; set; }
        public int DurationInDays { get; set; }
    }
}
