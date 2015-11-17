﻿#region License & Information

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
// "Librainian/Minus1to1TS.cs" was last cleaned by Rick on 2015/06/12 at 3:00 PM
#endregion License & Information

namespace Librainian.Maths {
    using System;
    using System.Runtime.Serialization;
    using System.Threading;

    /// <summary>
    /// Uses Interlocked to ensure thread safety and restricts the value to between -1 and 1.
    /// </summary>
    [DataContract]
    public class Minus1To1Ts : ICloneable {
        public const Double MaxValue = 1D;
        public const Double MinValue = -1D;
        private const Double NaNValue = 2D;
        private static readonly Random Rand = new Random( ( Int32 )DateTime.UtcNow.Ticks );

        /// <summary>ONLY used in the getter and setter.</summary>
        [DataMember]
        private Double _value;

        /// <summary>Initialize the value to a random value between -1 and 1.</summary>
        public Minus1To1Ts() {
            this.Value = Rand.NextDouble() - Rand.NextDouble();
        }

        /// <summary>Initialize the value to between -1 and 1.</summary>
        /// <param name="value"></param>
        public Minus1To1Ts(Double value) {
            this.Value = value;
        }

        public Double Value {
            get {
                return Interlocked.CompareExchange( ref this._value, this._value, NaNValue );
            }

            set {
                if ( value > MaxValue ) {
                    value = MaxValue;
                }
                else if ( value < MinValue ) {
                    value = MinValue;
                }
                Interlocked.CompareExchange( ref this._value, value, this._value );
            }
        }

        #region ICloneable Members
        public Object Clone() => new Minus1To1Ts( this.Value );
        #endregion ICloneable Members

        public static implicit operator Double(Minus1To1Ts special) => special.Value;

        public static implicit operator Minus1To1Ts(Double value) => new Minus1To1Ts( value );

        public override String ToString() => $"{this.Value:R}";
    }
}