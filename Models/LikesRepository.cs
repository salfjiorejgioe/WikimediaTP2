using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class LikesRepository : Repository<Like>
    {

        public List<Like> GetMediaLikes(int mediaId)
        {
            return ToList().Where(l => l.MediaId == mediaId).ToList();
        }
        public Like GetUserMediaLike(int userId, int mediaId)
        {
            return ToList().FirstOrDefault(l => l.UserId == userId && l.MediaId == mediaId);
        }


        public bool AlreadyLiked(int userId, int mediaId)
        {
            return ToList().Any(l => l.UserId == userId && l.MediaId == mediaId);
        }
    }
}