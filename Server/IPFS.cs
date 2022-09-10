using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace Server
{
    public class IPFS
    {
        public IPFS(string _ip)
        {
            URL = _ip;
        }

        private class ResultData
        {
            public ResultData()
            {
                StatusCode = HttpStatusCode.Continue;
                Data = "";
            }

            public HttpStatusCode StatusCode;
            public string Data;
        }

        private class Entrie
        {
            public Entrie()
            {
                Name = "";
                Type = 0;
                Size = 0;
                Hash = "";
            }

            public string Name { get; set; }
            public int Type { get; set; }
            public int Size { get; set; }
            public string Hash { get; set; }
        }
        private class lsData
        {
            public lsData()
            {
                Entries = new List<Entrie>();
            }

            public List<Entrie> Entries { get; set; }
        }
        private async Task<lsData?> files_ls(string url, string path)
        {
            ResultData result = await Http(url + "/api/v0/files/ls" + (true == string.IsNullOrEmpty(path) ? "" : "?arg=" + path));
            if (HttpStatusCode.OK != result.StatusCode)
            {
                Console.WriteLine($"files_ls StatusCode={result.StatusCode}, URL={url}, Path={path}");
                return null;
            }

            return JsonSerializer.Deserialize<lsData>(result.Data);
        }

        private class StatData
        {
            public StatData()
            {
                Hash = "";
                Size = 0;
                CumulativeSize = 0;
                Blocks = 0;
                Type = "";
            }

            public string Hash { get; set; }
            public int Size { get; set; }
            public int CumulativeSize { get; set; }
            public int Blocks { get; set; }
            public string Type { get; set; }
        }
        private async Task<StatData?> files_stat(string url, string path)
        {
            ResultData result = await Http(url + "/api/v0/files/stat?arg=" + path);
            if (HttpStatusCode.OK != result.StatusCode)
            {
                Console.WriteLine($"files_stat StatusCode={result.StatusCode}, URL={url}, Path={path}");
                return null;
            }

            return JsonSerializer.Deserialize<StatData>(result.Data);
        }

        private async Task<bool> files_rm(string url, string path)
        {
            ResultData result = await Http(url + $"/api/v0/files/rm?arg={path}&recursive=true&force=true");
            if (HttpStatusCode.OK != result.StatusCode)
            {
                Console.WriteLine($"files_rm StatusCode={result.StatusCode}, URL={url}, Path={path}");
                return false;
            }

            return true;
        }

        private async Task<bool> FileWrite(string url, string path, FileInfo finfo)
        {
            ResultData result = await Upload(url + $"/api/v0/files/write?arg={path + '/' + finfo.Name}&create=true&parents=true", finfo.Name, await File.ReadAllBytesAsync(finfo.FullName));
            if (HttpStatusCode.OK != result.StatusCode)
            {
                Console.WriteLine($"StatusCode={result.StatusCode}");
                return false;
            }

            return true;
        }
        private async Task<Dictionary<string, bool>> DirectoryTraverse(string url, string path, DirectoryInfo rootinfo)
        {
            Dictionary<string, bool> dicResult = new Dictionary<string, bool>();

            foreach (DirectoryInfo dinfo in rootinfo.GetDirectories())
            {
                foreach (KeyValuePair<string, bool> result in await DirectoryTraverse(url, path + '/' + dinfo.Name, dinfo))
                {
                    if (false == dicResult.ContainsKey(result.Key))
                    {
                        dicResult.Add(result.Key, result.Value);
                    }

                    dicResult[result.Key] = result.Value;
                }
            }

            foreach (FileInfo finfo in rootinfo.GetFiles())
            {
                string key = path + '/' + finfo.Name;
                if (false == dicResult.ContainsKey(key))
                {
                    dicResult.Add(key, false);
                }

                dicResult[key] = await FileWrite(url, path, finfo);
            }

            return dicResult;
        }
        private async Task<Dictionary<string, bool>?> files_write(string url, string ipfspath, string filepath)
        {
            FileAttributes attributes = File.GetAttributes(filepath);
            if (true == attributes.HasFlag(FileAttributes.Directory))
            {
                DirectoryInfo rootinfo = new DirectoryInfo(filepath);
                if (false == rootinfo.Exists)
                {
                    Console.WriteLine($"Directory Not Exists Paht={filepath}");
                    return null;
                }

                return await DirectoryTraverse(url, ipfspath, rootinfo);
            }
            else
            {
                FileInfo finfo = new FileInfo(filepath);
                if (false == finfo.Exists)
                {
                    Console.WriteLine($"File Not Exists Paht={filepath}");
                    return null;
                }

                Dictionary<string, bool> dicResult = new Dictionary<string, bool>();
                dicResult.Add(ipfspath + '/' + finfo.Name, await FileWrite(url, ipfspath, finfo));
                return dicResult;
            }
        }

        private async Task<ResultData> Http(string url)
        {
            ResultData data = new ResultData();

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpResponseMessage response = await httpClient.PostAsync(url, null))
                    {
                        data.StatusCode = response.StatusCode;
                        if (HttpStatusCode.OK == response.StatusCode)
                        {
                            data.Data = await response.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            Console.WriteLine($"URL={url}, Response.ReasonPhrase={response.ReasonPhrase}");
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException ErrorMessage={ex.Message}, InnerException.Message={ex.InnerException?.Message}");
            }
            catch (Exception ex2)
            {
                Console.WriteLine($"Exception ErrorMessage={ex2.Message}");
            }

            return data;
        }

        private async Task<ResultData> Upload(string url, string file_name, byte[] file_data)
        {
            ResultData data = new ResultData();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (MultipartFormDataContent content = new MultipartFormDataContent())
                    {
                        content.Add(new StreamContent(new MemoryStream(file_data)), "file", file_name);

                        using (HttpResponseMessage response = await client.PostAsync(url, content))
                        {
                            data.StatusCode = response.StatusCode;
                            if (HttpStatusCode.OK == response.StatusCode)
                            {
                                data.Data = await response.Content.ReadAsStringAsync();
                            }
                            else
                            {
                                Console.WriteLine($"URL={url}, Response.ReasonPhrase={response.ReasonPhrase}");
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException ErrorMessage={ex.Message}, InnerException.Message={ex.InnerException?.Message}");
            }
            catch (Exception ex2)
            {
                Console.WriteLine($"Exception ErrorMessage={ex2.Message}");
            }

            return data;
        }

        public async Task<Packet.StoC.ViewData?> ls(string path = "/")
        {
            lsData? rootlsdata = await files_ls(URL, path);
            if (null == rootlsdata)
            {
                return null;
            }

            StatData? rootstatdate = await files_stat(URL, path);
            if (null == rootstatdate)
            {
                return null;
            }

            Packet.StoC.ViewData rootviewdata = new Packet.StoC.ViewData();
            rootviewdata.CID = rootstatdate.Hash;

            if (true == rootstatdate.Type.Equals("directory"))
            {
                rootviewdata.Type = Packet.StoC.ViewData.DataType.Directory;

                if (null != rootlsdata!.Entries)
                {
                    foreach (Entrie entrie in rootlsdata!.Entries)
                    {
                        Packet.StoC.ViewData? leafviewdata = 0 == entrie.Type ? await ls(true == path.Equals("/") ? path + entrie.Name : path + "/" + entrie.Name) : new Packet.StoC.ViewData();

                        if (null != leafviewdata)
                        {
                            leafviewdata.Name = entrie.Name;
                            rootviewdata.listData.Add(leafviewdata);
                        }
                    }
                }
            }
            else
            {
                rootviewdata.Type = Packet.StoC.ViewData.DataType.File;
            }

            return rootviewdata;
        }

        public async Task<bool> Upload(string ipfspath)
        {
            Dictionary<string, bool>? dicResult = await files_write(URL, ipfspath, Server.Instance.UploadPath);
            if (null == dicResult)
            {
                return false;
            }

            foreach(KeyValuePair<string, bool> result in dicResult)
            {
                if(true == result.Value)
                {
                    continue;
                }

                Console.WriteLine($"Upload Fail. File={result.Key}");
            }

            return true;
        }

        public async Task<bool> Delete(string path)
        {
            return await files_rm(URL, path);
        }

        private string URL;
    }
}
