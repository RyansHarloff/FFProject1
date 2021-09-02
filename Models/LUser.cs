using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FFProject1.Models
{
    public class LUser
    {
        [Required]
        [EmailAddress]
        public string LEmail {get;set;}
        [Required]
        [MinLength(8,ErrorMessage ="Password must be 8 characters")]
        [DataType(DataType.Password)]
        public string LPassword {get;set;}
    }
}