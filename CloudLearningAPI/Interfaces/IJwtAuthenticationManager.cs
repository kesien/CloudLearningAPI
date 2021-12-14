using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudLearningAPI.Interfaces
{
    public interface IJwtAuthenticationManager
    {
        public string Authenticate(string username);
    }
}
