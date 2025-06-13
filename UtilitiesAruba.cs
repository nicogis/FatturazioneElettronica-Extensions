using Chilkat;
using System;
using System.IO;

namespace FatturazioneElettronica.Extensions
{
    public static partial class Utilities
    {

        /// <summary>
        /// Firma CAdES (p7m) con il servizio ARSS (Aruba Remote Signing Service)
        /// </summary>
        /// <param name="pathFile">Percorso completo del file da firmare.</param>
        /// <param name="certificato">Certificato in formato binario (senza chiave privata).Carica un certificato X.509 da byte con codifica ASN.1 DER. Nota: I dati possono contenere il certificato in qualsiasi formato.Può essere binario DER (ASN.1), PEM, Base64, ecc. il metodo rileverà automaticamente il formato.</param>
        /// <param name="lastError">Parametro di output che conterrà il messaggio di errore in caso di fallimento.</param>
        /// <param name="certId">ID del certificato remoto da utilizzare per la firma.</param>
        /// <param name="user">Nome utente per l'autenticazione al servizio ARSS.</param>
        /// <param name="password">Password per l'autenticazione al servizio ARSS.</param>
        /// <param name="otp">Codice OTP per l'autenticazione a due fattori.</param>
        /// <returns>True se la firma è avvenuta con successo, altrimenti false o viene generata un'eccezione.</returns>
        public static bool FirmaARSS(string pathFile, byte[] certificato, ref string lastError, string certId, string user, string password, string otp)
        {
            bool success = false;

            try
            {

                string sigFile = null;

                //  *******************************************************************
                //  Controlli preliminari
                //  *******************************************************************

                if (!File.Exists(pathFile))
                {
                    lastError = $"Il file '{pathFile}' non è stato trovato!";
                    return success;
                }

                sigFile = $"{pathFile}.{Enum.GetName(typeof(EstensioniFile), EstensioniFile.p7m)}";

                if (File.Exists(sigFile))
                {

                    lastError = $"Il file {sigFile} è già presente: eliminarlo prima di rifirmarlo!";
                    return success;
                }

                if (Utilities.glob.UnlockStatus == 0)
                {
                    lastError = "Licenza bloccata";
                    return success;
                }
                //  *******************************************************************
                // Carica il certificato usato per la firma. La chiave privata del certificato è memorizzata
                // sul server Aruba.it e la firma avverrà da remoto tramite
                // ARSS (Aruba Remote Signing Service).
                // Tuttavia, è comunque necessario avere il certificato localmente (senza chiave privata).
                Cert cert = new Cert();
                success = cert.LoadFromBinary(certificato);
                if (!success)
                {
                    lastError = cert.LastErrorText;
                    return success;
                }

                // Per firmare usando il servizio Aruba Remote Signing,
                // aggiungi le seguenti righe di codice per specificare le credenziali di autenticazione
                // e l'ID del certificato con chiave privata sul server da utilizzare.
                JsonObject jsonArss = new JsonObject();

                // Imposta "service" uguale a "ARSS" per indicare a Chilkat di usare ARSS per la firma.
                jsonArss.UpdateString("service", "ARSS");

                // Specifica l'ID del certificato, ad esempio "AS0..."
                // Questo certificato deve corrispondere a quello caricato nel codice sopra.
                jsonArss.UpdateString("certID", certId);
                jsonArss.UpdateString("otpPwd", otp);
                jsonArss.UpdateString("typeOtpAuth", "demoprod");
                jsonArss.UpdateString("user", user);
                jsonArss.UpdateString("userPWD", password);
                success = cert.SetCloudSigner(jsonArss);

                Crypt2 crypt = new Crypt2();
                success = crypt.SetSigningCert(cert);
                if (!success)
                {
                    lastError = crypt.LastErrorText;
                    return success;
                }

                // La proprietà CadesEnabled si applica a tutti i metodi che creano firme PKCS7.
                // Per creare una firma CAdES-BES, impostare questa proprietà a true.
                crypt.CadesEnabled = true;
                crypt.HashAlgorithm = "sha256";

                JsonObject signedAttrs = new JsonObject();
                signedAttrs.UpdateInt("contentType", 1);
                signedAttrs.UpdateInt("signingTime", 1);
                signedAttrs.UpdateInt("messageDigest", 1);
                signedAttrs.UpdateInt("signingCertificateV2", 1);
                crypt.SigningAttributes = signedAttrs.Emit();

                // Crea la firma CAdES-BES allegata, che contiene i dati originali.
                // Chilkat costruirà il .p7m localmente, ma utilizzerà (internamente) ARSS
                // per eseguire la firma RSA da remoto.
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
    }
}
