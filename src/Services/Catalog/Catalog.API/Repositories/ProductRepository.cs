using Catalog.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IConnectionMultiplexer _redisCache;

        public ProductRepository(IConnectionMultiplexer redisCache)
        {
            this._redisCache = redisCache ?? throw new ArgumentNullException(nameof(ProductRepository));

            var db = _redisCache.GetDatabase();
            var productHashEntries = db.HashGetAll("hashProducts");

            if ( productHashEntries.Length == 0)
            {
                foreach (Product product in SeedProducts())
                {
                    db.HashSet("hashProducts", new HashEntry[] { new HashEntry(product.Id, JsonConvert.SerializeObject(product)) });
                }
            }

        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var db = _redisCache.GetDatabase();
            var productHashEntries = await db.HashGetAllAsync("hashProducts");
            if (productHashEntries.Length > 0)
            {
                return Array.ConvertAll(productHashEntries, val => JsonConvert.DeserializeObject<Product>(val.Value)).ToList();
            }

            return null;
        }

        public async Task<Product> GetProduct(string id)
        {
            var db = _redisCache.GetDatabase();
            var product = await db.HashGetAsync("hashProducts", id);

            if (!string.IsNullOrEmpty(product))
            {
                return JsonConvert.DeserializeObject<Product>(product);
            }

            return null;
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            var db = _redisCache.GetDatabase();
            var productHashEntries = await db.HashGetAllAsync("hashProducts");
            if (productHashEntries.Length > 0)
            {
                var products =  Array.ConvertAll(productHashEntries, val => JsonConvert.DeserializeObject<Product>(val.Value)).ToList();
                return products.Where(x => x.Name == name).ToList();
            }

            return null;

        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
        {
            var db = _redisCache.GetDatabase();
            var productHashEntries = await db.HashGetAllAsync("hashProducts");
            if (productHashEntries.Length > 0)
            {
                var products = Array.ConvertAll(productHashEntries, val => JsonConvert.DeserializeObject<Product>(val.Value)).ToList();
                return products.Where(x => x.Category == categoryName).ToList();
            }

            return null;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentOutOfRangeException(nameof(product));
            }

            var db = _redisCache.GetDatabase();

            product.Id = Guid.NewGuid().ToString();

            await db.HashSetAsync("hashProducts", new HashEntry[] { new HashEntry(product.Id, JsonConvert.SerializeObject(product)) });

            return await GetProduct(product.Id);
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentOutOfRangeException(nameof(product));
            }

            var db = _redisCache.GetDatabase();

            await db.HashSetAsync("hashProducts", new HashEntry[] { new HashEntry(product.Id, JsonConvert.SerializeObject(product)) });

            return await GetProduct(product.Id);
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var db = _redisCache.GetDatabase();
            await db.HashDeleteAsync("hashProducts",new RedisValue(id));
            return true;
        }

        private static IEnumerable<Product> SeedProducts()
        {
            return new List<Product>()
            {
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f5",
                    Name = "IPhone X",
                    Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
                    ImageFile = "product-1.png",
                    Price = 950.00M,
                    Category = "Smart Phone"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f6",
                    Name = "Samsung 10",
                    Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
                    ImageFile = "product-2.png",
                    Price = 840.00M,
                    Category = "Smart Phone"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f7",
                    Name = "Huawei Plus",
                    Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
                    ImageFile = "product-3.png",
                    Price = 650.00M,
                    Category = "White Appliances"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f8",
                    Name = "Xiaomi Mi 9",
                    Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
                    ImageFile = "product-4.png",
                    Price = 470.00M,
                    Category = "White Appliances"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f9",
                    Name = "HTC U11+ Plus",
                    Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
                    ImageFile = "product-5.png",
                    Price = 380.00M,
                    Category = "Smart Phone"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47fa",
                    Name = "LG G7 ThinQ",
                    Summary = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus. Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, tenetur natus doloremque laborum quos iste ipsum rerum obcaecati impedit odit illo dolorum ab tempora nihil dicta earum fugiat. Temporibus, voluptatibus.",
                    ImageFile = "product-6.png",
                    Price = 240.00M,
                    Category = "Home Kitchen"
                }
            };
        }

    }
}
