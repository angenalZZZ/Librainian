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
// "Librainian/ParsingExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:55 PM

namespace Librainian.Parsing {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Entity.Design.PluralizationServices;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Numerics;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Xml;
    using Collections;
    using Extensions;
    using JetBrains.Annotations;
    using Linguistics;
    using Maths;
    using Maths.Numbers;
    using Measurement.Time;
    using Numerics;
    using NUnit.Framework;
    using Threading;

	public static class ParsingExtensions {
        public const String Doublespace = Singlespace + Singlespace;

        /// <summary>
        ///     abcdefghijklmnopqrstuvwxyz
        /// </summary>
        public const String Lowercase = "abcdefghijklmnopqrstuvwxyz";

        public const String MatchMoney = @"//\$\s*[-+]?([0-9]{0,3}(,[0-9]{3})*(\.[0-9]+)?)";

        /// <summary>
        ///     0123456789
        /// </summary>
        public const String Numbers = "0123456789";

        public const String Singlespace = @" ";

        public const String SplitByEnglish = @"(?:\p{Lu}(?:\.\p{Lu})+)(?:,\s*\p{Lu}(?:\.\p{Lu})+)*";

        /// <summary>
        ///     Regex pattern for words that don't start with a number
        /// </summary>
        public const String SplitByWordNotNumber = @"([a-zA-Z]\w+)\W*";

        /// <summary> ~`!@#$%^&*()-_=+?:,./\[]{}|' </summary>
        public const String Symbols = @"~`!@#$%^&*()-_=+<>?:,./\[]{}|'";

        /// <summary>
        ///     ABCDEFGHIJKLMNOPQRSTUVWXYZ
        /// </summary>
        public const String Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static String[] OrdinalSuffixes { get; } = { "th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th" };

        public static Regex RegexByWordBreak { get; } = new Regex( pattern: @"(?=\S*(?<=\w))\b", options: RegexOptions.Compiled | RegexOptions.Singleline );

        public static Regex RegexJustDigits { get; } = new Regex( @"\D+", RegexOptions.Compiled );

        public static Char[] SpaceSplitBy { get; } = { Singlespace[ 0 ] };

        /// <summary>
        /// </summary>
        public static String[] TensMap { get; } = { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

        /// <summary>
        /// </summary>
        public static String[] UnitsMap { get; } = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };

        /// <summary>
        ///     The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public static String[] UriRfc3986CharsToEscape { get; } = { "!", "*", "'", "(", ")" };

        public static String[] Vowels { get; } = "A,AI,AU,E,EA,EE,I,IA,IO,O,OA,OI,OO,OU,U".Split( ',' );

        public static String AllLetters { get; } = new String( Enumerable.Range( UInt16.MinValue, UInt16.MaxValue ).Select( i => ( Char )i ).Distinct().Where( Char.IsLetter ).OrderBy( c => c ).ToArray() );

        [NotNull]
        public static String AllLowercaseLetters { get; } = new String( Enumerable.Range( UInt16.MinValue, UInt16.MaxValue ).Select( i => ( Char )i ).Distinct().Where( Char.IsLetter ).Where( Char.IsLower ).OrderBy( c => c ).ToArray() );

        [NotNull]
        public static String AllUppercaseLetters { get; } = new String( Enumerable.Range( UInt16.MinValue, UInt16.MaxValue ).Select( i => ( Char )i ).Distinct().Where( Char.IsLetter ).Where( Char.IsUpper ).OrderBy( c => c ).ToArray() );

        public static String[] Consonants { get; } = "B,C,CH,CL,D,F,FF,G,GH,GL,J,K,L,LL,M,MN,N,P,PH,PS,R,RH,S,SC,SH,SK,ST,T,TH,V,W,X,Y,Z".Split( ',' );

        [NotNull]
        public static String EnglishAlphabetLowercase { get; } = "abcdefghijklmnopqrstuvwxyz";

