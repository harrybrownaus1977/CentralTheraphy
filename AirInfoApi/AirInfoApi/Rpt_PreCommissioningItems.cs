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
    
    public partial class Rpt_PreCommissioningItems
    {
        public System.Guid ReportItemID { get; set; }
        public Nullable<System.Guid> ReportID_fk { get; set; }
        public Nullable<System.Guid> ReportItemListID_fk { get; set; }
        public Nullable<bool> IsNotApplicable { get; set; }
        public Nullable<bool> IsSatisfactory { get; set; }
        public Nullable<bool> IsNonConforming { get; set; }
    
        public virtual Rpt_PreCommissioning Rpt_PreCommissioning { get; set; }
        public virtual Rpt_PreCommissioningItemsList Rpt_PreCommissioningItemsList { get; set; }
    }
}
