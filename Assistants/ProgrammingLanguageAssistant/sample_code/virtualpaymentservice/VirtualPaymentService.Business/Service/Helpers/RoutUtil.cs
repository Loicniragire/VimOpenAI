using System;
using System.Text;
using System.Text.RegularExpressions;

namespace VirtualPaymentService.Business.Service.Helpers
{
    public static class RouteUtil
    {
        /// <summary>
        /// Removes duplicate forward slashes. Accepts 0-n number of entityIds
        /// </summary>
        /// <remarks>
        /// Example:
        /// <code>
        /// var route = "/myResourcePath/{someEntityId}/lease/{someLeaseId}"; <br/>
        /// var entityId1 = 1234; <br/>
        /// var leaseId = 9876; <br/>
        /// <br/>
        /// // Result is: "myResourcePath/1234/lease/9876" <br/>
        /// var formattedRoute = RouteUtil.FormatRoute(route, entityId1, leaseId);
        /// </code>
        /// 
        /// If the entity id is being added to the very end of the route, it is not a requirement that the {entityId} marker is included. <br/>
        /// I.e. Had the route in the above example been equal to: <c>"/myResourcePath/{someEntityId}/lease/"</c>, <br/>
        /// the result would have still been <c>"myResourcePath/1234/lease/9876"</c>.
        /// </remarks>
        /// <param name="route">The endpoint of the service.</param>
        /// <param name="entityIds">entityIds to insert into or append to the route.</param>
        /// <returns>Combined endpoint and entityIds.</returns>
        public static string FormatRoute(string route, params string[] entityIds)
        {
            var cleanRoute = CleanRoute(route);

            var regexPattern = @"{.+?}";
            var splitMatches = Regex.Split(cleanRoute, regexPattern);

            var insertLocationCount = splitMatches.Length - 1;
            var entityCount = entityIds.Length;
            var maxAllowedEntities = insertLocationCount + 1;

            // Check if there too few or too many entities provided.
            if (insertLocationCount > entityCount || entityCount > maxAllowedEntities)
            {
                throw new ArgumentOutOfRangeException($"{nameof(entityIds)}", $"There was/were {entityCount} entityId(s) provided, {insertLocationCount} expected");
            }

            // Check if there are any entity id's to add. If not, return the clean cleanRoute and return.
            if (entityCount == 0)
            {
                return cleanRoute;
            }

            // Check if there are any insert locations, if not, add the entityId to the end of the cleanRoute and return.
            if (insertLocationCount == 0)
            {
                return string.Concat(cleanRoute.AsSpan(), "/", entityIds[0].AsSpan());
            }

            // Loop through the entityId's and replace each location with the correct value.
            var index = 0;
            var newRoute = new StringBuilder();
            foreach (var match in splitMatches)
            {
                if (index == insertLocationCount)
                {
                    newRoute.Append(match.AsSpan());
                    break;
                }

                newRoute.Append(string.Concat(match.AsSpan(), entityIds[index].AsSpan()));
                index++;
            }

            // Check if there is one more entityId than there are insert locations, if so append it to the end of the cleanRoute.
            if (entityCount == maxAllowedEntities)
            {
                newRoute.Append(string.Concat("/", entityIds[entityCount - 1].AsSpan()));
            }

            return newRoute.ToString();
        }

        private static string CleanRoute(string route)
        {
            return route.TrimStart('/').TrimEnd('/');
        }
    }
}
