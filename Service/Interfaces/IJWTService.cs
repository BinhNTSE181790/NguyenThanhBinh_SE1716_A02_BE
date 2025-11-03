using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IJWTService
    {
        string GenerateToken(int id, string name, string email, int role);
        ClaimsPrincipal ValidateToken(string token);
        string GenerateRefreshToken();
    }
}
