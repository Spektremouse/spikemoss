using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spikemoss.Models
{
    interface IDomainEntity
    {
        int ID { get; set; }
        string Table { get; }
    }
}
