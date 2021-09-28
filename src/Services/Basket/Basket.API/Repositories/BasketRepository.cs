using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        }

        public async Task DeleteBasket(string UserName)
        {
            await this._redisCache.RemoveAsync(UserName);
        }

        public async Task<ShoppingCart> GetBasket(string UserName)
        {
            var basket = await this._redisCache.GetStringAsync(UserName);
            if (String.IsNullOrEmpty(basket))
                return null;
            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await this._redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
            return await this.GetBasket(basket.UserName);
        }
    }
}
