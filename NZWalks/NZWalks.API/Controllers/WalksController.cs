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
        private readonly IWalkDifficultyRepository walkDifficultyRepository;
        private readonly IRegionRepository regionRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository,IWalkDifficultyRepository walkDifficultyRepository, IRegionRepository regionRepository)
        {
            _mapper = mapper;
            _walkRepository = walkRepository;
            this.walkDifficultyRepository = walkDifficultyRepository;
            this.regionRepository = regionRepository;
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
        public async Task<IActionResult> AddWalkAsync([FromBody] AddWalkRequest addWalkRequest)
        {
            //validate the incoming request
            if (!(await ValidateAddWalkAsync(addWalkRequest)))
            {
                return BadRequest(ModelState);
            }

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
            //validate the incoming request
            if (!(await ValidateUpdateWalkAsync(updateWalkRequest)))
            {
                return BadRequest(ModelState);
            }


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

        #region Private Methods

        private async Task<bool> ValidateAddWalkAsync(AddWalkRequest addWalkRequest)
        {
            //if (addWalkRequest == null)
            //{
            //    ModelState.AddModelError(nameof(addWalkRequest)
            //        , $"{nameof(addWalkRequest)} cannot be empty");
            //    return false;
            //}
            //if (string.IsNullOrWhiteSpace(addWalkRequest.Name))
            //{
            //    ModelState.AddModelError(nameof(addWalkRequest.Name)
            //        , $"{nameof(addWalkRequest.Name)} cannot be empty or white space");
            //}
            //if (addWalkRequest.Length <= 0)
            //{
            //    ModelState.AddModelError(nameof(addWalkRequest.Length)
            //        , $"{nameof(addWalkRequest.Length)} should be greater than 0");
            //}

            var region = await regionRepository.GetAsync(addWalkRequest.RegionId);
            if (region == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.RegionId)
                   , $"{nameof(addWalkRequest.RegionId)} is invalid");
            }

            var walkDifficulty = await walkDifficultyRepository.GetAsync(addWalkRequest.WalkDifficultyId);
            if (walkDifficulty == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.WalkDifficultyId)
                   , $"{nameof(addWalkRequest.WalkDifficultyId)} is invalid");
            }


            return true;


        }

        private async Task<bool> ValidateUpdateWalkAsync(UpdateWalkRequest updateWalkRequest)
        {
            //if (updateWalkRequest == null)
            //{
            //    ModelState.AddModelError(nameof(updateWalkRequest)
            //        , $"{nameof(updateWalkRequest)} cannot be empty");
            //    return false;
            //}
            //if (string.IsNullOrWhiteSpace(updateWalkRequest.Name))
            //{
            //    ModelState.AddModelError(nameof(updateWalkRequest.Name)
            //        , $"{nameof(updateWalkRequest.Name)} cannot be empty or white space");
            //}
            //if (updateWalkRequest.Length <= 0)
            //{
            //    ModelState.AddModelError(nameof(updateWalkRequest.Length)
            //        , $"{nameof(updateWalkRequest.Length)} should be greater than 0");
            //}

            var region = await regionRepository.GetAsync(updateWalkRequest.RegionId);
            if (region == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.RegionId)
                   , $"{nameof(updateWalkRequest.RegionId)} is invalid");
            }

            var walkDifficulty = await walkDifficultyRepository.GetAsync(updateWalkRequest.WalkDifficultyId);
            if (walkDifficulty == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.WalkDifficultyId)
                   , $"{nameof(updateWalkRequest.WalkDifficultyId)} is invalid");
            }


            return true;


        }

        #endregion
    }

}
