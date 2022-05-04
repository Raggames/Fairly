using EquiCount.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EquiCount
{
    public class BudgetController : IAbstractController
    {
        public Dictionary<Category, BudgetInfo> budgetsInfo { get; private set; }

        public static event Action<Category> OnBudgetExceeded;
        public Session Session;

        public BudgetController()
        {
            budgetsInfo = new Dictionary<Category, BudgetInfo>();
        }

        public void AllocateBudget(Category category, int amount)
        {
            if (!budgetsInfo.ContainsKey(category))
            {
                BudgetInfo info = new BudgetInfo(category, amount);
                budgetsInfo.Add(category, info);
            }
            else
            {
                budgetsInfo[category].allocatedAmount += amount;
            }
        }

        private void ComputeBudgetLevelForCategory(Category category)
        {
            float expenses = GetExpensesForCategory(category);

            if (budgetsInfo[category].allocatedAmount == 0)
            {
                budgetsInfo[category].expenses = expenses;
                budgetsInfo[category].purcentage = 0;
            }

            float purcentage = expenses / budgetsInfo[category].allocatedAmount * 100f;

            budgetsInfo[category].purcentage = purcentage;
            budgetsInfo[category].expenses = expenses;

            Debug.Log($"Budget level for {category} is {purcentage} %, with {expenses} expensed over {budgetsInfo[category].allocatedAmount} allocated budget.");
        }

        private float GetExpensesForCategory(Category category)
        {
            float expensesAmount = 0;
            List<TransactionData> tData = budgetsInfo[category].expensesData;
            for (int i = 0; i < tData.Count; ++i)
            {
                expensesAmount += tData[i].amount;
            }
            return expensesAmount;
        }

        /// <summary>
        /// Get purcentage of all expense over all provisions
        /// </summary>
        /// <returns></returns>
        public (float, float, float) GetOverallBudgetLevel(bool recompute = false)
        {
            Debug.Log("*********************** OVERALL BUDGET LEVEL **************************");
            
            if (recompute)
                Session.ComputeSession();

            Debug.Log(Session.expensesTotal / Session.provisionsTotal * 100f + " " + Session.expensesTotal + " " + Session.provisionsTotal);

            return (Session.expensesTotal / Session.provisionsTotal * 100f, Session.expensesTotal, Session.provisionsTotal);
        }

        public void OnInitializeSession()
        {
            budgetsInfo = new Dictionary<Category, BudgetInfo>();
        }

        public void OnPreComputeSession()
        {
            foreach (KeyValuePair<Category, BudgetInfo> budgetVP in budgetsInfo)
            {
                budgetVP.Value.Reset();
            }
        }

        public void OnPostComputeSession()
        {
            List<TransactionData> sessionExpenses = Session.transactions.Where(transaction => transaction.transactionType == TransactionType.Expense).ToList();
            for(int i = 0; i < sessionExpenses.Count; ++i)
            {
                if (budgetsInfo.ContainsKey(sessionExpenses[i].category))
                {
                    budgetsInfo[sessionExpenses[i].category].expensesData.Add(sessionExpenses[i]);
                }
                else
                {
                    Debug.Log($"Category {sessionExpenses[i].category} hasn't been setted for this session.");
                }
            }

            foreach(KeyValuePair<Category, BudgetInfo> budgetVP in budgetsInfo)
            {
                ComputeBudgetLevelForCategory(budgetVP.Key);

                if (budgetVP.Value.purcentage >= 100)
                {
                    OnBudgetExceeded?.Invoke(budgetVP.Key);
                }
            }
        }
    }
}
