using Train.ModelViews;

namespace Train.Services
{
    public class EmployeeService
    {
        // Assuming this is a mock data method, you might replace it with actual data retrieval logic.
        private List<EmployeeModelView> GetMockEmployeeData(int numberOfEmployees)
        {
            List<EmployeeModelView> employeeList = new List<EmployeeModelView>();

            // Mock data for demonstration purposes
            for (int i = 1; i <= numberOfEmployees; i++)
            {
                employeeList.Add(new EmployeeModelView
                {
                    EmployeeId = i,
                    FirstName = $"Employee{i}",
                    LastName = "Doe",
                    DateOfBirth = new DateTime(1990, 5, 15).AddYears(-i), // Adjusting birthdate for variety
                    Salary = 50000 + (i * 1000) // Adjusting salary for variety
                });
            }

            return employeeList;
        }

        public List<EmployeeModelView> GetEmployeeList()
        {
            // Replace this with your actual data retrieval logic
            return GetMockEmployeeData(5);
        }
    }
}
