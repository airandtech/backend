using System.Collections.Generic;
using AirandWebAPI.Core.Domain;

namespace AirandWebAPI.Services.Contract
{


    public interface IRegionService
    {
        IEnumerable<Region> GetAll();
    }

}