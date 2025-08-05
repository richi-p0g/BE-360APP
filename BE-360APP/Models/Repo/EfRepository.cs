
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Mysqlx.Crud;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
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
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbmobileConn")))
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
                    connection.Close();
                }

                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand($"  select u.id, u.username, u.password, u.namaKaryawan ,l.namaLevel, l.id, t.namaTemplate, u.nip, if(isnull(hbawahan.id)=1, false, true) EmpJunior from user360 u    join level l on l.id = u.idLevel     join template t on u.idTemplate = t.id     left join hierarki hbawahan on hbawahan.usernameAtasan = u.username   where u.username = '{dto.usernameFrom}' group by u.namaKaryawan desc  ", connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            rspn.id = (int)reader["id"]; //
                            rspn.usernameFrom = (string)reader["username"] ?? string.Empty; //
                            rspn.empJunior = (int)reader["EmpJunior"] == 1 ? true :  false; //
                            rspn.Password = (string)reader["password"] ?? string.Empty;
                            rspn.nip = (string)reader["nip"] ?? string.Empty;
                            rspn.fullName = (string)reader["namaKaryawan"] ?? string.Empty; //
                            rspn.position = (string)reader["namaLevel"] ?? string.Empty; //
                            rspn.templateName = (string)reader["namaTemplate"] ?? string.Empty; //
                        }
                    }
                    connection.Close();
                }

                if (string.IsNullOrEmpty(rspn.usernameFrom))
                {
                    rspn.isError = true;
                    rspn.Msg = $"Username {dto.usernameFrom} tidak ditemukan";
                    return rspn;
                }
                else if (rspn.nip.ToLower() != dto.Password.ToLower()) //password pakai nip
                {
                    rspn.isError = true;
                    rspn.Msg = $"Password tidak sesuai";
                    return rspn;
                }
            
                rspn.Msg = $" Welcome {rspn.usernameFrom}, Login " +rspn.Msg;
                    
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
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbmobileConn")))
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


        public async Task<Response> AssignToOtherRowUpdates(AssignToOtherDto dto)
        {
            try
            {
                var rspn = new Response();
                var deadline = dto.deadline == null ? "1900-01-01" : dto.deadline;
                bool optyisValid = false;

                if (!String.IsNullOrEmpty(dto.projectopty))
                {
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
                }

                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();

                    if (optyisValid || String.IsNullOrEmpty(dto.projectopty))
                    {
                        MySqlCommand command = new MySqlCommand($" UPDATE tasks " +
                            $" SET usernameFrom='{dto.usernameFrom}', projectopty='{dto.projectopty}', quickvalue={dto.quickvalue}, signstatus='{dto.signsatus}', deadline='{dto.deadline}', note='{dto.note}', updateDate='{DateTime.Now}', createAt='{dto.usernameFrom}'" +
                            $" WHERE id = {dto.id} ", connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            rspn.Msg = $"Tugas untuk {dto.fullnameAssign} berhasil di buat";
                        }
                    }
                    connection.Close();
                }

                if (!optyisValid && !String.IsNullOrEmpty(dto.projectopty))
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

        public async Task<Response> AssignToOtherRowSaves(AssignToOtherDto dto)
        {
            try
            {
                var rspn = new Response();
                var deadline = dto.deadline == null ? "1900-01-01" : dto.deadline;
                bool userisValid = false;
                bool optyisValid = false;

                if (!String.IsNullOrEmpty(dto.projectopty))
                {
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

                    if (userisValid && (optyisValid || String.IsNullOrEmpty(dto.projectopty)))
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
                else if(!optyisValid && !String.IsNullOrEmpty(dto.projectopty))
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

        public async Task<List<responReviewgetall>> responReviewgetalls(formFastRateDto dto)
        {
            try
            {
                List<responReviewgetall> d = new List<responReviewgetall>();
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();

                    //Layer 1
                    MySqlCommand cmd = new MySqlCommand($" select u.id, u.namaKaryawan, r.waktuRequest from requestreview r     join user360 u on u.username  = r.usernameYangRequest       where r.usernameYangMemberi = '{dto.usernameFrom}' and r.status = 'Belum Terisi' ", connection);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            d.Add(new responReviewgetall
                            {
                                id = (int)reader["id"],
                                namaKaryawan = (string)reader["namaKaryawan"],
                                timerequest = (DateTime)reader["waktuRequest"],
                            });
                        }
                    }

                    connection.Close();


                }
                return d;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<List<juniorEmployee>> personJuniors(formFastRateDto dto)
        {
            try
            {
                List<juniorEmployee> person = new List<juniorEmployee>();
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();

                    //Layer 1
                    MySqlCommand cmd = new MySqlCommand($" select u4.id, u4.namaKaryawan from question q  join template t on q.idTemplate = t.id  join user360 u on t.id = u.idTemplate  join peer p on p.usernamePertama = u.username  join hierarki hbawahan on hbawahan.usernameAtasan = u.username  join divisi d on d.id = u.idDivisi  join level l on l.id = u.idLevel  join user360 u4 on u4.username = hbawahan.usernameBawahan  where u.username = '{dto.usernameFrom}'  and u4.id not in ( select u.id from histreviewquestion h  join user360 u on u.username = h.usernameYgDireview  where quarter(NOW()) = quarter(waktuIsi) and h.usernameYgDireview  = u.username  ) group by u4.namaKaryawan asc ", connection);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            person.Add(new juniorEmployee
                            {
                                id = (int)reader["id"],
                                namaKaryawan = (string)reader["namaKaryawan"]
                            });
                        }
                    }

                    connection.Close();


                }
                    return person;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Response> GivenFeedbackonSubmits(personLayersubmitDto dto)
        {
            try
            {
                var rspn = new Response();
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            if (dto.selectedPersonLayer.Count() > 0)
                            {
                                foreach (var pLayer in dto.selectedPersonLayer)
                                {
                                     MySqlCommand cmd = new MySqlCommand();
                                     cmd = new MySqlCommand($" INSERT INTO requestreview (usernameYangRequest, usernameYangMemberi, waktuRequest) VALUES ((select username from user360 where id = {dto.id}), (select username from user360 where id = {pLayer.id}), NOW()) ", connection);
                                     cmd.Transaction = tran;
                                     await cmd.ExecuteNonQueryAsync();
                                }
                            }

                            if (dto.selectedEmployee.Count() > 0)
                            {
                                foreach (var emp in dto.selectedEmployee)
                                {
                                    
                                    MySqlCommand command = new MySqlCommand();
                                    command = new MySqlCommand($" INSERT INTO requestreview (usernameYangRequest, usernameYangMemberi, waktuRequest) VALUES ((select username from user360 where id = {dto.id}), (select username from user360 where id = {emp.id}), NOW())  ", connection);
                                    command.Transaction = tran;
                                    await command.ExecuteNonQueryAsync();
                                }
                                rspn.Msg = $"Feedback submitted successfully!";
                            }

                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw new Exception(ex.Message);
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


        public async Task<List<AssignToMeGetAll>> AssignToMeGetAlls(AssignToMeGetAllDto dto)
        {
            try
            {
                var mst = new List<AssignToMeGetAll>();

                return mst;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<structurformFastRating> formFastRatenextReviews(formFastRateDto dto)
        {
            try
            {
                var form = new structurformFastRating();

                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();

                    //2 id nama karyawan review tambahan
                    var karyawan = new List<dataKaryawan>();
                    MySqlCommand cmdkar = new MySqlCommand($" select u.id, u.namaKaryawan, d.nama, l.namaLevel, case when u.rating = 0 then '0' when u.rating = 1 then '1' when u.rating = 2 then '2' when u.rating = 3 then '3' when u.rating = 4 then '4' when u.rating = 5 then '5' end rating from user360 u  join divisi d on d.id = u.idDivisi  join level l on l.id = u.idLevel join question q on q.idTemplate = u.idTemplate where u.id in ('{dto.idname1}','{dto.idname2}') group by u.id, u.namaKaryawan, d.nama, l.namaLevel, u.rating desc ", connection);
                    using (var reader = await cmdkar.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            karyawan.Add(new dataKaryawan
                            {
                                namaLevel = (string)reader["namaLevel"],
                                namaKaryawan = (string)reader["namaKaryawan"],
                                nama = (string)reader["nama"],
                                rating = (string)reader["rating"],
                                idUser = (int)reader["id"]
                            });
                        }
                    }


                    //question
                    var question = new List<question>();
                    MySqlCommand cmdquest = new MySqlCommand($" select u.id, q.question from question q  join template t on q.idTemplate = t.id  join user360 u on t.id = u.idTemplate  where u.id in ('{dto.idname1}','{dto.idname2}')  ", connection);
                    using (var reader = await cmdquest.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            question.Add(new question
                            {
                                questions = (string)reader["question"],
                                idUser = (int)reader["id"]
                            });
                        }
                    }

                    connection.Close();


                    var data = new getData();
                    form.data = new List<getData>();
                    data.questions = new List<questionsData>();
                    var qq = new List<questionsData>();

                    //form structure
                    form.status = 1;
                    form.fullname = karyawan[0].namaKaryawan;
                    form.divlevel = karyawan[1].namaKaryawan;
                    foreach (var nama in karyawan)
                    {
                        int i = 1;
                        foreach (var q in question.Where(w=>w.idUser == nama.idUser))
                        {
                            qq.Add(new questionsData()
                            {
                                question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                _id = Guid.NewGuid().ToString().Replace("-", ""),
                                title = nama.namaKaryawan+ " | Divisi: " + nama.nama.ToUpper() + " | Jabatan: " + nama.namaLevel.ToUpper() + " | Rating: " + nama.rating,
                                description = i.ToString() + ". " + q.questions.ToString(),
                                remark = true,
                                type = "dropdown",
                                is_mandatory = true,
                                fields = new List<string> { "1", "2", "3", "4", "5" }
                            });
                            i++;
                        }

                        qq.Add(new questionsData()
                        {
                            question_id = Guid.NewGuid().ToString().Replace("-", ""),
                            _id = Guid.NewGuid().ToString().Replace("-", ""),
                            title = "",
                            description = i.ToString() + ". Menurut Anda apa yang perlu ditingkatkan/ diperbaiki oleh Bapak/Ibu " + nama.namaKaryawan,
                            maxline = 4,
                            remark = false,
                            type = "text",
                            is_mandatory = true,
                            fields = ["450"]
                        });
                    }
                    
                    data.questions.AddRange(qq);
                    form.data.Add(data);

                    return form;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<structurformFastRating> formFastRatesresponReviewnextForm(formFastRateDto dto)
        {
            try
            {
                var form = new structurformFastRating();
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();

                    var pilihkar = new List<string>();


                    //data karyawan
                    var karyawan = new dataKaryawan();
                    MySqlCommand cmdkar = new MySqlCommand($" select  u.namaKaryawan, d.nama, l.namaLevel, case when u.rating = 0 then '0' when u.rating = 1 then '1' when u.rating = 2 then '2' when u.rating = 3 then '3' when u.rating = 4 then '4' when u.rating = 5 then '5' end rating from user360 u  join divisi d on d.id = u.idDivisi  join level l on l.id = u.idLevel where u.username = '{dto.usernameFrom}' ", connection);
                    using (var reader = await cmdkar.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            karyawan.namaLevel = (string)reader["namaLevel"];
                            karyawan.namaKaryawan = (string)reader["namaKaryawan"];
                            karyawan.nama = (string)reader["nama"];
                            karyawan.rating = (string)reader["rating"];
                        }
                    }


                    //question
                    var question = new List<question>();
                    var query = $" select q.id, q.question, u.id idUser from question q  join template t on q.idTemplate = t.id  join user360 u on t.id = u.idTemplate  where u.id in ( ";
                    query += string.Join(",", dto.selectedPersonJuniors.Select(emp => emp.id));
                    query += $")";

                    MySqlCommand cmdquest = new MySqlCommand(query, connection);
                    using (var reader = await cmdquest.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            question.Add(new question
                            {
                                id = (int)reader["id"],
                                idUser = (int)reader["idUser"],
                                questions = (string)reader["question"],
                            });
                        }
                    }
                    

                    //general questions rating
                    var genQuestRating = new List<genQuestionRate>();
                    MySqlCommand cmdQuestA = new MySqlCommand($" select id, t.question from tempateratingquestiongeneral t  order by t.question asc ", connection);
                    using (var reader = await cmdQuestA.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            genQuestRating.Add(new genQuestionRate
                            {
                                id = (Int16)reader["id"],
                                questions = (string)reader["question"]
                            });
                        }
                    }

                    //general questions text
                    var genQuestTxt = new List<genQuestionText>();
                    MySqlCommand cmdQuestB = new MySqlCommand($" select id, t.question from templatequestiongeneral t  order by t.question asc ", connection);
                    using (var reader = await cmdQuestB.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            genQuestTxt.Add(new genQuestionText
                            {
                                id = (Int16)reader["id"],
                                questions = (string)reader["question"]
                            });
                        }
                    }


                    //upline new
                    var upline = new List<upline>();
                    var query2 = $"  select u4.id, u4.namaKaryawan  from user360 u join hierarki hbawahan on hbawahan.usernameAtasan = u.username  join user360 u4 on u4.username = hbawahan.usernameBawahan       where u4.id in ( ";
                    query2 += string.Join(",", dto.selectedPersonJuniors.Select(emp => emp.id));
                    query2 += $" ) group by u4.namaKaryawan desc";

                    MySqlCommand cmdupline = new MySqlCommand(query2, connection);
                    using (var reader = await cmdupline.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            upline.Add(new upline
                            {
                                id = (int)reader["id"],
                                namaKaryawan = (string)reader["namaKaryawan"]
                            });
                        }
                    }

                    connection.Close();


                    var data = new getData();
                    form.data = new List<getData>();
                    data.questions = new List<questionsData>();
                    var qq = new List<questionsData>();


                    //form structure
                    form.status = 1;
                    form.fullname = karyawan.namaKaryawan;
                    form.divlevel = "Divisi: " + karyawan.nama.ToUpper() + " | Jabatan: " + karyawan.namaLevel.ToUpper() + " | Rating: " + karyawan.rating;

                    foreach (var sub in upline)
                    {
                        int i = 1;
                        foreach (var q in question.Where(w => w.idUser == sub.id))
                        {
                            qq.Add(new questionsData()
                            {
                                question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                _id = sub.id.ToString() + "|" + q.id.ToString(),
                                title = sub.namaKaryawan,
                                description = i.ToString() + ". " + q.questions.ToString(),
                                remark = true,
                                type = "dropdown",
                                is_mandatory = true,
                                fields = new List<string> { "1", "2", "3", "4", "5" }
                            });
                            i++;
                        }

                        int r = 1;
                        foreach (var qrate in genQuestRating) //Question Rating
                        {
                            qq.Add(new questionsData()
                            {
                                question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                _id = sub.id.ToString() + "|" + qrate.id.ToString(),
                                title = sub.namaKaryawan,
                                description = r.ToString() + ". " + qrate.questions.ToString(),
                                remark = false,
                                type = "dropdown",
                                is_mandatory = true,
                                fields = new List<string> { "1", "2", "3", "4", "5" }
                            });
                            r++;
                        }


                        int t = 1;
                        foreach (var qtxt in genQuestTxt) //Question Text
                        {
                            qq.Add(new questionsData()
                            {
                                question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                _id = sub.id.ToString() + "|" + qtxt.id.ToString(),
                                title = sub.namaKaryawan,
                                description = t.ToString() + ". " + qtxt.questions.ToString(),
                                maxline = 4,
                                remark = false,
                                type = "text",
                                is_mandatory = true,
                                fields = ["450"]
                            });
                            t++;
                        }
                    }
                    data.questions.AddRange(qq);
                    form.data.Add(data);

                    return form;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<structurformFastRating> formFastRates(formFastRateDto dto)
        {
            try
            {
                var form = new structurformFastRating();
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();

                    var pilihkar = new List<string>();
                    if (dto.selectedPersonJuniors.Count() <= 0)
                    {
                        //pilih karyawan review tambahan
                        MySqlCommand cmdpilkar = new MySqlCommand($" select u.id, u.namaKaryawan from user360 u  join divisi d on d.id = u.idDivisi  join level l on l.id = u.idLevel order by u.namaKaryawan asc ", connection);
                        using (var reader = await cmdpilkar.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                pilihkar.Add((string)reader["namaKaryawan"] + "|" + Convert.ToString((int)reader["id"]));
                            }
                        }
                    }


                    //data karyawan
                    var karyawan = new dataKaryawan();
                    MySqlCommand cmdkar = new MySqlCommand($" select  u.namaKaryawan, d.nama, l.namaLevel, case when u.rating = 0 then '0' when u.rating = 1 then '1' when u.rating = 2 then '2' when u.rating = 3 then '3' when u.rating = 4 then '4' when u.rating = 5 then '5' end rating from user360 u  join divisi d on d.id = u.idDivisi  join level l on l.id = u.idLevel where u.username = '{dto.usernameFrom}' ", connection);
                    using (var reader = await cmdkar.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            karyawan.namaLevel = (string)reader["namaLevel"];
                            karyawan.namaKaryawan = (string)reader["namaKaryawan"];
                            karyawan.nama = (string)reader["nama"];
                            karyawan.rating = (string)reader["rating"];
                        }
                    }


                    //question
                    var question = new List<question>();
                    MySqlCommand cmdquest = new MySqlCommand($" select q.id, q.question from question q  join template t on q.idTemplate = t.id  join user360 u on t.id = u.idTemplate  where u.username = '{dto.usernameFrom}' ", connection);
                    using (var reader = await cmdquest.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            question.Add(new question
                            {
                                id = (int)reader["id"],
                                questions = (string)reader["question"]
                            });
                        }
                    }


                    //general questions rating
                    var genQuestRating = new List<genQuestionRate>();
                    MySqlCommand cmdQuestA = new MySqlCommand($"  select id, t.question from tempateratingquestiongeneral t  order by t.question asc ", connection);
                    using (var reader = await cmdQuestA.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            genQuestRating.Add(new genQuestionRate
                            {
                                id = (Int16)reader["id"],
                                questions = (string)reader["question"]
                            });
                        }
                    }

                    //general questions text
                    var genQuestTxt = new List<genQuestionText>();
                    MySqlCommand cmdQuestB = new MySqlCommand($" select id, t.question from templatequestiongeneral t  order by t.question asc ", connection);
                    using (var reader = await cmdQuestB.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            genQuestTxt.Add(new genQuestionText
                            {
                                id = (Int16)reader["id"],
                                questions = (string)reader["question"]
                            });
                        }
                    }


                    var upline = new List<upline>();
                    var p = new List<peer>();
                    if (dto.selectedPersonJuniors.Count() <= 0)
                    {
                        //upline
                        MySqlCommand cmdupline = new MySqlCommand($"  select u2.id, u2.namaKaryawan upline from question q  join template t on q.idTemplate = t.id  join user360 u on t.id = u.idTemplate  join hierarki hatasan on hatasan.usernameBawahan = u.username  join divisi d on d.id = u.idDivisi  join level l on l.id = u.idLevel   join user360 u2 on u2.username = hatasan.usernameAtasan where u.username = '{dto.usernameFrom}' group by u2.namaKaryawan desc  ", connection);
                        using (var reader = await cmdupline.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                upline.Add(new upline
                                {
                                    id = (int)reader["id"],
                                    namaKaryawan = (string)reader["upline"]
                                });
                            }
                        }

                        //peer
                        MySqlCommand cmdpeer = new MySqlCommand($"  select u3.id, u3.namaKaryawan peer from question q  join template t on q.idTemplate = t.id  join user360 u on t.id = u.idTemplate  join peer p on p.usernamePertama = u.username  join divisi d on d.id = u.idDivisi  join level l on l.id = u.idLevel  join user360 u3 on u3.username = p.usernameKedua  where u.username = '{dto.usernameFrom}'  group by u3.namaKaryawan desc  ", connection);
                        using (var reader = await cmdpeer.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                p.Add(new peer
                                {
                                    id = (int)reader["id"],
                                    namaKaryawan = (string)reader["peer"]
                                });
                            }
                        }
                    }

                    //subordinate karyawan junior 1 layer
                    var sb = new List<subordinate>();
                    MySqlCommand cmdsubordinate = new MySqlCommand($" select u4.id, u4.namaKaryawan sobordinate from question q  join template t on q.idTemplate = t.id  join user360 u on t.id = u.idTemplate  join peer p on p.usernamePertama = u.username  join hierarki hbawahan on hbawahan.usernameAtasan = u.username  join divisi d on d.id = u.idDivisi  join level l on l.id = u.idLevel  join user360 u4 on u4.username = hbawahan.usernameBawahan  where u.username = '{dto.usernameFrom}' group by u4.namaKaryawan desc ", connection);
                    using (var reader = await cmdsubordinate.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            sb.Add(new subordinate
                            {
                                id = (int)reader["id"],
                                namaKaryawan = (string)reader["sobordinate"]
                            });
                        }
                    }

                    if(dto.selectedPersonJuniors.Count() > 0 )
                    {
                        sb = sb.Where(w => dto.selectedPersonJuniors.Any(s => int.Parse(s.id) == w.id)).ToList();
                    }

                    connection.Close();


                    var data = new getData();
                    form.data = new List<getData>();
                    data.questions = new List<questionsData>();
                    var qq = new List<questionsData>();


                    //form structure
                    form.status = 1;
                    form.fullname = karyawan.namaKaryawan;
                    form.divlevel = "Divisi: " + karyawan.nama.ToUpper() + " | Jabatan: " + karyawan.namaLevel.ToUpper() + " | Rating: " + karyawan.rating;

                    if (dto.selectedPersonJuniors.Count() <= 0)
                    {
                        foreach (var upl in upline) //upline
                        {
                            int i = 1;
                            foreach (var q in question)
                            {
                                qq.Add(new questionsData()
                                {
                                    question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                    _id = upl.id.ToString() + "|" + q.id.ToString(),
                                    title = upl.namaKaryawan,
                                    description = i.ToString() + ". " + q.questions.ToString(),
                                    remark = true,
                                    type = "dropdown",
                                    is_mandatory = true,
                                    fields = new List<string> { "1", "2", "3", "4", "5" }
                                });
                                i++;
                            }
                            qq.Add(new questionsData()
                            {
                                question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                _id = upl.id + "|" + 0,
                                title = upl.namaKaryawan,
                                description = i.ToString() + ". Menurut Anda apa yang perlu ditingkatkan/ diperbaiki oleh Bapak/Ibu " + upl.namaKaryawan,
                                maxline = 4,
                                remark = false,
                                type = "text",
                                is_mandatory = true,
                                fields = ["450"]
                            });
                        }

                        foreach (var peer in p) //peer
                        {
                            int i = 1;
                            foreach (var q in question)
                            {
                                qq.Add(new questionsData()
                                {
                                    question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                    _id = peer.id.ToString() + "|" + q.id.ToString(),
                                    title = peer.namaKaryawan,
                                    description = i.ToString() + ". " + q.questions.ToString(),
                                    remark = true,
                                    type = "dropdown",
                                    is_mandatory = true,
                                    fields = new List<string> { "1", "2", "3", "4", "5" }
                                });
                                i++;
                            }
                            qq.Add(new questionsData()
                            {
                                question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                _id = peer.id.ToString() + "|" + "0",
                                title = peer.namaKaryawan,
                                description = i.ToString() + ". Menurut Anda apa yang perlu ditingkatkan/ diperbaiki oleh Bapak/Ibu " + peer.namaKaryawan,
                                maxline = 4,
                                remark = false,
                                type = "text",
                                is_mandatory = true,
                                fields = ["450"]
                            });
                        }
                    }

                    foreach (var sub in sb) //subordinate
                    {
                        int i = 1;
                        foreach (var q in question)
                        {
                            qq.Add(new questionsData()
                            {
                                question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                _id = sub.id.ToString() + "|" + q.id.ToString(),
                                title = sub.namaKaryawan,
                                description = i.ToString() + ". " + q.questions.ToString(),
                                remark = true,
                                type = "dropdown",
                                is_mandatory = true,
                                fields = new List<string> { "1", "2", "3", "4", "5" }
                            });
                            i++;
                        }

                        int r = 1;
                        foreach (var qrate in genQuestRating) //Question Rating
                        {
                            qq.Add(new questionsData()
                            {
                                question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                _id = sub.id.ToString() + "|" + qrate.id.ToString(),
                                title = sub.namaKaryawan,
                                description = r.ToString() + ". " + qrate.questions.ToString(),
                                remark = false,
                                type = "dropdown",
                                is_mandatory = true,
                                fields = new List<string> { "1", "2", "3", "4", "5" }
                            });
                            r++;
                        }


                        int t = 1;
                        foreach (var qtxt in genQuestTxt) //Question Text
                        {
                            qq.Add(new questionsData()
                            {
                                question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                _id = sub.id.ToString() + "|" + qtxt.id.ToString(),
                                title = sub.namaKaryawan,
                                description = t.ToString() + ". " + qtxt.questions.ToString(),
                                maxline = 4,
                                remark = false,
                                type = "text",
                                is_mandatory = true,
                                fields = ["450"]
                            });
                            t++;
                        }

                        if (dto.selectedPersonJuniors.Count() <= 0)
                        {
                            qq.Add(new questionsData()
                            {
                                question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                _id = sub.id.ToString() + "|" + "0",
                                title = sub.namaKaryawan,
                                description = i.ToString() + ". Menurut Anda apa yang perlu ditingkatkan/ diperbaiki oleh Bapak/Ibu " + sub.namaKaryawan,
                                maxline = 4,
                                remark = false,
                                type = "text",
                                is_mandatory = true,
                                fields = ["450"]
                            });
                        }
                    }

                    if (dto.selectedPersonJuniors.Count() <= 0)
                    {
                        //Review tambahan 2 karyawan
                        for (int i = 1; i < 3; i++)
                        {
                            qq.Add(new questionsData()
                            {
                                question_id = Guid.NewGuid().ToString().Replace("-", ""),
                                _id = "0",
                                title = i == 1 ? "Review Tambahan, Silakan Anda pilih maksimal 2 orang di luar team Anda yang sering berkoordinasi atau terlibat pekerjaan bersama dan berikan penilaian!" : "",
                                description = "Pilih Nama ke-" + i.ToString(),
                                remark = false,
                                type = "dropdown",
                                is_mandatory = true,
                                fields = pilihkar
                            });
                        }
                    }
                    data.questions.AddRange(qq);
                    form.data.Add(data);
                    
                    return form;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Allperson> personLayers(formFastRateDto dto)
        {
            try
            {
                Allperson person = new Allperson();
                person.personlayer2 = new List<subordinate>();
                person.allKaryawan = new List<allKaryawan>();

                fullnameAndid mst = new fullnameAndid();
                var form = new structurformFastRating();

                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();

                    //Layer 2 etc
                    var sb2 = new List<subordinate>();
                    MySqlCommand cmd = new MySqlCommand($" select u5.id, u5.namaKaryawan sobordinate from question q   join template t on q.idTemplate = t.id   join user360 u on t.id = u.idTemplate   join hierarki hbawahan on hbawahan.usernameAtasan = u.username join user360 u4 on u4.username = hbawahan.usernameBawahan  join hierarki hbawahan2 on hbawahan2.usernameAtasan = u4.username join user360 u5 on u5.username = hbawahan2.usernameBawahan  where u.username = '{dto.usernameFrom}' and u5.id not in (select u.id from histreviewquestion h  join user360 u on u.username = h.usernameYgDireview  where quarter(NOW()) = quarter(waktuIsi) and h.usernamePembuat = u.username  ) group by u5.namaKaryawan asc   ", connection);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            sb2.Add(new subordinate
                            {
                                id = (int)reader["id"],
                                namaKaryawan = (string)reader["sobordinate"]
                            });
                        }
                    }


                    //Layer 1 subordinate
                    var sb = new List<subordinate>();
                    MySqlCommand cmdsubordinate = new MySqlCommand($" select u4.id, u4.namaKaryawan sobordinate from question q  join template t on q.idTemplate = t.id  join user360 u on t.id = u.idTemplate  join peer p on p.usernamePertama = u.username  join hierarki hbawahan on hbawahan.usernameAtasan = u.username  join divisi d on d.id = u.idDivisi  join level l on l.id = u.idLevel  join user360 u4 on u4.username = hbawahan.usernameBawahan  where u.username = '{dto.usernameFrom}' and u4.id not in ( select u.id from histreviewquestion h  join user360 u on u.username = h.usernameYgDireview  where quarter(NOW()) = quarter(waktuIsi) and h.usernamePembuat = u.username  ) group by u4.namaKaryawan desc ", connection);
                    using (var reader = await cmdsubordinate.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            sb.Add(new subordinate
                            {
                                id = (int)reader["id"],
                                namaKaryawan = (string)reader["sobordinate"]
                            });
                        }
                    }

                    sb2.AddRange(sb);

                    //pilih all review karyawan
                    var allkrywn = new List<allKaryawan>();
                    var q = $" select u.id, u.namaKaryawan, d.nama division from user360 u join divisi d on d.id = u.idDivisi  join level l on l.id = u.idLevel ";

                    if (sb2.Count() > 0) q += $"where u.id not in ({string.Join(", ", sb2.Select(s => s.id))})";
                    q += "order by u.namaKaryawan asc";

                    MySqlCommand cmdpilkar = new MySqlCommand(q, connection);
                    using (var reader = await cmdpilkar.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            allkrywn.Add(new allKaryawan
                            {
                                id = (int)reader["id"],
                                namaKaryawan = (string)reader["namaKaryawan"],
                                Division = (string)reader["division"]
                            });
                        }
                    }
                    connection.Close();

                    person.personlayer2.AddRange(sb2);
                    person.allKaryawan.AddRange(allkrywn);

                    return person;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //insert HERE
        public async Task<Response> formFastRatesubmitForms(ListFastRatingRowSaveDto dto)
        {
            try
            {
                var respon = new Response();
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            // General rating header detail
                            var forms = dto.answerForm1.Where(W => W.type == "dropdown" && W.remarkdata == "null");
                            foreach (var attr in forms.GroupBy(g => g.idKaryawan))
                            {
                                var averageScore = attr.Select(s => s.answer.Split(',').Select(int.Parse).Sum()).FirstOrDefault() / attr.Select(s => s.answer.Count()).FirstOrDefault();
                                MySqlCommand cmd = new MySqlCommand($"INSERT INTO histratingquestiongeneral (usernameYgIsi, UsernameYgRequest, waktuIsi, idRequest, totalNilai) VALUES ('{dto.usernameFrom}', (select username from user360 u where u.id = {attr.Select(s => s.idKaryawan).FirstOrDefault()}), NOW(), 0, {averageScore})", connection);
                                cmd.Transaction = tran;
                                await cmd.ExecuteNonQueryAsync();

                                foreach (var attr1 in forms.Where(w => w.idKaryawan == attr.Select(s => s.idKaryawan).FirstOrDefault()))
                                {
                                    MySqlCommand cmd1 = new MySqlCommand($" INSERT INTO reviewgeneralrating (idHistReview, idQuestion, nilai) VALUES((select id from histratingquestiongeneral order by id desc limit 1), {attr1.idQuestion}, '{attr1.answer}') ", connection);
                                    cmd1.Transaction = tran;
                                    await cmd1.ExecuteNonQueryAsync();
                                }
                            }


                            // General text header detail
                            var forms1 = dto.answerForm1.Where(W => W.type == "text" && W.remarkdata == "null");
                            foreach (var attr in forms1.GroupBy(g => g.idKaryawan))
                            {
                                MySqlCommand cmd = new MySqlCommand($" INSERT INTO histfreetextquestiongeneral (usernameYgRequest, usernameYgIsi, waktuIsi, idRequest) VALUES((select username from user360 u where u.id = {attr.Select(s => s.idKaryawan).FirstOrDefault()}), '{dto.usernameFrom}', NOW(), 0); ", connection);
                                cmd.Transaction = tran;
                                await cmd.ExecuteNonQueryAsync();

                                foreach (var attr1 in forms1.Where(w => w.idKaryawan == attr.Select(s => s.idKaryawan).FirstOrDefault()))
                                {
                                    MySqlCommand cmd1 = new MySqlCommand($" INSERT INTO reviewgeneralfreetext (idHistReview, idQuestion, komentar) VALUES((select id from histfreetextquestiongeneral order by id desc limit 1), {attr1.idQuestion}, '{attr1.answer}') ", connection);
                                    cmd1.Transaction = tran;
                                    await cmd1.ExecuteNonQueryAsync();
                                }
                            }


                            // spesific question header detail
                            var forms2 = dto.answerForm1.Where(W => W.type == "dropdown" && W.remarkdata != "null");
                            foreach (var attr in forms2.GroupBy(g => g.idKaryawan))
                            {
                                var averageScore = attr.Select(s => s.answer.Split(',').Select(int.Parse).Sum()).FirstOrDefault() / attr.Select(s => s.answer.Count()).FirstOrDefault();
                                MySqlCommand cmd = new MySqlCommand($" INSERT INTO histreviewquestion (idRequest, usernamePembuat, usernameYgDireview, waktuIsi, totalNilai) VALUES(0, '{dto.usernameFrom}', (select username from user360 u where u.id = {attr.Select(s => s.idKaryawan).FirstOrDefault()}), NOW(), {averageScore}) ", connection);
                                cmd.Transaction = tran;
                                await cmd.ExecuteNonQueryAsync();


                                foreach (var attr1 in forms2.Where(w => w.idKaryawan == attr.Select(s => s.idKaryawan).FirstOrDefault()))
                                {
                                    MySqlCommand cmd1 = new MySqlCommand($" INSERT INTO reviewquestion (idHistReview, idQuestion, nilai, komentar) VALUES((select id from histreviewquestion order by id desc limit 1), {attr1.idQuestion}, '{attr1.answer}', '{attr1.remarkdata}') ", connection);
                                    cmd1.Transaction = tran;
                                    await cmd1.ExecuteNonQueryAsync();
                                }
                            }


                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw new Exception(ex.Message);
                        }
                    }
                    connection.Close();
                    return respon;
                }
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
                    MySqlCommand cmdA = new MySqlCommand($" select id, fullnameAssign, usernameAssign, projectopty, quickvalue, status, signstatus, deadline, note from tasks where usernameFrom = '{dto.usernameFrom}' and signstatus <> 'Reject' ", connection);
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

        public async Task<List<fullnameAndid>> listFastRatings(string username)
        {
            try
            {
                var mst = new List<fullnameAndid>();
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();
                    MySqlCommand cmdA = new MySqlCommand($" select p.id idpengajuantask, u.namaKaryawan, u.id iduser, case when luser.nilaiLevel < ltujuan.nilaiLevel then 'Subordinate' when luser.nilaiLevel > ltujuan.nilaiLevel then 'Upline' when luser.nilaiLevel = ltujuan.nilaiLevel then 'Peer' end Ratee from pengajuantask p join user u on p.usernameTujuan = u.username join level luser on luser.id = u.idLevel join user utujuan on p.usernameYangMengajukan = utujuan.username join level ltujuan on ltujuan.id = utujuan.idLevel where p.usernameYangMengajukan = 'sugi' ", connection);
                    using (var reader = await cmdA.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            mst.Add(new fullnameAndid
                            {
                                iduser = Convert.ToString((int)reader["iduser"]),
                                idpengajuantask = Convert.ToString((int)reader["idpengajuantask"]),
                                fullname = (string)reader["namaKaryawan"],
                                ratee = (string)reader["Ratee"],
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

        public async Task<List<fullnameAndid>> nameAssigns(int id, string fullname)
        {
            try
            {
                var mst = new List<fullnameAndid>();
                using (var connection = new MySqlConnection(_config.GetConnectionString("360dbConn")))
                {
                    connection.Open();
                    MySqlCommand cmdA = new MySqlCommand($" select id, namaKaryawan from user where namaKaryawan <> '' limit 6 ", connection);
                    using (var reader = await cmdA.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            mst.Add(new fullnameAndid
                            {
                                iduser = Convert.ToString((int)reader["id"]),
                                fullname = (string)reader["namaKaryawan"],
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
