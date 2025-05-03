using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Email
{
    public class EmailTemplateDto
    {
        Guid id { get; set; }
        string name { get; set; }
        string subject { get; set; }
        string body { get; set; }
        string description { get; set; }
        bool isUserCreated { get; set; } // true = user created, false = default template
    }
}
