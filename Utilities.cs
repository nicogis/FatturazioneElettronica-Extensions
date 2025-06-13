//-----------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Studio A&T s.r.l.">
//     Copyright (c) Studio A&T s.r.l. All rights reserved.
// </copyright>
// <author>Nicogis</author>
//-----------------------------------------------------------------------
namespace FatturazioneElettronica.Extensions
{
    using Chilkat;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// utilities per la fatturazione elettronica e la conservazione sostitutiva
    /// la libreria utilizzata la libreria chilkat http://www.chilkatsoft.com/
    /// Per utilizzarla in modalita trial (30gg) passare una stringa qualsiasi per il parametro unlock del metodo UnlockLicense
    /// </summary>
    public static partial class Utilities
    {
        private static Global glob;

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
            bool success;
            try
            {
                Utilities.glob = new Global();
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
        /// calcolo hash di un file
        /// </summary>
        /// <param name="pathFile">path and file</param>
        /// <param name="lastError">ultimo errore nella funzionalità</param>
        /// <param name="algorithm">algoritmo da utilizzare</param>
        /// <param name="encode">encode</param>
        /// <returns>hash del file</returns>
        public static string HashFile(string pathFile, ref string lastError, string algorithm = "sha256", string encode = "base64")
        {
            string hash = null;
            try
            {
                //  Controlli preliminari
                //  *******************************************************************
                if (!File.Exists(pathFile))
                {
                    lastError = $"Il file '{pathFile}' non è stato trovato!";
                    return hash;
                }
                //  *******************************************************************

                if (Utilities.glob.UnlockStatus == 0)
                {
                    lastError = "Licenza bloccata";
                    return hash;
                }

                Crypt2 crypt = new Crypt2
                {
                    HashAlgorithm = algorithm,
                    EncodingMode = encode
                };
                hash = crypt.HashFileENC(pathFile);
            }
            catch
            {
                throw;
            }

            return hash;

        }

        /// <summary>
        /// Firma del file
        /// </summary>
        /// <param name="pathFile">file da firmare</param>
        /// <param name="lastError">ultimo errore nella funzionalità</param>
        /// <param name="certSpec">
        /// Criterio per selezionare il certificato da un HSM o dallo store.
        /// I valori supportati sono:
        /// 
        /// - <b>""</b>: (stringa vuota) Se è collegato un solo HSM con un solo certificato contenente la chiave privata,
        ///   Chilkat selezionerà automaticamente quel certificato.
        /// 
        /// - <b>cn=&lt;common name&gt;</b>: Cerca e carica il certificato con il CN (Common Name) specificato.
        ///   Esempio: <c>cn=Mario Rossi</c>
        /// 
        /// - <b>subjectdn_withtags=&lt;DN con tag&gt;</b>: Cerca il certificato con DN contenente tag (es. C, O, OU, CN).
        ///   Esempio: <c>subjectdn_withtags=C=IT, O=Company, CN=www.site.it</c>
        /// 
        /// - <b>subjectdn=&lt;DN senza tag&gt;</b>: Cerca il certificato con DN senza tag.
        ///   Esempio: <c>subjectdn=IT, Roma, Company, www.site.it, info@site.it</c>
        /// 
        /// - <b>issuercn=&lt;seriale:CN emittente&gt;</b>: Cerca per numero di serie esadecimale e CN dell’emittente.
        ///   Esempio: <c>01020304:Let's Encrypt Authority X1</c>
        /// 
        /// - <b>serial=&lt;numero di serie esadecimale&gt;</b>: Cerca per numero seriale esadecimale.
        /// 
        /// - <b>thumbprint=&lt;SHA1 hex&gt;</b>: Cerca per impronta digitale SHA1 esadecimale.
        /// 
        /// - <b>policyoid=&lt;OID&gt;</b>: Cerca per policy OID del certificato. Esempio: <c>2.16.840.1.101.2.1.11.39</c>
        /// 
        /// - <b>o=&lt;organizzazione&gt;</b>: Cerca per Organization (O).
        /// - <b>c=&lt;paese&gt;</b>: Cerca per Country (C).
        /// - <b>l=&lt;località&gt;</b>: Cerca per Locality (L).
        /// - <b>ou=&lt;unità organizzativa&gt;</b>: Cerca per Organizational Unit (OU).
        /// - <b>st=&lt;stato/regione&gt;</b>: Cerca per State/Province (ST).
        /// - <b>e=&lt;email&gt;</b>: Cerca per Email (E).
        /// 
        /// - <b>[Solo Windows]</b>: È possibile specificare anche il nome di un CSP (Cryptographic Service Provider),
        ///   ad esempio <c>YubiHSM Key Storage Provider</c>. Questa modalità è mantenuta per retrocompatibilità:
        ///   in passato Chilkat gestiva HSM, token USB e smart card su Windows solo tramite CSP.
        ///   Oggi Chilkat supporta HSM multipiattaforma (Windows, Linux, macOS, iOS, Alpine Linux),
        ///   usando metodi come PKCS11, macOS Keychain, Windows ScMinidriver, MsCNG e CryptoAPI legacy.
        ///   Il provider viene rilevato automaticamente e viene scelto il metodo ottimale.
        /// 
        /// Alcuni esempi di CSP validi:
        /// - Microsoft Smart Card Key Storage Provider
        /// - Microsoft Base Smart Card Crypto Provider
        /// - Bit4id Universal Middleware Provider
        /// - YubiHSM Key Storage Provider (da v9.5.0.83)
        /// - SafeSign Standard Cryptographic Service Provider
        /// - eToken Base Cryptographic Provider
        /// - cryptoCertum3 CSP
        /// - FTSafe ePass1000 RSA Cryptographic Service Provider
        /// - SecureStoreCSP
        /// - Gemalto Classic Card CSP
        /// - EnterSafe ePass2003 CSP v1.0 / v2.0
        /// - PROXKey CSP India V1.0 / V2.0
        /// - TRUST KEY CSP V1.0
        /// - Watchdata Brazil CSP V1.0
        /// - Luna Cryptographic Services for Microsoft Windows
        /// - Safenet RSA Full Cryptographic Provider
        /// - nCipher Enhanced Cryptographic Provider
        /// - MySmartLogon NFC CSP
        /// - ActivClient Cryptographic Service Provider
        /// - Athena ASECard Crypto CSP
        /// - e molti altri...
        /// </param>
        /// <param name="pin">pin smartcard/usb (opzionale). Se non fornito comparirà una finestra di dialogo del sistema operativo Windows per indicare il pin</param>
        /// <param name="formatoFirma">formato di firma digitale da generare. Per tutti i tipi di file utilizza il formato CAdES mentre per i file xml è possibile utilizzare anche il formato XAdES</param>
        /// <returns>firma avvenuta con successo</returns>
        public static bool Firma(string pathFile, ref string lastError, string certSpec = "", string pin = null, FormatiFirma formatoFirma = FormatiFirma.CAdES)
        {
            bool success = false;
            try
            {
                string sigFile = null;

                //  Controlli preliminari
                //  *******************************************************************
                if (!File.Exists(pathFile))
                {
                    lastError = $"Il file '{pathFile}' non è stato trovato!";
                    return success;
                }

                if (formatoFirma == FormatiFirma.CAdES)
                {
                    sigFile = $"{pathFile}.{Enum.GetName(typeof(EstensioniFile), EstensioniFile.p7m)}";
                }
                else if (formatoFirma == FormatiFirma.XAdES)
                {
                    if (Path.GetExtension(pathFile).ToLowerInvariant() != $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.xml)}")
                    {
                        lastError = $"Per il formato firma XAdES indicare solo file in formato {Enum.GetName(typeof(EstensioniFile), EstensioniFile.xml)}!";
                        return success;
                    }

                    sigFile = $"{Path.Combine(Path.GetDirectoryName(pathFile), Path.GetFileNameWithoutExtension(pathFile))}_signed.{Enum.GetName(typeof(EstensioniFile), EstensioniFile.xml)}";

                }
                else
                {
                    lastError = "Formato firma non riconosciuto!";
                    return success;
                }

                if (File.Exists(sigFile))
                {
                    lastError = $"Il file {sigFile} è già presente: eliminarlo prima di rifirmarlo!";
                    return success;
                }
                //  *******************************************************************


                if (Utilities.glob.UnlockStatus == 0)
                {
                    lastError = "Licenza bloccata";
                    return success;
                }

                //  Utilizza il certificato su una smartcard o su USB.
                Cert cert = new Cert();

                //  Fornire il PIN della smartcard.
                //  Se il pin non è fornito esplicitamente qui,
                //  Se non fornito dovrebbe comparire una finestra di dialogo del sistema operativo Windows per indicare il pin
                if (!string.IsNullOrWhiteSpace(pin))
                {
                    cert.SmartCardPin = pin;
                }

                success = cert.LoadFromSmartcard(certSpec);
                if (success != true)
                {
                    lastError = cert.LastErrorText;
                    return success;
                }
                
                if (formatoFirma == FormatiFirma.CAdES)
                {
                    Crypt2 crypt = new Crypt2();

                    //  Fornisce il certificato per firmarlo
                    success = crypt.SetSigningCert(cert);
                    if (success != true)
                    {
                        lastError = crypt.LastErrorText;
                        return success;
                    }

                    //  Indica l'algoritmo da utilizzare
                    crypt.HashAlgorithm = "sha256";
                    crypt.CadesEnabled = true;

                    //  Specifico gli attributi firmati per essere inclusi.
                    //  (Questo è quello che fa un CAdES-BES compliant.)
                    JsonObject jsonSignedAttrs = new JsonObject();
                    jsonSignedAttrs.UpdateInt("contentType", 1);
                    jsonSignedAttrs.UpdateInt("signingTime", 1);
                    jsonSignedAttrs.UpdateInt("messageDigest", 1);
                    jsonSignedAttrs.UpdateInt("signingCertificateV2", 1);
                    crypt.SigningAttributes = jsonSignedAttrs.Emit();

                    //  Creo una firma CAdES-BES, che contiene i dati originali
                    success = crypt.CreateP7M(pathFile, sigFile);
                    if (!success)
                    {
                        lastError = crypt.LastErrorText;
                        return success;
                    }
                }
                else if (formatoFirma == FormatiFirma.XAdES)
                {
                    Xml xmlToSign = new Xml();
                    success = xmlToSign.LoadXmlFile(pathFile);
                    if (!success)
                    {
                        lastError = xmlToSign.LastErrorText;
                        return success;
                    }

                    XmlDSigGen gen = new XmlDSigGen();
                    string id = $"xmldsig-{Guid.NewGuid()}";
                    string nameSpace = xmlToSign.GetRoot().Tag;
                    
                    gen.SigLocation = nameSpace;
                    gen.SigId = id;
                    gen.SigNamespacePrefix = "ds";
                    gen.SigNamespaceUri = "http://www.w3.org/2000/09/xmldsig#";
                    gen.SigValueId = $"{id}-sigvalue";
                    gen.SignedInfoCanonAlg = "C14N";
                    gen.SignedInfoDigestMethod = "sha256";


                    // Nota: Chilkat automaticamente compilerà il testo "TO BE GENERATED BY CHILKAT" con i valori corretti quando l'xml verrà firmato

                    Xml object1 = new Xml
                    {
                        Tag = "xades:QualifyingProperties"
                    };
                    object1.AddAttribute("xmlns:xades", "http://uri.etsi.org/01903/v1.3.2#");
                    object1.AddAttribute("xmlns:xades141", "http://uri.etsi.org/01903/v1.4.1#");
                    object1.AddAttribute("Target", $"#{id}");
                    object1.UpdateAttrAt("xades:SignedProperties", true, "Id", $"{id}-signedprops");
                    object1.UpdateChildContent("xades:SignedProperties|xades:SignedSignatureProperties|xades:SigningTime", "TO BE GENERATED BY CHILKAT");
                    object1.UpdateAttrAt("xades:SignedProperties|xades:SignedSignatureProperties|xades:SigningCertificate|xades:Cert|xades:CertDigest|ds:DigestMethod", true, "Algorithm", "http://www.w3.org/2001/04/xmlenc#sha256");
                    object1.UpdateChildContent("xades:SignedProperties|xades:SignedSignatureProperties|xades:SigningCertificate|xades:Cert|xades:CertDigest|ds:DigestValue", "TO BE GENERATED BY CHILKAT");
                    object1.UpdateChildContent("xades:SignedProperties|xades:SignedSignatureProperties|xades:SigningCertificate|xades:Cert|xades:IssuerSerial|ds:X509IssuerName", "TO BE GENERATED BY CHILKAT");
                    object1.UpdateChildContent("xades:SignedProperties|xades:SignedSignatureProperties|xades:SigningCertificate|xades:Cert|xades:IssuerSerial|ds:X509SerialNumber", "TO BE GENERATED BY CHILKAT");

                    gen.AddObject("", object1.GetXml(), "", "");

                    // -------- Reference 1 --------
                    gen.KeyInfoId = $"{id}-keyinfo";
                    gen.AddSameDocRef($"{id}-keyinfo", "sha256", "", "", "");

                    // -------- Reference 2 --------
                    gen.AddSameDocRef("", "sha256", "", "", "");
                    gen.SetRefIdAttr("", $"{id}-ref0");

                    // -------- Reference 3 --------
                    gen.AddObjectRef($"{id}-signedprops", "sha256", "", "", "http://uri.etsi.org/01903#SignedProperties");

                    gen.SetX509Cert(cert, true);
                    gen.KeyInfoType = "X509Data";
                    gen.X509Type = "Certificate";

                    // Carica l'xml da firmare ...
                    StringBuilder sbXml = new StringBuilder();
                    xmlToSign.GetXmlSb(sbXml);

                    gen.Behaviors = "IndentedSignature,ForceAddEnvelopedSignatureTransform";

                    // Firma l'XML...
                    success = gen.CreateXmlDSigSb(sbXml);
                    if (!success)
                    {
                        lastError = xmlToSign.LastErrorText;
                        return success;
                    }

                    
                    
                    // Salva il file xml firmato.
                    success = sbXml.WriteFile(sigFile, "utf-8", false);

                    if (!success)
                    {
                        lastError = "Errore nel salvare il file firmato!";
                        return success;
                    }

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
        /// verifica la firma CAdES (file p7m) o XAdES (file xml)
        /// </summary>
        /// <param name="pathFileSign">documento firmato</param>
        /// <param name="lastError">ultimo errore nella funzionalità</param>
        /// <returns>se true la verifica è avvenuta con successo</returns>
        public static bool VerificaFirma(string pathFileSign, ref string lastError)
        {
            bool success = false;
            try
            {
                // controlli preliminari
                // ***************************************************
                string ext = Path.GetExtension(pathFileSign).ToLowerInvariant();
                if (!((ext == $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.p7m)}") || (ext == $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.xml)}")))
                {
                    lastError = $"Formato non riconosciuto: solo {Enum.GetName(typeof(EstensioniFile), EstensioniFile.p7m)} o {Enum.GetName(typeof(EstensioniFile), EstensioniFile.xml)}!";
                    return success;
                }

                if (!File.Exists(pathFileSign))
                {
                    lastError = "File da verificare non trovato!";
                    return success;
                }
                // ***************************************************


                if (Utilities.glob.UnlockStatus == 0)
                {
                    lastError = "Licenza bloccata";
                    return success;
                }

                
                if (ext == $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.p7m)}")
                {
                    // CAdES
                    BinData bd = new BinData();

                    success = bd.LoadFile(pathFileSign);
                    if (!success)
                    {
                        lastError = "Errore a caricare il file";
                        return success;
                    }

                    Crypt2 crypt = new Crypt2();

                    // Verifica ed estrae il payload contenuto nel .p7m.
                    success = crypt.OpaqueVerifyBd(bd);

                    if (!success)
                    {
                        lastError = crypt.LastErrorText;
                        return success;
                    }
                }
                else if (ext == $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.xml)}")
                {
                    // XAdES
                    StringBuilder sbXml = new StringBuilder();
                    success = sbXml.LoadFile(pathFileSign, "utf-8");
                    if (!success)
                    {
                        lastError = "Errore a caricare il file.";
                        return success;
                    }

                    XmlDSig dsig = new XmlDSig();

                    // Carico il file xml firmato.
                    success = dsig.LoadSignatureSb(sbXml);
                    if (!success)
                    {
                        lastError = dsig.LastErrorText;
                        return success;
                    }

                    // non sono presenti firme
                    if (dsig.NumSignatures == 0)
                    {
                        lastError = "Non sono presenti firme!";
                        return success;
                    }

                    // Un file xml può avere multiple firme.
                    // Verifico ogni firma:
                    int i = 0;

                    // verifico anche ogni riferimento digest.
                    // se si imposta verifyReferenceDigests a false è verificata solo la parte di signedInfo della firma
                    bool verifyReferenceDigests = true;
                    bool verified = false;
                    string message = null;
                    bool failed = false;
                    while (i < dsig.NumSignatures)
                    {
                        // selezione la i-esima firma da controllare.
                        dsig.Selector = i;
                        verified = dsig.VerifySignature(verifyReferenceDigests);
                        message += $"Firma numero :{Convert.ToString(i + 1)} verifica : {Convert.ToString(verified)}\n";
                        if ((!verified) && (!failed))
                        {
                            failed = true;
                        }

                        i++;
                    }

                    if (failed)
                    {
                        lastError = message;
                        return success;
                    }
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
        /// verifica la firma CAdES ed estrae il file originale. Il file verrà creato nella stessa cartella senza estensione
        /// </summary>
        /// <param name="pathFileSign">documento firmato</param>
        /// <param name="pathFile">path e nome file</param>
        /// <param name="lastError">ultimo errore nella funzionalità</param>
        /// <returns>verifica e estrazione avvenuta con successo</returns>
        /// <example>
        /// if (Utilities.VerificaEstraiFirma(@"c:\temp\IT01234567890_FPA01.xml.p7m", out pathFile, ref lastError))
        /// {
        ///       // pathFile -> c:\temp\IT01234567890_FPA01.xml
        /// }
        /// </example>
        public static bool VerificaEstraiFirma(string pathFileSign, out string pathFile, ref string lastError)
        {
            pathFile = null;
            string outputFile = Path.ChangeExtension(pathFileSign, null);
            bool success = Utilities.VerificaEstraiFirma(pathFileSign, outputFile, ref lastError);
            if (success)
            {
                pathFile = outputFile;
            }

            return success;
            
        }

        /// <summary>
        /// Verifica la firma CAdES ed estrae il file originale
        /// </summary>
        /// <param name="pathFileSign">documento firmato</param>
        /// <param name="pathFile">path e nome file</param>
        /// <param name="lastError">ultimo errore nella funzionalità</param>
        /// <returns>verifica e estrazione avvenuta con successo</returns>
        /// <example>
        ///    string pathFileOut = "c:\file\test.xml"
        ///    Utilities.VerificaEstraiFirma(@"c:\temp\IT01234567890_FPA01.xml.p7m", pathFileOut, ref lastError))
        /// 
        ///    // c:\file\test.xml file estratto
        /// 
        /// </example>
        public static bool VerificaEstraiFirma(string pathFileSign, string pathFile, ref string lastError)
        {
            bool success = false;
            try
            {

                // controlli preliminari
                // ***************************************************
                if (!File.Exists(pathFileSign))
                {
                    lastError = "File da verificare non trovato!";
                    return success;
                }

                if (pathFileSign.ToLowerInvariant() == pathFile.ToLowerInvariant())
                {
                    lastError = $"Il percorso/nome file da verificare è uguale a quello da estrarre!";
                    return success;
                }

                string ext = Path.GetExtension(pathFileSign).ToLowerInvariant();
                if (ext != $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.p7m)}")
                {
                    lastError = $"Formato non riconosciuto: solo {Enum.GetName(typeof(EstensioniFile), EstensioniFile.p7m)}!";
                    return success;
                }

                if (File.Exists(pathFile))
                {
                    lastError = $"Il file '{pathFile}' è già presente: eliminarlo prima di riestrarlo";
                    return success;
                }

                // ***************************************************


                if (Utilities.glob.UnlockStatus == 0)
                {
                    lastError = "Licenza bloccata";
                    return success;
                }

                Crypt2 crypt = new Crypt2();

                //  Verify and restore the original file:
                success = crypt.VerifyP7M(pathFileSign, pathFile);

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
        /// applica la marca temporale al file
        /// </summary>
        /// <param name="pathFileSign">file firmato</param>
        /// <param name="tsaUrl">url TSA</param>
        /// <param name="pathFileTimeStamped">file tsr da TSA</param>
        /// <param name="lastError">ultimo errore nella funzionalità</param>
        /// <param name="userName">user TSA (opzionale)</param>
        /// <param name="password">password TSA (opzionale)</param>
        /// <returns>true se la funzionalità ha avuto successo</returns>
        /// <example>
        ///  if (Utilities.MarcaTemporale(@"c:\temp\IT01234567890_FPA01.xml.p7m", "https://freetsa.org/tsr", out pathFileTimeStamped, ref lastError, "myUser", "myPassword"))
        ///  {
        ///       // pathFileTimeStamped -> c:\temp\IT01234567890_FPA01.xml.tsr
        ///  }
        /// </example>
        public static bool MarcaTemporale(string pathFileSign, string tsaUrl, out string pathFileTimeStamped, ref string lastError, string userName = null, string password = null)
        {

            bool success = false;
            pathFileTimeStamped = null;
            try
            {

                // controlli preliminari
                // ***************************************************
                if (!File.Exists(pathFileSign))
                {
                    lastError = $"Il file '{pathFileSign}' non è stato trovato!";
                    return success;
                }

                
                string output = Path.ChangeExtension(pathFileSign, $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.tsr)}");

                if (File.Exists(output))
                {
                    lastError = $"Il file '{output}' è già presente: eliminarlo prima di ricrearlo!";
                    return success;
                }

                // ***************************************************

                if (Utilities.glob.UnlockStatus == 0)
                {
                    lastError = "Licenza bloccata";
                    return success;
                }

                Crypt2 crypt = new Crypt2
                {
                    HashAlgorithm = "sha256",
                    EncodingMode = "base64"
                };

                string base64Hash = crypt.HashFileENC(pathFileSign);

                Http http = new Http();

                BinData requestToken = new BinData();
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

                
                HttpResponse resp = http.PBinaryBd("POST", tsaUrl, requestToken, "application/timestamp-query", false, false);
                if (!http.LastMethodSuccess)
                {
                    lastError = http.LastErrorText;
                    return success;
                }

                
                BinData timestampReply = new BinData();
                resp.GetBodyBd(timestampReply);
                if (!timestampReply.LastMethodSuccess)
                {
                    return success;
                }
               
                success = timestampReply.WriteFile(output);
                if (success)
                {
                    pathFileTimeStamped = output;
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
        /// <param name="pathFileSign">file firmato</param>
        /// <param name="pathFileTsd">file tsd creato</param>
        /// <param name="lastError">ultimo errore della funzionalità</param>
        /// <returns>true se la funzionalità ha avuto successo</returns>
        /// <example>
        ///  if (Utilities.CreaTsd(@"c:\temp\IT01234567890_FPA01.xml.tsr", @"c:\temp\IT01234567890_FPA01.xml.p7m", out pathFileTsd, ref lastError))
        ///  {
        ///       // pathFileTsd -> c:\temp\IT01234567890_FPA01.xml.tsd
        ///  }
        /// </example>
        public static bool CreaTsd(string pathFileTsr, string pathFileSign, out string pathFileTsd, ref string lastError)
        {
            bool success = false;
            pathFileTsd = null;
            try
            {

                // controlli preliminari
                // ***************************************************
                if (!File.Exists(pathFileTsr))
                {
                    lastError = $"Il file '{pathFileTsr}' non è stato trovato!";
                    return success;
                }

                if (Path.GetExtension(pathFileTsr).ToLowerInvariant() != $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.tsr)}")
                {
                    lastError = $"Formato non riconosciuto per il file {pathFileTsr}: solo {Enum.GetName(typeof(EstensioniFile), EstensioniFile.tsr)}!";
                    return success;
                }

                if (!File.Exists(pathFileSign))
                {
                    lastError = $"Il file '{pathFileSign}' non è stato trovato!";
                    return success;
                }

                if (Path.GetExtension(pathFileSign).ToLowerInvariant() != $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.p7m)}")
                {
                    lastError = $"Formato non riconosciuto per il file {pathFileSign}: solo {Enum.GetName(typeof(EstensioniFile), EstensioniFile.p7m)}!";
                    return success;
                }

                
                string output = Path.ChangeExtension(pathFileTsr, $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.tsd)}");

                if (File.Exists(output))
                {
                    lastError = $"Il file '{output}' è già presente: eliminarlo prima di ricrearlo!";
                    return success;
                }
                
                // ***************************************************


                if (Utilities.glob.UnlockStatus == 0)
                {
                    lastError = "Licenza bloccata";
                    return success;
                }


                BinData bdTsr = new BinData();
                success = bdTsr.LoadFile(pathFileTsr);
                if (!success)
                {
                    lastError = "Errore nel caricare il file tsr!";
                    return success;
                }

                // Carico il tsr in un oggetto ASN.1
                Asn asnTsr = new Asn();
                success = asnTsr.LoadEncoded(bdTsr.GetEncoded("base64"), "base64");
                if (!success)
                {
                    lastError = asnTsr.LastErrorText;
                    return success;
                }

                // Prendo il timestamp nell'xml
                Xml xmlTsr = new Xml();
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
                BinData bdContent = new BinData();
                success = bdContent.LoadFile(pathFileSign);
                if (!success)
                {
                    lastError = "Errore nel caricare il file p7m!";
                    return success;
                }

                // Costruisco il TimeStampData.  
                // Lo costruisco in XML e poi lo converto in ASN.1
                Xml xml = new Xml
                {
                    Tag = "sequence"
                };
                xml.UpdateChildContent("oid", "1.2.840.113549.1.9.16.1.31");
                xml.UpdateAttrAt("contextSpecific", true, "tag", "0");
                xml.UpdateAttrAt("contextSpecific", true, "constructed", "1");
                xml.UpdateChildContent("contextSpecific|sequence|int", "01");
                xml.UpdateChildContent("contextSpecific|sequence|octets", bdContent.GetEncoded("base64"));
                xml.UpdateAttrAt("contextSpecific|sequence|contextSpecific", true, "tag", "0");
                xml.UpdateAttrAt("contextSpecific|sequence|contextSpecific", true, "constructed", "1");

                Xml xContext = xml.GetChildWithTag("contextSpecific|sequence|contextSpecific");
                xContext.AddChildTree(xmlTsr);

                // Converto il file XML in ASN.1
                Asn tsd = new Asn();
                tsd.LoadAsnXml(xml.GetXml());

               
                // scrivo il timestamped
                success = tsd.WriteBinaryDer(output);

                if (success)
                {
                    pathFileTsd = output;
                }

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
        /// <returns>true se è formalmente corretto il nome del file della fattura</returns>
        public static bool IsValidNomeFileFattura(string fileName, ref string lastError)
        {
            try
            {
                string name = fileName; 
                
                if (Path.GetExtension(fileName).ToLowerInvariant() == $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.p7m)}")
                {
                    name = Path.GetFileNameWithoutExtension(fileName);
                }

                if (Path.GetExtension(name).ToLowerInvariant() != $".{Enum.GetName(typeof(EstensioniFile), EstensioniFile.xml)}")
                {
                    throw new Exception("Estensione file non valida!");
                }

                name = Path.GetFileNameWithoutExtension(name);

                string iso3166 = name.Substring(0, 2);

                if (iso3166 != iso3166.ToUpperInvariant())
                {
                    throw new Exception("Codice paese non valido!");
                }

                RegionInfo regionInfo = null;
                try
                {
                    regionInfo = new RegionInfo(iso3166);
                }
                catch
                {
                    throw new Exception("Codice paese non trovato!");
                }

                name = name.Substring(2);

                string[] s = name.Split('_');

                if (s.Length != 2)
                {
                    throw new Exception("Nome file fattura formalmente non corretto!");
                }

                string identificativo = s[0];
                string progressivo = s[1];

                if (regionInfo.TwoLetterISORegionName == "IT")
                {
                    if (identificativo.Length == 11)
                    {
                        if (!identificativo.All(char.IsDigit))
                        {
                            throw new Exception("L'identificativo da 11 deve avere solo numeri IT!");
                        }
                    }
                    else if (identificativo.Length == 16)
                    {
                        if (!identificativo.All(char.IsLetterOrDigit))
                        {
                            throw new Exception("L'identificativo da 16 deve avere solo caratteri alfanumerici IT!");
                        }
                    }
                    else
                    {
                        throw new Exception("L'identificativo deve essere di 11 o 16 caratteri per IT!");
                    }
                }
                else
                {
                    if ((identificativo.Length >= 2) && (identificativo.Length <= 28))
                    {
                        if (!identificativo.All(char.IsLetterOrDigit))
                        {
                            throw new Exception("L'identificativo deve avere solo caratteri alfanumerici!");
                        }
                    }
                    else
                    {
                        throw new Exception("L'identificativo deve essere compreso tra 2 e 28 caratteri per non IT!");
                    }
                }

                if ((progressivo.Length >= 1) && (progressivo.Length <= 5))
                {
                    if (!progressivo.All(char.IsLetterOrDigit))
                    {
                        throw new Exception("Il progressivo deve avere solo caratteri alfanumerici!");
                    }
                }
                else
                {
                    throw new Exception("Il progressivo deve avere fino ad un massimo di 5 caratteri alfanumerici!");
                }

                return true;
            }
            catch(Exception ex)
            {
                lastError = ex.Message;
                return false;
            }
        }

    }
}
