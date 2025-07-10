using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Entities;

namespace HRM_AI.Services.Common
{
    public class Pagination<T> where T : BaseEntity
    {
        public Pagination(List<T> data, int currentPage, int pageSize, int totalCount)
        {
            Data = data;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public List<T> Data { get; }
        public int CurrentPage { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
    }
}
