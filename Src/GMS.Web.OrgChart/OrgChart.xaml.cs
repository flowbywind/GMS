using System;
using System.Linq;
using System.Windows.Controls;
using GMS.Web.OrgChart.Controls;
using GMS.Web.OrgChart.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace GMS.Web.OrgChart
{
    public partial class OrgChart : UserControl
    {
        public OrgChart()
        {
            InitializeComponent();
        }

        public OrgChart(Branch mainBranch, IEnumerable<Branch> unAllocateBranch, IEnumerable<Staff> unAllocateStaff)
            : this()
        {
            this.mainBranchNode.Branch = mainBranch;

            var branch = new Branch() { Name = "未分配的员工", EnableAppendBranch = false };
            branch.Staffs = new ObservableCollection<Staff>(unAllocateStaff);
            this.unAllocateStaffNode.Branch = branch;

            branch = new Branch() { Name = "未分配的部门", EnableAppendStaff = false };
            branch.Embranchment = new ObservableCollection<Branch>(unAllocateBranch);
            branch.AppendBranch += new Action<Branch>(branch_AppendBranch);
            this.unAllocateBranchNode.Branch = branch;
        }

        void branch_AppendBranch(Branch branch)
        {
            AppendBranch(branch);
        }

        private void AppendBranch(Branch currentBranch)
        {
            var currentEmbranchment = currentBranch.Embranchment;
            var currentStaffs = currentBranch.Staffs;
            currentBranch.Embranchment = new ObservableCollection<Branch>();
            currentBranch.Staffs = new ObservableCollection<Staff>();

            foreach (var staff in currentStaffs)
                this.unAllocateStaffNode.Branch.Staffs.Add(staff);

            foreach (var branch in currentEmbranchment)
            {
                this.unAllocateBranchNode.Branch.Embranchment.Add(branch);
                AppendBranch(branch);
            }
        }
    }
}
