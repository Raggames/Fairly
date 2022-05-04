using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquiCount.Core
{
    public interface IAbstractController
    {
        public abstract void OnInitializeSession();
        public abstract void OnPreComputeSession();
        public abstract void OnPostComputeSession();
    }
}
