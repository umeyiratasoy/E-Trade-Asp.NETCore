using ETicaretAPI.Application.Abstractions.Token;
using AP = ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using ETicaretAPI.Application.DTOs.Facebook;
using System.Text.Json;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.Abstractions.Services;


namespace ETicaretAPI.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandRequest, FacebookLoginCommandResponse>
    {
        readonly IAuthService _authService;

        public FacebookLoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
        {
            var token = await _authService.FacebookLoginAsync(request.AuthToken, 15);  //saniye cinsinden süresi var tokenin örn:15
            return new()
            {
                Token = token
            };

        }
    }
}
