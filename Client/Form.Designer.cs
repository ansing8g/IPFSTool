namespace Client
{
    partial class Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.GroupBoxIPFS = new System.Windows.Forms.GroupBox();
            this.TreeViewIPFS = new System.Windows.Forms.TreeView();
            this.GroupBoxLocal = new System.Windows.Forms.GroupBox();
            this.TreeViewLocal = new System.Windows.Forms.TreeView();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.ButtonLocalPath = new System.Windows.Forms.Button();
            this.ButtonUpload = new System.Windows.Forms.Button();
            this.LabelIP = new System.Windows.Forms.Label();
            this.TextBoxIP = new System.Windows.Forms.TextBox();
            this.TextBoxPort = new System.Windows.Forms.TextBox();
            this.LabelPort = new System.Windows.Forms.Label();
            this.ButtonConnect = new System.Windows.Forms.Button();
            this.ButtonIPFSReload = new System.Windows.Forms.Button();
            this.LabelCID = new System.Windows.Forms.Label();
            this.TextBoxCID = new System.Windows.Forms.TextBox();
            this.TextBoxUpload = new System.Windows.Forms.TextBox();
            this.TextBoxLocalPath = new System.Windows.Forms.TextBox();
            this.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.TimerDispatcher = new System.Windows.Forms.Timer(this.components);
            this.GroupBoxIPFS.SuspendLayout();
            this.GroupBoxLocal.SuspendLayout();
            this.ContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBoxIPFS
            // 
            this.GroupBoxIPFS.Controls.Add(this.TreeViewIPFS);
            this.GroupBoxIPFS.Location = new System.Drawing.Point(0, 70);
            this.GroupBoxIPFS.Name = "GroupBoxIPFS";
            this.GroupBoxIPFS.Size = new System.Drawing.Size(730, 750);
            this.GroupBoxIPFS.TabIndex = 9;
            this.GroupBoxIPFS.TabStop = false;
            this.GroupBoxIPFS.Text = "IPFS";
            // 
            // TreeViewIPFS
            // 
            this.TreeViewIPFS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewIPFS.Location = new System.Drawing.Point(3, 35);
            this.TreeViewIPFS.Name = "TreeViewIPFS";
            this.TreeViewIPFS.Size = new System.Drawing.Size(724, 712);
            this.TreeViewIPFS.TabIndex = 3;
            this.TreeViewIPFS.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TreeViewIPFS_MouseClick);
            // 
            // GroupBoxLocal
            // 
            this.GroupBoxLocal.Controls.Add(this.TreeViewLocal);
            this.GroupBoxLocal.Location = new System.Drawing.Point(740, 70);
            this.GroupBoxLocal.Name = "GroupBoxLocal";
            this.GroupBoxLocal.Size = new System.Drawing.Size(730, 747);
            this.GroupBoxLocal.TabIndex = 9;
            this.GroupBoxLocal.TabStop = false;
            this.GroupBoxLocal.Text = "Local";
            // 
            // TreeViewLocal
            // 
            this.TreeViewLocal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewLocal.Location = new System.Drawing.Point(3, 35);
            this.TreeViewLocal.Name = "TreeViewLocal";
            this.TreeViewLocal.Size = new System.Drawing.Size(724, 709);
            this.TreeViewLocal.TabIndex = 6;
            // 
            // ButtonLocalPath
            // 
            this.ButtonLocalPath.Location = new System.Drawing.Point(743, 822);
            this.ButtonLocalPath.Name = "ButtonLocalPath";
            this.ButtonLocalPath.Size = new System.Drawing.Size(137, 46);
            this.ButtonLocalPath.TabIndex = 7;
            this.ButtonLocalPath.Text = "로컬 경로";
            this.ButtonLocalPath.UseVisualStyleBackColor = true;
            this.ButtonLocalPath.Click += new System.EventHandler(this.ButtonLocalPath_Click);
            // 
            // ButtonUpload
            // 
            this.ButtonUpload.Location = new System.Drawing.Point(743, 870);
            this.ButtonUpload.Name = "ButtonUpload";
            this.ButtonUpload.Size = new System.Drawing.Size(103, 46);
            this.ButtonUpload.TabIndex = 9;
            this.ButtonUpload.Text = "업로드";
            this.ButtonUpload.UseVisualStyleBackColor = true;
            this.ButtonUpload.Click += new System.EventHandler(this.ButtonUpload_Click);
            // 
            // LabelIP
            // 
            this.LabelIP.AutoSize = true;
            this.LabelIP.Location = new System.Drawing.Point(15, 12);
            this.LabelIP.Name = "LabelIP";
            this.LabelIP.Size = new System.Drawing.Size(34, 32);
            this.LabelIP.TabIndex = 9;
            this.LabelIP.Text = "IP";
            // 
            // TextBoxIP
            // 
            this.TextBoxIP.Location = new System.Drawing.Point(55, 9);
            this.TextBoxIP.Name = "TextBoxIP";
            this.TextBoxIP.Size = new System.Drawing.Size(343, 39);
            this.TextBoxIP.TabIndex = 0;
            // 
            // TextBoxPort
            // 
            this.TextBoxPort.Location = new System.Drawing.Point(500, 9);
            this.TextBoxPort.Name = "TextBoxPort";
            this.TextBoxPort.Size = new System.Drawing.Size(229, 39);
            this.TextBoxPort.TabIndex = 1;
            this.TextBoxPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxPort_KeyPress);
            // 
            // LabelPort
            // 
            this.LabelPort.AutoSize = true;
            this.LabelPort.Location = new System.Drawing.Point(437, 12);
            this.LabelPort.Name = "LabelPort";
            this.LabelPort.Size = new System.Drawing.Size(58, 32);
            this.LabelPort.TabIndex = 9;
            this.LabelPort.Text = "Port";
            // 
            // ButtonConnect
            // 
            this.ButtonConnect.Location = new System.Drawing.Point(770, 5);
            this.ButtonConnect.Name = "ButtonConnect";
            this.ButtonConnect.Size = new System.Drawing.Size(204, 46);
            this.ButtonConnect.TabIndex = 2;
            this.ButtonConnect.Text = "연결";
            this.ButtonConnect.UseVisualStyleBackColor = true;
            this.ButtonConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
            // 
            // ButtonIPFSReload
            // 
            this.ButtonIPFSReload.Location = new System.Drawing.Point(12, 871);
            this.ButtonIPFSReload.Name = "ButtonIPFSReload";
            this.ButtonIPFSReload.Size = new System.Drawing.Size(319, 46);
            this.ButtonIPFSReload.TabIndex = 5;
            this.ButtonIPFSReload.Text = "IPFS 정보 다시 가져오기";
            this.ButtonIPFSReload.UseVisualStyleBackColor = true;
            this.ButtonIPFSReload.Click += new System.EventHandler(this.ButtonIPFSReload_Click);
            // 
            // LabelCID
            // 
            this.LabelCID.AutoSize = true;
            this.LabelCID.Location = new System.Drawing.Point(5, 829);
            this.LabelCID.Name = "LabelCID";
            this.LabelCID.Size = new System.Drawing.Size(52, 32);
            this.LabelCID.TabIndex = 9;
            this.LabelCID.Text = "CID";
            // 
            // TextBoxCID
            // 
            this.TextBoxCID.Location = new System.Drawing.Point(68, 826);
            this.TextBoxCID.Name = "TextBoxCID";
            this.TextBoxCID.ReadOnly = true;
            this.TextBoxCID.Size = new System.Drawing.Size(662, 39);
            this.TextBoxCID.TabIndex = 4;
            // 
            // TextBoxUpload
            // 
            this.TextBoxUpload.Location = new System.Drawing.Point(852, 874);
            this.TextBoxUpload.Name = "TextBoxUpload";
            this.TextBoxUpload.ReadOnly = true;
            this.TextBoxUpload.Size = new System.Drawing.Size(618, 39);
            this.TextBoxUpload.TabIndex = 10;
            // 
            // TextBoxLocalPath
            // 
            this.TextBoxLocalPath.Location = new System.Drawing.Point(886, 825);
            this.TextBoxLocalPath.Name = "TextBoxLocalPath";
            this.TextBoxLocalPath.ReadOnly = true;
            this.TextBoxLocalPath.Size = new System.Drawing.Size(584, 39);
            this.TextBoxLocalPath.TabIndex = 8;
            // 
            // ContextMenuStrip
            // 
            this.ContextMenuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemDelete});
            this.ContextMenuStrip.Name = "contextMenuStrip1";
            this.ContextMenuStrip.Size = new System.Drawing.Size(159, 42);
            this.ContextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ContextMenuStrip_ItemClicked);
            // 
            // MenuItemDelete
            // 
            this.MenuItemDelete.Name = "MenuItemDelete";
            this.MenuItemDelete.Size = new System.Drawing.Size(158, 38);
            this.MenuItemDelete.Text = "Delete";
            // 
            // TimerDispatcher
            // 
            this.TimerDispatcher.Interval = 1;
            this.TimerDispatcher.Tick += new System.EventHandler(this.TimerDispatcher_Tick);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1474, 929);
            this.Controls.Add(this.TextBoxLocalPath);
            this.Controls.Add(this.TextBoxUpload);
            this.Controls.Add(this.TextBoxCID);
            this.Controls.Add(this.LabelCID);
            this.Controls.Add(this.ButtonIPFSReload);
            this.Controls.Add(this.ButtonConnect);
            this.Controls.Add(this.TextBoxPort);
            this.Controls.Add(this.LabelPort);
            this.Controls.Add(this.TextBoxIP);
            this.Controls.Add(this.LabelIP);
            this.Controls.Add(this.ButtonUpload);
            this.Controls.Add(this.ButtonLocalPath);
            this.Controls.Add(this.GroupBoxLocal);
            this.Controls.Add(this.GroupBoxIPFS);
            this.MaximizeBox = false;
            this.Name = "Form";
            this.Text = "IPFSTool Client";
            this.GroupBoxIPFS.ResumeLayout(false);
            this.GroupBoxLocal.ResumeLayout(false);
            this.ContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public GroupBox GroupBoxIPFS;
        public GroupBox GroupBoxLocal;
        public TreeView TreeViewIPFS;
        public TreeView TreeViewLocal;
        public FolderBrowserDialog FolderBrowserDialog;
        public Button ButtonLocalPath;
        public Button ButtonUpload;
        public Label LabelIP;
        public TextBox TextBoxIP;
        public TextBox TextBoxPort;
        public Label LabelPort;
        public Button ButtonConnect;
        public Button ButtonIPFSReload;
        public Label LabelCID;
        public TextBox TextBoxCID;
        public TextBox TextBoxUpload;
        public TextBox TextBoxLocalPath;
        public ContextMenuStrip ContextMenuStrip;
        public ToolStripMenuItem MenuItemDelete;
        public System.Windows.Forms.Timer TimerDispatcher;
    }
}