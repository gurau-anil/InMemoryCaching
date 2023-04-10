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
        private readonly IConfiguration _configuration;

        public CacheController(ILogger<CacheController> logger, IMemoryCache cache, IConfiguration configuration)
        {
            _logger = logger;
            _cache = cache;
            _configuration = configuration;
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
                AbsoluteExpiration = DateTime.Now.AddMinutes(_configuration.GetValue<double>("CacheExpiryOption:AbsoluteExpiration")),
                Priority = (CacheItemPriority) Convert.ToInt32(_configuration["CacheExpiryOption:CacheItemPriority"]),
                Size = Convert.ToInt32(_configuration["CacheExpiryOption:Size"]),
                SlidingExpiration = TimeSpan.FromMinutes(Convert.ToDouble(_configuration["CacheExpiryOption:SlidingExpiration"])),
            };
            _cache.Set(requestModel.key, requestModel.value, cacheExpiryOptions);
            return Ok();
        }
    }
}