## Utilities per Fatturazione elettronica verso la Pubblica Amministrazione/privati e conservazione sostituitiva delle fatture

### Descrizione
La libreria è stata sviluppata in c# e contiene funzionalità di ausilio alla fatturazione elettronica e alla conservazione sostitutiva delle fatture elettroniche

Attualmente sono presenti i seguenti metodi:

- *UnlockLicense* metodo globale per sbloccare le funzionalità della libreria. Per poterla utilizzare in modalità demo per 30gg indicare come codice di sblocco qualsiasi parola
```csharp
   if (Utilities.UnlockLicense("mycodeUnlock", ref lastError))
   {
         // libreria sbloccata con successo
   }
```

- *Firma* permette di firmare una fattura tramite chiavetta/smartcard
```csharp
    if (Utilities.Firma("Marco Rossi", @"c:\temp\IT01234567890_FPA01.xml", ref lastError, "12345(pin opzionale)"))
    {
        //se il metodo ha successo verrà creato il file firmato   
        pathFile -> c:\temp\IT01234567890_FPA01.xml.p7m
    }
```

- *VerificaEstraiFirma* verifica una fattura firmata ed estrae il file xml
```csharp
          if (Utilities.VerificaEstraiFirma(@"c:\temp\IT01234567890_FPA01.xml.p7m", out pathFile, ref lastError))
          {
                // se la firma è valida verrà estratto il file xml nella stessa cartella con lo stesso nome
                pathFile -> c:\temp\IT01234567890_FPA01.xml
          }
```

- *MarcaTemporale* applica una marca temporale ad un file

```csharp
          if (Utilities.MarcaTemporale(@"c:\temp\IT01234567890_FPA01.xml.p7m", "https://freetsa.org/tsr", out pathFileTimeStamped, ref lastError, "myUser (optional)", "myPassword (optional)"))
          {
                //se il metodo ha successo verrà creato il file tsr nella stessa cartella con lo stesso nome
                pathFileTimeStamped -> c:\temp\IT01234567890_FPA01.xml.p7m.tsr
          }
```

- *CreaTsd* crea un file tsd (RFC 5544) da un file tsr ed una fattura firmata
```csharp
          if (Utilities.CreaTsd(@"c:\temp\IT01234567890_FPA01.xml.p7m.tsr", @"c:\temp\IT01234567890_FPA01.xml.p7m", out pathFileTsd, ref lastError)
          {
                //se il metodo ha successo verrà creato il file tsd
                pathFileTsd -> c:\temp\IT01234567890_FPA01.xml.p7m.tsd
          }
```

- *IsValidNomeFileFattura* verifica se il nome di un file fattura xml è formalmente corretto. Non richiede lo sblocco della licenza

```csharp
   if (Utilities.IsValidNomeFileFattura("IT01234567890_FPA01.xml.p7m", ref lastError))
   {
         nome file formalmente corretto
   }
   
   /// file formalmente non corretto
   if (Utilities.IsValidNomeFileFattura("IT04567890_FPA01.xml", ref lastError))
   {
         
   }
   
```
- *HashFile* genera l'hash del file

```csharp
   string hash = Utilities.HashFile(@"c:\temp\IT01234567890_FPA01.xml.p7m", ref lastError, "sha256"))
```

### Esempi

Esempio per generare un indice del pacchetto di versamento delle fatture elettroniche

