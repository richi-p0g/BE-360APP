namespace BE_360APP.Models
{
    public class BaseModel
    {
    }

    public class usernameDto
    {
        public string? fullname { get; set; }
    }

    public class Registerfullname
    {
        public string? fullname { get; set; }
        public string? email { get; set; }
        public string? username { get; set; }
        public string? role { get; set; }

    }

    public class loginDto
    {
        public string usernamePekerja { get; set; }
        public string Password { get; set; }
    }

    public class registerDto
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string fullname { get; set; }
    }



    public class whereString
    {
        public int GetField { get; set; }
        public string? GetFlag { get; set; }
        public string? Param1 { get; set; }
        public string? Param2 { get; set; }
        public string? Param3 { get; set; }
        public string? Param4 { get; set; }
    }

    public class Dashboard
    {
        public string? MyProperty { get; set; }
    }

    public class Response
    {
        public string Msg { get; set; } = "Successfully";
        public bool isError { get; set; } = false;
    }

}
