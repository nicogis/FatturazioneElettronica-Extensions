﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// Codice sorgente generato automaticamente da xsd, versione=4.6.1055.0.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class PDV {
    
    private PDVDocumento[] documentoField;
    
    private string idField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Documento", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PDVDocumento[] Documento {
        get {
            return this.documentoField;
        }
        set {
            this.documentoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class PDVDocumento {
    
    private System.DateTime dataChiusuraField;
    
    private string oggettoDocumentoField;
    
    private string nomeFileField;
    
    private PDVDocumentoSoggettoProduttore soggettoProduttoreField;
    
    private PDVDocumentoDestinatario destinatarioField;
    
    private PDVDocumentoMoreInfo moreInfoField;
    
    private string hashField;
    
    private string hashTypeField;
    
    private System.DateTime storeTimeField;
    
    private string idDocumentoField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="date")]
    public System.DateTime DataChiusura {
        get {
            return this.dataChiusuraField;
        }
        set {
            this.dataChiusuraField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string OggettoDocumento {
        get {
            return this.oggettoDocumentoField;
        }
        set {
            this.oggettoDocumentoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string NomeFile {
        get {
            return this.nomeFileField;
        }
        set {
            this.nomeFileField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PDVDocumentoSoggettoProduttore SoggettoProduttore {
        get {
            return this.soggettoProduttoreField;
        }
        set {
            this.soggettoProduttoreField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PDVDocumentoDestinatario Destinatario {
        get {
            return this.destinatarioField;
        }
        set {
            this.destinatarioField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PDVDocumentoMoreInfo MoreInfo {
        get {
            return this.moreInfoField;
        }
        set {
            this.moreInfoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Hash {
        get {
            return this.hashField;
        }
        set {
            this.hashField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string HashType {
        get {
            return this.hashTypeField;
        }
        set {
            this.hashTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="date")]
    public System.DateTime StoreTime {
        get {
            return this.storeTimeField;
        }
        set {
            this.storeTimeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string IdDocumento {
        get {
            return this.idDocumentoField;
        }
        set {
            this.idDocumentoField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class PDVDocumentoSoggettoProduttore {
    
    private string nomeField;
    
    private string cognomeField;
    
    private string denominazioneField;
    
    private string codiceFiscaleField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Nome {
        get {
            return this.nomeField;
        }
        set {
            this.nomeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Cognome {
        get {
            return this.cognomeField;
        }
        set {
            this.cognomeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Denominazione {
        get {
            return this.denominazioneField;
        }
        set {
            this.denominazioneField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CodiceFiscale {
        get {
            return this.codiceFiscaleField;
        }
        set {
            this.codiceFiscaleField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class PDVDocumentoDestinatario {
    
    private string denominazioneField;
    
    private string codicefiscaleField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Denominazione {
        get {
            return this.denominazioneField;
        }
        set {
            this.denominazioneField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Codicefiscale {
        get {
            return this.codicefiscaleField;
        }
        set {
            this.codicefiscaleField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class PDVDocumentoMoreInfo {
    
    private string mimeTypeField;
    
    private string documentClassField;
    
    private PDVDocumentoMoreInfoDocumentType documentTypeField;
    
    private PDVDocumentoMoreInfoDocumentTipology documentTipologyField;
    
    private PDVDocumentoMoreInfoEmbedded embeddedField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string MimeType {
        get {
            return this.mimeTypeField;
        }
        set {
            this.mimeTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string DocumentClass {
        get {
            return this.documentClassField;
        }
        set {
            this.documentClassField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PDVDocumentoMoreInfoDocumentType DocumentType {
        get {
            return this.documentTypeField;
        }
        set {
            this.documentTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PDVDocumentoMoreInfoDocumentTipology DocumentTipology {
        get {
            return this.documentTipologyField;
        }
        set {
            this.documentTipologyField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PDVDocumentoMoreInfoEmbedded Embedded {
        get {
            return this.embeddedField;
        }
        set {
            this.embeddedField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public enum PDVDocumentoMoreInfoDocumentType {
    
    /// <remarks/>
    SDI11,
    
    /// <remarks/>
    FPA12,

    /// <remarks/>
    FPR12,

    /// <remarks/>
    B2B,
    
    /// <remarks/>
    B2C,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public enum PDVDocumentoMoreInfoDocumentTipology {
    
    /// <remarks/>
    Emessa,
    
    /// <remarks/>
    Ricevuta,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class PDVDocumentoMoreInfoEmbedded {
    
    private PDVDocumentoMoreInfoEmbeddedCedentePrestatore cedentePrestatoreField;
    
    private PDVDocumentoMoreInfoEmbeddedDatiGeneraliDocumento datiGeneraliDocumentoField;
    
    private PDVDocumentoMoreInfoEmbeddedCessionarioCommittente cessionarioCommittenteField;
    
    private PDVDocumentoMoreInfoEmbeddedIdentificativo identificativoField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PDVDocumentoMoreInfoEmbeddedCedentePrestatore CedentePrestatore {
        get {
            return this.cedentePrestatoreField;
        }
        set {
            this.cedentePrestatoreField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PDVDocumentoMoreInfoEmbeddedDatiGeneraliDocumento DatiGeneraliDocumento {
        get {
            return this.datiGeneraliDocumentoField;
        }
        set {
            this.datiGeneraliDocumentoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PDVDocumentoMoreInfoEmbeddedCessionarioCommittente CessionarioCommittente {
        get {
            return this.cessionarioCommittenteField;
        }
        set {
            this.cessionarioCommittenteField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public PDVDocumentoMoreInfoEmbeddedIdentificativo Identificativo {
        get {
            return this.identificativoField;
        }
        set {
            this.identificativoField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class PDVDocumentoMoreInfoEmbeddedCedentePrestatore {
    
    private string idPaeseField;
    
    private string idCodiceField;
    
    private string denominazioneField;
    
    private string nomeField;
    
    private string cognomeField;
    
    private string codiceFiscaleField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IdPaese {
        get {
            return this.idPaeseField;
        }
        set {
            this.idPaeseField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IdCodice {
        get {
            return this.idCodiceField;
        }
        set {
            this.idCodiceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Denominazione {
        get {
            return this.denominazioneField;
        }
        set {
            this.denominazioneField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Nome {
        get {
            return this.nomeField;
        }
        set {
            this.nomeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Cognome {
        get {
            return this.cognomeField;
        }
        set {
            this.cognomeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CodiceFiscale {
        get {
            return this.codiceFiscaleField;
        }
        set {
            this.codiceFiscaleField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class PDVDocumentoMoreInfoEmbeddedDatiGeneraliDocumento {
    
    private System.DateTime dataField;
    
    private string numeroField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="date")]
    public System.DateTime Data {
        get {
            return this.dataField;
        }
        set {
            this.dataField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Numero {
        get {
            return this.numeroField;
        }
        set {
            this.numeroField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class PDVDocumentoMoreInfoEmbeddedCessionarioCommittente {
    
    private string idPaeseField;
    
    private string idCodiceField;
    
    private string denominazioneField;
    
    private string nomeField;
    
    private string cognomeField;
    
    private string codiceFiscaleField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IdPaese {
        get {
            return this.idPaeseField;
        }
        set {
            this.idPaeseField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IdCodice {
        get {
            return this.idCodiceField;
        }
        set {
            this.idCodiceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Denominazione {
        get {
            return this.denominazioneField;
        }
        set {
            this.denominazioneField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Nome {
        get {
            return this.nomeField;
        }
        set {
            this.nomeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Cognome {
        get {
            return this.cognomeField;
        }
        set {
            this.cognomeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CodiceFiscale {
        get {
            return this.codiceFiscaleField;
        }
        set {
            this.codiceFiscaleField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class PDVDocumentoMoreInfoEmbeddedIdentificativo {
    
    private string progressivoInvioField;
    
    private string codiceDestinatarioField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ProgressivoInvio {
        get {
            return this.progressivoInvioField;
        }
        set {
            this.progressivoInvioField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CodiceDestinatario {
        get {
            return this.codiceDestinatarioField;
        }
        set {
            this.codiceDestinatarioField = value;
        }
    }
}
