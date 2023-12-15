using System;

namespace VirtualPaymentService.Business.Service.Helpers
{
    public static class DateUtil
    {
        /// <summary>
        /// Calculates start of date, authexpireDays in the past.
        /// Class and method are in place, temporarily, until asynchronous process gets built out.
        /// </summary>
        /// <param name="authExpireDays"></param>
        /// <returns>
        /// DateTime of UTC midnight for authExpireDays + 1 in the past, then returns as localTime conversion.
        /// For MST time (Prog current location), this places result always authExpireDays(Days) in the past,
        /// but at 5pm or 6pm, depending on DaylightSavingsTime. 
        /// UTC midnight (5pm or 6pm MST) is trigger for switching to next day.
        /// </returns>
        public static DateTime GetStartDateByAuthExpireDays(int authExpireDays, DateTime now)
        {
            // All Auth data records are currently stored in MST format for:
            // 'AuthorizationDateTime' (which this method's results ultimately key off of)
            // Leaving as UTC format to:
            //   1. Match existing functionality
            //   2. Functionality to work as expected, if still in use, when transition DateTime data storage to UTC format.
            // This calculation is a little odd and will likely have logic refactored after all consumers on VPO/VPS path
            // for vcard creation.

            var utcNow = now.ToUniversalTime();
            var utcBegin = utcNow.AddDays(-authExpireDays + 1).Date;
            return utcBegin.ToLocalTime();
        }

        
    }
}
