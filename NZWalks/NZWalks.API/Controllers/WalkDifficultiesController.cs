using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalkDifficultiesController : Controller
    {
        private readonly IWalkDifficultyRepository walkDifficultyRepository;
        private readonly IMapper mapper;

        public WalkDifficultiesController(IWalkDifficultyRepository walkDifficultyRepository, IMapper mapper)
        {
            this.walkDifficultyRepository = walkDifficultyRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalkDifficultiesAsync()
        {
            var walkDifficulties = await walkDifficultyRepository.GetAllAsync();


            var walkDifficultiesDTO = mapper.Map<List<Models.DTO.WalkDifficulty>>(walkDifficulties);
            return Ok(walkDifficultiesDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkDifficultyAsync")]
        public async Task<IActionResult> GetWalkDifficultyAsync(Guid id)
        {
            var walkDifficulty = await walkDifficultyRepository.GetAsync(id);
            if (walkDifficulty == null)
            {
                return NotFound("No Walk Difficulty Found With This ID");
            }
            var walkDifficultyDTO = mapper.Map<Models.DTO.WalkDifficulty>(walkDifficulty);


            return Ok(walkDifficultyDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(AddWalkDifficultyRequest addWalkDifficultyRequest)
        {
            //validate the incoming request
            //if (! ValidateAddAsync(addWalkDifficultyRequest))
            //{
            //    return BadRequest(ModelState);
            //}


            //convert dto to domain
            var walkDifficulty = new Models.Domain.WalkDifficulty()
            {
                Code = addWalkDifficultyRequest.Code
            };
            //add domain to database
            walkDifficulty =  await walkDifficultyRepository.AddAsync(walkDifficulty);

            //convert domain to dto
            var walkDifficultyDTO =  mapper.Map<Models.DTO.WalkDifficulty>(walkDifficulty);




            //send response back to client

            return CreatedAtAction(nameof(GetWalkDifficultyAsync), new { id = walkDifficulty.Id }, walkDifficultyDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkDifficultyAsync(Guid id, [FromBody] UpdateWalkDifficultyRequest updateWalkDifficultyRequest)
        {
            //validate the incoming request
            //if (!ValidateUpdateWalkDifficultyAsync(updateWalkDifficultyRequest))
            //{
            //    return BadRequest(ModelState);
            //}


            // convert dto to domain
            var walkDifficulty = new Models.Domain.WalkDifficulty()
            {
                Id = id,
                Code = updateWalkDifficultyRequest.Code
            };

            //send domain to db
            var existingWalkDifficulty = await walkDifficultyRepository.UpdateRegionAsync(id, walkDifficulty);
            //check if null
            if (existingWalkDifficulty == null)
            {
                return NotFound("No Walk Difficulty Found With This ID");

            }
            //convert domain to dto
            var walkDifficultyDTO = mapper.Map<Models.DTO.WalkDifficulty>(existingWalkDifficulty);
          
            //send the response to client
            return Ok(walkDifficultyDTO);
        }


        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkDifficultyAysnc(Guid id)
        {
            var walkDifficulty = await walkDifficultyRepository.DeleteAsync(id);
            if (walkDifficulty == null)
            {
                return NotFound("No Walk Difficulty Found With This ID");
            }
            var walkDifficultyDTO = mapper.Map<Models.DTO.WalkDifficulty>(walkDifficulty);
            return Ok(walkDifficultyDTO);
        }


        #region Private Methods

        private bool ValidateAddAsync(AddWalkDifficultyRequest addWalkDifficultyRequest)
        {
            if (addWalkDifficultyRequest == null)
            {
                ModelState.AddModelError(nameof(addWalkDifficultyRequest)
                    , $"Add Region Data is Required");
                return false;
            }
            if (string.IsNullOrWhiteSpace(addWalkDifficultyRequest.Code))
            {
                ModelState.AddModelError(nameof(addWalkDifficultyRequest.Code)
                    , $"{nameof(addWalkDifficultyRequest.Code)} cannot be empty or white space");
            }

            return true;
        }

        private bool ValidateUpdateWalkDifficultyAsync(UpdateWalkDifficultyRequest updateWalkDifficultyRequest)
        {
            if (updateWalkDifficultyRequest == null)
            {
                ModelState.AddModelError(nameof(updateWalkDifficultyRequest)
                    , $"Add Region Data is Required");
                return false;
            }
            if (string.IsNullOrWhiteSpace(updateWalkDifficultyRequest.Code))
            {
                ModelState.AddModelError(nameof(updateWalkDifficultyRequest.Code)
                    , $"{nameof(updateWalkDifficultyRequest.Code)} cannot be empty or white space");
            }

            return true;
        }

        #endregion

    }

}
