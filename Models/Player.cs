using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FFProject1.Models
{
    public class Player
    {
        [Key]
        public int PlayerId {get;set;}
        [Required]
        public string FirstName {get;set;}
        [Required]
        public string LastName {get;set;}
        [Required]
        public string Position {get;set;}
        [Required]
        public string Starter{get;set;}
        [Required]
        public string FantasyTeam {get;set;}
        [Required]
        public string Team {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public int UserId {get;set;}
        // Navigation
        public User User {get;set;}
        public List<WatchPlayer> UsersWhoWatched {get;set;}
    }
}