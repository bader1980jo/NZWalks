using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IWalkRepository _walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            _mapper = mapper;
            _walkRepository = walkRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegionsAsync()
        {
            //fetch data from database
            var walks = await _walkRepository.GetAllAsync();

            //convert data from domain to dto
            var walksDto = _mapper.Map<List<Models.DTO.Walk>>(walks);

            //return response
            return Ok(walksDto);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkAsync")]
        public async Task<IActionResult> GetWalkAsync(Guid id)
        {
            var walk = await _walkRepository.GetAsync(id);

            if (walk == null)
            {
                return NotFound();
            }

            var walkDTO = _mapper.Map<Models.DTO.Walk>(walk);

            return Ok(walkDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddRegionAsync([FromBody] AddWalkRequest addWalkRequest)
        {
            //convert dto to domain model
            var walk = new Models.Domain.Walk()
            {
                Name = addWalkRequest.Name,
                Length = addWalkRequest.Length,
                RegionId = addWalkRequest.RegionId,
                WalkDifficultyId = addWalkRequest.WalkDifficultyId
                
            };
            //pass domain object to repository
            walk = await _walkRepository.AddAsync(walk);

            //convert domain object to dto
            var walkDTO = new Models.DTO.Walk()
            {
                Id = walk.Id,
                Name = walk.Name,
                Length = walk.Length,
                WalkDifficultyId = walk.WalkDifficultyId,
                RegionId = walk.RegionId
            };

            //send the dto  response to client
            return CreatedAtAction(nameof(GetWalkAsync), new { id = walkDTO.Id }, walkDTO);
        }
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id, [FromBody] UpdateWalkRequest updateWalkRequest)
        {
            //convert dto to domain
            var walk = new Models.Domain.Walk()
            {
                Length = updateWalkRequest.Length,
                Name = updateWalkRequest.Name,
                RegionId = updateWalkRequest.RegionId,
                WalkDifficultyId = updateWalkRequest.WalkDifficultyId

            };
            //pass domain to repository
            var updatedWalk = await _walkRepository.UpdateWalkAsync(id, walk);
            //get domain in response or null
            if (updatedWalk == null)
            {
                return NotFound("Walk with this ID is not Found");
            }
            //convert back to dto
            var updatedWalkDTO = new Models.DTO.Walk()
            {
                Id = id,
                WalkDifficultyId = updatedWalk.WalkDifficultyId,
                RegionId = updatedWalk.RegionId,
                Name = updatedWalk.Name,
                Length = updatedWalk.Length
            };
            //send dto to client
            return Ok(updatedWalkDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAync([FromRoute] Guid id)
        {
            //call repo to delete walk
            var walk = await _walkRepository.DeleteAsync(id);

            if (walk == null)
            {
                return NotFound("No Walk Found with this ID");
            }

            //convert domain to dto
            var walkDTO = new Models.DTO.Walk()
            {
                Id = id,
                Name = walk.Name,
                Length = walk.Length,
                RegionId = walk.RegionId,
                WalkDifficultyId = walk.WalkDifficultyId
            };

            //send response back to client
            return Ok(walkDTO);
        }
    }

 }
