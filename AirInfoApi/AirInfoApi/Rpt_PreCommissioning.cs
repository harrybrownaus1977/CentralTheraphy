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
    
    public partial class Rpt_PreCommissioning
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Rpt_PreCommissioning()
        {
            this.Rpt_PreCommissioningItems = new HashSet<Rpt_PreCommissioningItems>();
        }
    
        public System.Guid ReportID { get; set; }
        public Nullable<System.Guid> SystemReportID_fk { get; set; }
        public string HeadBuilding { get; set; }
        public string HeadService { get; set; }
        public string HeadCustomer { get; set; }
        public string NonConformance { get; set; }
    
        public virtual tblSystemReport tblSystemReport { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rpt_PreCommissioningItems> Rpt_PreCommissioningItems { get; set; }
    }
}
