using EquiCount.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Systems.Pool;
using UnityEngine;

namespace EquiCount.Core
{
    public enum SessionState
    {
        Unloaded,
        Initializing,
        Computing,
        UpToDate
    }

    [Serializable]
    /// <summary>
    /// Manage the computing of session datas
    /// </summary>
    public class Session
    {
        #region Accessors
        /// <summary>
        /// App owner/ Account owner
        /// </summary>
        public SessionMember localClient { get; private set; }

        /// <summary>
        /// Client members of the session
        /// </summary>
        public List<SessionMember> sessionMembers { get; private set; }

        /// <summary>
        /// The current sessions
        /// </summary>
        public SessionData sessionData { get; private set; }

       
        /// <summary>
        /// All transactions of the sessions
        /// </summary>
        public List<TransactionData> transactions { get { return sessionData.transactionsData; } private set { sessionData.transactionsData = value; } }

        /// <summary>
        /// Balance between owner and other members
        /// </summary>
        public List<(SessionMember, SessionMember, float)> allCouplesBalances { get; private set; }

        /// <summary>
        /// Total amount of expenses over the session
        /// </summary>
        public float expensesTotal { get; private set; }

        /// <summary>
        /// Total amount of provisions over the session
        /// </summary>
        public float provisionsTotal { get; private set; }

        public TransactionData currentRebalance { get; private set; }

        #endregion

        #region Dependencies Injection

        private static List<IAbstractController> controllers = new List<IAbstractController>();

        public static IAbstractController InjectDependency(IAbstractController controller)
        {
            if (controllers.Exists((t) => controller.GetType() == t.GetType()))
            {
                Debug.Log("A controller of type " + controller.GetType() + " already exist as SessionManager dependency.");
            }

            OnInitializeSession += controller.OnInitializeSession;
            OnPreComputeSession += controller.OnPreComputeSession;
            OnPostComputeSession += controller.OnPostComputeSession;

            controllers.Add(controller);

            return controller;
        }

        public static void RemoveDependency(IAbstractController controller)
        {
            OnInitializeSession -= controller.OnInitializeSession;
            OnPreComputeSession -= controller.OnPreComputeSession;
            OnPostComputeSession -= controller.OnPostComputeSession;

            controllers.Remove(controller);
        }

        #endregion

        #region Events
        public static event Action OnInitializeSession;
        public static event Action OnPreComputeSession;
        public static event Action OnPostComputeSession;

        #endregion

      
        public static SessionData CreateData(string name, List<ClientData> sessionMembers)
        {
            SessionData newSession = new SessionData();
            newSession.name = name;
            newSession.ID = System.Guid.NewGuid().ToString();
            newSession.membersData = sessionMembers;
            newSession.transactionsData = new List<TransactionData>();

            return newSession;
        }

        public void Open(SessionData data, string ownerID)
        {
            OnInitializeSession?.Invoke();

            sessionData = data;

            sessionMembers = new List<SessionMember>();

            for (int i = 0; i < sessionData.membersData.Count; ++i)
            {
                SessionMember newClient = new SessionMember(sessionData.membersData[i]);
                sessionMembers.Add(newClient);
            }

            localClient = GetSessionMemberByID(ownerID);
        }

        public void Create(SessionData data, string ownerID)
        {
            OnInitializeSession?.Invoke();

            sessionData = data;

            transactions = new List<TransactionData>();
            sessionMembers = new List<SessionMember>();

            for (int i = 0; i < sessionData.membersData.Count; ++i)
            {
                SessionMember newClient = new SessionMember(sessionData.membersData[i]);
                sessionMembers.Add(newClient);
            }

            localClient = GetSessionMemberByID(ownerID);
        }
/*
        public void AddSessionMember(ClientData newClient)
        {
            sessionData.membersData.Add(newClient);

            for (int i = 0; i < sessionData.membersData.Count; ++i)
            {
                if (!sessionMembers.Exists(t => t.data.ID == newClient.ID))
                {
                    SessionMember client = new SessionMember(sessionData.membersData[i]);
                    sessionMembers.Add(client);
                }
            }
        }
*/
        public void CloseSession()
        {
            sessionData = null;
        }

        public void AddProvision(ProvisionData provisionData)
        {
            ClientData member = sessionData.membersData.Find(t => t.ID == provisionData.clientID);
            member.provisionsDatas.Add(provisionData);
        }

