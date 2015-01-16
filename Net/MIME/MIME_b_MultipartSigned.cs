using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
    /// <summary>
    /// This class represents MIME multipart/signed body. Defined in RFC 5751.
    /// </summary>
    public class MIME_b_MultipartSigned : MIME_b_Multipart
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="contentType">Content type.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>contentType</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public MIME_b_MultipartSigned(MIME_h_ContentType contentType) : base(contentType)
        {
            if(!string.Equals(contentType.TypeWithSubtype,"multipart/signed",StringComparison.CurrentCultureIgnoreCase)){
                throw new ArgumentException("Argument 'contentType.TypeWithSubype' value must be 'multipart/signed'.");
            }
        }

        #region static method Parse

        /// <summary>
        /// Parses body from the specified stream
        /// </summary>
        /// <param name="owner">Owner MIME entity.</param>
        /// <param name="defaultContentType">Default content-type for this body.</param>
        /// <param name="stream">Stream from where to read body.</param>
        /// <returns>Returns parsed body.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b>, <b>mediaTypedefaultContentTypeb></b> or <b>stream</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when any parsing errors.</exception>
        protected static new MIME_b Parse(MIME_Entity owner,MIME_h_ContentType defaultContentType,SmartStream stream)
        {
            if(owner == null){
                throw new ArgumentNullException("owner");
            }
            if(defaultContentType == null){
                throw new ArgumentNullException("defaultContentType");
            }
            if(stream == null){
                throw new ArgumentNullException("stream");
            }
            if(owner.ContentType == null || owner.ContentType.Param_Boundary == null){
                throw new ParseException("Multipart entity has not required 'boundary' paramter.");
            }
            
            MIME_b_MultipartSigned retVal = new MIME_b_MultipartSigned(owner.ContentType);
            ParseInternal(owner,owner.ContentType.TypeWithSubtype,stream,retVal);

            return retVal;
        }

        #endregion


        #region method GetCertificates

        /// <summary>
        /// Gets certificates contained in pkcs 7.
        /// </summary>
        /// <returns>Returns certificates contained in pkcs 7. Returns null if no certificates.</returns>
        public X509Certificate2Collection GetCertificates()
        {
            // multipart/signed must always have only 2 entities, otherwise invalid data.
            if(this.BodyParts.Count != 2){
                return null;
            }
                
            // Get signature. It should be 2 entity.
            MIME_Entity signatureEntity = this.BodyParts[1];

            SignedCms signedCms = new SignedCms();
            signedCms.Decode(((MIME_b_SinglepartBase)signatureEntity.Body).Data);

            return signedCms.Certificates;
        }

        #endregion

        #region method VerifySignature

        /// <summary>
        /// Checks if signature is valid and data not altered.
        /// </summary>
        /// <returns>Returns true if signature is valid, otherwise false.</returns>
        public bool VerifySignature()
        {
            // multipart/signed must always have only 2 entities, otherwise invalid data.
            if(this.BodyParts.Count != 2){
                return false;
            }
                
            // Get signature. It should be 2 entity.
            MIME_Entity signatureEntity = this.BodyParts[1];
                                                 
            System.IO.MemoryStream tmpStream = new System.IO.MemoryStream();
            this.BodyParts[0].ToStream(tmpStream,null,null,false);

            try{
                SignedCms signedCms = new SignedCms(new ContentInfo(tmpStream.ToArray()),true);
                signedCms.Decode(((MIME_b_SinglepartBase)signatureEntity.Body).Data);
                signedCms.CheckSignature(true);

                return true;
            }
            catch{
                return false;
            }
        }

        #endregion


        #region Properties implementation

        #endregion
    }
}
