

SELECT c.Id, c.Name, s.FirstName AS StudentFirstName, s.LastName AS StudentLastName, s.SlackHandle AS StudentSlack
                                      , i.FirstName AS InstructorFirstName, i.LastName AS InstructorLastName, i.SlackHandle AS InstructorSlack
                                        FROM Cohort c
                                        INNER JOIN Student s ON s.CohortId = c.Id
                                        INNER JOIN Instructor i ON i.CohortId = c.Id