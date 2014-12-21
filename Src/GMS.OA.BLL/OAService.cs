using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMS.OA.Contract;
using GMS.OA.DAL;
using GMS.Framework.Utility;
using System.Data.Objects;
using GMS.Framework.Contract;
using EntityFramework.Extensions;
using GMS.Core.Cache;

namespace GMS.OA.BLL
{
    public class OAService : IOAService
    {
        #region Staff CURD
        public Staff GetStaff(int id)
        {
            using (var dbContext = new OADbContext())
            {
                return dbContext.Staffs.Include("Branch").FirstOrDefault(a => a.ID == id);
            }
        }

        public IEnumerable<Staff> GetStaffList(StaffRequest request = null)
        {
            request = request ?? new StaffRequest();
            using (var dbContext = new OADbContext())
            {
                IQueryable<Staff> staffs = dbContext.Staffs.Include("Branch");

                if (!string.IsNullOrEmpty(request.Name))
                    staffs = staffs.Where(u => u.Name.Contains(request.Name));

                if (request.BranchId > 0)
                    staffs = staffs.Where(u => u.BranchId == request.BranchId);

                return staffs.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveStaff(Staff staff)
        {
            using (var dbContext = new OADbContext())
            {

                if (staff.ID > 0)
                {
                    dbContext.Update<Staff>(staff);
                }
                else
                {
                    dbContext.Insert<Staff>(staff);
                }
            }
        }

        public void DeleteStaff(List<int> ids)
        {
            using (var dbContext = new OADbContext())
            {
                dbContext.Staffs.Where(u => ids.Contains(u.ID)).Delete();
            }
        }
        #endregion


        #region Branch CURD
        public Branch GetBranch(int id)
        {
            using (var dbContext = new OADbContext())
            {
                return dbContext.Find<Branch>(id);
            }
        }

        public IEnumerable<Branch> GetBranchList(BranchRequest request = null)
        {
            request = request ?? new BranchRequest();
            using (var dbContext = new OADbContext())
            {
                IQueryable<Branch> branchs = dbContext.Branchs;

                if (!string.IsNullOrEmpty(request.Name))
                    branchs = branchs.Where(u => u.Name.Contains(request.Name));

                return branchs.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveBranch(Branch branch)
        {
            using (var dbContext = new OADbContext())
            {
                if (branch.ID > 0)
                {
                    dbContext.Update<Branch>(branch);
                }
                else
                {
                    dbContext.Insert<Branch>(branch);
                }
            }
        }

        public void DeleteBranch(List<int> ids)
        {
            using (var dbContext = new OADbContext())
            {
                dbContext.Branchs.Where(u => ids.Contains(u.ID)).Delete();
            }
        }
        #endregion

        /// <summary>
        /// 返回树形组织结构，根节点RootBranch(id=0, ParentId=0)，其下Staff和Branch为未分配的，根节点下的第一个节点为TopBranch(id=1, ParantId=0)，其他ParentId=0的归为RootBranch下
        /// </summary>
        /// <returns></returns>
        public Branch GetOrg()
        {
            using (var dbContext = new OADbContext())
            {
                var branchs = dbContext.Branchs.ToList();
                var staffs = dbContext.Staffs.ToList();

                var branch = new Branch();
                AppendBranch(branchs, staffs, branch);

                return branch;
            }
        }

        private void AppendBranch(IEnumerable<Branch> allBranch, IEnumerable<Staff> allStaff, Branch branch)
        {
            branch.Embranchment = allBranch.Where(b => b.ParentId == branch.ID).Select(b => new Branch() { ID = b.ID, Name = b.Name }).ToList();
            branch.Staffs = allStaff.Where(s => s.BranchId.Value == branch.ID).Select(b => new Staff() { ID = b.ID, Name = b.Name }).ToList();
            branch.Embranchment.ForEach(b => AppendBranch(allBranch, allStaff, b));
        }

        /// <summary>
        /// 保存树形组织结构（刷全表数据/BatchUpdate），先取出来所有的Branch,Staff的ParentId为0，然后在根据RootBranch的树形结构赋对应的值
        /// </summary>
        /// <param name="rootBranch"></param>
        public void SaveOrg(Branch rootBranch)
        {
            using (var dbContext = new OADbContext())
            {
                var branchs = dbContext.Branchs.ToList();
                branchs.ForEach(b => b.ParentId = 0);

                var staffs = dbContext.Staffs.ToList();
                staffs.ForEach(s => s.BranchId = 0);

                UpdateOrg(branchs, staffs, rootBranch);

                dbContext.SaveChanges();
            }
        }

        private void UpdateOrg(List<Branch> allBranch, List<Staff> allStaff, Branch currentBranch)
        {
            if (currentBranch.Staffs != null)
            {
                foreach (var staff in currentBranch.Staffs)
                {
                    var existStaff = allStaff.SingleOrDefault(s => s.ID == staff.ID);
                    if (existStaff != null)
                        existStaff.BranchId = currentBranch.ID;
                }
            }

            if (currentBranch.Embranchment != null)
            {
                foreach (var branch in currentBranch.Embranchment)
                {
                    var existBranch = allBranch.SingleOrDefault(b => b.ID == branch.ID);
                    if (existBranch != null)
                        existBranch.ParentId = currentBranch.ID;

                    UpdateOrg(allBranch, allStaff, branch);
                }
            }
        }
    }
}
