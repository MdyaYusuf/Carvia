using Carvia.Core.Exceptions;
using Carvia.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Carvia.Infrastructure.Controllers;

public abstract class BaseController : Controller
{
  [NonAction]
  public IActionResult CreateActionResult<T>(ReturnModel<T> result)
  {
    if (result.StatusCode == StatusCodes.Status204NoContent)
    {
      return new NoContentResult();
    }

    return new ObjectResult(result)
    {
      StatusCode = result.StatusCode
    };
  }

  [NonAction]
  protected Guid GetUserId()
  {
    string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid guid))
    {
      throw new AuthorizationException("İşlem için giriş yapmanız gerekmektedir.");
    }

    return guid;
  }

  [NonAction]
  protected Guid? TryGetUserId()
  {
    string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (Guid.TryParse(userId, out var id))
    {
      return id;
    }

    return null;
  }

  [NonAction]
  protected string GetUserRole()
  {
    return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
  }
}
