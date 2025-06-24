namespace BE_360APP.Models.Repo
{
    public interface IRepository
    {

        Task<List<Dashboard>> Dashboards(whereString ws);
        Task<Response> Logins(loginDto ws);
        Task<List<Registerfullname>> LookupFullNames(usernameDto dto);
        Task<Response> Registers(registerDto dto);
    }
}
