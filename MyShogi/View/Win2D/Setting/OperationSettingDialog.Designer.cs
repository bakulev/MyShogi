namespace MyShogi.View.Win2D.Setting
{
    partial class OperationSettingDialog
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.richSelector1 = new MyShogi.View.Win2D.Setting.RichSelector();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.richSelector4 = new MyShogi.View.Win2D.Setting.RichSelector();
            this.richSelector3 = new MyShogi.View.Win2D.Setting.RichSelector();
            this.richSelector2 = new MyShogi.View.Win2D.Setting.RichSelector();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.richSelector6 = new MyShogi.View.Win2D.Setting.RichSelector();
            this.richSelector7 = new MyShogi.View.Win2D.Setting.RichSelector();
            this.richSelector5 = new MyShogi.View.Win2D.Setting.RichSelector();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.richSelector8 = new MyShogi.View.Win2D.Setting.RichSelector();
            this.richSelector9 = new MyShogi.View.Win2D.Setting.RichSelector();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(831, 624);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richSelector1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(823, 598);
            this.tabPage1.TabIndex = 7;
            this.tabPage1.Text = "Piece";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // richSelector1
            // 
            this.richSelector1.GroupBoxTitle = "Is it possible to move the piece by dragging the mouse?";
            this.richSelector1.Location = new System.Drawing.Point(6, 6);
            this.richSelector1.Margin = new System.Windows.Forms.Padding(0);
            this.richSelector1.Name = "richSelector1";
            this.richSelector1.SelectionTexts = new string[] {
        "No,dragged_move_style_0.png,Movement of pieces by mouse drag is not allowed.",
        "Yes,dragged_move_style_1.png,Allows the movement of pieces by dragging the mouse."};
            this.richSelector1.Size = new System.Drawing.Size(812, 116);
            this.richSelector1.TabIndex = 3;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.richSelector4);
            this.tabPage2.Controls.Add(this.richSelector3);
            this.tabPage2.Controls.Add(this.richSelector2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(823, 598);
            this.tabPage2.TabIndex = 8;
            this.tabPage2.Text = "Game record";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // richSelector4
            // 
            this.richSelector4.GroupBoxTitle = "Key corresponding to the first / last advance of the game record";
            this.richSelector4.Location = new System.Drawing.Point(6, 122);
            this.richSelector4.Margin = new System.Windows.Forms.Padding(0);
            this.richSelector4.Name = "richSelector4";
            this.richSelector4.SelectionTexts = new string[] {
        "None,kifu_firstlastkey_0.png, None.",
        "Assign to ← and →,kifu_firstlastkey_1.png, cursor keys ← and →. If it is duplica" +
            "ted with the key corresponding to one move forward / back in the game record, it" +
            " will be invalid.",
        "Assign to ↑ and ↓,kifu_firstlastkey_2.png, cursor keys ↑ and ↓. If it is duplica" +
            "ted with the key corresponding to one move forward / back in the game record, it" +
            " will be invalid.",
        "Assign to Page,kifu_firstlastkey_3.png, PageUp and PageDown."};
            this.richSelector4.Size = new System.Drawing.Size(812, 116);
            this.richSelector4.TabIndex = 6;
            // 
            // richSelector3
            // 
            this.richSelector3.GroupBoxTitle = "Special key corresponding to one step of the game record";
            this.richSelector3.Location = new System.Drawing.Point(6, 238);
            this.richSelector3.Margin = new System.Windows.Forms.Padding(0);
            this.richSelector3.Name = "richSelector3";
            this.richSelector3.SelectionTexts = new string[] {
        "None,kifu_next_specialkey_0.png, None.",
        "Space,kifu_next_specialkey_1.png, Assign to space key.",
        "Assign to Enter,kifu_next_specialkey_2.png, Enter key."};
            this.richSelector3.Size = new System.Drawing.Size(812, 116);
            this.richSelector3.TabIndex = 5;
            // 
            // richSelector2
            // 
            this.richSelector2.GroupBoxTitle = "Key corresponding to one move forward / back of the game record";
            this.richSelector2.Location = new System.Drawing.Point(6, 6);
            this.richSelector2.Margin = new System.Windows.Forms.Padding(0);
            this.richSelector2.Name = "richSelector2";
            this.richSelector2.SelectionTexts = new string[] {
        "None,kifu_prevnextkey_0.png, none",
        "← and →,kifu_prevnextkey_1.png, Assign to the left and right of the cursor keys." +
            "",
        "↑ and ↓,kifu_prevnextkey_2.png, Assign above and below the cursor keys."};
            this.richSelector2.Size = new System.Drawing.Size(812, 116);
            this.richSelector2.TabIndex = 4;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.richSelector6);
            this.tabPage3.Controls.Add(this.richSelector7);
            this.tabPage3.Controls.Add(this.richSelector5);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(823, 598);
            this.tabPage3.TabIndex = 9;
            this.tabPage3.Text = "examination";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // richSelector6
            // 
            this.richSelector6.GroupBoxTitle = "Reflect the selected line on the mini board in the review window";
            this.richSelector6.Location = new System.Drawing.Point(6, 238);
            this.richSelector6.Margin = new System.Windows.Forms.Padding(0);
            this.richSelector6.Name = "richSelector6";
            this.richSelector6.SelectionTexts = new string[] {
        "None,cons_sendpv_key_0.png, none.",
        "Assign to space,cons_sendpv_key_1.png, space key. If it is duplicated with the k" +
            "ey of the game record operation, it will be invalidated.",
        "Assign to Enter,cons_sendpv_key_2.png, Enter key. If it is duplicated with the k" +
            "ey of the game record operation, it will be invalidated."};
            this.richSelector6.Size = new System.Drawing.Size(812, 116);
            this.richSelector6.TabIndex = 5;
            // 
            // richSelector7
            // 
            this.richSelector7.GroupBoxTitle = "Move the beginning / end of the selected line in the review window";
            this.richSelector7.Location = new System.Drawing.Point(6, 122);
            this.richSelector7.Margin = new System.Windows.Forms.Padding(0);
            this.richSelector7.Name = "richSelector7";
            this.richSelector7.SelectionTexts = new string[] {
        "None,cons_headtailkey_0.png, none",
        "Shift ← →,cons_headtailkey_1.png, Shift + Assign to the left and right of the cu" +
            "rsor key.",
        "Shift ↑ ↓,cons_headtailkey_2.png, Shift + Assign to the top and bottom of the cu" +
            "rsor keys.",
        "← and →,cons_headtailkey_3.png, Assign to the left and right of the cursor keys." +
            " If it is duplicated with the key of the game record operation, it will be inval" +
            "idated.",
        "↑ and ↓,cons_headtailkey_4.png, assign to the top and bottom of the cursor keys." +
            " If it is duplicated with the key of the game record operation, it will be inval" +
            "idated.",
        "(comma) When (dot) ,cons_headtailkey_5.png, (comma) and (dot) Assign to (period).",
        "Assign to Page,cons_headtailkey_6.png, PageUp and PageDown. If it is duplicated " +
            "with the key of the game record operation, it will be invalidated."};
            this.richSelector7.Size = new System.Drawing.Size(812, 116);
            this.richSelector7.TabIndex = 4;
            // 
            // richSelector5
            // 
            this.richSelector5.GroupBoxTitle = "Move selected line up and down in the review window";
            this.richSelector5.Location = new System.Drawing.Point(6, 6);
            this.richSelector5.Margin = new System.Windows.Forms.Padding(0);
            this.richSelector5.Name = "richSelector5";
            this.richSelector5.SelectionTexts = new string[] {
        "None,cons_prevnextkey_0.png, none",
        "Shift ← →,cons_prevnextkey_1.png, Shift + Assign to the left and right of the cu" +
            "rsor key.",
        "Shift ↑ ↓,cons_prevnextkey_2.png, Shift + Assign to the top and bottom of the cu" +
            "rsor keys.",
        "← and →,cons_prevnextkey_3.png, Assign to the left and right of the cursor keys." +
            " If it is duplicated with the key of the game record operation, it will be inval" +
            "idated.",
        "↑ and ↓,cons_prevnextkey_4.png, assign to the top and bottom of the cursor keys." +
            " If it is duplicated with the key of the game record operation, it will be inval" +
            "idated.",
        "(comma) When (dot) ,cons_prevnextkey_5.png, (comma) and (dot) Assign to (period).",
        "Assign to Page,cons_prevnextkey_6.png, PageUp and PageDown. If it is duplicated " +
            "with the key of the game record operation, it will be invalidated."};
            this.richSelector5.Size = new System.Drawing.Size(812, 116);
            this.richSelector5.TabIndex = 4;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.richSelector8);
            this.tabPage4.Controls.Add(this.richSelector9);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(823, 598);
            this.tabPage4.TabIndex = 10;
            this.tabPage4.Text = "Mini board";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // richSelector8
            // 
            this.richSelector8.GroupBoxTitle = "Move to the beginning / end on the mini board";
            this.richSelector8.Location = new System.Drawing.Point(6, 122);
            this.richSelector8.Margin = new System.Windows.Forms.Padding(0);
            this.richSelector8.Name = "richSelector8";
            this.richSelector8.SelectionTexts = new string[] {
        "None,mini_headtailkey_0.png, none",
        "Ctrl ← →,mini_headtailkey_1.png, Ctrl + Assign to the left and right of the curs" +
            "or key.",
        "Ctrl ↑ ↓,mini_headtailkey_2.png, Ctrl + Assign to the top and bottom of the curs" +
            "or keys.",
        "← and →,mini_headtailkey_3.png, Assign to the left and right of the cursor key. " +
            "If it is duplicated with the key of the game record operation, it will be invali" +
            "dated.",
        "↑ and ↓,mini_headtailkey_4.png, Assign above and below the cursor keys. If it is" +
            " duplicated with the key of the game record operation, it will be invalidated.",
        "(comma) When (dot) ,mini_headtailkey_5.png ,(comma) and (dot) Assign to (period). If it is dup" +
            "licated with the key of the game record operation, it will be invalidated.",
        "Assign to Page,mini_headtailkey_6.png, PageUp and PageDown. If it is duplicated " +
            "with the key of the game record operation, it will be invalidated."};
            this.richSelector8.Size = new System.Drawing.Size(812, 116);
            this.richSelector8.TabIndex = 5;
            // 
            // richSelector9
            // 
            this.richSelector9.GroupBoxTitle = "Go back / forward on the mini board";
            this.richSelector9.Location = new System.Drawing.Point(6, 6);
            this.richSelector9.Margin = new System.Windows.Forms.Padding(0);
            this.richSelector9.Name = "richSelector9";
            this.richSelector9.SelectionTexts = new string[] {
        "None,mini_prevnextkey_0.png, none",
        "Ctrl ← →,mini_prevnextkey_1.png, Ctrl + Assign to the left and right of the curs" +
            "or key.",
        "Ctrl ↑ ↓,mini_prevnextkey_2.png, Ctrl + Assign to the top and bottom of the curs" +
            "or keys.",
        "← and →,mini_prevnextkey_3.png, Assign to the left and right of the cursor keys." +
            " If it is duplicated with the key of the game record operation, it will be inval" +
            "idated.",
        "↑ and ↓,mini_prevnextkey_4.png, Assign above and below the cursor keys. If it is" +
            " duplicated with the key of the game record operation, it will be invalidated.",
        "(comma) When (dot) ,mini_prevnextkey_5.png ,(comma) and (dot) Assign to (period). If it is dup" +
            "licated with the key of the game record operation, it will be invalidated.",
        "Assign to Page,cons_prevnextkey_6.png, PageUp and PageDown. If it is duplicated " +
            "with the key of the game record operation, it will be invalidated."};
            this.richSelector9.Size = new System.Drawing.Size(812, 116);
            this.richSelector9.TabIndex = 6;
            // 
            // OperationSettingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(101F, 101F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(831, 624);
            this.Controls.Add(this.tabControl1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OperationSettingDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Operation setting dialog";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private RichSelector richSelector1;
        private System.Windows.Forms.TabPage tabPage2;
        private RichSelector richSelector2;
        private RichSelector richSelector3;
        private RichSelector richSelector4;
        private System.Windows.Forms.TabPage tabPage3;
        private RichSelector richSelector5;
        private RichSelector richSelector6;
        private RichSelector richSelector7;
        private System.Windows.Forms.TabPage tabPage4;
        private RichSelector richSelector8;
        private RichSelector richSelector9;
    }
}
