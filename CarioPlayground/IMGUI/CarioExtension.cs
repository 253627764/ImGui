﻿//#define DrawContentBox
//#define DrawPaddingBox

using System;
using Cairo;
using Context = Cairo.Context;
using Layout = IMGUI.ITextLayout;


namespace IMGUI
{
    public static class CairoEx
    {
        #region Basic element rendering

        /// <summary>
        /// Draw a box model
        /// </summary>
        /// <param name="g">the Cairo context</param>
        /// <param name="rect">the rect (of the border-box) in which to draw this box model </param>
        /// <param name="content">content of the box mode</param>
        /// <param name="style">style of the box model</param>
        public static void DrawBoxModel(this Context g, Rect rect, Content content, Style style)
        {
            //Widths of border
            var bt = style.BorderTop;
            var br = style.BorderRight;
            var bb = style.BorderBottom;
            var bl = style.BorderLeft;

            //Widths of padding
            var pt = style.PaddingTop;
            var pr = style.PaddingRight;
            var pb = style.PaddingBottom;
            var pl = style.PaddingLeft;

            /*
             * TODO Margin is temporarily not used. Margin should be used in layouting rects.
             */
            var mt = style.MarginTop;
            var mr = style.MarginRight;
            var mb = style.MarginBottom;
            var ml = style.MarginLeft;

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var btr = new Point(rect.Right, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);
            var bbl = new Point(rect.Left, rect.Bottom);
            var borderBoxRect = new Rect(btl, bbr);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var ptr = new Point(btr.X - br, btr.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);
            var pbl = new Point(bbl.X + bl, bbl.Y - bb);
            var paddingBoxRect = new Rect(ptl, pbr);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var ctr = new Point(ptr.X - pr, ptr.Y + pr);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var cbl = new Point(pbl.X + pl, pbl.Y - pb);
            var contentBoxRect = new Rect(ctl, cbr);

            /*
             * Render from inner to outer: Content, Padding, Border, Margin, Outline
             */

            //Content
            //Content-box background(draw as a filled rectangle now)
            g.FillRectangle(rect, style.BackgroundStyle.Color);
            //Content-box
            if (content != null)
            {
                if (content.Image != null)
                {
                    g.DrawImage(contentBoxRect, content.Image);
                }
                if (content.Layout != null)
                {
                    g.DrawText(contentBoxRect, content.Layout, style.Font, style.TextStyle);
                }
            }

            //Border
            //  Top
            if (bt != Length.Zero)
            {
                g.FillPolygon(new[] { ptl, btl, btr, ptr }, style.BorderTopColor);
            }
            //  Right
            if (br != Length.Zero)
            {
                g.FillPolygon(new[] { ptr, btr, bbr, pbr }, style.BorderRightColor);
            }
            //  Bottom
            if (bb != Length.Zero)
            {
                g.FillPolygon(new[] { pbr, bbr, bbl, pbl }, style.BorderBottomColor);
            }
            //  Left
            if (bl != Length.Zero)
            {
                g.FillPolygon(new[] {pbl, bbl, btl, ptl}, style.BorderLeftColor);
            }

            //Outline
            if(style.OutlineWidth != Length.Zero)
            {
                g.Rectangle(borderBoxRect.TopLeft.ToPointD(), borderBoxRect.Width, borderBoxRect.Height);
                g.LineWidth = style.OutlineWidth;
                g.SetSourceColor(style.OutlineColor);
                g.Stroke();
            }

#if DrawPaddingBox
            g.Rectangle(paddingBoxRect.TopLeft.ToPointD(), paddingBoxRect.Width, paddingBoxRect.Height);
            g.LineWidth = 1;
            g.SetSourceColor(CairoEx.ColorRgb(0,100,100));
            g.Stroke();
#endif

#if DrawContentBox
            g.Rectangle(contentBoxRect.TopLeft.ToPointD(), contentBoxRect.Width, contentBoxRect.Height);
            g.LineWidth = 1;
            g.SetSourceColor(CairoEx.ColorRgb(100, 0, 100));
            g.Stroke();
#endif
        }

