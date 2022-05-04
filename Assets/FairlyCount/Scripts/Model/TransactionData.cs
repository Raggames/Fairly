using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquiCount
{
    public enum TransactionType
    {
        Expense, 
        Payback,
        Rebalance
    }

    [Serializable]
    public class TransactionData : IData
    {
        public string ID { get; set; }

        public TransactionType transactionType;

        /// <summary>
        /// DateTime of the transaction (auto or manually)
        /// </summary>
        public DateTime dateTime;

        /// <summary>
        /// Comment of the buyer
        /// </summary>
        public string title;

        /// <summary>
        /// Client who is spending an amount for the spent
        /// </summary>
        public string senderID;

        /// <summary>
        /// The actual amount of the spent
        /// </summary>
        public float amount;

        /// <summary>
        /// The currency of transaction
        /// </summary>
        public Currency currency;

        /// <summary>
        /// See AppCore => transaction Categories
        /// </summary>
        public Category category;

        #region Expense
        /// <summary>
        /// All the clients who benefits the spent
        /// </summary>
        public List<string> payorsIDs;

        /// <summary>
        /// Weight in transaction foreach payor
        /// </summary>
        public Dictionary<string, float> weigthsInTransaction;
        #endregion

        #region Payback
        /// <summary>
        /// The target of the payback
        /// </summary>
        public string beneficiaryID;
        #endregion

        #region Rebalance
        /// <summary>
        /// The paybacks to execute to clear the debts of all members of the session
        /// </summary>
        public List<TransactionData> paybacksSequence;
        #endregion


        public TransactionData InitRebalance(string id, string senderID, List<TransactionData> paybackSequence)
        {
            this.transactionType = TransactionType.Rebalance;

            this.ID = id;
            this.senderID = senderID;
            this.paybacksSequence = paybackSequence;
            return this;
        }

        public TransactionData InitPayback(string id, string senderID, string beneficiaryID, float amount, Currency currency, DateTime dateTime)
        {
            this.transactionType = TransactionType.Payback;
            this.ID = id;
            this.senderID = senderID;
            this.beneficiaryID = beneficiaryID;
            this.amount = amount;
            this.currency = currency;
            this.dateTime = dateTime;

            return this;
        }

        public TransactionData InitExpense(string id, float amount, string title, string senderId, List<string> payorsIDs, Currency currency, Category category, DateTime dateTime)
        {
            this.transactionType = TransactionType.Expense;

            this.ID = id;
            this.amount = amount;
            this.title = title;
            this.senderID = senderId;
            this.payorsIDs = payorsIDs;
            this.currency = currency;
            this.category = category;
            this.dateTime = dateTime;
            return this;
        }

        public virtual void Clear()
        {
            ID = "";
            dateTime = DateTime.MinValue;
            title = "";
            senderID = "";
            amount = 0;
            currency = Currency.EUR;
            category = Category.None;
        }
    }
}
