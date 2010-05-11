using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using System.Net;
using System.ComponentModel;

namespace GTGHelper
{

    public class Redditor
    {
        public string Name;
        public string PostTime;
        public string CommentText; // Raw input string

        public bool ScoreCalculated = false;
        public int[] points;
        public int TotalScore;

        public string[] Predictions = new string[10]; // Parsed predictions
        public string ExtraComment; // Contains any extra text written in the comment
        bool Error = false; // Set if parsing failed
        public bool Edited = false;
        

        
        public bool ParseGTGComment()
        {
            Error = ParseRedditor();
            if (Edited)
                return false;
            return !Error;
        }

        // Try to get grid positions from comment
        bool ParseRedditor()
        {
            StringBuilder extra = new StringBuilder();
            if (CommentText == null)
                return false;
            // Read comment line for line
            string[] lines = CommentText.Split('\n');
            bool failed = false;
            int posParsed = 0;
            foreach (string str in lines)
            {
                str.Trim();
                string lowered = str.ToLower();
                if (lowered.Length == 0)
                    continue;
                // Get the position
                if (!lowered.StartsWith("p"))
                {
                    extra.AppendLine("> " + str);
                    continue;
                }
                int firstSpace = lowered.IndexOf(' ');
                if (firstSpace == -1)
                {
                    extra.AppendLine("> " + str);
                    continue;
                }
                string pos = lowered.Substring(1, firstSpace);
                // try to parse int
                int ipos;
                if (!int.TryParse(pos, out ipos))
                {
                    extra.AppendLine("> " + str);
                    continue;
                }
                if (ipos < 1 || ipos > 10)
                {
                    extra.AppendLine("> " + str);
                    continue;
                }

                // Now get the prediction
                string pred = str.Substring(firstSpace);
                pred = pred.Trim();
                if (pred.Length == 0)
                {
                    extra.AppendLine("> " + str);
                    continue;
                }
                if (Predictions[ipos - 1] != null)
                {
                    // Only post error once
                    if (!failed)
                    {
                        extra.AppendLine("[GTGHelper] !!! Got two predictions for one position, aborting this comment. Do a manual check:");
                        extra.AppendLine("> " + CommentText);
                        failed = true;
                    }
                }
                // Check if the same prediction has been made twice
                for (int i = 0; i < 10; i++)
                {
                    if (i != ipos-1 && Predictions[i] != null && Predictions[i] == pred)
                    {
                        extra.AppendLine("[GTGHelper] The same driver has been used twice!");
                        failed = true;
                    }
                }
                Predictions[ipos - 1] = pred;
                posParsed++;
            }
            ExtraComment = extra.ToString();
            // Check that this was a full predict
            for (int i = 0; i < 10; i++)
            {
                if (Predictions[i] == null)
                    return true;
            }
            if (posParsed != 10)
                failed = true;
            return failed;
        }

