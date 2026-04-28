using Microsoft.AspNetCore.Mvc.Razor;

namespace Carvia.Common;

public class FeatureViewLocationExpander : IViewLocationExpander
{
  public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
  {
    // {0} = Action, {1} = Controller
    return new[]
    {
      "/Features/{1}/{0}.cshtml",
      "/Features/Shared/{0}.cshtml",
    }.Concat(viewLocations);
  }

  public void PopulateValues(ViewLocationExpanderContext context)
  {

  }
}
