//-----------------------------------------------------------------------
// <copyright file="FormatiFirma.cs" company="Studio A&T s.r.l.">
//     Copyright (c) Studio A&T s.r.l. All rights reserved.
// </copyright>
// <author>Nicogis</author>
//-----------------------------------------------------------------------
namespace FatturazioneElettronica.Extensions
{
    /// <summary>
    /// Formati di firma digitale
    /// </summary>
    public enum FormatiFirma
    {
        /// <summary>
        /// Cryptographic Message Syntax Advanced Electronic Signature
        /// </summary>
        CAdES,
        /// <summary>
        /// XML Advanced Electronic Signature
        /// </summary>
        XAdES
    }
}
