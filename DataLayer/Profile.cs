﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binder.DataLayer
{
    public class Profile
    {

        public string title { get; set; }
        public List<Bind> binds { get; set; }

        public Profile(string title, int allowedBinds)
        {
            this.title = title;
            binds = new List<Bind>();

            for (int i = 0; i < allowedBinds; i++)
                binds.Add(new Bind(string.Empty));
        }
    }
}
