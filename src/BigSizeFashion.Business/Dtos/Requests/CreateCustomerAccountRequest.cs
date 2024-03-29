﻿using BigSizeFashion.Business.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Helpers.RequestObjects
{
    public class CreateCustomerAccountRequest
    {
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"[0]{1}[0-9]{9}")]
        [MaxLength(10)]
        [MinLength(10)]
        public string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string Fullname { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public GenderEnum Gender { get; set; }

        [Required]
        [Range(0,255)]
        public int Weight { get; set; }

        [Required]
        [Range(0, 255)]
        public int Height { get; set; }
    }
}
