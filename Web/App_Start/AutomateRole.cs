using Entities;
using Services;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Web.App_Start
{
    public class AutomateRole
    {
        public static void createRolesandUsers()
        {
            var context = new SVIContext();
            var dataRole = context.Roles.ToList();
            if (dataRole.Count == 0)
            {
                string role = ConfigurationManager.AppSettings["Role"];
                string[] roles = role.Split(',');
                foreach (var item in roles)
                {
                    Role model = new Role();
                    model.RoleId = Guid.NewGuid();
                    model.RoleName = item;
                    model.CreatedOn = DateTime.Now;
                    context.Roles.Add(model);
                    context.SaveChanges();
                }
                //Save SuperAdmin
                var user = new User();
                user.CreatedOn = DateTime.Now;
                user.Name = "Admin";
                user.Varified = true;
                user.RoleId = context.Roles.FirstOrDefault(x => x.RoleName == "Admin").RoleId;
                user.Email = ConfigurationManager.AppSettings["AdminEmail"];
                user.Password = HelperService.Instance.Encrypt(ConfigurationManager.AppSettings["AdminPassword"].ToString());
                context.Users.Add(user);
                context.SaveChanges();
            }
        }
    }
}