        public void AddTransaction(TransactionData toAdd)
        {
            transactions.Add(toAdd);
        }

        public async void RefreshSessionAsync(Action refreshCallback)
        {
            await Task.Run(() => ComputeSession());
            currentRebalance = await Task.Run<TransactionData>(() => ComputeRebalance());

            Debug.Log("End refresh session async.");

            refreshCallback?.Invoke();
        }

        public void ComputeSession()
        {
            // Reseting all temporary values
            allCouplesBalances = new List<(SessionMember, SessionMember, float)>();
            expensesTotal = 0;
            provisionsTotal = 0;

            OnPreComputeSession?.Invoke();

            // Initializng all clients (reset values from previous compute)

            for (int i = 0; i < sessionMembers.Count; ++i)
            {
                sessionMembers[i].InitClient(sessionMembers);
                provisionsTotal += sessionMembers[i].ComputeProvision(sessionData.currency);
            }

            // Finding the latest rebalances operations executed in the session
            // We don't want to compute old transactions that have been rebalanced.
            int startIndex = 0;
            for (int i = transactions.Count - 1; i >= 0; --i)
            {
                if (transactions[i].transactionType == TransactionType.Rebalance)
                {
                    startIndex = i + 1;
                    break;
                }
            }

            Debug.Log("*********************************************** Starting session computing at index " + startIndex + "*********************************** ");

            for (int i = startIndex; i < transactions.Count; ++i)
            {
                SessionMember sender = GetSessionMemberByID(transactions[i].senderID);
                float convertedAmount = CurrencyHandler.Convert(sessionData.currency, transactions[i].currency, transactions[i].amount);

                if (transactions[i].transactionType == TransactionType.Expense)
                {
                    expensesTotal += convertedAmount;

                    sender.totalEngagedExpense += convertedAmount; //buyerWeightInSpent *
                    TransactionData data = transactions[i];
                    List<SessionMember> targets = GetClients(data.payorsIDs);

                    float totalProvision = 0;// buyer.Provision;
                    for (int j = 0; j < targets.Count; ++j)
                    {
                        totalProvision += targets[j].Provision;
                    }

                    data.weigthsInTransaction = new Dictionary<string, float>();

                    for (int j = 0; j < targets.Count; ++j)
                    {
                        /*if (targets[j] != buyer)
                        {*/
                        float targetWeightInSpent = targets[j].Provision / totalProvision;
                        data.weigthsInTransaction.Add(targets[j].data.ID, targetWeightInSpent);

                        if (targets[j] != sender)
                        {
                            Debug.Log(targets[j].data.name + " doit " + convertedAmount * targetWeightInSpent + " à " + sender.data.name + " pour sa dépense de " + transactions[i].title + " d'un montant de " + transactions[i].amount);

                        }
                        else
                        {
                            Debug.Log(targets[j].data.name + " paye " + convertedAmount * targetWeightInSpent
                                                   + " pour sa dépense de " + transactions[i].title + " d'un montant de " + transactions[i].amount);
                        }

                        targets[j].debts[sender] += targetWeightInSpent * convertedAmount;
                        //}                       
                    }
                }
                else if (transactions[i].transactionType == TransactionType.Payback)
                {
                    ExecutePayback(transactions[i]);
                }
                else if (transactions[i].transactionType == TransactionType.Rebalance)
                {
                    ExecuteRebalance(transactions[i]);
                }

                sender.emittedTransactions.Add(transactions[i]);
            }

            for (int i = 0; i < sessionMembers.Count; ++i)
            {
                foreach (KeyValuePair<SessionMember, float> balanceVP in sessionMembers[i].debts.ToList())
                {
                    if (balanceVP.Key != sessionMembers[i] && !BalanceExist(balanceVP.Key, sessionMembers[i]))
                    {
                        float compFinalBal = balanceVP.Value - balanceVP.Key.debts[sessionMembers[i]];

                        // Rebalancing client to client debt 
                        // Basically ... If A owe to B et B owe to A
                        if (compFinalBal >= 0)
                        {
                            sessionMembers[i].debts[balanceVP.Key] = compFinalBal;
                            balanceVP.Key.debts[sessionMembers[i]] = 0;
                        }
                        else
                        {
                            balanceVP.Key.debts[sessionMembers[i]] = Math.Abs(compFinalBal);
                            sessionMembers[i].debts[balanceVP.Key] = 0;
                        }

                        // Adding the couple balance to a list as we can access it from some UI later and see all client-client balances
                        (SessionMember, SessionMember, float) newBalance = (balanceVP.Key, sessionMembers[i], compFinalBal);
                        allCouplesBalances.Add(newBalance);

                        Debug.Log($"{balanceVP.Key.data.name} ({balanceVP.Key.Provision}, {balanceVP.Key.totalEngagedExpense}, {balanceVP.Key.debts[balanceVP.Key]}) <-->  {sessionMembers[i].data.name} ({sessionMembers[i].Provision}, {sessionMembers[i].totalEngagedExpense}, {sessionMembers[i].debts[sessionMembers[i]]}) : {compFinalBal}");
                    }
                }
            }

            // Computing Client - Group and Group - Client balances
            for (int i = 0; i < sessionMembers.Count; ++i)
            {
                // Ce que le membre doit au groupe
                sessionMembers[i].totalOweToGroup = ComputeOwedToGroup(sessionMembers[i]);

                foreach (KeyValuePair<SessionMember, float> balanceVP in sessionMembers[i].debts)
                {
                    if (balanceVP.Key != sessionMembers[i])
                    {
                        sessionMembers[i].totalOwedByGroup += balanceVP.Key.debts[sessionMembers[i]];
                        Debug.Log(sessionMembers[i].data.name + " doit " + balanceVP.Value + " à " + balanceVP.Key.data.name);
                    }
                }

                //Console.WriteLine($"{currentSessionMembers[i].data.name} => Dû par le groupe : {currentSessionMembers[i].totalOwedByGroup}, Dû au groupe {currentSessionMembers[i].totalOwedToGroup}");
                Debug.Log(" >> Group balance of " + sessionMembers[i].data.name + " " + sessionMembers[i].groupBalance);
            }

            OnPostComputeSession?.Invoke();

            Debug.Log("********************************************************************************** ");
        }

