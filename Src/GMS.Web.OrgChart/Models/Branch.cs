using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace GMS.Web.OrgChart.Models
{
    public class Branch : ModelBase
    {
        public Branch()
        {
            this.Embranchment = new ObservableCollection<Branch>();
            this.Staffs = new ObservableCollection<Staff>();
        }

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

        private ObservableCollection<Branch> embranchment;
        public ObservableCollection<Branch> Embranchment
        {
            get { return embranchment; }
            set
            {
                embranchment = value;
                this.UpdateEmbranchment();
                embranchment.CollectionChanged += (s, e) => UpdateEmbranchment();
                NotifyPropertyChanged("Embranchment");
            }
        }

        private ObservableCollection<Staff> staffs;
        public ObservableCollection<Staff> Staffs
        {
            get { return staffs; }
            set
            {
                staffs = value;
                this.UpdateStaff();
                staffs.CollectionChanged += (s, e) => UpdateStaff();
                NotifyPropertyChanged("Staffs");
            }
        }

        private Point lineRenderTransformOrigin;
        [JsonIgnore]
        public Point LineRenderTransformOrigin
        {
            get { return lineRenderTransformOrigin; }
            set
            {
                lineRenderTransformOrigin = value;
                NotifyPropertyChanged("LineRenderTransformOrigin");
            }
        }

        private Transform lineRenderTransform;
        [JsonIgnore]
        public Transform LineRenderTransform
        {
            get { return lineRenderTransform; }
            set
            {
                lineRenderTransform = value;
                NotifyPropertyChanged("LineRenderTransform");
            }
        }

        private Visibility lineVisibility = Visibility.Collapsed;
        [JsonIgnore]
        public Visibility LineVisibility
        {
            get { return lineVisibility; }
            set
            {
                lineVisibility = value;
                NotifyPropertyChanged("LineVisibility");
            }
        }

        private bool enableAppendBranch = true;
        [JsonIgnore]
        public bool EnableAppendBranch
        {
            get { return enableAppendBranch; }
            set
            {
                enableAppendBranch = value;
                NotifyPropertyChanged("EnableAppendBranch");
            }
        }

        private bool enableAppendStaff = true;
        [JsonIgnore]
        public bool EnableAppendStaff
        {
            get { return enableAppendStaff; }
            set
            {
                enableAppendStaff = value;
                NotifyPropertyChanged("EnableAppendStaff");
            }
        }

        public event Action<Branch> AppendBranch;

        public void OnAppendBranch(Branch branch)
        {
            if (this.AppendBranch != null)
                this.AppendBranch(branch);
        }

        private void UpdateStaff()
        {
            foreach (var staff in this.Staffs)
                staff.ParentBranch = this;
        }

        private void UpdateEmbranchment()
        {   
            if (embranchment.Count == 0)
            {
                LineVisibility = Visibility.Collapsed;
                return;
            }
            else
            {
                LineVisibility = Visibility.Visible;
            }

            if (embranchment.Count == 1)
            {
                embranchment[0].ParentBranch = this;
                
                var scale = new ScaleTransform();
                scale.ScaleX = 0;
                embranchment[0].LineRenderTransform = scale;
                return;
            }

            for (int i = 0; i < embranchment.Count; i++)
            {
                embranchment[i].ParentBranch = this;
                
                var scale = new ScaleTransform();
                
                if (i == 0)
                {
                    scale.ScaleX = 0.5;
                    embranchment[i].LineRenderTransformOrigin = new Point(1,0);
                }
                else if (i == embranchment.Count - 1)
                {
                    scale.ScaleX = 0.5;
                    embranchment[i].LineRenderTransformOrigin = new Point(0, 0);
                }
                else
                {
                    scale.ScaleX = 1;
                }

                embranchment[i].LineRenderTransform = scale;
            }
        }
    }
}
