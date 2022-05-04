using EquiCount;
using EquiCount.Core;
using EquiCount.Instances;
using FairlyCount.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Systems.Pool;
using TMPro;
using UnityEngine;

namespace FairlyCount.View
{
    public class Controller : Singleton<Controller>
    {
        #region Debug
        public string testusername = "Rag";

        #endregion

        #region Members

        // WINDOWS
        public MainMenuWindow mainMenuWindow;
        public CurrentSessionWindow currentSessionWindow;

        // PANELS
        public TMP_InputField sessionName;

        #endregion

        #region Accessors
        public Session CurrentSession { get; set; }
        public List<SessionData> loadedDatas { get; private set; } = new List<SessionData>();
        public User localUser { get; private set; }

        #endregion

        #region Init

        public void OnEnable()
        {
        }

        public void OnDisable()
        {
        }

        private void Awake()
        {
            mainMenuWindow.Open();
            currentSessionWindow.Close();

            Load();
            DisplaySessionsList();
        }

        public void Load()
        {
            Save save = SaveManager.Load<Save>(new Save(new List<SessionData>()), "save.frl");
            loadedDatas = save.sessionsDatas;           
        }

        public void Save()
        {
            SaveManager.Save<Save>(new Save(loadedDatas), "save.frl");
        }

        #endregion

        private void DisplaySessionsList()
        {
            for (int i = 0; i < loadedDatas.Count; ++i)
            {
                Debug.Log("Session : " + loadedDatas[i].name);
            }
        }

        public List<ClientData> OpenLoadedSession(SessionData data)
        {
            CurrentSession = new Session();
            CurrentSession.Open(data, localUser.ID);
            return data.membersData;
        }

        public List<ClientData> CreateSession(string sessionName, ClientData youData, ClientData yourPartnerData)
        {
            CurrentSession = new Session();

            if (!loadedDatas.Exists(t => t.name == sessionName))
            {
                SessionData sessionData = Session.CreateData(sessionName, new List<ClientData>() { youData, yourPartnerData });
                loadedDatas.Add(sessionData);

                CurrentSession.Create(sessionData, localUser.ID);

                return new List<ClientData>() { youData, yourPartnerData };
            }

            Debug.LogError("You are trying to create a session but the name already exists.");
            return null;
        }

        public void CreateLocalUser(string username, string id)
        {
            localUser = Factory<User>.GetItem().Init(username, id);
        }

        public ClientData CreateClient(string clientName, string id = "none")
        {
            ClientData data = Factory<ClientData>.GetItem().Init(id == "none" ? Guid.NewGuid().ToString() : id, clientName);
            return data;
        }

        public ClientData GetClientFromID(string clientID)
        {
            return CurrentSession.sessionMembers.Find(t => t.data.ID == clientID).data;
        }

        public void AddProvision(string senderID, string comment, float amount, Currency currency)
        {
            ProvisionData newProvision = Factory<ProvisionData>.GetItem().Init(System.Guid.NewGuid().ToString(), senderID, amount, currency, comment);
            CurrentSession.AddProvision(newProvision);
        }

        public void AddExpense(string senderID, List<string> payorsIds, float amount, string comment, Currency currency, Category category)
        {
            TransactionData newExpense = Factory<TransactionData>.GetItem().InitExpense(System.Guid.NewGuid().ToString(), amount, comment, senderID, payorsIds, currency, category, DateTime.Now);
            CurrentSession.AddTransaction(newExpense);
        }

        public void AddPayback(string senderID, string beneficiaryID, float amount, Currency currency)
        {
            TransactionData newPayback = Factory<TransactionData>.GetItem().InitPayback(System.Guid.NewGuid().ToString(), senderID, beneficiaryID, amount, currency, DateTime.Now);
            CurrentSession.AddTransaction(newPayback);
        }

        public void AddRebalance()
        {
            TransactionData newRebalance = CurrentSession.ComputeRebalance();
            CurrentSession.AddTransaction(newRebalance);
        }

        public void RefreshCurrentSession(Action onRefreshed)
        {
            if (CurrentSession != null)
            {
                CurrentSession.RefreshSessionAsync(onRefreshed);
            }
        }


    }
}
