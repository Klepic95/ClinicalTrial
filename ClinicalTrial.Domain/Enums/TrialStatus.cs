using ClinicalTrial.Domain.Enums;
using Newtonsoft.Json;

namespace ClinicalTrial.Domain.Enums
{
    [JsonConverter(typeof(TrialStatusConverter))]
    public enum TrialStatus
    {

        NotStarted,
        Ongoing,
        Completed
    }
}

public class TrialStatusConverter : JsonConverter<TrialStatus>
{
    public override TrialStatus ReadJson(JsonReader reader, Type objectType, TrialStatus existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var stringValue = reader.Value?.ToString();
        return stringValue switch
        {
            "NotStarted" or "Not Started" => TrialStatus.NotStarted,
            "Ongoing" => TrialStatus.Ongoing,
            "Completed" => TrialStatus.Completed,
            _ => throw new JsonSerializationException($"Invalid value '{stringValue}' for TrialStatus.")
        };
    }

    public override void WriteJson(JsonWriter writer, TrialStatus value, JsonSerializer serializer)
    {
        var stringValue = value switch
        {
            TrialStatus.NotStarted => "Not Started",
            TrialStatus.Ongoing => "Ongoing",
            TrialStatus.Completed => "Completed",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };

        writer.WriteValue(stringValue);
    }
}