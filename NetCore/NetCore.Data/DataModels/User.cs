using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NetCore.Data.DataModels
{
    //1. Data Annotations
    public class User
    {
        [Key, StringLength(50), Column(TypeName = "varchar(50)", Order = 0)]
        public string UserId { get; set; }

        [Required, StringLength(100), Column(TypeName = "nvarchar(100)")]
        public string UserName { get; set; }

        //중복 안되는 인덱스 지정
        [Required, StringLength(320), Column(TypeName = "varchar(320)")]
        public string UserEmail { get; set; }

        [Required, StringLength(130), Column(TypeName = "nvarchar(130)")]
        public string Password { get; set; }

        [Required]
        public bool IsMembershipWithdrawn { get; set; }

        [Required]
        public DateTime JoinedUtcDate { get; set; }

        //FK 지정
        [ForeignKey("UserId")]
        public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }
    }
}
