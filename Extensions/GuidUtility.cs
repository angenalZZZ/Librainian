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
// "Librainian/GuidUtility.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Extensions {

    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>Helper methods for working with <see cref="Guid" />.</summary>
    /// <seealso cref="http://github.com/LogosBible/Logos.Utility/blob/master/src/Logos.Utility/GuidUtility.cs" />
    public static class GuidUtility {

        /// <summary>
        ///     The namespace for fully-qualified domain names (from RFC 4122, Appendix C).
        /// </summary>
        public static readonly Guid DnsNamespace = new Guid( "6ba7b810-9dad-11d1-80b4-00c04fd430c8" );

        /// <summary>The namespace for ISO OIDs (from RFC 4122, Appendix C).</summary>
        public static readonly Guid IsoOidNamespace = new Guid( "6ba7b812-9dad-11d1-80b4-00c04fd430c8" );

        /// <summary>The namespace for URLs (from RFC 4122, Appendix C).</summary>
        public static readonly Guid UrlNamespace = new Guid( "6ba7b811-9dad-11d1-80b4-00c04fd430c8" );

        /// <summary>Creates a name-based UUID using the algorithm from RFC 4122 �4.3.</summary>
        /// <param name="namespaceId">The ID of the namespace.</param>
        /// <param name="name">The name (within that namespace).</param>
        /// <returns>A UUID derived from the namespace and name.</returns>
        /// <remarks>
        ///     See
        ///     <a href="http://code.logos.com/blog/2011/04/generating_a_deterministic_guid.html">
        ///         Generating
        ///         a deterministic GUID
        ///     </a>
        ///     .
        /// </remarks>
        public static Guid Create( Guid namespaceId, String name ) => Create( namespaceId, name, 5 );

	    /// <summary>Creates a name-based UUID using the algorithm from RFC 4122 �4.3.</summary>
        /// <param name="namespaceId">The ID of the namespace.</param>
        /// <param name="name">The name (within that namespace).</param>
        /// <param name="version">
        ///     The version number of the UUID to create; this value must be either 3 (for MD5 hashing)
        ///     or 5 (for SHA-1 hashing).
        /// </param>
        /// <returns>A UUID derived from the namespace and name.</returns>
        /// <remarks>
        ///     See
        ///     <a href="http://code.logos.com/blog/2011/04/generating_a_deterministic_guid.html">
        ///         Generating
        ///         a deterministic GUID
        ///     </a>
        ///     .
        /// </remarks>
        public static Guid Create( Guid namespaceId, String name, Int32 version ) {
            if ( name == null ) {
                throw new ArgumentNullException( nameof( name ) );
            }
            if ( version != 3 && version != 5 ) {
                throw new ArgumentOutOfRangeException( nameof( version ), "version must be either 3 or 5." );
            }

            // convert the name to a sequence of octets (as defined by the standard or conventions
            // of its namespace) (step 3)
            // ASSUME: UTF-8 encoding is always appropriate
            var nameBytes = Encoding.UTF8.GetBytes( name );

            // convert the namespace UUID to network order (step 3)
            var namespaceBytes = namespaceId.ToByteArray();
            SwapByteOrder( namespaceBytes );

            // compute the hash of the name space ID concatenated with the name (step 4)
            Byte[] hash;
            using ( var algorithm = version == 3 ? ( HashAlgorithm )MD5.Create() : SHA1.Create() ) {
                algorithm.TransformBlock( namespaceBytes, 0, namespaceBytes.Length, null, 0 );
                algorithm.TransformFinalBlock( nameBytes, 0, nameBytes.Length );
                hash = algorithm.Hash;
            }

            // most bytes from the hash are copied straight to the bytes of the new GUID (steps 5-7,
            // 9, 11-12)
            var newGuid = new Byte[ 16 ];
            Array.Copy( hash, 0, newGuid, 0, 16 );

            // set the four most significant bits (bits 12 through 15) of the time_hi_and_version
            // field to the appropriate 4-bit version number from Section 4.1.3 (step 8)
            newGuid[ 6 ] = ( Byte )( ( newGuid[ 6 ] & 0x0F ) | ( version << 4 ) );

            // set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to
            // zero and one, respectively (step 10)
            newGuid[ 8 ] = ( Byte )( ( newGuid[ 8 ] & 0x3F ) | 0x80 );

            // convert the resulting UUID to local byte order (step 13)
            SwapByteOrder( newGuid );
            return new Guid( newGuid );
        }

        // Converts a GUID (expressed as a byte array) to/from network order (MSB-first).
        internal static void SwapByteOrder( Byte[] guid ) {
            SwapBytes( guid, 0, 3 );
            SwapBytes( guid, 1, 2 );
            SwapBytes( guid, 4, 5 );
            SwapBytes( guid, 6, 7 );
        }

        private static void SwapBytes( Byte[] guid, Int32 left, Int32 right ) {
            var temp = guid[ left ];
            guid[ left ] = guid[ right ];
            guid[ right ] = temp;
        }
    }
}