        [NotNull]
        public static String EnglishAlphabetUppercase { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        [NotNull]
        public static Lazy<PluralizationService> LazyPluralizationService { get; } = new Lazy<PluralizationService>( () => PluralizationService.CreateService( Thread.CurrentThread.CurrentCulture ) );

        public static ConcurrentDictionary<String, String> PluralCache { get; } = new ConcurrentDictionary<String, String>();

        /// <summary>
        ///     this doesn't handle apostrophe well
        /// </summary>
        public static Regex RegexBySentenceNotworking { get; } = new Regex( pattern: @"(?<=['""A-Za-z0-9][\.\!\?])\s+(?=[A-Z])", options: RegexOptions.Compiled | RegexOptions.Multiline );

        public static Regex RegexBySentenceStackoverflow { get; } = new Regex( "(?<Sentence>\\S.+?(?<Terminator>[.!?]|\\Z))(?=\\s+|\\Z)", RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled );

        /// <summary>
        ///     Add dashes to a pascal-cased String
        /// </summary>
        /// <param name="pascalCasedWord">String to convert</param>
        /// <returns>String</returns>
        public static String AddDashes( this String pascalCasedWord ) => Regex.Replace( Regex.Replace( Regex.Replace( pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1-$2" ), @"([a-z\d])([A-Z])", "$1-$2" ), @"[\s]", "-" );

        public static String AddSpacesBeforeUppercase( this String word ) {
            var sb = new StringBuilder( word.Length * 2 );
            foreach ( var c in word ) {
                if ( Char.IsUpper( c ) ) {
                    sb.Append( Singlespace );
                }
                sb.Append( c );
            }
            return sb.ToString().Trim();
        }

        /// <summary>
        ///     Add an undescore prefix to a pascasl-cased String
        /// </summary>
        /// <param name="pascalCasedWord"></param>
        /// <returns></returns>
        public static String AddUnderscorePrefix( this String pascalCasedWord ) => $"_{pascalCasedWord}";

        /// <summary>
        ///     Add underscores to a pascal-cased String
        /// </summary>
        /// <param name="pascalCasedWord">String to convert</param>
        /// <returns>String</returns>
        public static String AddUnderscores( this String pascalCasedWord ) => Regex.Replace( Regex.Replace( Regex.Replace( pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2" ), @"([a-z\d])([A-Z])", "$1_$2" ), @"[-\s]", "_" );

        public static String After( [NotNull] this String s, [NotNull] String splitter ) {
            if ( s == null ) {
                throw new ArgumentNullException( nameof( s ) );
            }
            if ( splitter == null ) {
                throw new ArgumentNullException( nameof( splitter ) );
            }
            return s.Substring( s.IndexOf( splitter, StringComparison.InvariantCulture ) + 1 ).TrimStart();
        }

        public static String Append( [CanBeNull] this String result, [CanBeNull] String appendThis ) => $"{result ?? String.Empty}{appendThis ?? String.Empty}";

        /// <summary>
        ///     Return the <see cref="tuple" /> formatted with the index.
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static String AsIndexed( this Tuple<String, Int32> tuple ) => $"{tuple.Item1}.[{tuple.Item2}]";

        /// <summary>
        ///     Return the <see cref="word" /> formatted with the <see cref="index" />.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static String AsIndexed( [NotNull] this String word, Int32 index ) {
            if ( word == null ) {
                throw new ArgumentNullException( nameof( word ) );
            }
            return $"{word}.[{index}]";
        }

        /// <summary>
        ///     Return an integer formatted as 1st, 2nd, 3rd, etc...
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static String AsOrdinal( this Int32 number ) {
            switch ( number % 100 ) {
                case 13:
                case 12:
                case 11:
                    return $"{number}th";
            }
            switch ( number % 10 ) {
                case 1:
                    return $"{number}st";

                case 2:
                    return $"{number}nd";

                case 3:
                    return $"{number}rd";

                default:
                    return $"{number}th";
            }
        }

        /// <summary>
        ///     Return the substring from 0 to the index of the splitter.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static String Before( [NotNull] this String s, [NotNull] String splitter ) {
            if ( s == null ) {
                throw new ArgumentNullException( nameof( s ) );
            }
            if ( splitter == null ) {
                throw new ArgumentNullException( nameof( splitter ) );
            }
            return s.Substring( 0, s.IndexOf( splitter, StringComparison.InvariantCulture ) ).TrimEnd();
        }

        public static IEnumerable<T> ConcatSingle<T>( [NotNull] this IEnumerable<T> sequence, T element ) {
            if ( sequence == null ) {
                throw new ArgumentNullException( nameof( sequence ) );
            }
            foreach ( var item in sequence ) {
                yield return item;
            }
            yield return element;
        }

        public static IDictionary<Char, UInt64> Count( this String text ) {
            var dict = new ConcurrentDictionary<Char, UInt64>();
            text.AsParallel().ForAll( c => dict.AddOrUpdate( c, 1, ( c1, arg2 ) => arg2 + 1 ) );
            return dict;
        }

        public static UInt64 Count( this String text, Char character ) => ( UInt64 )text.Where( c => c == character ).LongCount();

        /// <summary>
        ///     Computes the Damerau-Levenshtein Distance between two strings, represented as arrays of
        ///     integers, where each integer represents the code point of a character in the source
        ///     String. Includes an optional threshhold which can be used to indicate the maximum
        ///     allowable distance.
        /// </summary>
        /// <param name="source">An array of the code points of the first String</param>
        /// <param name="target">An array of the code points of the second String</param>
        /// <param name="threshold">Maximum allowable distance</param>
        /// <returns>
        ///     Int.MaxValue if threshhold exceeded; otherwise the Damerau-Leveshteim distance between
        ///     the strings
        /// </returns>
        public static Int32 DamerauLevenshteinDistance( [NotNull] this String source, [NotNull] String target, Int32 threshold ) {
            if ( source == null ) {
                throw new ArgumentNullException( nameof( source ) );
            }
            if ( target == null ) {
                throw new ArgumentNullException( nameof( target ) );
            }
            var length1 = source.Length;
            var length2 = target.Length;

            // Return trivial case - difference in String lengths exceeds threshhold
            if ( Math.Abs( length1 - length2 ) > threshold ) {
                return Int32.MaxValue;
            }

            // Ensure arrays [i] / length1 use shorter length
            if ( length1 > length2 ) {
                MathExtensions.Swap( ref target, ref source );
                MathExtensions.Swap( ref length1, ref length2 );
            }

            var maxi = length1;
            var maxj = length2;

            var dCurrent = new Int32[ maxi + 1 ];
            var dMinus1 = new Int32[ maxi + 1 ];
            var dMinus2 = new Int32[ maxi + 1 ];

            for ( var i = 0; i <= maxi; i++ ) {
                dCurrent[ i ] = i;
            }

            var jm1 = 0;

            for ( var j = 1; j <= maxj; j++ ) {

                // Rotate
                var dSwap = dMinus2;
                dMinus2 = dMinus1;
                dMinus1 = dCurrent;
                dCurrent = dSwap;

                // Initialize
                var minDistance = Int32.MaxValue;
                dCurrent[ 0 ] = j;
                var im1 = 0;
                var im2 = -1;

                for ( var i = 1; i <= maxi; i++ ) {
                    var cost = source[ im1 ] == target[ jm1 ] ? 0 : 1;

                    var del = dCurrent[ im1 ] + 1;
                    var ins = dMinus1[ i ] + 1;
                    var sub = dMinus1[ im1 ] + cost;

                    //Fastest execution for min value of 3 integers
                    var min = del > ins ? ( ins > sub ? sub : ins ) : ( del > sub ? sub : del );

                    if ( i > 1 && j > 1 && source[ im2 ] == target[ jm1 ] && source[ im1 ] == target[ j - 2 ] ) {
                        min = Math.Min( min, dMinus2[ im2 ] + cost );
                    }

                    dCurrent[ i ] = min;
                    if ( min < minDistance ) {
                        minDistance = min;
                    }
                    im1++;
                    im2++;
                }
                jm1++;
                if ( minDistance > threshold ) {
                    return Int32.MaxValue;
                }
            }

            var result = dCurrent[ maxi ];
            return result > threshold ? Int32.MaxValue : result;
        }

        public static Int32 EditDistanceParallel( this String s1, String s2 ) {
            var dist = new Int32[ s1.Length + 1, s2.Length + 1 ];
            for ( var i = 0; i <= s1.Length; i++ ) {
                dist[ i, 0 ] = i;
            }
            for ( var j = 0; j <= s2.Length; j++ ) {
                dist[ 0, j ] = j;
            }
            var numBlocks = Environment.ProcessorCount * 4;

            ParallelAlgorithms.Wavefront( ( startI, endI, startJ, endJ ) => {
                for ( var i = startI + 1; i <= endI; i++ ) {
                    for ( var j = startJ + 1; j <= endJ; j++ ) {
                        dist[ i, j ] = s1[ i - 1 ] == s2[ j - 1 ] ? dist[ i - 1, j - 1 ] : 1 + Math.Min( dist[ i - 1, j ], Math.Min( dist[ i, j - 1 ], dist[ i - 1, j - 1 ] ) );
                    }
                }
            }, s1.Length, s2.Length, numBlocks, numBlocks );

            return dist[ s1.Length, s2.Length ];
        }

        /// <summary>
        ///     for chaining empty strings with the ?? operator
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static String EmptyAsNull( this String value ) => value == "" ? null : value;

		/// <summary>
        ///     <para>Case insensitive String-end comparison.</para>
        ///     <para>( true example: cAt == CaT )</para>
        ///     <para>
        ///         <see cref="StringComparison.InvariantCultureIgnoreCase" />
        ///     </para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static Boolean EndsLike( this String source, String compare ) => source.EndsWith( compare, StringComparison.InvariantCultureIgnoreCase );

        public static IEnumerable<Char> EnglishOnly( this String s ) {
            try {
                var sb = new StringBuilder();
#pragma warning disable IDE0007 // Use implicit type
				foreach ( Match m in Regex.Matches( s, @"(\w+)|(\$\d+\.\d+)" ) ) {
#pragma warning restore IDE0007 // Use implicit type
					sb.Append( m.Value );
                }
                return sb.ToString().Trim();
            }
            catch ( Exception exception ) {
                exception.More();
                return s;
            }
        }

        /// <summary>
        ///     <para>Escapes a String according to the URI data String rules given in RFC 3986.</para>
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        /// <seealso cref="http://stackoverflow.com/questions/846487/how-to-get-uri-escapedatastring-to-comply-with-rfc-3986" />
        /// <seealso cref="http://meyerweb.com/eric/tools/dencoder/" />
        /// <seealso cref="http://www.ietf.org/rfc/rfc2396.txt" />
        /// <seealso cref="http://msdn.microsoft.com/en-us/Library/vstudio/bb968786(v=vs.100).aspx" />
        /// <remarks>
        ///     <para>
        ///         The <see cref="Uri.EscapeDataString" /> method is <i>supposed</i> to take on RFC 3986
        ///         behavior if certain elements are present in a .config file. Even if this actually worked
        ///         (which in my experiments it <i>doesn't</i>), we can't rely on every host actually having
        ///         this configuration element present.
        ///     </para>
        /// </remarks>
        public static String EscapeUriDataStringRfc3986( String value ) {

            // Start with RFC 2396 escaping by calling the .NET method to do the work. This MAY
            // sometimes exhibit RFC 3986 behavior (according to the documentation). If it does, the
            // escaping we do that follows it will be a no-op since the characters we search for to
            // replace can't possibly exist in the String.
            var escaped = new StringBuilder( Uri.EscapeDataString( value ) );

            // Upgrade the escaping to RFC 3986, if necessary.
            foreach ( var t in UriRfc3986CharsToEscape ) {
                escaped.Replace( t, Uri.HexEscape( t[ 0 ] ) );
            }

            // Return the fully-RFC3986-escaped String.

            return escaped.ToString();
        }

        public static Boolean ExactMatch( [NotNull] this String source, [NotNull] String compare ) {
            if ( source == null ) {
                throw new ArgumentNullException( nameof( source ) );
            }
            if ( compare == null ) {
                throw new ArgumentNullException( nameof( compare ) );
            }
            if ( source.Length == 0 || compare.Length == 0 ) {
                return false;
            }
            return source.SequenceEqual( compare );
        }

        [NotNull]
        public static String FirstSentence( this String text ) {
            if ( text.IsNullOrWhiteSpace() ) {
                return String.Empty;
            }
            var sentences = text.ToSentences().FirstOrDefault();
            return sentences?.ToString() ?? String.Empty;
        }

        public static String FirstWord( this String sentence ) => sentence.ToWords().FirstOrDefault() ?? String.Empty;

        /// <summary>
        /// </summary>
        /// <param name="rational"></param>
        /// <param name="numberOfDigits"></param>
        /// <returns></returns>
        /// <seealso
        ///     cref="http://kashfarooq.wordpress.com/2011/08/01/calculating-pi-in-c-part-3-using-the-net-4-bigrational-class/" />
        public static String Format( this BigRational rational, Int32 numberOfDigits ) {
            var numeratorShiftedToEnoughDigits = rational.Numerator * BigInteger.Pow( new BigInteger( 10 ), numberOfDigits );
            var bigInteger = numeratorShiftedToEnoughDigits / rational.Denominator;
            var toBeFormatted = bigInteger.ToString();
            var builder = new StringBuilder();
            builder.Append( toBeFormatted[ 0 ] );
            builder.Append( "." );
            builder.Append( toBeFormatted.Substring( 1, numberOfDigits - 1 ) );
            return builder.ToString();
        }

        public static String FromBase64( this String base64EncodedData ) {
            var base64EncodedBytes = Convert.FromBase64String( base64EncodedData );
            return Encoding.Unicode.GetString( base64EncodedBytes );
        }

        public static String FullSoundex( this String s ) {

            // the encoding information
            //const String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const String codes = "0123012D02245501262301D202";

            // some helpful regexes
            var hwBeginString = new Regex( "^D+" );
            var simplify = new Regex( @"(\d)\1*D?\1+" );
            var cleanup = new Regex( "[D0]" );

            // i need a capitalized String
            s = s.ToUpper();

            // i'm building the coded String using a String builder because i think this is probably
            // the fastest and least intensive way
            var coded = new StringBuilder();

            // do the encoding
            foreach ( var index in s.Select( t => AllUppercaseLetters.IndexOf( t ) ).Where( index => index >= 0 ) ) {
                coded.Append( codes[ index ] );
            }

            // okay, so here's how this goes . . . the first thing I do is assign the coded String
            // so that i can regex replace on it

            // then i remove repeating characters
            //result = repeating.Replace(result, "$1");
            var result = simplify.Replace( coded.ToString(), "$1" ).Substring( 1 );

            // now i need to remove any characters coded as D from the front of the String because
            // they're not really valid as the first code because they don't have an actual soundex
            // code value
            result = hwBeginString.Replace( result, String.Empty );

            // i used the char D to indicate that an h or w existed so that if to similar sounds
            // were separated by an h or a w that I could remove one of them. if the h or w does not
            // separate two similar sounds, then i need to remove it now
            result = cleanup.Replace( result, String.Empty );

            // return the first character followed by the coded String
            return $"{s[ 0 ]}{result}";
        }

        /// <summary>
        ///     Return possible variants of a name for name matching.
        /// </summary>
        /// <param name="input">String to convert</param>
        /// <param name="culture">The culture to use for conversion</param>
        /// <returns>IEnumerable&lt;String&gt;</returns>
        public static IEnumerable<String> GetNameVariants( this String input, CultureInfo culture ) {
            if ( String.IsNullOrEmpty( input ) ) {
                yield break;
            }

            yield return input;

            // try camel cased name
            yield return input.ToCamelCase( culture );

            // try lower cased name
            yield return input.ToLower( culture );

            // try name with underscores
            yield return input.AddUnderscores();

            // try name with underscores with lower case
            yield return input.AddUnderscores().ToLower( culture );

            // try name with dashes
            yield return input.AddDashes();

            // try name with dashes with lower case
            yield return input.AddDashes().ToLower( culture );

            // try name with underscore prefix
            yield return input.AddUnderscorePrefix();

            // try name with underscore prefix, using camel case
            yield return input.ToCamelCase( culture ).AddUnderscorePrefix();
        }

        /// <summary>
        ///     Add a space Before Each Capital Letter. then lowercase the whole string.
        ///     <para>See also: <seealso cref="AddSpacesBeforeUppercase" /></para>
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        [NotNull]
        public static String Humanize( [NotNull] this String word ) {
            if ( word == null ) {
                throw new ArgumentNullException( nameof( word ) );
            }

            return word.AddSpacesBeforeUppercase().ToLower( CultureInfo.CurrentUICulture );
        }

        public static String InOutputFormat( this String indexed ) => $"{indexed}-|";

        // .NET Char class already provides an static IsDigit method however it behaves differently depending on if char is a Latin or not.
        public static Boolean IsDigit( this Char c ) => c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9';

		public static Boolean IsJustNumbers( [CanBeNull] this String text ) {
            if ( null == text ) {
                return false;
            }

            if ( text.All( Char.IsNumber ) ) {
                return true;
            }

			if ( Double.TryParse( text, out var test ) ) {
				return true;
			}

			if ( Decimal.TryParse( text, out var test2 ) ) {
				return true;
			}

			return false;
        }

        public static Boolean IsJustNumbers( [CanBeNull] this String text, out Decimal result ) => Decimal.TryParse( text ?? String.Empty, out result );

        public static Boolean IsNullOrEmpty( [CanBeNull] this String value ) => String.IsNullOrEmpty( value );

        public static Boolean IsNullOrWhiteSpace( [CanBeNull] this String value ) => String.IsNullOrWhiteSpace( value );

		private static readonly Regex UpperCaseRegeEx = new Regex( @"^[A-Z]+$", RegexOptions.Compiled, Minutes.One );

		/// <summary>
		///     Checks to see if a String is all uppper case
		/// </summary>
		/// <param name="inputString">String to check</param>
		/// <returns>Boolean</returns>
		public static Boolean IsUpperCase( this String inputString ) => UpperCaseRegeEx.IsMatch( inputString );

        /// <summary>
        ///     <para>String sentence = "10 cats, 20 dogs, 40 fish and 1 programmer.";</para>
        ///     <para>
        ///         Should return:
        ///         <list type="">
        ///             <item>10</item><item>20</item><item>40</item><item>1</item>
        ///         </list>
        ///     </para>
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public static IEnumerable<String> JustDigits( this String sentence ) => RegexJustDigits.Split( sentence );

        /// <summary>
        ///     Example: String s = "123-123-1234".JustNumbers();
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String JustNumbers( this String s ) {
            try {
                var sb = new StringBuilder();
#pragma warning disable IDE0007 // Use implicit type
				foreach ( Match m in Regex.Matches( s, "[0-9]" ) ) {
#pragma warning restore IDE0007 // Use implicit type
					sb.Append( m.Value );
                }
                return sb.ToString();
            }
            catch ( Exception error ) {
                error.More();
                return s;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        /// <seealso cref="Word" />
        /// <seealso cref="Sentence" />
        public static IEnumerable<String> JustWords( this String sentence ) {
            var result = sentence.ToWords().Where( word => word.Any( Char.IsLetterOrDigit ) );
            return result;
        }

        /// <summary>
        ///     <para>Case insensitive String comparison.</para>
        ///     <para>( for example: cAt == CaT is true )</para>
        ///     <para>
        ///         <see cref="StringComparison.InvariantCultureIgnoreCase" />
        ///     </para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static Boolean Like( this String source, String compare ) => ( source ?? String.Empty ).Equals( compare ?? String.Empty, StringComparison.InvariantCultureIgnoreCase );

        /// <summary>
        ///     Convert the first letter of a String to lower case
        /// </summary>
        /// <param name="word">String to convert</param>
        /// <returns>String</returns>
        public static String MakeInitialLowerCase( this String word ) => String.Concat( word.Substring( 0, 1 ).ToLowerInvariant(), word.Substring( 1 ) );

        /// <summary>
        ///     Gets a <b>horrible</b> ROUGH guesstimate of the memory consumed by an object by using
        ///     <seealso cref="NetDataContractSerializer" /> .
        /// </summary>
        /// <param name="bob"></param>
        /// <returns></returns>
        public static Int64 MemoryUsed( [NotNull] this Object bob ) {
            if ( bob == null ) {
                throw new ArgumentNullException( nameof( bob ) );
            }
            try {
                using ( var s = new NullStream() ) {
                    var serializer = new NetDataContractSerializer { AssemblyFormat = FormatterAssemblyStyle.Full };
                    serializer.WriteObject( stream: s, graph: bob );
                    return s.Length;
                }
            }
            catch ( InvalidDataContractException exception ) {
                exception.More();
            }
            catch ( SerializationException exception ) {
                exception.More();
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return 0;
        }

        [Pure]
        [CanBeNull]
        public static String NullIfBlank( [CanBeNull] this String theString ) {
            if ( String.IsNullOrWhiteSpace( theString ) ) {
                return null;
            }
            theString = theString.Trim();
            return String.IsNullOrWhiteSpace( theString ) ? null : theString;
        }

        public static String NullIfEmpty( [CanBeNull] this String value ) => String.IsNullOrEmpty( value ) ? null : value;

        public static String NullIfEmptyOrWhiteSpace( [CanBeNull] this String value ) => String.IsNullOrWhiteSpace( value ) ? null : value;

        public static String NullIfJustNumbers( [CanBeNull] this String value ) => value.IsJustNumbers() ? null : value;

        public static Int32 NumberOfDigits( this BigInteger number ) => ( number * number.Sign ).ToString().Length;

        public static String PadMiddle( Int32 totalLength, String partA, String partB, Char paddingChar ) {
            var result = partA + partB;
            while ( result.Length < totalLength ) {
                result = result.Insert( partA.Length, paddingChar.ToString() );
            }
            while ( result.Length > totalLength ) {
                result = result.Remove( partA.Length, 1 );
            }

            return result;
        }

        public static String PadMiddle( Int32 totalLength, String partA, String partB, String partC, Char paddingChar = '_' ) {
            var padding = String.Empty.PadRight( ( totalLength - ( partA.Length + partB.Length + partC.Length ) ) / 2, paddingChar );
            return partA + padding + partB + String.Empty.PadRight( totalLength - ( partA.Length + padding.Length + partB.Length + partC.Length ), paddingChar ) + partC;
        }

        /// <summary>
        ///     Crude attempt at pluralizing a <paramref name="number" />.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="singular"></param>
        /// <returns></returns>
        public static String PluralOf( this UInt64 number, [NotNull] String singular ) {
            if ( singular == null ) {
                throw new ArgumentNullException( nameof( singular ) );
            }
            if ( 1 == number ) {
                return singular;
            }
			if ( PluralCache.TryGetValue( singular, out var plural ) ) {
				return plural;
			}

			if ( LazyPluralizationService.Value.IsPlural( singular ) ) {
                PluralCache[ singular ] = singular;
                return singular;
            }

            var pluralized = LazyPluralizationService.Value.Pluralize( singular );
            PluralCache[ singular ] = pluralized;
            return pluralized;
        }

        /// <summary>
        ///     Crude attempt at pluralizing a <paramref name="number" />.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="singular"></param>
        /// <returns></returns>
        public static String PluralOf( this Double number, [NotNull] String singular ) {
            if ( singular == null ) {
                throw new ArgumentNullException( nameof( singular ) );
            }
            if ( number.Near( 1 ) ) {
                return singular;
            }

			if ( PluralCache.TryGetValue( singular, out var plural ) ) {
				return plural;
			}

			if ( LazyPluralizationService.Value.IsPlural( singular ) ) {
                PluralCache[ singular ] = singular;
                return singular;
            }

            var pluralized = LazyPluralizationService.Value.Pluralize( singular );
            PluralCache[ singular ] = pluralized;
            return pluralized;
        }

        /// <summary>
        ///     Crude attempt at pluralizing a <paramref name="number" />.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="singular"></param>
        /// <returns></returns>
        public static String PluralOf( this Single number, [NotNull] String singular ) {
            if ( singular == null ) {
                throw new ArgumentNullException( nameof( singular ) );
            }
            if ( number.Near( 1 ) ) {
                return singular;
            }

			if ( PluralCache.TryGetValue( singular, out var plural ) ) {
				return plural;
			}

			if ( LazyPluralizationService.Value.IsPlural( singular ) ) {
                PluralCache[ singular ] = singular;
                return singular;
            }

            var pluralized = LazyPluralizationService.Value.Pluralize( singular );
            PluralCache[ singular ] = pluralized;
            return pluralized;
        }

        /// <summary>
        ///     Crude attempt at pluralizing a <paramref name="number" />.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="singular"></param>
        /// <returns></returns>
        public static String PluralOf( this Decimal number, [NotNull] String singular ) {
            if ( singular == null ) {
                throw new ArgumentNullException( nameof( singular ) );
            }

            if ( Decimal.One == number ) {
                return singular;
            }

			if ( PluralCache.TryGetValue( singular, out var plural ) ) {
				return plural;
			}

			if ( LazyPluralizationService.Value.IsPlural( singular ) ) {
                PluralCache[ singular ] = singular;
                return singular;
            }

            var pluralized = LazyPluralizationService.Value.Pluralize( singular );
            PluralCache[ singular ] = pluralized;
            return pluralized;
        }

        /// <summary>
        ///     Crude attempt at pluralizing a <paramref name="number" />.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="singular"></param>
        /// <returns></returns>
        public static String PluralOf( this Int32 number, [NotNull] String singular ) {
            if ( singular == null ) {
                throw new ArgumentNullException( nameof( singular ) );
            }

            if ( 1 == number ) {
                return singular;
            }

			if ( PluralCache.TryGetValue( singular, out var plural ) ) {
				return plural;
			}

			if ( LazyPluralizationService.Value.IsPlural( singular ) ) {
                PluralCache[ singular ] = singular;
                return singular;
            }

            var pluralized = LazyPluralizationService.Value.Pluralize( singular );
            PluralCache[ singular ] = pluralized;
            return pluralized;
        }

        /// <summary>
        ///     Crude attempt at pluralizing a <paramref name="number" />.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="singular"></param>
        /// <returns></returns>
        public static String PluralOf( this BigRational number, [NotNull] String singular ) {
            if ( singular == null ) {
                throw new ArgumentNullException( nameof( singular ) );
            }
            if ( number == BigRational.One ) {
                return singular;
            }
			if ( PluralCache.TryGetValue( singular, out var plural ) ) {
				return plural;
			}

			if ( LazyPluralizationService.Value.IsPlural( singular ) ) {
                PluralCache[ singular ] = singular;
                return singular;
            }

            var pluralized = LazyPluralizationService.Value.Pluralize( singular );
            PluralCache[ singular ] = pluralized;
            return pluralized;
        }

        /// <summary>
        ///     Crude attempt at pluralizing a <paramref name="number" />.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="singular"></param>
        /// <returns></returns>
        public static String PluralOf( this BigInteger number, [NotNull] String singular ) {
            if ( singular == null ) {
                throw new ArgumentNullException( nameof( singular ) );
            }
            if ( BigInteger.One == number ) {
                return singular;
            }
			if ( PluralCache.TryGetValue( singular, out var plural ) ) {
				return plural;
			}

			if ( LazyPluralizationService.Value.IsPlural( singular ) ) {
                PluralCache[ singular ] = singular;
                return singular;
            }

            var pluralized = LazyPluralizationService.Value.Pluralize( singular );
            PluralCache[ singular ] = pluralized;
            return pluralized;
        }

        public static String Prepend( [CanBeNull] this String result, [CanBeNull] String prependThis ) => $"{prependThis ?? String.Empty}{result ?? String.Empty}";

        public static String Quoted( [CanBeNull] this String s ) => $"\"{s}\"";

        public static String ReadToEnd( [NotNull] this MemoryStream ms ) {
            if ( ms == null ) {
                throw new ArgumentNullException( nameof( ms ) );
            }
            ms.Seek( 0, SeekOrigin.Begin );
            using ( var reader = new StreamReader( ms ) ) {
                return reader.ReadToEnd();
            }
        }

        public static UInt64 RealLength( [CanBeNull] this String s ) {
            if ( String.IsNullOrEmpty( s ) ) {
                return 0;
            }
            var stringInfo = new StringInfo( s );
            return ( UInt64 )stringInfo.LengthInTextElements;
        }

        public static String RemoveNullChars( this String text ) => text.Replace( "\0", String.Empty );

        /// <summary>
        ///     Remove leading and trailing " from a String
        /// </summary>
        /// <param name="input">String to parse</param>
        /// <returns>String</returns>
        public static String RemoveSurroundingQuotes( this String input ) {
            if ( input.StartsWith( "\"", StringComparison.Ordinal ) && input.EndsWith( "\"", StringComparison.Ordinal ) ) {

                // remove leading/trailing quotes
                input = input.Substring( 1, input.Length - 2 );
            }
            return input;
        }

        /// <summary>
        ///     Repeats the supplied string the specified number of times, putting the separator string between each repetition.
        /// </summary>
        /// <param name="this">The extended string.</param>
        /// <param name="repetitions">The number of repetitions of the string to make. Must not be negative.</param>
        /// <param name="separator">The separator string to place between each repetition. Must not be null.</param>
        /// <returns>
        ///     The subject string, repeated n times, where n = repetitions. Between each repetition will be the separator
        ///     string. If n is 0, this method will return String.Empty.
        /// </returns>
        public static String Repeat( this String @this, Int32 repetitions, String separator = "" ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "Repeat called on a null string." );
            }
            if ( separator == null ) {
                throw new ArgumentNullException( nameof( separator ) );
            }
            if ( repetitions < 0 ) {
                throw new ArgumentOutOfRangeException( nameof( repetitions ), "Value must not be negative." );
            }

            if ( repetitions == 0 ) {
                return String.Empty;
            }
            var builder = new StringBuilder( @this.Length * repetitions + separator.Length * ( repetitions - 1 ) );
            for ( var i = 0; i < repetitions; ++i ) {
                if ( i > 0 ) {
                    builder.Append( separator );
                }
                builder.Append( @this );
            }
            return builder.ToString();
        }

        public static String ReplaceAll( this String haystack, String needle, String replacement ) {
            Int32 pos;

            // Avoid a possible infinite loop
            if ( needle == replacement ) {
                return haystack;
            }
            while ( ( pos = haystack.IndexOf( needle, StringComparison.Ordinal ) ) > 0 ) {
                haystack = haystack.Substring( 0, pos ) + replacement + haystack.Substring( pos + needle.Length );
            }
            return haystack;
        }

        public static String ReplaceFirst( this String haystack, String needle, String replacement ) {
            var pos = haystack.IndexOf( needle, StringComparison.Ordinal );
            if ( pos < 0 ) {
                return haystack;
            }
            return haystack.Substring( 0, pos ) + replacement + haystack.Substring( pos + needle.Length );
        }

        public static String ReplaceHTML( this String s, String withwhat ) => Regex.Replace( s, @"<(.|\n)*?>", withwhat );

        /// <summary>
        ///     Reverse a String
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String Reverse( this String s ) {
            var charArray = s.ToCharArray();
            Array.Reverse( charArray );
            return new String( charArray );
        }

        /// <summary>
        /// </summary>
        /// <param name="myString"></param>
        /// <returns></returns>
        /// <seealso cref="http://codereview.stackexchange.com/questions/78065/reverse-a-sentence-quickly-without-pointers" />
        public static String ReverseWords( this String myString ) {
            var length = myString.Length;
            var tokens = new Char[ length ];
            var position = 0;
            Int32 lastIndex;
            for ( var i = length - 1; i >= 0; i-- ) {
                if ( myString[ i ] != ' ' ) {
                    continue;
                }
                lastIndex = length - position;
                for ( var k = i + 1; k < lastIndex; k++ ) {
                    tokens[ position ] = myString[ k ];
                    position++;
                }
                tokens[ position ] = ' ';
                position++;
            }

            lastIndex = myString.Length - position;
            for ( var i = 0; i < lastIndex; i++ ) {
                tokens[ position ] = myString[ i ];
                position++;
            }

            return new String( tokens );
        }

        public static String Right( this String s, Int32 count ) {
            var newString = String.Empty;

            if ( String.IsNullOrEmpty( s ) || count <= 0 ) {
                return newString;
            }

            var startIndex = s.Length - count;
            newString = startIndex > 0 ? s.Substring( startIndex, count ) : s;
            return newString;
        }

        /// <summary>
        ///     Case sensitive (<see cref="StringComparison.InvariantCulture" />) string comparison.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static Boolean Same( this String source, String compare ) => ( source ?? String.Empty ).Equals( compare ?? String.Empty, StringComparison.InvariantCulture );

        /// <summary>
        ///     Compute a Similarity between two strings. <br />
        ///     1. 0 is a full, bit for bit match. <br />
        /// </summary>
        /// <param name="source"></param>
        /// <param name="compare"></param>
        /// <param name="timeout"></param>
        /// <param name="matchReasons">preferably an empty queue</param>
        /// <returns></returns>
        /// <remarks>
        ///     The score is normalized such that 0 equates to no similarity and 1 is an exact match.
        /// </remarks>
        public static Double Similarity( [CanBeNull] this String source, [CanBeNull] String compare, [CanBeNull] ConcurrentQueue<String> matchReasons = null, TimeSpan? timeout = null ) {
            var similarity = new PotentialF( 0 );

            if ( source == null && compare == null ) {
                similarity.Add( 1 );
                goto noMoreTests;
            }
            if ( source == null ) {
                goto noMoreTests;
            }
            if ( compare == null ) {
                goto noMoreTests;
            }

            var stopwatch = StopWatch.StartNew();

            if ( !timeout.HasValue ) {
                timeout = Minutes.One;
            }

            if ( source.Length <= 0 || compare.Length <= 0 ) {
                goto noMoreTests;
            }

            if ( source.ExactMatch( compare ) ) {
                matchReasons?.Add( "ExactMatch( source, compare )" );
                similarity.Add( 1 );
                goto noMoreTests;
            }

            if ( source.SequenceEqual( compare ) ) {
                goto noMoreTests; //exact match. no more comparisons needed.
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }

            var votes = new VotallyD();

            votes.ForA( source.Length );
            votes.ForB( compare.Length );

            var sourceIntoUtf32Encoding = new UTF32Encoding( bigEndian: true, byteOrderMark: true, throwOnInvalidCharacters: false ).GetBytes( source );
            votes.ForA( sourceIntoUtf32Encoding.LongCount() );

            var compareIntoUtf32Encoding = new UTF32Encoding( bigEndian: true, byteOrderMark: true, throwOnInvalidCharacters: false ).GetBytes( compare );
            votes.ForB( compareIntoUtf32Encoding.LongCount() );

            // Test for exact same sequence
            if ( sourceIntoUtf32Encoding.SequenceEqual( compareIntoUtf32Encoding ) ) {
                votes.ForA( sourceIntoUtf32Encoding.Length );
                votes.ForB( compareIntoUtf32Encoding.Length );
                matchReasons.Add( "exact match as UTF32 encoded" );
                goto noMoreTests;
            }

            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }

            var compareReversed = compare.Reverse();
            if ( source.SequenceEqual( compareReversed ) ) {
                votes.ForA( source.Length );
                votes.ForB( compare.Length / 2.0 );
                matchReasons.Add( "partial String reversal" );
            }

            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }

            var sourceDistinct = new String( source.Distinct().ToArray() );
            var compareDistinct = new String( compare.Distinct().ToArray() );
            var compareDistinctReverse = new String( compareDistinct.Reverse().ToArray() );

            if ( sourceDistinct.SequenceEqual( compareDistinct ) ) {
                votes.ForA( sourceDistinct.Length );
                votes.ForB( compareDistinct.Length );
                matchReasons.Add( "exact match after Distinct()" );
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }

            if ( sourceDistinct.SequenceEqual( compareDistinctReverse ) ) {
                votes.ForA( sourceDistinct.Length * 2 );
                votes.ForB( compareDistinctReverse.Length );
                matchReasons.Add( "exact match after Distinct()" );
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }

            var tempcounter = 0;
            foreach ( var c in source ) {
                votes.ForA();
                if ( !compare.Contains( c ) ) {
                    continue;
                }
                votes.ForB();
                tempcounter++;
            }
            if ( tempcounter > 0 ) {
                matchReasons.Add( $"{tempcounter} characters found in compare from source" );
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }

            tempcounter = 0;
            foreach ( var c in compare ) {
                votes.ForB();
                if ( !source.Contains( c ) ) {
                    continue;
                }
                votes.ForA();
                tempcounter++;
            }
            if ( tempcounter > 0 ) {
                matchReasons.Add( $"{tempcounter} characters found in compare from source" );
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }

            if ( source.Contains( compare ) ) {
                votes.ForA( source.Length );
                votes.ForB( compare.Length );
                matchReasons.Add( "found compare String inside source String" );
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }

            if ( compare.Contains( source ) ) {
                votes.ForA( source.Length );
                votes.ForB( compare.Length );
                matchReasons.Add( "found source String inside compare String" );
            }
            if ( stopwatch.Elapsed > timeout ) {
                goto noMoreTests; //no more time for comparison
            }

            Single threshold = Math.Max( source.Length, compare.Length );
            var actualDamerauLevenshteinDistance = DamerauLevenshteinDistance( source: source, target: compare, threshold: ( Int32 )threshold );

            //TODO votes.ForB ???
            similarity.Add( threshold - actualDamerauLevenshteinDistance / threshold );

            if ( stopwatch.Elapsed > timeout ) {

                //TODO
            }

            //TODO

            noMoreTests:
            return similarity;
        }

        public static String Soundex( [NotNull] this String s, Int32 length = 4 ) {
            if ( s == null ) {
                throw new ArgumentNullException( nameof( s ) );
            }
            return FullSoundex( s ).PadRight( length, '0' ) // soundex is no shorter than
                                   .Substring( 0, length ); // and no longer than length
        }

		/// <summary>
		///     Same as calling <see cref="String.Split(String[], StringSplitOptions)" /> with an array of size 1.
		/// </summary>
		/// <param name="this">The extended string.</param>
		/// <param name="separator">The delimiter that splits substrings in the given string. Must not be null.</param>
		/// <param name="splitOptions">
		///     RemoveEmptyEntries to omit empty array elements from the array returned; or None to include
		///     empty array elements in the array returned.
		/// </param>
		/// <returns>See: <see cref="String.Split(String[], StringSplitOptions)" />.</returns>
		public static String[] Split( this String @this, String separator, StringSplitOptions splitOptions = StringSplitOptions.None ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "Split called on a null String." );
            }
            if ( separator == null ) {
                throw new ArgumentNullException( nameof( separator ) );
            }
            return @this.Split( new[] { separator }, splitOptions );
        }

        public static IEnumerable<String> SplitToChunks( [NotNull] this String s, Int32 chunks ) {
            if ( s == null ) {
                throw new ArgumentNullException( nameof( s ) );
            }
            var res = Enumerable.Range( 0, s.Length ).Select( index => new {
                index,
                ch = s[ index ]
            } ).GroupBy( f => f.index / chunks ).Select( g => String.Join( "", g.Select( z => z.ch ) ) );

            return res;
        }

        public static String StringFromResponse( [CanBeNull] this WebResponse response ) {
            var restream = response?.GetResponseStream();
            return restream != null ? new StreamReader( restream ).ReadToEnd() : String.Empty;
        }

        public static Byte[] StringToUtf32ByteArray( this String pXmlString ) => new UTF32Encoding().GetBytes( pXmlString );

        /// <summary>
        ///     Converts the String to UTF8 Byte array and is used in De serialization
        /// </summary>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        public static Byte[] StringToUtf8ByteArray( this String pXmlString ) => new UTF8Encoding().GetBytes( pXmlString );

        public static String StripHTML( this String s ) => Regex.Replace( s, @"<(.|\n)*?>", String.Empty ).Replace( "&nbsp;", " " );

        public static String StripTags( this String input, String[] allowedTags ) {
            var stripHTMLExp = new Regex( @"(<\/?[^>]+>)" );
            var output = input;

#pragma warning disable IDE0007 // Use implicit type
			foreach ( Match tag in stripHTMLExp.Matches( input ) ) {
#pragma warning restore IDE0007 // Use implicit type
				var htmlTag = tag.Value.ToLower();
                var isAllowed = false;

                foreach ( var allowedTag in allowedTags ) {
                    var offset = -1;

                    // Determine if it is an allowed tag "<tag>" , "<tag " and "</tag"
                    if ( offset != 0 ) {
                        offset = htmlTag.IndexOf( '<' + allowedTag + '>', StringComparison.Ordinal );
                    }
                    if ( offset != 0 ) {
                        offset = htmlTag.IndexOf( '<' + allowedTag + ' ', StringComparison.Ordinal );
                    }
                    if ( offset != 0 ) {
                        offset = htmlTag.IndexOf( "</" + allowedTag, StringComparison.Ordinal );
                    }

                    // If it matched any of the above the tag is allowed
                    if ( offset != 0 ) {
                        continue;
                    }
                    isAllowed = true;
                    break;
                }

                // Remove tags that are not allowed
                if ( !isAllowed ) {
                    output = output.ReplaceFirst( tag.Value, "" );
                }
            }

            return output;
        }

        public static String StripTagsAndAttributes( this String input, String[] allowedTags ) {
            /* Remove all unwanted tags first */
            var output = input.StripTags( allowedTags );

            /* Lambda functions */
	        String HrefMatch( Match m ) => m.Groups[ 1 ].Value + "href..;,;.." + m.Groups[ 2 ].Value;

	        String ClassMatch( Match m ) => m.Groups[ 1 ].Value + "class..;,;.." + m.Groups[ 2 ].Value;

	        String UnsafeMatch( Match m ) => m.Groups[ 1 ].Value + m.Groups[ 4 ].Value;

	        /* Allow the "href" attribute */
            output = new Regex( "(<a.*)href=(.*>)" ).Replace( output, HrefMatch );

            /* Allow the "class" attribute */
            output = new Regex( "(<a.*)class=(.*>)" ).Replace( output, ClassMatch );

            /* Remove unsafe attributes in any of the remaining tags */
            output = new Regex( @"(<.*) .*=(\'|\""|\w)[\w|.|(|)]*(\'|\""|\w)(.*>)" ).Replace( output, UnsafeMatch );

            /* Return the allowed tags to their proper form */
            output = output.ReplaceAll( "..;,;..", "=" );

            return output;
        }

		/// <summary>
		///     Just <see cref="String.Substring(Int32)" /> with a length check.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static String Sub( this String s, Int32 count ) {
            var length = Math.Min( count, s.Length );
            return s.Substring( 0, length );
        }

		/// <summary>
		///     Performs the same action as <see cref="String.Substring(Int32)" /> but counting from the end of the string (instead
		///     of the start).
		/// </summary>
		/// <param name="this">The extended string.</param>
		/// <param name="endIndex">The zero-based starting character position (from the end) of a substring in this instance.</param>
		/// <returns>Returns the original string with <paramref name="endIndex" /> characters removed from the end.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///     Thrown if endIndex is greater than the length of the string (or
		///     negative).
		/// </exception>
		public static String SubstringFromEnd( this String @this, Int32 endIndex ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "SubstringFromEnd called on a null string." );
            }
            if ( endIndex < 0 || endIndex > @this.Length ) {
                throw new ArgumentOutOfRangeException( nameof( endIndex ) );
            }
            return @this.Substring( 0, @this.Length - endIndex );
        }

