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
// "Librainian/StressTest.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Database {

    using System;
    using System.Data;
    using JetBrains.Annotations;
    using Measurement.Time;

    public static class StressTest {

        /// <summary>
        ///     How high can this computer count in one second?
        /// </summary>
        /// <returns></returns>
        public static UInt64 PerformBaselineCounting() {
            TimeSpan forHowLong = Seconds.One;
            var stopwatch = StopWatch.StartNew();
            var counter = 0UL;
            do {
                counter++;
                if ( stopwatch.Elapsed >= forHowLong ) {
                    break;
                }
            } while ( true );
            return counter;
        }

		/// <summary>
		///     How high can this database count in one second?
		/// </summary>
		/// <param name="database"></param>
		/// <param name="forHowLong"></param>
		/// <param name="multithread"></param>
		/// <returns></returns>
		public static UInt64 PerformDatabaseCounting( [NotNull] IDatabase database, out TimeSpan forHowLong, Boolean multithread = false ) {
            if ( database == null ) {
                throw new ArgumentNullException( nameof( database ) );
            }
            if ( multithread ) {
                throw new NotImplementedException( "yet" );
            }
            forHowLong = Seconds.One;
            var stopwatch = StopWatch.StartNew();
            var counter = 0UL;
            do {
	            counter = database.ExecuteScalar<UInt64>( $"select {counter} + cast(1 as bigint)  as [Result];", CommandType.Text );
                if ( stopwatch.Elapsed >= forHowLong ) {
                    break;
                }
            } while ( true );
            return counter;
        }
    }
}