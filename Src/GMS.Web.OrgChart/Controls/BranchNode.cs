using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GMS.Web.OrgChart.Models;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace GMS.Web.OrgChart.Controls
{
    public class BranchNode : ContentControl
    {
        public Branch Branch
        {
            get { return (Branch)GetValue(BranchProperty); }
            set { SetValue(BranchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Branch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BranchProperty =
            DependencyProperty.Register("Branch", typeof(Branch), typeof(BranchNode), new PropertyMetadata(null));
    
        #region Ctor
        public BranchNode()
            : base()
        {
            this.DefaultStyleKey = typeof(BranchNode);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var edit = this.GetTemplateChild("edit") as ButtonBase;
            if (edit != null)
            {
                edit.Click += (s, e) =>
                    {
                        var branchEdit = new BranchEdit();
                        branchEdit.DataContext = this.Branch;
                        branchEdit.Show();
                    };
            }

            this.titlePanel = this.GetTemplateChild("titlePanel") as Border;

            if (this.Branch.ParentBranch != null)
                this.BindDragEvent();
        }

        private void BindDragEvent()
        {
            bool isDragging = false;
            Point lastPosition = new Point(0, 0);

            Popup rootPopup = new Popup();
            BranchNode ghostContainer = null;

            Branch parentBranch = null;
            Border lastTitlePanel = null;


            this.titlePanel.MouseLeftButtonDown += (source, eventArgs) =>
            {
                this.IsHitTestVisible = false;

                isDragging = true;
                lastPosition = eventArgs.GetPosition(null);

                ghostContainer = new BranchNode();
                ghostContainer.Branch = this.Branch;
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

                    if (parentBranch != null && parentBranch != this.Branch.ParentBranch)
                    {
                        this.Branch.ParentBranch.Embranchment.Remove(this.Branch);
                        parentBranch.Embranchment.Add(this.Branch);
                        parentBranch.OnAppendBranch(this.Branch);
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

                        var parentBranchNode = this.GetParentBranchNode(element);
                        parentBranch = element.DataContext as Branch;
                        if (parentBranch.EnableAppendBranch)
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

        private BranchNode GetParentBranchNode(DependencyObject element)
        {
            var parentElement = VisualTreeHelper.GetParent(element);

            if (parentElement is BranchNode)
                return parentElement as BranchNode;
            else
                return GetParentBranchNode(parentElement);
        }

        private Border titlePanel;
    }
}