		/// <summary>
		///     Performs the same action as <see cref="String.Substring(Int32, Int32)" /> but counting from the end of the string
		///     (instead of the start).
		/// </summary>
		/// <param name="this">The extended string.</param>
		/// <param name="endIndex">The zero-based starting character position (from the end) of a substring in this instance.</param>
		/// <param name="length">The number of characters in the substring.</param>
		/// <returns>
		///     Returns <paramref name="length" /> characters of the subject string, counting backwards from
		///     <paramref name="endIndex" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		///     Thrown if endIndex is greater than the length of the string (or
		///     negative).
		/// </exception>
		public static String SubstringFromEnd( this String @this, Int32 endIndex, Int32 length ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "SubstringFromEnd called on a null string." );
            }
            if ( endIndex < 0 || endIndex > @this.Length ) {
                throw new ArgumentOutOfRangeException( nameof( endIndex ) );
            }
            return @this.Substring( @this.Length - endIndex - length, @this.Length - endIndex );
        }

        [Test]
        public static void Test() {
            Console.WriteLine( "<p>George</p><b>W</b><i>Bush</i>".StripTags( new[] { "i", "b" } ) );
            Console.WriteLine( "<p>George <img src='someimage.png' onmouseover='someFunction()'>W <i>Bush</i></p>".StripTags( new[] { "p" } ) );
            Console.WriteLine( "<a href='http://www.djksterhuis.org'>Martijn <b>Dijksterhuis</b></a>".StripTags( new[] { "a" } ) );

            const String test4 = "<a class=\"classof69\" onClick='crosssite.boom()' href='http://www.djksterhuis.org'>Martijn Dijksterhuis</a>";
            Console.WriteLine( test4.StripTagsAndAttributes( new[] { "a" } ) );
        }

        public static String ToBase64( this String plainText ) {
            var plainTextBytes = Encoding.Unicode.GetBytes( plainText ?? String.Empty );
            return Convert.ToBase64String( plainTextBytes );
        }

		/// <summary>
		/// Date plus Time
		/// </summary>
		/// <param name="when"></param>
		/// <returns></returns>
	    public static String ToLongDateTime( this DateTime when ) => when.ToLongDateString() + Singlespace + when.ToLongTimeString();

		/// <summary>
        ///     Converts a String to camel case
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">String to convert</param>
        /// <param name="culture"></param>
        /// <returns>String</returns>
        public static String ToCamelCase( this String lowercaseAndUnderscoredWord, CultureInfo culture ) => MakeInitialLowerCase( ToPascalCase( lowercaseAndUnderscoredWord, culture ) );

        /// <summary>
        ///     Same as <see cref="AsOrdinal" />, but might be slightly faster performance-wise.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static String ToOrdinal( this Int32 number ) {
            var n = Math.Abs( number );
            var lt = n % 100;
            return number + OrdinalSuffixes[ lt >= 11 && lt <= 13 ? 0 : n % 10 ];
        }

        /// <summary>
        ///     Converts a String to pascal case with the option to remove underscores
        /// </summary>
        /// <param name="text">String to convert</param>
        /// <param name="culture"></param>
        /// <param name="removeUnderscores">Option to remove underscores</param>
        /// <returns></returns>
        public static String ToPascalCase( this String text, CultureInfo culture, Boolean removeUnderscores = true ) {
            if ( String.IsNullOrEmpty( text ) ) {
                return String.Empty;
            }

            text = text.Replace( "_", " " );
            var joinString = removeUnderscores ? String.Empty : "_";
            var words = text.Split( ' ' );
            if ( words.Length <= 1 && !words[ 0 ].IsUpperCase() ) {
                return String.Concat( words[ 0 ].Substring( 0, 1 ).ToUpper( culture ), words[ 0 ].Substring( 1 ) );
            }

            for ( var i = 0; i < words.Length; i++ ) {
                if ( words[ i ].Length <= 0 ) {
                    continue;
                }
                var word = words[ i ];
                var restOfWord = word.Substring( 1 );

                if ( restOfWord.IsUpperCase() ) {
                    restOfWord = restOfWord.ToLower( culture );
                }

                var firstChar = Char.ToUpper( word[ 0 ], culture );
                words[ i ] = String.Concat( firstChar, restOfWord );
            }
            return String.Join( joinString, words );
        }

        [NotNull]
        public static IEnumerable<Sentence> ToSentences( [CanBeNull] this String paragraph ) {
            if ( paragraph == null ) {
                return Enumerable.Empty<Sentence>();
            }

            //clean it up some
            paragraph = paragraph.Replace( "\t", Singlespace );
            do {
                paragraph = paragraph.Replace( Doublespace, Singlespace );
            } while ( paragraph.Contains( Doublespace ) );
            paragraph = paragraph.Replace( "\n\n", Environment.NewLine );
            paragraph = paragraph.Replace( "\r\n", Environment.NewLine );
            paragraph = paragraph.Replace( "\r", Environment.NewLine );
            paragraph = paragraph.Replace( "\n", Environment.NewLine );

            //paragraph = paragraph.Replace( Environment.NewLine, Singlespace );

            while ( paragraph.Contains( Doublespace ) ) {
                paragraph = paragraph.Replace( oldValue: Doublespace, newValue: Singlespace );
            }

            var results = RegexBySentenceStackoverflow.Split( input: paragraph ).Select( s => s.Replace( Environment.NewLine, String.Empty ).Trim() ).Where( ts => !String.IsNullOrWhiteSpace( ts ) && !ts.Equals( "." ) );
            return results.Select( s => new Sentence( s ) );
        }

        /// <summary>
        ///     Returns the wording of a number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <seealso cref="http://stackoverflow.com/a/2730393/956364" />
        public static String ToVerbalWord( this Int32 number ) {
            if ( number == 0 ) {
                return "zero";
            }

            if ( number < 0 ) {
                return "minus " + ToVerbalWord( Math.Abs( number ) );
            }

            var words = String.Empty;

            if ( number / 1000000 > 0 ) {
                words += ToVerbalWord( number / 1000000 ) + " million ";
                number %= 1000000;
            }

            if ( number / 1000 > 0 ) {
                words += ToVerbalWord( number / 1000 ) + " thousand ";
                number %= 1000;
            }

            if ( number / 100 > 0 ) {
                words += ToVerbalWord( number / 100 ) + " hundred ";
                number %= 100;
            }

            if ( number <= 0 ) {
                return words;
            }

            if ( words != "" ) {
                words += "and ";
            }

            if ( number < 20 ) {
                words += UnitsMap[ number ];
            }
            else {
                words += TensMap[ number / 10 ];
                if ( number % 10 > 0 ) {
                    words += "-" + UnitsMap[ number % 10 ];
                }
            }

            return words;
        }

        /// <summary>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <seealso cref="http://stackoverflow.com/a/7829529/956364" />
        public static String ToVerbalWord( this Decimal number ) {
            if ( number == 0 ) {
                return "zero";
            }

            if ( number < 0 ) {
                return "minus " + ToVerbalWord( Math.Abs( number ) );
            }

            var intPortion = ( Int32 )number;
            var fraction = ( number - intPortion ) * 100;
            var decPortion = ( Int32 )fraction;

            var words = ToVerbalWord( intPortion );
            if ( decPortion <= 0 ) {
                return words;
            }
            words += " and ";
            words += ToVerbalWord( decPortion );
            return words;
        }

        public static IEnumerable<String> ToWords( [CanBeNull] this String sentence ) {

            //TODO try parsing with different splitters?
            // ...do we mabe want the most or least words or avg ?

            ////Regex r = new Regex( _regsplit );
            ////Regex r = new Regex( @"\b(\w+)\s+\b" ); almost works..
            //Regex r = .Split(    (\$\s*[\d,]+\.\d{2})\b
            // (\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?))$
            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"(?=\b\$[\d]+\.\d{4}\s+\b)" ).Split( sentence ) );
            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"([\w(?=\.\W)]+)|(\b\b)|(\$\d+\.\d+)" ).Split( sentence ) );
            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"([\w]+)" ).Split( sentence ) );

            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"\b((([''/,&\:\(\)\$\+\-\*\w\000-\032])|(-*\d+\.\d+[%]*))+[\s]+)+\b[\w'',%\(\)]+[.!?]([''\s]|$)" ).Split( sentence ) );

            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"(\s*\w+\W\s*)" ).Split( sentence ) );
            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"(\s*\$\d+\.\d+\D)" ).Split( sentence ) );

            //Regex r = new Regex( @"(\$\d+\.\d+)" );
            //AIBrain.Brain.BlackBoxClass.Diagnostic( r.Split( sentence ) );

            //AIBrain.Brain.BlackBoxClass.Diagnostic( new Regex( @"(\b\b)|(\$\d+\.\d+)" ).Split( sentence ) );

            if ( sentence == null ) {

                //throw new ArgumentNullException( nameof( sentence ) );
                return new[] { String.Empty };
            }
            var result = RegexByWordBreak.Split( sentence ).ToStrings( " " ).Split( SpaceSplitBy, StringSplitOptions.RemoveEmptyEntries );
            return result;

            //var sb = new StringBuilder( sentence.Length );
            //foreach ( var wrod in Regex_ByWordBreak.Split( sentence ) ) {
            //    sb.AppendFormat( " {0} ", wrod ?? String.Empty );
            //}
            //return sb.ToString().Split( SpaceSplitBy, StringSplitOptions.RemoveEmptyEntries );
        }

        /// <summary>
        ///     Attempt to conver the String into an XmlDocument. An empty XmlDocument will be returned
        ///     if the conversion throws an XmlException
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDoc( this String input ) {
            try {
                var doc = new XmlDocument();
                doc.LoadXml( input );
                return doc;
            }
            catch ( XmlException ) {
                return new XmlDocument();
            }
        }

        [CanBeNull]
        public static String Truncate( this String s, Int32 maxLen ) {
            if ( maxLen < 0 ) {
                throw new ArgumentException( "Maximum length must be greater than 0.", nameof( maxLen ) );
            }
            if ( String.IsNullOrEmpty( s ) ) {
                return s;
            }
            return s.Length <= maxLen ? s : s.Substring( 0, maxLen );
        }

        /// <summary>
        ///     To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        public static String Utf8ByteArrayToString( this Byte[] characters ) => new UTF8Encoding().GetString( characters );

        /// <summary>
        ///     Returns <paramref name="this" /> but culled to a maximum length of <paramref name="maxLength" /> characters.
        /// </summary>
        /// <param name="this">The extended string.</param>
        /// <param name="maxLength">The maximum desired length of the string.</param>
        /// <returns>A string containing the first <c>Min(this.Length, maxLength)</c> characters from the extended string.</returns>
        public static String WithMaxLength( this String @this, Int32 maxLength ) {
            if ( @this == null ) {
                throw new ArgumentNullException( nameof( @this ), "WithMaxLength called on a null string." );
            }
            return @this.Substring( 0, Math.Min( @this.Length, maxLength ) );
        }

        /// <summary>
        ///     <para>Remove duplicate words ONLY if the previous word was the same word.</para>
        /// </summary>
        /// <example>
        ///     Example: "My cat cat likes likes to to to eat food." Should become "My cat likes to eat food."
        /// </example>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String WithoutDuplicateWords( String s ) {
            if ( String.IsNullOrEmpty( s ) ) {
                return String.Empty;
            }

            var words = s.ToWords().ToList();

            //if ( 0 == words.Count() ) { return String.Empty; }
            //if ( 1 == words.Count() ) { return words.FirstOrDefault(); }

            var sb = new StringBuilder( words.Count );
            var prevWord = words.FirstOrDefault();
            sb.Append( prevWord );
            foreach ( var cur in words.Where( cur => !cur.Equals( prevWord ) ) ) {
                sb.Append( $" {cur}" );
            }

            //for ( int idx = 1; idx < words.Count(); idx++ ) {
            //    String wordA = words[ idx - 1 ];
            //    String wordB = words[ idx ];
            //    if ( !wordB.Equals( wordA ) ) {
            //        sb.Append( " " );
            //        sb.Append( wordB );
            //    }
            //}

            return sb.ToString();
        }

        /// <summary>
        ///     Uses a <see cref="Regex" /> to count the number of words.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Int32 WordCount( [NotNull] this String input ) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }
            try {
                return Regex.Matches( input, @"[^\ ^\t^\n]+" ).Count;
            }
            catch ( Exception ) {
                return -1;
            }
        }
    }
}
