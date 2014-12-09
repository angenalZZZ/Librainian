#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/TeraElectronVolts.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Physics {
    using System;
    using System.Diagnostics;
    using Annotations;
    using Librainian.Extensions;

    /// <summary>
    ///     Units of mass and energy in <see cref="TeraElectronVolts" />.
    /// </summary>
    /// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
    /// <see cref="http://wikipedia.org/wiki/SI_prefix" />
    /// <see cref="http://wikipedia.org/wiki/Giga-" />
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Immutable]
    public struct TeraElectronVolts : IComparable< MilliElectronVolts >, IComparable< ElectronVolts >, IComparable< MegaElectronVolts >, IComparable< TeraElectronVolts > {
        private const  Decimal InOneElectronVolt = 1E-12m;

        private const  Decimal InOneGigaElectronVolt = 1E-3m;

        private const  Decimal InOneKiloElectronVolt = 1E-9m;

        private const  Decimal InOneMegaElectronVolt = 1E-6m;

        private const  Decimal InOneMilliElectronVolt = 1E-15m;

        private const  Decimal InOneTeraElectronVolt = 1E0m;

        /// <summary>
        ///     About 79228162514264337593543950335.
        /// </summary>
        public static readonly TeraElectronVolts MaxValue = new TeraElectronVolts(Decimal.MaxValue );

        /// <summary>
        ///     About -79228162514264337593543950335.
        /// </summary>
        public static readonly TeraElectronVolts MinValue = new TeraElectronVolts(Decimal.MinValue );

        /// <summary>
        /// </summary>
        public static readonly TeraElectronVolts One = new TeraElectronVolts( 1 );

        /// <summary>
        /// </summary>
        public static readonly TeraElectronVolts Zero = new TeraElectronVolts( 0 );

        /// <summary>
        /// </summary>
        public readonly  Decimal Value;

        public TeraElectronVolts(Decimal units ) : this() {
            this.Value = units;
        }

        public TeraElectronVolts( GigaElectronVolts gigaElectronVolts ) : this() {
            this.Value = gigaElectronVolts.ToTeraElectronVolts().Value;
        }

        public TeraElectronVolts( MegaElectronVolts megaElectronVolts ) : this() {
            this.Value = megaElectronVolts.ToTeraElectronVolts().Value;
        }

        [UsedImplicitly]
        private String DebuggerDisplay => this.Display();

        public int CompareTo( ElectronVolts other ) => this.Value.CompareTo( other.ToTeraElectronVolts().Value );

        public int CompareTo( MegaElectronVolts other ) => this.Value.CompareTo( other.ToTeraElectronVolts().Value );

        public int CompareTo( MilliElectronVolts other ) => this.Value.CompareTo( other.ToTeraElectronVolts().Value );

        public int CompareTo( TeraElectronVolts other ) => this.Value.CompareTo( other.Value );

        public static TeraElectronVolts operator *( TeraElectronVolts left,Decimal right ) => new TeraElectronVolts( left.Value*right );

        public static TeraElectronVolts operator *(Decimal left, TeraElectronVolts right ) => new TeraElectronVolts( left*right.Value );

        public static TeraElectronVolts operator +( GigaElectronVolts left, TeraElectronVolts right ) => new TeraElectronVolts( left.ToTeraElectronVolts().Value + right.Value );

        public static TeraElectronVolts operator +( TeraElectronVolts left, TeraElectronVolts right ) => new TeraElectronVolts( left.Value + right.Value );

        public static Boolean operator <( TeraElectronVolts left, TeraElectronVolts right ) => left.Value.CompareTo( right.Value ) < 0;

        public static Boolean operator >( TeraElectronVolts left, TeraElectronVolts right ) => left.Value.CompareTo( right.Value ) > 0;

        public String Display() => String.Format( "{0} TeV", this.Value );

        public ElectronVolts ToElectronVolts() => new ElectronVolts( this.Value*InOneElectronVolt );

        public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( this.Value*InOneGigaElectronVolt );

        public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( this.Value*InOneKiloElectronVolt );

        public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( this.Value*InOneMegaElectronVolt );

        public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( this.Value*InOneMilliElectronVolt );

        public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value*InOneTeraElectronVolt );
    }
}
