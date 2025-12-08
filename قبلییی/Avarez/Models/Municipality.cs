using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data.Entity.Core.Objects.DataClasses;

namespace Avarez.Models
{
    public class Municipality
    {
        public static tblMunicipality CreatetblMunicipality(global::System.Int32 fldID, global::System.String fldName, global::System.Int32 fldCityID, global::System.String fldInformaticesCode, global::System.Byte fldServiceCode, global::System.String fldRWUserName, global::System.String fldRWPass, global::System.Int64 fldUserID, global::System.DateTime fldDate)
        {
            tblMunicipality tblMunicipality = new tblMunicipality();
            tblMunicipality.fldID = fldID;
            tblMunicipality.fldName = fldName;
            tblMunicipality.fldCityID = fldCityID;
            tblMunicipality.fldInformaticesCode = fldInformaticesCode;
            tblMunicipality.fldServiceCode = fldServiceCode;
            tblMunicipality.fldUserID = fldUserID;
            tblMunicipality.fldDate = fldDate;
            tblMunicipality.fldRWUserName = fldRWUserName;
            tblMunicipality.fldRWPass = fldRWPass;
            return tblMunicipality;
        }

       
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 fldID
        {
            get
            {
                return _fldID;
            }
            set
            {
                _fldID = value;
            }
        }
        private global::System.Int32 _fldID;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
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
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 fldCityID
        {
            get
            {
                return _fldCityID;
            }
            set
            {

                _fldCityID = value;

            }
        }
        private global::System.Int32 _fldCityID;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String fldInformaticesCode
        {
            get
            {
                return _fldInformaticesCode;
            }
            set
            {

                _fldInformaticesCode = value;

            }
        }
        private global::System.String _fldInformaticesCode;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Byte fldServiceCode
        {
            get
            {
                return _fldServiceCode;
            }
            set
            {

                _fldServiceCode = value;

            }
        }
        private global::System.Byte _fldServiceCode;
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public global::System.String fldRWUserName
        {
            get
            {
                return _fldRWUserName;
            }
            set
            {

                _fldRWUserName = value;

            }
        }
        private global::System.String _fldRWUserName;

        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public global::System.String fldRWPass
        {
            get
            {
                return _fldRWPass;
            }
            set
            {

                _fldRWPass = value;

            }
        }
        private global::System.String _fldRWPass;
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
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
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
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
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
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