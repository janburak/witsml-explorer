using System.Text.Json.Serialization;

namespace WitsmlExplorer.Api.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EntityType
    {
        Well,
        Wellbore,
        LogObject,
        MessageObjects,
        Rigs,
        Risks,
        Tubular,
        Trajectory,
        TrajectoryStation
    }
}
