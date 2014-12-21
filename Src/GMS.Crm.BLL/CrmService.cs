using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMS.Crm.Contract;
using GMS.Crm.DAL;
using GMS.Framework.Utility;
using System.Data.Objects;
using GMS.Framework.Contract;
using EntityFramework.Extensions;
using GMS.Core.Cache;

namespace GMS.Crm.BLL
{
    public class CrmService : ICrmService
    {
        #region Project CURD
        public Project GetProject(int id)
        {
            using (var dbContext = new CrmDbContext())
            {
                return dbContext.Find<Project>(id);
            }
        }

        public IEnumerable<Project> GetProjectList(ProjectRequest request = null)
        {
            request = request ?? new ProjectRequest();
            using (var dbContext = new CrmDbContext())
            {
                IQueryable<Project> projects = dbContext.Projects;

                if (!string.IsNullOrEmpty(request.Name))
                    projects = projects.Where(u => u.Name.Contains(request.Name));

                return projects.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveProject(Project project)
        {
            using (var dbContext = new CrmDbContext())
            {
                if (project.ID > 0)
                {
                    dbContext.Update<Project>(project);
                }
                else
                {
                    dbContext.Insert<Project>(project);
                }
            }
        }

        public void DeleteProject(List<int> ids)
        {
            using (var dbContext = new CrmDbContext())
            {
                dbContext.Projects.Where(u => ids.Contains(u.ID)).Delete();
            }
        }
        #endregion

        #region Customer CURD
        public Customer GetCustomer(int id)
        {
            using (var dbContext = new CrmDbContext())
            {
                return dbContext.Find<Customer>(id);
            }
        }

        public IEnumerable<Customer> GetCustomerList(CustomerRequest request = null)
        {
            request = request ?? new CustomerRequest();
            using (var dbContext = new CrmDbContext())
            {
                IQueryable<Customer> queryList = dbContext.Customers.Include("VisitRecords");

                if (request.Customer.UserId > 0)
                    queryList = queryList.Where(d => d.UserId == request.Customer.UserId);
                
                if (!string.IsNullOrEmpty(request.Customer.Name))
                    queryList = queryList.Where(d => d.Name.Contains(request.Customer.Name));

                if (!string.IsNullOrEmpty(request.Customer.Number))
                    queryList = queryList.Where(d => d.Number.Contains(request.Customer.Number));

                if (!string.IsNullOrEmpty(request.Customer.Username))
                    queryList = queryList.Where(d => d.Username.Contains(request.Customer.Username));

                if (!string.IsNullOrEmpty(request.Customer.Tel))
                    queryList = queryList.Where(d => d.Tel.Contains(request.Customer.Tel));

                if (request.Customer.Gender > 0)
                    queryList = queryList.Where(d => d.Gender == request.Customer.Gender);

                if (request.Customer.Category > 0)
                    queryList = queryList.Where(d => d.Category == request.Customer.Category);

                if (request.Customer.Profession > 0)
                    queryList = queryList.Where(d => d.Profession == request.Customer.Profession);

                if (request.Customer.AgeGroup > 0)
                    queryList = queryList.Where(d => d.AgeGroup == request.Customer.AgeGroup);

                return queryList.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveCustomer(Customer customer)
        {
            using (var dbContext = new CrmDbContext())
            {
                if (customer.ID > 0)
                {
                    if (dbContext.Customers.Any(c => c.Tel == customer.Tel && c.ID != customer.ID))
                        throw new BusinessException("Tel", "已存在此电话的客户！");

                    if (dbContext.Customers.Any(c => c.Number == customer.Number && c.ID != customer.ID))
                        throw new BusinessException("Number", "已存在此编号的客户！");
                    
                    dbContext.Update<Customer>(customer);
                }
                else
                {
                    if (dbContext.Customers.Any(c => c.Tel == customer.Tel))
                        throw new BusinessException("Tel", "已存在此电话的客户！");

                    if (dbContext.Customers.Any(c => c.Number == customer.Number))
                        throw new BusinessException("Number", "已存在此编号的客户！");
                    
                    dbContext.Insert<Customer>(customer);
                }
            }
        }

        public void DeleteCustomer(List<int> ids)
        {
            using (var dbContext = new CrmDbContext())
            {
                dbContext.Customers.Where(u => ids.Contains(u.ID)).Delete();
            }
        }
        #endregion

        #region VisitRecord CURD
        public VisitRecord GetVisitRecord(int id)
        {
            using (var dbContext = new CrmDbContext())
            {
                return dbContext.Find<VisitRecord>(id);
            }
        }

        public IEnumerable<VisitRecord> GetVisitRecordList(VisitRecordRequest request = null)
        {
            request = request ?? new VisitRecordRequest();
            using (var dbContext = new CrmDbContext())
            {
                IQueryable<VisitRecord> queryList = dbContext.VisitRecords.Include("Project").Include("Customer");

                var model = request.VisitRecord;

                if (!string.IsNullOrEmpty(model.Username))
                    queryList = queryList.Where(d => d.Username.Contains(model.Username));

                if (model.Customer != null && !string.IsNullOrEmpty(model.Customer.Name))
                    queryList = queryList.Where(d => d.Customer.Name.Contains(model.Customer.Name));

                if (model.Customer != null && !string.IsNullOrEmpty(model.Customer.Number))
                    queryList = queryList.Where(d => d.Customer.Number.Contains(model.Customer.Number));

                if (model.Customer != null && !string.IsNullOrEmpty(model.Customer.Tel))
                    queryList = queryList.Where(d => d.Customer.Tel.Contains(model.Customer.Tel));

                if (request.StartHour != null && request.EndHour != null)
                {
                    var startHour = request.StartHour.Value - 1;
                    var endHour = request.EndHour.Value - 1;

                    queryList = queryList.Where(d => d.VisitTime.Hour >= startHour && d.VisitTime.Hour <= endHour);
                }

                var startDate = request.StartDate == null ? DateTime.Now.AddMonths(-3) : request.StartDate.Value;
                queryList = queryList.Where(d => d.VisitTime > startDate);

                var endDate = request.EndDate == null ? DateTime.Now.AddDays(1) : request.EndDate.Value;
                queryList = queryList.Where(d => d.VisitTime < endDate);

                if (model.FollowStep > 0)
                    queryList = queryList.Where(d => d.FollowStep == model.FollowStep);

                if (model.FollowLevel > 0)
                    queryList = queryList.Where(d => d.FollowLevel == model.FollowLevel);

                if (model.ProjectId > 0)
                    queryList = queryList.Where(d => d.ProjectId == model.ProjectId);

                if (model.Motivation > 0)
                    queryList = queryList.Where(d => d.Motivation == model.Motivation);

                if (model.AreaDemand > 0)
                    queryList = queryList.Where(d => d.AreaDemand == model.AreaDemand);

                if (model.PriceResponse > 0)
                    queryList = queryList.Where(d => d.PriceResponse == model.PriceResponse);

                if (model.Focus > 0)
                    queryList = queryList.Where(d => (d.Focus & model.Focus) != 0);

                if (model.CognitiveChannel > 0)
                    queryList = queryList.Where(d => (d.CognitiveChannel & model.CognitiveChannel) != 0);

                if (model.VisitWay > 0)
                    queryList = queryList.Where(d => d.VisitWay == model.VisitWay);

                if (model.AreaId > 0)
                    queryList = queryList.Where(d => d.AreaId == model.AreaId);

                return queryList.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveVisitRecord(VisitRecord visitRecord)
        {
            using (var dbContext = new CrmDbContext())
            {
                if (visitRecord.ID > 0)
                {
                    dbContext.Update<VisitRecord>(visitRecord);
                }
                else
                {
                    dbContext.Insert<VisitRecord>(visitRecord);
                }
            }
        }

        public void DeleteVisitRecord(List<int> ids)
        {
            using (var dbContext = new CrmDbContext())
            {
                dbContext.VisitRecords.Where(u => ids.Contains(u.ID)).Delete();
            }
        }
        #endregion

        public IEnumerable<City> GetCityList(Request request = null)
        {
            request = request ?? new Request();
            using (var dbContext = new CrmDbContext())
            {
                IQueryable<City> citys = dbContext.Citys;
                return citys.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public IEnumerable<Area> GetAreaList(Request request = null)
        {
            request = request ?? new Request();
            using (var dbContext = new CrmDbContext())
            {
                IQueryable<Area> areas = dbContext.Areas;
                return areas.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public IEnumerable<UserAnalysis> GetUserAnalysis(DateTime startDate, DateTime endDate)
        {
            using (var dbContext = new CrmDbContext())
            {
                var result = dbContext.VisitRecords.Where(d => d.VisitTime > startDate && d.VisitTime < endDate).GroupBy(r => r.Username).Select(g => new UserAnalysis { UserName = g.Key, VisitRecordCount = g.Count(), CustomerCount = g.Select(c => c.CustomerId).Distinct().Count() }).ToList();
                return result;
            }
        }

        public IEnumerable<VisitStatistics> GetVisitStatistics(DateTime startDate, DateTime endDate)
        {
            using (var dbContext = new CrmDbContext())
            {
                var result = dbContext.VisitRecords.Where(d => d.VisitTime > startDate && d.VisitTime < endDate).GroupBy(r => r.VisitTime.Hour).Select(g => new VisitStatistics { Hour = g.Key, VisitRecordCount = g.Count(), VisitCount = g.Where(c => c.VisitWay == 2).Count(), TelCount = g.Where(c => c.VisitWay == 1).Count() }).ToList();
                return result;
            }
        }
    }
}
