using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business
{
    /// <summary>
    /// Expire Time Enumerator
    /// </summary>
    public enum ExpireAfter
    {
        /// <summary>
        /// Data stored in  cache always persists
        /// </summary>
        Infinite = 0,
        /// <summary>
        /// Data is stored in cache for 1 second
        /// </summary>
        OneSecond = 1,
        /// <summary>
        /// Data is stored in cache for 5 seconds
        /// </summary>
        FiveSeconds = 5,
        /// <summary>
        /// Data is stored in cache for 10 seconds
        /// </summary>
        TenSeconds = 10,
        /// <summary>
        /// Data is stored in cache for 30 seconds
        /// </summary>
        ThirtySeconds = 30,
        /// <summary>
        /// Data is stored in cache for 1 minute
        /// </summary>
        OneMinute = 1 * 60,
        /// <summary>
        /// Data is stored in cache for 5 minutes
        /// </summary>
        FiveMinutes = 5 * 60,
        /// <summary>
        /// Data is stored in cache for 10 minutes
        /// </summary>
        TenMinutes = 10 * 60,
        /// <summary>
        /// Data is stored in cache for 1 hour
        /// </summary>
        OneHour = 60 * 60,
        /// <summary>
        /// Data is stored in cache for 8 hours
        /// </summary>
        EightHour = 8 * 60 * 60,
        /// <summary>
        /// Data is stored in cache for 1 day
        /// </summary>
        OneDay = 24 * 60 * 60,
        /// <summary>
        /// Data is stored in cache for 1 week
        /// </summary>
        OneWeek = 7 * 24 * 60 * 60,
        /// <summary>
        /// Data is stored in cache for 1 month
        /// </summary>
        OneMonth = 30 * 24 * 60 * 60,
        /// <summary>
        /// Data is stored in cache for 6 months
        /// </summary>
        SixMonths = 6 * 30 * 24 * 60 * 60,
        /// <summary>
        /// Data is stored in cache for 360 days
        /// </summary>
        OneYear = 365 * 24 * 60 * 60
    }
}
