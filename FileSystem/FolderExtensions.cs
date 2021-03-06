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
// "Librainian/FolderExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Parsing;
    using Threading;

    public static class FolderExtensions {
        public static readonly Char[] InvalidPathChars = Path.GetInvalidPathChars();

        public static String CleanupForFolder( [ NotNull ] this String foldername ) {
            if ( String.IsNullOrWhiteSpace( value: foldername ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", paramName: nameof(foldername) );
            }

            var sb = new StringBuilder( foldername.Length, UInt16.MaxValue / 2 );
            foreach ( var c in foldername ) {
                if ( !InvalidPathChars.Contains( c) ) {
                    sb.Append( c );
                }
            }

            /*
            var idx = foldername.IndexOfAny( InvalidPathChars );

			while ( idx.Any() ) {
                if ( idx.Any() ) {
                    foldername = foldername.Remove( idx, 1 );
                }
				idx = foldername.IndexOfAny( InvalidPathChars );
			}
            return foldername.Trim();
            */

            return sb.ToString().Trim();
        }

        /// <summary>
        ///     Returns a list of all files copied.
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destinationFolder"></param>
        /// <param name="searchPatterns"></param>
        /// <param name="overwriteDestinationDocuments"></param>
        /// <param name="crc">Calculate the CRC64 of source and destination documents.</param>
        /// <returns></returns>
        public static IEnumerable<DocumentCopyStatistics> CopyFiles( [NotNull] this Folder sourceFolder, [NotNull] Folder destinationFolder, IEnumerable<String> searchPatterns, Boolean overwriteDestinationDocuments = true, Boolean crc = true ) {
            if ( sourceFolder == null ) {
                throw new ArgumentNullException( nameof( sourceFolder ) );
            }
            if ( destinationFolder == null ) {
                throw new ArgumentNullException( nameof( destinationFolder ) );
            }

            var documentCopyStatistics = new ConcurrentBag<DocumentCopyStatistics>();

            if ( !sourceFolder.DemandPermission( FileIOPermissionAccess.Read ) ) {
                return documentCopyStatistics;
            }

            if ( !destinationFolder.DemandPermission( FileIOPermissionAccess.Write ) ) {
                return documentCopyStatistics;
            }

            var sourceFiles = sourceFolder.GetDocuments( searchPatterns );

            Parallel.ForEach( sourceFiles.AsParallel(), ThreadingExtensions.DiskIntensive, sourceDocument => {
                try {
                    var beginTime = DateTime.UtcNow;

                    var statistics = new DocumentCopyStatistics { TimeStarted = beginTime, SourceDocument = sourceDocument };

                    if ( crc ) {
                        statistics.SourceDocumentCRC64 = sourceDocument.Crc64();
                    }

                    var destinationDocument = new Document( destinationFolder, sourceDocument.FileName() );

                    if ( overwriteDestinationDocuments && destinationDocument.Exists() ) {
                        destinationDocument.Delete();
                    }

                    File.Copy( sourceDocument.FullPathWithFileName, destinationDocument.FullPathWithFileName );

                    if ( crc ) {
                        statistics.DestinationDocumentCRC64 = destinationDocument.Crc64();
                    }

                    var endTime = DateTime.UtcNow;

                    if ( !destinationDocument.Exists() ) {
                        return;
                    }

                    statistics.BytesCopied = destinationDocument.Size() ?? 0;
                    if ( crc ) {
                        statistics.BytesCopied *= 2;
                    }

                    statistics.TimeTaken = endTime - beginTime;
                    statistics.DestinationDocument = destinationDocument;
                    documentCopyStatistics.Add( statistics );
                }
                catch ( Exception ) {

                    //swallow any errors
                }
            } );

            return documentCopyStatistics;
        }

        public static IEnumerable<Folder> FindFolder( [NotNull] this String folderName ) {
            if ( folderName == null ) {
                throw new ArgumentNullException( nameof( folderName ) );
            }

            //First check across all known drives.
            var found = false;
            foreach ( var drive in DriveInfo.GetDrives() ) {
                var path = Path.Combine( drive.RootDirectory.FullName, folderName );
                var asFolder = new Folder( path );
                if ( asFolder.Exists() ) {
                    found = true;
                    yield return asFolder;
                }
            }
            if ( found ) {
                yield break;
            }

            //Next, check subfolders, beginning with the first drive.
            foreach ( var drive in Drive.GetDrives() ) {
                var folders = drive.GetFolders();
                foreach ( var folder in folders ) {
                    var parts = SplitPath( folder );
                    if ( parts.Any( s => s.Like( folderName ) ) ) {
                        found = true;
                        yield return folder;
                    }
                }
            }

            if ( !found ) {
            }
        }

        /// <summary>
        ///     <seealso cref="PathSplitter" />.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<String> SplitPath( String path ) {
            var parts = ( path ?? String.Empty ).Split( Folder.FolderSeparatorChar, StringSplitOptions.RemoveEmptyEntries );
            return parts;
        }

        /// <summary>
        ///     <seealso cref="PathSplitter" />.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static IEnumerable<String> SplitPath( [NotNull] this DirectoryInfo info ) {
            if ( info == null ) {
                throw new ArgumentNullException( nameof( info ) );
            }
            return SplitPath( info.FullName );
        }

        /// <summary>
        ///     <para>Returns true if the <see cref="Document" /> no longer seems to exist.</para>
        ///     <para>Returns null if existence cannot be determined.</para>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="tryFor"></param>
        /// <returns></returns>
        public static Boolean? TryDeleting( this Folder folder, TimeSpan tryFor ) {
            var stopwatch = StopWatch.StartNew();
            TryAgain:
            try {
                if ( !folder.Exists() ) {
                    return true;
                }
                Directory.Delete( folder.FullName );
                return !Directory.Exists( folder.FullName );
            }
            catch ( DirectoryNotFoundException ) { }
            catch ( PathTooLongException ) { }
            catch ( IOException ) {

                // IOExcception is thrown when the file is in use by any process.
                if ( stopwatch.Elapsed <= tryFor ) {
                    Thread.Yield();
                    Application.DoEvents();
                    goto TryAgain;
                }
            }
            catch ( UnauthorizedAccessException ) { }
            catch ( ArgumentNullException ) { }
            finally {
                stopwatch.Stop();
            }
            return null;
        }
    }
}