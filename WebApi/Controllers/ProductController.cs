using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.ProductCatalog.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductCatalogService _productCatalogService;

        public ProductController()
        {
            var serviceProxyFactory = new ServiceProxyFactory(channel =>
                new FabricTransportServiceRemotingClientFactory());

            _productCatalogService = serviceProxyFactory
                .CreateServiceProxy<IProductCatalogService>(
                    new Uri("fabric:/ECommerce/ECommerce.ProductCatalog"),
                    new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _productCatalogService.GetProductsAsync();
        }

        [HttpPost]
        public async Task CreateProductAsync([FromBody]Product product)
        {
            await _productCatalogService.AddProductAsync(product);
        }
    }
}