using System;
using System.Collections.Generic;
using System.Text;

namespace StudentExercisesAPI.Models

{
    public class Student : Person
    {
        //public Student(Cohort cohort)
        //{
        //    Cohort = cohort;
        //    Exercises = new List<Exercise>();
        //}
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();

    }
}
