using EquiCount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FairlyCount.View
{
    public class UIExpense : UITransaction
    {
        [Header("Data")]
        public TransactionData data;

        [Header("Members")]
        public TextMeshProUGUI title_Text;
        public TextMeshProUGUI sender_Text;
        public TextMeshProUGUI value_Text;
        public TextMeshProUGUI currency_Text;
        public TextMeshProUGUI date_Text;
        public TextMeshProUGUI ratio_Text;

        public void Init(TransactionData data)
        {
            title_Text.text = data.title;
            sender_Text.text = Controller.Instance.GetClientFromID(data.senderID).name;
            value_Text.text = data.amount.ToString();
            currency_Text.text = "€";
            date_Text.text = data.dateTime.ToString();

            string rat = "";
            foreach(KeyValuePair<string, float> weight in data.weigthsInTransaction)
            {
                double value = Math.Round(weight.Value * data.amount, 2);
                rat += Controller.Instance.GetClientFromID(weight.Key).name + " : " + value.ToString() + " " + data.currency.ToString() + " ";
            }
            
            ratio_Text.text = rat;
        }
    }
}
