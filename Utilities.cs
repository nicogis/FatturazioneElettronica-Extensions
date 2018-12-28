namespace FatturazioneElettronica.Extensions
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// utilities per la fatturazione elettronica e la conservazione sostitutiva
    /// la libreria utilizzata la libreria chilkat http://www.chilkatsoft.com/
    /// Per utilizzarla in modalita trial (30gg) passare una stringa qualsiasi per il parametro unlock del metodo UnlockLicense
    /// </summary>
    public static class Utilities
    {
        private static Chilkat.Global glob;

        /// <summary>
        /// sblocco della licenza Chilkat http://www.chilkatsoft.com/
        /// metodo da chiamare prima di utilizzare gli altri metodo che necessitano di licenza
        /// una volta chiamato non necessita di essere richiamato
        /// </summary>
        /// <param name="unlock">codice di sblocco licenza</param>
        /// <param name="lastError">ultimo errore nella funzionalità</param>
        /// <returns>true sblocco della licenza con successo</returns>
        public static bool UnlockLicense(string unlock, ref string lastError)
        {
            bool success = false;
            try
            {
                Utilities.glob = new Chilkat.Global();
                success = Utilities.glob.UnlockBundle(unlock);

                if (!success)
                {
                    lastError = Utilities.glob.LastErrorText;
                    return success;
                }

                int status = Utilities.glob.UnlockStatus;

                if (status != 0)
                {
                    success = true;
                }

                lastError = Utilities.glob.LastErrorText;
            }
            catch
            {
                throw;
            }

            return success;
        }

        /// <summary>
        /// Firma fattura elettronica
        /// </summary>
        /// <param name="SubjectCN">Subject Common Name</param>
        /// <param name="pathFile">file fattura elettronica</param>
        /// <param name="lastError">ultimo errore nella funzionalità</param>
        /// <param name="pin">pin smartcard/usb (opzionale). Se non fornito dovrebbe comparire una finestra di dialogo del sistema operativo Windows per indicare il pin</param>
        /// <returns>firma avvenuta con successo</returns>
        public static bool Firma(string SubjectCN, string pathFile, ref string lastError, string pin = null)
        {
            bool success = false;
            try
            {
                string fileName = Path.GetFileName(pathFile);

                if (!Utilities.IsValidNomeFileFattura(fileName, ref lastError, false))
                {
                    return success;
                }


                if (Utilities.glob.UnlockStatus == 0)
                {
                    lastError = "Licenza bloccata";
                    return success;
                }

                Chilkat.Crypt2 crypt = new Chilkat.Crypt2();

                //  Utilizza il certificato su una smartcard o su USB.
                Chilkat.Cert cert = new Chilkat.Cert();

                //  Passa il Subject CN del certificato al metodo LoadByCommonName.
                success = cert.LoadByCommonName(SubjectCN);
                if (success != true)
                {
                    lastError = cert.LastErrorText;
                    return success;
                }

                //  Fornire il PIN della smartcard.
                //  Se il pin non è fornito esplicitamente qui,
                //  Se non fornito dovrebbe comparire una finestra di dialogo del sistema operativo Windows per indicare il pin
                if (!string.IsNullOrWhiteSpace(pin))
                {
                    cert.SmartCardPin = pin;
                }

                //  Fornisce il certificato per firmarlo
                success = crypt.SetSigningCert(cert);
                if (success != true)
                {
                    lastError = crypt.LastErrorText;
                    return success;
                }

                //  Indica l'algoritmo da utilizzare
                crypt.HashAlgorithm = "sha256";

                //  Specifico gli attributi firmati per essere inclusi.
                //  (Questo è quello che fa un CAdES-BES compliant.)
                Chilkat.JsonObject jsonSignedAttrs = new Chilkat.JsonObject();
                jsonSignedAttrs.UpdateInt("contentType", 1);
                jsonSignedAttrs.UpdateInt("signingTime", 1);
                jsonSignedAttrs.UpdateInt("messageDigest", 1);
                jsonSignedAttrs.UpdateInt("signingCertificateV2", 1);
                crypt.SigningAttributes = jsonSignedAttrs.Emit();


                string sigFile = $"{pathFile}.{Enum.GetName(typeof(EstensioniFile), EstensioniFile.p7m)}";

                //  Creo una firma CAdES-BES, che contiene i dati originali
                success = crypt.CreateP7M(pathFile, sigFile);
                if (!success)
                {
                    lastError = crypt.LastErrorText;
                    return success;
                }

                success = true;
            }
            catch
            {
                throw;
            }

            return success;
        }

        /// <summary>
        /// verifica la firma della fattura ed estrae il file originale
        /// </summary>
        /// <param name="pathFileSign">fattura firmata</param>
        /// <param name="pathFile">path e file della fattura</param>
        /// <param name="lastError">ultimo errore nella funzionalità</param>
        /// <returns>verifica e estrazione avvenuta con successo</returns>
        /// <example>
        /// if (Utilities.VerificaEstraiFirma(@"c:\temp\IT01234567890_FPA01.xml.p7m", out pathFile, ref lastError))
        /// {
        ///       pathFile -> c:\temp\IT01234567890_FPA01.xml
        /// }
        /// </example>
        public static bool VerificaEstraiFirma(string pathFileSign, out string pathFile, ref string lastError)
        {
            bool success = false;
            pathFile = null;
            try
            {
                string fileName = Path.GetFileName(pathFileSign);

                if (!Utilities.IsValidNomeFileFattura(fileName, ref lastError))
                {
                    return success;
                }

                if (Utilities.glob.UnlockStatus == 0)
                {
                    lastError = "Licenza bloccata";
                    return success;
                }

                Chilkat.Crypt2 crypt = new Chilkat.Crypt2();

                string outputFile = Path.ChangeExtension(pathFileSign, null);


                //  Verify and restore the original file:
                success = crypt.VerifyP7M(pathFileSign, outputFile);

                if (!success)
                {
                    lastError = crypt.LastErrorText;
                    return success;
                }

                pathFile = outputFile;
                success = true;
            }
            catch
            {
                throw;
            }

            return success;

        }

        /// <summary>
        /// applica la marca temporale alla fattura elettronica
        /// </summary>
        /// <param name="pathFileSign">file firmato della fattura elettronica</param>
        /// <param name="tsaUrl">url TSA</param>
        /// <param name="pathFileTimeStamped">file tsr da TSA</param>
        /// <param name="lastError">ultimo errore nella funzionalità</param>
        /// <param name="userName">user TSA (opzionale)</param>
        /// <param name="password">password TSA (opzionale)</param>
        /// <returns>true se la funzionalità ha avuto successo</returns>
        /// <example>
        ///  if (Utilities.MarcaTemporale(@"c:\temp\IT01234567890_FPA01.xml.p7m", "https://freetsa.org/tsr", out pathFileTimeStamped, ref lastError, "myUser", "myPassword"))
        ///  {
        ///       pathFileTimeStamped -> c:\temp\IT01234567890_FPA01.xml.p7m.tsr
        ///  }
        /// </example>
        public static bool MarcaTemporale(string pathFileSign, string tsaUrl, out string pathFileTimeStamped, ref string lastError, string userName = null, string password = null)
        {

            bool success = false;
            pathFileTimeStamped = null;
            try
            {
                string fileName = Path.GetFileName(pathFileSign);

                if (!Utilities.IsValidNomeFileFattura(fileName, ref lastError))
                {
                    return success;
                }

                if (Utilities.glob.UnlockStatus == 0)
                {
                    lastError = "Licenza bloccata";
                    return success;
                }

                Chilkat.Crypt2 crypt = new Chilkat.Crypt2();
                crypt.HashAlgorithm = "sha256";
                crypt.EncodingMode = "base64";

                string base64Hash = crypt.HashFileENC(pathFileSign);

                Chilkat.Http http = new Chilkat.Http();

                Chilkat.BinData requestToken = new Chilkat.BinData();
                string optionalPolicyOid = string.Empty;
                bool addNonce = false;
                bool requestTsaCert = false;

                if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
                {
                    http.Login = userName;
                    http.Password = password;
                    http.BasicAuth = true;
                }

                //  Create a time-stamp request token
                success = http.CreateTimestampRequest("sha256", base64Hash, optionalPolicyOid, addNonce, requestTsaCert, requestToken);
                if (!success)
                {
                    lastError = http.LastErrorText;
                    return success;
                }

                
                Chilkat.HttpResponse resp = http.PBinaryBd("POST", tsaUrl, requestToken, "application/timestamp-query", false, false);
                if (!http.LastMethodSuccess)
                {
                    lastError = http.LastErrorText;
                    return success;
                }

                
                Chilkat.BinData timestampReply = new Chilkat.BinData();
                resp.GetBodyBd(timestampReply);
                if (!timestampReply.LastMethodSuccess)
                {
                    return success;
                }

                
                string s = Path.ChangeExtension(fileName, $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.tsr)}");
                success = timestampReply.WriteFile(s);
                if (success)
                {
                    pathFileTimeStamped = s;
                }
                
            }
            catch
            {
                throw;
            }

            return success;
        }

        /// <summary>
        /// converte file tsr in formato tsd (RFC 5544)
        /// </summary>
        /// <param name="pathFileTsr">file tsr</param>
        /// <param name="pathFileSign">file fattura firmata</param>
        /// <param name="pathFileTsd">file tsd creato</param>
        /// <param name="lastError">ultimo errore della funzionalità</param>
        /// <returns>true se la funzionalità ha avuto successo</returns>
        public static bool CreaTsd(string pathFileTsr, string pathFileSign, out string pathFileTsd, ref string lastError)
        {
            bool success = false;
            pathFileTsd = null;
            try
            {
                string fileName = Path.GetFileName(pathFileSign);

                if (!Utilities.IsValidNomeFileFattura(fileName, ref lastError))
                {
                    return success;
                }


                if (Utilities.glob.UnlockStatus == 0)
                {
                    lastError = "Licenza bloccata";
                    return success;
                }


                Chilkat.BinData bdTsr = new Chilkat.BinData();
                success = bdTsr.LoadFile(pathFileTsr);
                if (!success)
                {
                    lastError = "Errore nel caricare il file tsr!";
                    return success;
                }

                // Carico il tsr in un oggetto ASN.1
                Chilkat.Asn asnTsr = new Chilkat.Asn();
                success = asnTsr.LoadEncoded(bdTsr.GetEncoded("base64"), "base64");
                if (!success)
                {
                    lastError = asnTsr.LastErrorText;
                    return success;
                }

                // Prendo il timestamp nell'xml
                Chilkat.Xml xmlTsr = new Chilkat.Xml();
                xmlTsr.LoadXml(asnTsr.AsnToXml());

                // Il timestamp inizia in questo modo
                // Rimuovo la prima sub-tree

                //     <?xml version="1.0" encoding="utf-8"?>
                //     <sequence>
                //         <sequence>       <---- Remove this sub-tree.
                //             <int>00</int>
                //             <sequence>
                //                 <utf8>Operation Okay</utf8>
                //             </sequence>
                //         </sequence>
                //         <sequence>   
                //             <oid>1.2.840.113549.1.7.2</oid>
                //             <contextSpecific tag="0" constructed="1">
                //             ...

                // Rimuovo la prima sub-tree..
                xmlTsr.RemoveChildByIndex(0);

                // Combiniamo il timestamp e il .p7m in un timestampData
                Chilkat.BinData bdContent = new Chilkat.BinData();
                success = bdContent.LoadFile(pathFileSign);
                if (!success)
                {
                    lastError = "Errore nel caricare il file p7m!";
                    return success;
                }

                // Costruisco il TimeStampData.  
                // Lo costruisco in XML e poi lo converto in ASN.1
                Chilkat.Xml xml = new Chilkat.Xml();
                xml.Tag = "sequence";
                xml.UpdateChildContent("oid", "1.2.840.113549.1.9.16.1.31");
                xml.UpdateAttrAt("contextSpecific", true, "tag", "0");
                xml.UpdateAttrAt("contextSpecific", true, "constructed", "1");
                xml.UpdateChildContent("contextSpecific|sequence|int", "01");
                xml.UpdateChildContent("contextSpecific|sequence|octets", bdContent.GetEncoded("base64"));
                xml.UpdateAttrAt("contextSpecific|sequence|contextSpecific", true, "tag", "0");
                xml.UpdateAttrAt("contextSpecific|sequence|contextSpecific", true, "constructed", "1");

                Chilkat.Xml xContext = xml.GetChildWithTag("contextSpecific|sequence|contextSpecific");
                xContext.AddChildTree(xmlTsr);

                // Converto il file XML in ASN.1
                Chilkat.Asn tsd = new Chilkat.Asn();
                tsd.LoadAsnXml(xml.GetXml());

                // scrivo il timestamped
                success = tsd.WriteBinaryDer(pathFileTsd);
            }
            catch
            {
                throw;
            }

            return success;
        }

        /// <summary>
        /// verifica se il nome del file della fattura è formalmente corretto
        /// </summary>
        /// <param name="fileName">nome del file</param>
        /// <param name="lastError">ultimo errore della funzionalità</param>
        /// <param name="signed">indicare se si tratta di un file firmato</param>
        /// <returns>true se è formalmente corretto il nome del file della fattura</returns>
        public static bool IsValidNomeFileFattura(string fileName, ref string lastError, bool signed = true)
        {
            string s = string.Empty;
            if (signed)
            {
                s = $@"\.{Enum.GetName(typeof(EstensioniFile), EstensioniFile.p7m)}";
            }

            bool verifyName = Regex.IsMatch(fileName, @"^[A-Z]{2}[0-9]{11}_[0-9A-Z]{5}\.(?i:xml" + s + ")$", RegexOptions.Multiline);
            if (!verifyName)
            {
                lastError = "Nome file fattura formalmente non corretto!";
                
            }

            return verifyName;
        }

    }
}
