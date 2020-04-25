using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.ProductCatalog
{
    class ServiceFabricProductRepository : IProductRepository
    {
        private readonly IReliableStateManager _stageManager;
        private const string ProductsKey = "products";

        public ServiceFabricProductRepository(IReliableStateManager stateManager)
        {
            _stageManager = stateManager;
        }

        public async Task AddProductAsync(Product product)
        {
            var productsState = await _stageManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>(ProductsKey);
            using var tx = _stageManager.CreateTransaction();
            await productsState.AddOrUpdateAsync(tx, product.Id, product, (id, current) => product);

            await tx.CommitAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            // get reference to products from state manager
            // get an enumerable from that data
            // get an enumerator from that
            // enumerate
            var productsState = await _stageManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>(ProductsKey);

            using var tx = _stageManager.CreateTransaction();
            var enumerableProducts = await productsState.CreateEnumerableAsync(tx);
            var productsEnumerator = enumerableProducts.GetAsyncEnumerator();

            var result = new List<Product>();

            while (await productsEnumerator.MoveNextAsync(CancellationToken.None))
            {
                result.Add(productsEnumerator.Current.Value);
            }

            return result;
        }
    }
}
