using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace GTGHelper
{
    

    [DataContract]
    public class RedditObject
    {
        [DataMember]
        public string kind { get; set; }
        [DataMember]
        public RedditData data { get; set; }
    }

    [DataContract]
    public class RedditData : Object
    {
        public RedditData() { children = new List<RedditObject>();  }
        [DataMember]
        public List<RedditObject> children { get; set; }
        [DataMember]
        public string body { get; set; }
        [DataMember]
        public string author { get; set; }
        [DataMember]
        public string created_utc { get; set; }
        [DataMember]
        public RedditObject replies { get; set; }
    }

    public class Comment
    {
        public string name { get; set; }
        public string body { get; set; }
        public string commentTime { get; set; }
        public List<Comment> replies = new List<Comment>();
    }

    class JSONParser : Parser
    {
        public static void ParseUrl(object sender, DoWorkEventArgs e)
        {
            string url = (string)e.Argument + "/.json";
            Hook.WriteLine("Reading url: " + url);
            Stream stream;
            try
            {
                // Try to fetch from webpage
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);

                // Auto-detecting proxy takes a horribly long time, so disable it by default
                if (!UseProxy)
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
            // Read HTML from response stream
            string html;
            using (StreamReader rd = new StreamReader(stream))
            {
                html = rd.ReadToEnd();
            }

            // Try to parse the html as a reddit GTG comment page
            JSONParser parse = new JSONParser(html);
            e.Result = parse;
        }

        private Comment parseComments(RedditObject obj) {
            RedditData data = obj.data;
            Comment comment = new Comment();
            comment.name = data.author;
            comment.commentTime = data.created_utc;
            comment.body = data.body;
            if (data.replies != null)
            {
                foreach (var item in data.replies.data.children)
                {
                    Comment subComment = parseComments(item);
                    if (subComment != null)
                    {
                        comment.replies.Add(subComment);
                    }
                }
            }
            return comment;
        }

        // Parse HTML page
        public JSONParser(String json)
        {
            // Hack!
            // Reddit puts empty strings "" when there is no replies. C# doesn't like this,
            // so i just remove all instances off "replies": "", from the JSON
            json = json.Replace("\"replies\": \"\",", "");
            Hook.WriteLine("Starting to parse JSON");
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<RedditObject> baseObject2 = ser.Deserialize<List<RedditObject>>(json);

            // Create a clean comment tree
            List<Comment> comments = new List<Comment>();
            foreach (var item in baseObject2[1].data.children)
            {
                Comment comment = parseComments(item);
                comments.Add(parseComments(item));
            }

            Hook.WriteLine("JSON stage complete. Checking comments...");


            foreach (var item in comments)
            {
                ParseComment(item);
            }
        }


        // Handle single comments HTML
        void ParseComment(Comment comment)
        {
            CommentCount++;

            // Read comment info
            Redditor red = new Redditor();
            red.Name = comment.name;
            red.PostTime = comment.commentTime;
            red.CommentText = comment.body;

            // Clean up
            red.PostTime = red.PostTime.Replace("&#32;", " ");
            // Made edits more visible
            if (red.PostTime.Contains("*") || red.PostTime.Contains("&#42;"))
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

            foreach (var item in comment.replies)
            {
                ParseComment(item);
            }
        }


    }
}
