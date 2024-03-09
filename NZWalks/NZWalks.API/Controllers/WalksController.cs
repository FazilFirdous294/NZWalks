using AutoMapper;
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

        public WalksController(IWalkRepository walkRepository, IMapper mapper)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
        }
        [HttpGet]
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
        public async Task<IActionResult> AddWalkAsync([FromBody]Models.DTO.AddWalkRequest addWalkRequest)
        {
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

        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id,
            [FromBody]Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
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
    }
}
