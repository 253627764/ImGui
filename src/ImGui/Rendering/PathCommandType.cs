﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui.Rendering
{
    internal enum PathCommandType
    {
        PathMoveTo,
        PathLineTo,
        PathCurveTo,
        PathClosePath,
        PathArc,
        Stroke,
        Fill,
    }
}
