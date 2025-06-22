using Microsoft.AspNetCore.Mvc;

using Presentation.Files;

namespace Presentation.Controllers
{
    [ApiController]
    public class LinesController : ControllerBase
    {
        private readonly IFileReader reader;

        public LinesController(IFileReader reader)
        {
            this.reader = reader;
        }

        [Route("lines/{index}")]
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromRoute] int index)
        {
            try
            {
                var line = await this.reader.GetLineAsync(index);

                return Ok(line);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
