using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace AtlasOnlineCheckBypass.Controllers
{
  [Route("api/[controller]")]
  public class BypassController:Controller
  {
    private string _cacheBypasskey = "BypassSettings";
    private readonly IMemoryCache _memoryCache;

    public BypassController(IMemoryCache memoryCache)
    {
      _memoryCache = memoryCache;
    }

    [HttpGet("[action]")]
    public IActionResult CurrentSettings()
    {
      var cacheValue = GetBypassSettingsFromCache();

      return Ok(cacheValue);
    }

    [HttpPut("[action]")]
    public IActionResult SetBypassSettings([FromBody] BypassSettings value)
    {
      SaveBypassSettingInCache(value);

      return Ok();
    }

    [HttpGet("[action]")]
    public IActionResult GetCreditCheck()
    {
      var cacheValue = GetBypassSettingsFromCache();

      return Ok(cacheValue.CreditCheck);
    }

    [HttpGet("[action]")]
    public IActionResult GetAvsCheck()
    {
      var cacheValue = GetBypassSettingsFromCache();

      return Ok(cacheValue.AvsCheck);
    }

    [HttpGet("[action]")]
    public IActionResult GetAffordabilityCheck()
    {
      var cacheValue = GetBypassSettingsFromCache();

      return Ok(cacheValue.AffordabilityCheck);
    }

    [HttpGet("[action]")]
    public IActionResult GetFraudCheck()
    {
      var cacheValue = GetBypassSettingsFromCache();

      return Ok(cacheValue.FraudCheck);
    }

    [HttpGet("[action]")]
    public IActionResult GetXdsCheck()
    {
      var cacheValue = GetBypassSettingsFromCache();

      return Ok(cacheValue.XdsCheck);
    }

    private BypassSettings GetBypassSettingsFromCache()
    {
      BypassSettings cacheValue;
      if (!_memoryCache.TryGetValue(_cacheBypasskey, out cacheValue))
      {
        // Key not in cache, so get data.
        cacheValue = new BypassSettings();
        SaveBypassSettingInCache(cacheValue);
      }
      return cacheValue;
    }

    private void SaveBypassSettingInCache(BypassSettings cacheValue)
    {
      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
          // Keep in cache for this time, reset time if accessed.
          .SetSlidingExpiration(TimeSpan.MaxValue);

      // Save data in cache.
      _memoryCache.Set(_cacheBypasskey, cacheValue, cacheEntryOptions);
    }


    public class BypassSettings
    {
      public bool CreditCheck;
      public bool AvsCheck;
      public bool AffordabilityCheck;
      public bool FraudCheck;
      public bool XdsCheck;
    }
  }


}

