﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.RequestModels
{
    public class VideoCategoryRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
