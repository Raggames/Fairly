using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Systems.Pool;

namespace EquiCount
{
    public interface IData : IPoolable
    {
        public string ID { get; set; }
    }
}
