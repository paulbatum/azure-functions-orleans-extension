using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    [Binding]
    public class OrleansAttribute : Attribute
    {
        [AppSetting]
        public string ClusterId { get; set; }

        [AppSetting]
        public string ServiceId { get; set; }

        public Type GrainInterface { get; set; }

        public OrleansAttribute()
        {
            
        }
    }
}
