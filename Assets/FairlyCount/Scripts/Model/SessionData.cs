using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquiCount
{
    [Serializable]
    public class SessionData : IData
    {
        public string ID { get; set; }
        public string name;

        public Currency currency;

        public List<ClientData> membersData;

        public List<TransactionData> transactionsData;

        public SessionData() { }

        public SessionData Init(string id, string name, Currency currency, List<ClientData> clientDatas)
        {
            this.ID = id;
            this.name = name;
            this.currency = currency;
            this.membersData = clientDatas;
            return this;
        }

        public void Clear()
        {
            ID = "";
            name = "";
            currency = Currency.EUR;
            membersData.Clear();
            transactionsData.Clear();
        }
    }
}
