using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace GTGHelper
{
    // Represents a racer in the championship.
    // Used for supplying alternative names.
    [Serializable()]
    public class Racer
    {
        // Full name of the driver.
        public string Name;
        // Alternatives will contain Name.split(WhiteSpace) + any alternative spelling of this racer.
        public List<string> Alternatives = new List<string>();

        public Racer()
        {
        }

        public void SetName(string Name)
        {
            Alternatives.Clear();
            this.Name = Name;

            // Split up name and add to Alternatives list
            foreach (string splitted in Name.Split(' '))
            {
                if (splitted.Equals(' '))
                    continue;
                Alternatives.Add(splitted);
            }
        }

        // Returns true if the parameter is contained in the name of this driver
        public bool ContainsName(string str)
        {
            // Do lowercase compare
            str = str.ToLower();
            foreach (string alt in Alternatives)
            {
                if (alt.ToLower().Equals(str))
                    return true;
            }
            return false;
        }

        // Show Full Name in the listboxes
        public override string ToString()
        {
            return Name;
        }
    }
}
