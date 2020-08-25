## Utilities per Fatturazione elettronica verso la Pubblica Amministrazione/privati e conservazione sostitutiva delle fatture

### Descrizione
La libreria è stata sviluppata in c# e contiene funzionalità di ausilio alla fatturazione elettronica e alla conservazione sostitutiva delle fatture elettroniche

Attualmente sono presenti i seguenti metodi:

- *UnlockLicense* metodo globale per sbloccare le funzionalità della libreria Chilkat. Per poterla utilizzare in modalità demo per 30gg indicare come codice di sblocco qualsiasi parola

```csharp
   if (Utilities.UnlockLicense("mycodeUnlockChilkat", ref lastError))
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
                // la firma è valida e verrà estratto il file xml nella stessa cartella con lo stesso nome
                pathFile -> c:\temp\IT01234567890_FPA01.xml
          }

          string pathFile = "c:\file\test.xml";
          if (Utilities.VerificaEstraiFirma(@"c:\temp\IT01234567890_FPA01.xml.p7m", pathFile, ref lastError))
          {
                // la firma è valida e verrà estratto il file xml
                pathFile -> c:\file\test.xml
          }
```


- *VerificaFirma* verifica una fattura firmata

```csharp

          if (!Utilities.VerificaFirma(@"c:\temp\IT01234567890_FPA01.xml.p7m", ref lastError))
          {
                // la firma non è valida
          }

```


- *MarcaTemporale* applica una marca temporale ad un file

```csharp
          // con Aruba, ad esempio, il TSA è https://servizi.arubapec.it/tsa/ngrequest.php
	  
          if (Utilities.MarcaTemporale(@"c:\temp\IT01234567890_FPA01.xml.p7m", "https://freetsa.org/tsr", out pathFileTimeStamped, ref lastError, "myUser (optional)", "myPassword (optional)"))
          {
                // se il metodo ha successo verrà creato il file tsr nella stessa cartella con lo stesso nome
                pathFileTimeStamped -> c:\temp\IT01234567890_FPA01.xml.p7m.tsr
          }
```

- *CreaTsd* crea un file tsd (RFC 5544) da un file tsr ed una fattura firmata
```csharp
          if (Utilities.CreaTsd(@"c:\temp\IT01234567890_FPA01.xml.p7m.tsr", @"c:\temp\IT01234567890_FPA01.xml.p7m", out pathFileTsd, ref lastError)
          {
                // se il metodo ha successo verrà creato il file tsd
                pathFileTsd -> c:\temp\IT01234567890_FPA01.xml.p7m.tsd
          }
```

- *IsValidNomeFileFattura* verifica se il nome di un file fattura xml è formalmente corretto. Non richiede lo sblocco della licenza
```csharp
          if (Utilities.IsValidNomeFileFattura("IT01234567890_FPA01.xml.p7m", ref lastError))
          {
                nome file formalmente corretto
          }
   
          // file formalmente non corretto
          if (Utilities.IsValidNomeFileFattura("IT04567890_FPA01.xml", ref lastError))
          {
         
          }
```

- *HashFile* genera hash del file
```csharp

   string hash = Utilities.HashFile(@"c:\temp\IT01234567890_FPA01.xml.p7m", ref lastError, "sha256")

   // hash sha256 e encode hex
   string hash = Utilities.HashFile(@"c:\temp\IT01234567890_FPA01.xml.p7m", ref lastError, encode: "hex")
```


### Esempi

Esempio per generare un indice del pacchetto di versamento delle fatture elettroniche. L'esempio necessita anche della libreria https://github.com/nicogis/FatturazioneElettronica

