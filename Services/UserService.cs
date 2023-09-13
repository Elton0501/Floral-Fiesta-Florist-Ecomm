using DataModels;
using Entities;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Services
{
    public class UserService
    {
        #region singleton
        public static UserService Instance
        {
            get
            {
                if (instance == null) instance = new UserService();
                return instance;
            }
        }
        private static UserService instance { get; set; }

        public UserService()
        {
        }
        #endregion

        public IEnumerable<User> GetUsers()
        {
            var data = new List<User>();
            using (var context = new SVIContext())
            {
                data = context.Users.OrderByDescending(x => x.Id).ToList();
                return data;
            }
        }

        public Address GetAddress(int id)
        {
            using(var context = new SVIContext())
            {
                return context.Addresses.FirstOrDefault(x=>x.UserId == id);
            }
        }

        public string GetCurrentUserLogin()
        {
            string data = null;
            HttpCookie WebCookie = HttpContext.Current.Request.Cookies[Constant.WebCookie];
            if (WebCookie != null)
            {
                 data = HelperService.Instance.Decrypt(WebCookie.Value.ToString());
            }
            return data;
        }
        public User GetUser(string loginUser)
        {
            using (var context = new SVIContext())
            {
                return context.Users.FirstOrDefault(x => x.Email == loginUser);
            }
        }

        public string GetCurrentUserName(string email)
        {
            string data = null;
            using (var context = new SVIContext())
            {
                var getdata = context.Users.FirstOrDefault(x => x.Email == email);
                data = getdata != null ? getdata.Name != "" && getdata.Name != null ? getdata.Name : email : null;
            }
            return data;
        }

        public User GetUserByUID(int userId)
        {
            using (var context = new SVIContext())
            {
                return context.Users.FirstOrDefault(x => x.Id == userId);
            }
        }

        public void SaveAddress(AddressDataModels address)
        {
            using(var context = new SVIContext())
            {
                var data = context.Addresses.FirstOrDefault(x=>x.UserId == address.UserId);
                if (data == null)
                {
                    Address address1 = new Address();
                    address1.UserId = address.UserId;
                    address1.HouseNo = address.HouseNo;
                    address1.Street = address.Street;
                    address1.Country = address.Country;
                    address1.State = address.State;
                    address1.Pincode = address.Pincode;
                    address1.City = address.City;
                    address1.LandMark = address.LandMark;
                    context.Addresses.Add(address1);
                    context.SaveChanges();
                }
                else
                {
                    context.Entry(address).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public void RemoveUser(int id)
        {
            using(var context = new SVIContext())
            {
                var AdminRole = context.Roles.FirstOrDefault(x => x.RoleName == Constant.Role.Admin);
                var data = context.Users.FirstOrDefault(x=>x.Id == id);
                if (data.RoleId != AdminRole.RoleId)
                {
                    context.Users.Remove(data);
                    context.SaveChanges();
                }
            }
        }
    }
}
