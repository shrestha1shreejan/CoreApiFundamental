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
    [Route("api/camp/{moniker}/[controller]")]
    [ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _generator;
        #region Constructor

        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator generator)
        {
            _repository = repository;
            _mapper = mapper;
            _generator = generator;
        }

        #endregion

        [HttpGet]
        public async Task<ActionResult<TalksDTO[]>> Get(string moniker)
        {
            try
            {
                var talks = await _repository.GetTalksByMonikerAsync(moniker, true);
                return _mapper.Map<TalksDTO[]>(talks);
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB error");
            }
            
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalksDTO>> GetTalk(string moniker, int id )
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id ,true);

                if (talk == null)
                {
                    return NotFound(" Talk not found ");
                }
                return _mapper.Map<TalksDTO>(talk);
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB error");
            }

        }


        [HttpPost()]
        public async Task<ActionResult<TalksDTO>> CreateTalk(string moniker, TalksDTO model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null)
                {
                    return BadRequest($"Camp for the specified moniker {moniker} doesn't exists");
                }
                var talk = _mapper.Map<Talk>(model);
                talk.Camp = camp;

                // speaker
                if (model.Speaker == null)
                {
                    return BadRequest("Speaker ID is required");
                }
                var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker == null)
                {
                    return BadRequest("Speaker could not be found");
                }
                talk.Speaker = speaker;

                _repository.Add(talk);

                if (await _repository.SaveChangesAsync())
                {
                    var url = _generator.GetPathByAction(HttpContext, "Get", values: new { moniker, id = model.TalkId });
                    return Created(url, _mapper.Map<TalksDTO>(talk));
                }

            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB error");
            }

            return BadRequest("Failed to create the talk");

        }



        [HttpPost("{id:int}")]
        public async Task<ActionResult<TalksDTO>> UpdateTalkByID(string moniker, int id, TalksDTO model)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id, true);
                if (talk == null)
                {
                    BadRequest("Talk not found");
                }

                _mapper.Map(model, talk);

                if (model.Speaker != null)
                {
                    var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if (speaker != null)
                    {
                        talk.Speaker = speaker;
                    }
                }

                if(await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<TalksDTO>(talk);
                }
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB error");
            }

            return BadRequest("Failed to Update the talk");
        }



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTalk(string moniker, int id)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id, true);
                if (talk == null)
                {
                    return NotFound("Failed to find the talk");
                }
                _repository.Delete(talk);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
               
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB error");
            }
            return BadRequest("Failed to delete the talk");
        }

    }
}