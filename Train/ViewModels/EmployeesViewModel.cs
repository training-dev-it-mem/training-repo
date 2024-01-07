﻿using Train.Models;

namespace Train.ViewModels
{
    public class EmployeesViewModel
    {
        public List<Employee> Employees { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
