using Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;

namespace DAL
{
    public sealed class DB
    {
        #region singleton setup
        private static readonly DB instance = new DB();
        public static DB Instance { get { return instance; } }
        #endregion

        public static MediasRepository Medias { get; set; } = new MediasRepository();

    }
}