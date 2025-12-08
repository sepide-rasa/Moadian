using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data.Entity.Core.Objects.DataClasses;
namespace Avarez.Models
{
    public class User
    {
        public static tblUser CreatetblUser(global::System.Int64 fldID, global::System.String fldName, global::System.Boolean fldStatus, global::System.String fldPassword, global::System.String fldUserName, global::System.String fldMelliCode, global::System.DateTime fldStartDate, global::System.DateTime fldDate)
        {
            tblUser tblUser = new tblUser();
            tblUser.fldID = fldID;
            tblUser.fldName = fldName;
            tblUser.fldStatus = fldStatus;
            tblUser.fldPassword = fldPassword;
            tblUser.fldUserName = fldUserName;
            tblUser.fldMelliCode = fldMelliCode;
            tblUser.fldStartDate = fldStartDate;
            tblUser.fldDate = fldDate;
            return tblUser;
        }


    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
                [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
                [DataMemberAttribute()]
        public global::System.Int64 fldID
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
        private global::System.Int64 _fldID;
    
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
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String fldFamily
        {
            get
            {
                return _fldFamily;
            }
            set
            {
                _fldFamily = value;

            }
        }
        private global::System.String _fldFamily;
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Boolean fldStatus
        {
            get
            {
                return _fldStatus;
            }
            set
            {
                _fldStatus =value;

            }
        }
        private global::System.Boolean _fldStatus;

    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String fldPassword
        {
            get
            {
                return _fldPassword;
            }
            set
            {
                _fldPassword = value;
            }
        }
        private global::System.String _fldPassword;

    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String fldUserName
        {
            get
            {
                return _fldUserName;
            }
            set
            {
                _fldUserName = value;
            }
        }
        private global::System.String _fldUserName;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String fldMelliCode
        {
            get
            {
                return _fldMelliCode;
            }
            set
            {
                _fldMelliCode = value;
            }
        }
        private global::System.String _fldMelliCode;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String fldEmail
        {
            get
            {
                return _fldEmail;
            }
            set
            {

                _fldEmail = value;
 
            }
        }
        private global::System.String _fldEmail;

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String fldNumberAgoTel
        {
            get
            {
                return _fldNumberAgoTel;
            }
            set
            {

                _fldNumberAgoTel = value;

            }
        }
        private global::System.String _fldNumberAgoTel;
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String fldTel
        {
            get
            {
                return _fldTel;
            }
            set
            {
               
                _fldTel = value;

            }
        }
        private global::System.String _fldTel;

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String fldMobile
        {
            get
            {
                return _fldMobile;
            }
            set
            {
                _fldMobile = value;
            }
        }
        private global::System.String _fldMobile;

    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String fldStartDate
        {
            get
            {
                return _fldStartDate;
            }
            set
            {

                _fldStartDate = value;
            }
        }
        private global::System.String _fldStartDate;

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int64> fldCountryDivisions
        {
            get
            {
                return _fldCountryDivisions;
            }
            set
            {

                _fldCountryDivisions = value;

            }
        }
        private Nullable<global::System.Int64> _fldCountryDivisions;

    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int64> fldUserID
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
        private Nullable<global::System.Int64> _fldUserID;

    
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

        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
        [DataMemberAttribute()]
        public int fldType
        {
            get
            {
                return _fldType;
            }
            set
            {
                _fldType = value;
            }
        }
        private int _fldType;

        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
        [DataMemberAttribute()]
        public int fldCode
        {
            get
            {
                return _fldCode;
            }
            set
            {
                _fldCode = value;
            }
        }
        private int _fldCode;
    }
}