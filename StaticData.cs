using System;
using System.Collections.Generic;
using System.Text;

namespace Shorthander
{
    public static class StaticData
    {
        public const string MAGIC = "(Shorthander)";

        public static readonly string[] HEADER = new string[]
        {
            "&F1 #####################################################################&NL",
            "&F1 #                                                                   #&NL",
            "&F1 #                           SHORTHANDER                             #&NL",
            "&F1 #                                                                   #&NL",
            "&F1 #####################################################################&NL&NL"
        };

        public static readonly string CURSOR = " @> ";

    }
}
