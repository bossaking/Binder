using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binder.DataLayer
{
    public class Profile
    {

        public string title { get; set; }
        public List<string> binds { get; set; }

        public Profile(string title)
        {
            this.title = title;
            binds = new List<string>(10);
        }
    }
}
