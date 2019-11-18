using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuctionalAPIController : ControllerBase
    {
        private readonly IConfiguration _config;
        #region Constructor

        public FuctionalAPIController(IConfiguration config)
        {
            _config = config;
        }

        #endregion

        [HttpOptions("reloadConfig")]
        public IActionResult ReloadConfig()
        {
            try
            {
                var root = (IConfigurationRoot)_config;
                root.Reload();
                return Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}