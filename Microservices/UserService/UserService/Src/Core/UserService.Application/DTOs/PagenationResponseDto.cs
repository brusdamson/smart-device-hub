using System.Collections.Generic;

namespace UserService.Application.DTOs
{
    public class PagenationResponseDto<T>
    {
        public PagenationResponseDto(List<T> data, int count, int pageNumber, int pageSize)
        {
            Data = data;
            Count = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public List<T> Data { get; set; }
        public int Count { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
