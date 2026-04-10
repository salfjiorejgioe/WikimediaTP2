using DAL;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MediaId { get; set; }
        public int OwnerId { get; set; }
        [JsonIgnore]
        public User Owner => DB.Users.Get(OwnerId).Copy();



    }
}