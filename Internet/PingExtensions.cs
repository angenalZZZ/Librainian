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
// "Librainian/PingExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Internet {

    using System;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Threading.Tasks;

    /// <summary>
    ///     <para>Extension methods for working with Ping asynchronously.</para>
    ///     <para>Copyright (c) Microsoft Corporation. All rights reserved.</para>
    /// </summary>
    public static class PingExtensions {

        /// <summary>
        ///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="address">
        ///     An IPAddress that identifies the computer that is the destination for the ICMP echo message.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask( this Ping ping, IPAddress address, Object userToken ) => SendTaskCore( ping, userToken, tcs => ping.SendAsync( address, tcs ) );

        /// <summary>
        ///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="hostNameOrAddress">
        ///     A String that identifies the computer that is the destination for the ICMP echo message.
        ///     The value specified for this parameter can be a host name or a String representation of
        ///     an IP address.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask( this Ping ping, String hostNameOrAddress, Object userToken ) => SendTaskCore( ping, userToken, tcs => ping.SendAsync( hostNameOrAddress, tcs ) );

        /// <summary>
        ///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="address">
        ///     An IPAddress that identifies the computer that is the destination for the ICMP echo message.
        /// </param>
        /// <param name="timeout">
        ///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo
        ///     message) to wait for the ICMP echo reply message.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask( this Ping ping, IPAddress address, Int32 timeout, Object userToken ) => SendTaskCore( ping, userToken, tcs => ping.SendAsync( address, timeout, tcs ) );

        /// <summary>
        ///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="hostNameOrAddress">
        ///     A String that identifies the computer that is the destination for the ICMP echo message.
        ///     The value specified for this parameter can be a host name or a String representation of
        ///     an IP address.
        /// </param>
        /// <param name="timeout">
        ///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo
        ///     message) to wait for the ICMP echo reply message.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask( this Ping ping, String hostNameOrAddress, Int32 timeout, Object userToken ) => SendTaskCore( ping, userToken, tcs => ping.SendAsync( hostNameOrAddress, timeout, tcs ) );

        /// <summary>
        ///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="address">
        ///     An IPAddress that identifies the computer that is the destination for the ICMP echo message.
        /// </param>
        /// <param name="timeout">
        ///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo
        ///     message) to wait for the ICMP echo reply message.
        /// </param>
        /// <param name="buffer">
        ///     A Byte array that contains data to be sent with the ICMP echo message and returned in
        ///     the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask( this Ping ping, IPAddress address, Int32 timeout, Byte[] buffer, Object userToken ) => SendTaskCore( ping, userToken, tcs => ping.SendAsync( address, timeout, buffer, tcs ) );

        /// <summary>
        ///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="hostNameOrAddress">
        ///     A String that identifies the computer that is the destination for the ICMP echo message.
        ///     The value specified for this parameter can be a host name or a String representation of
        ///     an IP address.
        /// </param>
        /// <param name="timeout">
        ///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo
        ///     message) to wait for the ICMP echo reply message.
        /// </param>
        /// <param name="buffer">
        ///     A Byte array that contains data to be sent with the ICMP echo message and returned in
        ///     the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask( this Ping ping, String hostNameOrAddress, Int32 timeout, Byte[] buffer, Object userToken ) => SendTaskCore( ping, userToken, tcs => ping.SendAsync( hostNameOrAddress, timeout, buffer, tcs ) );

        /// <summary>
        ///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="address">
        ///     An IPAddress that identifies the computer that is the destination for the ICMP echo message.
        /// </param>
        /// <param name="timeout">
        ///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo
        ///     message) to wait for the ICMP echo reply message.
        /// </param>
        /// <param name="buffer">
        ///     A Byte array that contains data to be sent with the ICMP echo message and returned in
        ///     the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
        /// </param>
        /// <param name="options">
        ///     A PingOptions object used to control fragmentation and Time-to-Live values for the ICMP
        ///     echo message packet.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask( this Ping ping, IPAddress address, Int32 timeout, Byte[] buffer, PingOptions options, Object userToken ) => SendTaskCore( ping, userToken, tcs => ping.SendAsync( address, timeout, buffer, options, tcs ) );

        /// <summary>
        ///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
        /// </summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="hostNameOrAddress">
        ///     A String that identifies the computer that is the destination for the ICMP echo message.
        ///     The value specified for this parameter can be a host name or a String representation of
        ///     an IP address.
        /// </param>
        /// <param name="timeout">
        ///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo
        ///     message) to wait for the ICMP echo reply message.
        /// </param>
        /// <param name="buffer">
        ///     A Byte array that contains data to be sent with the ICMP echo message and returned in
        ///     the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
        /// </param>
        /// <param name="options">
        ///     A PingOptions object used to control fragmentation and Time-to-Live values for the ICMP
        ///     echo message packet.
        /// </param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task<PingReply> SendTask( this Ping ping, String hostNameOrAddress, Int32 timeout, Byte[] buffer, PingOptions options, Object userToken ) => SendTaskCore( ping, userToken, tcs => ping.SendAsync( hostNameOrAddress, timeout, buffer, options, tcs ) );

        /// <summary>The core implementation of SendTask.</summary>
        /// <param name="ping">The Ping.</param>
        /// <param name="userToken">A user-defined object stored in the resulting Task.</param>
        /// <param name="sendAsync">
        ///     A delegate that initiates the asynchronous send. The provided TaskCompletionSource must
        ///     be passed as the user-supplied state to the actual Ping.SendAsync method.
        /// </param>
        /// <returns></returns>
        private static Task<PingReply> SendTaskCore( Ping ping, Object userToken, Action<TaskCompletionSource<PingReply>> sendAsync ) {

            // Validate we're being used with a real smtpClient. The rest of the arg validation will
            // happen in the call to sendAsync.
            if ( ping == null ) {
                throw new ArgumentNullException( nameof( ping ) );
            }

            // Create a TaskCompletionSource to represent the operation
            var tcs = new TaskCompletionSource<PingReply>( userToken );

            // Register a handler that will transfer completion results to the TCS Task
	        void Handler( Object sender, PingCompletedEventArgs e ) => EapCommon.HandleCompletion( tcs, e, () => e.Reply, () => ping.PingCompleted -= Handler );

	        ping.PingCompleted += Handler;

            // Try to start the async operation. If starting it fails (due to parameter validation)
            // unregister the handler before allowing the exception to propagate.
            try {
                sendAsync( tcs );
            }
            catch ( Exception exc ) {
                ping.PingCompleted -= Handler;
                tcs.TrySetException( exc );
            }

            // Return the task to represent the asynchronous operation
            return tcs.Task;
        }
    }
}