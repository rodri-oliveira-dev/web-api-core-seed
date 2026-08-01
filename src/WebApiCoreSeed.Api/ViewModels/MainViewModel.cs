using System;
using System.ComponentModel.DataAnnotations;
using WebApiCoreSeed.SampleRestaurant.Models.Attributes;

namespace WebApiCoreSeed.Api.ViewModels
{
    public abstract class MainViewModel
    {
        [Key]
        [NotEmpty]
        public Guid Id { get; set; }
    }
}