using InMemoryCaching.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCaching.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheController> _logger;

        public CacheController(ILogger<CacheController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet("{key}")]
        public IActionResult GetCache(string key)
        {
            string retVal = string.Empty;
            _cache.TryGetValue(key, out retVal);
            if (String.IsNullOrEmpty(retVal))
            {
                return NotFound($"Value for the key : {key} not found.");
            }
            return Ok(retVal);
        }
        [HttpPost]
        public IActionResult SetCache(CacheRequestModel requestModel)
        {
            MemoryCacheEntryOptions cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(2),
                Priority = CacheItemPriority.High,
                Size = 1024,
                SlidingExpiration = TimeSpan.FromMinutes(2),
            };
            _cache.Set(requestModel.key, requestModel.value, cacheExpiryOptions);
            return Ok();
        }
    }
}