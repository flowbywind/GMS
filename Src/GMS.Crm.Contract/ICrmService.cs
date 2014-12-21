using System;
using System.Collections.Generic;
using GMS.Framework.Contract;

namespace GMS.Crm.Contract
{
    public interface ICrmService
    {
        Project GetProject(int id);
        IEnumerable<Project> GetProjectList(ProjectRequest request = null);
        void SaveProject(Project project);
        void DeleteProject(List<int> ids);

        Customer GetCustomer(int id);
        IEnumerable<Customer> GetCustomerList(CustomerRequest request = null);
        void SaveCustomer(Customer customer);
        void DeleteCustomer(List<int> ids);

        VisitRecord GetVisitRecord(int id);
        IEnumerable<VisitRecord> GetVisitRecordList(VisitRecordRequest request = null);
        void SaveVisitRecord(VisitRecord visitRecord);
        void DeleteVisitRecord(List<int> ids);

        IEnumerable<City> GetCityList(Request request = null);
        IEnumerable<Area> GetAreaList(Request request = null);

        IEnumerable<UserAnalysis> GetUserAnalysis(DateTime startDate, DateTime endDate);
        IEnumerable<VisitStatistics> GetVisitStatistics(DateTime startDate, DateTime endDate);
    }
}
