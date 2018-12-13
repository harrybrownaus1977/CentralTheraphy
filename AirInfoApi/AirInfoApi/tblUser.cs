//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AirInfoApi
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblUser()
        {
            this.tblProjectTechnicians = new HashSet<tblProjectTechnician>();
        }
    
        public System.Guid UserID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public System.Guid CreatedByUserID_fk { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.Guid> ModifiedByUserID_fk { get; set; }
        public bool Archived { get; set; }
        public int RoleID_fk { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public Nullable<System.DateTime> LastPasswordChangeDate { get; set; }
        public bool Active { get; set; }
        public string ResetCode { get; set; }
        public Nullable<System.DateTime> ResetCodeExpiry { get; set; }
        public bool CanDelete { get; set; }
    
        public virtual luRole luRole { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblProjectTechnician> tblProjectTechnicians { get; set; }
    }
}