using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SavageOrcs.BusinessObjects { 

    public class User : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; } = "";

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; } = "";

        [PersonalData]
        public bool? FromUkraine { get; set; }

        public virtual ICollection<Mark> Marks { get; set; } = new HashSet<Mark>();

        public virtual ICollection<Map> Maps { get; set; } = new HashSet<Map>();

        public virtual ICollection<Curator> Curators { get; set; } = new HashSet<Curator>();

    } 
}