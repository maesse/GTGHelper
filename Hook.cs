using System;
using System.Collections.Generic;
using System.Text;

namespace GTGHelper
{
    public static class Hook
    {
        public static Form1 form;

        public static void WriteLine(string str)
        {
            if (form != null)
                form.WriteLine(str);
        }
    }
}
