using System;
using System.Collections.Generic;

namespace GMS.OA.Contract
{
    public interface IOAService
    {
        Staff GetStaff(int id);
        IEnumerable<Staff> GetStaffList(StaffRequest request = null);
        void SaveStaff(Staff staff);
        void DeleteStaff(List<int> ids);

        Branch GetBranch(int id);
        IEnumerable<Branch> GetBranchList(BranchRequest request = null);
        void SaveBranch(Branch branch);
        void DeleteBranch(List<int> ids);

        Branch GetOrg();
        void SaveOrg(Branch rootBranch);
    }
}
