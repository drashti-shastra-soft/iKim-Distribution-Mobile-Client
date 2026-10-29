using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.CustomeControls
{
    public class BottomBarShapeDrawable : IDrawable
    {
        public int SelectedIndex { get; set; } = 0;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float width = dirtyRect.Width;
            float height = dirtyRect.Height;

            canvas.FillColor = Colors.White;

            float tabWidth = width / 5f; // adjust tab count
            float centerX = (SelectedIndex + 0.5f) * tabWidth;

            PathF path = new PathF();

            path.MoveTo(0, 0);
            path.LineTo(centerX - 60, 0);

            path.CurveTo(centerX - 30, 0, centerX - 30, 60, centerX, 60);
            path.CurveTo(centerX + 30, 60, centerX + 30, 0, centerX + 60, 0);

            path.LineTo(width, 0);
            path.LineTo(width, height);
            path.LineTo(0, height);
            path.Close();

            canvas.FillPath(path);
        }
    }
}
