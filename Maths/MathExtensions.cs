﻿// Copyright 2018 Rick@AIBrain.org.
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
// "Librainian/MathExtensions.cs" was last cleaned by Rick on 2018/02/03 at 1:08 AM

namespace Librainian.Maths {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Media.Media3D;
    using Collections;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Numbers;
    using Numerics;
    using Parsing;

    public static class MathExtensions {
        [StructLayout( layoutKind: LayoutKind.Explicit )]
        public struct DecimalTo {
            [FieldOffset( offset: 0 )]
            public Decimal Decimal;

            [FieldOffset( offset: 0 )]
            public Guid Guid;

            [FieldOffset( offset: 0 )]
            public FourBytes Bytes;
        }

        public delegate Int32 FibonacciCalculator( Int32 n );

        private const Int32 MaxBits = 32; // you may want to pass this and use generics to allow more or less bits

        /// <summary>
        ///     Table used for reversing bits.
        /// </summary>
        private static readonly Byte[] BitReverseTable256 = { 0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0, 0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8, 0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4, 0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC, 0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2, 0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA, 0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6, 0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE, 0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1, 0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9, 0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5, 0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD, 0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3, 0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB, 0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7, 0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF };

        /// <summary>
        ///     Store the complete list of values that will fit in a 32-bit unsigned integer without overflow.
        /// </summary>
        private static readonly UInt32[] FibonacciLookup = { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711, 28657, 46368, 75025, 121393, 196418, 317811, 514229, 832040, 1346269, 2178309, 3524578, 5702887, 9227465, 14930352, 24157817, 39088169, 63245986, 102334155, 165580141, 267914296, 433494437, 701408733, 1134903170, 1836311903, 2971215073 };

        private static readonly Byte[] RGBRGB565565 = { 5, 6, 5, 5, 6, 5 }; // make some common formats like this

        /// <summary>
        ///     <para>Add <paramref name="tax" /> of <paramref name="number" /> to <paramref name="number" />.</para>
        ///     <para>If the tax is 6% on $50, then you would call this function like this:</para>
        ///     <para>var withTax = AddTax( 50.00, 0.06 );</para>
        ///     <para>Assert( withTax == 53.00 );</para>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="tax"></param>
        /// <returns></returns>
        public static Decimal AddTax( this Decimal number, Decimal tax ) {
            var total = number * ( 1.0m + tax );
            return total;
        }

        /// <summary>
        ///     <para>Add <paramref name="percentTax" /> of <paramref name="number" /> to <paramref name="number" />.</para>
        ///     <para>If the tax is 6% on $50, then you would call this function like this:</para>
        ///     <para>var withTax = AddTaxPercent( 50.00, 6.0 );</para>
        ///     <para>Assert( withTax == 53.00 );</para>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="percentTax"></param>
        /// <returns></returns>
        public static Decimal AddTaxPercent( this Decimal number, Decimal percentTax ) {
            var taxInDecimal = percentTax / 100.0m;
            var tax = 1.0m + taxInDecimal;
            var total = number * tax;
            return total;
        }

        /// <summary>
        ///     Add two <see cref="UInt64" /> without the chance of "throw new
        ///     ArgumentOutOfRangeException( "amount", String.Format( "Values {0} and {1} are loo large
        ///     to handle.", amount, uBigInteger ) );"
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="overflowed"></param>
        public static UBigInteger AddWithoutOverFlow( this UInt64 left, UInt64 right, out Boolean overflowed ) {
            var result = new UBigInteger( value: left ) + new UBigInteger( value: right );
            overflowed = result >= UInt64.MaxValue;
            return result;
        }

        /// <summary>
        ///     Allow <paramref name="left" /> to increase or decrease by a signed number;
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="overflowed"></param>
        /// <returns></returns>
        public static BigInteger AddWithoutOverFlow( this UInt64 left, Int64 right, out Boolean overflowed ) {
            var result = new BigInteger( value: left ) + new BigInteger( value: right );
            overflowed = result >= UInt64.MaxValue;
            return result;
        }

        /// <summary>
        ///     Returns true if <paramref name="number" /> is greater than or equal to 1.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Pure]
        [DebuggerStepThrough]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean Any( this Int16 number ) => number >= 1;

        /// <summary>
        ///     Returns true if <paramref name="number" /> is greater than or equal to 1.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Pure]
        [DebuggerStepThrough]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean Any( this Int32 number ) => number >= 1;

        /// <summary>
        ///     Returns true if <paramref name="number" /> is greater than or equal to 1.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Pure]
        [DebuggerStepThrough]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean Any( this Int64 number ) => number >= 1;

        /// <summary>
        ///     Returns true if <paramref name="number" /> is greater than or equal to 1.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Pure]
        [DebuggerStepThrough]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean Any( this UInt16 number ) => number >= 1;

        /// <summary>
        ///     Returns true if <paramref name="number" /> is greater than or equal to 1.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Pure]
        [DebuggerStepThrough]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean Any( this UInt32 number ) => number >= 1;

        /// <summary>
        ///     Returns true if <paramref name="number" /> is greater than or equal to 1.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Pure]
        [DebuggerStepThrough]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean Any( this UInt64 number ) => number >= 1;

        /// <summary>
        ///     Returns true if <paramref name="number" /> is greater than or equal to 1.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Pure]
        [DebuggerStepThrough]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean Any( this Decimal number ) => number >= 1;

        /// <summary>
        ///     Returns true if <paramref name="number" /> is greater than or equal to 1.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Pure]
        [DebuggerStepThrough]
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static Boolean Any( this Double number ) => number >= 1;

        /// <summary>
        ///     Return true if an <see cref="IComparable" /> value is <see cref="Between{T}" /> two
        ///     inclusive values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="startInclusive"></param>
        /// <param name="endInclusive"></param>
        /// <returns></returns>
        /// <example>5. Between(1, 10) == true</example>
        /// <example>5. Between(10, 1) == true</example>
        /// <example>5. Between(10, 6) == false</example>
        /// <example>5. Between(5, 5)) == true</example>
        public static Boolean Between<T>( this T target, T startInclusive, T endInclusive ) where T : IComparable {
            if ( startInclusive.CompareTo( obj: endInclusive ) == 1 ) {
                return target.CompareTo( obj: startInclusive ) <= 0 && target.CompareTo( obj: endInclusive ) >= 0;
            }

            return target.CompareTo( obj: startInclusive ) >= 0 && target.CompareTo( obj: endInclusive ) <= 0;
        }

        /// <summary>
        ///     Returns a new <typeparamref name="T" /> that is the value of <paramref name="this" />, constrained between
        ///     <paramref name="min" /> and <paramref name="max" />.
        /// </summary>
        /// <param name="this">The extended T.</param>
        /// <param name="min">The minimum value of the <typeparamref name="T" /> that can be returned.</param>
        /// <param name="max">The maximum value of the <typeparamref name="T" /> that can be returned.</param>
        /// <returns>The equivalent to: <c>this &lt; min ? min : this &gt; max ? max : this</c>.</returns>
        public static T Clamp<T>( this T @this, T min, T max ) where T : IComparable<T> {
            if ( @this.CompareTo( other: min ) < 0 ) {
                return min;
            }

            if ( @this.CompareTo( other: max ) > 0 ) {
                return max;
            }

            return @this;
        }

        /// <summary>
        ///     Combine two <see cref="UInt32" /> values into one <see cref="UInt64" /> value. Use
        ///     Split() for the reverse.
        /// </summary>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <returns></returns>
        public static UInt64 Combine( this UInt32 high, UInt32 low ) => ( ( UInt64 )high << 32 ) | low;

