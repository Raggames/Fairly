using EquiCount;
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
    public class CreateSessionPanel : MonoBehaviour, IAbstractPanel
    {
        private IAbstractWindow _masterWindow;
        public IAbstractWindow masterWindow { get { return _masterWindow; } set { _masterWindow = value; } }

        public TMP_InputField sessionName_InputField;
        public TMP_InputField yourPartner_InputField;

        public void Close()
        {
            this.gameObject.SetActive(true);
        }

        public void Open()
        {
            this.gameObject.SetActive(false);
        }

        public void CreateSession()
        {
            ClientData youData = Factory<ClientData>.GetItem().Init(Controller.Instance.localUser.ID, Controller.Instance.localUser.username);
            ClientData yourPartnerData = Factory<ClientData>.GetItem().Init("testpartnerID", yourPartner_InputField.text);

            Controller.Instance.CreateSession(sessionName_InputField.text, youData, yourPartnerData);
        }
    }
}
