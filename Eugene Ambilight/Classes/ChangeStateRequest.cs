using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eugene_Ambilight.Classes
{
    public class ChangeStateRequest
    {
        [Newtonsoft.Json.JsonProperty("state")]
        public bool? State { get; set; }
    }
}
