
// Object we return inside the HTTP response header. Client can retrived pagination detail from this object (header)

namespace API.ExternalHelpers
{
  public class HeadPagination
  {
    public HeadPagination(int pageCurrent, int pageTotal, int itemsTotal, int perPageItems)
    {
      PageCurrent = pageCurrent;
      PageTotal = pageTotal;
      ItemsTotal = itemsTotal;
      PerPageItems = perPageItems;
    }

    public int PageCurrent { get; set; }
    public int PageTotal { get; set; }
    public int ItemsTotal { get; set; }
    public int PerPageItems { get; set; }


  }
}