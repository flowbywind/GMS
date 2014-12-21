using System;
using System.Collections.Generic;
using GMS.Framework.Contract;

namespace GMS.Crm.Contract
{
    public class ProjectRequest : Request
    {
        public string Name { get; set; }
    }

    public class CustomerRequest : Request
    {
        public CustomerRequest()
        {
            this.Customer = new Customer();
        }
        
        public Customer Customer { get; set; }
    }

    public class VisitRecordRequest : Request
    {
        public VisitRecordRequest()
        {
            this.VisitRecord = new VisitRecord();
        }

        public int? StartHour { get; set; }
        public int? EndHour { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public VisitRecord VisitRecord { get; set; }
    }

    public class UserAnalysis
    {
        public string UserName { get; set; }
        public int VisitRecordCount { get; set; }
        public int CustomerCount { get; set; }
    }

    public class VisitStatistics
    {
        public int Hour { get; set; }
        public int VisitRecordCount { get; set; }
        public int VisitCount { get; set; }
        public int TelCount { get; set; }
    }
}