        /// <summary>
        ///     Combine two bytes into one <see cref="UInt16" />.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static UInt16 CombineBytes( this Byte low, Byte high ) => BitConverter.ToUInt16( value: BitConverter.IsLittleEndian ? new[] { high, low } : new[] { low, high }, startIndex: 0 );

        /// <summary>
        ///     Combine two bytes into one <see cref="UInt16" /> with little endianess.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        /// <seealso cref="CombineTwoBytesLittleEndianess" />
        public static UInt16 CombineTwoBytesHighEndianess( this Byte low, Byte high ) => ( UInt16 )( high + ( low << 8 ) );

        /// <summary>
        ///     Combine two bytes into one <see cref="UInt16" /> with little endianess.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        /// <seealso cref="CombineTwoBytesHighEndianess" />
        public static UInt16 CombineTwoBytesLittleEndianess( this Byte low, Byte high ) => ( UInt16 )( low + ( high << 8 ) );

        public static Byte[] Concat( this Byte[] first, Byte[] second ) {
            var buffer = new Byte[first.Length + second.Length];
            Buffer.BlockCopy( src: first, srcOffset: 0, dst: buffer, dstOffset: 0, count: first.Length );
            Buffer.BlockCopy( src: second, srcOffset: 0, dst: buffer, dstOffset: first.Length, count: second.Length );
            return buffer;
        }

        public static Byte[] Concat( this Byte[] first, Byte second ) {
            var buffer = new Byte[first.Length + 1];
            Buffer.BlockCopy( src: first, srcOffset: 0, dst: buffer, dstOffset: 0, count: first.Length );
            buffer[buffer.Length - 1] = second;
            return buffer;
        }

        public static IEnumerable<T> Concat<T>( this IEnumerable<T> first, T second ) {
            foreach ( var item in first ) {
                yield return item;
            }

            yield return second;
        }

        /// <summary>
        ///     ConvertBigIntToBcd
        /// </summary>
        /// <param name="numberToConvert"></param>
        /// <param name="howManyBytes"></param>
        /// <returns></returns>
        /// <seealso cref="http://github.com/mkadlec/ConvertBigIntToBcd/blob/master/ConvertBigIntToBcd.cs" />
        public static Byte[] ConvertBigIntToBcd( this Int64 numberToConvert, Int32 howManyBytes ) {
            var convertedNumber = new Byte[howManyBytes];
            var strNumber = numberToConvert.ToString();
            var currentNumber = String.Empty;

            for ( var i = 0; i < howManyBytes; i++ ) {
                convertedNumber[i] = 0xff;
            }

            for ( var i = 0; i < strNumber.Length; i++ ) {
                currentNumber += strNumber[index: i];

                if ( i == strNumber.Length - 1 && i % 2 == 0 ) {
                    convertedNumber[i / 2] = 0xf;
                    convertedNumber[i / 2] |= ( Byte )( ( Int32.Parse( s: currentNumber ) % 10 ) << 4 );
                }

                if ( i % 2 == 0 ) {
                    continue;
                }

                var value = Int32.Parse( s: currentNumber );
                convertedNumber[( i - 1 ) / 2] = ( Byte )( value % 10 );
                convertedNumber[( i - 1 ) / 2] |= ( Byte )( ( value / 10 ) << 4 );
                currentNumber = String.Empty;
            }

            return convertedNumber;
        }

        /// <summary>
        ///     Counts the number of set (bit = 1) bits in a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Number of set (1) bits.</returns>
        public static Int32 CountBits( this Int64 value ) {
            Int32 i;
            for ( i = 0; value != 0; i++ ) {
                value &= value - 1;
            }

            return i;
        }

        /// <summary>
        ///     Counts the number of set (bit = 1) bits in a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Number of set (1) bits.</returns>
        public static Int32 CountBits( this UInt64 value ) {
            Int32 i;
            for ( i = 0; value != 0; i++ ) {
                value &= value - 1;
            }

            return i;
        }

        /// <summary>
        ///     Counts the number of set (bit = 1) bits in a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Number of set (1) bits.</returns>
        public static Int32 CountBits( this Int32 value ) {
            Int32 i;
            for ( i = 0; value != 0; i++ ) {
                value &= value - 1;
            }

            return i;
        }

        /// <summary>
        ///     Counts the number of set (bit = 1) bits in a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Number of set (1) bits.</returns>
        public static Int32 CountBits( this UInt32 value ) {
            Int32 i;
            for ( i = 0; value != 0; i++ ) {
                value &= value - 1;
            }

            return i;
        }

        /// <summary>
        ///     Counts the number of set (bit = 1) bits in a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Number of set (1) bits.</returns>
        public static Int32 CountBits( this Int16 value ) {
            Int32 i;
            for ( i = 0; value != 0; i++ ) {
                value &= ( Int16 )( value - 1 );
            }

            return i;
        }

        /// <summary>
        ///     Counts the number of set (bit = 1) bits in a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Number of set (1) bits.</returns>
        public static Int32 CountBits( this UInt16 value ) {
            Int32 i;
            for ( i = 0; value != 0; i++ ) {
                value &= ( UInt16 )( value - 1 );
            }

            return i;
        }

        /// <summary>
        ///     Counts the number of set (bit = 1) bits in a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Number of set (1) bits.</returns>
        public static Int32 CountBits( this Byte value ) {
            Int32 i;
            for ( i = 0; value != 0; i++ ) {
                value &= ( Byte )( value - 1 );
            }

            return i;
        }

        /// <summary>
        ///     Counts the number of set (bit = 1) bits in a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Number of set (1) bits.</returns>
        public static Int32 CountBits( this SByte value ) {
            Int32 i;
            for ( i = 0; value != 0; i++ ) {
                value &= ( SByte )( value - 1 );
            }

            return i;
        }

        public static Double Crop( this Double x ) => Math.Truncate( d: x * 100.0D ) / 100.0D;

        public static Single Crop( this Single x ) => ( Single )( Math.Truncate( d: x * 100.0f ) / 100.0f );

        public static Single Cubed( this Single number ) => number * number * number;

        public static Double Cubed( this Double number ) => number * number * number;

        public static Decimal Cubed( this Decimal number ) => number * number * number;

        public static String Decimal2Packed( this Decimal d ) {
            var output = new Boolean[10];
            var input = new Boolean[12];

            for ( var i = 0; i < 3; i++ ) {
                var a = ( Int32 )( ( Int32 )d / Math.Pow( x: 10, y: i ) ) % 10;
                for ( var j = 0; j < 4; j++ ) {
                    input[j + i * 4] = ( a & ( 1 << j ) ) != 0;
                }
            }

            output[0] = input[0];
            output[1] = input[7] | ( input[11] & input[3] ) | ( !input[11] & input[1] );
            output[2] = input[11] | ( input[7] & input[3] ) | ( !input[7] & input[2] );
            output[3] = input[11] | input[7] | input[3];
            output[4] = input[4];
            output[5] = input[5] | ( !input[11] & input[7] & input[1] ) | ( input[11] & input[3] );
            output[6] = ( input[6] & ( !input[11] | !input[3] ) ) | ( !input[11] & input[7] & input[2] ) | ( input[7] & input[3] );
            output[7] = input[8];
            output[8] = input[9] | ( input[11] & input[1] ) | ( input[11] & input[5] & input[3] );
            output[9] = input[10] | ( input[11] & input[2] ) | ( input[11] & input[6] & input[3] );

            var sb = new StringBuilder();
            for ( var i = 9; i >= 0; i-- ) {
                sb.Append( value: output[i] ? '1' : '0' );
            }

            return sb.ToString();
        }

