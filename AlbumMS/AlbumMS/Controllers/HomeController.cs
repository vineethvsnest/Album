using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlbumMS.Models;
using System.Net.Http;
using PagedList;
using System.Threading;
namespace AlbumMS.Controllers
{
    public class HomeController : Controller
    {
        List<Photos> lstModelPhotoList = null;

        [HttpGet]
        public ActionResult Index(string strOption, string strSearch, int? pageNumber)
        {            
            try
            {
                ViewBag.strOption = strOption;
                ViewBag.strSearch = strSearch;

                if (TempData["Data"] == null)
                {
                    //Get All details of Album,Photo,User
                    lstModelPhotoList = getAllPhotoAlbumUserDetails();
                }
                else
                {
                    lstModelPhotoList = (List<Photos>)TempData["Data"];
                }
                if (strOption == "Title")
                    return View(lstModelPhotoList.Where(x => x.AlbumsList.title.Contains(strSearch) || strSearch == null).ToPagedList(pageNumber ?? 1, 10));
                else if (strOption == "UserName")
                    return View(lstModelPhotoList.Where(x => x.UserList.name.Contains(strSearch) || strSearch == null).ToPagedList(pageNumber ?? 1, 10));
                else
                    return View(lstModelPhotoList.ToPagedList(pageNumber ?? 1, 10));


            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Home", "Index"));
            } 
        }
        public List<Photos> getAllPhotoAlbumUserDetails()
        {

            string strPhotoUrl = "https://jsonplaceholder.typicode.com/photos";
            string strAlbumUrl = "https://jsonplaceholder.typicode.com/albums";
            string strUserUrl = "https://jsonplaceholder.typicode.com/users";
                     
            List<Photos> lstPhotoList = null;
            List<Albums> lstAlbumList = null;
            List<User> lstUserList = null;


            var PhotoList = GetPhotoDetailsFromAPI(strPhotoUrl);
            lstPhotoList = (List<Photos>)PhotoList.Result;
            lstModelPhotoList = lstPhotoList.GroupBy(p => p.albumId).Select(g => g.First()).ToList();


            var AlbumList = GetAlbumDetailsFromAPI(strAlbumUrl);
            lstAlbumList = (List<Albums>)AlbumList.Result;

            foreach (Photos item in lstModelPhotoList)
            {
                if (lstAlbumList.Where(a => a.id == item.albumId).FirstOrDefault() != null)
                {
                    item.AlbumsList.title = lstAlbumList.Where(a => a.id == item.albumId).FirstOrDefault().title;
                    item.UserList.id = lstAlbumList.Where(a => a.id == item.albumId).FirstOrDefault().userId;
                    item.AlbumsList.id = lstAlbumList.Where(a => a.id == item.albumId).FirstOrDefault().id;
                }
            }

            var UserList = GetUserDetailsFromAPI(strUserUrl);
            lstUserList = (List<User>)UserList.Result;

            foreach (Photos item in lstModelPhotoList)
            {
                if (lstUserList.Where(a => a.id == item.UserList.id).FirstOrDefault() != null)
                {
                    item.UserList.name = lstUserList.Where(a => a.id == item.UserList.id).FirstOrDefault().name;
                    item.UserList.username = lstUserList.Where(a => a.id == item.UserList.id).FirstOrDefault().username;
                    item.UserList.email = lstUserList.Where(a => a.id == item.UserList.id).FirstOrDefault().email;
                    item.UserList.phone = lstUserList.Where(a => a.id == item.UserList.id).FirstOrDefault().phone;
                    item.UserList.address.street = lstUserList.Where(a => a.id == item.UserList.id).FirstOrDefault().address.street;
                    item.UserList.address.suite = lstUserList.Where(a => a.id == item.UserList.id).FirstOrDefault().address.suite;
                    item.UserList.address.city = lstUserList.Where(a => a.id == item.UserList.id).FirstOrDefault().address.city;
                    item.UserList.address.zipcode = lstUserList.Where(a => a.id == item.UserList.id).FirstOrDefault().address.zipcode;
                }
            }
            TempData["Data"] = lstModelPhotoList;
            return lstModelPhotoList;

        }
        public async System.Threading.Tasks.Task<List<Photos>> GetPhotoDetailsFromAPI(string PhotoUrl)
        {
            //Get all Photos - API
            HttpClient client;          
            List<Photos> lstPhotoList = null;            
            using (client = new HttpClient())
            {
                client.BaseAddress = new Uri(PhotoUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync(PhotoUrl);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = await result.Content.ReadAsStringAsync();
                    lstPhotoList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Photos>>(data);
                 }              
           }            
            return lstPhotoList;
        }
        public async System.Threading.Tasks.Task<List<Albums>> GetAlbumDetailsFromAPI(string strAlbumUrl)
        {
            //Get all Albums- API
            HttpClient client;
            List<Albums> lblAlbumList = null;            

            using (client = new HttpClient())
            {
                client.BaseAddress = new Uri(strAlbumUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync(strAlbumUrl);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = await result.Content.ReadAsStringAsync();
                    lblAlbumList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Albums>>(data);
                }
            }           
            return lblAlbumList;
        }

