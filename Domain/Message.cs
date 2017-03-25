using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    [Serializable]
    public class Message
    {
        public User User { get; set; }
        public string Content { get; set; }

        public Message() { }
        public Message(string message)
        {
            Content = message;
        }

        public override string ToString()
        {
            if (User == null)
                return Content;
            else
                return string.Format("{0}: {1}", User.Name, Content);
        }
    }
}
