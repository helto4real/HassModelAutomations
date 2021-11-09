using System;
using System.Linq;
using NetDaemon.HassModel.Common;
using NetDaemon.HassModel.Entities;

namespace NetDaemon.HassModel.Extensions
{
    public static class HassModelExtensions
    {
        internal static string GetDomainFromEntity(string entity)
        {
            var entityParts = entity.Split('.');
            if (entityParts.Length != 2)
                throw new Exception($"entity_id is mal-formatted {entity}");

            return entityParts[0];
        }

        internal static string GetDomainForServiceCall(string[] entityIds)
        {
            var domainsUsed = entityIds.Select(n => GetDomainFromEntity(n));
            // get first entitys domain
            return domainsUsed.Distinct().Count() == 1 ? domainsUsed.First() : "homeassistant";

        }

        public static void TurnOn(this IHaContext ha, params string[] entityIds)
        {
            if (entityIds.Length == 0)
                throw new ArgumentNullException(nameof(entityIds));

            ha.CallService(GetDomainForServiceCall(entityIds), "turn_on", new ServiceTarget { EntityIds = entityIds });
        }

        public static void TurnOff(this IHaContext ha, params string[] entityIds)
        {
            if (entityIds.Length == 0)
                throw new ArgumentNullException(nameof(entityIds));

            ha.CallService(GetDomainForServiceCall(entityIds), "turn_off", new ServiceTarget { EntityIds = entityIds });
        }
    }
}