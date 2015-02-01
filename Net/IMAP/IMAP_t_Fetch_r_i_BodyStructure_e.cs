using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;
using LumiSoft.Net.MIME;
using LumiSoft.Net.IMAP;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is base class for IMAP server FETCH BODYSTRUCTURE MIME entities. Defined in RFC 3501 7.4.2.
    /// </summary>
    public abstract class IMAP_t_Fetch_r_i_BodyStructure_e
    {
        private IMAP_t_Fetch_r_i_BodyStructure_e_Multipart m_pParent = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected IMAP_t_Fetch_r_i_BodyStructure_e()
        {
        }


        #region method SetParent

        /// <summary>
        /// Sets this entity parent entity.
        /// </summary>
        /// <param name="parent">Parent entity.</param>
        internal void SetParent(IMAP_t_Fetch_r_i_BodyStructure_e_Multipart parent)
        {
            m_pParent = parent;
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets Content-Type header value. Value null means not specified.
        /// </summary>
        public abstract MIME_h_ContentType ContentType
        {
            get;
        }

        /// <summary>
        /// Gets Content-Disposition header value. Value null means not specified.
        /// </summary>
        public abstract MIME_h_ContentDisposition ContentDisposition
        {
            get;
        }

        /// <summary>
        /// Gets content language. Value null means not specified.
        /// </summary>
        public abstract string Language
        {
            get;
        }

        /// <summary>
        /// Gets content location. Value null means not specified.
        /// </summary>
        public abstract string Location
        {
            get;
        }

        
        /// <summary>
        /// Gets IMAP fetch body[part-specifier] value. This value can be used to fetch this entity from full message.
        /// </summary>
        public string PartSpecifier
        {
            get{
                string retVal = "";

                if(m_pParent == null){
                    retVal = "1";
                }
                else{
                    IMAP_t_Fetch_r_i_BodyStructure_e_Multipart currentParent = m_pParent;
                    while(currentParent != null){
                        int index = currentParent.IndexOfBodyPart(this) + 1;

                        retVal = "." + index.ToString() + retVal;

                        // Move <--- left upper parent.
                        currentParent = currentParent.m_pParent;
                    }

                    // multipart message always starts with "1." and childs after.
                    retVal = "1" + retVal;
                }

                return retVal;
            }
        }

        #endregion
    }
}
