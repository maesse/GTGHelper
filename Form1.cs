using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Xml.Serialization;
using System.Xml;

namespace GTGHelper
{
    // Delegate for appending text to the GUI from another thread.
    public delegate void DriverListChanged();
    delegate void AppendTextDelegate(string str);

    public partial class Form1 : Form
    {
        
        
        Parser parser;
        static string UpdateURL;
        bool _commentLock = true;
        bool _racerLock = true;
        bool commentLock { get { return _commentLock; } set { _commentLock = value; CheckLock(); } } // true untill comments are parsed
        bool racerLock { get { return _racerLock; } set { _racerLock = value; CheckLock(); } } // true untill racewinners has been set

        DriverListChanged driverChangedDelegate;

        // List of all racers in the champtionship
        List<Racer> Racers = new List<Racer>();

        // Inits program
        void Init()
        {
            Racers = DriverLoader.GetRacers();

            // Add racers to UI
            driverlist.Items.Clear();
            driverlist.Items.AddRange(Racers.ToArray());

            driverChangedDelegate = new DriverListChanged(RefreshDriverLists);
        }

        private void RefreshDriverLists()
        {
            // Add racers to UI
            driverlist.Items.Clear();
            driverlist.Items.AddRange(Racers.ToArray());

            
        }

        public Form1()
        {
            InitializeComponent();
            // Set title
            this.Text = "GTGGHelper " + Program.VERSION;
            Hook.form = this;

            Init();
            CheckForUpdate();
        }

        // Enables/Disables main parse button
        void CheckLock()
        {
            if (!commentLock && !racerLock)
                buttonCalculatePoints.Enabled = true;
            else
                buttonCalculatePoints.Enabled = false;
        }

        // Output line to the textarea
        public void WriteLine(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                // Handle cross-thread call
                AppendTextDelegate d = new AppendTextDelegate(WriteLine);
                this.Invoke(d, new object[] { text });
            } else
                richTextBox1.Text += text + '\n';
        }

        // "Read Comments" button for fetching webpage
        private void buttonParseUrl_Click(object sender, EventArgs e)
        {
            // Reset fields
            richTextBox1.Text = "";
            commentLock = true;
            labelPred.Text = "";
            labelNonPred.Text = "";

            // Let a async thread handle the rest
            string url = textBox1.Text;
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork += new DoWorkEventHandler(JSONParser.ParseUrl);
            worker.RunWorkerAsync(url);
        }

        // Start a async check for update
        void CheckForUpdate()
        {
            string url = "http://github.com/maesse/GTGHelper/raw/master/version.txt";
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(updateCheck_RunWorkerCompleted);
            worker.DoWork += new DoWorkEventHandler(DoCheckForUpdate);
            worker.RunWorkerAsync(url);
        }

        void DoCheckForUpdate(object sender, DoWorkEventArgs e)
        {
            string url = e.Argument as string;
            if (url == null)
                return;
            Stream stream;
            try
            {
                // Try to fetch from webpage
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);

                // Auto-detecting proxy takes a horribly long time, so disable it by default
                if (!Parser.UseProxy)
                    wr.Proxy = null;

                // Don't want to get rejected because of a bad UserAgent.
                wr.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2.3) Gecko/20100401 Firefox/3.6.3";
                wr.Referer = "http://www.google.com/";

