﻿using System;
using Cairo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImGui;
using System.Diagnostics;
using Content = ImGui.Content;

namespace Test
{
    [TestClass]
    public class ContentTest
    {
        public void DrawContent(Rect rect, Content content, Style style)
        {
            var surface = CairoEx.BuildSurface((int)rect.Width, (int)rect.Height, CairoEx.ColorMetal, Format.Rgb24);
            var context = new Context(surface);

            context.DrawBoxModel(rect, content, style);

            string outputPath = "D:\\ContentTest";
            if (!System.IO.Directory.Exists(outputPath))
            {
                System.IO.Directory.CreateDirectory(outputPath);
            }

            string filePath = outputPath + "\\" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fff_") + surface.GetHashCode() + ".png";
            surface.WriteToPng(filePath);
            surface.Dispose();
            context.Dispose();

            Process.Start("rundll32.exe", @"""C:\Program Files\Windows Photo Viewer\PhotoViewer.dll"",ImageView_Fullscreen " + filePath);
        }

        [TestInitialize]
        public void Initialize()
        {
            Application.InitSysDependencies();
        }


        [TestMethod]
        public void ShowAnEmptyBox()
        {
            DrawContent(new Rect(400, 300), Content.None, Style.None);
        }

        [TestMethod]
        public void ShowATextLeadingAligned()
        {
            Rect rect = new Rect(400, 300);
            Content content = new Content("New Text");
            Style style = Skin.current.Label["Normal"];
            style.TextStyle = new TextStyle
            {
                LineSpacing = style.TextStyle.LineSpacing,
                TextAlignment = TextAlignment.Leading,
                TabSize = style.TextStyle.TabSize
            };
            content.Build(rect.Size, style);

            DrawContent(rect, content, style);
        }

        [TestMethod]
        public void ShowATextCenterAligned()
        {
            Rect rect = new Rect(400, 300);
            Content content = new Content("New Text");
            Style style = Skin.current.Label["Normal"];
            style.TextStyle = new TextStyle
            {
                LineSpacing = style.TextStyle.LineSpacing,
                TextAlignment = TextAlignment.Center,
                TabSize = style.TextStyle.TabSize
            };
            content.Build(rect.Size, style);

            DrawContent(rect, content, style);
        }

        [TestMethod]
        public void ShowATextTrailingAligned()
        {
            Content content = new Content("New Text");
            Style style = Skin.current.Label["Normal"];
            style.TextStyle = new TextStyle
            {
                LineSpacing = style.TextStyle.LineSpacing,
                TextAlignment = TextAlignment.Trailing,
                TabSize = style.TextStyle.TabSize
            };
            Rect rect = new Rect(400, 300);
            content.Build(rect.Size, style);

            DrawContent(rect, content, style);
        }

        [TestMethod]
        public void ShowATextAutoSized()
        {
            Content content = new Content("New Text");
            Style style = Style.Make(new[]
            {
                    new StyleModifier{Name = "BorderTop", Value = 10},
                    new StyleModifier{Name = "BorderRight", Value = 10},
                    new StyleModifier{Name = "BorderBottom", Value = 10},
                    new StyleModifier{Name = "BorderLeft", Value = 10},
                    
                    new StyleModifier{Name = "PaddingTop", Value = 10},
                    new StyleModifier{Name = "PaddingRight", Value = 10},
                    new StyleModifier{Name = "PaddingBottom", Value = 10},
                    new StyleModifier{Name = "PaddingLeft", Value = 10},
                new StyleModifier
                {
                    Name = "TextStyle",
                    Value = new TextStyle
                    {
                        TextAlignment = TextAlignment.Leading,
                    }
                }
            });
            Size size = style.CalcSize(content, new LayoutOption[0]);
            content.Build(size, style);
            Rect rect = new Rect(size);
            DrawContent(rect, content, style);
            content.Dispose();
        }

        [TestMethod]
        public void ShowATextWidthAutoSizedHeightFixed()
        {
            Content content = new Content("New Text");
            Style style = Style.Make(new[]
            {
                new StyleModifier
                {
                    Name = "TextStyle",
                    Value = new TextStyle
                    {
                        TextAlignment = TextAlignment.Leading,
                    }
                }
            });
            Size size = style.CalcSize(content, new []{GUILayout.Height(100)});
            content.Build(size, style);
            Rect rect = new Rect(size);
            DrawContent(rect, content, style);
            content.Dispose();
        }

        [TestMethod]
        public void ShowATextWidthFixedHeightAutoSized()
        {
            Content content = new Content("New Text");
            Style style = Style.Make(new[]
            {
                new StyleModifier
                {
                    Name = "TextStyle",
                    Value = new TextStyle
                    {
                        TextAlignment = TextAlignment.Leading,
                    }
                }
            });
            Size size = style.CalcSize(content, new[] { GUILayout.Width(100) });
            content.Build(size, style);
            Rect rect = new Rect(size);
            DrawContent(rect, content, style);
            content.Dispose();
        }


    }
}