```csharp
        using FatturazioneElettronica.Type.V_1_2_1;
        using FatturazioneElettronica.Extensions;

        //cartella (pathProduttore) con le fatture elettroniche firmate e/o non firmate
        //i file xml delle fatture firmate verranno estratti nella stessa cartella (pathProduttore)
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
        const string extensionFileUnSigned = "xml";

        const string mimeType = "text/xml"; //RFC 3023

        //tipo di documento
        const string documentClass = "Fattura elettronica";

        //tipo di fatture (emesse/ricevute)
	//in questo esempio sono emesse
        PDVDocumentoMoreInfoDocumentTipology tipologiaFattura = PDVDocumentoMoreInfoDocumentTipology.Emessa;

        try
        {

            string lastError = null;
            if (!Utilities.UnlockLicense("my unlock", ref lastError))
            {
                throw new Exception(lastError);
            }

            
            DirectoryInfo d = new DirectoryInfo(pathProduttore);
            List<FileInfo> files = d.GetFiles($"*.{extensionFileSigned}").ToList();
            files.AddRange(d.GetFiles($"*.{extensionFileUnSigned}"));

            List<PDVDocumento> pdvDocumenti = new List<PDVDocumento>();

            string outFile;
            foreach (FileInfo s in files)
            {                
                //verifico ed estraggo le firmate
                if (s.Extension == $".{extensionFileSigned}")
                {

                    if (!Utilities.VerificaEstraiFirma(s.FullName, out string outF, ref lastError))
                    {
                        throw new Exception(lastError);
                    }

                    outFile = outF;
                }
                else if (s.Extension == $".{extensionFileUnSigned}")
                {
                    outFile = s.FullName;
                }
                else
                {
                    throw new Exception($"Estensione non trovata ({extensionFileSigned}, {extensionFileUnSigned}) per il file {s.FullName}!");
                }



                string hash = Utilities.HashFile(s.FullName, ref lastError, algorithm);
                if (string.IsNullOrWhiteSpace(hash))
                {
                    throw new Exception(lastError);
                }

                PDVDocumento pdvDocumento = null;

                FatturaElettronica.CreateInvoice(outFile, out IFatturaElettronicaType f);
                FatturaElettronicaType fatturaElettronicaType = f as FatturaElettronicaType;

                CedentePrestatoreType cedentePrestatoreType = fatturaElettronicaType.FatturaElettronicaHeader.CedentePrestatore;
                CessionarioCommittenteType cessionarioCommittenteType = fatturaElettronicaType.FatturaElettronicaHeader.CessionarioCommittente;
                DatiGeneraliDocumentoType datiGeneraliDocumentoType = fatturaElettronicaType.FatturaElettronicaBody[0].DatiGenerali.DatiGeneraliDocumento;



                pdvDocumento = new PDVDocumento
                {
                    DataChiusura = DateTime.Now
                };

                PDVDocumentoDestinatario pdvDocumentoDestinatario = new PDVDocumentoDestinatario
                {
                    Codicefiscale = destinatario,
                    Denominazione = destinatarioDenominazione
                };

                pdvDocumento.Destinatario = pdvDocumentoDestinatario;
                pdvDocumento.Hash = hash;
                pdvDocumento.HashType = algorithm;
                pdvDocumento.IdDocumento = Guid.NewGuid().ToString();
                pdvDocumento.NomeFile = s.Name;

                PDVDocumentoMoreInfoEmbedded pdvDocumentoMoreInfoEmbedded = new PDVDocumentoMoreInfoEmbedded();

                PDVDocumentoMoreInfoEmbeddedCedentePrestatore pdvDocumentoMoreInfoEmbeddedCedentePrestatore = new PDVDocumentoMoreInfoEmbeddedCedentePrestatore
                {
                    CodiceFiscale = cedentePrestatoreType.DatiAnagrafici.CodiceFiscale
                };

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

                PDVDocumentoMoreInfoEmbeddedDatiGeneraliDocumento pdvDocumentoMoreInfoEmbeddedDatiGeneraliDocumento = new PDVDocumentoMoreInfoEmbeddedDatiGeneraliDocumento
                {
                    Numero = datiGeneraliDocumentoType.Numero,
                    Data = datiGeneraliDocumentoType.Data
                };

                PDVDocumentoMoreInfoEmbeddedIdentificativo pdvDocumentoMoreInfoEmbeddedIdentificativo = new PDVDocumentoMoreInfoEmbeddedIdentificativo
                {
                    CodiceDestinatario = fatturaElettronicaType.FatturaElettronicaHeader.DatiTrasmissione.CodiceDestinatario,
                    ProgressivoInvio = fatturaElettronicaType.FatturaElettronicaHeader.DatiTrasmissione.ProgressivoInvio
                };

                pdvDocumentoMoreInfoEmbedded.CedentePrestatore = pdvDocumentoMoreInfoEmbeddedCedentePrestatore;
                pdvDocumentoMoreInfoEmbedded.CessionarioCommittente = pdvDocumentoMoreInfoEmbeddedCessionarioCommittente;
                pdvDocumentoMoreInfoEmbedded.DatiGeneraliDocumento = pdvDocumentoMoreInfoEmbeddedDatiGeneraliDocumento;
                pdvDocumentoMoreInfoEmbedded.Identificativo = pdvDocumentoMoreInfoEmbeddedIdentificativo;


                PDVDocumentoMoreInfo pdvDocumentoMoreInfo = new PDVDocumentoMoreInfo
                {
                    DocumentClass = documentClass,
                    Embedded = pdvDocumentoMoreInfoEmbedded,
                    MimeType = mimeType,


                    DocumentType = (PDVDocumentoMoreInfoDocumentType)Enum.Parse(typeof(PDVDocumentoMoreInfoDocumentType), Enum.GetName(typeof(FormatoTrasmissioneType), fatturaElettronicaType.FatturaElettronicaHeader.DatiTrasmissione.FormatoTrasmissione)),
                    DocumentTipology = tipologiaFattura
                };


                pdvDocumento.MoreInfo = pdvDocumentoMoreInfo;

                PDVDocumentoSoggettoProduttore pdvDocumentoSoggettoProduttore = new PDVDocumentoSoggettoProduttore
                {
                    CodiceFiscale = produttore
                };


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

                // firma indicePacchettoDiVersamento da smartcard/usb
                //Utilities.Firma("mySubjectCM", indicePacchettoDiVersamento, ref lastError, "00000");

                // marca indicePacchettoDiVersamento
                //Utilities.MarcaTemporale(indicePacchettoDiVersamento + $".{extensionFileSigned}", "https://myurltsr", out string indicePacchettoDiVersamentoSignedMarked, ref lastError, "myuser","mypwd");

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore: {ex.Message}!");
        }

        Console.ReadKey();
```

