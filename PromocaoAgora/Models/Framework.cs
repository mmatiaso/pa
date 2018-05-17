using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace PromocaoAgora.Models
{
    public class Framework
    {
        public string CortaTexto(string texto, int numchrs)
        {
            if (texto.Length > numchrs)
            {
                texto = texto.Substring(0, numchrs) + "...";
            }
            return texto;
        }

        #region Cookie

        public void write_cookie(String namecookie, String valuecookie, int Ndays)
        {

            HttpCookie cookie = new HttpCookie(namecookie);
            cookie.Value = valuecookie;
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            System.Web.HttpContext.Current.Response.Cookies[namecookie].Expires = DateTime.Now.AddMonths(Ndays);


        }

        public void cookie_off(String namecookie) //remove o cookie
        {
            //Pega o nome do que cookie que o usuário informou
            //String txtcookie = System.Web.HttpContext.Current.Request.Cookies[namecookie].Value.ToString();
            HttpCookie cookie = new HttpCookie(namecookie);
            cookie = System.Web.HttpContext.Current.Request.Cookies[namecookie];
            if (cookie != null)
            {
                cookie.Value = null;
                cookie.Expires = DateTime.Now.AddMonths(-40);
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            }


        }

        public string read_cookie(string namecookie)
        {
            String txtcookie = "";
            HttpCookie cookie = new HttpCookie(namecookie);
            cookie = System.Web.HttpContext.Current.Request.Cookies[namecookie];
            if (cookie != null)
            {
                txtcookie = System.Web.HttpContext.Current.Request.Cookies[namecookie].Value;
            }
            return txtcookie;
        }

        #endregion Cookie

        #region Arquivo

        public bool CriaDiretorio(String _local)
        {
            string root = HttpContext.Current.Server.MapPath(_local);//ConfigurationManager.AppSettings["DominioFisico"].ToString();//"D:\\workspace_vs\\BlathNew\\BlathNew\\Content\\Images\\";
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeletaDiretorio(String _local)
        {
            string root = ConfigurationManager.AppSettings["DominioFisico"].ToString();
            if (Directory.Exists(root + _local))
            {
                Directory.Delete(root + _local, true);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeletaArquivo(String _localfile)
        {
            //string root = ConfigurationManager.AppSettings["DominioFisico"].ToString();
            if (File.Exists(_localfile))
            {
                File.Delete(_localfile);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CopiaDirParaDir(String _origem, String _destino)
        {
            if (Directory.Exists(_origem))
            {
                string[] files = Directory.GetFiles(_origem);
                string fileName;
                string destFile;
                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = Path.GetFileName(s);
                    destFile = Path.Combine(_destino, fileName);
                    File.Copy(s, destFile, true);

                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CopiaArqParaDir(String _origemArq, String _destino)
        {
            if (File.Exists(_origemArq))
            {

                string fileName;
                string destFile;
                fileName = Path.GetFileName(_origemArq);
                destFile = Path.Combine(_destino, fileName);
                File.Copy(_origemArq, destFile, true);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckArquivo(String _localfile)
        {
            if (File.Exists(_localfile))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UploadArquivo(FileUpload _file, String _local, String _filenametosave)
        {
            if (Directory.Exists(_local))
            {
                String caminho = _local + "\\" + _filenametosave;//_file.FileName;
                String fullpath = caminho;
                _file.SaveAs(fullpath);
                /*if(_file.PostedFile.ContentType != "image/jpeg")
                {
                  System.Drawing.Image imgs = System.Drawing.Image.FromFile(fullpath);
                  imgs.Save(fullpath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }*/
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion Arquivo


        public string SendMail(string _from, string _to, string _subject, string _content, bool isUrl = false, string _nome = "")
        {
            MailMessage message = new MailMessage();
            if(_from == "") { _from = "promocaoagoraoficial@gmail.com"; }
            string senderEmail = _from;//"promocaoagoraoficial@gmail.com";// "atendimento@blath.com.br";
            string senderName = "Promoção Agora";
            string fromEmail = _from;
            string fromName = "Promoção Agora";
            string host = "smtp.gmail.com";// "mail.blath.com.br";
            int port = 587;// 8889;
            bool enablessl = true;// false;

            message.Sender = new MailAddress(senderEmail, senderName);

            message.From = new MailAddress(fromEmail, fromName);

            message.ReplyTo = new MailAddress(fromEmail, fromName);

            //message.To.Add(new MailAddress("mmatiaso@hotmail.com"));
            //masi de um receptor
            if (_to.IndexOf(';') > 0)
            {
                string[] receptors = _to.Split(';');
                foreach (string receptor in receptors)
                {
                    message.To.Add(new MailAddress(receptor));
                }
            }
            else
            {
                message.To.Add(new MailAddress(_to));
            }

           // message.CC.Add(new MailAddress("mmatiaso@gmail.com"));

            message.Subject = _subject;

            if (isUrl)
            {
                _content = HttpContent(_content);
            }
            //replace nome
            _content = _content.Replace("##EMAIL##", _to);
            _content = _content.Replace("##NOME##", _nome);

            message.Body = _content;//"<br/><hr/><p>Promoção Agora<br/>Endereço:<br/>Av. Tenente Coronel Muniz e Aragão, 71 - Anil, Jacarepaguá - Rio de Janeiro, RJ - Brasil</p>";//"<div style='background-color:#f5f5f5;padding:4px;width:550px;'><span style='font-size:20px;font-family:Trebuchet MS;'>Blath</span></div><br/>" + _content;

            message.IsBodyHtml = true;

            message.SubjectEncoding = System.Text.Encoding.UTF8;

            //message.ReplyTo = new MailAddress(fromEmail);

            //message.Headers.Add("Return-Path", "bounce@ofertanow.com.br");

            string snha = ConfigurationManager.AppSettings["GmailEmailSenha"];

            SmtpClient smtp = new SmtpClient()
            {
                Host = host,
                Port = port,
                EnableSsl = enablessl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("promocaoagoraoficial@gmail.com", snha)
            };

            try
            {
                smtp.Send(message);
                return "OK";
            }
            catch (Exception exc)
            {

                //System.Web.HttpContext.Current.Response.Redirect("http://taticasdevendascom.br?erro=" + exc.Message);
                return exc.Message;
                //throw new Exception("Erro no envio da mensagem");

            }

        }

        public string HttpContent(string url)
        {
            WebRequest objRequest = System.Net.HttpWebRequest.Create(url);
            StreamReader sr = new StreamReader(objRequest.GetResponse().GetResponseStream());
            string result = sr.ReadToEnd();
            sr.Close();
            return result;
        }

        public string TirarAcentos(string texto)
        {
            string textor = "";

            for (int i = 0; i < texto.Length; i++)
            {
                if (texto[i].ToString() == "ã") textor += "a";
                else if (texto[i].ToString() == "á") textor += "a";
                else if (texto[i].ToString() == "à") textor += "a";
                else if (texto[i].ToString() == "â") textor += "a";
                else if (texto[i].ToString() == "ä") textor += "a";
                else if (texto[i].ToString() == "é") textor += "e";
                else if (texto[i].ToString() == "è") textor += "e";
                else if (texto[i].ToString() == "ê") textor += "e";
                else if (texto[i].ToString() == "ë") textor += "e";
                else if (texto[i].ToString() == "í") textor += "i";
                else if (texto[i].ToString() == "ì") textor += "i";
                else if (texto[i].ToString() == "ï") textor += "i";
                else if (texto[i].ToString() == "õ") textor += "o";
                else if (texto[i].ToString() == "ó") textor += "o";
                else if (texto[i].ToString() == "ò") textor += "o";
                else if (texto[i].ToString() == "ö") textor += "o";
                else if (texto[i].ToString() == "ú") textor += "u";
                else if (texto[i].ToString() == "ù") textor += "u";
                else if (texto[i].ToString() == "ü") textor += "u";
                else if (texto[i].ToString() == "ç") textor += "c";
                else if (texto[i].ToString() == "Ã") textor += "A";
                else if (texto[i].ToString() == "Á") textor += "A";
                else if (texto[i].ToString() == "À") textor += "A";
                else if (texto[i].ToString() == "Â") textor += "A";
                else if (texto[i].ToString() == "Ä") textor += "A";
                else if (texto[i].ToString() == "É") textor += "E";
                else if (texto[i].ToString() == "È") textor += "E";
                else if (texto[i].ToString() == "Ê") textor += "E";
                else if (texto[i].ToString() == "Ë") textor += "E";
                else if (texto[i].ToString() == "Í") textor += "I";
                else if (texto[i].ToString() == "Ì") textor += "I";
                else if (texto[i].ToString() == "Ï") textor += "I";
                else if (texto[i].ToString() == "Õ") textor += "O";
                else if (texto[i].ToString() == "Ó") textor += "O";
                else if (texto[i].ToString() == "Ò") textor += "O";
                else if (texto[i].ToString() == "Ö") textor += "O";
                else if (texto[i].ToString() == "Ú") textor += "U";
                else if (texto[i].ToString() == "Ù") textor += "U";
                else if (texto[i].ToString() == "Ü") textor += "U";
                else if (texto[i].ToString() == "Ç") textor += "C";
                else if (texto[i].ToString() == " ") textor += "-";
                else if (texto[i].ToString() == ".") textor += "";
                else if (texto[i].ToString() == ",") textor += "";
                else if (texto[i].ToString() == ":") textor += "";
                else if (texto[i].ToString() == ";") textor += "";
                else if (texto[i].ToString() == "[") textor += "";
                else if (texto[i].ToString() == "]") textor += "";
                else if (texto[i].ToString() == "_") textor += "-";
                else if (texto[i].ToString() == "|") textor += "-";
                else if (texto[i].ToString() == "'") textor += "";
                else if (texto[i].ToString() == "/") textor += "";
                else if (texto[i].ToString() == "$") textor += "";
                else if (texto[i].ToString() == "%") textor += "";
                else if (texto[i].ToString() == "&") textor += "";
                else if (texto[i].ToString() == "*") textor += "";
                else if (texto[i].ToString() == "(") textor += "";
                else if (texto[i].ToString() == ")") textor += "";
                else textor += texto[i];
            }
            return textor;
        }

        public string SendMailSendGrid(string _from, string _fromname, string _subject, string _content, string _emailrec, string _nomerec)
        {
            try
            {
                MailMessage mailMsg = new MailMessage();

                // To
                mailMsg.To.Add(new MailAddress(_emailrec, _nomerec));

                // From
                mailMsg.From = new MailAddress(_from, _fromname);

                // Subject and multipart/alternative Body
                mailMsg.Subject = _subject;
                string text = _content;
                string html = @"<div style='width:600px; font-family:arial;font-size:12pt;line-height:22pt;'>" + _content + "</div>";
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, "text/plain"));
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, "text/html"));

                // Init SmtpClient and send
                SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("mmatiaso", "mmatiasomacinho2");
                smtpClient.Credentials = credentials;

                smtpClient.Send(mailMsg);
                string[] keys = mailMsg.Headers.AllKeys;
                string hmsg = "";
                foreach (string s in keys)
                {
                    hmsg += s + " - " + mailMsg.Headers[s];
                }
                return hmsg;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}