﻿using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using AP = ETicaretAPI.Domain.Entities.Identity;
using ETicaretAPI.Application.DTOs.Facebook;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Apis.Auth;
using ETicaretAPI.Domain.Entities.Identity;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Features.Commands.AppUser.LoginUser;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Persistence.Services
{
    public class AuthService : IAuthService
    {
        readonly HttpClient _httpClient;
        readonly IConfiguration _configuration;
        readonly UserManager<AP.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly SignInManager<AP.AppUser> _signInManager;
        readonly IUserService _userService;


        public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration, UserManager<AP.AppUser> userManager, ITokenHandler tokenHandler, SignInManager<AppUser> signInManager, IUserService userService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _signInManager = signInManager;
            _userService = userService;
        }

        async Task<Token> CreateUserExternalAsync(AppUser user, string email,string name,UserLoginInfo info, int accessTokenLifeTime )
        {
            bool result = user != null;

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        UserName = email,
                        NameSurname = name
                    };
                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }
            if (result)
            {

                await _userManager.AddLoginAsync(user, info); //aspnetuserlogins


                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);
                await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 15);
                return token;

            }
            throw new Exception("Invalid exteral authentication");
        }



        public async Task<Token> FacebookLoginAsync(string authToken, int accessTokenLifeTime)
        {
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["ExternalLoginSettings:Facebook:Client_ID"]}&client_secret={_configuration["ExternalLoginSettings:Facebook:Client_Secret"]}&grant_type=client_credentials"); // facebookloginappid

            FacebookAccessTokenResponse? facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponse>(accessTokenResponse);
            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={authToken}&access_token={facebookAccessTokenResponse?.AccessToken}");

            FacebookAccessTokenValidation? validation = JsonSerializer.Deserialize<FacebookAccessTokenValidation>(userAccessTokenValidation);
            if (validation?.Data.IsValid != null)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={authToken}");
                FacebookUserInfoResponse? userInfo = JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoResponse);
                var info = new UserLoginInfo("FACEBOOK", validation.Data.UserId, "FACEBOOK");



                AP.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                return await CreateUserExternalAsync(user, userInfo.Email,userInfo.Name, info,accessTokenLifeTime);
                 
            }
            throw new Exception("Invalid exteral authentication");

        }

        public async Task<Token> GoogleLoginAsync(string idToken, int accessTokenLifeTime)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["ExternalLoginSettings:Google:Client_ID"] } //googleclientid
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            var info = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");



            AP.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            return await CreateUserExternalAsync(user, payload.Email, payload.Name, info, accessTokenLifeTime);
        }

        public async Task<Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime)
        {
            AP.AppUser user = await _userManager.FindByNameAsync(usernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(usernameOrEmail);
            if (user == null)
                throw new NotFoundUserException();

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded) //bşrlı
            {
                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);
                await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 15);

                return token;
            }

                throw new AuthenticationErrorException();
        }

        public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
        {
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user != null)
            {
                Token token = _tokenHandler.CreateAccessToken(15);
                await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 15);
                return token;
            }
            else
                throw new NotFoundUserException();

        }
    }
}
