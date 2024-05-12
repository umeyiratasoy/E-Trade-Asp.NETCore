using ETicaretAPI.Application.Abstractions.Hubs;
using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Product.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommandRequest, CreateProductCommandResponse>
    {
        //readonly IProductWriteRepository _productWriteRepository;
        //readonly IProductHubService _productHubService;

        //public CreateProductCommandHandler(IProductWriteRepository productWriteRepository, IProductHubService productHubService)
        //{
        //    _productWriteRepository = productWriteRepository;
        //    _productHubService = productHubService;
        //}

        readonly IProductService _productService;
        readonly IProductHubService _productHubService;

        public CreateProductCommandHandler(IProductService productService, IProductHubService productHubService)
        {
            _productService = productService;
            _productHubService = productHubService;
        }

        public async Task<CreateProductCommandResponse> Handle(CreateProductCommandRequest request, CancellationToken cancellationToken)
        {
            await _productService.CreateAsync(new()
            {
                Name = request.Name,
                Price = request.Price,
                Stock = request.Stock,
            });
            await _productHubService.ProductAddedMessageAsync($"{request.Name} isminde ürün eklenmiştir.");
            return new();
        }
    }
}
