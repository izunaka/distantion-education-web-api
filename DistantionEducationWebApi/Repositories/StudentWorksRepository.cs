using DistantionEducationWebApi.Models;
using DistantionEducationWebApi.Queries;
using Newtonsoft.Json;
using Npgsql;

namespace DistantionEducationWebApi.Repositories
{
    public class StudentWorksRepository
    {
        const string connectionStringName = "Default";

        private readonly StudentWorksQueries _queries;
        private readonly NpgsqlConnection _connection;

        public StudentWorksRepository(IConfiguration configurationManager, StudentWorksQueries queries) {
            var connectionString = configurationManager.GetConnectionString(connectionStringName);
            _connection = new NpgsqlConnection(connectionString);
            _queries = queries;
        }

        public List<StudentWork> GetAllWorks()
        {
            _connection.Open();
            var command = new NpgsqlCommand(_queries.GetAllStudentWorks(), _connection);
            var reader = command.ExecuteReader();

            var works = new List<StudentWork>();

            while (reader.Read())
            {
                works.Add(new StudentWork()
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2)
                });
            }
            _connection.Close();
            return works;
        }

        public StudentWork GetWorkById(Guid id)
        {
            _connection.Open();
            var command = new NpgsqlCommand(_queries.GetStudentWorkById(), _connection);

            command.Parameters.AddWithValue("id", id);

            var reader = command.ExecuteReader();

            var response = new List<dynamic>();

            while (reader.Read())
            {
                response.Add(new
                {
                    WorkId = reader.GetGuid(0),
                    WorkName = reader.GetString(1),
                    WorkDescription = reader.GetString(2),
                    TaskId = reader.GetGuid(3),
                    TaskDescription = reader.GetString(4),
                    TaskTypeId = reader.GetInt32(5),
                    TaskTypeCode = reader.GetString(6),
                    TaskTypeDescription = reader.GetString(7),
                    TaskTypeRule = reader.GetString(8),
                    AnswerId = reader.GetGuid(9),
                    AnswerDescription = reader.GetString(10),
                    IsAnswerRight = reader.GetBoolean(11)
                });
            }
            _connection.Close();
            if (response.Count == 0)
            {
                return null;
            }

            return new StudentWork()
            {
                Id = response[0].WorkId,
                Name = response[0].WorkName,
                Description = response[0].WorkDescription,
                Tasks = response.GroupBy(e => e.TaskId).Select(e => new WorkTask()
                {
                    Id = e.First().TaskId,
                    Description = e.First().TaskDescription,
                    Type = new TaskType()
                    {
                        Id = e.First().TaskTypeId,
                        Code = e.First().TaskTypeCode,
                        Description = e.First().TaskTypeDescription,
                        Rule = JsonConvert.DeserializeObject<TypeRule>(e.First().TaskTypeRule)
                    },
                    Answers = e.Select(a => new TaskAnswer()
                    {
                        Id = a.AnswerId,
                        Description = a.AnswerDescription,
                        IsRight = a.IsAnswerRight
                    })
                })
            };
        }

        public List<TaskType> GetTaskTypes()
        {
            _connection.Open();
            var command = new NpgsqlCommand(_queries.GetAllTaskTypes(), _connection);
            var reader = command.ExecuteReader();

            var result = new List<TaskType>();
            while (reader.Read())
            {
                result.Add(new TaskType()
                {
                    Id = reader.GetInt32(0),
                    Code = reader.GetString(1),
                    Description = reader.GetString(2),
                    Rule = JsonConvert.DeserializeObject<TypeRule>(reader.GetString(3))
                });
            }
            _connection.Close();
            return result;
        }

        public void CreateWork(StudentWork work)
        {
            _connection.Open();
            var command = new NpgsqlCommand(_queries.CreateStudentWork(), _connection);
            command.Parameters.AddWithValue("id", work.Id);
            command.Parameters.AddWithValue("name", work.Name);
            command.Parameters.AddWithValue("description", work.Description ?? "");
            command.ExecuteNonQuery();

            foreach (var task in work.Tasks)
            {
                command = new NpgsqlCommand(_queries.CreateWorkTask(), _connection);
                command.Parameters.AddWithValue("id", task.Id);
                command.Parameters.AddWithValue("type", task.Type.Id);
                command.Parameters.AddWithValue("description", task.Description);
                command.Parameters.AddWithValue("studentWorkId", work.Id);
                command.ExecuteNonQuery();

                foreach (var answer in task.Answers)
                {
                    command = new NpgsqlCommand(_queries.CreateTaskAnswer(), _connection);
                    command.Parameters.AddWithValue("id", answer.Id);
                    command.Parameters.AddWithValue("workTaskId", task.Id);
                    command.Parameters.AddWithValue("description", answer.Description);
                    command.Parameters.AddWithValue("isRight", answer.IsRight);
                    command.ExecuteNonQuery();
                }
            }
            _connection.Close();
        }

        public void DeleteWork(Guid workId)
        {
            _connection.Open();
            var command = new NpgsqlCommand(_queries.DeleteWork(), _connection);
            command.Parameters.AddWithValue("id", workId);
            command.ExecuteNonQuery();
            _connection.Close();
        }
    }
}
