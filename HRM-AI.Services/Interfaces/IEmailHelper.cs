using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_AI.Services.Interfaces
{
    public interface IEmailHelper
    {
        Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml);
    }
}
