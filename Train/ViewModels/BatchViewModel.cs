﻿using System.ComponentModel.DataAnnotations;
using Train.Models;

namespace Train.ViewModels
{
    public class BatchViewModel
    {
        public BatchViewModel()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        [DisplayFormat]
        [DataType(DataType.Date)]
        [Required]
        public DateTime StartDate { get; set; }= DateTime.Now;
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Please select Course")]
        [Display(Name = "Course")]
        public string CourseId { get; set; }
    }
}
