using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JamalKhanah.Core.DTO.EntityDto;

public class ProviderPhotosDto
{
    [Required]
    public string Photo { get; set; }
}