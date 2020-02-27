using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NetCore.Data.Classes
{
    public class UserRole
    {
        [Key]
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public byte RolePriority { get; set; }
        public System.DateTime ModifiedUtcDate { get; set; }

        public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }
    }
}
