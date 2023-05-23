using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using Witsml.Data;
using Witsml.Data.MudLog;
using Witsml.Data.Rig;
using Witsml.Data.Tubular;

using WitsmlExplorer.Api.Jobs.Common;

namespace WitsmlExplorer.Api.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EntityType
    {
        Well,
        Wellbore,
        BhaRun,
        FormationMarker,
        Log,
        Message,
        MudLog,
        Rig,
        Risk,
        Tubular,
        Trajectory,
        WbGeometry,
    }

    public static class EntityTypeHelper
    {
        public static Dictionary<EntityType, string> ToPluralLowercase()
        {
            return Enum.GetValues(typeof(EntityType))
               .Cast<EntityType>()
               .ToDictionary(
                entityType => entityType,
               entityType =>
               {
                   string lower = entityType.ToString().ToLowerInvariant();
                   return lower.Last() == 'y' ? lower.Remove(lower.Length - 1) + "ies" : lower + "s";
               });
        }

        public static WitsmlObjectOnWellbore ToObjectOnWellbore(EntityType type)
        {
            return type switch
            {
                EntityType.BhaRun => new WitsmlBhaRun(),
                EntityType.FormationMarker => new WitsmlFormationMarker(),
                EntityType.Log => new WitsmlLog(),
                EntityType.Message => new WitsmlMessage(),
                EntityType.MudLog => new WitsmlMudLog(),
                EntityType.Rig => new WitsmlRig(),
                EntityType.Risk => new WitsmlRisk(),
                EntityType.Tubular => new WitsmlTubular(),
                EntityType.Trajectory => new WitsmlTrajectory(),
                EntityType.WbGeometry => new WitsmlWbGeometry(),
                EntityType.Well => null,
                EntityType.Wellbore => null,
                _ => null,
            };
        }

        public static WitsmlObjectOnWellbore FromObjectReference(EntityType type, ObjectReference reference)
        {
            WitsmlObjectOnWellbore objectOnWellbore = ToObjectOnWellbore(type);
            objectOnWellbore.Uid = reference.Uid;
            objectOnWellbore.UidWellbore = reference.WellboreUid;
            objectOnWellbore.UidWell = reference.WellUid;
            return objectOnWellbore;
        }

        public static IWitsmlObjectList ToObjectList(EntityType type)
        {
            return type switch
            {
                EntityType.BhaRun => new WitsmlBhaRuns(),
                EntityType.FormationMarker => new WitsmlFormationMarkers(),
                EntityType.Log => new WitsmlLogs(),
                EntityType.Message => new WitsmlMessages(),
                EntityType.MudLog => new WitsmlMudLogs(),
                EntityType.Rig => new WitsmlRigs(),
                EntityType.Risk => new WitsmlRisks(),
                EntityType.Tubular => new WitsmlTubulars(),
                EntityType.Trajectory => new WitsmlTrajectories(),
                EntityType.WbGeometry => new WitsmlWbGeometrys(),
                EntityType.Well => null,
                EntityType.Wellbore => null,
                _ => null,
            };
        }
    }
}
