using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampController : ControllerBase
    {
        private readonly ICampRepository _repo;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _generator;

        public CampController(ICampRepository repository, IMapper mapper, LinkGenerator generator)
        {
            _repo = repository;
            _mapper = mapper;
            _generator = generator;
        }


        [HttpGet]
        public async Task<ActionResult<CampDTO[]>> Get(bool includeTalks = false)
        {
            try
            {
                var result = await _repo.GetAllCampsAsync(includeTalks);
                var camps = _mapper.Map<CampDTO[]>(result);
                return camps;
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB error");
            }
           
        }
        
        [HttpGet("{moniker}", Name ="GetCampById")]          
        public async Task<ActionResult<CampDTO>> Get(string moniker)
        {
            try
            {
                var result = await _repo.GetCampAsync(moniker);

                if (result == null)
                {
                    return NotFound();
                }
                var camps = _mapper.Map<CampDTO>(result);
                return camps;
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB error");
            }

        }


        [HttpGet("search")]
        public async Task<ActionResult<CampDTO[]>> SearchByDate(DateTime eventDate, bool includeTalks = false)
        {
            try
            {
                var result = await _repo.GetAllCampsByEventDate(eventDate,includeTalks);

                if (!result.Any())
                {
                    return NotFound();
                }
                var camps = _mapper.Map<CampDTO[]>(result);
                return camps;
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB error");
            }
        }


        [HttpPost]
        public async Task<ActionResult<CampDTO>> Post (CampDTO campModel)
        {
            try
            {
                // Validation for dublicate moniker
                var campByMoniker = await _repo.GetCampAsync(campModel.Moniker);
                if (campByMoniker != null)
                {
                    return BadRequest("Camp with the Moniker already exists, please select a different Moniker");
                }


                var location = _generator.GetPathByAction("Get", "Camp", new { moniker = campModel.Moniker });

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest(" Moniker is invalid ");
                }

                var camp = _mapper.Map<Camp>(campModel);
                _repo.Add(camp);
                if (await _repo.SaveChangesAsync())
                {
                    return Created(location, _mapper.Map<CampDTO>(camp));
                }
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB error");
            }

            return BadRequest();
        }


        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampDTO>> Update(string moniker, CampDTO model)
        {
            try
            {

                var campToUpdate = await _repo.GetCampAsync(moniker);

                if (campToUpdate == null)
                {
                    return NotFound($"Camp with the moniker {moniker} does not exists!!!");
                }

                _mapper.Map(model, campToUpdate);

                if (await _repo.SaveChangesAsync())
                {
                    return _mapper.Map<CampDTO>(campToUpdate);
                }

            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB error");
            }

            return BadRequest(" Failed to update the Camp ");
        }


        [HttpDelete("{moniker}")]
        public async Task<IActionResult> DeleteCamp (string moniker)
        {
            try
            {
                var campToDelete = await _repo.GetCampAsync(moniker);

                if (campToDelete == null)
                {
                    return NotFound($"Camp with the moniker {moniker} does not exists!!!");
                }

                _repo.Delete(campToDelete);

                if (await _repo.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB error");
            }

            return BadRequest("Failed to delete camp");
        }
    }
}