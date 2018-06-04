using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Net.Sockets;
using ICSharpCode.SharpZipLib.GZip;

namespace Gardener.Crawler.Api.Util
{
    class HttpClient
    {
        public static async Task<string> Do(string requestUri)
        {
            string host = string.Empty;
            int port = 80;
            string pathAndQuery = "/";
            string scheme = string.Empty;

            string originalString = requestUri;

            scheme = originalString.Substring(0, originalString.IndexOf("://"));
            originalString = originalString.Substring(originalString.IndexOf("://") + 3);

            host = originalString.Substring(0, originalString.IndexOf("/"));
            originalString = originalString.Substring(originalString.IndexOf("/"));

            if (host.IndexOf(":") > -1)
            {
                port = int.Parse(host.Substring(0, host.IndexOf(":")));
            }

            if(!string.IsNullOrEmpty(originalString))
            {
                pathAndQuery = originalString;
            }            

            return await Do(host, port, pathAndQuery, scheme, "GET", "");
        }

        public static async Task<string> Do(Uri requestUri)
        {
            return await Do(requestUri.OriginalString);
        }

        public static async Task<string> Do(string host, int port, string pathAndQuery, string scheme, string method, string body)
        {
            try
            {
                TcpClient tcpClient = new TcpClient();

                StringBuilder requestHeaders = new StringBuilder();

                requestHeaders.AppendFormat("{0} {1} HTTP/1.1", method, pathAndQuery).Append("\r\n");
                requestHeaders.AppendFormat("Host: {0}", host).Append("\r\n");
                requestHeaders.Append("Connection: keep-alive").Append("\r\n");
                requestHeaders.Append("User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36").Append("\r\n");
                requestHeaders.Append("Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8").Append("\r\n");
                requestHeaders.Append("Accept-Encoding: gzip").Append("\r\n");
                requestHeaders.Append("Accept-Language: zh-CN,zh;q=0.8").Append("\r\n");
                requestHeaders.Append("Cache-Control: no-cache").Append("\r\n");

                byte[] buffer = Encoding.UTF8.GetBytes(requestHeaders.Append("\r\n").ToString() + body);

                await tcpClient.ConnectAsync(host, port);

                if(!tcpClient.Connected)
                {
                    return string.Empty;
                }

                tcpClient.SendTimeout = 30000;
                tcpClient.ReceiveTimeout = 30000;

                tcpClient.Client.Send(buffer);

                string response = string.Empty;

                List<byte> list = new List<byte>();
                StringBuilder stringBuilder = new StringBuilder(4096);

                byte CR = Convert.ToByte('\r');
                byte LF = Convert.ToByte('\n');

                do
                {
                    byte[] responseBuffer = new byte[4096];
                    int len = tcpClient.Client.Receive(responseBuffer);

                    if (len > 0)
                    {
                        for (int i = 0; i < len; i++)
                        {
                            list.Add(responseBuffer[i]);
                        }
                        stringBuilder.Append(Encoding.UTF8.GetString(responseBuffer, 0, len));
                    }
                } while (tcpClient.Client.Available > 0);

                response = stringBuilder.ToString();

                if(string.IsNullOrEmpty(response))
                {
                    return string.Empty;
                }

                string header = response.Substring(0, response.IndexOf("\r\n\r\n"));

                if (string.IsNullOrEmpty(header))
                {
                    return string.Empty;
                }

                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                string[] headers = header.Split(new String[] { "\r\n" }, StringSplitOptions.None);

                if(headers.Length > 0)
                {
                    string[] statusLine = headers[0].Split(new String[] { " " }, StringSplitOptions.None);
                    string httpVersion = statusLine[0];
                    string statusCode = statusLine[1];
                    string reasonPhrase = statusLine[2];

                    if(statusCode != "200")
                    {
                        return "";
                    }
                }

                for(int i = 1; i < headers.Length; i++)
                {
                    string row = headers[i];

                    string[] kv = row.Split(new String[] { ": " }, StringSplitOptions.None);

                    if(dictionary.ContainsKey(kv[0]))
                    {
                        dictionary[kv[0]] += "; " + kv[1];
                    }
                    else
                    {
                        dictionary.Add(kv[0], kv[1]);
                    }                    
                }

                int index = 0;                

                for (int i = 0; i < list.Count - 4; i++)
                {
                    if(list[i] == CR && list[i + 1] == LF && list[i + 2] == CR && list[i + 3] == LF)
                    {
                        index = i + 4;
                        break;
                    }
                }

                for (int i = index; i < list.Count - 2; i++)
                {
                    if (list[i] == CR && list[i + 1] == LF)
                    {
                        index = i + 2;
                        break;
                    }
                }

                byte[] data = new byte[list.Count - index];

                for (int i = 0; i < list.Count - index; i++)
                {
                    data[i] = list[i + index];
                }

                if(dictionary["Content-Encoding"] == "gzip")
                {
                    using (MemoryStream inStream = new MemoryStream(data))
                    {
                        using (MemoryStream outStream = new MemoryStream())
                        {
                            GZip.Decompress(inStream, outStream, false);

                            outStream.Position = 0;

                            string _data = string.Empty;
                            using (StreamReader reader = new StreamReader(outStream))
                            {
                                _data = reader.ReadToEnd();
                            }

                            return _data;
                        }
                    }
                }
                else
                {
                    return Encoding.UTF8.GetString(data);
                }
            }
            catch
            {
                return null;
            }            
        }
    }
}
