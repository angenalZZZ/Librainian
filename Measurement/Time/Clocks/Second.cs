#region License & Information

// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Second.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM
#endregion License & Information

namespace Librainian.Measurement.Time.Clocks {
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using Extensions;
    using FluentAssertions;

    /// <summary>A simple struct for a <see cref="Second" />.</summary>
    [DataContract( IsReference = true )]
    [Serializable]
    [Immutable]
    public sealed class Second : IClockPart {
        public static readonly Byte[] ValidSeconds = Enumerable.Range( 0, Seconds.InOneMinute ).Select( i => ( Byte )i ).OrderBy( b => b ).ToArray();

        /// <summary>should be 59</summary>
        public static readonly Byte MaximumValue = ValidSeconds.Max();

        /// <summary>should be 0</summary>
        public static readonly Byte MinimumValue = ValidSeconds.Min();

        public static readonly Second Maximum = new Second( MaximumValue );
        public static readonly Second Minimum = new Second( MinimumValue );

        [DataMember]
        public readonly Byte Value;

        static Second() {
            MaximumValue.Should().BeGreaterThan( MinimumValue );
        }

        public Second(Byte value) {
            if ( !ValidSeconds.Contains( value ) ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." );
            }
            this.Value = value;
        }

        /// <summary>Allow this class to be visibly cast to a <see cref="SByte" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator SByte(Second value) => ( SByte )value.Value;

        /// <summary>Allow this class to be read as a <see cref="Byte" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Byte(Second value) => value.Value;

        /// <summary>Provide the next second.</summary>
        public Second Next(out Boolean ticked) {
            ticked = false;
            var next = this.Value + 1;
            if ( next > MaximumValue ) {
                next = MinimumValue;
                ticked = true;
            }
            return new Second( ( Byte )next );
        }

        /// <summary>Provide the previous second.</summary>
        public Second Previous(out Boolean ticked) {
            ticked = false;
            var next = this.Value - 1;
            if ( next < MinimumValue ) {
                next = MaximumValue;
                ticked = true;
            }
            return new Second( ( Byte )next );
        }
    }
}