﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreCodeCamp.DTO
{
    public class CampDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public string Moniker { get; set; }               
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        [Range(1, 100)]
        public int Length { get; set; } = 1;

        public string Venue { get; set; }
        public string LocationAddress1 { get; set; }
        public string LocationAddress2 { get; set; }
        public string LocationAddress3 { get; set; }
        public string LocationCityTown { get; set; }
        public string LocationStateProvince { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }


        public ICollection<TalksDTO> Talks { get; set; }
    }
}