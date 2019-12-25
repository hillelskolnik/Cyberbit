using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using HillelTasksApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HillelTasksApi.Controllers
{
//    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // To Do Add as connection String
        private const string conStr = @"Data Source=.\sqlexpress; Initial Catalog=HillelsTasks; Integrated Security=True";

        // To Do Add as global static function
        private static SqlConnection GetConection()
        {
            return new SqlConnection(conStr);
        }

        [HttpGet]
        [Route("UserToken")]
        public async Task<int> UserToken(string name)
        {
            int token = -1;
            try
            {
                using (SqlConnection con = GetConection())
                {
                    con.Open();
                    string sql = $"select id from Users where name=@name";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        var res = await cmd.ExecuteScalarAsync();
                        if (res != null)
                        {
                            token = (int)res;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                
            }
            return token;
        }

        [HttpGet]
        [Route("Tasks")]
        public async Task<IEnumerable<TaskDue>> Tasks(int userToken)
        {
            List<TaskDue> tasks = new List<TaskDue>();
            try
            {
                using (SqlConnection con = GetConection())
                {
                    con.Open();
                    var sql = "Select * From Tasks inner join Users ON Users.Id=Tasks.UserId where Users.Id=@id;";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", userToken);
                        var reader = await cmd.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            IDataRecord rec = (IDataRecord)reader;
                            TaskDue task = new TaskDue
                            {
                                Id = (int)rec["Id"],
                                TaskName=rec["TaskName"].ToString(),
                                DueDate=rec.GetDateTime(2),
                                UserId = (int)rec["UserId"],
                                Done = (bool)rec["Done"]
                            };

                            tasks.Add(task);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }

            return tasks;
        }

        [HttpPost]
        [Route("InsertTasks")]

        public async Task<bool> InsertTask(int userToken, string title, DateTime dueDate)
        {
            try
            {
                using (SqlConnection con = GetConection())
                {
                    con.Open();
                    int max = 0;
                    var maxSql = "select MAX(Id) as maxId FROM Tasks;";
                    using (SqlCommand cmd = new SqlCommand(maxSql, con))
                    {
                        var res = await cmd.ExecuteScalarAsync();
                        if (res != null)
                        {
                            max = (int)res;
                        }
                    }

                    max++;

                    var sql = "insert into Tasks (Id,TaskName,DueDate,UserId,Done) values(@id,@title,@dueDate,@userId,0);";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", max);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@dueDate", dueDate);
                        cmd.Parameters.AddWithValue("@userId", userToken);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

        [HttpPost]
        [Route("SetTaskDone")]

        public async Task<bool> SetTaskDone(int id)
        {
            try
            {
                using (SqlConnection con = GetConection())
                {
                    con.Open();
                    var sql = "UPDATE Tasks SET Done=1 WHERE id=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

    }
}