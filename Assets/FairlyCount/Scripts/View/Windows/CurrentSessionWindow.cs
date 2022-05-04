using EquiCount;
using EquiCount.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FairlyCount.View
{
    public class CurrentSessionWindow : MonoBehaviour, IAbstractWindow
    {
        private Session session;

        [Header("Upper Menu Content")]
        public TextMeshProUGUI session_NameText;
        public TextMeshProUGUI localUser_NameText;
        public TextMeshProUGUI partnerUser_NameText;
        public TextMeshProUGUI localUser_ProvisionText;
        public TextMeshProUGUI partnerUser_ProvisionText;

        [Header("Transactions Content")]
        public Transform transactionsContent;

        public UIExpense pf_UiExpense;
        public UIPayback pf_UiPayback;
        public List<UITransaction> uiTransactions = new List<UITransaction>();

        [Header("Down Menu Content")]
        public TextMeshProUGUI localUser_BalanceText;
        public TextMeshProUGUI partnerUser_BalanceText;


        public void Open()
        {
            this.gameObject.SetActive(true);
        }

        public void Close()
        {
            this.gameObject.SetActive(false);

            for(int i = 0; i < uiTransactions.Count; ++i)
            {
                ObjectPool.Instance.DespawnGo(uiTransactions[i].gameObject);
            }
        }

        public void Refresh(Session session)
        {
            this.session = session;

            session_NameText.text = session.sessionData.name;

            localUser_NameText.text = session.sessionData.membersData[0].name;
            partnerUser_NameText.text = session.sessionData.membersData[1].name;
            localUser_ProvisionText.text = session.sessionMembers[0].Provision.ToString();
            partnerUser_ProvisionText.text = session.sessionMembers[1].Provision.ToString();

            localUser_BalanceText.text = session.sessionMembers[0].groupBalance.ToString();

            if (session.sessionMembers[0].groupBalance < 0)
                localUser_BalanceText.color = Color.red;
            else
                localUser_BalanceText.color = Color.green;

            partnerUser_BalanceText.text = session.sessionMembers[1].groupBalance.ToString();

            if (session.sessionMembers[1].groupBalance < 0)
                partnerUser_BalanceText.color = Color.red;
            else
                partnerUser_BalanceText.color = Color.green;

            for (int i = 0; i < session.sessionData.transactionsData.Count; ++i)
            {
                switch (session.sessionData.transactionsData[i].transactionType)
                {
                    case TransactionType.Expense:
                        UIExpense expense = ObjectPool.Instance.SpawnGo(pf_UiExpense.gameObject, Vector3.zero, transactionsContent).GetComponent<UIExpense>();
                        expense.Init(session.sessionData.transactionsData[i]);
                        break;
                    case TransactionType.Payback:
                        UIPayback payback = ObjectPool.Instance.SpawnGo(pf_UiPayback.gameObject, Vector3.zero, transactionsContent).GetComponent<UIPayback>();
                        payback.Init(session.sessionData.transactionsData[i]);
                        break;
                    case TransactionType.Rebalance:
                        break;
                }
            }
        }
    }
}
