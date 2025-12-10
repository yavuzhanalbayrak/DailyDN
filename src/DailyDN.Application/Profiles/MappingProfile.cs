using AutoMapper;
using DailyDN.Application.Dtos.RedisUser;
using DailyDN.Application.Features.Auth.RefreshToken;
using DailyDN.Application.Features.Auth.VerifyOtp;
using DailyDN.Application.Features.Posts.Add;
using DailyDN.Application.Features.Posts.GetList;
using DailyDN.Application.Features.Posts.GetList.Response;
using DailyDN.Application.Features.Users.GetUserById;
using DailyDN.Application.Features.Users.GetUserById.Responses;
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

            CreateMap<User, GetUserByIdQueryResponse>()
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.UserRoles));

            CreateMap<UserRole, UserRoleResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Role.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<User, RedisUserDto>()
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.UserRoles));

            CreateMap<UserRole, RedisUserRoleDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Role.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<RedisUserRoleDto, UserRole>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => new Role((Domain.Enums.Role)src.Id)));

            CreateMap<RedisUserDto, User>()
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.UserRoles))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}