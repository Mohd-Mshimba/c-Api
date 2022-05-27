using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.Net;
using System.IO;
using ZanMalipo.ViewModels;

namespace Signature
{
    public class Utility
    {
        string privateStorePath = @"Certificates\gepgclientprivatekey.pfx";
        string publicStorePath = @"Certificates\gepgclientpubliccertificate.pfx";
        string gepgPublicCertStorePath = @"Certificates\gepgpubliccertificate.pfx";
        string certPass = "passpass";
        RSACryptoServiceProvider rsaCrypto = null;
        gepgBillSubReq newBill = null;

        public String createBill(BillResponse bill)
        {
            BillHdr billHdr = new BillHdr() { SpCode = "SP108", RtrRespFlg = "true" };
            List<BillItem> billItems = new List<BillItem>();
            
            foreach (var item in bill.services)
            {
                billItems.Add(new BillItem()
                {
                    BillItemRef = "14353355" + item.Id,
                    UseItemRefOnPay = "N",
                    BillItemAmt = item.Price,
                    BillItemEqvAmt = item.Price,
                    BillItemMiscAmt = 0.00,
                    GfsCode = item.GFSCode,
                });
            }

            BillTrxInf billTrxInf = new BillTrxInf()
            {
                BillId = "000001",
                SubSpCode = "1001",
                SpSysId = "TPF001",
                BillAmt = bill.Amount,
                MiscAmt = 0.00,
                BillExprDt = "2022-05-27T12:00:00",
                PyrId = "T121AAA",
                PyrName = bill.BillName,
                BillDesc = "Road Safety",
                BillGenDt = "2022-05-26T12:00:00",
                BillGenBy = 1,
                BillApprBy = "TPF001",
                PyrCellNum = "0713525539",
                PyrEmail = "tinno@yahoo.com",
                Ccy = "TZS",
                BillEqvAmt = bill.Amount,
                RemFlag = "true",
                BillPayOpt = 3,
                BillItems = billItems
            };

            newBill = new gepgBillSubReq() { 
                BillHdr = billHdr, 
                BillTrxInf = billTrxInf 
            };

            XmlSerializer xs = null;
            XmlSerializerNamespaces ns = null;
            XmlWriterSettings settings = null;
            string outString = "";

            XmlWriter xw = null;

            try
            {
                settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                //settings.Indent = true;
                ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                StringBuilder sb = new StringBuilder();
                xs = new XmlSerializer(typeof(gepgBillSubReq));

                xw = XmlWriter.Create(sb, settings);

                xs.Serialize(xw, newBill, ns);
                xw.Flush();
                outString = sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (xw != null)
                {
                    xw.Close();
                }
            }

            return outString;
        }

        public string finaliseSignedMsg(string content, string sign)
        {
            Gepg gepgBill = new Gepg() { gepgBillSubReq = newBill, gepgSignature = sign };

            XmlSerializer xs = null;
            XmlSerializerNamespaces ns = null;
            XmlWriterSettings settings = null;
            XmlWriter xw = null;
            String outString = String.Empty;

            try
            {
                ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                settings = new XmlWriterSettings();
                //settings.Indent = true;
                StringBuilder sb = new StringBuilder();
                xs = new XmlSerializer(typeof(Gepg));
                xw = XmlWriter.Create(sb, settings);

                xs.Serialize(xw, gepgBill, ns);
                xw.Flush();
                outString = sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (xw != null)
                {
                    xw.Close();
                }
            }
            Console.WriteLine(outString + outString.Length);
            return outString;
        }

        public string SerializeClean(gepgBillSubReq bill)
        {
            XmlSerializer xs = null;
            //These are the objects that will free us from extraneous markup.
            XmlWriterSettings settings = null;
            XmlSerializerNamespaces ns = null;

            //We use a XmlWriter instead of a StringWriter.
            XmlWriter xw = null;

            String outString = String.Empty;

            try
            {
                //To get rid of the xml declaration we create an 
                //XmlWriterSettings object and tell it to OmitXmlDeclaration.
                settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;

                //To get rid of the default namespaces we create a new
                //set of namespaces with one empty entry.
                ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                StringBuilder sb = new StringBuilder();

                xs = new XmlSerializer(typeof(gepgBillSubReq));

                //We create a new XmlWriter with the previously created settings 
                //(to OmitXmlDeclaration).
                xw = XmlWriter.Create(sb, settings);

                //We call xs.Serialize and pass in our custom 
                //XmlSerializerNamespaces object.
                xs.Serialize(xw, bill, ns);

                xw.Flush();

                outString = sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (xw != null)
                {
                    xw.Close();
                }
            }
            return outString;
        }

