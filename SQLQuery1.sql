SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, e.Name, c.Name 
                                        FROM Student s 
                                        INNER JOIN Cohort c ON s.CohortId = c.Id
                                        INNER JOIN StudentExerciseRel se ON se.StudentId = s.Id
                                        INNER JOIN Exercise e ON se.ExerciseId = e.Id