namespace NotMVVMProject.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Materials : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Materials()
        {
            MaterialToProduct = new HashSet<MaterialToProduct>();
        }

        [Key]
        [StringLength(50)]
        public string MaterialName { get; set; }

        [Required]
        [StringLength(50)]
        public string Price { get; set; }

        public int Amount { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialToProduct> MaterialToProduct { get; set; }
    }
}