        /// <summary>
        ///     <para>Return the smallest possible value above <see cref="Decimal.Zero" /> for a <see cref="Decimal" />.</para>
        ///     <para>1E-28</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Pure]
        public static Decimal Epsilon( this Decimal number ) => MathConstants.EpsilonDecimal;

        public static Double Erf( this Double x ) {
            // constants
            const Double a1 = 0.254829592;
            const Double a2 = -0.284496736;
            const Double a3 = 1.421413741;
            const Double a4 = -1.453152027;
            const Double a5 = 1.061405429;
            const Double p = 0.3275911;

            // Save the sign of x
            var sign = x < 0 ? -1 : 1;
            x = Math.Abs( value: x );

            // A&S formula 7.1.26
            var t = 1.0 / ( 1.0 + p * x );
            var y = 1.0 - ( ( ( ( a5 * t + a4 ) * t + a3 ) * t + a2 ) * t + a1 ) * t * Math.Exp( d: -x * x );

            return sign * y;
        }

        /// <summary>
        ///     Compute fibonacci series up to Max (&gt; 1).
        ///     Example: foreach (int i in Fib(10)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static IEnumerable<Int32> Fib( Int32 max ) {
            var a = 0;
            var b = 1;
            yield return 1;

            for ( var i = 0; i < max - 1; i++ ) {
                var c = a + b;
                yield return c;

                a = b;
                b = c;
            }
        }

        public static UInt32 FibonacciSequence( this UInt32 n ) => FibonacciLookup[n];

        [DebuggerStepThrough]
        [Pure]
        public static Double FiftyPercentOf( this Double x ) {
            var result = x * 0.5;
            return result < 1.0 ? 1 : result;
        }

        [DebuggerStepThrough]
        [Pure]
        public static Int32 FiftyPercentOf( this Int32 x ) {
            var result = x * 0.5;
            return result < 1.0 ? 1 : ( Int32 )result;
        }

        public static Double ForceBounds( this Double thisDouble, Double minLimit, Double maxLimit ) => Math.Max( val1: Math.Min( val1: thisDouble, val2: maxLimit ), val2: minLimit );

        [DebuggerStepThrough]
        [Pure]
        public static Int32 FractionOf( this Int32 x, Double top, Double bottom ) {
            var result = top * x / bottom;
            return result < 1.0 ? 1 : ( Int32 )result;
        }

        [DebuggerStepThrough]
        [Pure]
        public static Double FractionOf( this Double x, Double top, Double bottom ) => top * x / bottom;

        [DebuggerStepThrough]
        [Pure]
        public static Single FractionOf( this Single x, Single top, Single bottom ) => top * x / bottom;

        [DebuggerStepThrough]
        [Pure]
        public static UInt64 FractionOf( this UInt64 x, UInt64 top, UInt64 bottom ) => top * x / bottom;

