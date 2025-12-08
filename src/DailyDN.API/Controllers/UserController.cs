using DailyDN.Application.Common.Model;
using DailyDN.Application.Features.Auth.ForgotPassword;
using DailyDN.Application.Features.Auth.Login;
using DailyDN.Application.Features.Auth.RefreshToken;
using DailyDN.Application.Features.Auth.Register;
using DailyDN.Application.Features.Auth.ResetPassword;
using DailyDN.Application.Features.Auth.VerifyOtp;
using DailyDN.Application.Features.Users.GetUserById;
using DailyDN.Application.Features.Users.UpdateProfilePhoto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyDN.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class UserController(IMediator _mediator) : ControllerBase
    {
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result))]
        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetUserByIdQuery(id);
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result))]
        [HttpPut("profile-photo")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateProfilePhoto([FromForm] UpdateProfilePhotoCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

    }
}