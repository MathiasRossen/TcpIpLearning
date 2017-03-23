using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    [Serializable]
    public class User
    {
        string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;

                if (name.Length > 20)
                    name = name.Substring(0, 20);
            }
        }

        public User(string name)
        {
            Name = name;
        }
    }
}
