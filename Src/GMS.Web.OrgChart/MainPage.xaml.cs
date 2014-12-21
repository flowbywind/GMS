using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Browser;
using System.Net;
using Newtonsoft.Json;
using GMS.Web.OrgChart.Models;
using System.Collections.ObjectModel;


namespace GMS.Web.OrgChart
{
    public partial class MainPage : UserControl
    {
 
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(HtmlPage.Document.DocumentUri, "GetOrg");

            var wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);

            wc.DownloadStringAsync(uri);
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var branch = JsonConvert.DeserializeObject<Branch>(e.Result);

            this.Dispatcher.BeginInvoke(() =>
            {
                var mainBranch = branch.Embranchment.FirstOrDefault();
                if (mainBranch == null)
                    return;

                var unAllocateBranch = branch.Embranchment.Skip(1);
                var unAllocateStaff = branch.Staffs;

                orgChart = new OrgChart(mainBranch, unAllocateBranch, unAllocateStaff);
                orgChart.save.Click += new RoutedEventHandler(save_Click);
                this.activity.Content = orgChart;
                this.activity.IsActive = false;
            });
        }

        void save_Click(object sender, RoutedEventArgs e)
        {
            var mainBranch = orgChart.mainBranchNode.Branch;
            var unAllocateBranch = orgChart.unAllocateBranchNode.Branch.Embranchment;
            var unAllocateStaff = orgChart.unAllocateStaffNode.Branch.Staffs;

            var org = JsonConvert.SerializeObject(mainBranch, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            Uri uri = new Uri(HtmlPage.Document.DocumentUri, "SaveOrg");
            var wc = new WebClient();
            wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted);
            wc.UploadStringAsync(uri, org);

            this.activity.Message = "保存数据中，请稍等....";
            this.activity.IsActive = true;
        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.activity.IsActive = false;
                HtmlPage.Window.Alert(e.Result);
            });
        }

        private OrgChart orgChart;
    }
}
