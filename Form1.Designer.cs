namespace GTGHelper
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.driverlist = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.racewinners = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelNonPred = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelPred = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(118, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(587, 20);
            this.textBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Read Comments";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(298, 38);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(407, 384);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // driverlist
            // 
            this.driverlist.FormattingEnabled = true;
            this.driverlist.Items.AddRange(new object[] {
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
            "Karun Chandhok ",
            "Bruno Senna",
            "Pedro de la Rosa",
            "Kamui Kobayashi",
            "Timo Glock",
            "Lucas di Grassi"});
            this.driverlist.Location = new System.Drawing.Point(6, 32);
            this.driverlist.Name = "driverlist";
            this.driverlist.Size = new System.Drawing.Size(116, 134);
            this.driverlist.TabIndex = 3;
            this.driverlist.DoubleClick += new System.EventHandler(this.SelectDriver);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Driver List";
            // 
            // racewinners
            // 
            this.racewinners.FormattingEnabled = true;
            this.racewinners.Location = new System.Drawing.Point(159, 32);
            this.racewinners.Name = "racewinners";
            this.racewinners.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.racewinners.Size = new System.Drawing.Size(120, 134);
            this.racewinners.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(125, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Race Result:";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(116, 170);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(60, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "Reset";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Clear_Click);
            // 
            // listBox1
            // 
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox1.Enabled = false;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "P1",
            "P2",
            "P3",
            "P4",
            "P5",
            "P6",
            "P7",
            "P8",
            "P9",
            "P10"});
            this.listBox1.Location = new System.Drawing.Point(128, 32);
            this.listBox1.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBox1.Size = new System.Drawing.Size(28, 132);
            this.listBox1.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Controls.Add(this.driverlist);
            this.groupBox1.Controls.Add(this.racewinners);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(286, 204);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Race Results...";
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(182, 170);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(97, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "Calculate Points";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelNonPred);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.labelPred);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(6, 248);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(286, 85);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Info";
            // 
            // labelNonPred
            // 
            this.labelNonPred.AutoSize = true;
            this.labelNonPred.Location = new System.Drawing.Point(125, 29);
            this.labelNonPred.Name = "labelNonPred";
            this.labelNonPred.Size = new System.Drawing.Size(27, 13);
            this.labelNonPred.TabIndex = 3;
            this.labelNonPred.Text = "N/A";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Non-Predictions found:";
            // 
            // labelPred
            // 
            this.labelPred.AutoSize = true;
            this.labelPred.Location = new System.Drawing.Point(127, 16);
            this.labelPred.Name = "labelPred";
            this.labelPred.Size = new System.Drawing.Size(27, 13);
            this.labelPred.TabIndex = 1;
            this.labelPred.Text = "N/A";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Predictions found:";
            // 
            // Form1
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 434);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(438, 382);
            this.Name = "Form1";
            this.Text = "GTGHelper 0.1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ListBox driverlist;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox racewinners;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelPred;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelNonPred;
        private System.Windows.Forms.Label label4;
    }
}

