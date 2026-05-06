using Microsoft.AspNetCore.Mvc.Razor;

namespace Carvia.Infrastructure.Razor;

public class FeatureViewLocationExpander : IViewLocationExpander
{
  public IEnumerable<string> ExpandViewLocations(
    ViewLocationExpanderContext context,
    IEnumerable<string> viewLocations)
  {
    // {0} = Action Name
    // {1} = Controller Name
    return new[]
    {
      "/Features/{1}/Views/{0}.cshtml",      // e.g., /Features/Cars/Views/Index.cshtml
      "/Features/Shared/Views/{0}.cshtml",   // e.g., /Features/Shared/Views/_Layout.cshtml
      "/Views/Shared/{0}.cshtml"             // Fallback for global shared views
    }.Concat(viewLocations);
  }

  public void PopulateValues(ViewLocationExpanderContext context)
  {
    // No additional values needed for this implementation
  }
}