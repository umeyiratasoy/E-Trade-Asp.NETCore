using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.ProductImageFile.GetProductImages
{
    // GetProductImagesQR : GetProductImagesQueryRequest
    public class GetProductImagesQR : IRequest<List<GetProductImagesQRP>>
    {
        public string Id { get; set; }
    }
}
