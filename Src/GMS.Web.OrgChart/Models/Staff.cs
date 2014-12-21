using System;
using Newtonsoft.Json;

namespace GMS.Web.OrgChart.Models
{
    public class Staff : ModelBase
    {
        public int ID { get; set; }
        
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private Branch parentBranch;
        [JsonIgnore]
        public Branch ParentBranch
        {
            get { return parentBranch; }
            set
            {
                parentBranch = value;
                NotifyPropertyChanged("ParentBranch");
            }
        }
    }
}
