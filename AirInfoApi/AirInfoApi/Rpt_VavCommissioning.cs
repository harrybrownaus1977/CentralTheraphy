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
    
    public partial class Rpt_VavCommissioning
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Rpt_VavCommissioning()
        {
            this.Rpt_VavCommissioningVavs = new HashSet<Rpt_VavCommissioningVavs>();
        }
    
        public System.Guid ReportID { get; set; }
        public Nullable<System.Guid> SystemReportID_fk { get; set; }
        public Nullable<int> NoVavs { get; set; }
        public Nullable<bool> SetUpComplete { get; set; }
        public string HeadBuilding { get; set; }
        public Nullable<double> AHUDesignTotal { get; set; }
    
        public virtual tblSystemReport tblSystemReport { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rpt_VavCommissioningVavs> Rpt_VavCommissioningVavs { get; set; }
    }
}