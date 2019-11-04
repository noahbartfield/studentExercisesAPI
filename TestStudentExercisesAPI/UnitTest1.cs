using Newtonsoft.Json;
using StudentExercisesAPI.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace TestStudentExercisesAPI
{
    public class StudentTest
    {
        [Fact]
        public async Task Test_Get_All_Students()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/students");


                string responseBody = await response.Content.ReadAsStringAsync();
                var studentList = JsonConvert.DeserializeObject<List<Student>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(studentList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Create_Student()
        {
            using (var client = new APIClientProvider().Client)
            {

                Student jimStudent = new Student
                {
                    FirstName = "Jim",
                    LastName = "Bimtimin",
                    SlackHandle = "@jimb",
                    CohortId = 2
                };

                var jimAsJSON = JsonConvert.SerializeObject(jimStudent);

                var response = await client.PostAsync(
                    "/api/students",
                    new StringContent(jimAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newStudent = JsonConvert.DeserializeObject<Student>(responseBody);


                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Jim", newStudent.FirstName);
                Assert.Equal("Bimtimin", newStudent.LastName);
                Assert.Equal("@jimb", newStudent.SlackHandle);
                Assert.Equal(2, newStudent.CohortId);
            }
        }

        [Fact]
        public async Task Test_Modify_Student()
        {
            int newCohortId = 1;

            using (var client = new APIClientProvider().Client)
            {
                Student modifiedJim = new Student
                {
                    FirstName = "Michael",
                    LastName = "Stiles",
                    SlackHandle = "@michaelstiles",
                    CohortId = newCohortId
                };
                var modifiedJimAsJSON = JsonConvert.SerializeObject(modifiedJim);

                var response = await client.PutAsync(
                    "/api/students/1",
                    new StringContent(modifiedJimAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);



                var getJim = await client.GetAsync("/api/students/1");
                getJim.EnsureSuccessStatusCode();

                string getJimBody = await getJim.Content.ReadAsStringAsync();
                Student newJim = JsonConvert.DeserializeObject<Student>(getJimBody);

                Assert.Equal(HttpStatusCode.OK, getJim.StatusCode);
                Assert.Equal(newCohortId, newJim.CohortId);
            }
        }
    }
}
