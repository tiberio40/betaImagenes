using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSHttpClientSample.json
{
    class Person
    {
        public Description description { get; set; }
        public string requestId { get; set; }
        public Metadata metadata { get; set; }
    }
}
