using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.RemoveProductImage
{
    // RemoveProductImageCR : RemoveProductImageCommandRequest
    public class RemoveProductImageCR : IRequest<RemoveProductImageCRP>
    {
        public string Id { get; set; }
        public string? ImageId { get; set; }
    }
}
