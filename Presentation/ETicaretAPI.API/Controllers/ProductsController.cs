using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.Services;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using F = ETicaretAPI.Domain.Entities;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly private IProductWriteRepository  _productWriteRepository;
        readonly private IProductReadRepository _productReadRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        readonly IFileService _fileService;
        readonly IFileReadRepository _fileReadRepository;
        readonly IFileWriteRepository _fileWriteRepository;
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository; 
        readonly IProductImageFileReadRepository _productImageFileReadRepository;
        readonly IInvoiceFileReadRepository _invoiceFileReadRepository;
        readonly IInvoiceFileWriteRepository _invoiceFileWriteRepository;

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IWebHostEnvironment webHostEnvironment, IFileService fileService, IFileReadRepository fileReadRepository, IFileWriteRepository fileWriteRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IProductImageFileReadRepository productImageFileReadRepository, IInvoiceFileReadRepository invoiceFileReadRepository, IInvoiceFileWriteRepository invoiceFileWriteRepository)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
            _fileReadRepository = fileReadRepository;
            _fileWriteRepository = fileWriteRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _productImageFileReadRepository = productImageFileReadRepository;
            _invoiceFileReadRepository = invoiceFileReadRepository;
            _invoiceFileWriteRepository = invoiceFileWriteRepository;
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
        public async Task<ActionResult> Get([FromQuery]Pagination pagination)
        {
            await Task.Delay(1500);
            var totalCount = _productReadRepository.GetAll(false).Count();
            var products= _productReadRepository.GetAll(false).Skip(pagination.Page * pagination.Size).Take(pagination.Size).Select(p => new
            {
                p.Id,
                p.Name,
                p.Stock,
                p.Price,
                p.CreatedDate,
                p.UpdatedDate
            }).ToList(); // sayfalama mantığı 3*5 + 5  = 20
            return Ok(new
            {
                totalCount,
                products
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id )
        {
            return Ok(await _productReadRepository.GetByIdAsync(id,false));
        }

        [HttpPost]
        public async Task<ActionResult> Post(VM_Create_Product model)
        {
            await _productWriteRepository.AddAsync(new()
            {
                Name= model.Name,
                Price= model.Price,
                Stock= model.Stock,
            });
            await _productWriteRepository.SaveAsync();
            return StatusCode((int)HttpStatusCode.Created); //httpde 201 
        }

        [HttpPut] 
        public async Task<ActionResult> Put(VM_Update_Product model)
        {
           Product product = await _productReadRepository.GetByIdAsync(model.Id);
           product.Price = model.Price;
           product.Stock = model.Stock;
            product.Name = model.Name;
            await _productWriteRepository.SaveAsync();
                    
            return Ok();
        
        }

        [HttpDelete("{id}")]
        public  async Task<ActionResult> Delete(string id )
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveAsync();
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload()
        {
            //var datas = await _fileService.UploadAsync("resource/product-images", Request.Form.Files);
            //await _productImageFileWriteRepository.AddRangeAsync(datas.Select(d => new ProductImageFile()
            //{
            //    FileName = d.fileName,
            //    Path = d.path
            //}).ToList());
            //await _productImageFileWriteRepository.SaveAsync();

            //var datas = await _fileService.UploadAsync("resource/invoices", Request.Form.Files);
            //await _invoiceFileWriteRepository.AddRangeAsync(datas.Select(d => new InvoiceFile()
            //{
            //    FileName = d.fileName,
            //    Path = d.path,
            //    Price = new Random().Next()
            //}).ToList()); ;

            var datas = await _fileService.UploadAsync("resource/files", Request.Form.Files);
            await _fileWriteRepository.AddRangeAsync(datas.Select(d => new F.File()
            {
                FileName = d.fileName,
                Path = d.path,
            }).ToList()); ;
            await _fileWriteRepository.SaveAsync();
            return Ok();



            //string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath,"resource/product-images");

            //if(!Directory.Exists(uploadPath))
            //   Directory.CreateDirectory(uploadPath);

            //Random  r = new Random();
            //foreach (IFormFile file in Request.Form.Files)
            //{   
            //    string fullPath = Path.Combine(uploadPath,$"{r.Next()}{Path.GetExtension(file.FileName)}");
            //    using FileStream fileStream = new(fullPath,FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);
            //    await file.CopyToAsync(fileStream);
            //    await fileStream.FlushAsync();
            //}
            //return  Ok();

            //28ders --


        }
    }
}
