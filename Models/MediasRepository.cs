using DAL;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class MediasRepository : Repository<Media>
    {
        public List<string> MediasCategories()
        {
            List<string> Categories = new List<string>();
            foreach (Media media in ToList().OrderBy(m => m.Category))
            {
                if (Categories.IndexOf(media.Category) == -1)
                {
                    Categories.Add(media.Category);
                }
            }
            return Categories;
        }

        public List<string> MediasUsers()
        {
            List<string> Users = new List<string>();
            foreach (Media media in ToList().OrderBy(m => m.Owner.Name))
            {
                if (Users.IndexOf(media.Owner.Name) == -1)
                {
                    Users.Add(media.Owner.Name);
                }
            }
            return Users;
        }




        public List<Media> GetUserMedias(int userId)
        {
            return ToList().Where(m => m.OwnerId == userId).ToList();
        }
        public List<User> GetMediaOwners()
        {
            return ToList().GroupBy(m => m.OwnerId).Select(g => DB.Users.Get(g.Key)).Where(u => u != null).OrderBy(u => u.Name).ToList();
        }

        public void DeleteUserMedias(int userId)
        {
            foreach (var media in GetUserMedias(userId))
            {
                Delete(media.Id);
            }
        }

        public override bool Delete(int id)
        {
            DB.Likes.DeleteMediaLikes(id);
            return base.Delete(id);

        }
    }
}