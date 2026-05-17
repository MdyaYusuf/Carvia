using Carvia.Infrastructure.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Carvia.Features.Shared;

public class SharedController : BaseController
{
  [Route("Shared/Error")]
  public IActionResult Error()
  {
    return View();
  }

  [HttpGet("legal")]
  public IActionResult Legal()
  {
    ViewData["IsHeroPage"] = false;

    return View("~/Features/Shared/Views/Legal.cshtml");
  }
}
