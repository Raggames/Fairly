using EquiCount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FairlyCount.View
{
    public class MainMenuWindow : MonoBehaviour, IAbstractWindow
    {
        [Header("Panels")]
        public CreateSessionPanel createSessionPanel;

        [Header("Sessions Preview Content")]
        public UISessionPreview pf_UiSessionPreview;
        public List<UISessionPreview> uiSessionPreviews = new List<UISessionPreview>();

        public void Open()
        {
            this.gameObject.SetActive(true);
        }

        public void Close()
        {
            this.gameObject.SetActive(false);

            createSessionPanel.Close();
        }

        public void Init()
        {

        }

        public void OpenSession(string sessionDataId)
        {
            SessionData data = Controller.Instance.loadedDatas.Find(t => t.ID == sessionDataId);
            if(data != null)
            {
                // On ouvre la session
                Controller.Instance.OpenLoadedSession(data);
                // On appelle un refresh, qui va computer les données de la session, et déclencher une callback 
                Controller.Instance.RefreshCurrentSession(OpenSessionWindow);
            }
            else
            {
                Debug.LogError("Data is null for this ID");
            }
        }

        private void OpenSessionWindow()
        {
            // On ferme cette fenêtre
            Controller.Instance.mainMenuWindow.Close();
            // Pour raffraichir on clean tout avec un close d'abord
            Controller.Instance.currentSessionWindow.Close();
            // Puis on ouvre la fenêtre de session courante
            Controller.Instance.currentSessionWindow.Open();
            // Et on l'initialise
            Controller.Instance.currentSessionWindow.Refresh(Controller.Instance.CurrentSession);
        }
    }
}
