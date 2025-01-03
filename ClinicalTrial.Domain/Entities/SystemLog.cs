
using ClinicalTrial.Domain.Enums;

namespace ClinicalTrial.Domain.Entities
{
    public class SystemLog
    {
        public string ResourceType { get; set; } 
        public DateTime CreatedAt { get; set; }
        public Event Event { get; set; }
        public object[]? ChangeSet { get; set; }
        public string? Comment { get; set; }

        public SystemLog()
        {
            ResourceType = "ClinicalTrial";
        }
    }
}