                // Get the response stream
                stream = wr.GetResponse().GetResponseStream();
            }
            catch
            {
                return;
            }
            string html;
            using (StreamReader rd = new StreamReader(stream))
            {
                html = rd.ReadToEnd();
            }

            string[] splitted = html.Split('\n');
            // Expect 3 lines at least
            if (splitted.Length < 3)
                return;
            int versionint = 0;
            // Try to parse versionint
            if (!int.TryParse(splitted[0], out versionint))
                return;

            if (versionint <= Program.VersionInt)
                return;

            // We have a new version. Get url
            string newversionUrl = splitted[1];

            e.Result = splitted;

        }

        void updateCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string[] args = e.Result as string[];
            if (args == null)
                return;

            linkLabel1.Text = "New version available";
            UpdateURL = args[2];
            linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
            
        }

        void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = UpdateURL;
            proc.Start();
        }

        // Called when the URL/GTG parser finishes
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Parser parse = e.Result as Parser;
            // Check if parsing is OK, and disables the comment lock
            if (parse != null && parse.parseResults.Count > 0)
            {
                parser = parse;
                commentLock = false;
                labelPred.Text = "" + parser.parseResults.Count;
                labelNonPred.Text = "" + parser.failedComments.Count;
                this.AcceptButton = buttonCalculatePoints;
            }
        }

        // Got doubleclick event on a driver
        private void SelectDriver(object sender, EventArgs e)
        {
            if (sender is ListControl)
            {
                // Limit drivers to 10
                if (racewinners.Items.Count == 10)
                    return;
                
                Racer racer = driverlist.SelectedItem as Racer;

                // Ensure that a Racer object has been selected
                if (racer == null)
                    return;

                // Check if driver is already added
                if (racewinners.Items.Contains(racer))
                    return;

                // Add to RaceWinners
                racewinners.Items.Add(racer);

                // Check if we now have 10 winners.
                if (racewinners.Items.Count == 10)
                    racerLock = false;

                // Remove from DriverList
                driverlist.Items.RemoveAt(driverlist.SelectedIndex);
            }
        }

        // Clear race winner list
        private void Clear_Click(object sender, EventArgs e)
        {
            // Re-insert all the drivers in the driverlist
            driverlist.Items.Clear();
            driverlist.Items.AddRange(Racers.ToArray());

            // Clear racewinners
            racewinners.Items.Clear();
            racerLock = true;
        }

        // Calculate points for redditors
        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            // Sanity check
            if (racewinners.Items.Count != 10)
                throw new Exception("This shouldn't happen. Tried to calculate points without 10 racewinners");

            if (parser == null || parser.parseResults.Count == 0)
                throw new Exception("This shouldn't happen. Tried to calculate points without any comments");

            // Grab racewinners
            Racer[] winners = new Racer[10];
            for (int i = 0; i < 10; i++)
            {
                winners[i] = (Racer)racewinners.Items[i];
            }

            List<Redditor> failedCalc = new List<Redditor>();
            List<Redditor> successCalc = new List<Redditor>();
            // Handle each GTG Comment
            foreach (Redditor red in parser.parseResults)
            {
                red.ExtraParseComment = "";
                // Will be set to false if a prediction couldn't be parsed
                bool parseOk = true;

                // Gather 10 points
                int[] points = new int[10];

                // Go through predictions
                // Check if the prediction name is a known racer
                for (int i = 0; i < 10; i++)
                {
                    string predictName = red.Predictions[i];
                    if (predictName.Equals("!MISSING-PREDICTION"))
                        continue;
                    string[] predTokens = predictName.ToLower().Split(' ');
                    
                    bool nameOk = false;
                    // Iterate over tokens
                    for (int j = 0; j < predTokens.Length && !nameOk; j++)
                    {
                        string token = predTokens[j];

                        // Compare to known racers
                        for (int h = 0; h < Racers.Count; h++)
                        {
                            Racer racer = Racers[h];
                            if (racer.ContainsName(token))
                            {
                                nameOk = true;
                                break;
                            }
                        }
                    }

                    if (!nameOk)
                    {
                        // Abort this redditor
                        red.ExtraParseComment += "[ScoreCalculator] I don't understand this: " + predictName + "\n";
                        parseOk = false;
                        break;
                    }

                    // Now check against racewinners
                    bool foundWinner = false;
                    for (int j = 0; j < winners.Length && !foundWinner; j++)
                    {
                        Racer racer = winners[j];

                        // Check each predictTokens
                        for (int g = 0; g < predTokens.Length; g++)
                        {
                            // Check against racername
                            if (racer.ContainsName(predTokens[g]))
                            {
                                // Redditor gets point for this prediction
                                foundWinner = true;

                                // Was it spot on?
                                if (i == j)
                                {
                                    points[i] = 4;
                                } // One off? 
                                else if (i + 1 == j || i - 1 == j)
                                {
                                    points[i] = 2;
                                } // prediction is in top10
                                else
                                {
                                    points[i] = 1;
                                }

                                break;
                            }
                        }
                    }
                    // Prediction didn't pay out, no points..
                    if (!foundWinner)
                        points[i] = 0;

                }

                if (!parseOk)
                {
                    // Save failed redditor for later handling
                    failedCalc.Add(red);
                    continue;
                }

                // Add up points
                int total = 0;
                for (int i = 0; i < 10; i++)
                {
                    total += points[i];
                }

                // Save Calc results..
                red.ScoreCalculated = true;
                red.points = points;
                red.TotalScore = total;

                successCalc.Add(red);
            }

            // Show God/Bad comment count
            labelPred.Text = successCalc.Count + " Good, " + failedCalc.Count + " Bad comments";

            // Sort redditors by score and print result.
            successCalc.Sort((a, b) => { return b.TotalScore.CompareTo(a.TotalScore); });
            Hook.WriteLine("Reddit GuessTheGrid Points:");
            Hook.WriteLine(string.Format("Good predictions:   \t{0,2}", successCalc.Count));
            Hook.WriteLine(string.Format("Bad predictions:    \t{0,2}", failedCalc.Count));
            Hook.WriteLine(string.Format("Failed predictions: \t{0,2}", parser.failedComments.Count));
            if (successCalc.Count > 0)
            {
                Hook.WriteLine("------------------------------------");
                Hook.WriteLine("---    Listing Good Predictions  ---");
                Hook.WriteLine("------------------------------------");
            
                foreach (Redditor red in successCalc)
                {
                    // Print name, point table and total score
                    string redString = string.Format("{0,-32}", red.Name) + "\t[";
                    for (int i = 0; i < 10; i++)
                    {
                        if (!red.Predictions[i].Equals("!MISSING-PREDICTION"))
                            redString += red.points[i] + "+";
                        else
                            redString += "?+";
                    }
                    redString = redString.Substring(0, redString.Length - 1);
                    redString += "] \tTotal: " + red.TotalScore + " points.";
                    Hook.WriteLine(redString);
                }
            }
            if (failedCalc.Count > 0)
            {
                Hook.WriteLine("------------------------------------");
                Hook.WriteLine("---   Listing Bad Predictions    ---");
                Hook.WriteLine("------------------------------------");
                foreach (Redditor red in failedCalc)
                {
                    Hook.WriteLine(red.ToString());
                }
            }
            if (parser.failedComments.Count > 0)
            {
                Hook.WriteLine("------------------------------------");
                Hook.WriteLine("----  Listing FAILED Comments   ----");
                Hook.WriteLine("------------------------------------");
                foreach (Redditor red in parser.failedComments)
                {
                    Hook.WriteLine(red.ToString());
                }
            }
            Hook.WriteLine("------------------------------------");
            Hook.WriteLine("GTGHelper finished. :)");
        }

        private void driverButton_Click(object sender, EventArgs e)
        {
            DriverEditUI driverEdit = new DriverEditUI(Racers, driverChangedDelegate);
            driverEdit.ShowDialog(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.ResetText();
        }
    }
}
