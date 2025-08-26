using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public abstract class BaseContract
    {
        public string RequestType { get; set; } = "";
        public string ReplyQueue { get; set; } = "";
    }
}
