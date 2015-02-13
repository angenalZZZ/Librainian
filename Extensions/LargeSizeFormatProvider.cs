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
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// "Librainian/LargeSizeFormatProvider.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM

namespace Librainian.Extensions {
    using System;
    using JetBrains.Annotations;
    using Maths;

    public class LargeSizeFormatProvider : IFormatProvider, ICustomFormatter {
        private const String FileSizeFormat = "fs";

        public String Format( String format, object arg, IFormatProvider formatProvider ) {
            if ( format == null || !format.StartsWith( FileSizeFormat ) ) {
                return DefaultFormat( format, arg, formatProvider );
            }

            if ( arg is String ) {
                return DefaultFormat( format, arg, formatProvider );
            }

            UInt64 size;
            try {
                size = Convert.ToUInt64( arg );
            }
            catch ( InvalidCastException) {
                return DefaultFormat( format, arg, formatProvider );
            }

            var suffix = "n/a";
            if ( size.Between( MathExtensions.OneTeraByte, UInt64.MaxValue ) ) {
                size /= MathExtensions.OneTeraByte;
                suffix = "trillion";
            }
            else if ( size.Between( MathExtensions.OneGigaByte, MathExtensions.OneTeraByte ) ) {
                size /= MathExtensions.OneGigaByte;
                suffix = "billion";
            }
            else if ( size.Between( MathExtensions.OneMegaByte, MathExtensions.OneGigaByte ) ) {
                size /= MathExtensions.OneMegaByte;
                suffix = "million";
            }
            else if ( size.Between( MathExtensions.OneKiloByte, MathExtensions.OneMegaByte ) ) {
                size /= MathExtensions.OneKiloByte;
                suffix = "thousand";
            }
            else if ( size.Between( UInt64.MinValue, MathExtensions.OneKiloByte ) ) {
                suffix = "";
            }

            return String.Format( "{0:N0} {1}", size, suffix );
        }

        public object GetFormat( [NotNull] Type formatType ) {
            if ( formatType == null ) {
                throw new ArgumentNullException( nameof( formatType ) );
            }
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        private static String DefaultFormat( String format, object arg, IFormatProvider formatProvider ) {
            var formattableArg = arg as IFormattable;
            return formattableArg?.ToString( format, formatProvider ) ?? arg.ToString();
        }
    }
}