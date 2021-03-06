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
// "Librainian/DocumentCopyStatistics.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;

    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public class DocumentCopyStatistics {

        [JsonProperty]
        public UInt64 BytesCopied {
            get; set;
        }

        [JsonProperty]
        [CanBeNull]
        public Document DestinationDocument {
            get; set;
        }

        [JsonProperty]
        [CanBeNull]
        public String DestinationDocumentCRC64 {
            get; set;
        }

        [JsonProperty]
        [CanBeNull]
        public Document SourceDocument {
            get; set;
        }

        [JsonProperty]
        [CanBeNull]
        public String SourceDocumentCRC64 {
            get; set;
        }

        [JsonProperty]
        public DateTime TimeStarted {
            get; set;
        }

        [JsonProperty]
        public TimeSpan TimeTaken {
            get; set;
        }

        public Double BytesPerMillisecond() {
            if ( Math.Abs( this.TimeTaken.TotalMilliseconds ) < Double.Epsilon ) {
                return 0;
            }
            return this.BytesCopied / this.TimeTaken.TotalMilliseconds;
        }

        public Double MegabytesPerSecond() {
            if ( Math.Abs( this.TimeTaken.TotalSeconds ) < Double.Epsilon ) {
                return 0;
            }
            var mb = this.BytesCopied / ( Double )MathConstants.OneMegaByte;
            return mb / this.TimeTaken.TotalSeconds;
        }

        public Double MillisecondsPerByte() {
            if ( this.BytesCopied <= 0 ) {
                return 0;
            }
            return this.TimeTaken.TotalMilliseconds / this.BytesCopied;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override String ToString() => $"{this.SourceDocument?.FileName()} copied to {this.DestinationDocument.Folder} @ {this.MegabytesPerSecond()}MB/s";

    }
}