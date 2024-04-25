using AP =  ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;

namespace ETicaretAPI.Application.Features.Commands.AppUser.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
    {

        readonly UserManager<AP.AppUser> _userManager;
        readonly SignInManager<AP.AppUser> _signInManager;
        readonly ITokenHandler _tokenHandler;

        public LoginUserCommandHandler(UserManager<AP.AppUser> userManager, SignInManager<AP.AppUser> signInManager, ITokenHandler tokenHandler)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenHandler = tokenHandler;
        }

        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            AP.AppUser user = await _userManager.FindByNameAsync(request.UsernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(request.UsernameOrEmail);
            if (user == null)
                throw new NotFoundUserException();

            SignInResult result = await  _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if(result.Succeeded) //bşrlı
            {
                Token token = _tokenHandler.CreateAccessToken(5);
                return new LoginUserSuccessCommandResponse()
                {
                    Token = token
                };
            }
            //return new LoginUserErrorCommandResponse()
            //{
            //    Message = "kullanici adi veya sifre hatalidir."
            //};

            throw new AuthenticationErrorException();
        }
    }
}
