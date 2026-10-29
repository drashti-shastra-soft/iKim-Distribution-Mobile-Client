using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.CustomeControls
{
    public class NavBarDrawable : IDrawable
    {
        public int ActiveIndex { get; set; } = 0;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float w = dirtyRect.Width;
            float h = dirtyRect.Height;

            if (w <= 0 || h <= 0) return; //  Guard against zero size

            float tabWidth = w / 5f;
            float cx = (ActiveIndex * tabWidth) + (tabWidth / 2f);

            float curveW = 22f;
            float curveH = 8f;
            float barTop = 22f;

            //  Clear background first
            canvas.FillColor = Colors.Transparent;
            canvas.FillRectangle(dirtyRect);

            var path = new PathF();

            path.MoveTo(0, barTop);
            path.LineTo(cx - curveW, barTop);

            // Left curve down
            path.CurveTo(
                cx - curveW + 8, barTop,
                cx - 10, barTop + curveH,
                cx, barTop + curveH
            );

            // Right curve up
            path.CurveTo(
                cx + 10, barTop + curveH,
                cx + curveW - 8, barTop,
                cx + curveW, barTop
            );

            path.LineTo(w, barTop);
            path.LineTo(w, h);
            path.LineTo(0, h);
            path.Close();

            //  Draw shadow border
            canvas.StrokeColor = Color.FromArgb("#20000000");
            canvas.StrokeSize = 1f;
            canvas.DrawPath(path);

            // Fill white
            canvas.FillColor = Colors.White;
            canvas.FillPath(path);
        }
    }
}
