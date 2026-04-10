using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace Models
{
    public enum MediaSortBy { Title, PublishDate, Likes }

    public class Media : Record
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string YoutubeId { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.Now;

        public int OwnerId { get; set; } = 1;
        public bool Shared { get; set; } = true;
        [JsonIgnore]
        public User Owner => DB.Users.Get(OwnerId).Copy();


        [JsonIgnore]
        public List<Like> Likes { get { return DB.Likes.ToList().Where(l => l.MediaId == Id).ToList(); } }


        [JsonIgnore]
        public int LikesCount { get { return Likes.Count(); } }

        [JsonIgnore]
        public List<User> LikedUsers => Likes.Select(l => DB.Users.Get(l.UserId).Copy()).ToList();

        [JsonIgnore]
        public bool IsLikedByCurrentUser => Likes.Any(l => l.UserId == Models.User.ConnectedUser.Id);
        public string LikedUsersNames
        {
            get
            {
                var users = Likes.Select(l => DB.Users.Get(l.UserId)).
                    Where(u => u != null).Select(u => u.Name);
                return string.Join(",", users);
            }
        }

        public override bool IsValid()
        {
            if (!HasRequiredLength(Title, 1)) return false;
            if (!HasRequiredLength(Category, 1)) return false;
            if (!HasRequiredLength(Description, 1)) return false;
            if (DB.Medias.ToList().Where(m => m.YoutubeId == YoutubeId && m.Id != Id).Any()) return false;
            return true;
        }
    }
}