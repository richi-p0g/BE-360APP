
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace BE_360APP.Models.Repo
{
    public class EfRepository : IRepository
    {
        private readonly IConfiguration _config;
        public EfRepository(IConfiguration configuration) => _config = configuration;


        public async Task<Response> Logins(loginDto dto)
        {
            try
            {
                var rspn = new Response();
                using (var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    var dt = new loginDto();
                    MySqlCommand command = new MySqlCommand($" select usernamePekerja as Username from user_task where usernamePekerja = '{dto.usernamePekerja}' ", connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            dt.usernamePekerja = (string)reader["usernamePekerja"];
                           //Password = (string)reader["Password"]
                        }
                    }

                    if (String.IsNullOrEmpty(dt.usernamePekerja)) { 
                        rspn.isError = true;
                        rspn.Msg = $"Username {dto.usernamePekerja} Not Found !!";
                        connection.Close();
                        return rspn;
                    }

                    //validasi Verifikasi Tim IT
                    if (String.IsNullOrEmpty(""))
                    {
                        rspn.isError = true;
                        rspn.Msg = $"Username {dto.usernamePekerja} belum diverifikasi, silahkan hubungi IT Administrator !!";
                        connection.Close();
                        return rspn;
                    }

                    rspn.Msg = $" Welcome {dt.usernamePekerja}, Login " +rspn.Msg;

                    connection.Close();
                }
                return rspn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<Registerfullname>> LookupFullNames(usernameDto dto)
        {
            try
            {
                List<Registerfullname> dt = new List<Registerfullname>();
                using (var connection = new SqlConnection(_config.GetConnectionString("x3dbConnection")))
                {
                    connection.Open();
                    //var Param1 = dto.username == "null" ? String.Empty : ws.Param1;

                    SqlCommand command = new SqlCommand($" SPSagex3dbfor360apps 1, '{dto.fullname}' ", connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            dt.Add(new Registerfullname
                            {
                                fullname = (string)reader["NOMUSR"],
                                email = (string)reader["ADDEML"],
                                username = (string)reader["LOGIN"],
                                role = (string)reader["TEXTE"]
                            });
                        }
                    }
                    connection.Close();
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<Response> Registers(registerDto dto)
        {
            try
            {
                var rspn = new Response();
                using (var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand($" INSERT INTO XXX VALUE ({dto.fullname}, {dto.email}, xxxxx ) ", connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        rspn.Msg = $" Hi {dto.fullname}, your Password is created " + rspn.Msg;
                    }
                    connection.Close();
                }
                return rspn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }




        public async Task<List<Dashboard>> Dashboards(whereString ws)
        {
            try
            {
                List<Dashboard> dtsales = new List<Dashboard>();
                using (var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var Param1 = ws.Param1 == "null" ? String.Empty : ws.Param1;

                    MySqlCommand command = new MySqlCommand($" select NAMA as Company from divisi d ", connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            dtsales.Add(new Dashboard
                            {
                                MyProperty = (string)reader["Company"]
                            });
                        }
                    }
                    connection.Close();
                }
                return dtsales;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
