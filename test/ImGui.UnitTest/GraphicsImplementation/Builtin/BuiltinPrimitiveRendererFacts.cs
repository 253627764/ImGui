﻿using ImGui.Common.Primitive;
using ImGui.GraphicsImplementation;
using ImGui.Rendering;
using Xunit;

namespace ImGui.UnitTest.Rendering
{
    public class BuiltinPrimitiveRendererFacts
    {
        public class DrawPath
        {
            internal static void CheckExpectedImage(PathPrimitive primitive, int width, int height, string expectedImageFilePath)
            {
                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                    var mesh = new Mesh();
                    primitiveRenderer.DrawPathPrimitive(mesh, primitive, Vector.Zero);

                    context.Clear();
                    context.DrawShapeMesh(mesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Fact]
            public void StrokeAPath()
            {
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 80));
                primitive.PathLineTo(new Point(80, 80));
                primitive.PathLineTo(new Point(80, 10));
                primitive.PathClose();
                primitive.PathStroke(2, Color.Red);

                CheckExpectedImage(primitive, 100, 100,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawPath.StrokeAPath.png");
            }

            [Fact]
            public void FillAPath()
            {
                var primitive = new PathPrimitive();
                primitive.PathMoveTo(new Point(10, 10));
                primitive.PathLineTo(new Point(10, 80));
                primitive.PathLineTo(new Point(80, 80));
                primitive.PathClose();
                primitive.PathFill(Color.Red);

                CheckExpectedImage(primitive, 100, 100,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawPath.FillAPath.png");
            }

            [Fact]
            public void FillARect()
            {
                var primitive = new PathPrimitive();
                primitive.PathRect(new Rect(10, 10, 80, 60));
                primitive.PathFill(Color.Red);

                CheckExpectedImage(primitive, 100, 100,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawPath.FillARect.png");
            }
        }

        public class DrawText
        {
            internal static void CheckExpectedImage(TextPrimitive primitive, int width, int height, Rect contentRect, string expectedImageFilePath)
            {
                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                    var textMesh = new TextMesh();
                    primitiveRenderer.DrawTextPrimitive(textMesh, primitive, contentRect, new StyleRuleSet(), Vector.Zero);

                    context.Clear();
                    context.DrawTextMesh(textMesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }
                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Theory]
            [InlineData("Hello你好こんにちは")]
            [InlineData("textwithoutspace")]
            [InlineData("text with space")]
            public void DrawOnelineText(string text)
            {
                TextPrimitive primitive = new TextPrimitive(text);

                CheckExpectedImage(primitive, 200, 50, new Rect(10, 10, 200, 40),
                    $@"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawText.DrawOnelineText_{text}.png");
            }
        }

        public class DrawImage
        {
            internal static void CheckExpectedImage(ImagePrimitive primitive, int width, int height, Rect contentRect, StyleRuleSet style, string expectedImageFilePath)
            {
                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                    var mesh = new Mesh();
                    primitiveRenderer.DrawImagePrimitive(mesh, primitive, contentRect, style, Vector.Zero);

                    context.Clear();
                    context.DrawImageMesh(mesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Fact]
            public void DrawOriginalImage()
            {
                var primitive = new ImagePrimitive(@"assets\images\logo.png");
                var styleRuleSet = new StyleRuleSet {BackgroundColor = Color.White};

                CheckExpectedImage(primitive, 300, 400,
                    new Rect(10, 10, primitive.Image.Width, primitive.Image.Height), styleRuleSet,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawImage.DrawOriginalImage.png");
            }
        }

        public class DrawSlicedImage
        {
            internal static void CheckExpectedImage(ImagePrimitive primitive, int width, int height, Rect rect, StyleRuleSet style, string expectedImageFilePath)
            {
                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                    var mesh = new Mesh();
                    primitiveRenderer.DrawSlicedImagePrimitive(mesh, primitive, rect, style, Vector.Zero);

                    context.Clear();
                    context.DrawImageMesh(mesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Fact]
            public void DrawOneImage()
            {
                var primitive = new ImagePrimitive(@"assets\images\button.png");
                var styleRuleSet = new StyleRuleSet {BorderImageSlice = (83, 54, 54, 54)};

                CheckExpectedImage(primitive, 300, 400,
                    new Rect(2, 2, primitive.Image.Width + 50, primitive.Image.Height + 100), styleRuleSet,
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawSlicedImage.DrawOneImage.png");
            }
        }

        public class DrawBoxModel
        {
            [Fact]
            public void DrawEmptyBoxModel()
            {
                var styleRuleSet = new StyleRuleSet();
                var styleRuleSetBuilder = new StyleRuleSetBuilder(styleRuleSet);
                styleRuleSetBuilder
                    .BackgroundColor(Color.White)
                    .Border((1, 3, 1, 3))
                    .BorderColor(Color.Black)
                    .Padding((10, 5, 10, 5));
                var rect = new Rect(10, 10, 300, 60);
                const string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawBoxModel.DrawEmptyBoxModel.png";
                const int width = 400, height = 100;

                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                    var mesh = new Mesh();
                    mesh.CommandBuffer.Add(DrawCommand.Default);
                    primitiveRenderer.SetShapeMesh(mesh);
                    var textMesh = new TextMesh();
                    primitiveRenderer.SetTextMesh(textMesh);
                    var imageMesh = new Mesh();
                    primitiveRenderer.SetImageMesh(imageMesh);
                    primitiveRenderer.DrawBoxModel(rect, styleRuleSet);

                    context.Clear();
                    context.DrawShapeMesh(mesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Fact]
            public void DrawBoxModelWithTextContent()
            {
                TextPrimitive textPrimitive = new TextPrimitive("Hello你好こんにちは");
                var styleRuleSet = new StyleRuleSet();
                var styleRuleSetBuilder = new StyleRuleSetBuilder(styleRuleSet);
                styleRuleSetBuilder
                    .BackgroundColor(Color.White)
                    .Border((1, 3, 1, 3))
                    .BorderColor(Color.Black)
                    .Padding((10, 5, 10, 5))
                    .FontSize(24)
                    .FontColor(Color.Black);
                var rect = new Rect(10, 10, 350, 60);

                const string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawBoxModel.DrawBoxModelWithTextContent.png";
                const int width = 400, height = 100;

                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                    var mesh = new Mesh();
                    mesh.CommandBuffer.Add(DrawCommand.Default);
                    primitiveRenderer.SetShapeMesh(mesh);
                    var textMesh = new TextMesh();
                    primitiveRenderer.SetTextMesh(textMesh);
                    var imageMesh = new Mesh();
                    primitiveRenderer.SetImageMesh(imageMesh);
                    primitiveRenderer.DrawBoxModel(textPrimitive, rect, styleRuleSet);

                    context.Clear();
                    context.DrawShapeMesh(mesh);
                    context.DrawTextMesh(textMesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }

            [Fact]
            public void DrawBoxModelWithImageContent()
            {
                var primitive = new ImagePrimitive(@"assets\images\logo.png");

                var ruleSet = new StyleRuleSet();
                var styleSetBuilder = new StyleRuleSetBuilder(ruleSet);
                styleSetBuilder
                    .BackgroundColor(Color.White)
                    .Border((top: 1, right: 3, bottom: 1, left: 3))
                    .BorderColor(Color.LightBlue)
                    .Padding((10, 5, 10, 5));
                var rect = new Rect(10, 10, 300, 400);

                const string expectedImageFilePath =
                    @"GraphicsImplementation\Builtin\images\BuiltinPrimitiveRendererFacts.DrawBoxModel.DrawBoxModelWithImageContent.png";
                const int width = 500, height = 500;

                byte[] imageRawBytes;
                using (var context = new RenderContextForTest(width, height))
                {
                    BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                    var mesh = new Mesh();
                    mesh.CommandBuffer.Add(DrawCommand.Default);
                    primitiveRenderer.SetShapeMesh(mesh);
                    var imageMesh = new Mesh();
                    primitiveRenderer.SetImageMesh(imageMesh);
                    primitiveRenderer.DrawBoxModel(primitive, rect, ruleSet);

                    context.Clear();
                    context.DrawShapeMesh(mesh);
                    context.DrawImageMesh(imageMesh);

                    imageRawBytes = context.GetRenderedRawBytes();
                }

                Util.CheckExpectedImage(imageRawBytes, width, height, expectedImageFilePath);
            }
        }
    }
}
