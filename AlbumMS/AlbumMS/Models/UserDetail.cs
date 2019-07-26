using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PagedList;
namespace AlbumMS.Models
{
    public class Photos
    {
        public Photos()
        {          
            UserList = new User();
            AlbumsList = new Albums();          
        }
        public int albumId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string thumbnailUrl { get; set; }
        public User UserList { get; set; }
        public Albums AlbumsList { get; set; }
        public PagedList<Photos> ModelPhotoList { get; set; }
    }    
    public class User
    { 
        public User()
        {
            address = new UserAddress();
        }
        public int id { get; set; }
       
        public string name { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public UserAddress address { get; set; }
        public string phone { get; set; }
        public string website { get; set; }  
    }
    public class UserAddress
    {
        public UserAddress()
        {
            listgeo = new Geo();
        }
        public string street { get; set; }
        public string suite { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public Geo listgeo { get; set; }

    }
    public class Geo
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }
    public class Albums
    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
    }

}