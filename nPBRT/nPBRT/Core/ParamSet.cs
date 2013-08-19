using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core
{
    public class ParamSet
    {
        private Hashtable parameters;

        public ParamSet()
        {
            parameters = new Hashtable();
        }

        public void AddParam(string name, object value)
        {
            parameters[name] = value;
        }
        public object GetParam(string name, object def)
        {
            if (parameters.ContainsKey(name))
            {
                return parameters[name];
            }
            else
            {
                return def;
            }
        }

        public void Clear()
        {
            parameters.Clear();
        }
    }
}
