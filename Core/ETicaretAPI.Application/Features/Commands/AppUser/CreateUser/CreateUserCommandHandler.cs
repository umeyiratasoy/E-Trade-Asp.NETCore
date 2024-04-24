using U = ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ETicaretAPI.Application.Exceptions;

namespace ETicaretAPI.Application.Features.Commands.AppUser.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
    {
        readonly UserManager<U.AppUser> _userManager;

        public CreateUserCommandHandler(UserManager<U.AppUser> userManager)
        {
            _userManager = userManager;
        } 

        public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            IdentityResult result = await  _userManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.UserName,
                Email = request.Email,
                NameSurname = request.NameSurname
            }, request.Password);


            CreateUserCommandResponse response = new() { Succeeded = result.Succeeded };
            if (result.Succeeded)
                response.Message = "Kulanıcı başarıyla oluşturuldu!";
            else
                foreach (var error in result.Errors)
                    response.Message += $"{error.Code} - {error.Description}\n";
            return response;

            //throw new UserCreateFailedException();
        }
    }
}
