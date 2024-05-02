using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ETicaretAPI.Application.Features.Commands.Product.UpdateProduct;
using Microsoft.Extensions.Logging;

namespace ETicaretAPI.Application.Features.Queries.Product.GetAllProduct
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQueryRequest, GetAllProductQueryResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly ILogger<UpdateProductCommandHandler> _logger;

        public GetAllProductQueryHandler(IProductReadRepository productReadRepository, ILogger<UpdateProductCommandHandler> logger)
        {
            _productReadRepository = productReadRepository;
            _logger = logger;
        }
        public async Task<GetAllProductQueryResponse> Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all products");
            var totalCount = _productReadRepository.GetAll(false).Count();
            var products = _productReadRepository.GetAll(false)
                                  .Skip(request.Page * request.Size)
                                  .Take(request.Size)
                                  .Select(p => new
                                  {
                                      p.Id,
                                      p.Name,
                                      p.Stock,
                                      p.Price,
                                      p.CreatedDate,
                                      p.UpdatedDate
                                  }).ToList(); // sayfalama mantığı 3*5 + 5  = 20

            return new()
            {
                Products = products,
                TotalCount = totalCount
            };

        }
    }
}
