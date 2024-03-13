using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
  
    public class WalksController : Controller
    {
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;
        private readonly IRegionRepository regionRepository;

        public WalksController(IWalkRepository walkRepository, IMapper mapper,
            IRegionRepository regionRepository)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
            this.regionRepository = regionRepository;
        }

        [HttpGet]
        [Authorize(Roles = "reader")]
        public async Task<IActionResult> GetAllWalksAsync()
        {
            // Fetch Data from database - domain walks

            var walksDomain = await walkRepository.GetAllAsync();


            // Convert Domain walks to DTO Walks

            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walksDomain);

            //Return Response

            return Ok(walksDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkAsync")]
        [Authorize(Roles = "reader")]
        public async Task<IActionResult> GetWalkAsync(Guid id)
        {
            // Get Walk Domain Object from database
           var walkDomain = await walkRepository.GetAsync(id);

            // Convert Domain Object to DTO
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            // Return Response
            return Ok(walkDTO);
        }

        [HttpPost]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> AddWalkAsync([FromBody]Models.DTO.AddWalkRequest addWalkRequest)
        {
            // Validate the incoming request
            if (!await ValidateAddWalkAsync(addWalkRequest))
            {
                return BadRequest(ModelState);
            }

            // Convert DTO to Domain Object
            var walkDomain = new Models.Domain.Walk
            {
                length = addWalkRequest.length,
                Name = addWalkRequest.Name,
                RegionId = addWalkRequest.RegionId,
                WalkDiffcultyId = addWalkRequest.WalkDiffcultyId
            };
            // Pass Domain object to repository to persist this
            walkDomain = await walkRepository.AddAsync(walkDomain);

            // Convert the Domain object back to DTO
            var WalkDTO = new Models.DTO.Walk
            {
                Id = walkDomain.Id,
                length = walkDomain.length,
                Name = walkDomain.Name,
                RegionId = walkDomain.RegionId,
                WalkDiffcultyId = walkDomain.WalkDiffcultyId
            };
            // Send DTO response bact to Client

            return CreatedAtAction(nameof(GetWalkAsync),new {id = WalkDTO.Id},WalkDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id,
            [FromBody]Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            // Validate incoming request
            if(!(await ValidateUpdateWalkAsync(updateWalkRequest)))
            {
                return BadRequest(ModelState);
            }

            // Convert DTO to Domain Object
            var walkDomain = new Models.Domain.Walk
            {
                length = updateWalkRequest.length,
                Name = updateWalkRequest.Name,
                RegionId = updateWalkRequest.RegionId,
                WalkDiffcultyId = updateWalkRequest.WalkDiffcultyId
            };
            //pas Details to repositor - Get Domain object response (or Null)
               walkDomain = await walkRepository.UpdateAsync(id, walkDomain);
            // Handle Null (Not Found)
            if (walkDomain == null)
            {
                return NotFound();
            }

            //Convert back domain to DTO
            var walkDTO = new Models.DTO.Walk
            {
                Id = walkDomain.Id,
                length = walkDomain.length,
                Name = walkDomain.Name,
                RegionId = walkDomain.RegionId,
                WalkDiffcultyId = walkDomain.WalkDiffcultyId
            };
            // Return Response
            return Ok(walkDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "writer")]
        public async Task<IActionResult> DeleteWalkAsync(Guid id)
        {
            // Call Repository to delete walk
            var walkDomain = await walkRepository.DeleteAsync(id);

            if (walkDomain == null)
            {
                return NotFound();
            }
           var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);
            return Ok(walkDTO);
        }


        #region Private  methods
        private async Task<bool> ValidateAddWalkAsync(Models.DTO.AddWalkRequest addWalkRequest)
        {
            //if (addWalkRequest == null)
            //{
            //    ModelState.AddModelError(nameof(addWalkRequest),
            //       $"{nameof(addWalkRequest)} Cannot be empty.");
            //    return false;
            //}
            //if (string.IsNullOrWhiteSpace(addWalkRequest.Name))
            //{
            //    ModelState.AddModelError(nameof(addWalkRequest.Name),
            //    $"{nameof(addWalkRequest.Name)} is required");
            //}

            //if (addWalkRequest.length <= 0)
            //{
            //    ModelState.AddModelError(nameof(addWalkRequest.length),
            //    $"{nameof(addWalkRequest.length)} Should be greater than zero.");
            //}

            var region = await regionRepository.GetAsync(addWalkRequest.RegionId);
            if (region == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.RegionId),
              $"{nameof(addWalkRequest.RegionId)}is invalid.");
            }

            if (ModelState.ErrorCount >0)
            {
                return false;
            }
            return true;

        }

        private async Task<bool> ValidateUpdateWalkAsync(Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            //if (updateWalkRequest == null)
            //{
            //    ModelState.AddModelError(nameof(updateWalkRequest),
            //       $"{nameof(updateWalkRequest)} Cannot be empty.");
            //    return false;
            //}
            //if (string.IsNullOrWhiteSpace(updateWalkRequest.Name))
            //{
            //    ModelState.AddModelError(nameof(updateWalkRequest.Name),
            //    $"{nameof(updateWalkRequest.Name)} is required");
            //}

            //if (updateWalkRequest.length <= 0)
            //{
            //    ModelState.AddModelError(nameof(updateWalkRequest.length),
            //    $"{nameof(updateWalkRequest.length)} Should be greater than zero.");
            //}

            var region = await regionRepository.GetAsync(updateWalkRequest.RegionId);
            if (region == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.RegionId),
              $"{nameof(updateWalkRequest.RegionId)}is invalid.");
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;

        }
        #endregion
    }
}
