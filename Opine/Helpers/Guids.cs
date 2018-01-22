using System;

namespace Opine
{
    public static class Guids
    {
        public static Guid ToGuid(string s)
        {
            return Guid.Parse(s);
        }

        public static Guid ToGuid(object o)
        {
            if (o is Guid g)
            {
                return g;
            }
            else if (o is string s)
            {
                return ToGuid(s);
            }
            else
            {
                return ToGuid(o.ToString());
            }
        }
    }
}