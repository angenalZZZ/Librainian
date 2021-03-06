﻿// Copyright 2016 Rick@AIBrain.org.
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
// "Librainian/WhenRange.cs" was last cleaned by Rick on 2016/06/18 at 10:55 PM

namespace Librainian.Measurement.Time {

    using Newtonsoft.Json;

    /// <summary>
    ///     Represents a <see cref="UniversalDateTime" /> range with minimum and maximum values.
    /// </summary>
    [JsonObject]
    public struct WhenRange {

        /// <summary>Length of the range (difference between maximum and minimum values).</summary>
        [JsonProperty]
        public readonly Span Length;

        /// <summary>Maximum value</summary>
        [JsonProperty]
        public readonly UniversalDateTime Max;

        /// <summary>Minimum value</summary>
        [JsonProperty]
        public readonly UniversalDateTime Min;

        /// <summary>Initializes a new instance of the <see cref="WhenRange" /> class</summary>
        /// <param name="min">Minimum value of the range</param>
        /// <param name="max">Maximum value of the range</param>
        public WhenRange( UniversalDateTime min, UniversalDateTime max ) {
            if ( min < max ) {
                this.Min = min;
                this.Max = max;
            }
            else {
                this.Min = max;
                this.Max = min;
            }
            var δ = this.Max.Value - this.Min.Value;
            this.Length = new Span( planckTimes: δ );
        }
    }
}