using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JamalKhanah.Core.DTO;

public class GetAllServices
{
    public string ServiceProviderName { get; set; } = null;
    public string ServiceProviderId { get; set; } = null;
    public string SearchName { get; set; } = null;
    public int? CityId { get; set; } = null;
    public int? MainSectionId { get; set; } = null;
    public bool? InHome { get; set; } = null;

    public bool? InCenter  { get; set; } = null;

    public ServiceTypeDto ServiceTypeDto { get; set; } 


    public bool OrderFromNew { get; set; }= true ;

    public int? StartPrice { get; set; } = null;

    public int? EndPrice { get; set; } = null;
}

public enum ServiceTypeDto
{
    Both, Center ,FreeAgent 
}