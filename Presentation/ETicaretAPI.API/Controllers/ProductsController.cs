using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using F = ETicaretAPI.Domain.Entities;
using System.Net;
using ETicaretAPI.Application.Abstractions.Storage;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MediatR;
using ETicaretAPI.Application.Features.Commands.Product.CreateProduct;
using ETicaretAPI.Application.Features.Queries.Product.GetAllProduct;
using ETicaretAPI.Application.Features.Queries.Product.GetByIdProduct;
using ETicaretAPI.Application.Features.Commands.Product.UpdateProduct;
using ETicaretAPI.Application.Features.Commands.Product.RemoveProduct;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.RemoveProductImage;
using ETicaretAPI.Application.Features.Queries.ProductImageFile.GetProductImages;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly private IProductWriteRepository _productWriteRepository;
        readonly private IProductReadRepository _productReadRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        readonly IFileReadRepository _fileReadRepository;
        readonly IFileWriteRepository _fileWriteRepository;
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        readonly IProductImageFileReadRepository _productImageFileReadRepository;
        readonly IInvoiceFileReadRepository _invoiceFileReadRepository;
        readonly IInvoiceFileWriteRepository _invoiceFileWriteRepository;
        readonly IStorageService _storageService;
        readonly IConfiguration _configuration;

        readonly IMediator _mediator;

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IWebHostEnvironment webHostEnvironment, IFileReadRepository fileReadRepository, IFileWriteRepository fileWriteRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IProductImageFileReadRepository productImageFileReadRepository, IInvoiceFileReadRepository invoiceFileReadRepository, IInvoiceFileWriteRepository invoiceFileWriteRepository, IStorageService storageService, IConfiguration configuration, IMediator mediator)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _webHostEnvironment = webHostEnvironment;
            _fileReadRepository = fileReadRepository;
            _fileWriteRepository = fileWriteRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _productImageFileReadRepository = productImageFileReadRepository;
            _invoiceFileReadRepository = invoiceFileReadRepository;
            _invoiceFileWriteRepository = invoiceFileWriteRepository;
            _storageService = storageService;
            _configuration = configuration;
            _mediator = mediator;
        }




        //[HttpGet]
        //public async Task Gett()
        //{
        //    await _productWriteRepository.AddRangeAsync(new()
        //    {
        //        new() {Id = Guid.NewGuid(), Name = "Product1", Price = 100, CreatedDate = DateTime.UtcNow, Stock=1},
        //        new() {Id = Guid.NewGuid(), Name = "Product2", Price = 200, CreatedDate = DateTime.UtcNow, Stock=1},
        //        new() {Id = Guid.NewGuid(), Name = "Product3", Price = 300, CreatedDate = DateTime.UtcNow, Stock=1},
        //        new() {Id = Guid.NewGuid(), Name = "Product4", Price = 400, CreatedDate = DateTime.UtcNow, Stock=1}
        //    }
        //        );
        //    await _productWriteRepository.SaveAsync();
        //}

        //    //Product p = await _productReadRepository.GetByIdAsync("", false);
        //    //p.Name = "ahmet";
        //    //await _productWriteRepository.SaveAsync();

        //    //await _productWriteRepository.AddAsync(new() { Name = "C#  Product", Price = 1.200F, Stock = 10, CreatedDate = DateTime.UtcNow });
        //    //await _productWriteRepository.SaveAsync();

        //    Product product = await _productReadRepository.GetByIdAsync("2c14983f-8cc6-443d-a075-a9e5406d3268");
        //    product.Stock = 20;
        //    await _productWriteRepository.SaveAsync();
        //}




        //[HttpGet]
        //public async Task Get()
        //{
        //    //var customerId = Guid.NewGuid();
        //    //await _customerWriteRepository.AddAsync(new() {Id = customerId, Name="Ahmet" });

        //    //await _orderWriteRepository.AddAsync(new() { Description = "bla bla ", Address = "erzurum", CustomerId = customerId});
        //    //await _orderWriteRepository.AddAsync(new() { Description = "bla bla 2 ", Address = "samsun",CustomerId = customerId });
        //    //await _productWriteRepository.SaveAsync();

        //    Order order = await _orderReadRepository.GetByIdAsync("9524d9eb-4f48-4b27-b287-5818456cef11");
        //    order.Address = "bafra";
        //    await _orderWriteRepository.SaveAsync();
        //}

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
        {
            //await Task.Delay(1500); // bekleme süresi
            GetAllProductQueryResponse response = await _mediator.Send(getAllProductQueryRequest);
            return Ok(response);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult> Get([FromRoute]GetByIdProductQueryRequest getByIdProductQueryRequest)
        {
            GetByIdProductQueryResponse response = await _mediator.Send(getByIdProductQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreateProductCommandRequest createProductCommandRequest)
        {
            CreateProductCommandResponse response = await _mediator.Send(createProductCommandRequest);
            return StatusCode((int)HttpStatusCode.Created); //httpde 201 
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody]UpdateProductCommandRequest updateProductCommandRequest)
        {

            UpdateProductCommandResponse response = await _mediator.Send(updateProductCommandRequest);
            return Ok();
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete([FromRoute] RemoveProductCommandRequest removeProductCommandRequest)
        {
            RemoveProductCommandResponse response =  await _mediator.Send(removeProductCommandRequest);
            return Ok();
        }



        [HttpPost("[action]")]
        public async Task<IActionResult> Upload([FromQuery] UploadProductImageCR uploadProductImageCommandRequest)
        {
            uploadProductImageCommandRequest.Files = Request.Form.Files;
            UploadProductImageCRP response = await _mediator.Send(uploadProductImageCommandRequest);
            return Ok();
        }



        [HttpGet("[action]/{id}")]
        public async Task<ActionResult> GetProductImages([FromRoute]GetProductImagesQR getProductImagesQR)
        {
            List<GetProductImagesQRP> response = await _mediator.Send(getProductImagesQR);
            return Ok(response);
        }



        [HttpDelete("[action]/{id}")] 
        public async Task<ActionResult> DeleteProductImage([FromRoute]RemoveProductImageCR removeProductImageCR, [FromQuery] string imageId)
        {
           removeProductImageCR.ImageId =  imageId;
           RemoveProductImageCRP response = await _mediator.Send(removeProductImageCR);
           return Ok();    


        }


    }
}
