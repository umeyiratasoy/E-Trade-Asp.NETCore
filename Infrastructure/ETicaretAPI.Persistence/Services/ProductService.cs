using ETicaretAPI.Application.Abstractions.Hubs;
using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.Validators.Product;
using ETicaretAPI.Application.ViewModels.Baskets;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETicaretAPI.Persistence.Repositories;

namespace ETicaretAPI.Persistence.Services
{
    public class ProductService : IProductService
    {
        readonly IProductWriteRepository _productWriteRepository;
        readonly IProductReadRepository _productReadRepository;
        readonly CreateProductValidator _createProductValidator;
        readonly IQRCodeService _qrCodeService;

        public ProductService(IProductWriteRepository productWriteRepository, CreateProductValidator createProductValidator, IQRCodeService qrCodeService, IProductReadRepository productReadRepository)
        {
            _productWriteRepository = productWriteRepository;
            _createProductValidator = createProductValidator;
            _qrCodeService = qrCodeService;
            _productReadRepository = productReadRepository;
        }
        public async Task CreateAsync(VM_Create_Product model)
        {
            // Modeli doğrula
            var validationResult = await _createProductValidator.ValidateAsync(model);

            // Doğrulama başarısızsa istisna fırlat
            if (!validationResult.IsValid)
            {
                    throw new ValidationException(validationResult.Errors);
            }

            // Doğrulama başarılı ise ürünü ekleyip kaydet
            await _productWriteRepository.AddAsync(new Product
            {
                Name = model.Name,
                Price = model.Price,
                Stock = model.Stock,
            });
            await _productWriteRepository.SaveAsync();
        }

        public async Task<byte[]> QrCodeToProductAsync(string productId)
        {
            Product product = await _productReadRepository.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            var plainObject = new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Stock,
                product.CreatedDate
            };
            string plainText = JsonSerializer.Serialize(plainObject);

            return _qrCodeService.GenerateQRCode(plainText);
        }
    }
}
