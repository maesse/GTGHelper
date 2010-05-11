using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using HtmlAgilityPack;

namespace GTGHelper
{
    public partial class Form1 : Form
    {
        delegate void AppendTextDelegate(string str);
        public Parser parser;
        bool _commentLock = true;
        bool _racerLock = true;
        bool commentLock { get { return _commentLock; } set { _commentLock = value; CheckLock(); } } // true untill comments are parsed
        bool racerLock { get { return _racerLock; } set { _racerLock = value; CheckLock(); } } // true untill racewinners has been set

        // Racer names
        static string[] drivers = new string[] {"Jenson", "Button",
                                                "Lewis", "Hamilton",
                                                "Michael", "Schumacher",
                                                "Nico", "Rosberg",
                                                "Sebastian", "Vettel",
                                                "Mark", "Webber",
                                                "Felipe", "Massa",
                                                "Fernando", "Alonso",
                                                "Rubens", "Barrichello",
                                                "Nico", "Hulkenberg",
                                                "Robert", "Kubica",
                                                "Vitaly", "Petrov",
                                                "Adrian", "Sutil",
                                                "Vitantonio", "Liuzzi",
                                                "Sebastien", "Buemi",
                                                "Jaime", "Alguersuari",
                                                "Jarno", "Trulli",
                                                "Heikki", "Kovalainen",
                                                "Karun", "Chandhok",
                                                "Bruno", "Senna",
                                                "Pedro", "Rosa",
                                                "Kamui", "Kobayashi",
                                                "Timo", "Glock",
                                                "Lucas", "Grassi" };

        static string[] fullDrivers = new string[] 
        {
            "Jenson Button",
            "Lewis Hamilton",
            "Michael Schumacher",
            "Nico Rosberg",
            "Sebastian Vettel",
            "Mark Webber",
            "Felipe Massa",
            "Fernando Alonso",
            "Rubens Barrichello",
            "Nico Hulkenberg",
            "Robert Kubica",
            "Vitaly Petrov",
            "Adrian Sutil",
            "Vitantonio Liuzzi",
            "Sebastien Buemi",
            "Jaime Alguersuari",
            "Jarno Trulli",
            "Heikki Kovalainen",
            "Karun Chandhok",
            "Bruno Senna",
            "Pedro de la Rosa",
            "Kamui Kobayashi",
            "Timo Glock",
            "Lucas di Grassi"
        };

        public Form1()
        {
            
            InitializeComponent();
            Hook.form = this;
        }

        // Enables/Disables main parse button
        void CheckLock()
        {
            if (!commentLock && !racerLock)
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }

        

        public void WriteLine(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                AppendTextDelegate d = new AppendTextDelegate(WriteLine);
                this.Invoke(d, new object[] { text });
            } else
                richTextBox1.Text += text + '\n';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Fetch webpage
            richTextBox1.Text = "";
            commentLock = true;
            labelPred.Text = "";
            labelNonPred.Text = "";
            string url = textBox1.Text;
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork += new DoWorkEventHandler(Parser.ParseUrl);
            worker.RunWorkerAsync(url);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Check if parsing is OK, and disables the comment lock
            if (parser != null && parser.parseResults.Count > 0)
            {
                commentLock = false;
                labelPred.Text = "" + parser.parseResults.Count;
                labelNonPred.Text = "" + parser.failedComments.Count;
                this.AcceptButton = button2;
            }
        }

        // Got doubleclick event on a driver
        private void SelectDriver(object sender, EventArgs e)
        {
            if (sender is ListControl)
            {
                // Limit drivers to 10
                if (racewinners.Items.Count == 10)
                {
                    return;
                }

                string driverName = ((ListControl)sender).Text;

                // Check if driver is already added
                for (int i = 0; i < racewinners.Items.Count; i++)
                {
                    if (((string)racewinners.Items[i]).Equals(driverName))
                        return;
                }
                // Now add the driver
                racewinners.Items.Add(driverName);
                if (racewinners.Items.Count == 10)
                    racerLock = false;
                driverlist.Items.RemoveAt(driverlist.SelectedIndex);
            }
        }

