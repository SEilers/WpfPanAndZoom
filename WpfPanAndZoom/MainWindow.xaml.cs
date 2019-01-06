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

namespace WpfPanAndZoom
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        float _zoom = 1;

        bool dragging;

        bool panning = false;

        double startPanningX;
        double startPanningY;

        double endPanningX;
        double endPanningY;

        double dx;
        double dy;

        TransformGroup transformGroup = new TransformGroup();

        Transform translation = new System.Windows.Media.TranslateTransform(0, 0);
        Transform scale = new System.Windows.Media.ScaleTransform(1, 1);

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            mwGrid.MouseWheel += MwGrid_MouseWheel;
            mwGrid.MouseDown += MwGrid_MouseDown;
            mwGrid.MouseMove += MwGrid_MouseMove;
            mwGrid.MouseUp += MwGrid_MouseUp;

            transformGroup.Children.Add(scale);
            transformGroup.Children.Add(translation);

            CustomControls.Widget w1 = new CustomControls.Widget();
            w1.Width = 200;
            w1.Height = 150;
            mwGrid.Children.Add(w1);

            w1.Border.BorderThickness = new Thickness(3);
            w1.Header.Text = "Widget 1";
           
            transformGroup.Children[1] = new TranslateTransform(10, 10);
            w1.RenderTransform = transformGroup;

            //CustomControls.Widget w2 = new CustomControls.Widget();
            //w2.Width = 200;
            //w2.Height = 150;
            //mwGrid.Children.Add(w2);

            //transformGroup.Children[1] = new TranslateTransform(200, 200);
            //w2.RenderTransform = transformGroup;

        
          








        }

        private void MwGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            panning = false;
            endPanningX = dx;
            endPanningY = dy;
        }

        private void MwGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (panning && e.RightButton == MouseButtonState.Pressed)
            {
                double currentX = Mouse.GetPosition(this).X;
                double currentY = Mouse.GetPosition(this).Y;
                dx = currentX - startPanningX;
                dy = currentY - startPanningY;
                transformGroup.Children[1] = new TranslateTransform(endPanningX + dx, endPanningY + dy);
                mwGrid.RenderTransform = transformGroup;
            }

            if (dragging && e.LeftButton == MouseButtonState.Pressed)
            {
                double x = Mouse.GetPosition(mwGrid).X;
                double y = Mouse.GetPosition(mwGrid).Y;

                //if (selectedControl != null)
                //{
                //    //selectedControl.RenderTransform = new TranslateTransform(x, y);
                //    Canvas.SetLeft(selectedControl, x);
                //    Canvas.SetTop(selectedControl, y);
                //}
            }
        }

        private void MwGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                panning = true;
                startPanningX = Mouse.GetPosition(this).X;
                startPanningY = Mouse.GetPosition(this).Y;
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                dragging = true;
            }
        }

        private void MwGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                _zoom *= 1.1f;
            }
            else
            {
                _zoom *= 0.9f;
            }

            transformGroup.Children[0] = new System.Windows.Media.ScaleTransform(_zoom, _zoom);

            mwGrid.RenderTransform = transformGroup;
        }
    }
}
