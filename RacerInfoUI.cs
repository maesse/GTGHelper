using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GTGHelper
{
    public partial class RacerInfoUI : UserControl
    {
        Racer racer;
        RacerlistChanged changeDelegate;

        public RacerInfoUI(Racer racer, RacerlistChanged changeDelegate)
        {
            this.racer = racer;
            this.changeDelegate = changeDelegate;
            InitializeComponent();
            driverNameBox.Text = racer.Name;

            string[] splittedName = racer.Name.Split(' ');

            foreach (string alt in racer.Alternatives)
            {
                bool fromRealName = false;
                foreach (string n in splittedName)
                {
                    if (n.Equals(alt))
                    {
                        fromRealName = true;
                        break;
                    }
                }
                if (fromRealName)
                {
                    continue;
                }
                namesBox.AppendText(alt);
                namesBox.AppendText("\n");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // save in racer
            racer.SetName(driverNameBox.Text);
            
            string[] names = namesBox.Text.Split('\n');
            racer.Alternatives.AddRange(names);

            changeDelegate();
        }
    }
}
