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
// "Librainian/ProgressStream.cs" was last cleaned by Rick on 2016/06/18 at 10:55 PM

namespace Librainian.OperatingSystem.Streams {

    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using Measurement.Frequency;

    public sealed class ProgressStream : ContainerStream {
        private Int32 _lastProgress;

        private DateTime _lastProgressUpdate = DateTime.UtcNow.AddSeconds( -1 );

        public ProgressStream( Stream stream ) : base( stream: stream ) {
            if ( stream.CanRead && stream.CanSeek && stream.Length > 0 ) {
                return;
            }
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
            throw new ArgumentException( "stream" );
        }

        public ProgressChangedEventHandler ProgressChanged {
            get; set;
        }

        public override Int32 Read( Byte[] buffer, Int32 offset, Int32 count ) {
            var amountRead = this.Stream.Read( buffer, offset, count );

            var newProgress = ( Int32 )( 1024.0 * ( this.Position / ( Double )this.Length ) );
            if ( newProgress <= this._lastProgress || DateTime.UtcNow - this._lastProgressUpdate < Hertz.Sixty ) {
                return amountRead;
            }
            this._lastProgressUpdate = DateTime.UtcNow;
            this._lastProgress = newProgress;
            var progressChanged = this.ProgressChanged;
            progressChanged?.Invoke( this, new ProgressChangedEventArgs( this._lastProgress, null ) );

            return amountRead;
        }

        /// <summary>
        ///     When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <returns>The new position within the current stream.</returns>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">
        ///     A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point
        ///     used to obtain the new position.
        /// </param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The stream does not support seeking, such as if the stream is constructed from a pipe or
        ///     console output.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     Methods were called after the stream was closed.
        /// </exception>
        /// <filterpriority>1</filterpriority>
        public override Int64 Seek( Int64 offset, SeekOrigin origin ) => this.Stream.Seek( offset: offset, origin: origin );

        /// <summary>
        ///     When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The stream does not support both writing and seeking, such as if the stream is
        ///     constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     Methods were called after the stream was closed.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public override void SetLength( Int64 value ) => this.Stream.SetLength( value: value );

        /// <summary>
        ///     When overridden in a derived class, writes a sequence of bytes to the current stream and
        ///     advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        ///     An array of bytes. This method copies <paramref name="count" /> bytes from
        ///     <paramref name="buffer" /> to the current stream.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes
        ///     to the current stream.
        /// </param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is greater than the
        ///     buffer length.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="buffer" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     Methods were called after the stream was closed.
        /// </exception>
        /// <filterpriority>1</filterpriority>
        public override void Write( Byte[] buffer, Int32 offset, Int32 count ) => this.Stream.Write( buffer: buffer, offset: offset, count: count );
    }
}