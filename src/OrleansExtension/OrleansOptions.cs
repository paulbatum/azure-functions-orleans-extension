using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    public class OrleansOptions
    {
        public string StorageProviderName { get; set; }
        public string StorageProviderSettingName { get; set; }      
    }
}