        public override string ToString()
        {
            string pred = "";
            if (Error)
                pred = "[GTGHelper] Couldn't find any prediction, perhaps do a manual check.\n";
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    pred += string.Format("\tP{0}: {1}\n", i+1, Predictions[i]);
                }
            }
            return string.Format("Name: {0} \t Posted: {1}\nPredictions:\n{2}Unparsed comment content:\n{3}--------------------------------\n", Name, PostTime, pred, ExtraComment);
        }
    }

    public class Parser
    {
        public List<Redditor> parseResults = new List<Redditor>(); // Nodes parsed as GTG comments
        public List<HtmlNode> failedNodes = new List<HtmlNode>(); // Nodes that couldn't be parsed as comments
        public List<Redditor> failedComments = new List<Redditor>(); // Comments that couldn't be GTG parsed
        HtmlDocument doc;
        public int CommentCount = 0;
        static string EMPTY_STRING = "                                                                          ";
        public static bool UseProxy = false;
        public static void ParseUrl(object sender, DoWorkEventArgs e)
        {
            string url = (string)e.Argument;
            Hook.WriteLine("Reading url: " + url);
            Stream stream;
            try
            {
                // Try to fetch from webpage
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
                // Auto-detecting proxy takes a horribly long time, so disable it by default
                if(!UseProxy)
                    wr.Proxy = null;

                wr.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2.3) Gecko/20100401 Firefox/3.6.3";
                wr.Referer = "http://www.google.com/";
                stream = wr.GetResponse().GetResponseStream();
            }
            catch
            {
                Hook.WriteLine("Could not fetch anything from " + url);
                Hook.WriteLine("Aborting...");
                return;
            }
            Hook.WriteLine("Got response..");
            Hook.WriteLine("Reading HTML");
            // Read HTML from response stream
            string html;
            using (StreamReader rd = new StreamReader(stream))
            {
                html = rd.ReadToEnd();
            }

            Hook.WriteLine("Starting to parse comments...");
            // Try to parse the html as a reddit GTG comment page
            Parser parse = new Parser(html);
        }

        public Parser(string str)
        {
            Hook.form.parser = this;
            ReadDocument(str);
            HtmlNode node;

            // Fail...
            if (doc.DocumentNode.ChildNodes.Count < 2
                || doc.DocumentNode.ChildNodes[1].ChildNodes.Count < 2
                || doc.DocumentNode.ChildNodes[1].ChildNodes[1].ChildNodes.Count < 4
                || doc.DocumentNode.ChildNodes[1].ChildNodes[1].ChildNodes[3].ChildNodes.Count < 3
                || doc.DocumentNode.ChildNodes[1].ChildNodes[1].ChildNodes[3].ChildNodes[2].ChildNodes.Count < 3)
            {
                Hook.WriteLine("[GTGHelper] Either this is not a reddit GTG comment page, or reddit has changed some stuff around and you need to message mazing.");
                return;
            }
            Hook.WriteLine("--------------------------------\n");
            // Read comment tree
            node = doc.DocumentNode.ChildNodes[1].ChildNodes[1].ChildNodes[3].ChildNodes[2].ChildNodes[2];
            foreach (HtmlNode nodes in node.ChildNodes)
            {
                ParseComment(nodes, 0);
            }

            Hook.WriteLine(string.Format("Finished reading {0} comments", CommentCount));
        }

        void ParseComment(HtmlNode node, int indent)
        {
            string str = EMPTY_STRING.Substring(0, indent);

            // Check if all fields are present
            if (node.ChildNodes.Count < 3 || node.ChildNodes[2].ChildNodes.Count < 2 || node.ChildNodes[2].ChildNodes[0].ChildNodes.Count < 6 || node.ChildNodes[2].ChildNodes[1].ChildNodes.Count < 4)
            {
                // Save the failed node
                failedNodes.Add(node);
                return;
            }

            CommentCount++;
            // Read comment info
            Redditor red = new Redditor();
            red.Name = node.ChildNodes[2].ChildNodes[0].ChildNodes[0].InnerText;
            int postTimeOffset = 5;
            // Reddit moderator adds some stuff
            if (node.ChildNodes[2].ChildNodes[0].ChildNodes.Count == 8)
                postTimeOffset = 6;
            red.PostTime = node.ChildNodes[2].ChildNodes[0].ChildNodes[postTimeOffset].InnerText;
            red.CommentText = node.ChildNodes[2].ChildNodes[1].ChildNodes[3].InnerText;

            // Clean up
            red.PostTime = red.PostTime.Replace("&#32;", " ");
            if(red.PostTime.Contains("*") || red.PostTime.Contains("&#42;"))
            {
                red.PostTime = red.PostTime.Replace("*", "*(EDITED!!!)");
                red.PostTime = red.PostTime.Replace("&#42;", "*(EDITED!!!)");
                red.Edited = true;
            }

            if (red.ParseGTGComment())
                parseResults.Add(red);
            else
                failedComments.Add(red);
            
            //Hook.WriteLine(red.ToString());
        }

        void ReadDocument(string str)
        {
            doc = new HtmlDocument();
            doc.LoadHtml(str);
        }
    }
}
