
namespace Client
{
    public class NetworkLogic
    {
        public NetworkLogic(Form _From)
        {
            this.m_from = _From;
            this.m_dicViewData = new Dictionary<string, Packet.StoC.ViewData>();
        }

        private Form m_from;
        private Dictionary<string, Packet.StoC.ViewData> m_dicViewData;

        public void GetViewData(string path, out Packet.StoC.ViewData? viewdata)
        {
            viewdata = null;

            if (false == m_dicViewData.ContainsKey(path)) return;

            viewdata = m_dicViewData[path];
        }

        public void KeepAlive(ClientSocket _socket, Packet.StoC.KeepAlive _recvdata)
        {
            _socket.Send(new Packet.CtoS.KeepAlive());
        }

        private TreeNode CreateViewData(Packet.StoC.ViewData _data, string path)
        {
            TreeNode new_node = new TreeNode(_data.Name);

            foreach (Packet.StoC.ViewData leaf in _data.listData)
            {
                string nextpath = true == path.Equals("/") ? path + leaf.Name : path + "/" + leaf.Name;
                m_dicViewData.Add(nextpath, new Packet.StoC.ViewData()
                {
                    Name = leaf.Name,
                    CID = leaf.CID,
                    Type = leaf.Type
                });

                new_node.Nodes.Add(CreateViewData(leaf, nextpath));
            }

            return new_node;
        }
        public void ViewData(ClientSocket _socket, Packet.StoC.ViewData _recvdata)
        {
            m_dicViewData.Clear();
            m_from.TreeViewIPFS.Nodes.Clear();

            if (false == string.IsNullOrEmpty(_recvdata.Name))
            {
                m_dicViewData.Add("/", new Packet.StoC.ViewData()
                {
                    Name = _recvdata.Name,
                    CID = _recvdata.CID,
                    Type = _recvdata.Type
                });

                m_from.TreeViewIPFS.Nodes.Add(CreateViewData(_recvdata, "/"));
            }

            m_from.TreeViewIPFS.Update();
        }

        public void Delete(ClientSocket _socket, Packet.StoC.Delete _recvdata)
        {
            if(false == _recvdata.Success)
            {
                MessageBox.Show("Delete Fail", "Delete");
            }

            _socket.Send(new Packet.CtoS.ViewData());
        }

        public void Upload(ClientSocket _socket, Packet.StoC.Upload _recvdata)
        {
            m_from.TextBoxUpload.Text = "";

            if (Packet.StoC.Upload.State.Overlap == _recvdata.ResultState ||
                Packet.StoC.Upload.State.Fail == _recvdata.ResultState)
            {
                MessageBox.Show($"Message={_recvdata.Message}", "Upload Fail", MessageBoxButtons.OK);
                return;
            }

            if (Packet.StoC.Upload.State.Complete == _recvdata.ResultState)
            {
                m_from.TextBoxUpload.Text = "Upload Complete";
                _socket.Send(new Packet.CtoS.ViewData());
            }
        }

        public void File(ClientSocket _socket, Packet.StoC.File _recvdata)
        {
            string path = (_recvdata.LoaclPath + _recvdata.SelectPath + _recvdata.FilePath).Replace('/', '\\');
            if (false == System.IO.File.Exists(path))
            {
                _socket.Send(new Packet.CtoS.File());
                return;
            }

            _socket.Send(new Packet.CtoS.File()
            {
                FilePath = _recvdata.FilePath,
                Data = System.IO.File.ReadAllBytes(path)
            });

            m_from.TextBoxUpload.Text = _recvdata.FilePath.Replace("/", "");
        }
    }
}
