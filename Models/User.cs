using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FFProject1.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required]
        [EmailAddress]
        public string Email {get;set;}
        [Required]
        [MinLength(8, ErrorMessage ="Password must be 8 Characters")]
        [DataType(DataType.Password)]
        public string Password {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List<Player> AllPlayers {get;set;}
        public List<Team> AllTeams {get;set;}
        public List<WatchPlayer> PlayersWatched {get;set;}

        [NotMapped]
        [Required]
        [DataType("Password")]
        [Compare("Password")]
        public string Confirm {get;set;}
    }
}
