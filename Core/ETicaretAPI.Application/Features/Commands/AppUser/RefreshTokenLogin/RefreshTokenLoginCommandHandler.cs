﻿using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.RefreshTokenLogin
{

    public class RefreshTokenLoginCommandHandler : IRequestHandler<RefreshTokenLoginCommandRQ, RefreshTokenLoginCommandRP>
    {
        readonly IAuthService _authService;

        public RefreshTokenLoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<RefreshTokenLoginCommandRP> Handle(RefreshTokenLoginCommandRQ request, CancellationToken cancellationToken)
        {
            Token token = await _authService.RefreshTokenLoginAsync(request.RefreshToken);
            return new()
            {
                Token = token,
            };
        }
    }
}