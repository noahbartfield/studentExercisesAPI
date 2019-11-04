using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentExercisesAPI.Models;
using Microsoft.Extensions.Configuration;

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CohortController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CohortController(IConfiguration config)
        {
            _config = config;
        }
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.Name, s.FirstName AS StudentFirstName, s.LastName AS StudentLastName
                                      , s.SlackHandle AS StudentSlackHandle, s.CohortId AS StudentCohortId
                                      , s.Id AS StudentId, i.FirstName AS InstructorFirstName, i.LastName AS InstructorLastName
                                      , i.SlackHandle AS InstructorSlackHandle, i.CohortId AS InstructorCohortId, i.Specialty, i.Id AS InstructorId
                                        FROM Cohort c
                                        INNER JOIN Student s ON s.CohortId = c.Id
                                        INNER JOIN Instructor i ON i.CohortId = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, Cohort> cohorts = new Dictionary<int, Cohort>();

                    while (reader.Read())
                    {
                        int cohortId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!cohorts.ContainsKey(cohortId))
                        {
                            Cohort newCohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            };

                            cohorts.Add(cohortId, newCohort);

                        };
                        Cohort fromDictionary = cohorts[cohortId];

                        if (!reader.IsDBNull(reader.GetOrdinal("StudentId")))
                        {
                            Student aStudent = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("StudentCohortId"))
                            };
                            fromDictionary.Students.Add(aStudent);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("InstructorId")))
                        {
                            Instructor anInstructor = new Instructor
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                                FirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("InstructorCohortId")),
                                Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
                            };
                            fromDictionary.Instructors.Add(anInstructor);
                        }

                    }
                    reader.Close();

                    return Ok(cohorts.Values);
                }
            }
        }

        [HttpGet("{id}", Name = "GetCohort")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.Name, s.FirstName AS StudentFirstName, s.LastName AS StudentLastName
                                      , s.SlackHandle AS StudentSlackHandle, s.CohortId AS StudentCohortId
                                      , s.Id AS StudentId, i.FirstName AS InstructorFirstName, i.LastName AS InstructorLastName
                                      , i.SlackHandle AS InstructorSlackHandle, i.CohortId AS InstructorCohortId, i.Specialty, i.Id AS InstructorId
                                        FROM Cohort c
                                        INNER JOIN Student s ON s.CohortId = c.Id
                                        INNER JOIN Instructor i ON i.CohortId = c.Id
                                        Where c.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    //Dictionary<int, Cohort> cohort = new Dictionary<int, Cohort>();

                    Cohort cohort = new Cohort();

                    cohort = null;

                    while (reader.Read())
                    {
                        int cohortId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (cohort == null)
                        {
                            Cohort newCohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            };

                            cohort = newCohort;

                        };
                        //Cohort fromDictionary = cohort[cohortId];

                        if (!reader.IsDBNull(reader.GetOrdinal("StudentId")))
                        {
                            int studentId = reader.GetInt32(reader.GetOrdinal("StudentId"));
                            if (!cohort.Students.Any(s => s.Id == studentId))
                            {
                                Student aStudent = new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("StudentCohortId"))
                                };
                                cohort.Students.Add(aStudent);
                            };
                            
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("InstructorId")))
                        {
                            int instructorId = reader.GetInt32(reader.GetOrdinal("InstructorId"));
                            if (!cohort.Instructors.Any(i => i.Id == instructorId))
                            {
                                Instructor anInstructor = new Instructor
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("InstructorCohortId")),
                                    Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
                                };
                                cohort.Instructors.Add(anInstructor);
                            }
                            
                        }

                    }
                    reader.Close();



                    return Ok(cohort);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cohort cohort)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Cohort (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";
                    cmd.Parameters.Add(new SqlParameter("@name", cohort.Name));

                    int newId = (int)cmd.ExecuteScalar();
                    cohort.Id = newId;
                    return CreatedAtRoute("GetCohort", new { id = newId }, cohort);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Cohort cohort)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Cohort
                                            SET Name = @name
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@name", cohort.Name));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!CohortExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        private bool CohortExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, Students, Instructors
                        FROM Cohort
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

    }
}