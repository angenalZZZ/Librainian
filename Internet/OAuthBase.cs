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
// "Librainian/OAuthBase.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    public class OAuthBase {
        protected const String Hmacsha1SignatureType = "HMAC-SHA1";

        protected const String OAuthCallbackKey = "oauth_callback";

        //
        // List of know and used oauth parameters' names
        //
        protected const String OAuthConsumerKeyKey = "oauth_consumer_key";

        protected const String OAuthNonceKey = "oauth_nonce";

        protected const String OAuthParameterPrefix = "oauth_";

        protected const String OAuthSignatureKey = "oauth_signature";

        protected const String OAuthSignatureMethodKey = "oauth_signature_method";

        protected const String OAuthTimestampKey = "oauth_timestamp";

        protected const String OAuthTokenKey = "oauth_token";

        protected const String OAuthTokenSecretKey = "oauth_token_secret";

        protected const String OAuthVersion = "1.0";

        protected const String OAuthVersionKey = "oauth_version";

        protected const String PlainTextSignatureType = "PLAINTEXT";

        protected const String Rsasha1SignatureType = "RSA-SHA1";

        protected const String UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        protected readonly Random Random = new Random();

        /// <summary>
        ///     Provides a predefined set of algorithms that are supported officially by the protocol
        /// </summary>
        public enum SignatureTypes {
            Hmacsha1,
            Plaintext,
            Rsasha1
        }

        /// <summary>
        ///     This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower
        ///     case.
        ///     While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        public static String UrlEncode( String value ) {
            var result = new StringBuilder();

            foreach ( var symbol in value ) {
                if ( UnreservedChars.IndexOf( symbol ) != -1 ) {
                    result.Append( symbol );
                }
                else {
                    result.Append( $"{'%'}{( Int32 )symbol:X2}" );
                }
            }

            return result.ToString();
        }

        /// <summary>
        ///     Generate a nonce
        /// </summary>
        /// <returns></returns>
        public virtual String GenerateNonce() => this.Random.Next( 123400, 9999999 ).ToString();

	    /// <summary>
        ///     Generates a signature using the HMAC-SHA1 algorithm
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="timeStamp"></param>
        /// <param name="nonce"></param>
        /// <param name="normalizedUrl"></param>
        /// <param name="normalizedRequestParameters"></param>
        /// <returns>A base64 string of the hash value</returns>
        public String GenerateSignature( Uri url, String consumerKey, String consumerSecret, String token, String tokenSecret, String httpMethod, String timeStamp, String nonce, out String normalizedUrl, out String normalizedRequestParameters ) => this.GenerateSignature( url, consumerKey, consumerSecret, token, tokenSecret, httpMethod, timeStamp, nonce, SignatureTypes.Hmacsha1, out normalizedUrl, out normalizedRequestParameters );

	    /// <summary>
        ///     Generates a signature using the specified signatureType
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="nonce"></param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <param name="timeStamp"></param>
        /// <param name="normalizedUrl"></param>
        /// <param name="normalizedRequestParameters"></param>
        /// <returns>A base64 string of the hash value</returns>
        public String GenerateSignature( Uri url, String consumerKey, String consumerSecret, String token, String tokenSecret, String httpMethod, String timeStamp, String nonce, SignatureTypes signatureType, out String normalizedUrl, out String normalizedRequestParameters ) {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch ( signatureType ) {
                case SignatureTypes.Plaintext:
                    return HttpUtility.UrlEncode( $"{consumerSecret}&{tokenSecret}" );

                case SignatureTypes.Hmacsha1:
                    var signatureBase = this.GenerateSignatureBase( url, consumerKey, token, tokenSecret, httpMethod, timeStamp, nonce, Hmacsha1SignatureType, out normalizedUrl, out normalizedRequestParameters );

                    var hmacsha1 = new HMACSHA1 { Key = Encoding.ASCII.GetBytes( $"{UrlEncode( consumerSecret )}&{( String.IsNullOrEmpty( tokenSecret ) ? "" : UrlEncode( tokenSecret ) )}" ) };

                    return this.GenerateSignatureUsingHash( signatureBase, hmacsha1 );

                case SignatureTypes.Rsasha1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException( "Unknown signature type", nameof( signatureType ) );
            }
        }

        /// <summary>
        ///     Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="nonce"></param>
        /// <param name="signatureType">
        ///     The signature type. To use the default values use
        ///     <see cref="OAuthBase.SignatureTypes">OAuthBase.SignatureTypes</see>.
        /// </param>
        /// <param name="timeStamp"></param>
        /// <param name="normalizedUrl"></param>
        /// <param name="normalizedRequestParameters"></param>
        /// <returns>The signature base</returns>
        public String GenerateSignatureBase( Uri url, String consumerKey, String token, String tokenSecret, String httpMethod, String timeStamp, String nonce, String signatureType, out String normalizedUrl, out String normalizedRequestParameters ) {
            if ( token == null ) {
                token = String.Empty;
            }

            if ( tokenSecret == null ) {

                // ReSharper disable once RedundantAssignment
                tokenSecret = String.Empty;
            }

            if ( String.IsNullOrEmpty( consumerKey ) ) {
                throw new ArgumentNullException( nameof( consumerKey ) );
            }

            if ( String.IsNullOrEmpty( httpMethod ) ) {
                throw new ArgumentNullException( nameof( httpMethod ) );
            }

            if ( String.IsNullOrEmpty( signatureType ) ) {
                throw new ArgumentNullException( nameof( signatureType ) );
            }

            var parameters = GetQueryParameters( url.Query );
            parameters.Add( new QueryParameter( OAuthVersionKey, OAuthVersion ) );
            parameters.Add( new QueryParameter( OAuthNonceKey, nonce ) );
            parameters.Add( new QueryParameter( OAuthTimestampKey, timeStamp ) );
            parameters.Add( new QueryParameter( OAuthSignatureMethodKey, signatureType ) );
            parameters.Add( new QueryParameter( OAuthConsumerKeyKey, consumerKey ) );

            if ( !String.IsNullOrEmpty( token ) ) {
                parameters.Add( new QueryParameter( OAuthTokenKey, token ) );
            }

            parameters.Sort( new QueryParameterComparer() );

            normalizedUrl = $"{url.Scheme}://{url.Host}";
            if ( !( url.Scheme == "http" && url.Port == 80 || url.Scheme == "https" && url.Port == 443 ) ) {
                normalizedUrl += $":{url.Port}";
            }
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters( parameters );

            var signatureBase = new StringBuilder();
            signatureBase.Append( $"{httpMethod.ToUpper()}&" );
            signatureBase.Append( $"{UrlEncode( normalizedUrl )}&" );
            signatureBase.Append( $"{UrlEncode( normalizedRequestParameters )}" );

            return signatureBase.ToString();
        }

        /// <summary>
        ///     Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">
        ///     The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or
        ///     a key it should be set prior to calling this method
        /// </param>
        /// <returns>A base64 string of the hash value</returns>
        public String GenerateSignatureUsingHash( String signatureBase, HashAlgorithm hash ) => this.ComputeHash( hash, signatureBase );

	    /// <summary>
        ///     Generate the timestamp for the signature
        /// </summary>
        /// <returns></returns>
        public virtual String GenerateTimeStamp() {

            // Default implementation of UNIX time of the current UTC time
            var ts = DateTime.UtcNow - new DateTime( 1970, 1, 1, 0, 0, 0, 0 );
            return Convert.ToInt64( ts.TotalSeconds ).ToString();
        }

        /// <summary>
        ///     Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a string representing the normalized parameters</returns>
        protected static String NormalizeRequestParameters( IList<QueryParameter> parameters ) {
            var sb = new StringBuilder();
            for ( var i = 0; i < parameters.Count; i++ ) {
                var p = parameters[ i ];
                sb.Append( $"{p.Name}={p.Value}" );

                if ( i < parameters.Count - 1 ) {
                    sb.Append( "&" );
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        private static List<QueryParameter> GetQueryParameters( String parameters ) {
            if ( parameters.StartsWith( "?" ) ) {
                parameters = parameters.Remove( 0, 1 );
            }

            var result = new List<QueryParameter>();

            if ( !String.IsNullOrEmpty( parameters ) ) {
                var p = parameters.Split( '&' );
                foreach ( var s in p.Where( s => !String.IsNullOrEmpty( s ) && !s.StartsWith( OAuthParameterPrefix ) ) ) {
                    if ( s.Contains( "=" ) ) {
                        var temp = s.Split( '=' );
                        result.Add( new QueryParameter( temp[ 0 ], temp[ 1 ] ) );
                    }
                    else {
                        result.Add( new QueryParameter( s, String.Empty ) );
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">
        ///     The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its
        ///     derivatives, they should be initialized prior to passing it to this function
        /// </param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        private String ComputeHash( HashAlgorithm hashAlgorithm, String data ) {
            if ( hashAlgorithm == null ) {
                throw new ArgumentNullException( nameof( hashAlgorithm ) );
            }

            if ( String.IsNullOrEmpty( data ) ) {
                throw new ArgumentNullException( nameof( data ) );
            }

            var dataBuffer = Encoding.ASCII.GetBytes( data );
            var hashBytes = hashAlgorithm.ComputeHash( dataBuffer );

            return Convert.ToBase64String( hashBytes );
        }

        /// <summary>
        ///     Provides an internal structure to sort the query parameter
        /// </summary>
        protected class QueryParameter {

            public QueryParameter( String name, String value ) {
                this.Name = name;
                this.Value = value;
            }

            public String Name {
                get;
            }

            public String Value {
                get;
            }
        }

        /// <summary>
        ///     Comparer class used to perform the sorting of the query parameters
        /// </summary>
        protected class QueryParameterComparer : IComparer<QueryParameter> {

            public Int32 Compare( QueryParameter x, QueryParameter y ) => x?.Name == y?.Name ? String.CompareOrdinal( x?.Value, y?.Value ) : String.CompareOrdinal( x?.Name, y?.Name );

        }
    }
}