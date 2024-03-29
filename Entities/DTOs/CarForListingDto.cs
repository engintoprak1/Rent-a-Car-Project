﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Entities.Concrete;

namespace Entities.DTOs
{
    public class CarForListingDto:IDto
    {
        public int Id { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }                  
        public string ColorName { get; set; }
        public string Description { get; set; }
        public decimal DailyPrice { get; set; }
        public int ModelYear { get; set; }
        public bool AvailableForRent { get; set; }
        public CarImage Image { get; set; }
        public int Findeks { get; set; }
    }
}
