using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquiCount
{
    [Serializable]
    public class ClientData : IData
    {
        public string ID { get; set; }
        public string name;

        public List<ProvisionData> provisionsDatas;

        public ClientData()
        {
            
        }

        public ClientData Init(string id, string name, List<ProvisionData> provisions = null)
        {
            this.ID = id;
            this.name = name;

            if (provisions != null)
            {
                provisionsDatas = provisions;
            }
            else
            {
                provisionsDatas = new List<ProvisionData>();
            }

            return this;
        }

        public void Clear()
        {
            ID = "";
            name = "";
            provisionsDatas.Clear();
        }
    }
}
