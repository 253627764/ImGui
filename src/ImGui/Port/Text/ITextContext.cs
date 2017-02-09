﻿using System;

namespace ImGui
{
    /// <summary>
    /// Text-related(layout, hit-test, etc.) functions
    /// </summary>
    internal interface ITextContext : IDisposable
    {
        /// <summary>
        /// size of the font in the text
        /// </summary>
        int FontSize { get; }

        /// <summary>
        /// alignment of the text in the rectangle
        /// </summary>
        TextAlignment Alignment { get; set; }

        /// <summary>
        /// max width of the text
        /// </summary>
        int MaxWidth { get; set; }

        /// <summary>
        /// max height of the text
        /// </summary>
        int MaxHeight { get; set; }

        /// <summary>
        /// the layout box
        /// </summary>
        Rect Rect { get; }

        /// <summary>
        /// the text
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// text building callbacks used to retrive text rendering data
        /// </summary>
        /// <param name="offset">offset of the text base-point against the rectangle position(bottom-left of the rectangle)</param>
        /// <param name="pointAdder">called when a point is added</param>
        /// <param name="bezierAdder">called when a bezier segment is added</param>
        /// <param name="pathCloser">called when the current path should be closed</param>
        /// <param name="figureBeginner">call when a figure begins</param>
        /// <param name="figureEnder">call when a figure ends</param>
        void Build(Point offset, PointAdder pointAdder, BezierAdder bezierAdder, PathCloser pathCloser, FigureBeginner figureBeginner, FigureEnder figureEnder);

        /// <summary>
        /// Get nearest character index from the point.
        /// </summary>
        /// <param name="pointX">x, relative to the top-left location of the layout box.</param>
        /// <param name="pointY">y, relative to the top-left location of the layout box.</param>
        /// <param name="isInside">whether the point is inside the text string</param>
        /// <returns>nearest character index from the point</returns>
        uint XyToIndex(float pointX, float pointY, out bool isInside);

        /// <summary>
        /// Given a character index and whether the caret is on the leading or trailing edge of that position.
        /// </summary>
        /// <param name="textPosition">character index</param>
        /// <param name="isTrailingHit">whether the caret is on the leading or trailing edge of that position</param>
        /// <param name="pointX">position x</param>
        /// <param name="pointY">position y</param>
        /// <param name="height">the height of the text</param>
        void IndexToXY(uint textPosition, bool isTrailingHit,
            out float pointX, out float pointY, out float height);
    }
}