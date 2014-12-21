using System;
using GMS.Web.OrgChart.Models;
using System.Windows.Controls;
using System.Windows;

namespace GMS.Web.OrgChart.Controls
{
    public partial class StaffEdit : ChildWindow
    {
        public StaffEdit()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var branch = this.DataContext as Staff;
            branch.Name = this.name.Text;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

