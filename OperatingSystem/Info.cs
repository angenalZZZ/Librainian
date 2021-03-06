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
// "Librainian/Info.cs" was last cleaned by Rick on 2016/12/02 at 9:10 PM

namespace Librainian.OperatingSystem {

	using System;
	using JetBrains.Annotations;
	using Microsoft.Win32;

	/// <summary>
	///     Static class that adds convenient methods for getting information on the running computers
	///     basic hardware and os setup.
	/// </summary>
	/// <remarks>Adapted from <see cref="http://stackoverflow.com/a/37755503/956364" />.</remarks>
	public static class Info {

		private const String CurrentVersion = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";

		/// <summary>
		///     Returns the Windows build. "rs1_release"
		/// </summary>
		[CanBeNull]
		public static String BuildBranch() =>

#pragma warning disable CC0021 // Use nameof
			Info.TryGetRegistryKeyHKLM( Info.CurrentVersion, "BuildBranch", out var value ) ? value : null;
#pragma warning restore CC0021 // Use nameof


		/// <summary>
		///     Returns the Windows build. "14393"
		/// </summary>
		public static UInt32? BuildMajor() {

			if ( Info.TryGetRegistryKeyHKLM( Info.CurrentVersion, "CurrentBuildNumber", out var value ) ) {
				return Convert.ToUInt32( value );
			}

			if ( Info.TryGetRegistryKeyHKLM( Info.CurrentVersion, "CurrentBuild", out value ) ) {
				return Convert.ToUInt32( value );
			}

			return null;
		}

		/// <summary>
		///     Returns the Windows build.
		/// </summary>
		public static UInt32? BuildMinor() {

			if ( Info.TryGetRegistryKeyHKLM( Info.CurrentVersion, "UBR", out var value ) ) {
				return Convert.ToUInt32( value );
			}

			return null;
		}

		/// <summary>
		///     Returns whether or not the current computer is a server or not.
		/// </summary>
		public static Boolean? IsServer() {
			if ( Info.TryGetRegistryKeyHKLM( Info.CurrentVersion, "InstallationType", out var installationType ) ) {
				return !installationType.Like( "Client" );
			}

			return null;
		}

		/// <summary>
		///     Returns the Windows release id.
		/// </summary>
		public static UInt32? ReleaseId() => Info.TryGetRegistryKeyHKLM( Info.CurrentVersion, "ReleaseId", out var value ) ? Convert.ToUInt32( value ) : null;

		public static Boolean TryGetRegistryKeyHKLM( [ NotNull ] String path, [ NotNull ] String key, out dynamic value ) {
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
#pragma warning disable CC0003 // Your catch maybe include some Exception
			catch {
				return false;
			}
#pragma warning restore CC0003 // Your catch maybe include some Exception
		}

		/// <summary>
		///     Returns the Windows major version number for this computer.
		/// </summary>
		public static UInt32? VersionMajor() => Info.TryGetRegistryKeyHKLM( Info.CurrentVersion, "CurrentMajorVersionNumber", out var value ) ? ( UInt32? )( UInt32 )value : null;

		/// <summary>
		///     Returns the Windows minor version number for this computer.
		/// </summary>
		public static UInt32? VersionMinor() => Info.TryGetRegistryKeyHKLM( Info.CurrentVersion, "CurrentMinorVersionNumber", out var value ) ? ( UInt32? )( UInt32 )value : null;

	}

}