        /// <summary>
        /// WARNING  !! This method require that the session has been updated...
        /// Computing the shortest wau to rebalance all debts.
        /// </summary>
        public TransactionData ComputeRebalance()
        {
            List<SessionMember> currentSessionCopy = new List<SessionMember>();
            for (int i = 0; i < sessionMembers.Count; ++i)
            {
                currentSessionCopy.Add(sessionMembers[i].Clone());
            }

            List<SessionMember> owedToGroup = currentSessionCopy.Where((client) => client.groupBalance < 0).ToList();
            List<SessionMember> owedByGroup = currentSessionCopy.Where((client) => client.groupBalance > 0).ToList();

            owedToGroup.Sort((a, b) => a.groupBalance.CompareTo(b.groupBalance));
            owedByGroup.Sort((a, b) => a.groupBalance.CompareTo(b.groupBalance));
            owedByGroup.Reverse();

            Debug.Log("Owe To Group : ");
            owedToGroup.ForEach((client) => { Console.WriteLine(client.data.name + " " + client.groupBalance); });
            Debug.Log("Owed By Group : ");
            owedByGroup.ForEach((client) => { Console.WriteLine(client.data.name + " " + client.groupBalance); });

            List<TransactionData> paybackOrders = new List<TransactionData>();

            for (int i = 0; i < owedToGroup.Count; ++i)
            {
                int index = 0;
                while (owedToGroup[i].groupBalance < -.001f)
                {
                    // Si on doit de l'argent à ce membre
                    if (owedByGroup[index].groupBalance > 0)
                    {
                        float payBackToIndex = Math.Abs(owedToGroup[i].groupBalance) <= owedByGroup[index].groupBalance ? Math.Abs(owedToGroup[i].groupBalance) : owedByGroup[index].groupBalance;

                        paybackOrders.Add(CreatePaybackOrder(owedToGroup[i].data.ID, owedByGroup[index].data.ID, payBackToIndex, sessionData.currency));

                        // Ce que le membre doit au groupe diminue
                        owedToGroup[i].totalOweToGroup -= payBackToIndex;
                        // Ce que le groupe doit au membre diminue
                        owedByGroup[index].totalOwedByGroup -= payBackToIndex;

                        index++;
                    }
                    else
                    {
                        index++;
                    }

                }
            }

            Debug.Log("Résumé de la session si les remboursements sont effectués ...");
            currentSessionCopy.ForEach((client) => { Debug.Log("Group balance of : " + client.data.name + " => " + client.groupBalance); });

            return Factory<TransactionData>.GetItem().InitRebalance(System.Guid.NewGuid().ToString(), localClient.data.ID, paybackOrders);
        }

