using EquiCount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairlyCount.Model
{
    [Serializable]
    public class Save
    {
        public List<SessionData> sessionsDatas = new List<SessionData>();

        public Save(List<SessionData> data)
        {
            this.sessionsDatas = data;
        }
    }
}