        public async System.Threading.Tasks.Task<List<User>> GetUserDetailsFromAPI(string strUserUrl)
        {
            //Get all user details- API
            HttpClient client;
            List<User> lstUserList = null;            

            using (client = new HttpClient())
            {
                client.BaseAddress = new Uri(strUserUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync(strUserUrl);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = await result.Content.ReadAsStringAsync();
                    lstUserList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(data);
                }
            }           
            return lstUserList;
        } 
        public ActionResult AlbumDetails(int? AlbumId)
        {
            //Get All Alubum Details By AlbumID
            try
            {
                string strPhotoUrl = "https://jsonplaceholder.typicode.com/photos";
                List<Photos> lstPhotoList = null;
                var PhotoList = GetPhotoDetailsFromAPI(strPhotoUrl);
                lstPhotoList = (List<Photos>)PhotoList.Result;
                return View(lstPhotoList.Where(x => x.albumId == AlbumId));
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Home", "Index"));
            }
        }
        public ActionResult toHomePage()
        {
            //Redirect to HomePage
            return RedirectToAction("Index");
        }
        public ActionResult UserDetails(int userid)
        {
            try
            {
                IList<PostData> lstAllPostData = null;
                List<PostData> lstPostData = null;
                User lstuserList = null;

                string strPostUrl = "https://jsonplaceholder.typicode.com/posts";

                var PostList = GetAllPostsFromAPI(strPostUrl);
                lstAllPostData = (List<PostData>)PostList.Result;
                lstPostData = lstAllPostData.Where(x => x.userId == userid).ToList();

                string strUserUrl = "https://jsonplaceholder.typicode.com/users/" + userid;
                var UserList = GetUserDetailFromAPI(strUserUrl);
                lstuserList = UserList.Result;

                foreach (PostData item in lstPostData)
                {
                    item.lslUserDetails.name = lstuserList.name;
                    item.lslUserDetails.username = lstuserList.username;
                    item.lslUserDetails.email = lstuserList.email;
                    item.lslUserDetails.phone = lstuserList.phone;
                    item.lslUserDetails.address.street = lstuserList.address.street;
                    item.lslUserDetails.address.suite = lstuserList.address.suite;
                    item.lslUserDetails.address.city = lstuserList.address.city;
                    item.lslUserDetails.address.zipcode = lstuserList.address.zipcode;
                }
                return View(lstPostData);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Home", "Index"));
            }

        }
        public async System.Threading.Tasks.Task<User>GetUserDetailFromAPI(string strUserUrl)
        {
            //Get user detail- API
            HttpClient client;
            User lstUserList = null;

            using (client = new HttpClient())
            {
                client.BaseAddress = new Uri(strUserUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync(strUserUrl);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = await result.Content.ReadAsStringAsync();
                    lstUserList = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(data);
                }
            }
            return lstUserList;
        }
        public async System.Threading.Tasks.Task<IList<PostData>> GetAllPostsFromAPI(string strPostUrl)
        {
            //Get all Posts from API
            HttpClient client;
            IList<PostData> lstPostList = null;
            using (client = new HttpClient())
            {
                client.BaseAddress = new Uri(strPostUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync(strPostUrl);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = await result.Content.ReadAsStringAsync();
                    lstPostList = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<PostData>>(data);
                }
            }
            return lstPostList;
        }
    }
}