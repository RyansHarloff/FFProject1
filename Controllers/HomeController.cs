using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FFProject1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace FFProject1.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                // Checking DataBase for unique email
                if(_context.Users.Any(e => e.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in Use!");
                    return View("index");
                } else
                {
                    //Time to hash password
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    _context.Add(newUser);
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("loggedin", newUser.UserId);
                    return View("Dashboard");
                }
            }else{
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LUser LogUser)
        {
            if(ModelState.IsValid)
            {
                //Find the email in DB
                User userInDb = _context.Users.FirstOrDefault(u => u.Email == LogUser.LEmail);
                if(userInDb == null)
                {
                    //No Email in DB
                    ModelState.AddModelError("LEmail", "Invalid Login Attempt!");
                    return View("Index");
                } else
                {
                    //Found the User, Compare Password
                    PasswordHasher<LUser> hasher = new PasswordHasher<LUser>();
                    var result = hasher.VerifyHashedPassword(LogUser, userInDb.Password, LogUser.LPassword);
                    if(result == 0)
                    {
                        ModelState.AddModelError("LEmail", "Invalid Login Attempt!");
                        return View("Index");
                    }
                    HttpContext.Session.SetInt32("loggedIn", userInDb.UserId);
                    return RedirectToAction("DashBoard");
                }
            } else{
                return View("Index");
            }
        }
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            int? loggedIn = HttpContext.Session.GetInt32("loggedIn");
            if(loggedIn != null)
            {
                ViewBag.Player =_context.Players.Include(s => s.UsersWhoWatched).Where(a => a.UserId == (int)loggedIn).ToList();
                ViewBag.OtherTeams =_context.UserTeams.OrderBy(g => g.TeamName).ToList();
                ViewBag.User = _context.Users.Include(h => h.AllPlayers).FirstOrDefault(t => t.UserId == (int)loggedIn);
                return View();
            } else{
                return RedirectToAction("Dashboard");
            }
        }
        [HttpGet("AddPlayer")]
        public IActionResult AddPlayer() {
            ViewBag.Player =_context.Players.ToList();
            return View();
        }
        [HttpPost("AddPlayertoDb")]
        public IActionResult AddPlayertoDb(Player newPlayer) {
            if(ModelState.IsValid) {
                newPlayer.UserId = (int)HttpContext.Session.GetInt32("loggedIn");
                _context.Add(newPlayer);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            } else {
                return View("AddPlayer");
            }
        }
        [HttpGet("AddTeam")]
        public IActionResult AddTeam() 
        {
            int? loggedIn = HttpContext.Session.GetInt32("loggedIn");
            if(loggedIn != null) 
            {
                ViewBag.OtherTeams =_context.UserTeams.ToList();
                ViewBag.Players = _context.Players.ToList();
                return View();
            } else {
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost("AddTeamtoDb")]
        public IActionResult AddPlayertoDb(Team newTeam) {
            if(ModelState.IsValid) {
                newTeam.UserId = (int)HttpContext.Session.GetInt32("loggedIn");
                _context.Add(newTeam);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            } else {
                return View("AddPlayer");
            }
        }

        [HttpGet("like/{UserId}/{PlayerId}")]
        public IActionResult Like(int UserId, int PlayerId)
        {
            int? loggedIn = HttpContext.Session.GetInt32("loggedIn");
            if(loggedIn != null)
            {
                if((int)loggedIn != UserId)
                {
                    return RedirectToAction("Logout");
                } else{
                    WatchPlayer newWatchPlayer = new WatchPlayer();
                    newWatchPlayer.UserId = UserId;
                    newWatchPlayer.PlayerId = PlayerId;
                    _context.WatchedPlayers.Add(newWatchPlayer);
                    _context.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
            } else {
                return RedirectToAction("Index");
            }
        } 
        [HttpGet("unlike/{UserId}/{PlayerId}")]
        public IActionResult Unlike(int UserId, int PlayerId)
        {
            int? loggedIn = HttpContext.Session.GetInt32("loggedIn");
            if(loggedIn != null)
            {
                if((int)loggedIn != UserId)
                {
                    return RedirectToAction("Logout");
                } else {
                    WatchPlayer PlayerToRemove = _context.WatchedPlayers.FirstOrDefault(d => d.PlayerId == PlayerId && d.UserId == UserId);
                    _context.WatchedPlayers.Remove(PlayerToRemove);
                    _context.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
            } else {
                return RedirectToAction("Index");
            }
        }
        [HttpGet("UpdatePlayer/{PlayerId}")]
        public IActionResult UpdatePlayer(int PlayerId) {
            Player PlayerToUpdate = _context.Players.FirstOrDefault(j => j.PlayerId == PlayerId);
            ViewBag.Player =_context.Players.ToList();
            return View(PlayerToUpdate);
        }
        [HttpPost("UpdatePlayerInDb/{PlayerId}")]
        public IActionResult UpdatePlayerInDb(int PlayerId, Player PlayerToUpdate) {
            Player oldPlayer = _context.Players.FirstOrDefault(a => a.PlayerId == PlayerId);
            if(ModelState.IsValid) {
                oldPlayer.FirstName = PlayerToUpdate.FirstName;
                oldPlayer.LastName = PlayerToUpdate.LastName;
                oldPlayer.Position = PlayerToUpdate.Position;
                oldPlayer.Starter = PlayerToUpdate.Starter;
                oldPlayer.FantasyTeam = PlayerToUpdate.FantasyTeam;
                oldPlayer.Team = PlayerToUpdate.Team;
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            } else {
                return View("Update", oldPlayer);
            }
        }

        [HttpGet("UpdateTeam/{TeamId}")]
        public IActionResult UpdateTeam(int TeamId) {
            Team TeamToUpdate = _context.UserTeams.FirstOrDefault(j => j.TeamId == TeamId);
            ViewBag.Teams =_context.UserTeams.ToList();
            return View(TeamToUpdate);
        }
        [HttpPost("UpdateTeamInDb/{TeamId}")]
        public IActionResult UpdateTeamInDb(int TeamId, Team TeamToUpdate) {
            Team oldTeam = _context.UserTeams.FirstOrDefault(a => a.TeamId == TeamId);
            if(ModelState.IsValid) {
                oldTeam.TeamName = TeamToUpdate.TeamName;
                oldTeam.PointType = TeamToUpdate.PointType;
                oldTeam.TeamOwner = TeamToUpdate.TeamOwner;
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            } else {
                return View("Update", oldTeam);
            }
        }
        [HttpGet("contact")]
        public IActionResult Contact()
        {
            return View();
        }
        [HttpGet("/delete/{PlayerId}")]
        public IActionResult Delete(int PlayerId)
        {
            Player PlayertoDelete = _context.Players.SingleOrDefault(a => a.PlayerId == PlayerId);
            _context.Players.Remove(PlayertoDelete);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("/deleteteam/{TeamId}")]
        public IActionResult Deleteteam(int TeamId)
        {
            Team TeamtoDelete = _context.UserTeams.SingleOrDefault(a => a.TeamId == TeamId);
            _context.UserTeams.Remove(TeamtoDelete);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
