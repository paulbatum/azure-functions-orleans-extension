using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{    
    [Binding]
    public class OrleansActorTriggerAttribute : Attribute
    {
        public string GrainType { get; set; }
    }
}
