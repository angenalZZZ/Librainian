// Copyright 2015 Rick@AIBrain.org.
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
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/Disk.cs" was last cleaned by Rick on 2015/11/13 at 11:30 PM

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Management;
    using Extensions;

    /// <summary>
    ///     A physical storage medium. HD, usb, dvd, etc...
    /// </summary>
    [Immutable]
    public class Disk {

        public Disk( Int32 diskNumber ) {
            this.SerialNumber = GetSerialNumber( diskNumber );
        }

        public String SerialNumber { get; private set; }

        public static String GetSerialNumber( Int32 diskNumber ) {
            //TODO
            var mosDisks = new ManagementObjectSearcher( "SELECT * FROM Win32_DiskDrive" );

            // Loop through each object (disk) retrieved by WMI
            foreach ( var o in mosDisks.Get() ) {
                var moDisk = o as ManagementObject;

                // Add the HDD to the list (use the Model field as the item's caption)
                if ( moDisk != null ) {
                    return moDisk[ "Model" ].ToString();
                }
            }
            return String.Empty;
        }

    }

}