        public static Size MeasureBoxModel(Content content, Style style)
        {
            //Widths of border
            var bt = style.BorderTop;
            var br = style.BorderRight;
            var bb = style.BorderBottom;
            var bl = style.BorderLeft;

            //Widths of padding
            var pt = style.PaddingTop;
            var pr = style.PaddingRight;
            var pb = style.PaddingBottom;
            var pl = style.PaddingLeft;
            
            /*
             * TODO Margin is temporarily not used(all zero)
             */
            var mt = style.MarginTop;
            var mr = style.MarginRight;
            var mb = style.MarginBottom;
            var ml = style.MarginLeft;

            //calc box model rect
            var boxRect = content.GetRect();
            boxRect.Inflate(
                br + bl + pr + pl + mr + ml,
                bt + bb + pt + pb + mt + mb);
            return boxRect.Size;
        }

        #region primitive draw helpers

        internal static void StrokeRectangle(this Context g,
            Rect rect,
            Color topColor,
            Color rightColor,
            Color bottomColor,
            Color leftColor)
        {
            g.NewPath();
            g.MoveTo(rect.TopLeft.ToPointD());
            g.SetSourceColor(topColor);
            g.LineTo(rect.TopRight.ToPointD()); //Top
            g.SetSourceColor(rightColor);
            g.LineTo(rect.BottomRight.ToPointD()); //Right
            g.SetSourceColor(bottomColor);
            g.LineTo(rect.BottomLeft.ToPointD()); //Bottom
            g.SetSourceColor(leftColor);
            g.LineTo(rect.TopLeft.ToPointD()); //Left
            g.ClosePath();
            g.Stroke();
        }

        internal static void StrokeRectangle(this Context g,
            Rect rect,
            Color color)
        {
            g.NewPath();
            g.MoveTo(rect.TopLeft.ToPointD());
            g.SetSourceColor(color);
            g.LineTo(rect.TopRight.ToPointD()); //Top
            g.LineTo(rect.BottomRight.ToPointD()); //Right
            g.LineTo(rect.BottomLeft.ToPointD()); //Bottom
            g.LineTo(rect.TopLeft.ToPointD()); //Left
            g.ClosePath();
            g.Stroke();
        }

        internal static void FillRectangle(this Context g,
            Rect rect,
            Color color)
        {
            g.NewPath();
            g.MoveTo(rect.TopLeft.ToPointD());
            g.SetSourceColor(color);
            g.LineTo(rect.TopRight.ToPointD());
            g.LineTo(rect.BottomRight.ToPointD());
            g.LineTo(rect.BottomLeft.ToPointD());
            g.LineTo(rect.TopLeft.ToPointD());
            g.ClosePath();
            g.Fill();
        }

        internal static void FillPolygon(this Context g, Point[] vPoint, Color color)
        {
            if (vPoint.Length <= 2)
            {
                throw new ArgumentException("vPoint should contains 3 ore more points!");
            }

            g.NewPath();
            g.SetSourceColor(color);
            g.MoveTo(vPoint[0].ToPointD());
            foreach (var t in vPoint)
            {
                g.LineTo(t.ToPointD());
            }
            g.ClosePath();
            g.Fill();
        }

        internal static void StrokeLineStrip(this Context g, Point[] vPoint, Color color)
        {
            if (vPoint.Length <= 2)
            {
                throw new ArgumentException("vPoint should contains 3 ore more points!");
            }

            g.NewPath();
            g.SetSourceColor(color);
            g.MoveTo(vPoint[0].ToPointD());
            foreach (var t in vPoint)
            {
                g.LineTo(t.ToPointD());
            }
            g.Stroke();
        }

        internal static void StrokePolygon(this Context g, Point[] vPoint, Color color)
        {
            if (vPoint.Length <= 2)
            {
                throw new ArgumentException("vPoint should contains 3 ore more points!");
            }

            g.NewPath();
            g.SetSourceColor(color);
            g.MoveTo(vPoint[0].ToPointD());
            foreach (var t in vPoint)
            {
                g.LineTo(t.ToPointD());
            }
            g.ClosePath();
            g.Stroke();
        }

        internal static void StrokeCircle(this Context g, PointD center, float radius, Color color)
        {
            g.NewPath();
            g.LineWidth = 1;
            g.SetSourceColor(color);
            g.Arc(center.X, center.Y, radius, 0, 2 * Math.PI);
            g.Stroke();
        }

        internal static void FillCircle(this Context g, PointD center, float radius, Color color)
        {
            g.NewPath();
            g.SetSourceColor(color);
            g.Arc(center.X, center.Y, radius, 0, 2 * Math.PI);
            g.Fill();
        }

        internal static void DrawLine(this Context g, Point p0, Point p1, float width, Color color)
        {
            g.NewPath();
            g.Save();
            g.SetSourceColor(color);
            g.LineWidth = width;
            g.MoveTo(p0.ToPointD());
            g.LineTo(p1.ToPointD());
            g.Stroke();
            g.Restore();
        }
        #endregion


