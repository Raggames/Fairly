using EquiCount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FairlyCount.View
{
    public class UISessionPreview : MonoBehaviour
    {
        protected MainMenuWindow context;
        protected string sessionDataID;
        
        public void Init(MainMenuWindow context, string sessionDataId)
        {
            this.context = context;
            this.sessionDataID = sessionDataId;
        }

        public void OpenSession()
        {
            context.OpenSession(sessionDataID);
        }
    }
}
