using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WebSecure.Models
{
    [Table("UserTokens")]
    public class UserTokens
    {
        [Key]
        public long HashId { get; set; }
        public string PasswordSalt { get; set; }
        public long UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
