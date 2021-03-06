// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Time.cs" was last cleaned by Rick on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time {

    using System;
    using Clocks;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para></para>
    /// </summary>
    [JsonObject]
    [Immutable]
    public struct Time {
        public static Time Zero = new Time( Hour.Minimum, Minute.Minimum, Second.Minimum, Millisecond.Minimum );

        /// <summary>
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="millisecond"></param>
        /// <param name="microsecond"></param>
        public Time( Byte hour = 0, Byte minute = 0, Byte second = 0, UInt16 millisecond = 0, UInt16 microsecond = 0 ) : this() {
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.Millisecond = millisecond;
            this.Microsecond = microsecond;
        }

        /// <summary>
        /// </summary>
        /// <param name="dateTime"></param>
        public Time( DateTime dateTime ) : this( hour: ( Byte )dateTime.Hour, minute: ( Byte )dateTime.Minute, second: ( Byte )dateTime.Second, millisecond: ( UInt16 )dateTime.Millisecond ) { }

        /// <summary>
        /// </summary>
        /// <param name="span"></param>
        public Time( Span span ) : this( hour: ( Byte )span.Hours.Value, minute: ( Byte )span.Minutes.Value, second: ( Byte )span.Seconds.Value, millisecond: ( UInt16 )span.Milliseconds.Value, microsecond: ( UInt16 )span.Microseconds.Value ) { }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Hour Hour {
            get;
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Microsecond Microsecond {
            get;
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Millisecond Millisecond {
            get;
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Minute Minute {
            get;
        }

        /// <summary>
        /// </summary>
        [JsonProperty]
        public Second Second {
            get;
        }

        public static explicit operator Time( DateTime dateTime ) => new Time( ( Byte )dateTime.Hour, ( Byte )dateTime.Minute, ( Byte )dateTime.Second, ( UInt16 )dateTime.Millisecond );

	    /// <summary>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static implicit operator DateTime( Time date ) => new DateTime( year: DateTime.MinValue.Year, month: DateTime.MinValue.Month, day: DateTime.MinValue.Day, hour: date.Hour.Value, minute: date.Minute.Value, second: date.Second.Value, millisecond: date.Millisecond.Value );

        /// <summary>
        ///     Get the local system's computer time.
        /// </summary>
        public static Time Now() {
            var now = DateTime.Now;
            return new Time( hour: ( Byte )now.Hour, minute: ( Byte )now.Minute, second: ( Byte )now.Second, millisecond: ( UInt16 )now.Millisecond );
        }

        public static Time UtcNow() {
            var now = DateTime.UtcNow;
            return new Time( hour: ( Byte )now.Hour, minute: ( Byte )now.Minute, second: ( Byte )now.Second, millisecond: ( UInt16 )now.Millisecond );
        }
    }
}