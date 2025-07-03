using AutoMapper;
using DailyDN.Application.Features.Auth.VerifyOtp;
using DailyDN.Infrastructure.Models;

namespace DailyDN.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TokenResponse, VerifyOtpCommandResponse>();
        }
    }
}