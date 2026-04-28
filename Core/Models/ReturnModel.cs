namespace Carvia.Core.Models;

public class ReturnModel<T>
{
  public bool Success { get; set; }
  public string? Message { get; set; }
  public T? Data { get; set; }
  public int StatusCode { get; set; }
  public List<string>? Errors { get; set; }
}

public class NoData
{

}
