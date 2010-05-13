using System;
using System.Collections.Generic;
using System.Text;

namespace GTGHelper
{
    // Static hook to the Form class. Used for outputting to the textarea.
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