        // Clear race winner list
        private void Clear_Click(object sender, EventArgs e)
        {
            driverlist.Items.Clear();
            driverlist.Items.AddRange(fullDrivers);
            racewinners.Items.Clear();
            racerLock = true;
        }

        // Calculate points for redditors
        private void button2_Click(object sender, EventArgs e)
        {
            if (racewinners.Items.Count != 10)
                throw new Exception("This shouldn't happen. Tried to calculate points without 10 racewinners");

            if (parser == null || parser.parseResults.Count == 0)
                throw new Exception("This shouldn't happen. Tried to calculate points without any comments");

            // Grab racewinners
            string[] winners = new string[10];
            for (int i = 0; i < 10; i++)
            {
                winners[i] = (string)racewinners.Items[i];
            }
            List<Redditor> failedCalc = new List<Redditor>();
            List<Redditor> successCalc = new List<Redditor>();
            // Handle each GTG Comment
            foreach (Redditor red in parser.parseResults)
            {
                // Will be set to false if a prediction couldn't be parsed
                bool parseOk = true;

                // Gather 10 points
                int[] points = new int[10];

                // Go through predictions
                for (int i = 0; i < 10; i++)
                {
                    string predictName = red.Predictions[i];
                    // Check if the prediction name is a known racer
                    string[] predTokens = predictName.ToLower().Split(' ');
                    bool nameOk = false;
                    for (int j = 0; j < predTokens.Length && !nameOk; j++)
                    {
                        string token = predTokens[j];
                        for (int h = 0; h < drivers.Length; h++)
                        {
                            if (drivers[h].ToLower().Equals(token))
                            {
                                nameOk = true;
                                break;
                            }
                        }
                    }

                    if (!nameOk)
                    {
                        // Abort this redditor
                        red.ExtraComment += "[ScoreCalculator] I don't understand this: " + predictName + "\n";
                        parseOk = false;
                        break;
                    }

                    // Now check against racewinners
                    bool foundWinner = false;
                    for (int j = 0; j < winners.Length && !foundWinner; j++)
                    {
                        string[] splitted = winners[j].ToLower().Split(' ');
                        // Check both first and lastname
                        for (int h = 0; h < splitted.Length && !foundWinner; h++)
                        {
                            // Check each predictTokens
                            for (int g = 0; g < predTokens.Length ; g++)
                            {
                                if (splitted[h].Equals(predTokens[g]))
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
            Hook.WriteLine("Good predictions: " + successCalc.Count + " - Bad predictions: " + failedCalc.Count);
            Hook.WriteLine("------------------------------------");
            Hook.WriteLine("---    Listing Good Predictions  ---");
            Hook.WriteLine("------------------------------------");
            foreach (Redditor red in successCalc)
            {
                // Print name, point table and total score
                string redString = string.Format("{0,-32}", red.Name) + "\t[";
                for (int i = 0; i < 10; i++)
                {
                    redString += red.points[i] + "+";
                }
                redString = redString.Substring(0, redString.Length - 1);
                redString += "] \tTotal: " + red.TotalScore + " points.";
                Hook.WriteLine(redString);
            }
            Hook.WriteLine("------------------------------------");
            Hook.WriteLine("---   Listing Bad Predictions    ---");
            Hook.WriteLine("------------------------------------");
            foreach (Redditor red in failedCalc)
            {
                Hook.WriteLine(red.ToString());
            }
            Hook.WriteLine("------------------------------------");
            Hook.WriteLine("- Score calculation complete.");
            Hook.WriteLine("------------------------------------");
            Hook.WriteLine("----  Listing FAILED Comments   ----");
            Hook.WriteLine("------------------------------------");
            foreach (Redditor red in parser.failedComments)
            {
                Hook.WriteLine(red.ToString());
            }
            Hook.WriteLine("------------------------------------");
            Hook.WriteLine("------------------------------------");
            Hook.WriteLine("GTGHelper finished :)");
        }
    }
}
