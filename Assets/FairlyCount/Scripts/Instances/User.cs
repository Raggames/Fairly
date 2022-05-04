using EquiCount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairlyCount
{
    /// <summary>
    /// Classe de l'utilisateur local
    /// </summary>
    public class User : IData
    {        
        public string ID { get; set; }
        public string username;

        public User Init(string name, string id)
        {
            ID = id;
            username = name;
            return this;
        }

        public void Clear()
        {
            ID = "";
            username = "";
        }
    }
}
