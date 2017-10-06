using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Frame), typeof(TextGameExperiment.Droid.Renderers.FrameRenderer))]
namespace TextGameExperiment.Droid.Renderers
{        
    public class FrameRenderer : Xamarin.Forms.Platform.Android.FrameRenderer
    {
        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            DrawOutline(canvas, canvas.Width, canvas.Height, 4f); // 4f = corner radius
        }

        private void DrawOutline(Canvas canvas, int width, int height, float cornerRadius)
        {
            using (var paint = new Paint { AntiAlias = true })
            using (var path = new Path())
            using (Path.Direction direction = Path.Direction.Cw)
            using (Paint.Style style = Paint.Style.Stroke)
            using (var rect = new RectF(0, 0, width, height))
            {
                float rx = Forms.Context.ToPixels(cornerRadius);
                float ry = Forms.Context.ToPixels(cornerRadius);
                path.AddRoundRect(rect, rx, ry, direction);

                paint.StrokeWidth = 2f;
                paint.SetStyle(style);
                paint.Color = this.Element.OutlineColor.ToAndroid();
                canvas.DrawPath(path, paint);
            }

        }
    }
}