using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IProductService
    {
        Task CreateAsync(VM_Create_Product model);
    }
}
