using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GTGHelper
{
    public delegate void RacerlistChanged();
    public partial class DriverEditUI : Form
    {
        List<Racer> racers;

        RacerlistChanged changeDelegate;
        DriverListChanged applyDelegate;
        public DriverEditUI(List<Racer> racers, DriverListChanged applyDelegate)
        {
            this.applyDelegate = applyDelegate;
            this.racers = racers;

            InitializeComponent();

            foreach(Racer racer in racers) 
            {
                driverList.Items.Add(racer);
            }
            changeDelegate = new RacerlistChanged(updateDriverList);
        }

        private void updateDriverList()
        {
            int index = driverList.SelectedIndex;
            driverList.Items.Clear();
            driverList.Items.AddRange(racers.ToArray());
            driverList.SelectedIndex = index;
        }

        private void driverListIndexChanged(object sender, EventArgs e)
        {
            optionsPanel.Controls.Clear();

            int index = driverList.SelectedIndex;
            if (index < 0 || index >= racers.Count)
            {
                return;
            }

            Racer racer = racers[index];
            RacerInfoUI racerUI = new RacerInfoUI(racer, changeDelegate);
            
            optionsPanel.Controls.Add(racerUI);
            racerUI.Dock = DockStyle.Fill;
        }

        private void removeDriverButton_Click(object sender, EventArgs e)
        {
            int index = driverList.SelectedIndex;
            if (index < 0 || index >= racers.Count)
            {
                return;
            }

            racers.RemoveAt(index);
            driverList.ClearSelected();
            driverList.Items.RemoveAt(index);
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            DriverLoader.SaveDriverList(racers);
            applyDelegate();
            this.Close();
        }

        private void addDriverButton_Click(object sender, EventArgs e)
        {
            Racer newracer = new Racer();
            newracer.Name = "New Racer";

            racers.Add(newracer);
            driverList.Items.Add(newracer);
        }
    }
}
