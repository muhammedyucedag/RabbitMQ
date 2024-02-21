using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class CreateExcelMessage
    {
        public Guid UserId { get; set; }
        public Guid FileId { get; set; }
    }
}
