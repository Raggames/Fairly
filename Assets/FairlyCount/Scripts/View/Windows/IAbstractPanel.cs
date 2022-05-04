using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairlyCount.View
{
    public interface IAbstractPanel : IAbstractWindow
    {
        public IAbstractWindow masterWindow { get; set; }
    }
}
