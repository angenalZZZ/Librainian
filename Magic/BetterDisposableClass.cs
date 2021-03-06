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
// "Librainian/BetterDisposableClass.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Magic {

    using System;
    using Newtonsoft.Json;

    /// <summary>Hopefully, a more useful <see cref="IDisposable" /> pattern.</summary>
    [JsonObject]
    public abstract class BetterDisposableClass : IDisposable {

        ~BetterDisposableClass() {
            this.Dispose();
        }

        private Boolean IsDisposedOfManaged {
            get; set;
        }

        private Boolean IsDisposedOfNative {
            get; set;
        }

        public void Dispose() {
            try {
                if ( this.IsDisposedOfManaged ) {
                    return;
                }
                this.CleanUpManagedResources();
                this.IsDisposedOfManaged = true;
            }
            catch ( Exception exception ) {
                exception.More();
            }
            finally {
                try {
                    if ( !IsDisposedOfNative ) {
                        this.CleanUpNativeResources();
                        this.IsDisposedOfNative = true;
                        GC.SuppressFinalize( this );
                    }
                }
                catch ( Exception exception ) {
                    exception.More();
                }
            }
        }

        protected virtual void CleanUpManagedResources() {
        }

        protected virtual void CleanUpNativeResources() {
        }
    }
}