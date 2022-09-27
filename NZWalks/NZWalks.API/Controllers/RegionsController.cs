using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegionsController : Controller
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;
        public RegionsController(IRegionRepository regionRepository, IMapper mapper)
        {
            _regionRepository = regionRepository;
            _mapper = mapper;
        }

        
     

        [HttpGet]
       public async Task<IActionResult> GetAllRegions()
        {
            var regions = await _regionRepository.GetAllAsync();


            //return region dto
            //var regionsDto = new List<Models.DTO.Region>();

            //regions.ToList().ForEach(region =>
            //{
            //    //var regionDTO = new Models.DTO.Region()
            //    //{
            //    //    Id = region.Id,
            //    //    Name = region.Name,
            //    //    Area = region.Area,
            //    //    Code = region.Code,
            //    //    Lat = region.Lat,
            //    //    Long = region.Long,
            //    //    Population = region.Population,
            //    //    //Walks = region.Walks,

            //    //};

                
                
            //});
            var regionsDTO = _mapper.Map<List<Models.DTO.Region>>(regions);
            return Ok(regionsDTO);
        }
    }
}
