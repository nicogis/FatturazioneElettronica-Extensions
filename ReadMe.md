## Utilities per Fatturazione elettronica verso la Pubblica Amministrazione/privati e conservazione sostituitiva delle fatture

### Descrizione
La libreria è stata sviluppata in c# e contiene funzionalità di ausilio alla fatturazione elettronica e alla conservazione sostitutiva delle stesse

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

- *VerificaEstraiFirma* verifica una fattura firmata ed estrarre il file xml
```csharp
          if (Utilities.VerificaEstraiFirma(@"c:\temp\IT01234567890_FPA01.xml.p7m", out pathFile, ref lastError))
          {
                // se la firma è valida verrà estratto il file xml
                pathFile -> c:\temp\IT01234567890_FPA01.xml
          }
```

- *MarcaTemporale* applica una marca temporale ad un file

```csharp
          if (Utilities.MarcaTemporale(@"c:\temp\IT01234567890_FPA01.xml.p7m", "https://freetsa.org/tsr", out pathFileTimeStamped, ref lastError, "myUser (optional)", "myPassword (optional)"))
          {
                //se il metodo ha successo verrà creato il file tsr
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
   if (Utilities.IsValidNomeFileFattura("IT04567890_FPA01.xml.p7m", ref lastError))
   {
         
   }
   
```

### Requisiti

- E' richiesto il framework Microsoft .NET 4.6.2
- Licenza d'uso della libreria Chilkat (per dettagli visitare il sito https://www.chilkatsoft.com) 

### Installazione
```
	PM> Install-Package StudioAT.FatturazioneElettronica.Extensions -Version 1.0.0
```
dalla Console di Gestione Pacchetti di Visual Studio

