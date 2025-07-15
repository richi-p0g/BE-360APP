
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
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

        public async Task<allmasters> allMasters()
        {
            try
            {
                var f = new List<finalValue>();
                var q = new List<quickValue>();
                var s = new List<status>();
                var ss = new List<signstatus>();
                var mst = new allmasters();
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();
                    MySqlCommand cmdA = new MySqlCommand($" Select value, Description from finalvalue ", connection);
                    MySqlCommand cmdB = new MySqlCommand($" Select value, Description from quickvalue ", connection);
                    MySqlCommand cmdC = new MySqlCommand($" Select Description from status ", connection);
                    MySqlCommand cmdD = new MySqlCommand($" Select Description from signstatus ", connection);

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

                    using (var reader = await cmdC.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            s.Add(new status
                            {
                                statusvalue = (string)reader["Description"],
                            });
                        }
                    }

                    using (var reader = await cmdD.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            ss.Add(new signstatus
                            {
                                signstatusvalue = (string)reader["Description"],
                            });
                        }
                    }
                    mst.finals.AddRange(f);
                    mst.quicks.AddRange(q);
                    mst.status.AddRange(s);
                    mst.signstatus.AddRange(ss);
                    connection.Close();

                    return mst;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<projectOpty>> masterProjectopties()
        {
            try
            {
                var mst = new List<projectOpty>();
                using (var connection = new MySqlConnection(_config.GetConnectionString("ppcConn")))
                {
                    connection.Open();
                    MySqlCommand cmdA = new MySqlCommand($" Select idOpti, namaOpti, tglBuat from opti where tglBuat >= date_sub(now(), interval 8 month) order by tglBuat desc ", connection);
                    using (var reader = await cmdA.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            mst.Add(new projectOpty
                            {
                                Number = (string)reader["idOpti"],
                                Desc = (string)reader["namaOpti"]
                            });
                        }
                    }

                    connection.Close();

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
                var deadline = dto.deadline == null ? "1900-01-01" : dto.deadline;
                bool userisValid = false;
                bool optyisValid = false;

                using (var connppc = new MySqlConnection(_config.GetConnectionString("ppcConn")))
                {
                    connppc.Open();
                    MySqlCommand cmdB = new MySqlCommand($" Select idOpti from opti where idOpti = '{dto.projectopty}' order by tglBuat desc ", connppc);
                    using (var reader = await cmdB.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            optyisValid = true;
                        }
                    }
                    connppc.Close();
                }

                using (var connection = new SqlConnection(_config.GetConnectionString("x3dbConn"))) //validate username & fullname
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand($" SPSagex3dbfor360apps 2, '{dto.usernameAssign}', '{dto.fullnameAssign}' ", connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            userisValid = true;
                        }
                    }
                    connection.Close();
                }


                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();

                    if (optyisValid && userisValid)
                    {
                        MySqlCommand command = new MySqlCommand($" INSERT INTO tasks " +
                            $"(usernameAssign , fullnameAssign, usernameFrom, projectopty, quickvalue, status, signstatus, deadline, note, createDate, createAt)" +
                            $"VALUES('{dto.usernameAssign}', '{dto.fullnameAssign}', '{dto.usernameFrom}', '{dto.projectopty}', {dto.quickvalue}, '{dto.status}', '{dto.signsatus}', '{deadline}', '{dto.note}', '{DateTime.Now}', '{dto.usernameFrom}')", connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            rspn.Msg = $"Tugas untuk {dto.fullnameAssign} berhasil di buat";
                        }
                    }
                    connection.Close();
                }

                if (!userisValid)
                {
                    rspn.isError = true;
                    rspn.Msg = $" Nama {dto.fullnameAssign} atau Username {dto.usernameAssign}, tidak valid";
                }
                else if(!optyisValid)
                {
                    rspn.isError = true;
                    rspn.Msg = $" Project/Opty {dto.projectopty}, tidak valid";
                } 

                return rspn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AssignToOtherGetAll>> AssignToOtherGetAlls(AssignToOtherGetAllDto dto)
        {
            try
            {
                var mst = new List<AssignToOtherGetAll>();
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();
                    MySqlCommand cmdA = new MySqlCommand($" select id, fullnameAssign, usernameAssign, projectopty, quickvalue, status, signstatus, deadline, note from tasks where usernameFrom = '{dto.usernameFrom}' ", connection);
                    using (var reader = await cmdA.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            mst.Add(new AssignToOtherGetAll
                            {
                                RowId = Convert.ToString((int)reader["id"]),
                                fullnameAssign = (string)reader["fullnameAssign"] +"|" + (string)reader["usernameAssign"],
                                usernameAssign = (string)reader["usernameAssign"],
                                projectopty = (string)reader["projectopty"],
                                quickvalue = Convert.ToString((int)reader["quickvalue"]),
                                status = (string)reader["status"],
                                signstatus = (string)reader["signstatus"],
                                deadlineday = Convert.ToDateTime((DateTime)reader["deadline"]).Date.Day,
                                deadlinemonth = Convert.ToDateTime((DateTime)reader["deadline"]).Date.Month,
                                deadlineyear = Convert.ToDateTime((DateTime)reader["deadline"]).Date.Year,
                                note = (string)reader["note"],
                            });
                        }
                    }

                    connection.Close();

                    return mst;
                }
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
                using (var connection = new SqlConnection(_config.GetConnectionString("x3dbConn")))
                {
                    connection.Open();
                    
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
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
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



        /*
        public async Task<List<Dashboard>> Dashboards(whereString ws)
        {
            try
            {
                List<Dashboard> dtsales = new List<Dashboard>();
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
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
        }*/

    }
}
