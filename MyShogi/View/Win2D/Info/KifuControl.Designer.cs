namespace MyShogi.View.Win2D
{
    partial class KifuControl
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.listView1 = new MyShogi.View.Win2D.ListViewEx();
            this.toolTip1 = new MyShogi.View.Win2D.ToolTipEx(this.components);
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 108);
            this.button1.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(54, 27);
            this.button1.TabIndex = 1;
            this.button1.TabStop = false;
            this.button1.Text = "Main score";
            this.toolTip1.SetToolTip(this.button1, "Return to the procedure of the main score.");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(118, 108);
            this.button3.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(59, 27);
            this.button3.TabIndex = 3;
            this.button3.TabStop = false;
            this.button3.Text = "No branch";
            this.toolTip1.SetToolTip(this.button3, "Delete this branch.");
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(61, 108);
            this.button2.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(54, 27);
            this.button2.TabIndex = 2;
            this.button2.TabStop = false;
            this.button2.Text = "Next branch";
            this.toolTip1.SetToolTip(this.button2, "Proceed to the next branch.");
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(179, 108);
            this.button4.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(59, 27);
            this.button4.TabIndex = 4;
            this.button4.TabStop = false;
            this.button4.Text = "One move";
            this.toolTip1.SetToolTip(this.button4, "Remove the last move in this change.");
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(240, 108);
            this.button5.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(24, 27);
            this.button5.TabIndex = 3;
            this.button5.TabStop = false;
            this.button5.Text = "＋";
            this.toolTip1.SetToolTip(this.button5, "Make the letters a little bigger.");
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(266, 108);
            this.button6.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(24, 27);
            this.button6.TabIndex = 3;
            this.button6.TabStop = false;
            this.button6.Text = "－";
            this.toolTip1.SetToolTip(this.button6, "Make the letters smaller.");
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(8, 4);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.OwnerDraw = true;
            this.listView1.Size = new System.Drawing.Size(282, 102);
            this.listView1.TabIndex = 4;
            this.listView1.TabStop = false;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listView1_ColumnWidthChanged);
            this.listView1.ClientSizeChanged += new System.EventHandler(this.listView1_ClientSizeChanged);
            // 
            // KifuControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(101F, 101F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.Name = "KifuControl";
            this.Size = new System.Drawing.Size(315, 148);
            this.SizeChanged += new System.EventHandler(this.KifuControl_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private MyShogi.View.Win2D.ToolTipEx toolTip1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private MyShogi.View.Win2D.ListViewEx listView1;
    }
}