        public string generateSignature(string strUnsignedContent)
        {
            string signature = string.Empty;
            try
            {
                X509Certificate2 certificate = new X509Certificate2();
                certificate.Import(privateStorePath, certPass, X509KeyStorageFlags.PersistKeySet);
                rsaCrypto = (RSACryptoServiceProvider)certificate.PrivateKey;

                if (rsaCrypto == null)
                {
                   Console.WriteLine("No certificate found!");
                }
                else
                {
                    SHA1Managed sha1 = new SHA1Managed();
                    byte[] hash;
                    //compute sha1
                    hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(strUnsignedContent));
                    // Sign the hash                
                    byte[] signedHash = rsaCrypto.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
                    string signedHashString = Convert.ToBase64String(signedHash);
                    signature = signedHashString;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return signature;
        }

        public bool VerifyData(string strContent, string strSignature)
        {
            try
            {
                byte[] str = Encoding.UTF8.GetBytes(strContent);
                byte[] signature = Convert.FromBase64String(strSignature);

                // read the public key 
                X509Certificate2 certificate = new X509Certificate2();
                certificate = new X509Certificate2(publicStorePath);
                // certificate.Import(publicStorePath, certPass, X509KeyStorageFlags.PersistKeySet);
                rsaCrypto = (RSACryptoServiceProvider)certificate.PublicKey.Key;

                // compute the hash again, also we can pass it as a parameter
                SHA1Managed sha1hash = new SHA1Managed();
                byte[] hashdata = sha1hash.ComputeHash(str);

                if (rsaCrypto.VerifyHash(hashdata, "SHA1", signature))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public string sendHttpRequest(String content)
        {
            try
            {
                // Create a request using a URL that can receive a post.   
                WebRequest request = WebRequest.Create("http://uat1.gepg.go.tz/api/bill/sigqrequest");
                // Set the Method property of the request to POST.  
                request.Method = "POST";
                // Create POST data and convert it to a byte array.  
                byte[] byteArray = Encoding.UTF8.GetBytes(content);
                // Set the ContentType property of the WebRequest.  
                request.ContentType = "application/xml";
                // Set the ContentLength property of the WebRequest. 
                request.ContentLength = byteArray.Length;
                //Set Custom Headers
                request.Headers.Add("Gepg-Code", "SP20000");
                request.Headers.Add("Gepg-Com", "default.sp.in");
                // Get the request stream.  
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.  
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.  
                dataStream.Close();


                // Get the response.  
                WebResponse response = request.GetResponse();
                // Display the status.  
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.  
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();
                // Display the content.  
                Console.WriteLine(responseFromServer);
                // Clean up the streams.  
                reader.Close();
                dataStream.Close();
                response.Close();
                return responseFromServer;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "";
            }


        }

        internal bool VerifyGePGData(string gepgResponse)
        {
            string dataTag = "gepgBillSubReqAck";
            string sigTag = "gepgSignature";

            string data = getContent(gepgResponse,dataTag);
            string signature = getSig(gepgResponse, sigTag);

            try
            {
                byte[] str = Encoding.UTF8.GetBytes(data);
                byte[] sig = Convert.FromBase64String(signature);

                // read the public key 
                X509Certificate2 certificate = new X509Certificate2();
                certificate.Import(gepgPublicCertStorePath, certPass, X509KeyStorageFlags.PersistKeySet);
                rsaCrypto = (RSACryptoServiceProvider)certificate.PublicKey.Key;

                // compute the hash again, also we can pass it as a parameter
                SHA1Managed sha1hash = new SHA1Managed();
                byte[] hashdata = sha1hash.ComputeHash(str);

                if (rsaCrypto.VerifyHash(hashdata, "SHA1", sig))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

        }

        private string getContent(string rawData, string dataTag)
        {
            string content = rawData.Substring(rawData.IndexOf(dataTag) - 1, rawData.LastIndexOf(dataTag) + dataTag.Length + 2 - rawData.IndexOf(dataTag));
            return content;
        }

        private string getSig(string rawData, string sigTag)
        {
            string content = rawData.Substring(rawData.IndexOf(sigTag) + sigTag.Length + 1, rawData.LastIndexOf(sigTag) - rawData.IndexOf(sigTag) - sigTag.Length - 3);
            return content;
        }
    }
}