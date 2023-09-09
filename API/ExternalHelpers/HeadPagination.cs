
// Object we return inside the HTTP response header. Client can retrived pagination detail from this object (header)

namespace API.ExternalHelpers
{
  public class HeadPagination
  {
    public HeadPagination(int pageCurrent, int pageTotal, int itemsTotal, int perPageItems)
    {
      currentPage = pageCurrent;
      totalPages = pageTotal;
      totalItems = itemsTotal;
      itemsPerPage = perPageItems;
    }

    public int currentPage { get; set; }
    public int itemsPerPage { get; set; }
    public int totalItems { get; set; }
    public int totalPages { get; set; }


  }
}