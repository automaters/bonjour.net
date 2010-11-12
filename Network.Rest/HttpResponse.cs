using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.IO.Compression;

namespace Network.Rest
{
    public class HttpResponse<T> : HttpMessage, IClientResponse<T>, IServerResponse
        where T : HttpResponse<T>,new()
    {
        public HttpResponse()
        {
            ResponseCode = HttpStatusCode.OK;
            Protocol = HttpProtocol.HTTP11;
        }



        public static T FromBytes(byte[] bytes)
        {
            return Parse(Encoding.UTF8.GetString(bytes));
        }

        public HttpStatusCode ResponseCode { get; set; }
        public string ResponseMessage { get; set; }

        protected XmlDocument document = null;

        public XmlDocument Document
        {
            get
            {
                if (document == null)
                {
                    document = new XmlDocument();
                    if (Body.Length > 0)
                        document.Load(Body);
                }
                return document;
            }
        }


        private static T Parse(string responseString)
        {
            T response = new T();
            StringReader reader = new StringReader(responseString);
            string line = reader.ReadLine();
            string[] firstLine = line.Split(' ');
            //VERSION RESPONSECODE RESPONSEMESSAGE
            response.Protocol = firstLine[0];
            response.ResponseCode = (HttpStatusCode)int.Parse(firstLine[1]);
            response.ResponseMessage = string.Join(" ", firstLine, 2, firstLine.Length - 2);
            response.ReadHeaders(reader);
            StreamWriter sw = new StreamWriter(response.Body);
            sw.Write(reader.ReadToEnd());
            response.Body.Seek(0, SeekOrigin.Begin);
            return response;
        }

        public override string ToString()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                WriteTo(new BinaryWriter(ms));
                ms.Position = 0;
                using (StreamReader reader = new StreamReader(ms))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        #region IResponse Members

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Encoding.GetBytes(string.Format("{3} {0} {1}{2}", (int)ResponseCode, ResponseMessage, Environment.NewLine, Protocol)));
            foreach (KeyValuePair<string, string> header in Headers)
            {
                if (header.Key != "Host")
                    writer.Write(Encoding.GetBytes(string.Format("{0}:{1}{2}", header.Key, header.Value, Environment.NewLine)));
            }
            writer.Write(Encoding.GetBytes(Environment.NewLine));
            Body.Seek(0, SeekOrigin.Begin);
            Body.WriteTo(writer.BaseStream);
            writer.Write(Encoding.GetBytes(Environment.NewLine));
            writer.Write(Encoding.GetBytes(Environment.NewLine));
        }

        public T GetResponse(BinaryReader reader)
        {
            string line = BinaryHelper.ReadLine(reader);
            if (line == null)
                return null;
            string[] firstLine = line.Split(' ');
            //VERSION RESPONSECODE RESPONSEMESSAGE
            Protocol = firstLine[0];
            ResponseCode = (HttpStatusCode)int.Parse(firstLine[1]);
            ResponseMessage = string.Join(" ", firstLine, 2, firstLine.Length - 2);
            ReadHeaders(reader);
            if (this.ContentLength > 0)
            {
                BinaryWriter bw = new BinaryWriter(this.Body);
                bw.Write(reader.ReadBytes(this.ContentLength));
                Body.Seek(0, SeekOrigin.Begin);
                string encoding;
                if (Headers.TryGetValue("CONTENT-ENCODING", out encoding))
                {
                    if (encoding.ToLower() == "gzip")
                    {
                        var unzip = new GZipStream(Body, CompressionMode.Decompress);
                        byte[] buffer = new byte[1024];
                        int length = 0;
                        MemoryStream unzippedStream = new MemoryStream();
                        while ((length = unzip.Read(buffer, 0, buffer.Length)) > 0)
                            unzippedStream.Write(buffer, 0, length);
                        Body.Close();
                        Body.Dispose();
                        Body = unzippedStream;
                        Body.Seek(0, SeekOrigin.Begin);
                    }
                }
                LoadContent();
            }
            return this as T;
            //StreamReader reader = new StreamReader(binaryReader.BaseStream);
            //return ReadFrom(reader);
        }

        protected virtual void LoadContent()
        {
        }

        #endregion

        #region IResponse<HttpResponse> Members


        public T GetResponse(byte[] requestBytes)
        {
            return HttpResponse<T>.FromBytes(requestBytes);
        }

        #endregion
    }

    public class HttpResponse : HttpResponse<HttpResponse>
    {
        public HttpResponse()
        {
            Protocol = HttpProtocol.HTTP11;
        }

    }
}
