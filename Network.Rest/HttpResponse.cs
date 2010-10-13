using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;

namespace Network.Rest
{
    public class HttpResponse : HttpMessage, IResponse<HttpResponse>
    {
        public HttpResponse()
        {
            ResponseCode = HttpStatusCode.OK;
        }



        public static HttpResponse FromBytes(byte[] bytes)
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


        private static HttpResponse Parse(string responseString)
        {
            HttpResponse response = new HttpResponse();
            StringReader reader = new StringReader(responseString);
            string line = reader.ReadLine();
            string[] firstLine = line.Split(' ');
            //VERSION RESPONSECODE RESPONSEMESSAGE
            response.HttpVersion = HttpVersion.HTTP11;
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
            writer.Write(Encoding.GetBytes(string.Format("HTTP/1.1 {0} {1}{2}", (int)ResponseCode, ResponseMessage, Environment.NewLine)));
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

        public HttpResponse GetResponse(BinaryReader reader)
        {
            string line = BinaryHelper.ReadLine(reader);
            string[] firstLine = line.Split(' ');
            //VERSION RESPONSECODE RESPONSEMESSAGE
            HttpVersion = HttpVersion.HTTP11;
            ResponseCode = (HttpStatusCode)int.Parse(firstLine[1]);
            ResponseMessage = string.Join(" ", firstLine, 2, firstLine.Length - 2);
            ReadHeaders(reader);
            BinaryWriter bw = new BinaryWriter(this.Body);
            bw.Write(reader.ReadBytes(this.ContentLength));
            Body.Seek(0, SeekOrigin.Begin);
            return this;
            //StreamReader reader = new StreamReader(binaryReader.BaseStream);
            //return ReadFrom(reader);
        }

        #endregion

        //private HttpResponse ReadFrom(StreamReader reader)
        //{

        //    string line = reader.ReadLine();
        //    string[] firstLine = line.Split(' ');
        //    //VERSION RESPONSECODE RESPONSEMESSAGE
        //    HttpVersion = HttpVersion.HTTP11;
        //    ResponseCode = (HttpStatusCode)int.Parse(firstLine[1]);
        //    ResponseMessage = string.Join(" ", firstLine, 2, firstLine.Length - 2);
        //    ReadHeaders(reader);
        //    BinaryWriter bw = new BinaryWriter(this.Body);
        //    BinaryReader breader = new BinaryReader(reader.BaseStream);
        //    bw.Write(breader.ReadBytes(this.ContentLength));
        //    Body.Seek(0, SeekOrigin.Begin);
        //    return this;

        //}


        #region IResponse<HttpResponse> Members


        public HttpResponse GetResponse(byte[] requestBytes)
        {
            return HttpResponse.FromBytes(requestBytes);
        }

        #endregion
    }
}
