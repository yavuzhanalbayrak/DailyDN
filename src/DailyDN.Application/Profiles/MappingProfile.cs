using AutoMapper;
using DailyDN.Application.Features.Auth.VerifyOtp;
using DailyDN.Application.Features.Posts.Add;
using DailyDN.Domain.Entities;
using DailyDN.Infrastructure.Models;

namespace DailyDN.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TokenResponse, VerifyOtpCommandResponse>();
            CreateMap<AddPostCommand, Post>();
        }
    }
}