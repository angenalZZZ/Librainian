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
// "Librainian/MilliElectronVolts.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Physics {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Librainian.Extensions;

    /// <summary>Units of mass and energy in Thousandths of <see cref="ElectronVolts" />.</summary>
    /// <seealso cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
    /// <seealso cref="http://wikipedia.org/wiki/SI_prefix" />
    /// <seealso cref="http://wikipedia.org/wiki/Milli-" />
    /// <seealso cref="http://wikipedia.org/wiki/Electronvolt" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct MilliElectronVolts : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<MegaElectronVolts>, IComparable<GigaElectronVolts> {
        public const Decimal InOneElectronVolt = 1E3m;

        public const Decimal InOneGigaElectronVolt = 1E12m;

        public const Decimal InOneKiloElectronVolt = 1E6m;

        public const Decimal InOneMegaElectronVolt = 1E9m;

        public const Decimal InOneMilliElectronVolt = 1E0m;

        public const Decimal InOneTeraElectronVolt = 1E15m;

        /// <summary>About 79228162514264337593543950335.</summary>
        public static readonly MilliElectronVolts MaxValue = new MilliElectronVolts( Decimal.MaxValue );

        /// <summary>About -79228162514264337593543950335.</summary>
        public static readonly MilliElectronVolts MinValue = new MilliElectronVolts( Decimal.MinValue );

        public static readonly MilliElectronVolts Zero = new MilliElectronVolts( 0 );

        public readonly Decimal Value;

        public MilliElectronVolts( Decimal units ) : this() => this.Value = units;

	    public static implicit operator ElectronVolts( MilliElectronVolts milliElectronVolts ) => milliElectronVolts.ToElectronVolts();

        public static Boolean operator <( MilliElectronVolts left, MilliElectronVolts right ) => left.Value.CompareTo( right.Value ) < 0;

        public static Boolean operator >( MilliElectronVolts left, MilliElectronVolts right ) => left.Value.CompareTo( right.Value ) > 0;

        public Int32 CompareTo( ElectronVolts other ) => this.ToElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo( GigaElectronVolts other ) => this.ToGigaElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo( MegaElectronVolts other ) => this.ToMegaElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo( MilliElectronVolts other ) => this.Value.CompareTo( other.Value );

        public Int32 CompareTo( KiloElectronVolts other ) => this.ToKiloElectronVolts().Value.CompareTo( other.Value );

        public String Simpler() {
            var list = new HashSet<String> { this.ToTeraElectronVolts().ToString(), this.ToGigaElectronVolts().ToString(), this.ToMegaElectronVolts().ToString(), this.ToElectronVolts().ToString(), this.ToMilliElectronVolts().ToString() };
            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public ElectronVolts ToElectronVolts() => new ElectronVolts( this.Value * InOneElectronVolt );

        public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( this.Value * InOneGigaElectronVolt );

        public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( this.Value * InOneKiloElectronVolt );

        public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( this.Value * InOneMegaElectronVolt );

        public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( this.Value * InOneMilliElectronVolt );

        public override String ToString() => $"{this.Value} meV";

	    public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value * InOneTeraElectronVolt );
    }
}