using Amos;
using Amos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Amos
{
    /// <summary>
    /// Summary description for ImageManager
    /// </summary>
    public class ImageManager : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string fileIdStr = "";
            if (context.Request.QueryString["id"] != null)
                fileIdStr = context.Request.QueryString["id"];
            else
                throw new ArgumentException("No parameter specified");

            int fileId = 0;
            try
            {
                fileId = Convert.ToInt32(fileIdStr.Split('_')[1]);
            }
            catch
            {
                throw new Exception("Failed to split image id");
            }

            ApplicationDbContext cdb = new ApplicationDbContext();
            var image = cdb.AmosFiles.Where(x => x.FileId == fileId).FirstOrDefault();

            if (image == null) throw new Exception("No image found");
            else
            {
                context.Response.ContentType = image.ContentType;
                Stream strm = new MemoryStream(image.Content);
                byte[] buffer = new byte[4096];

                int byteSeq = strm.Read(buffer, 0, 4096);

                while (byteSeq > 0)
                {
                    context.Response.OutputStream.Write(buffer, 0, byteSeq);
                    byteSeq = strm.Read(buffer, 0, 4096);
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}