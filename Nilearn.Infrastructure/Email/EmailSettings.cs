using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Infrastructure.Email
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpFrom { get; set; }
        public bool EnableSsl { get; set; }
    }
}
