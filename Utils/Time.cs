using System;
using System.Globalization;

using Microsoft.SPOT;


namespace uk.andyjohnson0.Netduino.Utils
{
    /// <summary>
    /// Time zone factory class. Returns a TimeZone object for a supplied time zone identifier.
    /// </summary>
    public static class TimeZoneFactory
    {
        /// <summary>
        /// Returns a TimeZone object for a supplied time zone identifier.
        /// </summary>
        /// <param name="tzId">
        /// Time zone identifier.
        /// Currently the only supported time zone ID is Microsoft.SPOT.TimeZoneId.London.
        /// </param>
        /// <returns>a TimeZone object for the supplied time zone identifier.</returns>
        /// <exception cref="NotImplementedException">
        /// TimeZone identifier not supported.
        /// The current implementation supports only Microsoft.SPOT.TimeZoneId.London.
        /// </exception>
        public static TimeZone Create(TimeZoneId tzId)
        {
            switch (tzId)
            {
                case TimeZoneId.London:
                    return new TimeZone_Uk();
                default:
                    throw new NotImplementedException("Time zone not supported");
            }
        }
    }


    /// <summary>
    /// Implementation of System.Timezone for the UK.
    /// </summary>
    internal class TimeZone_Uk : TimeZone
    {
        /// <summary>
        /// Constructor. Initialise a TimeZone_Uk object.
        /// </summary>
        internal TimeZone_Uk()
        {
        }


        /// <summary>
        /// Gets the daylight saving time zone name.
        /// </summary>
        public override string DaylightName
        {
            get { return "GMT Daylight Time"; }
        }

        /// <summary>
        /// Gets the standard time zone name.
        /// </summary>
        public override string StandardName
        {
            get { return "GMT Standard Time"; }
        }


        /// <summary>
        /// Returns the daylight saving time period for a particular year.
        /// </summary>
        /// <param name="year">The year that the daylight saving time period applies to.</param>
        /// <returns>
        /// A System.Globalization.DaylightTime object that contains the start and end date for daylight saving time in year.
        /// </returns>
        public override DaylightTime GetDaylightChanges(int year)
        {
            // British Summer Time operates from the last Sunday in March until the last Sunday in October.
            DateTime start = new DateTime(year, 03, 31, 01, 00, 00);
            while (start.DayOfWeek != DayOfWeek.Sunday)
                start = start.AddDays(-1);
            DateTime end = new DateTime(year, 10, 31, 01, 00, 00);
            while (end.DayOfWeek != DayOfWeek.Sunday)
                end = end.AddDays(-1);
            return new DaylightTime(start, end, new TimeSpan(1, 0, 0));
        }


        /// <summary>
        /// Returns the Coordinated Universal Time (UTC) offset for the specified local time.
        /// See notes at http://msdn.microsoft.com/en-us/library/system.timezone.getutcoffset.aspx
        /// </summary>
        /// <param name="time">A date and time value.</param>
        /// <returns>The Coordinated Universal Time (UTC) offset from the supplied time.</returns>
        public override TimeSpan GetUtcOffset(DateTime time)
        {
            switch (time.Kind)
            {
                case DateTimeKind.Local:
                    DaylightTime changes = GetDaylightChanges(time.Year);
                    if ((time >= changes.Start) && (time <= changes.End))
                        return changes.Delta;
                    else
                        return new TimeSpan(0, 0, 0);
                case DateTimeKind.Utc:
                    // THIS needs to check for local time.
                    return new TimeSpan(0, 0, 0);
                default:
                    return new TimeSpan(0, 0, 0);
            }
        }


        /// <summary>
        /// Returns a value indicating whether a specified date and time is within a daylight saving time period.
        /// </summary>
        /// <param name="time">A date and time.</param>
        /// <returns>true if time is in a daylight saving time period; otherwise, false.</returns>
        public override bool IsDaylightSavingTime(DateTime time)
        {
            DaylightTime changes = GetDaylightChanges(time.Year);
            return (time >= changes.Start) && (time <= changes.End);
        }


        /// <summary>
        /// Returns the local time that corresponds to a specified date and time value.
        /// </summary>
        /// <param name="time">A Coordinated Universal Time (UTC) time.</param>
        /// <returns>A DateTime object whose value is the local time that corresponds to the supplied time.</returns>
        public override DateTime ToLocalTime(DateTime time)
        {
            switch (time.Kind)
            {
                case DateTimeKind.Local:
                    return time;
                case DateTimeKind.Utc:
                    DaylightTime changes = GetDaylightChanges(time.Year);
                    if ((time >= changes.Start) && (time <= changes.End))
                        return DateTime.SpecifyKind(time + changes.Delta, DateTimeKind.Local);
                    else
                        return DateTime.SpecifyKind(time, DateTimeKind.Local);
                default:
                    return time + GetUtcOffset(time);
            }
        }


        /// <summary>
        /// Returns the Coordinated Universal Time (UTC) that corresponds to a specified time.
        /// </summary>
        /// <param name="time">A date and time.</param>
        /// <returns>A DateTime object whose value is the Coordinated Universal Time (UTC) that corresponds to the specified time.</returns>
        public override DateTime ToUniversalTime(DateTime time)
        {
            switch (time.Kind)
            {
                case DateTimeKind.Local:
                    return time - GetUtcOffset(time);
                case DateTimeKind.Utc:
                    return time;
                default:
                    return time;
            }
        }
    }
}
