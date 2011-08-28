using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using System.Net;
using System.ComponentModel;

namespace GTGHelper
{

    public class Parser
    {
        public List<Redditor> parseResults = new List<Redditor>(); // Nodes parsed as GTG comments
        public List<HtmlNode> failedNodes = new List<HtmlNode>(); // Nodes that couldn't be parsed as comments
        public List<Redditor> failedComments = new List<Redditor>(); // Comments that couldn't be GTG parsed
        HtmlDocument doc;
        public int CommentCount = 0;
        static string EMPTY_STRING = "                                                                          ";
        public static bool UseProxy = false;

        // Fetch a webpage and parse it with a new Parse instance.
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

                // Don't want to get rejected because of a bad UserAgent.
                wr.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2.3) Gecko/20100401 Firefox/3.6.3";
                wr.Referer = "http://www.google.com/";

                // Get the response stream
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
            e.Result = parse;
        }

        // Parse HTML page
        public Parser(string html)
        {
            ReadDocument(html);
            HtmlNode node;

            // Fail...
            if (doc.DocumentNode.ChildNodes.Count < 2
                || doc.DocumentNode.ChildNodes[1].ChildNodes.Count < 2
                || doc.DocumentNode.ChildNodes[1].ChildNodes[1].ChildNodes.Count < 8
                || doc.DocumentNode.ChildNodes[1].ChildNodes[1].ChildNodes[4].ChildNodes.Count < 3
                || doc.DocumentNode.ChildNodes[1].ChildNodes[1].ChildNodes[4].ChildNodes[2].ChildNodes.Count < 4)
            {
                Hook.WriteLine("[GTGHelper] Either this is not a reddit GTG comment page, or reddit has changed some stuff around and you need to message mazing.");
                return;
            }
            
            // Read comment tree
            node = doc.DocumentNode.ChildNodes[1].ChildNodes[1].ChildNodes[4].ChildNodes[2].ChildNodes[2];
            foreach (HtmlNode nodes in node.ChildNodes)
            {
                ParseComment(nodes, 0);
            }

            Hook.WriteLine(string.Format("Finished reading {0} comments", CommentCount));
            Hook.WriteLine("--------------------------------\n");
        }

        // Handle single comments HTML
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
            red.Name = node.ChildNodes[2].ChildNodes[0].ChildNodes[1].InnerText;
            red.PostTime = node.ChildNodes[2].ChildNodes[0].Element("time").InnerText;
            red.CommentText = node.ChildNodes[2].ChildNodes[1].ChildNodes[3].InnerText;

            // Clean up
            red.PostTime = red.PostTime.Replace("&#32;", " ");
            // Made edits more visible
            if(red.PostTime.Contains("*") || red.PostTime.Contains("&#42;"))
            {
                red.PostTime = red.PostTime.Replace("*", "*(EDITED!!!)");
                red.PostTime = red.PostTime.Replace("&#42;", "*(EDITED!!!)");
                red.Edited = true;
            }

            red.ExtraComment = "";

            // Try to parse the comments content
            if (red.ParseGTGComment())
                parseResults.Add(red);
            else
                failedComments.Add(red);
        }

        void ReadDocument(string str)
        {
            doc = new HtmlDocument();
            doc.LoadHtml(str);
        }
    }
}
