using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Role
    {
        public int Id { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