        #region text
        /// <summary>
        /// Draw layouted text
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect">region of the text</param>
        /// <param name="layout">text laout</param>
        /// <param name="font">font</param>
        /// <param name="textStyle">text styles</param>
        public static void DrawText(this Context g, Rect rect, Layout layout, Font font, TextStyle textStyle)
        {
            g.NewPath();
            g.SetSourceColor(font.Color);
            Point p = rect.TopLeft;
            g.MoveTo(p.ToPointD());
            layout.BuildPath(g);
            g.AppendPath(layout.Path);
            g.Fill();
        }
        #endregion

        #endregion

        #region Color

        public static readonly Color ColorClear = ColorArgb(0, 0, 0, 0);
        public static readonly Color ColorBlack = ColorRgb(0, 0, 0);
        public static readonly Color ColorWhite = ColorRgb(255, 255, 255);
        public static readonly Color ColorMetal = ColorRgb(192, 192, 192);
        public static readonly Color ColorBlue = ColorRgb(0, 0, 255);
        public static readonly Color ColorLightBlue = ColorRgb(46, 167, 224);
        public static readonly Color ColorDarkBlue = ColorRgb(3, 110, 184);
        public static readonly Color ColorPink = ColorRgb(255, 192, 203);

        

        public static void SetSourceColor(this Context context, Color color)
        {
            context.SetSourceRGBA(color.R,color.G,color.B,color.A);
        }

        public static Color ColorRgb(byte r, byte g, byte b)
        {
            return new Color(r/255.0,g/255.0,b/255.0,1.0);
        }

        public static Color ColorArgb(byte a, byte r, byte g, byte b)
        {
            return new Color(r/255.0,g/255.0,b/255.0,a/255.0);
        }

        public static Color ColorArgb(uint colorValue)
        {
            return ColorArgb(
                (byte) ((colorValue >> 24) & 0xff),
                (byte) ((colorValue >> 16) & 0xff),
                (byte) ((colorValue >> 8) & 0xff),
                (byte) (colorValue & 0xff)
                );
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - saturation));
            byte q = Convert.ToByte(value * (1 - f * saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            if(hi == 0)
                return CairoEx.ColorArgb(255, v, t, p);
            if(hi == 1)
                return CairoEx.ColorArgb(255, q, v, p);
            if(hi == 2)
                return CairoEx.ColorArgb(255, p, v, t);
            if(hi == 3)
                return CairoEx.ColorArgb(255, p, q, v);
            if(hi == 4)
                return CairoEx.ColorArgb(255, t, p, v);

            return CairoEx.ColorArgb(255, v, p, q);
        }
#endregion

        #region Image
        //TODO specfiy image fill method (fill, unscale, 9-box, etc.) and alignment
        public static void DrawImage(this Context g, Rect destRect, Rect srcRect, Texture image)
        {
            float xScale = (float)destRect.Width / (float)srcRect.Width;
            float yScale = (float)destRect.Height / (float)srcRect.Height;
            g.Save();
            g.Translate(destRect.X, destRect.Y);
            g.Scale(xScale, yScale);
            g.SetSourceSurface(image._surface, 0, 0);
            g.Rectangle(destRect.TopLeft.ToPointD(), destRect.Width, destRect.Height);
            g.Paint();
            g.Restore();
        }

        public static void DrawImage(this Context g, Rect rect, Texture image)
        {
            g.DrawImage(rect, new Rect(new Size(image.Width, image.Height)), image);
        }

        public static ImageSurface CreateImage(string pngFilePath)
        {
            ImageSurface surface;
            try
            {
                surface = new ImageSurface(pngFilePath);
            }
            catch (Exception)
            {
                return null;
            }
            return surface;
        } 

        #endregion

        static CairoEx()
        {
        }

        /// <summary>
        /// Build a ImageSurface
        /// </summary>
        /// <param name="Width">width</param>
        /// <param name="Height">height</param>
        /// <param name="Color">color</param>
        /// <param name="format">surface format</param>
        /// <returns>the created ImageSurface</returns>
        public static ImageSurface BuildSurface(int Width, int Height, Color Color, Format format)
        {
            var surface = new ImageSurface(format, Width, Height);
            var c = new Context(surface);
            c.Rectangle(0, 0, Width, Height);
            c.SetSourceColor(Color);
            c.Fill();
            c.Dispose();
            return surface;
        }
    }

}
