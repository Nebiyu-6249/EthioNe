using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace EthioNegarit.WebUI.ViewModels
{
    public class LocationViewModel
    {
        public IList<Location> Locations { get; set; }
        public Advertisement Advertisement { get; set; }
        [HiddenInput]
        public Guid AdvertId { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        public string CityName { get; set; }
        public string Description { get; set; }
    }
}