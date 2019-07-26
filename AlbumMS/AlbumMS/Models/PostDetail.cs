using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlbumMS.Models
{
        public class PostData
        {
            public PostData()
            {
                lslUserDetails = new User();
            }
            public int userId { get; set; }

            public int id { get; set; }
            public string title { get; set; }

            public string body { get; set; }

            public User lslUserDetails { get; set; }
        }
 }