        /// <summary>
        ///     Greatest Common Divisor for int
        /// </summary>
        /// <remarks>Uses recursion, passing a remainder each time.</remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Int32 gcd( this Int32 x, Int32 y ) {
            while ( true ) {
                if ( y == 0 ) {
                    return x;
                }

                var x1 = x;
                x = y;
                y = x1 % y;
            }
        }

        /// <summary>
        ///     Greatest Common Divisor for long
        /// </summary>
        /// <remarks>Uses recursion, passing a remainder each time.</remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Int64 gcd( this Int64 x, Int64 y ) {
            while ( true ) {
                if ( y == 0 ) {
                    return x;
                }

                var x1 = x;
                x = y;
                y = x1 % y;
            }
        }

        /// <summary>
        ///     Greatest Common Divisor for int
        /// </summary>
        /// <remarks>Uses a while loop and remainder.</remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Int32 GCD( Int32 a, Int32 b ) {
            while ( b != 0 ) {
                var remainder = a % b;
                a = b;
                b = remainder;
            }

            return a;
        }

        /// <summary>
        ///     Greatest Common Divisor for long
        /// </summary>
        /// <remarks>Uses a while loop and remainder.</remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Int64 GCD( Int64 a, Int64 b ) {
            while ( b != 0 ) {
                var remainder = a % b;
                a = b;
                b = remainder;
            }

            return a;
        }

        /// <summary>
        ///     Greatest Common Divisor for int
        /// </summary>
        /// <remarks>
        ///     More like the ancient greek Euclid originally devised. It uses a while loop with subtraction.
        /// </remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Int32 GCD2( Int32 x, Int32 y ) {
            while ( x != y ) {
                if ( x > y ) {
                    x = x - y;
                }
                else {
                    y = y - x;
                }
            }

            return x;
        }

        /// <summary>
        ///     Greatest Common Divisor for long
        /// </summary>
        /// <remarks>
        ///     More like the ancient greek Euclid originally devised. It uses a while loop with subtraction.
        /// </remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Int64 GCD2( Int64 x, Int64 y ) {
            while ( x != y ) {
                if ( x > y ) {
                    x = x - y;
                }
                else {
                    y = y - x;
                }
            }

            return x;
        }

        [DebuggerStepThrough]
        [Pure]
        public static Single Half( this Single number ) => number / 2.0f;

        [DebuggerStepThrough]
        [Pure]
        public static Double Half( this Double number ) => number / 2.0d;

        [DebuggerStepThrough]
        [Pure]
        public static Byte Half( this Byte number ) => ( Byte )( number / 2 );

        [DebuggerStepThrough]
        [Pure]
        public static TimeSpan Half( this TimeSpan timeSpan ) => TimeSpan.FromTicks( value: timeSpan.Ticks.Half() );

        [DebuggerStepThrough]
        [Pure]
        public static Int32 Half( this Int32 number ) => ( Int32 )( number / 2.0f );

        [DebuggerStepThrough]
        [Pure]
        public static Int16 Half( this Int16 number ) => ( Int16 )( number / 2.0f );

        [DebuggerStepThrough]
        [Pure]
        public static UInt16 Half( this UInt16 number ) => ( UInt16 )( number / 2.0f );

        [DebuggerStepThrough]
        [Pure]
        public static UInt32 Half( this UInt32 number ) => ( UInt32 )( number / 2.0f );

        [DebuggerStepThrough]
        [Pure]
        public static UInt64 Half( this UInt64 number ) => ( UInt64 )( number / 2.0d );

        [DebuggerStepThrough]
        [Pure]
        public static Int64 Half( this Int64 number ) => ( Int64 )( number / 2.0d );

        [DebuggerStepThrough]
        [Pure]
        public static Decimal Half( this Decimal number ) => number / 2.0m;

        /// <summary>
        ///     <para>
        ///         If the <paramref name="number" /> is less than <see cref="Decimal.Zero" />, then return
        ///         <see cref="Decimal.Zero" />.
        ///     </para>
        ///     <para>Otherwise return the <paramref name="number" />.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static Decimal IfLessThanZeroThenZero( this Decimal number ) => number < Decimal.Zero ? Decimal.Zero : number;

        /// <summary>
        ///     <para>
        ///         If the <paramref name="number" /> is less than <see cref="BigInteger.Zero" />, then
        ///         return <see cref="Decimal.Zero" />.
        ///     </para>
        ///     <para>Otherwise return the <paramref name="number" />.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static BigInteger IfLessThanZeroThenZero( this BigInteger number ) => number < BigInteger.Zero ? BigInteger.Zero : number;

        /// <summary>
        ///     <para>
        ///         If the <paramref name="number" /> is less than <see cref="BigRational.Zero" />, then
        ///         return <see cref="Decimal.Zero" />.
        ///     </para>
        ///     <para>Otherwise return the <paramref name="number" />.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static BigRational IfLessThanZeroThenZero( this BigRational number ) => number < BigRational.Zero ? BigRational.Zero : number;

        /// <summary>
        ///     <para>
        ///         If the <paramref name="number" /> is less than <see cref="BigRational.Zero" />, then
        ///         return <see cref="Decimal.Zero" />.
        ///     </para>
        ///     <para>Otherwise return the <paramref name="number" />.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static BigRational IfLessThanZeroThenZero( this BigRational? number ) {
            if ( !number.HasValue || number <= BigRational.Zero ) {
                return BigRational.Zero;
            }

            return number.Value;
        }

        [DebuggerStepThrough]
        [Pure]
        public static Boolean IsEven( this Int32 value ) => 0 == value % 2;

        [DebuggerStepThrough]
        [Pure]
        public static Boolean IsEven( this Int64 value ) => 0 == value % 2;

        [DebuggerStepThrough]
        [Pure]
        public static Boolean IsNegative( this Single value ) => value < 0.0f;

        [DebuggerStepThrough]
        [Pure]
        public static Boolean IsNumber( this Single value ) {
            if ( Single.IsNaN( f: value ) ) {
                return false;
            }

            return !Single.IsInfinity( f: value );
        }

        [DebuggerStepThrough]
        [Pure]
        public static Boolean IsNumber( this Double value ) {
            if ( Double.IsNaN( d: value ) ) {
                return false;
            }

            return !Double.IsInfinity( d: value );
        }

        [DebuggerStepThrough]
        [Pure]
        public static Boolean IsOdd( this Int32 value ) => 0 != value % 2;

        [DebuggerStepThrough]
        [Pure]
        public static Boolean IsOdd( this Int64 value ) => 0 != value % 2;

        [DebuggerStepThrough]
        [Pure]
        public static Boolean IsPositive( this Single value ) => value > 0.0f;

        [DebuggerStepThrough]
        [Pure]
        public static Boolean IsPowerOfTwo( this Int32 number ) => ( number & -number ) == number;

        /// <summary>
        ///     Linearly interpolates between two values.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="target">Target value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        [DebuggerStepThrough]
        [Pure]
        public static Single Lerp( this Single source, Single target, Single amount ) => source + ( target - source ) * amount;

        /// <summary>
        ///     Linearly interpolates between two values.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="target">Target value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        [DebuggerStepThrough]
        [Pure]
        public static Double Lerp( this Double source, Double target, Single amount ) => source + ( target - source ) * amount;

        /// <summary>
        ///     Linearly interpolates between two values.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="target">Target value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        [DebuggerStepThrough]
        [Pure]
        public static UInt64 Lerp( this UInt64 source, UInt64 target, Single amount ) => ( UInt64 )( source + ( target - source ) * amount );

        /// <summary>
        ///     Linearly interpolates between two values.
        /// </summary>
        /// <param name="source">Source value.</param>
        /// <param name="target">Target value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        [DebuggerStepThrough]
        [Pure]
        public static UInt32 Lerp( this UInt32 source, UInt32 target, Single amount ) => ( UInt32 )( source + ( target - source ) * amount );

        [DebuggerStepThrough]
        [Pure]
        public static Double LogFactorial( this Int32 n ) {
            if ( n < 0 ) {
                throw new ArgumentOutOfRangeException();
            }

            if ( n <= 254 ) {
                return MathConstants.Logfactorialtable[n];
            }

            var x = n + 1d;
            return ( x - 0.5 ) * Math.Log( d: x ) - x + 0.5 * Math.Log( d: 2 * Math.PI ) + 1.0 / ( 12.0 * x );
        }

        /// <summary>
        ///     compute log(1+x) without losing precision for small values of x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static Double LogOnePlusX( this Double x ) {
            if ( x <= -1.0 ) {
                throw new ArgumentOutOfRangeException( paramName: nameof( x ), message: $"Invalid input argument: {x}" );
            }

            if ( Math.Abs( value: x ) > 1e-4 ) {
                // x is large enough that the obvious evaluation is OK
                return Math.Log( d: 1.0 + x );
            }

            // Use Taylor approx. log(1 + x) = x - x^2/2 with error roughly x^3/3 since |x| < 10^-4,
            // |x|^3 < 10^-12, relative error less than 10^-8
            return ( -0.5 * x + 1.0 ) * x;
        }

        [DebuggerStepThrough]
        [Pure]
        public static Boolean Near( this Double number, Double target ) => Math.Abs( value: number - target ) <= Double.Epsilon;

        [DebuggerStepThrough]
        [Pure]
        public static Boolean Near( this Single number, Single target ) => Math.Abs( value: number - target ) <= Single.Epsilon;

        [DebuggerStepThrough]
        [Pure]
        public static Boolean Near( this Point here, Point there ) => here.X.Near( target: there.X ) && here.Y.Near( target: there.Y );

        [DebuggerStepThrough]
        [Pure]
        public static Boolean Near( this Point3D here, Point3D there ) => here.X.Near( target: there.X ) && here.Y.Near( target: there.Y ) && here.Z.Near( target: there.Z );

        [DebuggerStepThrough]
        [Pure]
        public static Boolean Near( this Decimal number, Decimal target ) => Math.Abs( value: number - target ) <= MathConstants.EpsilonDecimal;

        [DebuggerStepThrough]
        [Pure]
        public static Boolean Near( this BigRational number, BigRational target ) {
            var difference = number - target;
            if ( difference < BigRational.Zero ) {
                difference = -difference;
            }

            return difference <= MathConstants.EpsilonDecimal;
        }

        //public static Boolean Near( this PointF here, PointF there ) {
        //    return here.X.Near( there.X ) && here.Y.Near( there.Y );
        //}
        [DebuggerStepThrough]
        [Pure]
        public static Boolean Near( this BigInteger number, BigInteger target ) {
            var difference = number - target;
            return BigInteger.Zero == difference;
        }

        [DebuggerStepThrough]
        [Pure]
        public static Boolean Near( this UInt64 number, UInt64 target ) => number - target <= UInt64.MinValue;

        [DebuggerStepThrough]
        [Pure]
        public static Boolean Near( this Int64 number, Int64 target ) => number - target <= Int64.MinValue;

        [DebuggerStepThrough]
        [Pure]
        public static Boolean Near( this UBigInteger number, UBigInteger target ) => number - target <= UBigInteger.Epsilon;

        [DebuggerStepThrough]
        [Pure]
        public static Double Nested( this Double x ) => Math.Sqrt( d: x * 100.0 ) / 100.0d;

        [DebuggerStepThrough]
        [Pure]
        public static Single Nested( this Single x ) => ( Single )( Math.Sqrt( d: x * 100.0 ) / 100.0f );

        [Obsolete]
        [DebuggerStepThrough]
        [Pure]
        public static Int32 Nested( this Int32 x ) => ( Int32 )Math.Sqrt( d: x );

        /// <summary>
        ///     Remove all the trailing zeros from the decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static Decimal Normalize( this Decimal value ) => value / 1.000000000000000000000000000000000m;

        /// <summary>
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <seealso cref="http://stackoverflow.com/a/18363540/956364" />
        [DebuggerStepThrough]
        [Pure]
        public static Decimal NthRoot( this Decimal baseValue, Int32 n ) {
            if ( n == 1 ) {
                return baseValue;
            }

            Decimal deltaX;
            var x = 0.1M;
            do {
                deltaX = ( baseValue / x.Pow( n: n - 1 ) - x ) / n;
                x += deltaX;
            } while ( Math.Abs( value: deltaX ) > 0 );

            return x;
        }

        [DebuggerStepThrough]
        [Pure]
        public static UInt64 OneHundreth( this UInt64 x ) => x / 100;

        [DebuggerStepThrough]
        [Pure]
        public static UInt64 OneQuarter( this UInt64 x ) => x / 4;

        [DebuggerStepThrough]
        [Pure]
        public static Single OneQuarter( this Single x ) => x / 4.0f;

        [DebuggerStepThrough]
        [Pure]
        public static UInt64 OneTenth( this UInt64 x ) => x / 10;

        [DebuggerStepThrough]
        [Pure]
        public static Single OneThird( this Single x ) => x / 3.0f;

        /// <summary>
        ///     Finds the parity of a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True for even, False for odd.</returns>
        [DebuggerStepThrough]
        [Pure]
        public static Boolean Parity( this Int64 value ) {
            Int64 i;
            for ( i = 0; value != 0; value >>= 1 ) {
                i += value & 1;
            }

            return i % 2 == 1;
        }

        /// <summary>
        ///     Finds the parity of a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True for even, False for odd.</returns>
        [DebuggerStepThrough]
        [Pure]
        public static Boolean Parity( this UInt64 value ) {
            UInt64 i;
            for ( i = 0; value != 0; value >>= 1 ) {
                i += value & 1;
            }

            return i % 2 == 1;
        }

        /// <summary>
        ///     Finds the parity of a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True for even, False for odd.</returns>
        [DebuggerStepThrough]
        [Pure]
        public static Boolean Parity( this Int32 value ) {
            Int32 i;
            for ( i = 0; value != 0; value >>= 1 ) {
                i += value & 1;
            }

            return i % 2 == 1;
        }

        /// <summary>
        ///     Finds the parity of a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True for even, False for odd.</returns>
        [DebuggerStepThrough]
        [Pure]
        public static Boolean Parity( this UInt32 value ) {
            UInt32 i;
            for ( i = 0; value != 0; value >>= 1 ) {
                i += value & 1;
            }

            return i % 2 == 1;
        }

        /// <summary>
        ///     Finds the parity of a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True for even, False for odd.</returns>
        [DebuggerStepThrough]
        [Pure]
        public static Boolean Parity( this Int16 value ) {
            Int32 i;
            for ( i = 0; value != 0; value >>= 1 ) {
                i += value & 1;
            }

            return i % 2 == 1;
        }

        /// <summary>
        ///     Finds the parity of a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True for even, False for odd.</returns>
        [DebuggerStepThrough]
        [Pure]
        public static Boolean Parity( this UInt16 value ) {
            Int32 i;
            for ( i = 0; value != 0; value >>= 1 ) {
                i += value & 1;
            }

            return i % 2 == 1;
        }

        /// <summary>
        ///     Finds the parity of a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True for even, False for odd.</returns>
        [DebuggerStepThrough]
        [Pure]
        public static Boolean Parity( this SByte value ) {
            Int32 i;
            for ( i = 0; value != 0; value >>= 1 ) {
                i += value & 1;
            }

            return i % 2 == 1;
        }

        /// <summary>
        ///     Finds the parity of a given value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True for even, False for odd.</returns>
        [DebuggerStepThrough]
        [Pure]
        public static Boolean Parity( this Byte value ) => ( ( ( ( UInt64 )( value * 0x0101010101010101 ) & 0x8040201008040201 ) % 0x1FF ) & 1 ) != 0;

        [DebuggerStepThrough]
        [Pure]
        public static Double Phi( this Double x ) {
            // constants
            const Double a1 = 0.254829592;
            const Double a2 = -0.284496736;
            const Double a3 = 1.421413741;
            const Double a4 = -1.453152027;
            const Double a5 = 1.061405429;
            const Double p = 0.3275911;

            // Save the sign of x
            var sign = x < 0 ? -1 : 1;
            x = Math.Abs( value: x ) / Math.Sqrt( d: 2.0 );

            // A&S formula 7.1.26
            var t = 1.0 / ( 1.0 + p * x );
            var y = 1.0 - ( ( ( ( a5 * t + a4 ) * t + a3 ) * t + a2 ) * t + a1 ) * t * Math.Exp( d: -x * x );

            return 0.5 * ( 1.0 + sign * y );
        }

        [DebuggerStepThrough]
        [Pure]
        public static Decimal Pow( this Decimal baseValue, Int32 n ) {
            for ( var i = 0; i < n - 1; i++ ) {
                baseValue *= baseValue;
            }

            return baseValue;
        }

        /// <summary>
        ///     <see cref="Decimal" /> raised to the nth power.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <seealso cref="http://stackoverflow.com/questions/429165/raising-a-Decimal-to-a-power-of-Decimal" />
        [DebuggerStepThrough]
        [Pure]
        public static Decimal Pow( this Decimal x, UInt32 n ) {
            var a = 1m;
            var e = new BitArray( bytes: BitConverter.GetBytes( value: n ) );

            for ( var i = e.Count - 1; i >= 0; --i ) {
                a *= a;
                if ( e[index: i] ) {
                    a *= x;
                }
            }

            return a;
        }

        public static IEnumerable<Int32> Primes( this Int32 max ) {
            yield return 2;
            var found = new List<Int32> { 3 };
            var candidate = 3;
            while ( candidate <= max ) {
                var candidate1 = candidate;
                var candidate2 = candidate;
                if ( found.TakeWhile( prime => prime * prime <= candidate1 ).All( prime => candidate2 % prime != 0 ) ) {
                    found.Add( item: candidate );
                    yield return candidate;
                }

                candidate += 2;
            }
        }

        [DebuggerStepThrough]
        [Pure]
        public static Decimal Quarter( this Decimal number ) => number / 4.0m;

        /// <summary>
        ///     Reverses the bit order of a variable (ie: 0100 1000 becomes 0001 0010)
        /// </summary>
        /// <param name="source">Source value to reverse</param>
        /// <returns>Input value with reversed bits</returns>
        [DebuggerStepThrough]
        [Pure]
        public static Byte ReverseBits( this Byte source ) => ( Byte )( ( ( ( ( source * 0x0802 ) & 0x22110 ) | ( ( source * 0x8020 ) & 0x88440 ) ) * 0x10101 ) >> 16 );

        /// <summary>
        ///     Reverses the bit order of a variable (ie: 0100 1000 becomes 0001 0010)
        /// </summary>
        /// <param name="source">Source value to reverse</param>
        /// <returns>Input value with reversed bits</returns>
        public static Int32 ReverseBits( this Int32 source ) => ( BitReverseTable256[source & 0xff] << 24 ) | ( BitReverseTable256[( source >> 8 ) & 0xff] << 16 ) | ( BitReverseTable256[( source >> 16 ) & 0xff] << 8 ) | BitReverseTable256[( source >> 24 ) & 0xff];

        /// <summary>
        ///     Reverses the bit order of a variable (ie: 0100 1000 becomes 0001 0010)
        /// </summary>
        /// <param name="source">Source value to reverse</param>
        /// <returns>Input value with reversed bits</returns>
        public static UInt32 ReverseBits( this UInt32 source ) => ( UInt32 )( ( BitReverseTable256[source & 0xff] << 24 ) | ( BitReverseTable256[( source >> 8 ) & 0xff] << 16 ) | ( BitReverseTable256[( source >> 16 ) & 0xff] << 8 ) | BitReverseTable256[( source >> 24 ) & 0xff] );

        /// <summary>
        ///     Reverses the bit order of a variable (ie: 0100 1000 becomes 0001 0010)
        /// </summary>
        /// <param name="source">Source value to reverse</param>
        /// <returns>Input value with reversed bits</returns>
        public static UInt16 ReverseBits( this UInt16 source ) {
            source = ( UInt16 )( ( ( source >> 1 ) & 0x5555 ) | ( ( source & 0x5555 ) << 1 ) );
            source = ( UInt16 )( ( ( source >> 2 ) & 0x3333 ) | ( ( source & 0x3333 ) << 2 ) );
            source = ( UInt16 )( ( ( source >> 4 ) & 0x0F0F ) | ( ( source & 0x0F0F ) << 4 ) );
            return ( UInt16 )( ( source >> 8 ) | ( source << 8 ) );
        }

        /// <summary>
        ///     Reverses the bit order of a variable (ie: 0100 1000 becomes 0001 0010)
        /// </summary>
        /// <param name="source">Source value to reverse</param>
        /// <returns>Input value with reversed bits</returns>
        public static Int16 ReverseBits( this Int16 source ) {
            source = ( Int16 )( ( ( source >> 1 ) & 0x5555 ) | ( ( source & 0x5555 ) << 1 ) );
            source = ( Int16 )( ( ( source >> 2 ) & 0x3333 ) | ( ( source & 0x3333 ) << 2 ) );
            source = ( Int16 )( ( ( source >> 4 ) & 0x0F0F ) | ( ( source & 0x0F0F ) << 4 ) );
            return ( Int16 )( ( source >> 8 ) | ( source << 8 ) );
        }

        public static Double Root( this Double x, Double root ) => Math.Pow( x: x, y: 1.0 / root );

        public static Double Root( this Decimal x, Decimal root ) => Math.Pow( x: ( Double )x, y: ( Double )( 1.0m / root ) );

        /// <summary>
        ///     Truncate, don't round. Just chop it off.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns>Bitcoin ftw!</returns>
        public static Decimal Sanitize( this Decimal number, UInt16 decimalPlaces = 8 ) {
            number *= ( Decimal )Math.Pow( x: 10, y: decimalPlaces );

            number = ( UInt64 )number; //Truncate, don't round. Just chop it off.

            number *= ( Decimal )Math.Pow( x: 10, y: -decimalPlaces );

            return number;
        }

        /// <summary>
        ///     Smooths a value to between 0 and 1.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [Pure]
        public static Double Sigmoid0To1( this Double value ) => 1.0D / ( 1.0D + Math.Exp( d: -value ) );

        /// <summary>
        ///     Smooths a value to between -1 and 1.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <seealso cref="http://www.wolframalpha.com/input/?i=1+-+%28+2+%2F+%281+%2B+Exp%28+v+%29+%29+%29%2C+v+from+-10+to+10" />
        [DebuggerStepThrough]
        [Pure]
        public static Double SigmoidNeg1To1( this Double value ) => 1.0D - 2.0D / ( 1.0D + Math.Exp( d: value ) );

        public static Double Slope( [NotNull] this List<TimeProgression> data ) {
            if ( data == null ) {
                throw new ArgumentNullException( paramName: nameof( data ) );
            }

            var averageX = data.Average( d => d.MillisecondsPassed );
            var averageY = data.Average( d => d.Progress );

            return data.Sum( d => ( d.MillisecondsPassed - averageX ) * ( d.Progress - averageY ) ) / data.Sum( d => Math.Pow( x: d.MillisecondsPassed - averageX, y: 2 ) );
        }

        /// <summary>
        ///     Return the integer part and the fraction parts of a <see cref="Decimal" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Tuple<Decimal, Decimal> Split( this Decimal value ) {
            var parts = value.ToString( "R" ).Split( '.' );
            var result = new Tuple<Decimal, Decimal>( item1: Decimal.Parse( s: parts[0] ), item2: Decimal.Parse( s: "0." + parts[1] ) );
            return result;
        }

        /// <summary>
        ///     Return the integer part and the fraction parts of a <see cref="Double" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Tuple<Double, Double> Split( this Double value ) {
            var parts = value.ToString( "R" ).Split( '.' );
            return new Tuple<Double, Double>( item1: Double.Parse( s: parts[0] ), item2: Double.Parse( s: "0." + parts[1] ) );
        }

        /// <summary>
        ///     Split one <see cref="UInt64" /> value into two <see cref="UInt32" /> values. Use
        ///     <see cref="Combine" /> for the reverse.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        public static void Split( this UInt64 value, out UInt32 high, out UInt32 low ) {
            high = ( UInt32 )( value >> 32 );
            low = ( UInt32 )( value & UInt32.MaxValue );
        }

        [DebuggerStepThrough]
        [Pure]
        public static Single Squared( this Single number ) => number * number;

        [DebuggerStepThrough]
        [Pure]
        public static Double Squared( this Double number ) => number * number;

        [DebuggerStepThrough]
        [Pure]
        public static Decimal Squared( this Decimal number ) => number * number;

        public static Double SquareRootOfProducts( this IEnumerable<Double> data ) {
            var sorted = new List<Double>( collection: data.Where( d => Math.Abs( value: d ) >= Double.Epsilon ).OrderBy( d => d ) );

            var aggregate = BigRational.One;

            while ( sorted.Any() ) {
                sorted.TakeFirst( item: out var smallest );
                smallest.Should().NotBe( unexpected: Double.NaN );
                smallest.Should().NotBe( unexpected: Double.NegativeInfinity );
                smallest.Should().NotBe( unexpected: Double.PositiveInfinity );

                if ( !sorted.TakeLast( item: out var largest ) ) {
                    largest = 1;
                }

                largest.Should().NotBe( unexpected: Double.NaN );
                largest.Should().NotBe( unexpected: Double.NegativeInfinity );
                largest.Should().NotBe( unexpected: Double.PositiveInfinity );

                aggregate = aggregate * smallest;
                aggregate = aggregate * largest;

                //aggregate.Should().NotBe( Double.NaN );
                //aggregate.Should().NotBe( Double.NegativeInfinity );
                //aggregate.Should().NotBe( Double.PositiveInfinity );
            }

            //foreach ( Double d in data ) {aggregate = aggregate * d;}
            return Math.Sqrt( d: ( Double )aggregate );
        }

        public static Decimal SquareRootOfProducts( this IEnumerable<Decimal> data ) {
            var aggregate = data.Aggregate( seed: 1.0m, func: ( current, d ) => current * d );
            return ( Decimal )Math.Sqrt( d: ( Double )aggregate );
        }

        /// <summary>
        ///     Subtract <paramref name="tax" /> of <paramref name="total" /> from <paramref name="total" />.
        ///     <para>If the tax was 6% on $53, then you would call this function like this:</para>
        ///     <para>var withTax = SubtractTax( 53.00, 0.06 );</para>
        ///     <para>Assert( withTax == 50.00 );</para>
        /// </summary>
        /// <param name="total"></param>
        /// <param name="tax"></param>
        /// <returns></returns>
        public static Decimal SubtractTax( this Decimal total, Decimal tax ) {
            var taxed = total / ( 1.0m + tax );
            return taxed;
        }

        /// <summary>
        ///     Subtract <paramref name="right" /> away from <paramref name="left" /> without the chance
        ///     of "throw new ArgumentOutOfRangeException( "amount", String.Format( "Values {0} and {1}
        ///     are loo small to handle.", amount, uBigInteger ) );"
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static UInt64 SubtractWithoutUnderFlow( this UInt64 left, UInt64 right ) {
            var integer = new UBigInteger( value: left ) - new UBigInteger( value: right );
            if ( integer < new UBigInteger( value: UInt64.MinValue ) ) {
                return UInt64.MinValue;
            }

            return ( UInt64 )integer;
        }

        /// <summary>
        ///     <para>Returns the sum of all <see cref="BigInteger" />.</para>
        /// </summary>
        /// <param name="bigIntegers"></param>
        /// <returns></returns>
        public static BigInteger Sum( [NotNull] this IEnumerable<BigInteger> bigIntegers ) => bigIntegers.Aggregate( seed: BigInteger.Zero, func: ( current, bigInteger ) => current + bigInteger );

        public static void Swap<T>( ref T arg1, ref T arg2 ) {
            var temp = arg1;
            arg1 = arg2;
            arg2 = temp;
        }

        public static Int32 ThreeFourths( this Int32 x ) {
            var result = 3.0 * x / 4.0;
            return result < 1.0 ? 1 : ( Int32 )result;
        }

        public static UInt64 ThreeQuarters( this UInt64 x ) => 3 * x / 4;

        public static Single ThreeQuarters( this Single x ) => 3.0f * x / 4.0f;

        public static Double ThreeQuarters( this Double x ) => 3.0d * x / 4.0d;

        [Pure]
        public static TimeSpan Thrice( this TimeSpan timeSpan ) => TimeSpan.FromTicks( value: timeSpan.Ticks.Thrice() );

        public static Int64 Thrice( this Int64 number ) => number * 3L;

        public static IEnumerable<Int32> Through( this Int32 startValue, Int32 end ) {
            Int32 offset;
            if ( startValue < end ) {
                offset = 1;
            }
            else {
                offset = -1;
            }

            for ( var i = startValue; i != end + offset; i += offset ) {
                yield return i;
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 102.To(204)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<Byte> To( this Byte start, Byte end, Byte step = 1 ) {
            if ( step <= 1 ) {
                step = 1;
            }

            if ( start <= end ) {
                for ( var b = start; b <= end; b += step ) {
                    yield return b;
                    if ( b == Byte.MaxValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
            else {
                for ( var b = start; b >= end; b -= step ) {
                    yield return b;
                    if ( b == Byte.MinValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<UInt64> To( this Int32 start, UInt64 end, UInt64 step = 1 ) {
            if ( start < 0 ) {
                throw new ArgumentOutOfRangeException( paramName: nameof( start ), message: "'low' must be equal to or greater than zero." );
            }

            if ( step == 0UL ) {
                step = 1UL;
            }

            var reFrom = ( UInt64 )start;

            if ( start <= ( Decimal )end ) {
                for ( var ul = reFrom; ul <= end; ul += step ) {
                    yield return ul;
                    if ( ul == UInt64.MaxValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
            else {
                for ( var ul = reFrom; ul >= end; ul -= step ) {
                    yield return ul;
                    if ( ul == UInt64.MinValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="start">inclusive</param>
        /// <param name="end">inclusive</param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<Int32> To( this Int32 start, Int32 end, Int32 step = 1 ) {
            if ( start < 0 ) {
                throw new ArgumentOutOfRangeException( paramName: nameof( start ), message: "'low' must be equal to or greater than zero." );
            }

            if ( step == 0 ) {
                step = 1;
            }

            var reFrom = start; //bug here is the bug if from is less than zero

            if ( start <= end ) {
                for ( var ul = reFrom; ul <= end; ul += step ) {
                    yield return ul;
                    if ( ul == Int32.MaxValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
            else {
                for ( var ul = reFrom; ul >= end; ul -= step ) {
                    yield return ul;
                    if ( ul == Int32.MinValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="from"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<UInt64> To( this UInt64 from, UInt64 end, UInt64 step = 1 ) {
            if ( step == 0UL ) {
                step = 1UL;
            }

            if ( from <= end ) {
                for ( var ul = from; ul <= end; ul += step ) {
                    yield return ul;
                    if ( ul == UInt64.MaxValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
            else {
                for ( var ul = from; ul >= end; ul -= step ) {
                    yield return ul;
                    if ( ul == UInt64.MinValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="from"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<Int64> To( this Int64 from, Int64 end, Int64 step = 1 ) {
            if ( step == 0L ) {
                step = 1L;
            }

            if ( from <= end ) {
                for ( var ul = from; ul <= end; ul += step ) {
                    yield return ul;
                    if ( ul == Int64.MaxValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
            else {
                for ( var ul = from; ul >= end; ul -= step ) {
                    yield return ul;
                    if ( ul == Int64.MinValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
        }

        ///// <summary>
        /////     Creates an enumerable that iterates the range [fromInclusive, toExclusive).
        ///// </summary>
        ///// <param name="fromInclusive">The lower bound, inclusive.</param>
        ///// <param name="toExclusive">The upper bound, exclusive.</param>
        ///// <returns>The enumerable of the range.</returns>
        //public static IEnumerable<BigInteger> To( this BigInteger fromInclusive, BigInteger toExclusive ) {
        //    for ( var i = fromInclusive; i < toExclusive; i++ ) {
        //        yield return i;
        //    }
        //}

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<BigInteger> To( this BigInteger from, BigInteger to, UInt64 step = 1 ) {
            if ( step == 0UL ) {
                step = 1UL;
            }

            if ( from <= to ) {
                for ( var ul = from; ul <= to; ul += step ) {
                    yield return ul;
                }
            }
            else {
                for ( var ul = from; ul >= to; ul -= step ) {
                    yield return ul;
                }
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<BigInteger> To( this Int64 from, BigInteger to, UInt64 step = 1 ) {
            if ( step == 0UL ) {
                step = 1UL;
            }

            BigInteger reFrom = from;

            if ( reFrom <= to ) {
                for ( var ul = reFrom; ul <= to; ul += step ) {
                    yield return ul;
                }
            }
            else {
                for ( var ul = reFrom; ul >= to; ul -= step ) {
                    yield return ul;
                }
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="start"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<BigRational> To( this Int32 start, BigRational to, BigRational step ) {
            if ( step < 0 ) {
                step = 1;
            }

            BigRational reFrom = start;

            if ( reFrom <= to ) {
                for ( var ul = reFrom; ul <= to; ul += step ) {
                    yield return ul;
                }
            }
            else {
                for ( var ul = reFrom; ul >= to; ul -= step ) {
                    yield return ul;
                }
            }
        }

        /// <summary>
        ///     Return each <see cref="DateTime" /> between <paramref name="from" /> and
        ///     <paramref name="to" />, stepped by a <see cref="TimeSpan" /> ( <paramref name="step" />).
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        /// <remarks>//TODO Untested code!</remarks>
        /// <example>
        ///     var now = DateTime.UtcNow; var then = now.AddMinutes( 10 ); var minutes = now.To( then,
        ///     TimeSpan.FromMinutes( 1 ) ); foreach ( var dateTime in minutes ) { Console.WriteLine(
        ///     dateTime ); }
        /// </example>
        public static IEnumerable<DateTime> To( this DateTime from, DateTime to, TimeSpan? step = null ) {
            if ( !step.HasValue ) {
                TimeSpan diff;
                if ( from > to ) {
                    diff = from - from;
                }
                else {
                    diff = to - from;
                }

                if ( diff.TotalDays > 1 ) {
                    step = Days.One;
                }
                else if ( diff.TotalHours > 1 ) {
                    step = Hours.One;
                }
                else if ( diff.TotalMinutes > 1 ) {
                    step = Minutes.One;
                }
                else if ( diff.TotalSeconds > 1 ) {
                    step = Seconds.One;
                }
                else {
                    step = Milliseconds.One;
                }
            }

            if ( from > to ) {
                if ( !step.HasValue ) {
                    yield break;
                }

                for ( var dateTime = from; dateTime >= to; dateTime -= step.Value ) {
                    yield return dateTime;
                }
            }
            else {
                if ( !step.HasValue ) {
                    yield break;
                }

                for ( var dateTime = from; dateTime <= to; dateTime += step.Value ) {
                    yield return dateTime;
                }
            }
        }

        public static IEnumerable<Single> To( this Single start, Single end, Single step ) {
            var count = end - start + 1.0f;
            for ( var idx = 0.0f; idx < count; idx += step ) {
                yield return start + idx;
            }
        }

        public static IEnumerable<Double> To( this Double start, Double end, Single step ) {
            var count = end - start + 1.0;
            for ( var idx = 0.0; idx < count; idx += step ) {
                yield return start + idx;
            }
        }

        public static IEnumerable<Decimal> To( this Decimal start, Decimal end ) {
            var count = end - start + 1;
            for ( var i = 0; i < count; i++ ) {
                yield return start + i;
            }
        }

        public static UInt64? ToUInt64( this String text ) => UInt64.TryParse( s: text, result: out var result ) ? ( UInt64? )result : null;

        public static String ToHex( this IEnumerable<Byte> input ) {
            if ( input == null ) {
                throw new ArgumentNullException( paramName: nameof( input ) );
            }

            return input.Aggregate( "", ( current, b ) => current + b.ToString( "x2" ) );
        }

        public static String ToHex( this UInt32 value ) => BitConverter.GetBytes( value: value ).Aggregate( "", ( current, b ) => current + b.ToString( "x2" ) );

        public static String ToHex( this UInt64 value ) => BitConverter.GetBytes( value: value ).Aggregate( "", ( current, b ) => current + b.ToString( "x2" ) );


        public static String ToHexNumberString( this IEnumerable<Byte> value ) => Bits.ToString( value: value.Reverse().ToArray() ).Replace( "-", "" ).ToLower();

        public static String ToHexNumberString( this UInt256 value ) => value.ToByteArray().ToHexNumberString();

        /// <summary>
        ///     <see
        ///         cref="http://stackoverflow.com/questions/17575375/how-do-i-convert-an-int-to-a-String-in-c-sharp-without-using-tostring" />
        /// </summary>
        /// <param name="number"></param>
        /// <param name="base"></param>
        /// <param name="minDigits"></param>
        /// <returns></returns>
        public static String ToStringWithBase( this Int32 number, Int32 @base, Int32 minDigits = 1 ) {
            if ( minDigits < 1 ) {
                minDigits = 1;
            }

            if ( number == 0 ) {
                return new String( c: '0', count: minDigits );
            }

            var s = "";
            if ( @base < 2 || @base > MathConstants.NumberBaseChars.Length ) {
                return s;
            }

            var neg = false;
            if ( @base == 10 && number < 0 ) {
                neg = true;
                number = -number;
            }

            var n = ( UInt32 )number;
            var b = ( UInt32 )@base;
            while ( ( n > 0 ) | ( minDigits-- > 0 ) ) {
                s = MathConstants.NumberBaseChars[index: ( Int32 )( n % b )] + s;
                n /= b;
            }

            if ( neg ) {
                s = "-" + s;
            }

            return s;
        }

        public static UInt32 PackBitFields( UInt16[] values, Byte[] bitFields ) {
            UInt32 retVal = values[0]; //we set the first value right away
            for ( var f = 1; f < values.Length; f++ ) {
                retVal <<= bitFields[f]; //we shift the previous value
                retVal += values[f]; //and add our current value //on some processors | (pipe) will be faster here
            }

            return retVal;
        }

        public static UInt16[] GetBitFields( UInt32 packedBits, Byte[] bitFields ) {
            var fields = bitFields.Length - 1; // number of fields to unpack
            var retArr = new UInt16[fields + 1]; // init return array
            var curPos = 0; // current field bit position (start)
            for ( var f = fields; f >= 0; f-- ) // loop from last
            {
                var lastEnd = curPos; // position where last field ended
                curPos += bitFields[f]; // we get where the current value starts
                var leftShift = MaxBits - curPos; // we figure how much left shift we gotta apply for the other numbers to overflow into oblivion
                retArr[f] = ( UInt16 )( ( packedBits << leftShift ) >> ( leftShift + lastEnd ) ); // we do magic
            }

            return retArr;
        }

        public static Int64 Truncate( this Single number ) => ( Int64 )number;

        public static Int64 Truncate( this Double number ) => ( Int64 )number;

        /// <summary>
        ///     <para>Attempt to parse a fraction from a String.</para>
        /// </summary>
        /// <example>" 1234 / 346 "</example>
        /// <param name="numberString"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Boolean TryParse( [CanBeNull] this String numberString, out BigRational result ) {
            result = BigRational.Zero;

            if ( null == numberString ) {
                return false;
            }

            numberString = numberString.Trim();
            if ( numberString.IsNullOrEmpty() ) {
                return false;
            }

            var parts = numberString.Split( '/' ).ToList();
            if ( parts.Count != 2 ) {
                return false;
            }

            var top = parts.TakeFirst();
            if ( top.IsNullOrWhiteSpace() ) {
                return false;
            }

            top = top.Trim();

            var bottom = parts.TakeLast();
            if ( String.IsNullOrWhiteSpace( value: bottom ) ) {
                return false;
            }

            parts.Should().BeEmpty();
            if ( parts.Count > 0 ) {
                return false;
            }

            BigInteger.TryParse( value: top, result: out var numerator );

            BigInteger.TryParse( value: bottom, result: out var denominator );

            result = new BigRational( numerator: numerator, denominator: denominator );

            return true;
        }

        public static Boolean TrySplitDecimal( this Decimal value, out BigInteger beforeDecimalPoint, out BigInteger afterDecimalPoint ) {
            var theString = value.ToString( "R" );
            if ( !theString.Contains( "." ) ) {
                theString += ".0";
            }

            var split = theString.Split( '.' );
            split.Should().HaveCount( expected: 2, because: "otherwise invalid" );

            afterDecimalPoint = BigInteger.Zero;

            return BigInteger.TryParse( value: split[0], result: out beforeDecimalPoint ) && BigInteger.TryParse( value: split[1], result: out afterDecimalPoint );
        }

        [Pure]
        public static TimeSpan Twice( this TimeSpan timeSpan ) => TimeSpan.FromTicks( value: timeSpan.Ticks.Twice() );

        public static Single Twice( this Single x ) => x * 2.0f;

        public static Double Twice( this Double number ) => number * 2d;

        public static Decimal Twice( this Decimal number ) => number * 2m;

        public static Int64 Twice( this Int64 number ) => number * 2L;

        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static UInt64 RotateLeft( this UInt64 original, Int32 bits ) => ( original << bits ) | ( original >> ( 64 - bits ) );

        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static UInt64 RotateRight( this UInt64 original, Int32 bits ) => ( original >> bits ) | ( original << ( 64 - bits ) );

        /// <summary>
        ///     untested
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        /// <remarks>fixed ( Byte* pbyte = &amp;bb[pos] ) { return *( ( UInt64* )pbyte ); }</remarks>
        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public static UInt64 ToUInt64( this Byte[] bytes, Int32 pos ) => ( UInt64 )( bytes[pos++] | ( bytes[pos++] << 8 ) | ( bytes[pos++] << 16 ) | ( bytes[pos] << 24 ) );
    }
}