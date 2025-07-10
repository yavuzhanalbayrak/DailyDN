using DailyDN.Application.Common.Attributes;
using DailyDN.Application.Common.Model;
using DailyDN.Application.Features.Posts.Add;
using DailyDN.Application.Features.Posts.GetList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyDN.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class PostController(IMediator _mediator) : ControllerBase
    {
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<GetListQueryResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<GetListQueryResponse>))]
        [HttpGet]
        [MapToApiVersion("1.0")]
        [Authorized("PostGet")]
        public async Task<IActionResult> GetList([FromQuery] GetListQuery query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result))]
        [HttpPost]
        [MapToApiVersion("1.0")]
        [Authorized("PostAdd")]
        public async Task<IActionResult> Add([FromBody] AddPostCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}