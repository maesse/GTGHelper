using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Net;
using System.ComponentModel;

namespace GTGHelper
{

    public abstract class Parser
    {
        public List<Redditor> parseResults = new List<Redditor>(); // Nodes parsed as GTG comments
        public List<Redditor> failedComments = new List<Redditor>(); // Comments that couldn't be GTG parsed
        public int CommentCount = 0;
        public static bool UseProxy = false;
    }
}
