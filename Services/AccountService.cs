using DataModels;
using Entities;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AccountService
    {
        #region singleton
        public static AccountService Instance
        {
            get
            {
                if (instance == null) instance = new AccountService();
                return instance;
            }
        }
        private static AccountService instance { get; set; }
        public AccountService()
        {
        }
        #endregion

        public ResultModel Regiter(User user)
        {
            var result = new ResultModel();
            result.Result = false;
            using (var context = new SVIContext())
            {
                var data = context.Users.FirstOrDefault(x => x.Email.ToLower() == user.Email.ToLower());
                if (data == null)
                {
                    user.Password = HelperService.Instance.Encrypt(user.Password);
                    context.Users.Add(user);
                    context.SaveChanges();
                    result.Result = true;
                    result.Value1 = user.Id.ToString();
                }
                else
                {
                    result.Messsage = "This user already exists";
                }
            }
            return result;
        }

        public Guid GetRoleIdByName(string roleName)
        {
            using (var context = new SVIContext())
            {
                return context.Roles.FirstOrDefault(x => x.RoleName == roleName).RoleId;
            }
        }

        public ResultModel Login(string email, string password)
        {
            var result = new ResultModel();
            result.Result = false;
            password = HelperService.Instance.Encrypt(password);
            using (var context = new SVIContext())
            {
                var adminRole = context.Roles.FirstOrDefault(x => x.RoleName == Constant.Role.Admin);
                var userRole = context.Roles.FirstOrDefault(x => x.RoleName == Constant.Role.User);
                var newdata = context.Users.ToList();
                var data = context.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower()
                && x.Password == password
                );
                if (data == null)
                {
                    result.Messsage = "Please enter a valid email or password";
                }
                else if (data != null && data.Varified == false)
                {
                    result.Messsage = "Please verify your e-mail.";
                }
                else
                {
                    result.Result = true;
                    result.Value1 = data.Name.ToString();
                }

                if(data != null && data.RoleId == adminRole.RoleId)
                {
                    result.Value2 = "Admin";
                }
                else if (data != null && data.RoleId == userRole.RoleId)
                {
                    result.Value2 = "User";
                }
            }
            return result;
        }

        public ResultModel ForgotPassword(string email)
        {
            var result = new ResultModel();
            result.Result = false;
            using (var context = new SVIContext())
            {
                var data = context.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
                if (data == null)
                {
                    result.Messsage = "This email does not exist";
                }
                else
                {
                    result.Result = true;
                    result.Messsage = "Please reset your password using the mail in your inbox.";
                }
            }
            return result;
        }

    }
}
