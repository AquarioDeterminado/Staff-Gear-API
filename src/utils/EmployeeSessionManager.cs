namespace API.src.utils
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using API.src.models;
    using Microsoft.IdentityModel.Tokens;

    public static class EmployeeSessionManager
    {
        private static AdventureWorksContext _db = new AdventureWorksContext();
        

        static public async Task<bool> EmployeeIsActive(int id)
        {
            var emp = await _db.Employee.FindAsync(id);
            return emp != null && emp.CurrentFlag == false; // true = ativo
        }

        static public bool UserIsHimself(ClaimsPrincipal user, int id)
        {
            var employeeUserName = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(employeeUserName))
                return false;

            var userEmployeeId = _db.Users
                .Where(u => u.Username.ToString() == employeeUserName)
                .Select(u => u.EmployeeId)
                .FirstOrDefault();

            return userEmployeeId == id;
        }
    }
}