        public void DebugSimplifiedDebt(bool recompute = false)
        {
            Debug.Log("************** Dette simplifiée **************");

            TransactionData data = currentRebalance;
            if(recompute)
                data = ComputeRebalance();

            for (int i = 0; i < data.paybacksSequence.Count; ++i)
            {
                Debug.Log(data.paybacksSequence[i].senderID + " owe " + data.paybacksSequence[i].amount + data.paybacksSequence[i].currency + " to " + data.paybacksSequence[i].beneficiaryID);
            }
            Debug.Log("*********************************************");
        }

        private void ExecuteRebalance(TransactionData data)
        {
            Debug.Log("Executing Rebalance...");

            for (int i = 0; i < data.paybacksSequence.Count; ++i)
            {
                ExecutePayback(data.paybacksSequence[i]);
            }

            // Reseting all debts to 0 after rebalance.
            for (int i = 0; i < sessionMembers.Count; ++i)
            {
                for (int j = 0; j < sessionMembers.Count; ++j)
                {
                    sessionMembers[i].debts[sessionMembers[j]] = 0;
                }
            }
        }

        private void ExecutePayback(TransactionData data)
        {
            SessionMember beneficiary = GetSessionMemberByID(data.beneficiaryID);
            SessionMember sender = GetSessionMemberByID(data.senderID);
            float convertedAmount = CurrencyHandler.Convert(sessionData.currency, data.currency, data.amount);

            float diff = sender.debts[beneficiary] - convertedAmount;
            if (diff < 0)
            {
                sender.debts[beneficiary] = 0;
                beneficiary.debts[sender] = Math.Abs(diff);
            }
            else
            {
                float debt = sender.debts[beneficiary];
                sender.debts[beneficiary] -= convertedAmount;
            }

            Debug.Log($"{sender.data.name} rembourse {data.amount} à {beneficiary.data.name} ({data.title})");
        }

        public float ComputeOwedToGroup(SessionMember client)
        {
            float result = 0;

            foreach (KeyValuePair<SessionMember, float> debtsValuePair in client.debts)
            {
                if (debtsValuePair.Key != client)
                {
                    result += debtsValuePair.Value;
                }
            }
            return result;
        }

        public TransactionData CreatePaybackOrder(string senderID, string beneficiaryID, float amount, Currency currency)
        {
            Debug.Log($"Creating payback order from {senderID} to {beneficiaryID}. Amount : {amount}{currency.ToString()} ");
            return Factory<TransactionData>.GetItem().InitPayback(System.Guid.NewGuid().ToString(), senderID, beneficiaryID, amount, currency, DateTime.Now);
        }

        private bool BalanceExist(SessionMember a, SessionMember b)
        {
            for (int i = 0; i < allCouplesBalances.Count; ++i)
            {
                if (allCouplesBalances[i].Item1 == a && allCouplesBalances[i].Item2 == b
                    || allCouplesBalances[i].Item2 == a && allCouplesBalances[i].Item1 == b)
                {
                    return true;
                }
            }
            return false;
        }

        public SessionMember GetSessionMemberByID(string clientID)
        {
            return sessionMembers.Find(t => t.data.ID == clientID);
        }

        public List<SessionMember> GetClients(List<string> clientIDS)
        {
            List<SessionMember> result = new List<SessionMember>();
            for (int i = 0; i < clientIDS.Count; ++i)
            {
                result.Add(GetSessionMemberByID(clientIDS[i]));
            }

            return result;
        }

        public List<TransactionData> GetDebtsToClient(string fromClientID, string toClientID)
        {
            Debug.Log("Get All debts from " + fromClientID + " to " + toClientID);

            var debtsFrom_To = transactions.Where((transaction) =>
            transaction != null
            && transaction.transactionType == TransactionType.Expense
            && transaction.senderID == toClientID
            && transaction.payorsIDs.Contains(fromClientID));

            return debtsFrom_To.ToList();
        }

        public List<TransactionData> GetAllDebts(string fromClientID)
        {
            Debug.Log("Get All debts of " + fromClientID);

            var debtsFrom_To = transactions.Where((transaction) =>
                        transaction != null
                        && transaction.transactionType == TransactionType.Expense
                        && transaction.payorsIDs.Contains(fromClientID));

            return debtsFrom_To.ToList();
        }
    }
}
