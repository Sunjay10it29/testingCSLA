﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Data
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    public DataPortalController()
    {
      UseTextSerialization = true;
    }

    [HttpGet]
    public string Get()
    {
      return "Running";
    }

    public override Task PostAsync([FromQuery] string operation)
    {
      return base.PostAsync(operation);
    }
  }
}