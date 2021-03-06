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
// "Librainian/Windows.cs" was last cleaned by Rick on 2016/08/18 at 9:25 AM

namespace Librainian.Security {

    using System;
    using System.Security;
    using System.Security.Principal;

    public static class Windows {

        /// <summary>Determine if the current user is in the role of <see cref="WindowsBuiltInRole" />.</summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static Boolean IsUserInRole( this WindowsBuiltInRole role ) {
            try {
                using ( var windowsIdentity = WindowsIdentity.GetCurrent() ) {
                    var windowsPrincipal = new WindowsPrincipal( windowsIdentity );
                    return windowsPrincipal.IsInRole( role );
                }
            }
            catch ( SecurityException ) { }
            catch ( ArgumentNullException ) { }
            catch ( ArgumentException ) { }

            return false;
        }
    }
}
