﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs
{
    public class CarForUpdateDto : IDto
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public string ModelName { get; set; }
        public int ColorId { get; set; }
        public int ModelYear { get; set; }
        public decimal DailyPrice { get; set; }
        public string Description { get; set; }
        public List<string> Images { get; set; }
        public int Findeks { get; set; }
    }
}
