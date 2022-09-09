using System.IO;
using System.Text.Json;

namespace Server
{
    public class ConfigInfo
    {
        public ConfigInfo()
        {
            Port = 0;
            IPFSAddress = "";
            UploadRootPath = "UploadRoot";
        }

        public int Port { get; set; }
        public string IPFSAddress { get; set; }
        public string UploadRootPath { get; set; }
    }

    public class Config
    {
        public Config()
        {
            ConfigInfo = null;
        }

        public bool LoadConfig(string _path)
        {
            FileInfo finfo = new FileInfo(_path);
            if(false == finfo.Exists)
            {
                return false;
            }

            FileStream fs = finfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(fs);
            string jsondata = sr.ReadToEnd();
            sr.Close();
            fs.Close();

            ConfigInfo = JsonSerializer.Deserialize<ConfigInfo>(jsondata);
            return true;
        }

        public ConfigInfo? ConfigInfo { get; private set; }
    }
}
