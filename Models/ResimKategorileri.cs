//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nakliyat.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ResimKategorileri
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ResimKategorileri()
        {
            this.Resimler = new HashSet<Resimler>();
        }
    
        public int id { get; set; }
        public string KategoriAdı { get; set; }
        public int Genislik { get; set; }
        public int Yukseklik { get; set; }
        public Nullable<int> MaxValue { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Resimler> Resimler { get; set; }
    }
}