using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GMS.Web.OrgChart.Models;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace GMS.Web.OrgChart.Controls
{
    public class StaffNode : ContentControl
    {
        public Staff Staff
        {
            get { return (Staff)GetValue(StaffProperty); }
            set { SetValue(StaffProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Staff.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StaffProperty =
            DependencyProperty.Register("Staff", typeof(Staff), typeof(StaffNode), new PropertyMetadata(null));

        
        
        #region Ctor
        public StaffNode()
            : base()
        {
            this.DefaultStyleKey = typeof(StaffNode);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var edit = this.GetTemplateChild("edit") as ButtonBase;
            if (edit != null)
            {
                edit.Click += (s, e) =>
                    {
                        var staffEdit = new StaffEdit();
                        staffEdit.DataContext = this.Staff;
                        staffEdit.Show();
                    };
            }

            this.BindDragEvent();
            
        }

        private void BindDragEvent()
        {
            bool isDragging = false;
            Point lastPosition = new Point(0, 0);

            Popup rootPopup = new Popup();
            StaffNode ghostContainer = null;

            Branch parentBranch = null;
            Border lastTitlePanel = null;

            this.MouseLeftButtonDown += (source, eventArgs) =>
            {
                this.IsHitTestVisible = false;
                eventArgs.Handled = true;

                isDragging = true;
                lastPosition = eventArgs.GetPosition(null);

                ghostContainer = new StaffNode();
                ghostContainer.Staff = this.Staff;
                ghostContainer.Opacity = 0;
                ghostContainer.CaptureMouse();

                Point p = eventArgs.GetPosition(this);
                rootPopup.IsOpen = true;
                rootPopup.HorizontalOffset = lastPosition.X - p.X;
                rootPopup.VerticalOffset = lastPosition.Y - p.Y;

                rootPopup.Child = ghostContainer;
                rootPopup.IsOpen = true;

                ghostContainer.MouseLeftButtonUp += (s, e) =>
                {
                    rootPopup.Child = null;

                    isDragging = false;
                    this.ReleaseMouseCapture();
                    this.IsHitTestVisible = true;

                    if (lastTitlePanel != null)
                        lastTitlePanel.BorderBrush = lastTitlePanel.Resources["normalBorder"] as Brush;

                    if (parentBranch != null && parentBranch != this.Staff.ParentBranch)
                    {
                        this.Staff.ParentBranch.Staffs.Remove(this.Staff);
                        parentBranch.Staffs.Add(this.Staff);
                    }
                };

                ghostContainer.MouseMove += (s, e) =>
                {
                    if (!isDragging)
                        return;

                    if (lastTitlePanel != null)
                        lastTitlePanel.BorderBrush = lastTitlePanel.Resources["normalBorder"] as Brush;

                    ghostContainer.Opacity = 0.6;

                    UserControl rootPage = Application.Current.RootVisual as UserControl;
                    var elements = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(rootPage), rootPage).OfType<Border>().Where(b => b.Name == "titlePanel");


                    if (elements.Count() > 0)
                    {
                        var element = elements.Last();
                        parentBranch = element.DataContext as Branch;

                        if (parentBranch.EnableAppendStaff)
                        {
                            element.BorderBrush = element.Resources["hightlightBorder"] as Brush;
                            lastTitlePanel = element;
                        }
                        else
                        {
                            parentBranch = null;
                        }
                    }
                    else
                    {
                        parentBranch = null;
                    }


                    Point currentPosition = e.GetPosition(null);

                    double dX = currentPosition.X - lastPosition.X;
                    double dY = currentPosition.Y - lastPosition.Y;

                    lastPosition = currentPosition;

                    Transform oldTransform = ghostContainer.RenderTransform;
                    TransformGroup rt = new TransformGroup();
                    TranslateTransform newPos = new TranslateTransform();
                    newPos.X = dX;
                    newPos.Y = dY;

                    if (oldTransform != null)
                    {
                        rt.Children.Add(oldTransform);
                    }
                    rt.Children.Add(newPos);

                    MatrixTransform mt = new MatrixTransform();
                    mt.Matrix = rt.Value;

                    ghostContainer.RenderTransform = mt;

                };
            };
        }
        #endregion
    }
}
