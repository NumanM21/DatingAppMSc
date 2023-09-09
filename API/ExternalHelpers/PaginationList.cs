

using Microsoft.EntityFrameworkCore;

namespace API.ExternalHelpers
{
  // T is generic, can be any type (can work with any object) -> We specify the type when we use it!
  public class PaginationList<T> : List<T>
  {
    public PaginationList(IEnumerable<T> items, int pageNumber, int size, int count)
    {
      this.currentPage = pageNumber;
      this.totalPage = (int)Math.Ceiling(count / (double)size); //(10 / 3 = 3.333) -> 4 pages
      this.PageSize = size;
      this.totalCount = count;
      AddRange(items); // return all of our items when we call this method
    }

    public int currentPage { get; set; }
    public int totalPage { get; set; }
    public int PageSize { get; set; }
    public int totalCount { get; set; }


    // Pass this method our query -> Built up in memory by EF. Doesn't execute until we use one of the methods below
    public static async Task<PaginationList<T>> AsyncCreate(IQueryable<T> source, int pageNumber, int pageSize)
    {
      // count of items from our query (before pagination). Executed against our DB (all users counted without any filters)
      var count = await source.CountAsync();
      //pageNum -1 so we don't skip any users if we are on page 1

      //FIXME: Debug
      var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

      return new PaginationList<T>(items, pageNumber, pageSize, count);

    }

  }
}