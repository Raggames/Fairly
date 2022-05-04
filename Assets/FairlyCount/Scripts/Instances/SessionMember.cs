using EquiCount.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquiCount.Instances
{
    public class SessionMember
    {
        public ClientData data { get; private set; }

        private List<ProvisionData> provisions;

        public float Provision { get; set; }

        /// <summary>
        /// Represent what THIS client owe to Key clients
        /// </summary>
        public Dictionary<SessionMember, float> debts = new Dictionary<SessionMember, float>();

        /// <summary>
        /// Expenses engaged by the client over the session
        /// </summary>
        public List<TransactionData> emittedTransactions = new List<TransactionData>();
        /// <summary>
        /// Montant de tous les frais engagés et avancés par le client
        /// </summary>
        public float totalEngagedExpense = 0;

        /// <summary>
        /// Montant de tous les frais réels dûs par le client 
        /// /// </summary>
        public float totalReelExpense = 0;

        /// <summary>
        /// Amount that the client owe to the group members (undifferenced)
        /// </summary>
        public float totalOweToGroup = 0;
        /// <summary>
        /// Amount that all the group owe to the client
        /// </summary>
        public float totalOwedByGroup = 0;
        
        public float groupBalance
        {
            get
            {
                return totalOwedByGroup - totalOweToGroup;
            }
        }

        public SessionMember(ClientData data)
        {
            this.data = data;

            if(this.data.provisionsDatas != null)
            {
                provisions = this.data.provisionsDatas;
            }
            else
            {
                provisions = new List<ProvisionData>();
            }
        }

        public SessionMember Clone()
        {
            SessionMember clone = new SessionMember(this.data);

            clone.debts = this.debts;
            clone.emittedTransactions = this.emittedTransactions;
            clone.totalEngagedExpense = this.totalEngagedExpense;
            clone.totalOwedByGroup = this.totalOwedByGroup;
            clone.totalOweToGroup = this.totalOweToGroup;
            clone.Provision = this.Provision;

            return clone;
        }

        public void InitClient(List<SessionMember> sessionMembers)
        {
            debts.Clear();
            emittedTransactions.Clear();

            totalEngagedExpense = 0;
            totalReelExpense = 0;
            totalOwedByGroup = 0;
            totalOweToGroup = 0;

            for(int i = 0; i < sessionMembers.Count; ++i)
            {
                // We don't add self client to balances dictionnary
                //if (sessionMembers[i].data.ID != data.ID)
                //{
                    debts.Add(sessionMembers[i], 0);
                //}
            }
        }

        public float ComputeProvision(Currency sessionCurrency)
        {
            Provision = 0;
            for(int i = 0; i < provisions.Count; ++i)
            {
                Provision += CurrencyHandler.Convert(sessionCurrency, provisions[i].currency, provisions[i].amount);
            }

            return Provision;
        }        
    }
}
