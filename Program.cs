﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GTGHelper
{
    static class Program
    {
        public static readonly string VERSION = "0.4.5";
        public static readonly int VersionInt = 5;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            foreach (string str in args)
            {
                if (str.Equals("--proxy"))
                    Parser.UseProxy = true;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
