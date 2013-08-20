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
         
        public double FindOneDouble(string name, double def)
        {
            if (parameters.ContainsKey(name))
            {
                try
                {
                    return (double)parameters[name];
                }
                catch  
                {
                    return def;
                }
            }
            else
            {
                return def;
            }
        }


        public Point FindOnePoint(string name, Point def)
        {
            if (parameters.ContainsKey(name))
            {
                try
                {
                    return (Point)parameters[name];
                }
                catch
                {
                    return def;
                }
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
