using System.Collections.Generic;
using System.Linq;

namespace BlogClubeLeitura.Models
{
    public static class PaginatedListExtensions
    {
        public static PaginatedList<dynamic> ToDynamicList<T>(this PaginatedList<T> list)
        {
            var dynamicList = new PaginatedList<dynamic>(list.Cast<dynamic>().ToList(), list.Count, list.PageIndex, list.TotalPages);
            return dynamicList;
        }
    }
}