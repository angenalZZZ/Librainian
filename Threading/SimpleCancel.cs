﻿// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/SimpleCancel.cs" was last cleaned by Rick on 2015/09/25 at 12:49 AM

namespace Librainian.Threading {

    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Don't like the <see cref="CancellationTokenSource" /> throwing exceptions after
    ///     <see cref="CancellationTokenSource.Cancel()" /> is called. I understand why, I just don't
    ///     like it. Plus, this version has the Dates and Times of the cancel requests.
    /// </summary>
    public sealed class SimpleCancel : IDisposable {

        private Int64 _cancelRequests;

        public SimpleCancel() {
            this.Reset();
        }

        //public ConcurrentQueue< DateTime > CancelRequests { get; } = new ConcurrentQueue< DateTime >();

        /// <summary></summary>
        public DateTime? OldestCancelRequest { get; private set; }

        public DateTime? YoungestCancelRequest { get; private set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => this.RequestCancel( false );

        /// <summary>Returns true if the cancel request was approved.</summary>
        /// <param name="throwIfAlreadyRequested"></param>
        /// <param name="throwMessage"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">
        ///     Thrown if a cancellation has already been requested.
        /// </exception>
        public Boolean Cancel( Boolean throwIfAlreadyRequested = false, String throwMessage = "" ) => RequestCancel( throwIfAlreadyRequested, throwMessage );

        /// <summary></summary>
        /// <returns></returns>
        public Int64 GetCancelsRequestedCounter() => Interlocked.Read( ref _cancelRequests );

        /// <summary></summary>
        public Boolean HaveAnyCancellationsBeenRequested() => Interlocked.Read( ref _cancelRequests ) > 0;

        /// <summary>Returns true if the cancel request was approved.</summary>
        /// <param name="throwIfAlreadyRequested"></param>
        /// <param name="throwMessage"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">
        ///     Thrown if a cancellation has already been requested.
        /// </exception>
        public Boolean RequestCancel( Boolean throwIfAlreadyRequested = false, String throwMessage = "" ) {
            if ( throwIfAlreadyRequested && this.HaveAnyCancellationsBeenRequested() ) {
                throw new TaskCanceledException( throwMessage );
            }
            var now = DateTime.UtcNow;
            if ( !this.OldestCancelRequest.HasValue ) {
                //TODO name these better
                this.OldestCancelRequest = now; //TODO check logic here, might be backwards
            }
            if ( !this.YoungestCancelRequest.HasValue || ( this.YoungestCancelRequest.Value < now ) ) {
                this.YoungestCancelRequest = now;
            }
            Interlocked.Increment( ref this._cancelRequests );

            //this.CancelRequests.Enqueue( now );
            return true;
        }

        /// <summary>Resets all requests back to starting values.</summary>
        public void Reset() => Interlocked.Add( ref _cancelRequests, -Interlocked.Read( ref _cancelRequests ) );

    }

}
