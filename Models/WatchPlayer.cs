using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FFProject1.Models
{
    public class WatchPlayer
    {
        [Key]
        public int WatchPlayerId {get;set;}
        public int UserId {get;set;}
        public int PlayerId {get;set;}
        public User User {get;set;}
        public Player Player {get;set;}
    }
}