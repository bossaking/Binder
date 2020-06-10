using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binder.DataLayer
{
    public class Bind
    {

        public string text { get; set; }
        public bool imSend { get; set; }

        public Bind(string text)
        {
            this.text = text;
            imSend = false;
        }

    }
}
