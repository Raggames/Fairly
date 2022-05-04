using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquiCount
{
    public class BudgetInfo
    {
        public Category category;
        public Currency currency;

        public int allocatedAmount { get; set; }

        public List<TransactionData> expensesData { get; set; }

        public float purcentage { get; set; }

        public float expenses { get; set; }

        public BudgetInfo(Category category, int amount)
        {
            this.category = category;
            this.allocatedAmount = amount;
            this.expensesData = new List<TransactionData>();
        }             
        
        public void Reset()
        {
            purcentage = 0;
            expenses = 0;
            expensesData.Clear();
        }
    }
}
