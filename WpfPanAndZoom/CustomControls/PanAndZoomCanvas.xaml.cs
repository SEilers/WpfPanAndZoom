using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfPanAndZoom.CustomControls
{
    /// <summary>
    /// Interaktionslogik für PanAndZoomCanvas.xaml
    /// https://stackoverflow.com/questions/35165349/how-to-drag-rendertransform-with-mouse-in-wpf
    /// </summary>
    public partial class PanAndZoomCanvas : Canvas
    {
        #region Variables
        private readonly MatrixTransform transform = new MatrixTransform();
        private Point pressedMouse;

        bool dragging;
        UIElement selectedElement;
        Vector draggingDelta;
        #endregion

        public PanAndZoomCanvas()
        {
            InitializeComponent();

            MouseDown += PanAndZoomCanvas_MouseDown;
            MouseUp += PanAndZoomCanvas_MouseUp;
            MouseMove += PanAndZoomCanvas_MouseMove;
            MouseWheel += PanAndZoomCanvas_MouseWheel;
        }

        private void PanAndZoomCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                pressedMouse = transform.Inverse.Transform(e.GetPosition(this));
            }


            if (e.ChangedButton == MouseButton.Left)
            {
                if (this.Children.Contains((UIElement)e.Source))
                {
                    selectedElement = (UIElement)e.Source;
                    Point mousePosition = Mouse.GetPosition(this);
                    double x = Canvas.GetLeft(selectedElement);
                    double y = Canvas.GetTop(selectedElement);
                    Point elementPosition = new Point(x, y);
                    draggingDelta = elementPosition - mousePosition;
                }
                dragging = true;
            }
        }

        private void PanAndZoomCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragging = false;
            selectedElement = null;
        }

        private void PanAndZoomCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Point mouse = transform.Inverse.Transform(e.GetPosition(this));
                Vector delta = Point.Subtract(mouse, pressedMouse);
                var translate = new TranslateTransform(delta.X, delta.Y);
                transform.Matrix = translate.Value * transform.Matrix;

                foreach (UIElement child in this.Children)
                {
                    child.RenderTransform = transform;
                }
            }

            if (dragging && e.LeftButton == MouseButtonState.Pressed)
            {
                double x = Mouse.GetPosition(this).X;
                double y = Mouse.GetPosition(this).Y;

                if (selectedElement != null)
                {
                    Canvas.SetLeft(selectedElement, x + draggingDelta.X);
                    Canvas.SetTop(selectedElement, y + draggingDelta.Y);
                }
            }
        }

        private void PanAndZoomCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            float scale = 1.1f;
            if (e.Delta < 0)
            {
                scale = 1f / scale;
            }
            Point mouse = e.GetPosition(this);

            Matrix matrix = transform.Matrix;
            matrix.ScaleAt(scale, scale, mouse.X, mouse.Y);
            transform.Matrix = matrix;

            foreach (UIElement child in this.Children)
            {
                double x = Canvas.GetLeft(child);
                double y = Canvas.GetTop(child);

                double sx = x * scale;
                double sy = y * scale;

                Canvas.SetLeft(child, sx);
                Canvas.SetTop(child, sy);

                child.RenderTransform = transform;
            }
        }
    }
}
