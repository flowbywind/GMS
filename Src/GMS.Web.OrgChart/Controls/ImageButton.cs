using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace GMS.Web.OrgChart.Controls
{
	public class ImageButton : Button
	{
        public ImageButton()
            : base()
		{
            this.DefaultStyleKey = typeof(ImageButton);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();	
		}

        public static readonly DependencyProperty normalImageProperty = DependencyProperty.Register("NormalImage", typeof(ImageSource), typeof(ImageButton), null);
        public ImageSource NormalImage
        {
            get { return (ImageSource)GetValue(normalImageProperty); }
            set { SetValue(normalImageProperty, value); }
        }

        public static readonly DependencyProperty hoverImageProperty = DependencyProperty.Register("HoverImage", typeof(ImageSource), typeof(ImageButton), null);
        public ImageSource HoverImage
        {
            get { return (ImageSource)GetValue(normalImageProperty); }
            set { SetValue(hoverImageProperty, value); }
        }

        public static readonly DependencyProperty clickImageProperty = DependencyProperty.Register("ClickImage", typeof(ImageSource), typeof(ImageButton), null);
        public ImageSource ClickImage
        {
            get { return (ImageSource)GetValue(normalImageProperty); }
            set { SetValue(clickImageProperty, value); }
        }
	}
}
