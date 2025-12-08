using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data.Entity.Core.Objects.DataClasses;

namespace Avarez.Models
{
    public class Bank
    {
        public static Bank CreatetblBank(global::System.Int32 fldID, global::System.String fldName, global::System.Int32 fldBankTypeID, global::System.Byte fldCentralBankCode, global::System.String fldInfinitiveBank, global::System.Int64 fldUserID, global::System.DateTime fldDate)
        {
            Bank tblBank = new Bank();
            tblBank.fldID = fldID;
            tblBank.fldName = fldName;
            tblBank.fldBankTypeID = fldBankTypeID;
            tblBank.fldCentralBankCode = fldCentralBankCode;
            tblBank.fldInfinitiveBank = fldInfinitiveBank;
            tblBank.fldUserID = fldUserID;
            tblBank.fldDate = fldDate;
            return tblBank;
        }


        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [DataMemberAttribute()]
        public global::System.Int32 fldID
        {
            get
            {
                return _fldID;
            }
            set
            {
                if (_fldID != value)
                {

                    _fldID = value;
                   
                }
            }
        }
        private global::System.Int32 _fldID;

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
        [DataMemberAttribute()]
        public global::System.String fldName
        {
            get
            {
                return _fldName;
            }
            set
            {
               
                _fldName = value;
               
            }
        }
        private global::System.String _fldName;

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
        [DataMemberAttribute()]
        public global::System.Int32 fldBankTypeID
        {
            get
            {
                return _fldBankTypeID;
            }
            set
            {
                
                _fldBankTypeID = value;
               
            }
        }
        private global::System.Int32 _fldBankTypeID;

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
        [DataMemberAttribute()]
        public global::System.Byte fldCentralBankCode
        {
            get
            {
                return _fldCentralBankCode;
            }
            set
            {

                _fldCentralBankCode = value;

            }
        }
        private global::System.Byte _fldCentralBankCode;

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
        [DataMemberAttribute()]
        public global::System.String fldInfinitiveBank
        {
            get
            {
                return _fldInfinitiveBank;
            }
            set
            {
                _fldInfinitiveBank = value;
            }
        }
        private global::System.String _fldInfinitiveBank;

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
        [DataMemberAttribute()]
        public global::System.Int64 fldUserID
        {
            get
            {
                return _fldUserID;
            }
            set
            {
                _fldUserID = value;
            }
        }
        private global::System.Int64 _fldUserID;

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public global::System.String fldDesc
        {
            get
            {
                return _fldDesc;
            }
            set
            {
                _fldDesc = value;
            }
        }
        private global::System.String _fldDesc;

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
        [DataMemberAttribute()]
        public global::System.DateTime fldDate
        {
            get
            {
                return _fldDate;
            }
            set
            {
                _fldDate = value;
            }
        }
        private global::System.DateTime _fldDate;

        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
        [DataMemberAttribute()]
        public string fldImage
        {
            get
            {
                return _fldImage;
            }
            set
            {
                _fldImage = value;
            }
        }
        private string _fldImage;
    }
}