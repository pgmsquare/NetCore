using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NetCore.Data.DataModels
{
    /// <summary>
    /// 사용자 소유권한
    /// </summary>
    public class UserRolesByUser
    {
        [Key, StringLength(50), Column(TypeName = "varchar(50)")]
        public string UserId { get; set; }

        [Key, StringLength(50), Column(TypeName = "varchar(50)")]
        public string RoleId { get; set; }

        [Required]
        public DateTime OwnedUtcDate { get; set; }

        public virtual User User { get; set; }

        public virtual UserRole UserRole { get; set; }
    }
}
