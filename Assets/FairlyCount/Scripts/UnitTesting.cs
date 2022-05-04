using EquiCount.Core;
using FairlyCount.View;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EquiCount
{
    public class UnitTesting : MonoBehaviour
    {
        public static BudgetController budgetControllerDependency;

        public void Start()
        {
            /*ClientData enzo = new ClientData();
            enzo.ID = "NZO";
            enzo.name = "Enzo";
            ClientData nina = new ClientData();
            nina.ID = "NNA";
            nina.name = "Nina";
            *//*ClientData wawa = new ClientData("Wawa");
            wawa.ID = "WWA";
            ClientData bruce = new ClientData("Bruce");
            bruce.ID = "BRU";
*//*
            ProvisionData enzoIncome = new ProvisionData()
            {
                amount = 2014,
                clientID = enzo.ID,
                comment = "Salaire d'Enzo",
            };

            ProvisionData ninaIncome = new ProvisionData()
            {
                amount = 1277.3f,
                clientID = nina.ID,
                comment = "Salaire de Nina",
            };

            enzo.provisionsDatas.Add(enzoIncome);
            nina.provisionsDatas.Add(ninaIncome);
         
            ExpenseData spent1 = new ExpenseData()
            {
                amount = 670,
                senderID = enzo.ID,
                payorsIDs = new System.Collections.Generic.List<string>() { enzo.ID,  nina.ID },
                currency = Currency.EUR,
                comment = "Loyer",
                category = Category.Loyer,
            };

            ExpenseData spent2 = new ExpenseData()
            {
                amount = 83,
                senderID = enzo.ID,
                payorsIDs = new System.Collections.Generic.List<string>() { enzo.ID, nina.ID },
                currency = Currency.EUR,
                comment = "Elec",
                category = Category.Factures,
            };

            ExpenseData spent3 = new ExpenseData()
            {
                amount = 200,
                senderID = nina.ID,
                payorsIDs = new System.Collections.Generic.List<string>() { enzo.ID, nina.ID },
                currency = Currency.EUR,
                comment = "Courses",
                category = Category.BCC,
            };

            SessionManager.OpenSession(
                SessionManager.CreateSession("",
                    new System.Collections.Generic.List<ClientData>()
                    { enzo, nina }),
                enzo.ID);

            budgetControllerDependency = SessionManager.InjectDependency(new BudgetController()) as BudgetController;

            SessionManager.AddTransaction(spent1);
            SessionManager.AddTransaction(spent2);
            SessionManager.AddTransaction(spent3);
        
            budgetControllerDependency.AllocateBudget(Category.Loyer, 670);
            budgetControllerDependency.AllocateBudget(Category.Factures, 250);
            budgetControllerDependency.AllocateBudget(Category.BCC, 400);
            budgetControllerDependency.AllocateBudget(Category.Loisir, 300);

            BudgetController.OnBudgetExceeded += BudgetController_OnBudgetExceeded;

            SessionManager.RefreshSessionAsync(() => OnEndRefresh());    */


            Controller controller = FindObjectOfType<Controller>();

            controller.CreateLocalUser("Enzo", "localUserID");


            ClientData enzoTest = controller.CreateClient(controller.localUser.username, controller.localUser.ID);
            ClientData ninaTest = controller.CreateClient("Nina", "ninatestID12345");

            List<ClientData> clients = controller.CreateSession("testSession2", enzoTest, ninaTest);

            if (clients != null)
            {
                enzoTest = clients[0];
                ninaTest = clients[1];

                controller.AddExpense(enzoTest.ID, new List<string>() { ninaTest.ID, enzoTest.ID }, 670, "Loyer", Currency.EUR, Category.None);
                controller.AddExpense(enzoTest.ID, new List<string>() { ninaTest.ID, enzoTest.ID }, 83, "Electricité", Currency.EUR, Category.None);
                controller.AddExpense(ninaTest.ID, new List<string>() { ninaTest.ID, enzoTest.ID }, 215.30f, "Courses1", Currency.EUR, Category.None);
                controller.AddExpense(ninaTest.ID, new List<string>() { ninaTest.ID, enzoTest.ID }, 175.25f, "Courses2", Currency.EUR, Category.None);
                controller.AddExpense(enzoTest.ID, new List<string>() { ninaTest.ID, enzoTest.ID }, 248.47f, "Courses3", Currency.EUR, Category.None);
                //controller.AddPayback(enzoTest.ID, ninaTest.ID, 50, Currency.EUR);

                controller.AddProvision(enzoTest.ID, "SALAIRE", 1814, Currency.EUR);
                controller.AddProvision(ninaTest.ID, "SALAIRE", 1200, Currency.EUR);

                controller.Save();

                controller.RefreshCurrentSession(OnRefreshedCallback);
            }
            else
            {
                controller.OpenLoadedSession(controller.loadedDatas[0]);
                controller.RefreshCurrentSession(OnRefreshedCallback);
            }
        }

        private void OnRefreshedCallback()
        {
            Debug.Log("Session refreshed !");

            //AddRebalance();
            Controller.Instance.CurrentSession.DebugSimplifiedDebt();

            Controller.Instance.CurrentSession.GetAllDebts(Controller.Instance.localUser.ID).ForEach((debt) =>
            {
                if (debt != null && debt.senderID != Controller.Instance.localUser.ID)
                {
                    Debug.Log("Amount : " + debt.weigthsInTransaction[Controller.Instance.localUser.ID] * debt.amount + " owed to " + Controller.Instance.GetClientFromID(debt.senderID).name + " Reason : " + debt.title);
                }
            });

            Controller.Instance.mainMenuWindow.Close();

            // Pour raffraichir on clean tout avec un close d'abord
            Controller.Instance.currentSessionWindow.Close();
            // Puis on ouvre la fenêtre et on init
            Controller.Instance.currentSessionWindow.Open();
            Controller.Instance.currentSessionWindow.Refresh(Controller.Instance.CurrentSession);
        }

        /*  private static void BudgetController_OnBudgetExceeded(Category obj)
          {
              Debug.Log("!!!!!! Budget exceeded !!!!!!!=> " + obj.ToString() + " ************************************ ");
          }

          private static void OnEndRefresh()
          {
              budgetControllerDependency.GetOverallBudgetLevel();

              Session.DebugSimplifiedDebt();

              Session.GetAllDebts("NNA").ForEach((debt) => Debug.Log("Amount : " + debt.weigthsInTransaction["NNA"] * debt.amount +  " owed to " + debt.senderID + " Reason : " + debt.comment));
              Session.GetDebtsToClient("NNA", "NZO").ForEach((debt) => Debug.Log("Amount : " + debt.weigthsInTransaction["NNA"] * debt.amount + " Reason : " + debt.comment));


              Session.AddTransaction(Session.currentRebalance);

              Session.ComputeSession();
          }*/
    }
}
