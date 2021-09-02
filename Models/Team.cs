using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FFProject1.Models
{
    public class Team
    {
        [Key]
        public int TeamId {get;set;}
        [Required]
        public string TeamName {get;set;}
        [Required]
        public string PointType {get;set;}
        [Required]
        public string TeamOwner {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public int UserId {get;set;}
        public Team UserTeams {get;set;}
        public List<Player> AllPlayers {get;set;}
        public List<WatchPlayer> PlayersWatched {get;set;}
    }
}