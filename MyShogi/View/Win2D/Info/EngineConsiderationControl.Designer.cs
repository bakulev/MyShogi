namespace MyShogi.View.Win2D
{
    partial class EngineConsiderationControl
    {
        /// <summary> 
        /// Required designer variables.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up all resources in use.
        /// </summary>
        /// <param name="disposing">Specify true if you want to destroy the managed resource, false otherwise.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// This method is required for designer support. The contents of this method
        /// Do not change it in the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SendToMainKifu = new System.Windows.Forms.ToolStripMenuItem();
            this.ReplaceMainKifu = new System.Windows.Forms.ToolStripMenuItem();
            this.PastePvToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteKifToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new MyShogi.View.Win2D.ListViewEx();
            this.toolTip1 = new MyShogi.View.Win2D.ToolTipEx(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(2, 2);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(185, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.textBox1, "The engine name is displayed.");
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.White;
            this.textBox2.Location = new System.Drawing.Point(187, 2);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(144, 20);
            this.textBox2.TabIndex = 2;
            this.textBox2.TabStop = false;
            this.toolTip1.SetToolTip(this.textBox2, "This is the next move that the engine expects.");
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.White;
            this.textBox3.Location = new System.Drawing.Point(331, 2);
            this.textBox3.Margin = new System.Windows.Forms.Padding(2);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(163, 20);
            this.textBox3.TabIndex = 2;
            this.textBox3.TabStop = false;
            this.toolTip1.SetToolTip(this.textBox3, "The number of phases searched.");
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.Color.White;
            this.textBox4.Location = new System.Drawing.Point(496, 2);
            this.textBox4.Margin = new System.Windows.Forms.Padding(2);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(131, 20);
            this.textBox4.TabIndex = 2;
            this.textBox4.TabStop = false;
            this.toolTip1.SetToolTip(this.textBox4, "The number of search phases per second. (Nodes Per Second)");
            // 
            // textBox5
            // 
            this.textBox5.BackColor = System.Drawing.Color.White;
            this.textBox5.Location = new System.Drawing.Point(628, 2);
            this.textBox5.Margin = new System.Windows.Forms.Padding(2);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(131, 20);
            this.textBox5.TabIndex = 2;
            this.textBox5.TabStop = false;
            this.toolTip1.SetToolTip(this.textBox5, "HASH usage rate of the engine.");
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Candidate move 1 move",
            "2 candidate moves",
            "3 candidate moves",
            "4 candidate moves",
            "5 candidate moves",
            "6 candidate moves",
            "7 candidate moves",
            "8 candidate moves",
            "9 candidate moves",
            "10 candidate moves",
            "11 candidate moves",
            "12 candidate moves",
            "13 candidate moves",
            "14 candidate moves",
            "15 candidate moves"});
            this.comboBox1.Location = new System.Drawing.Point(2, 27);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(105, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.comboBox1, "The number of candidate moves to display.");
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(108, 27);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(58, 21);
            this.button1.TabIndex = 4;
            this.button1.Text = "Order of arrival";
            this.toolTip1.SetToolTip(this.button1, "If it is \"arrival order\", the readings will be displayed in the order sent from t" +
        "he engine.");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(17, 17);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SendToMainKifu,
            this.ReplaceMainKifu,
            this.PastePvToClipboard,
            this.PasteKifToClipboard});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(427, 92);
            // 
            // SendToMainKifu
            // 
            this.SendToMainKifu.Name = "SendToMainKifu";
            this.SendToMainKifu.Size = new System.Drawing.Size(426, 22);
            this.SendToMainKifu.Text = "Send this reading line to the main game record as a branch game record (& S)";
            this.SendToMainKifu.Click += new System.EventHandler(this.SendToMainKifu_Click);
            // 
            // ReplaceMainKifu
            // 
            this.ReplaceMainKifu.Name = "ReplaceMainKifu";
            this.ReplaceMainKifu.Size = new System.Drawing.Size(426, 22);
            this.ReplaceMainKifu.Text = "Replace the main game record with this reading line (& R)";
            this.ReplaceMainKifu.Click += new System.EventHandler(this.ReplaceMainKifu_Click);
            // 
            // PastePvToClipboard
            // 
            this.PastePvToClipboard.Name = "PastePvToClipboard";
            this.PastePvToClipboard.Size = new System.Drawing.Size(426, 22);
            this.PastePvToClipboard.Text = "Paste the reading line to the clipboard with the displayed character string (& P)";
            this.PastePvToClipboard.Click += new System.EventHandler(this.PastePvToClipboard_Click);
            // 
            // PasteKifToClipboard
            // 
            this.PasteKifToClipboard.Name = "PasteKifToClipboard";
            this.PasteKifToClipboard.Size = new System.Drawing.Size(426, 22);
            this.PasteKifToClipboard.Text = "Paste the reading line into the clipboard in KIF format (& K)";
            this.PasteKifToClipboard.Click += new System.EventHandler(this.PasteKifToClipboard_Click);
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 97);
            this.listView1.Margin = new System.Windows.Forms.Padding(2);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(801, 122);
            this.listView1.TabIndex = 1;
            this.listView1.TabStop = false;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listView1_ColumnWidthChanged);
            this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
            this.listView1.ClientSizeChanged += new System.EventHandler(this.listView1_ClientSizeChanged);
            this.listView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDown);
            // 
            // EngineConsiderationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(101F, 101F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listView1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "EngineConsiderationControl";
            this.Size = new System.Drawing.Size(804, 221);
            this.Resize += new System.EventHandler(this.EngineConsiderationControl_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyShogi.View.Win2D.ListViewEx listView1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button1;
        private MyShogi.View.Win2D.ToolTipEx toolTip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem SendToMainKifu;
        private System.Windows.Forms.ToolStripMenuItem ReplaceMainKifu;
        private System.Windows.Forms.ToolStripMenuItem PastePvToClipboard;
        private System.Windows.Forms.ToolStripMenuItem PasteKifToClipboard;
    }
}
