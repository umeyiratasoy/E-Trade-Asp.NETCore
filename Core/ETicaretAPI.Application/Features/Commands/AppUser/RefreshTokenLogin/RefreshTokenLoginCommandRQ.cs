using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.RefreshTokenLogin
{
    //Request
    public class RefreshTokenLoginCommandRQ:IRequest<RefreshTokenLoginCommandRP>
    {
        public string RefreshToken { get; set; }
    }
}
