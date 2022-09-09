namespace Client
{
    public partial class Form : System.Windows.Forms.Form
    {
        public NetworkLogic m_NetworkLogic;
        private ClientSocket m_socket;

        public Form()
        {
            InitializeComponent();

            m_NetworkLogic = new NetworkLogic(this);
            m_socket = new ClientSocket(this);
            TimerDispatcher.Start();
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            if (false == m_socket.Connected)
            {
                if (false == m_socket.Connect(TextBoxIP.Text, int.Parse(TextBoxPort.Text)))
                {
                    MessageBox.Show($"IP={TextBoxIP.Text}, Port={TextBoxPort.Text}", "Connect Fail");
                    return;
                }
            }
            else
            {
                m_socket.Disconnect();
            }
        }

        private void ButtonIPFSReload_Click(object sender, EventArgs e)
        {
            m_socket.Send(new Packet.CtoS.ViewData());
        }

        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            TreeNode directoryNode = new TreeNode(directoryInfo.Name);

            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            }

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                directoryNode.Nodes.Add(new TreeNode(file.Name));
            }

            return directoryNode;
        }
        private void ButtonLocalPath_Click(object sender, EventArgs e)
        {
            if(DialogResult.OK != FolderBrowserDialog.ShowDialog())
            {
                return;
            }

            TreeViewLocal.Nodes.Clear();
            TreeViewLocal.Nodes.Add(CreateDirectoryNode(new DirectoryInfo(FolderBrowserDialog.SelectedPath)));

            TextBoxLocalPath.Text = FolderBrowserDialog.SelectedPath;
        }

        private void GetLeaf(TreeNode rootnode, string path, ref List<string> listFile)
        {
            if (0 < rootnode.Nodes.Count)
            {
                foreach (TreeNode node in rootnode.Nodes)
                {
                    GetLeaf(node, path + '/' + rootnode.Text, ref listFile);
                }
            }
            else
            {
                listFile.Add(path + "/" + rootnode.Text.Replace('\\', '/').Replace("//", "/"));
            }
        }
        private void ButtonUpload_Click(object sender, EventArgs e)
        {
            if(null == TreeViewIPFS.SelectedNode)
            {
                MessageBox.Show("IPFS Select Node Null", "TreeView IPFS", MessageBoxButtons.OK);
                return;
            }

            if (null == TreeViewLocal.SelectedNode)
            {
                MessageBox.Show("Local Select Node Null", "TreeView Local", MessageBoxButtons.OK);
                return;
            }

            string IPFSPath = TreeViewIPFS.SelectedNode.FullPath.Replace('\\', '/').Replace("//", "/");
            string LocalPath = TreeViewLocal.SelectedNode.FullPath.Replace('\\', '/').Replace("//", "/");

            if (DialogResult.Yes != MessageBox.Show($"{IPFSPath} <= {LocalPath}", "Upload", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2))
            {
                return;
            }

            TreeNode node = TreeViewLocal.SelectedNode.Parent;
            string SelectPath = "";
            while(null != node)
            {
                if(node.Nodes.Count > 0)
                {
                    SelectPath = '/' + node.Text + SelectPath;
                }

                node = node.Parent;
            }

            string[] tempSelectPath = LocalPath.Split('/');

            Packet.CtoS.Upload upload = new Packet.CtoS.Upload();
            upload.IPFSPath = IPFSPath;
            upload.LocalPath = TextBoxLocalPath.Text.Replace($"\\{TreeViewLocal.TopNode.Text}", "");
            upload.SelectPath =  SelectPath;
            List<string> listFile = new List<string>();
            GetLeaf(TreeViewLocal.SelectedNode, "", ref listFile);
            upload.listFile = listFile;

            m_socket.Send(upload);
        }

        private void TreeViewIPFS_MouseClick(object sender, MouseEventArgs e)
        {
            TreeNode trv = TreeViewIPFS.GetNodeAt(e.X, e.Y);
            TreeViewIPFS.SelectedNode = trv;

            m_NetworkLogic.GetViewData(trv.FullPath.Replace('\\', '/').Replace("//", "/"), out Packet.StoC.ViewData? viewdata);
            if(null != viewdata)
            {
                TextBoxCID.Text = viewdata.CID;
            }

            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip.Show(TreeViewIPFS, new Point(e.X, e.Y));
            }
        }

        private void TextBoxPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
            
            if(e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                ButtonConnect_Click(sender, e);
            }
        }

        private void ContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch(e.ClickedItem.Text)
            {
                case "Delete":
                    {
                        if(DialogResult.Yes == MessageBox.Show($"{TreeViewIPFS.SelectedNode.Text}을 삭제 하시겠습니까?", "Delete", MessageBoxButtons.YesNo))
                        {
                            m_socket.Send(new Packet.CtoS.Delete()
                            {
                                Path = TreeViewIPFS.SelectedNode.FullPath.Replace('\\', '/').Replace("//", "/")
                            });
                        }
                    }
                    break;
            }
        }

        private void TimerDispatcher_Tick(object sender, EventArgs e)
        {
            if (false == m_socket.PopEvent(out SocketEventInfo eventinfo))
            {
                return;
            }

            switch (eventinfo.Event)
            {
                case SocketEventInfo.EventType.OnError:
                    {
                        if(null == eventinfo.ErrorType || null == eventinfo.Exception)
                        {
                            break;
                        }

                        MessageBox.Show($"ErrorType={eventinfo.ErrorType}\r\nMessage={eventinfo.Exception.Message}", "Network Error");
                    }
                    break;
                case SocketEventInfo.EventType.OnConnect:
                    {
                        TextBoxIP.ReadOnly = true;
                        TextBoxPort.ReadOnly = true;
                        ButtonConnect.Text = "연결 끊기";

                        m_socket.Send(new Packet.CtoS.ViewData());
                    }
                    break;
                case SocketEventInfo.EventType.OnDisconnect:
                    {
                        TextBoxIP.ReadOnly = false;
                        TextBoxIP.Text = "";
                        TextBoxPort.ReadOnly = false;
                        TextBoxPort.Text = "";
                        ButtonConnect.Text = "연결";
                    }
                    break;
                case SocketEventInfo.EventType.OnSend:
                    {

                    }
                    break;
                case SocketEventInfo.EventType.OnReceive:
                    {
                        if(null == eventinfo.Packet)
                        {
                            break;
                        }

                        ServerModule.Network.FunctionBase<ClientSocket, Packet.Define.PacketIndex>? func_handler;
                        Type? packet_type = null;
                        if (false == m_socket.Dispatcher.GetFunction(eventinfo.PacketIndex, out func_handler, out packet_type))
                        {
                            return;
                        }
                        
                        if(null == func_handler ||
                            null == packet_type)
                        {
                            return;
                        }

                        object? packet = Activator.CreateInstance(packet_type);
                        if(false == m_socket.Converter.Deserialize(eventinfo.Packet, packet_type, out packet))
                        {
                            return;
                        }
                        
                        if(null == packet)
                        {
                            return;
                        }

                        func_handler.ExecuteFunction(m_socket, (ServerModule.Network.PacketBase<Packet.Define.PacketIndex>)packet);
                    }
                    break;
            }
        }
    }
}
