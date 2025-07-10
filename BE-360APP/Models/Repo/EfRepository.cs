
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

                    MySqlCommand cmdsetAndroid = new MySqlCommand($" select * from setFrameAndroid order by createDate desc limit 1", connection);
                    using (var reader = await cmdsetAndroid.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {

                            List<int> T = new List<int>();

                            foreach (var item in (List<string>)reader["availableRowsPerPage"].ToString().Split(", ").ToList())
                            {
                                T.Add(int.Parse(item));
                            }

                            rspn.rowsPerPage = (int)reader["rowsPerPage"];
                            rspn.availableRowsPerPage = T;
                            rspn.currentVersion = (string)reader["currentVersion"];
                            rspn.linkUpdate = (string)reader["linkUpdate"];
                        }
                    }

                    MySqlCommand command = new MySqlCommand($" select * from register where username = '{dto.usernamePekerja}' ", connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            rspn.usernamePekerja = (string)reader["username"];
                            rspn.Password = (string)reader["Password"];
                            rspn.fullName = (string)reader["fullName"];
                            rspn.email = (string)reader["email"];
                            rspn.isVerify = (bool)reader["isVerify"];
                            rspn.role = (string)reader["role"];
                            rspn.id = (int)reader["id"];
                        }
                    }


                     // sementara bypass 
                    if (string.IsNullOrEmpty(rspn.usernamePekerja))
                    { 
                        rspn.isError = true;
                        rspn.Msg = $"Username {dto.usernamePekerja} tidak ditemukan";
                        connection.Close();
                        return rspn;
                    } else if (rspn.Password.ToLower() != dto.Password.ToLower())
                    {
                        rspn.isError = true;
                        rspn.Msg = $"Password tidak sesuai";
                        connection.Close();
                        return rspn;
                    }

                    // sementara bypass
                    //validasi Verifikasi Tim IT
                    if (!rspn.isVerify) 
                    {
                        rspn.isError = true;
                        rspn.Msg = $"Hi {rspn.fullName}, akun anda belum verifikasi, silahkan hubungi IT admin";
                        connection.Close();
                        return rspn;
                    }

                    rspn.Msg = $" Welcome {rspn.usernamePekerja}, Login " +rspn.Msg;
                    
                    connection.Close();
                }
                return rspn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<mastervalues> masterValues()
        {
            try
            {
                var f = new List<finalValue>();
                var q = new List<quickValue>();
                var mst = new mastervalues();
                using (var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    MySqlCommand cmdA = new MySqlCommand($" Select * from finalvalue ", connection);
                    using (var reader = await cmdA.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            f.Add(new finalValue 
                            { 
                                final = (int)reader["value"],
                                desc = (string)reader["Description"],
                            }); 
                        }
                    }

                    MySqlCommand cmdB = new MySqlCommand($" Select * from quickvalue ", connection);
                    using (var reader = await cmdB.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            q.Add(new quickValue
                            {
                                quick = (int)reader["value"],
                                desc = (string)reader["Description"],
                            });
                        }
                    }
                    connection.Close();

                    mst.finals.AddRange(f);
                    mst.quicks.AddRange(q);

                    return mst;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<Response> AssignToOtherRowSaves(AssignToOtherDto dto)
        {
            try
            {
                var rspn = new Response();
                using (var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand($" INSERT INTO tasks " +
                        $"(usernameAssign , fullnameAssign, usernameFrom, projectopty, quickvalue, status, signstatus, deadline, note, createDate, createAt)" +
                        $"VALUES('{dto.usernameAssign}', '{dto.fullnameAssign}', '{dto.usernameFrom}', '{dto.projectopty}', {dto.quickvalue}, '{dto.status}', '{dto.signsatus}', '{dto.deadline}', '{dto.note}', '{DateTime.Now}', '{dto.usernameFrom}')", connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        rspn.Msg = $"Tugas untuk {dto.fullnameAssign} berhasil di buat";
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
                                RowIdSage = Convert.ToString((decimal)reader["ROWID"]),
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
                int sumUserRegistered = 0;
                using (var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    MySqlCommand cmddataisExist = new MySqlCommand($" select id from register where username = '{dto.username}' ", connection);
                    using (var reader = await cmddataisExist.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            sumUserRegistered++;
                        }
                    }

                    if(sumUserRegistered > 0)
                    {
                        rspn.isError = true;
                        rspn.Msg = $" Hi {dto.fullname}, {sumUserRegistered} your account already exist";
                    } 
                    else
                    {
                        MySqlCommand command = new MySqlCommand($" INSERT INTO register VALUES((select (Id)+1 from register r order by id desc limit 1), '{dto.username}', '{dto.fullname}', '{dto.email}', '{dto.role}', '{dto.password}', 0, '{DateTime.Now}', '{dto.username}', '{DateTime.Now}', '{dto.username}') ", connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            rspn.Msg = $" Hi {dto.fullname}, your password is created " + rspn.Msg;
                        }
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
