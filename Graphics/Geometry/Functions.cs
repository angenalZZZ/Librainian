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
// "Librainian/Functions.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics.Geometry {

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Numerics;
    using DDD;

    public static class Functions {

        /// <summary>
        ///     Calculates the intersection line segment between 2 lines (not segments). Returns false
        ///     if no solution can be found.
        /// </summary>
        /// <returns></returns>
        public static Boolean CalculateLineLineIntersection( this Vector3 line1Point1, Vector3 line1Point2, Vector3 line2Point1, Vector3 line2Point2, out Vector3 resultSegmentPoint1, out Vector3 resultSegmentPoint2 ) {

            // Algorithm is ported from the C algorithm of
            // Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
            //resultSegmentPoint1 = Vector3.Empty;
            //resultSegmentPoint2 = Vector3.Empty;

            //var p1 = line1Point1;
            //var p3 = line2Point1;
            //var p4 = line2Point2;
            var p13 = line1Point1 - line2Point1;
            var p43 = line2Point2 - line2Point1;

            if ( p43.LengthSquared() < Single.Epsilon ) {
                resultSegmentPoint1 = new Vector3();
                resultSegmentPoint2 = new Vector3();
                return false;
            }

            var p2 = line1Point2;
            var p21 = p2 - line1Point1;
            if ( p21.LengthSquared() < Single.Epsilon ) {
                resultSegmentPoint1 = new Vector3();
                resultSegmentPoint2 = new Vector3();
                return false;
            }

            var d1343 = p13.X * p43.X + p13.Y * p43.Y + p13.Z * p43.Z;
            var d4321 = p43.X * p21.X + p43.Y * p21.Y + p43.Z * p21.Z;
            var d1321 = p13.X * p21.X + p13.Y * p21.Y + p13.Z * p21.Z;
            var d4343 = p43.X * p43.X + p43.Y * p43.Y + p43.Z * p43.Z;
            var d2121 = p21.X * p21.X + p21.Y * p21.Y + p21.Z * p21.Z;

            var denom = d2121 * d4343 - d4321 * d4321;
            if ( Math.Abs( denom ) < Single.Epsilon ) {
                resultSegmentPoint1 = new Vector3();
                resultSegmentPoint2 = new Vector3();
                return false;
            }

            var numer = d1343 * d4321 - d1321 * d4343;

            var mua = numer / denom;
            resultSegmentPoint1 = new Vector3 { X = line1Point1.X + mua * p21.X, Y = line1Point1.Y + mua * p21.Y, Z = line1Point1.Z + mua * p21.Z };

            var mub = ( d1343 + d4321 * mua ) / d4343;
            resultSegmentPoint2 = new Vector3 { X = line2Point1.X + mub * p43.X, Y = line2Point1.Y + mub * p43.Y, Z = line2Point1.Z + mub * p43.Z };

            return true;
        }

        /// <summary>
        ///     Calculates the intersection line segment between 2 lines (not segments). Returns false
        ///     if no solution can be found.
        /// </summary>
        /// <returns></returns>
        public static Boolean CalculateLineLineIntersection( this CoordinateF line1Point1, CoordinateF line1Point2, CoordinateF line2Point1, CoordinateF line2Point2, out CoordinateF resultSegmentPoint1, out CoordinateF resultSegmentPoint2 ) {
            Debugger.Break();

            // Algorithm is ported from the C algorithm of Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
            resultSegmentPoint1 = CoordinateF.Empty;
            resultSegmentPoint2 = CoordinateF.Empty;

            var p1 = line1Point1;
            var p2 = line1Point2;
            var p3 = line2Point1;
            var p4 = line2Point2;
            var p13 = p1 - p3;
            var p43 = p4 - p3;

            if ( p43.SquareLength < Single.Epsilon ) {
                return false;
            }
            var p21 = p2 - p1;
            if ( p21.SquareLength < Single.Epsilon ) {
                return false;
            }

            var d1343 = p13.X * p43.X + p13.Y * p43.Y + p13.Z * p43.Z;
            var d4321 = p43.X * p21.X + p43.Y * p21.Y + p43.Z * p21.Z;
            var d1321 = p13.X * p21.X + p13.Y * p21.Y + p13.Z * p21.Z;
            var d4343 = p43.X * p43.X + p43.Y * p43.Y + p43.Z * p43.Z;
            var d2121 = p21.X * p21.X + p21.Y * p21.Y + p21.Z * p21.Z;

            var denom = d2121 * d4343 - d4321 * d4321;
            if ( Math.Abs( denom ) < Single.Epsilon ) {
                return false;
            }
            var numer = d1343 * d4321 - d1321 * d4343;

            var mua = numer / denom;
            resultSegmentPoint1 = new CoordinateF( x: p1.X + mua * p21.X, y: p1.Y + mua * p21.Y, z: p1.Z + mua * p21.Z );

            var mub = ( d1343 + d4321 * mua ) / d4343;
            resultSegmentPoint2 = new CoordinateF( x: p3.X + mub * p43.X, y: p3.Y + mub * p43.Y, z: p3.Z + mub * p43.Z );

            return true;
        }

        /// <summary>
        /// Angles of a rectangle.
        /// </summary>
        [Flags]
        public enum RectAngles {
            None = 0,
            TopLeft = 1,
            TopRight = 2,
            BottomLeft = 4,
            BottomRight = 8,
            All = TopLeft | TopRight | BottomLeft | BottomRight
        }

        /// <summary>
        /// Draw and fill a rounded rectangle.
        /// </summary>
        /// <param name="g">The graphics object to use.</param>
        /// <param name="p">The pen to use to draw the rounded rectangle. If <code>null</code>, the border is not drawn.</param>
        /// <param name="b">The brush to fill the rounded rectangle. If <code>null</code>, the internal is not filled.</param>
        /// <param name="r">The rectangle to draw.</param>
        /// <param name="horizontalDiameter">Horizontal diameter for the rounded angles.</param>
        /// <param name="verticalDiameter">Vertical diameter for the rounded angles.</param>
        /// <param name="rectAngles">Angles to round.</param>
        public static void DrawAndFillRoundedRectangle( this Graphics g, Pen p, Brush b, Rectangle r, Int32 horizontalDiameter, Int32 verticalDiameter, RectAngles rectAngles ) {
            // get out data
            var x = r.X;
            var y = r.Y;
            var width = r.Width;
            var height = r.Height;
            
            // adapt horizontal and vertical diameter if the rectangle is too little
            if ( width < horizontalDiameter ) {
                horizontalDiameter = width;
            }

            if ( height < verticalDiameter ) {
                verticalDiameter = height;
            }
            /*
             * The drawing is the following:
             *
             *             a
             *      P______________Q
             *    h /              \ b
             *   W /                \R
             *    |                  |
             *  g |                  | c
             *   V|                  |S
             *    f\                / d
             *     U\______________/T
             *             e
             */
            Boolean tl = ( rectAngles & RectAngles.TopLeft ) != 0,
                tr = ( rectAngles & RectAngles.TopRight ) != 0,
                br = ( rectAngles & RectAngles.BottomRight ) != 0,
                bl = ( rectAngles & RectAngles.BottomLeft ) != 0;

            var pointP = tl ?
                new Point( x + horizontalDiameter / 2, y ) :
                new Point( x, y );

            var pointQ = tr ?
                new Point( x + width - horizontalDiameter / 2 - 1, y ) :
                new Point( x + width - 1, y );

            var pointR = tr ?
                new Point( x + width - 1, y + verticalDiameter / 2 ) :
                pointQ;

            var pointS = br ?
                new Point( x + width - 1, y + height - verticalDiameter / 2 - 1 ) :
                new Point( x + width - 1, y + height - 1 );

            var pointT = br ?
                new Point( x + width - horizontalDiameter / 2 - 1 ) :
                pointS;

            var pointU = bl ?
                new Point( x + horizontalDiameter / 2, y + height - 1 ) :
                new Point( x, y + height - 1 );

            var pointV = bl ?
                new Point( x, y + height - verticalDiameter / 2 - 1 ) :
                pointU;

            var pointW = tl ?
                new Point( x, y + verticalDiameter / 2 ) :
                pointP;

            using ( var gp = new GraphicsPath() ) {
                // a
                gp.AddLine( pointP, pointQ );
                
                // b
                if ( tr ) {
                    gp.AddArc( x + width - horizontalDiameter - 1, y, horizontalDiameter, verticalDiameter, 270, 90 );
                }

                // c
                gp.AddLine( pointR, pointS );
                
                // d
                if ( br ) {
                    gp.AddArc( x + width - horizontalDiameter - 1, y + height - verticalDiameter - 1, horizontalDiameter, verticalDiameter, 0, 90 );
                }

                // e
                gp.AddLine( pointT, pointU );
                
                // f
                if ( bl ) {
                    gp.AddArc( x, y + height - verticalDiameter - 1, horizontalDiameter, verticalDiameter, 90, 90 );
                }

                // g
                gp.AddLine( pointV, pointW );
                
                // h
                if ( tl ) {
                    gp.AddArc( x, y, horizontalDiameter, verticalDiameter, 180, 90 );
                }

                // end
                gp.CloseFigure();

                // draw
                if ( b != null ) {
                    g.FillPath( b, gp );
                }
                if ( p != null ) {
                    g.DrawPath( p, gp );
                }
            }
        }
    }
}