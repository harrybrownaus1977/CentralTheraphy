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
    
    public partial class Rpt_PreCommissioningItemsList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Rpt_PreCommissioningItemsList()
        {
            this.Rpt_PreCommissioningItems = new HashSet<Rpt_PreCommissioningItems>();
        }
    
        public System.Guid ReportItemListID { get; set; }
        public Nullable<System.Guid> TemplateID_fk { get; set; }
        public string CategoryName { get; set; }
        public string ItemName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public Nullable<int> CategoryOrder { get; set; }
        public int FranchiseID_fk { get; set; }
    
        public virtual FlowTech_MasterReportList FlowTech_MasterReportList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rpt_PreCommissioningItems> Rpt_PreCommissioningItems { get; set; }
    }
}