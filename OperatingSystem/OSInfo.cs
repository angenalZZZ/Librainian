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
// "Librainian/OSInfo.cs" was last cleaned by Rick on 2016/07/28 at 2:19 PM

namespace Librainian.OperatingSystem {

    using System;
    using JetBrains.Annotations;
    using Microsoft.Win32;

    /// <summary>
    ///     Static class that adds convenient methods for getting information on the running computers basic hardware and os
    ///     setup.
    /// </summary>
    /// <remarks>Adapted from <see cref="http://stackoverflow.com/a/37755503/956364" />.</remarks>
    public static class OSInfo {

        public const String CurrentVersion = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";

        /// <summary>
        ///     Returns the Windows build.
        /// </summary>
        [CanBeNull]
        public static String BuildBranch() {
            dynamic value;

            if ( TryGeRegistryKey( CurrentVersion, "BuildBranch", out value ) ) {
                return value;
            }

            return null;
        }

        /// <summary>
        ///     Returns the Windows build.
        /// </summary>
        public static UInt32? BuildMajor() {
            dynamic value;

            if ( TryGeRegistryKey( CurrentVersion, "CurrentBuildNumber", out value ) ) {
                return Convert.ToUInt32( value );
            }

            if ( TryGeRegistryKey( CurrentVersion, "CurrentBuild", out value ) ) {
                return Convert.ToUInt32( value );
            }

            return null;
        }

        /// <summary>
        ///     Returns the Windows build.
        /// </summary>
        public static UInt32? BuildMinor() {
            dynamic value;

            if ( TryGeRegistryKey( CurrentVersion, "UBR", out value ) ) {
                return Convert.ToUInt32( value );
            }

            return null;
        }

        /// <summary>
        ///     Returns whether or not the current computer is a server or not.
        /// </summary>
        public static Boolean? IsServer() {
            dynamic installationType;
            if ( TryGeRegistryKey( CurrentVersion, "InstallationType", out installationType ) ) {
                return !installationType.Equals( "Client" );
            }

            return null;
        }

        /// <summary>
        ///     Returns the Windows build.
        /// </summary>
        public static UInt32? ReleaseID() {
            dynamic value;

            if ( TryGeRegistryKey( CurrentVersion, "ReleaseId", out value ) ) {
                return Convert.ToUInt32( value );
            }

            return null;
        }

        /// <summary>
        ///     Returns the Windows major version number for this computer.
        /// </summary>
        public static UInt32? WinMajorVersion() {
            dynamic value;

            // The 'CurrentMajorVersionNumber' string value in the CurrentVersion key is new for Windows 10,
            // and will most likely (hopefully) be there for some time before MS decides to change this - again...
            if ( TryGeRegistryKey( CurrentVersion, "CurrentMajorVersionNumber", out value ) ) {
                return ( UInt32 )value;
            }

            // When the 'CurrentMajorVersionNumber' value is not present we fallback to reading the previous key used for this: 'CurrentVersion'
            if ( !TryGeRegistryKey( CurrentVersion, "CurrentVersion", out value ) ) {
                return null;
            }

            var versionParts = ( ( String )value ).Split( '.' );
            if ( versionParts.Length != 2 ) {
                return null;
            }
            UInt32 majorAsUInt;
            return UInt32.TryParse( versionParts[ 0 ], out majorAsUInt ) ? ( UInt32? )majorAsUInt : null;
        }

        /// <summary>
        ///     Returns the Windows minor version number for this computer.
        /// </summary>
        public static UInt32? WinMinorVersion() {
            dynamic value;

            // The 'CurrentMinorVersionNumber' string value in the CurrentVersion key is new for Windows 10,
            // and will most likely (hopefully) be there for some time before MS decides to change this - again...
            if ( TryGeRegistryKey( CurrentVersion, "CurrentMinorVersionNumber", out value ) ) {
                return ( UInt32 )value;
            }

            // When the 'CurrentMinorVersionNumber' value is not present we fallback to reading the previous key used for this: 'CurrentVersion'
            if ( !TryGeRegistryKey( CurrentVersion, "CurrentVersion", out value ) ) {
                return null;
            }

            var versionParts = ( ( String )value ).Split( '.' );
            if ( versionParts.Length != 2 ) {
                return null;
            }
            UInt32 minorAsUInt;
            return UInt32.TryParse( versionParts[ 1 ], out minorAsUInt ) ? ( UInt32? )minorAsUInt : null;
        }

        private static Boolean TryGeRegistryKey( [NotNull] String path, [NotNull] String key, out dynamic value ) {
            if ( path == null ) {
                throw new ArgumentNullException( nameof( path ) );
            }
            if ( key == null ) {
                throw new ArgumentNullException( nameof( key ) );
            }
            value = null;
            try {
                using ( var rk = Registry.LocalMachine.OpenSubKey( path ) ) {
                    if ( rk == null ) {
                        return false;
                    }
                    value = rk.GetValue( key );
                    return value != null;
                }
            }
            catch {
                return false;
            }
        }
    }
}