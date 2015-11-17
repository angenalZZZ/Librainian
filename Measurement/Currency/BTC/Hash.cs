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
// "Librainian/Hash.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM
#endregion License & Information

namespace Librainian.Measurement.Currency.BTC {
    using System;
    using System.Linq;

    /// <summary></summary>
    /// <see cref="http://github.com/mb300sd/Bitcoin-Tool" />
    public class Hash {
        public readonly Byte[] HashBytes;

        public Hash(Byte[] b) {
            this.HashBytes = b;
        }

        public Byte this[ Int32 i ] {
            get {
                return this.HashBytes[ i ];
            }
            set {
                this.HashBytes[ i ] = value;
            }
        }

        public static implicit operator Byte[] (Hash h) => h.HashBytes;

        public static implicit operator Hash(Byte[] b) => new Hash( b );

        public override Boolean Equals(Object h) {
            var hash1 = h as Hash;
            return ( hash1 != null ) && this.HashBytes.SequenceEqual( hash1.HashBytes );
        }

        public override Int32 GetHashCode() {
            if ( this.HashBytes.Length >= 4 ) {
                return ( this.HashBytes[ 0 ] << 24 ) | ( this.HashBytes[ 1 ] << 16 ) | ( this.HashBytes[ 2 ] << 8 ) | ( this.HashBytes[ 3 ] << 0 );
            }
            return this.HashBytes.GetHashCode();
        }
    }
}