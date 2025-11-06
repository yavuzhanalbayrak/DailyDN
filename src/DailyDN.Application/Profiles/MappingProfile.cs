using AutoMapper;
using DailyDN.Application.Features.Auth.RefreshToken;
using DailyDN.Application.Features.Auth.VerifyOtp;
using DailyDN.Application.Features.Posts.Add;
using DailyDN.Application.Features.Posts.GetList;
using DailyDN.Application.Features.Posts.GetList.Response;
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
            CreateMap<User, UserResponse>();
            CreateMap<TokenResponse, RefreshTokenCommandResponse>();

            CreateMap<Post, PostResponse>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<List<Post>, GetPostListQueryResponse>()
                .ForMember(dest => dest.Posts, opt => opt.MapFrom(src => src));
        }
    }
}