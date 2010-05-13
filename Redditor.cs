using System;
using System.Collections.Generic;
using System.Text;

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
                string lowered = str.Trim().ToLower();

                if (lowered.Length == 0)
                    continue;

                // Remove leading "*" if it is there
                if (lowered.StartsWith("*"))
                {
                    lowered = lowered.Substring(1).Trim();
                }

                // Get the position
                bool noP = false;
                if (!lowered.StartsWith("p"))
                {
                    noP = true;
                    //extra.AppendLine("> " + str);
                    //continue;
                }
                int firstSpace = lowered.IndexOf(' ');
                if (firstSpace == -1)
                {
                    extra.AppendLine("> " + str);
                    continue;
                }
                string pos = lowered.Substring(noP?0:1, firstSpace).Trim();
                pos = pos.TrimEnd(new char[] { '.', ':' });
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
                pred = pred.Trim().TrimEnd('.');
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
                    if (i != ipos - 1 && Predictions[i] != null && Predictions[i] == pred)
                    {
                        extra.AppendLine("[GTGHelper] The same driver (" + pred + ") has been used twice! Do a manual check.");
                        failed = true;
                    }
                }
                Predictions[ipos - 1] = pred;
                posParsed++;
            }
            ExtraComment = extra.ToString();
            // Allow one missing prediction
            if (posParsed < 9)
                failed = true;
            if (posParsed == 9)
            {
                Hook.WriteLine(string.Format("[GTGHelper] Redditor '{0}' only has 9 predictions!", Name));
            }
            // Parser ignores anything other than P1-P10, so max parsed should be 10
            if (posParsed > 10)
                failed = true;
            if (!failed)
            {
                // Check that this was a full predict
                for (int i = 0; i < 10; i++)
                {
                    if (Predictions[i] == null)
                    {
                        Predictions[i] = "!MISSING-PREDICTION";
                        ExtraComment += "[GTGHelper] Comment didn't contain prediction for P" + (i + 1) + "!\n";
                    }
                }
            }
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
                    pred += string.Format("\tP{0}: {1}\n", i + 1, Predictions[i]);
                }
            }
            if (pred.Length > 0 && !Error)
                pred = "Predictions:\n" + pred + "\n";
            //else if(Error)
            //    pred = "[GTGHelper] Couldn't find any prediction, perhaps do a manual check.\n";
            return string.Format("Name: {0} \t Posted: {1}\n{2}Unparsed comment content:\n{3}--------------------------------\n", Name, PostTime, pred, ExtraComment);
        }
    }
}