Esempio controllo hash metadati - file fattura
```csharp
    
    string metadato = @"c:\temp\IT01234567890_FPA01_MT_001.xml";
    using (XmlReader reader = XmlReader.Create(metadato)) 
    {

        if (xmlSerializer.CanDeserialize(reader))
        {
            FileMetadati_Type fileMetadati = (FileMetadati_Type)xmlSerializer.Deserialize(reader);

            string fileCorrente = Path.Combine("c:\temp", fileMetadati.NomeFile);
            if (fileMetadati.Hash.ToLowerInvariant() != Utilities.HashFile(fileCorrente,ref lastError, encode:"hex").ToLowerInvariant())
            {
                throw new Exception($"Hash dichiarato nel metadato differente da quello calcolato sul file:{fileCorrente}");
            }
        }
        else
        {

            throw new Exception($"Errore a decodificare il metadato:{metadato}");

        }

    }


    Classe generata da xsd MessaggiFatturaTypes_v10.xsd    

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fattura/messaggi/v1.0")]
    [System.Xml.Serialization.XmlRootAttribute("FileMetadati", Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fattura/messaggi/v1.0", IsNullable = false)]
    public partial class FileMetadati_Type
    {

        private string identificativoSdIField;

        private string nomeFileField;

        private string hashField;

        private string codiceDestinatarioField;

        private string formatoField;

        private string tentativiInvioField;

        private string messageIdField;

        private string noteField;

        private string versioneField;

        public FileMetadati_Type()
        {
            this.versioneField = "1.0";
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IdentificativoSdI
        {
            get
            {
                return this.identificativoSdIField;
            }
            set
            {
                this.identificativoSdIField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NomeFile
        {
            get
            {
                return this.nomeFileField;
            }
            set
            {
                this.nomeFileField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Hash
        {
            get
            {
                return this.hashField;
            }
            set
            {
                this.hashField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CodiceDestinatario
        {
            get
            {
                return this.codiceDestinatarioField;
            }
            set
            {
                this.codiceDestinatarioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Formato
        {
            get
            {
                return this.formatoField;
            }
            set
            {
                this.formatoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        public string TentativiInvio
        {
            get
            {
                return this.tentativiInvioField;
            }
            set
            {
                this.tentativiInvioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MessageId
        {
            get
            {
                return this.messageIdField;
            }
            set
            {
                this.messageIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Note
        {
            get
            {
                return this.noteField;
            }
            set
            {
                this.noteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string versione
        {
            get
            {
                return this.versioneField;
            }
            set
            {
                this.versioneField = value;
            }
        }
    }
```

### Requisiti

- E' richiesto il framework Microsoft .NET 4.6.2
- Licenza d'uso della libreria Chilkat 9.5.0.82 (per dettagli visitare il sito https://www.chilkatsoft.com) 

### Installazione
```
	PM> Install-Package StudioAT.FatturazioneElettronica.Extensions -Version 1.0.4
```
dalla Console di Gestione Pacchetti di Visual Studio

##### Nel progetto Visual Studio impostare la piattaforma di destinazione a x64.

