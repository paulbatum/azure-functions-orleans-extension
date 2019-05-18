using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class OrleansStartupTriggerAttribute : Attribute
    {
        [AppSetting]
        public string ClusterId { get; set; }

        [AppSetting]
        public string ServiceId { get; set; }

        [AppSetting]
        public string PersistenceConnectionStringSetting { get; set; }

        public Type GrainType { get; set; }

        public OrleansStartupTriggerAttribute()
        {

        }
    }
}
