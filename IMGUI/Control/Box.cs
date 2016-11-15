﻿using System.Diagnostics;

namespace ImGui
{
    /// <summary>
    /// Box
    /// </summary>
    /// <remarks>
    /// The box is a simple control containing an optinal Content.
    /// </remarks>
    internal class Box
    {
        public Box(string name, Form form, Content content)
        {
        }

        public static void DoControl(Rect rect, Content content, string name)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUIPrimitive.DrawBoxModel(rect, content, Skin.current.Box);
            }
        }
    }
}