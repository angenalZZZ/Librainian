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
// "Librainian/ParallelAlgorithms.cs" was last cleaned by Rick on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Copyright (c) Microsoft Corporation. All rights reserved.
    ///     File: ParallelAlgorithms_Wavefront.cs
    /// </summary>
    public static class ParallelAlgorithms {

        /// <summary>
        ///     Executes a function for each value in a range, returning the first result achieved and
        ///     ceasing execution.
        /// </summary>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="fromInclusive">The start of the range, inclusive.</param>
        /// <param name="toExclusive">The end of the range, exclusive.</param>
        /// <param name="body">The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeFor<TResult>( this Int32 fromInclusive, Int32 toExclusive, Func<Int32, TResult> body ) => fromInclusive.SpeculativeFor( toExclusive, ThreadingExtensions.CPUIntensive, body );

        /// <summary>
        ///     Executes a function for each value in a range, returning the first result achieved and
        ///     ceasing execution.
        /// </summary>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="fromInclusive">The start of the range, inclusive.</param>
        /// <param name="toExclusive">The end of the range, exclusive.</param>
        /// <param name="options">The options to use for processing the loop.</param>
        /// <param name="body">The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeFor<TResult>( this Int32 fromInclusive, Int32 toExclusive, ParallelOptions options, Func<Int32, TResult> body ) {

            // Validate parameters; the Parallel.For we delegate to will validate the rest
            if ( body == null ) {
                throw new ArgumentNullException( nameof( body ) );
            }

            // Store one result. We box it if it's a value type to avoid torn writes and enable
            // CompareExchange even for value types.
            Object result = null;

            // Run all bodies in parallel, stopping as soon as one has completed.
            Parallel.For( fromInclusive, toExclusive, options, ( i, loopState ) => {

                // Run an iteration. When it completes, store (box) the result, and cancel the rest
                Interlocked.CompareExchange( ref result, body( i ), null );
                loopState.Stop();
            } );

            // Return the computed result
            return ( TResult )result;
        }

        /// <summary>
        ///     Executes a function for each element in a source, returning the first result achieved
        ///     and ceasing execution.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the source.</typeparam>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="source">The input elements to be processed.</param>
        /// <param name="body">The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeForEach<TSource, TResult>( this IEnumerable<TSource> source, Func<TSource, TResult> body ) => source.SpeculativeForEach( ThreadingExtensions.CPUIntensive, body );

        /// <summary>
        ///     Executes a function for each element in a source, returning the first result achieved
        ///     and ceasing execution.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the source.</typeparam>
        /// <typeparam name="TResult">The type of the data returned.</typeparam>
        /// <param name="source">The input elements to be processed.</param>
        /// <param name="options">The options to use for processing the loop.</param>
        /// <param name="body">The function to execute for each element.</param>
        /// <returns>The result computed.</returns>
        public static TResult SpeculativeForEach<TSource, TResult>( this IEnumerable<TSource> source, ParallelOptions options, Func<TSource, TResult> body ) {

            // Validate parameters; the Parallel.ForEach we delegate to will validate the rest
            if ( body == null ) {
                throw new ArgumentNullException( nameof( body ) );
            }

            // Store one result. We box it if it's a value type to avoid torn writes and enable
            // CompareExchange even for value types.
            Object result = null;

            // Run all bodies in parallel, stopping as soon as one has completed.
            Parallel.ForEach( source, options, ( item, loopState ) => {

                // Run an iteration. When it completes, store (box) the result, and cancel the rest
                Interlocked.CompareExchange( ref result, body( item ), null );
                loopState.Stop();
            } );

            // Return the computed result
            return ( TResult )result;
        }

        /// <summary>
        ///     Invokes the specified functions, potentially in parallel, canceling outstanding
        ///     invocations once ONE completes.
        /// </summary>
        /// <typeparam name="T">Specifies the type of data returned by the functions.</typeparam>
        /// <param name="functions">The functions to be executed.</param>
        /// <returns>A result from executing one of the functions.</returns>
        public static T SpeculativeInvoke<T>( params Func<T>[] functions ) => ThreadingExtensions.CPUIntensive.SpeculativeInvoke( functions );

        /// <summary>
        ///     Invokes the specified functions, potentially in parallel, canceling outstanding
        ///     invocations once ONE completes.
        /// </summary>
        /// <typeparam name="T">Specifies the type of data returned by the functions.</typeparam>
        /// <param name="options">The options to use for the execution.</param>
        /// <param name="functions">The functions to be executed.</param>
        /// <returns>A result from executing one of the functions.</returns>
        public static T SpeculativeInvoke<T>( this ParallelOptions options, params Func<T>[] functions ) {

            // Validate parameters
            if ( options == null ) {
                throw new ArgumentNullException( nameof( options ) );
            }
            if ( functions == null ) {
                throw new ArgumentNullException( nameof( functions ) );
            }

            // Speculatively invoke each function
            return functions.SpeculativeForEach( options, function => function() );
        }

        /// <summary>
        ///     Process in parallel a matrix where every cell has a dependency on the cell above it and
        ///     to its left.
        /// </summary>
        /// <param name="processBlock">
        ///     The action to invoke for every block, supplied with the start and end indices of the
        ///     rows and columns.
        /// </param>
        /// <param name="numRows">The number of rows in the matrix.</param>
        /// <param name="numColumns">The number of columns in the matrix.</param>
        /// <param name="numBlocksPerRow">
        ///     Partition the matrix into this number of blocks along the rows.
        /// </param>
        /// <param name="numBlocksPerColumn">
        ///     Partition the matrix into this number of blocks along the columns.
        /// </param>
        public static void Wavefront( this Action<Int32, Int32, Int32, Int32> processBlock, Int32 numRows, Int32 numColumns, Int32 numBlocksPerRow, Int32 numBlocksPerColumn ) {

            // Validate parameters
            if ( numRows <= 0 ) {
                throw new ArgumentOutOfRangeException( nameof( numRows ) );
            }
            if ( numColumns <= 0 ) {
                throw new ArgumentOutOfRangeException( nameof( numColumns ) );
            }
            if ( numBlocksPerRow <= 0 || numBlocksPerRow > numRows ) {
                throw new ArgumentOutOfRangeException( nameof( numBlocksPerRow ) );
            }
            if ( numBlocksPerColumn <= 0 || numBlocksPerColumn > numColumns ) {
                throw new ArgumentOutOfRangeException( nameof( numBlocksPerColumn ) );
            }
            if ( processBlock == null ) {
                throw new ArgumentNullException( nameof( processBlock ) );
            }

            // Compute the size of each block
            var rowBlockSize = numRows / numBlocksPerRow;
            var columnBlockSize = numColumns / numBlocksPerColumn;

            Wavefront( ( row, column ) => {
                var startI = row * rowBlockSize;
                var endI = row < numBlocksPerRow - 1 ? startI + rowBlockSize : numRows;

                var startJ = column * columnBlockSize;
                var endJ = column < numBlocksPerColumn - 1 ? startJ + columnBlockSize : numColumns;

                processBlock( startI, endI, startJ, endJ );
            }, numBlocksPerRow, numBlocksPerColumn );
        }

        /// <summary>
        ///     Process in parallel a matrix where every cell has a dependency on the cell above it and
        ///     to its left.
        /// </summary>
        /// <param name="processRowColumnCell">
        ///     The action to invoke for every cell, supplied with the row and column indices.
        /// </param>
        /// <param name="numRows">The number of rows in the matrix.</param>
        /// <param name="numColumns">The number of columns in the matrix.</param>
        public static void Wavefront( this Action<Int32, Int32> processRowColumnCell, Int32 numRows, Int32 numColumns ) {

            // Validate parameters
            if ( numRows <= 0 ) {
                throw new ArgumentOutOfRangeException( nameof( numRows ) );
            }
            if ( numColumns <= 0 ) {
                throw new ArgumentOutOfRangeException( nameof( numColumns ) );
            }
            if ( processRowColumnCell == null ) {
                throw new ArgumentNullException( nameof( processRowColumnCell ) );
            }

            // Store the previous row of tasks as well as the previous task in the current row
            var prevTaskRow = new Task[ numColumns ];
            Task prevTaskInCurrentRow = null;
            var dependencies = new Task[ 2 ];

            // Create a task for each cell
            for ( var row = 0; row < numRows; row++ ) {
                prevTaskInCurrentRow = null;
                for ( var column = 0; column < numColumns; column++ ) {

                    // In-scope locals for being captured in the task closures
                    Int32 j = row, i = column;

                    // Create a task with the appropriate dependencies.
                    Task curTask;
                    if ( row == 0 && column == 0 ) {

                        // Upper-left task kicks everything off, having no dependencies
                        curTask = Task.Run( () => processRowColumnCell( j, i ) );
                    }
                    else if ( row == 0 || column == 0 ) {

                        // Tasks in the left-most column depend only on the task above them, and
                        // tasks in the top row depend only on the task to their left
                        var antecedent = column == 0 ? prevTaskRow[ 0 ] : prevTaskInCurrentRow;

                        
                        curTask = antecedent?.ContinueWith( p => {
                            p.Wait(); // Necessary only to propagate exceptions
                            processRowColumnCell( j, i );
                        } );
                    }
                    else // row > 0 && column > 0
                    {
                        // All other tasks depend on both the tasks above and to the left
                        dependencies[ 0 ] = prevTaskInCurrentRow;
                        dependencies[ 1 ] = prevTaskRow[ column ];
                        curTask = Task.Factory.ContinueWhenAll( dependencies, ps => {
                            Task.WaitAll( ps ); // Necessary only to propagate exceptions
                            processRowColumnCell( j, i );
                        } );
                    }

                    // Keep track of the task just created for future iterations
                    prevTaskRow[ column ] = prevTaskInCurrentRow = curTask;
                }
            }

            // Wait for the last task to be done.
            prevTaskInCurrentRow?.Wait();
        }
    }
}