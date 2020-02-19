using System;
using System.ComponentModel.DataAnnotations;
using Restaurante.IO.Business.Models.Attributes;

namespace Restaurante.IO.Api.ViewModels
{
    public abstract class MainViewModel
    {
        [Key]
        [NotEmpty]
        public Guid Id { get; set; }
    }
}