using Google.Protobuf.WellKnownTypes;

namespace BE_360APP.Models
{
    public class BaseModel
    {
    }


    public class AssignToOtherDto
    {
        public int id { get; set; }
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
        public string usernameFrom { get; set; }
        public string Password { get; set; }
        public string position { get; set; }
        public string templateName { get; set; }
        public string nip { get; set; }
        public bool empJunior { get; set; }
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

    public class masterProjectopties
    {
        public int id { get; set; }
        public string? projectopty { get; set; }
    }


    public class allmasters
    {
        public List<quickValue> quicks { get; set; } = new List<quickValue>();
        public List<finalValue> finals { get; set; } = new List<finalValue>();
        public List<status> status { get; set; } = new List<status>();
        public List<signstatus> signstatus { get; set; } = new List<signstatus>();
    }

    public class status
    {
        public int id { get; set; }
        public string statusvalue { get; set; }
    }

    public class signstatus
    {
        public int id { get; set; }
        public string signstatusvalue { get; set; }
    }

    public class quickValue
    {
        public int quick { get; set; }
        public string desc { get; set; }
    }

    public class finalValue
    {
        public int final { get; set; }
        public string desc { get; set; }

    }

    public class projectOpty
    {
        public string Number { get; set; }
        public string Desc { get; set; }

    }

    public class AssignToOtherGetAllDto
    {
        public string usernameFrom { get; set; }
    }

    public class responReviewgetall
    {
        public int id { get; set; }
        public string namaKaryawan { get; set; }
        public DateTime timerequest { get; set; }
    }

    public class personLayersubmitDto
    {
        public int id { get; set; }
        public string usernameFrom { get; set; }
        public List<Emp> selectedEmployee { get; set; }
        public List<pLayer> selectedPersonLayer { get; set; }
    }

    public class Emp
    {
        public string id { get; set; }
        public string? name { get; set; }
    }

    public class pLayer
    {
        public string id { get; set; }
        public string? name { get; set; }
    }

    public class formFastRateDto
    {
        public int id { get; set; }
        public string? usernameFrom { get; set; }
        public string? idname1 { get; set; }
        public string? idname2 { get; set; }
        public List<Emp> selectedPersonJuniors { get; set; }
    }

    public class ansForm1
    {
        public string idKaryawan { get; set; }
        public string idQuestion { get; set; }
        public string type { get; set; }
        public string answer { get; set; }
        public string remarkdata { get; set; }
    }


    public class ListFastRatingRowSaveDto
    {
        public int id { get; set; }
        public int iduser { get; set; }
        public string usernameFrom { get; set; }
        public List<ansForm1> answerForm1 { get; set; }
    }

    public class Allperson
    {
        public List<subordinate> personlayer2 { get; set; }
        public List<allKaryawan> allKaryawan { get; set; }
    }

    public class structurformFastRating
    {
        public string? fullname { get; set; }
        public string? divlevel { get; set; }
        public int status { get; set; }
        public List<getData>? data { get; set; }
    }



    public class genQuestionText
    {
        public Int16 id { get; set; }
        public string questions { get; set; }
    }

    public class genQuestionRate
    {
        public Int16 id { get; set; }
        public string questions { get; set; }
    }

    public class question
    {
        public int idUser { get; set; }
        public int id { get; set; }
        public string questions { get; set; }
    }


    public class additionReview
    {
        public int id { get; set; }
        public string namaKaryawan { get; set; }
    }

    public class upline
    {
        public int id { get; set; }
        public string namaKaryawan { get; set; }
    }

    public class allKaryawan
    {
        public int id { get; set; }
        public string namaKaryawan { get; set; }
        public string Division { get; set; }
    }

    public class juniorEmployee
    {
        public int id { get; set; }
        public string namaKaryawan { get; set; }
    }

    public class subordinate
    {
        public int id { get; set; }
        public string namaKaryawan { get; set; }
    }

    public class peer
    {
        public int id { get; set; }
        public string namaKaryawan { get; set; }
    }

    public class dataKaryawan
    {
        public int idUser { get; set; }
        public string namaKaryawan { get; set; }
        public string nama { get; set; }
        public string namaLevel { get; set; }
        public string rating { get; set; }
    }


    public class getData
    {
        public List<questionsData>? questions { get; set; }
    }

    public class questionsData
    {
        public string? question_id { get; set; }
        public List<string> fields { get; set; }
        public string _id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public int maxline { get; set; }
        public bool is_mandatory { get; set; }
        public bool remark { get; set; }
    }

    public class fieldsData
    {
        public string bodyField { get; set; }
    }


    public class AssignToMeGetAllDto
    {
        public string usernameFrom { get; set; }
    }


    public class AssignToMeGetAll
    {
        public string RowId { get; set; }
        public string fullnameAssign { get; set; }
        public string usernameAssign { get; set; }
        public string projectopty { get; set; }
        public string quickvalue { get; set; }
        public string status { get; set; }
        public string signstatus { get; set; }
        //public DateTime deadline { get; set; }
        public int deadlineyear { get; set; }
        public int deadlinemonth { get; set; }
        public int deadlineday { get; set; }
        public string note { get; set; }
    }


    public class fullnameAndid
    {
        public string idpengajuantask { get; set; }
        public string iduser { get; set; }
        public string fullname { get; set; }
        public string ratee { get; set; }
    }


    public class AssignToOtherGetAll
    {
        public string RowId { get; set; }
        public string fullnameAssign { get; set; }
        public string usernameAssign { get; set; }
        public string projectopty { get; set; }
        public string quickvalue { get; set; }
        public string status { get; set; }
        public string signstatus { get; set; }
        //public DateTime deadline { get; set; }
        public int deadlineyear { get; set; }
        public int deadlinemonth { get; set; }
        public int deadlineday { get; set; }
        public string note { get; set; }
    }

}
