﻿// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/mLTC.cs" was last cleaned by Rick on 2014/12/09 at 10:23 AM

namespace Librainian.Measurement.Currency.LTC {
    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;

	// ReSharper disable once UseNameofExpression
	[DebuggerDisplay( "{Formatted,nq}" )]
    [UsedImplicitly]
    public struct mLTC : ICoin {

        public Decimal FaceValue => 0.001M;

        public String Formatted => String.Format( "Ł{0:f8}", this.FaceValue );
    }
}