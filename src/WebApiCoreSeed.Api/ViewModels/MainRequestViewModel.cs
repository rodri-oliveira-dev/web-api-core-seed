using System;
using System.ComponentModel.DataAnnotations;
using WebApiCoreSeed.SampleRestaurant.Models.Attributes;

namespace WebApiCoreSeed.Api.ViewModels
{
    public abstract class MainRequestViewModel
    {
        [Required]
        [NotEmpty]
        public required Guid Id { get; set; }
    }
}
