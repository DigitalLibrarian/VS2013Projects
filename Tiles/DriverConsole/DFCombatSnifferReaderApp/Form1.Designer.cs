namespace DfCombatSnifferReaderApp
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.StrikeTree = new System.Windows.Forms.TreeView();
            this.StrikesNodeDisplayListView = new System.Windows.Forms.ListView();
            this.StrikesSplitContainer = new System.Windows.Forms.SplitContainer();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.ReportLogTabPage = new System.Windows.Forms.TabPage();
            this.reportLogListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.StrikesTabPage = new System.Windows.Forms.TabPage();
            this.UnitsTabPage = new System.Windows.Forms.TabPage();
            this.UnitsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.UnitsTree = new System.Windows.Forms.TreeView();
            this.UnitsNodeDisplayListView = new System.Windows.Forms.ListView();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StrikesSplitContainer)).BeginInit();
            this.StrikesSplitContainer.Panel1.SuspendLayout();
            this.StrikesSplitContainer.Panel2.SuspendLayout();
            this.StrikesSplitContainer.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.ReportLogTabPage.SuspendLayout();
            this.StrikesTabPage.SuspendLayout();
            this.UnitsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UnitsSplitContainer)).BeginInit();
            this.UnitsSplitContainer.Panel1.SuspendLayout();
            this.UnitsSplitContainer.Panel2.SuspendLayout();
            this.UnitsSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1160, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // StrikeTree
            // 
            this.StrikeTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StrikeTree.FullRowSelect = true;
            this.StrikeTree.Location = new System.Drawing.Point(0, 0);
            this.StrikeTree.Name = "StrikeTree";
            this.StrikeTree.Size = new System.Drawing.Size(883, 516);
            this.StrikeTree.TabIndex = 1;
            this.StrikeTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.StrikeTree_AfterSelect);
            this.StrikeTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.StrikeTree_NodeMouseDoubleClick);
            this.StrikeTree.KeyUp += new System.Windows.Forms.KeyEventHandler(this.StrikeTree_KeyUp);
            // 
            // StrikesNodeDisplayListView
            // 
            this.StrikesNodeDisplayListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StrikesNodeDisplayListView.FullRowSelect = true;
            this.StrikesNodeDisplayListView.Location = new System.Drawing.Point(0, 0);
            this.StrikesNodeDisplayListView.Name = "StrikesNodeDisplayListView";
            this.StrikesNodeDisplayListView.Size = new System.Drawing.Size(240, 516);
            this.StrikesNodeDisplayListView.TabIndex = 2;
            this.StrikesNodeDisplayListView.UseCompatibleStateImageBehavior = false;
            this.StrikesNodeDisplayListView.View = System.Windows.Forms.View.List;
            // 
            // StrikesSplitContainer
            // 
            this.StrikesSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StrikesSplitContainer.Location = new System.Drawing.Point(6, 6);
            this.StrikesSplitContainer.Name = "StrikesSplitContainer";
            // 
            // StrikesSplitContainer.Panel1
            // 
            this.StrikesSplitContainer.Panel1.Controls.Add(this.StrikeTree);
            // 
            // StrikesSplitContainer.Panel2
            // 
            this.StrikesSplitContainer.Panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.StrikesSplitContainer.Panel2.Controls.Add(this.StrikesNodeDisplayListView);
            this.StrikesSplitContainer.Panel2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.StrikesSplitContainer.Size = new System.Drawing.Size(1138, 516);
            this.StrikesSplitContainer.SplitterDistance = 883;
            this.StrikesSplitContainer.SplitterWidth = 15;
            this.StrikesSplitContainer.TabIndex = 6;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(69, 24);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(348, 21);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Session: ";
            // 
            // TabControl
            // 
            this.TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl.Controls.Add(this.ReportLogTabPage);
            this.TabControl.Controls.Add(this.StrikesTabPage);
            this.TabControl.Controls.Add(this.UnitsTabPage);
            this.TabControl.Location = new System.Drawing.Point(0, 53);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(1160, 556);
            this.TabControl.TabIndex = 10;
            // 
            // ReportLogTabPage
            // 
            this.ReportLogTabPage.Controls.Add(this.reportLogListView);
            this.ReportLogTabPage.Location = new System.Drawing.Point(4, 22);
            this.ReportLogTabPage.Name = "ReportLogTabPage";
            this.ReportLogTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ReportLogTabPage.Size = new System.Drawing.Size(1152, 530);
            this.ReportLogTabPage.TabIndex = 0;
            this.ReportLogTabPage.Text = "Report Log";
            this.ReportLogTabPage.UseVisualStyleBackColor = true;
            // 
            // reportLogListView
            // 
            this.reportLogListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.reportLogListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportLogListView.FullRowSelect = true;
            this.reportLogListView.Location = new System.Drawing.Point(3, 3);
            this.reportLogListView.MultiSelect = false;
            this.reportLogListView.Name = "reportLogListView";
            this.reportLogListView.Size = new System.Drawing.Size(1146, 524);
            this.reportLogListView.TabIndex = 0;
            this.reportLogListView.UseCompatibleStateImageBehavior = false;
            this.reportLogListView.View = System.Windows.Forms.View.Details;
            this.reportLogListView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.reportLogListView_KeyUp);
            this.reportLogListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.reportLogListView_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Text";
            this.columnHeader1.Width = 800;
            // 
            // StrikesTabPage
            // 
            this.StrikesTabPage.Controls.Add(this.StrikesSplitContainer);
            this.StrikesTabPage.Location = new System.Drawing.Point(4, 22);
            this.StrikesTabPage.Name = "StrikesTabPage";
            this.StrikesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.StrikesTabPage.Size = new System.Drawing.Size(1152, 530);
            this.StrikesTabPage.TabIndex = 1;
            this.StrikesTabPage.Text = "Strikes";
            this.StrikesTabPage.UseVisualStyleBackColor = true;
            // 
            // UnitsTabPage
            // 
            this.UnitsTabPage.Controls.Add(this.UnitsSplitContainer);
            this.UnitsTabPage.Location = new System.Drawing.Point(4, 22);
            this.UnitsTabPage.Name = "UnitsTabPage";
            this.UnitsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.UnitsTabPage.Size = new System.Drawing.Size(1152, 530);
            this.UnitsTabPage.TabIndex = 2;
            this.UnitsTabPage.Text = "Units";
            this.UnitsTabPage.UseVisualStyleBackColor = true;
            // 
            // UnitsSplitContainer
            // 
            this.UnitsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UnitsSplitContainer.Location = new System.Drawing.Point(3, 3);
            this.UnitsSplitContainer.Name = "UnitsSplitContainer";
            // 
            // UnitsSplitContainer.Panel1
            // 
            this.UnitsSplitContainer.Panel1.Controls.Add(this.UnitsTree);
            // 
            // UnitsSplitContainer.Panel2
            // 
            this.UnitsSplitContainer.Panel2.Controls.Add(this.UnitsNodeDisplayListView);
            this.UnitsSplitContainer.Size = new System.Drawing.Size(1146, 524);
            this.UnitsSplitContainer.SplitterDistance = 881;
            this.UnitsSplitContainer.SplitterWidth = 15;
            this.UnitsSplitContainer.TabIndex = 0;
            // 
            // UnitsTree
            // 
            this.UnitsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UnitsTree.Location = new System.Drawing.Point(0, 0);
            this.UnitsTree.Name = "UnitsTree";
            this.UnitsTree.Size = new System.Drawing.Size(881, 524);
            this.UnitsTree.TabIndex = 0;
            this.UnitsTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.UnitsTree_AfterSelect);
            // 
            // UnitsNodeDisplayListView
            // 
            this.UnitsNodeDisplayListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UnitsNodeDisplayListView.Location = new System.Drawing.Point(0, 0);
            this.UnitsNodeDisplayListView.Name = "UnitsNodeDisplayListView";
            this.UnitsNodeDisplayListView.Size = new System.Drawing.Size(250, 524);
            this.UnitsNodeDisplayListView.TabIndex = 0;
            this.UnitsNodeDisplayListView.UseCompatibleStateImageBehavior = false;
            this.UnitsNodeDisplayListView.View = System.Windows.Forms.View.List;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1160, 609);
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Df Combat Sniffer Log Viewer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.StrikesSplitContainer.Panel1.ResumeLayout(false);
            this.StrikesSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.StrikesSplitContainer)).EndInit();
            this.StrikesSplitContainer.ResumeLayout(false);
            this.TabControl.ResumeLayout(false);
            this.ReportLogTabPage.ResumeLayout(false);
            this.StrikesTabPage.ResumeLayout(false);
            this.UnitsTabPage.ResumeLayout(false);
            this.UnitsSplitContainer.Panel1.ResumeLayout(false);
            this.UnitsSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UnitsSplitContainer)).EndInit();
            this.UnitsSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TreeView StrikeTree;
        private System.Windows.Forms.ListView StrikesNodeDisplayListView;
        private System.Windows.Forms.SplitContainer StrikesSplitContainer;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage ReportLogTabPage;
        private System.Windows.Forms.TabPage StrikesTabPage;
        private System.Windows.Forms.ListView reportLogListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TabPage UnitsTabPage;
        private System.Windows.Forms.SplitContainer UnitsSplitContainer;
        private System.Windows.Forms.TreeView UnitsTree;
        private System.Windows.Forms.ListView UnitsNodeDisplayListView;
    }
}

