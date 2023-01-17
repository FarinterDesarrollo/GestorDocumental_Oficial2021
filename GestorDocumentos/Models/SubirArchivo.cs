using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace GestorDocumentos.Models
{
    public class SubirArchivo
    {
        public string rutaacceso { get; set; }
        public string confirmacion { get; set; }
        public Exception error { get; set; }
        public void subirarchivo(string ruta, string rutalocal)
        {
            try
            {
                //file.SaveAs();
                FtpWebRequest ftpRequest;
                FtpWebResponse ftpResponse;

                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(ruta));
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Proxy = null;
                ftpRequest.UseBinary = true;
                ftpRequest.Credentials = new NetworkCredential("desarrollo", "1nfr42018");
                //ftpRequest.Credentials = new NetworkCredential("", "");
                FileInfo ff = new FileInfo(rutalocal);

                byte[] fileContents = new byte[ff.Length];
                using (FileStream fr = ff.OpenRead())
                {
                    fr.Read(fileContents, 0, Convert.ToInt32(ff.Length));
                }
                using (Stream writer = ftpRequest.GetRequestStream())
                {
                    writer.Write(fileContents, 0, fileContents.Length);
                }

                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                this.confirmacion = "Fichero Guardado";
            }
            catch (Exception ex)
            {
                this.error = ex;
            }
        }
    }
}