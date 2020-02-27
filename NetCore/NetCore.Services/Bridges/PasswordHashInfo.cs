using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Services.Bridges
{
    public class PasswordHashInfo
    {
        public string GUIDSalt { get; set; }

        public string RNGSalt { get; set; }

        public string PasswordHash { get; set; }
    }
}
