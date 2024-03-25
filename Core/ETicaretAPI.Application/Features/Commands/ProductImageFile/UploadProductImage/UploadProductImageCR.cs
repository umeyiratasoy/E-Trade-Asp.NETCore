using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage
{
    ///UploadProductImageCR : UploadProductImageCommandRequest
    public class UploadProductImageCR:IRequest<UploadProductImageCRP>
    {
        public string Id { get; set; }
        public IFormFileCollection? Files { get; set; }


    }
}