```csharp
        // cartella con le fatture elettroniche firmate
        // in questo esempio verranno verificate ed estratte le fatture non firmate nella stessa cartella
        string pathProduttore = @"C:\Temp\Fatture201901";
        
        //dati del produttore
        string produttore = "IT01234567890";
        string produttoreDenominazione = "Società XXXX s.r.l.";
        string produttoreNome = null;
        string produttoreCognome = null;

        //dati del destinatario
        string destinatarioDenominazione = "Società YYYY s.r.l.";
        string destinatario = "IT11223344556";

        //algoritmo per l'hash dei file
        const string algorithm = "sha256";

        const string extensionFileSigned = "p7m";
        const string mimeType = "text/xml"; //RFC 3023

        //tipo di documento
        const string documentClass = "Fattura elettronica";

        //tipo di fatture (emesse/ricevute)
        PDVDocumentoMoreInfoDocumentTipology tipologiaFattura = PDVDocumentoMoreInfoDocumentTipology.Emessa;


        try
        {

            string lastError = null;
            if (!Utilities.UnlockLicense("my unlock", ref lastError))
            {
                throw new Exception(lastError);
            }

            
            DirectoryInfo d = new DirectoryInfo(pathProduttore);
            FileInfo[] files = d.GetFiles($"*.{extensionFileSigned}");

            List<PDVDocumento> pdvDocumenti = new List<PDVDocumento>();

            foreach (FileInfo s in files)
            {
                string outFile;
                if (!Utilities.VerificaEstraiFirma(s.FullName, out outFile, ref lastError))
                {
                    throw new Exception(lastError);
                }

                string hash = Utilities.HashFile(s.FullName, ref lastError, algorithm);
                if (string.IsNullOrWhiteSpace(hash))
                {
                    throw new Exception(lastError);
                }

                PDVDocumento pdvDocumento = null;

                FatturazioneElettronica.FatturaElettronica.CreateInvoice(outFile, out FatturaElettronicaType fatturaElettronicaType);


                CedentePrestatoreType cedentePrestatoreType = fatturaElettronicaType.FatturaElettronicaHeader.CedentePrestatore;
                CessionarioCommittenteType cessionarioCommittenteType = fatturaElettronicaType.FatturaElettronicaHeader.CessionarioCommittente;
                DatiGeneraliDocumentoType datiGeneraliDocumentoType = fatturaElettronicaType.FatturaElettronicaBody[0].DatiGenerali.DatiGeneraliDocumento;



                pdvDocumento = new PDVDocumento();
                pdvDocumento.DataChiusura = DateTime.Now;
                PDVDocumentoDestinatario pdvDocumentoDestinatario = new PDVDocumentoDestinatario();

                
                pdvDocumentoDestinatario.Codicefiscale = destinatario;
                pdvDocumentoDestinatario.Denominazione = destinatarioDenominazione;

                pdvDocumento.Destinatario = pdvDocumentoDestinatario;
                pdvDocumento.Hash = hash;
                pdvDocumento.HashType = algorithm;
                pdvDocumento.IdDocumento = Guid.NewGuid().ToString();
                pdvDocumento.NomeFile = s.Name;

                PDVDocumentoMoreInfoEmbedded pdvDocumentoMoreInfoEmbedded = new PDVDocumentoMoreInfoEmbedded();

                PDVDocumentoMoreInfoEmbeddedCedentePrestatore pdvDocumentoMoreInfoEmbeddedCedentePrestatore = new PDVDocumentoMoreInfoEmbeddedCedentePrestatore();
                pdvDocumentoMoreInfoEmbeddedCedentePrestatore.CodiceFiscale = cedentePrestatoreType.DatiAnagrafici.CodiceFiscale;

                int i = 0;
                foreach (ItemsChoiceType ict in cedentePrestatoreType.DatiAnagrafici.Anagrafica.ItemsElementName)
                {
                    if (ict == ItemsChoiceType.Denominazione)
                    {
                        pdvDocumentoMoreInfoEmbeddedCedentePrestatore.Denominazione = cedentePrestatoreType.DatiAnagrafici.Anagrafica.Items[i].ToString();
                        break;
                    }
                    else if (ict == ItemsChoiceType.Nome)
                    {
                        pdvDocumentoMoreInfoEmbeddedCedentePrestatore.Nome = cedentePrestatoreType.DatiAnagrafici.Anagrafica.Items[i].ToString();
                    }
                    else if (ict == ItemsChoiceType.Cognome)
                    {
                        pdvDocumentoMoreInfoEmbeddedCedentePrestatore.Cognome = cedentePrestatoreType.DatiAnagrafici.Anagrafica.Items[i].ToString();
                    }
                    else
                    {
                        throw new Exception($"Errore nel cedente prestatore: {nameof(ItemsChoiceType)} non trovato!");
                    }

                    i++;
                }


                pdvDocumentoMoreInfoEmbeddedCedentePrestatore.IdCodice = cedentePrestatoreType.DatiAnagrafici.IdFiscaleIVA.IdCodice;
                pdvDocumentoMoreInfoEmbeddedCedentePrestatore.IdPaese = cedentePrestatoreType.DatiAnagrafici.IdFiscaleIVA.IdPaese;


                PDVDocumentoMoreInfoEmbeddedCessionarioCommittente pdvDocumentoMoreInfoEmbeddedCessionarioCommittente = new PDVDocumentoMoreInfoEmbeddedCessionarioCommittente();


                i = 0;
                foreach (ItemsChoiceType ict in cessionarioCommittenteType.DatiAnagrafici.Anagrafica.ItemsElementName)
                {
                    if (ict == ItemsChoiceType.Denominazione)
                    {
                        pdvDocumentoMoreInfoEmbeddedCessionarioCommittente.Denominazione = cessionarioCommittenteType.DatiAnagrafici.Anagrafica.Items[i].ToString();
                        break;
                    }
                    else if (ict == ItemsChoiceType.Nome)
                    {
                        pdvDocumentoMoreInfoEmbeddedCessionarioCommittente.Nome = cessionarioCommittenteType.DatiAnagrafici.Anagrafica.Items[i].ToString();
                    }
                    else if (ict == ItemsChoiceType.Cognome)
                    {
                        pdvDocumentoMoreInfoEmbeddedCessionarioCommittente.Cognome = cessionarioCommittenteType.DatiAnagrafici.Anagrafica.Items[i].ToString();
                    }
                    else
                    {
                        throw new Exception($"Errore nel cedente prestatore: {nameof(ItemsChoiceType)} non trovato!");
                    }

                    i++;
                }


                if (cessionarioCommittenteType.DatiAnagrafici.IdFiscaleIVA != null)
                {
                    pdvDocumentoMoreInfoEmbeddedCessionarioCommittente.IdCodice = cessionarioCommittenteType.DatiAnagrafici.IdFiscaleIVA.IdCodice;
                    pdvDocumentoMoreInfoEmbeddedCessionarioCommittente.IdPaese = cessionarioCommittenteType.DatiAnagrafici.IdFiscaleIVA.IdPaese;
                }
                else if ((cessionarioCommittenteType.DatiAnagrafici.IdFiscaleIVA == null) && (string.IsNullOrWhiteSpace(cessionarioCommittenteType.DatiAnagrafici.CodiceFiscale)))
                {
                    throw new Exception("Dati fiscali cessionario committente non trovati");
                }

                pdvDocumentoMoreInfoEmbeddedCessionarioCommittente.CodiceFiscale = cessionarioCommittenteType.DatiAnagrafici.CodiceFiscale;

                PDVDocumentoMoreInfoEmbeddedDatiGeneraliDocumento pdvDocumentoMoreInfoEmbeddedDatiGeneraliDocumento = new PDVDocumentoMoreInfoEmbeddedDatiGeneraliDocumento();
                pdvDocumentoMoreInfoEmbeddedDatiGeneraliDocumento.Numero = datiGeneraliDocumentoType.Numero;
                pdvDocumentoMoreInfoEmbeddedDatiGeneraliDocumento.Data = datiGeneraliDocumentoType.Data;

                PDVDocumentoMoreInfoEmbeddedIdentificativo pdvDocumentoMoreInfoEmbeddedIdentificativo = new PDVDocumentoMoreInfoEmbeddedIdentificativo();
                pdvDocumentoMoreInfoEmbeddedIdentificativo.CodiceDestinatario = fatturaElettronicaType.FatturaElettronicaHeader.DatiTrasmissione.CodiceDestinatario;
                pdvDocumentoMoreInfoEmbeddedIdentificativo.ProgressivoInvio = fatturaElettronicaType.FatturaElettronicaHeader.DatiTrasmissione.ProgressivoInvio;

                pdvDocumentoMoreInfoEmbedded.CedentePrestatore = pdvDocumentoMoreInfoEmbeddedCedentePrestatore;
                pdvDocumentoMoreInfoEmbedded.CessionarioCommittente = pdvDocumentoMoreInfoEmbeddedCessionarioCommittente;
                pdvDocumentoMoreInfoEmbedded.DatiGeneraliDocumento = pdvDocumentoMoreInfoEmbeddedDatiGeneraliDocumento;
                pdvDocumentoMoreInfoEmbedded.Identificativo = pdvDocumentoMoreInfoEmbeddedIdentificativo;


                PDVDocumentoMoreInfo pdvDocumentoMoreInfo = new PDVDocumentoMoreInfo();
                pdvDocumentoMoreInfo.DocumentClass = documentClass;
                pdvDocumentoMoreInfo.Embedded = pdvDocumentoMoreInfoEmbedded;
                pdvDocumentoMoreInfo.MimeType = mimeType;


                pdvDocumentoMoreInfo.DocumentType = (PDVDocumentoMoreInfoDocumentType)Enum.Parse(typeof(PDVDocumentoMoreInfoDocumentType), Enum.GetName(typeof(FormatoTrasmissioneType), fatturaElettronicaType.FatturaElettronicaHeader.DatiTrasmissione.FormatoTrasmissione));
                pdvDocumentoMoreInfo.DocumentTipology = tipologiaFattura;


                pdvDocumento.MoreInfo = pdvDocumentoMoreInfo;

                PDVDocumentoSoggettoProduttore pdvDocumentoSoggettoProduttore = new PDVDocumentoSoggettoProduttore();
                pdvDocumentoSoggettoProduttore.CodiceFiscale = produttore;


                if ((string.IsNullOrWhiteSpace(produttoreDenominazione)) && (!((string.IsNullOrWhiteSpace(produttoreNome)) && (string.IsNullOrWhiteSpace(produttoreCognome)))))
                {
                    pdvDocumentoSoggettoProduttore.Nome = produttoreNome;
                    pdvDocumentoSoggettoProduttore.Cognome = produttoreCognome;
                }
                else if (((string.IsNullOrWhiteSpace(produttoreNome)) && (string.IsNullOrWhiteSpace(produttoreCognome))) && (!string.IsNullOrWhiteSpace(produttoreDenominazione)))
                {
                    pdvDocumentoSoggettoProduttore.Denominazione = produttoreDenominazione;
                }
                else
                {
                    throw new Exception("Errore nella configurazione dei produttori!");
                }

                pdvDocumento.OggettoDocumento = fatturaElettronicaType.FatturaElettronicaBody[0].DatiBeniServizi.DettaglioLinee[0].Descrizione;
                pdvDocumento.SoggettoProduttore = pdvDocumentoSoggettoProduttore;
                pdvDocumento.StoreTime = DateTime.UtcNow;

                pdvDocumenti.Add(pdvDocumento);
            }

            if (pdvDocumenti.Count == 0)
            {
                Console.WriteLine("Non sono stati trovati documenti!");
            }
            else
            {

                PDV pdv = new PDV();
                string id = Guid.NewGuid().ToString();
                pdv.Id = id;
                pdv.Documento = pdvDocumenti.ToArray();

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PDV));

                string indicePacchettoDiVersamento = Path.Combine(d.FullName, $"{id}.xml");

                using (TextWriter textWriter = new StreamWriter(indicePacchettoDiVersamento))
                {
                    xmlSerializer.Serialize(textWriter, pdv);
                }

                Console.WriteLine("Operazione eseguita con successo!");

                //firma indicePacchettoDiVersamento da smartcard/usb
                //Utilities.Firma("mySubjectCM", indicePacchettoDiVersamento, ref lastError, "00000");

                //marca indicePacchettoDiVersamento
                //Utilities.MarcaTemporale(indicePacchettoDiVersamento + $".{extensionFileSigned}", "https://myurltsr", out string indicePacchettoDiVersamentoSignedMarked, ref lastError, "myuser","mypwd");

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore: {ex.Message}!");
        }

        Console.ReadKey();
```

### Requisiti

- E' richiesto il framework Microsoft .NET 4.6.2
- Licenza d'uso della libreria Chilkat (per dettagli visitare il sito https://www.chilkatsoft.com) 

### Installazione
```
	PM> Install-Package StudioAT.FatturazioneElettronica.Extensions -Version 1.0.3
```
dalla Console di Gestione Pacchetti di Visual Studio

##### Nel progetto Visual Studio impostare la piattaforma di destinazione a x64.

