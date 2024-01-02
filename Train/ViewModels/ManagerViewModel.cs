﻿using Train.Models;

namespace Train.ModelViews
{
    public class ManagerViewModel
    {
        public List<Managers> Managers { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}