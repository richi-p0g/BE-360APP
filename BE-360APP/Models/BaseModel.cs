namespace BE_360APP.Models
{
    public class BaseModel
    {
    }


    public class AssignToOtherDto
    {
        public int idusernameFrom { get; set; }
        public string? usernameFrom { get; set; }
        public string? fullnameFrom { get; set; }
        public string? usernameAssign { get; set; }
        public string? fullnameAssign { get; set; }

        public string? projectopty { get; set; }
        public int quickvalue { get; set; }
        public string? status { get; set; }
        public string? signsatus { get; set; }
        public string? deadline { get; set; }
        public string? note { get; set; }
    }
        public class usernameDto
    {
        public string? fullname { get; set; }
    }

    public class Registerfullname
    {
        public string? RowIdSage { get; set; }
        public string? fullname { get; set; }
        public string? email { get; set; }
        public string? username { get; set; }
        public string? role { get; set; }

    }

    public class loginDto
    {
        public int id { get; set; }
        public string fullName { get; set; }
        public string usernamePekerja { get; set; }
        public string Password { get; set; }
        public string email { get; set; }
        public bool isVerify { get; set; }
        public string role { get; set; }
    }

    public class registerDto
    {
        public string RowIdSage { get; set; }
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

    public class Response : loginDto
    {
        public string Msg { get; set; } = "Successfully";
        public bool isError { get; set; } = false;
        public int rowsPerPage { get; set; }
        public string? currentVersion { get; set; }
        public string? linkUpdate { get; set; }
        public List<int>? availableRowsPerPage { get; set; }
    }


    public class mastervalues
    {
        public List<quickValue> quicks { get; set; } = new List<quickValue>();
        public List<finalValue> finals { get; set; } = new List<finalValue>();
    }

    public class quickValue
    {
        //public int id { get; set; }
        public int quick { get; set; }
        public string desc { get; set; }

    }

    public class finalValue
    {
        //public int id { get; set; }
        public int final { get; set; }
        public string desc { get; set; }

    }


}
