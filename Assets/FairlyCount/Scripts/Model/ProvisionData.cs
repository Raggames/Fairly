using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquiCount
{
    /// <summary>
    /// External provisions to a Client, could be an Income
    /// </summary>
    public class ProvisionData : IData
    {
        public string ID { get; set ; }

        public string clientID;
        public float amount;
        public Currency currency;
        public string comment;

        public ProvisionData() { }

        public ProvisionData Init(string id, string clientID, float amount, Currency currency, string comment)
        {
            this.ID = id;
            this.clientID = clientID;
            this.amount = amount;
            this.currency = currency;
            this.comment = comment;

            return this;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
