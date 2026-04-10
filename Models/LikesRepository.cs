using DAL;
using System.Collections.Generic;
using System.Linq;
using WebApplication.Models;

namespace Models
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

        public List<Like> GetUserLikes(int userId)
        {
            return ToList().Where(l => l.UserId == userId).ToList();
        }

        public void DeleteMediaLikes(int mediaId)
        {
            foreach (var like in GetMediaLikes(mediaId))
            {
                Delete(like.Id);
            }
        }

        public void DeleteUserLikes(int userId)
        {
            foreach (var like in GetUserLikes(userId))
            {
                Delete(like.Id);
            }
        }
    }
}