namespace Repository.DTOs
{
    public class NewsArticleDTO
    {
        public class NewsArticleResponse
        {
            public int NewsArticleId { get; set; }
            public string NewsTitle { get; set; } = string.Empty;
            public string? Headline { get; set; }
            public DateTime CreatedDate { get; set; }
            public string NewsContent { get; set; } = string.Empty;
            public string? NewsSource { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public int NewsStatus { get; set; }
            public int CreatedById { get; set; }
            public string CreatedByName { get; set; } = string.Empty;
            public int? UpdatedById { get; set; }
            public string? UpdatedByName { get; set; }
            public DateTime? ModifiedDate { get; set; }
            public List<TagInfo>? Tags { get; set; }
        }

        public class TagInfo
        {
            public int TagId { get; set; }
            public string TagName { get; set; } = string.Empty;
        }

        public class CreateNewsArticleRequest
        {
            public string NewsTitle { get; set; } = string.Empty;
            public string? Headline { get; set; }
            public string NewsContent { get; set; } = string.Empty;
            public string? NewsSource { get; set; }
            public int CategoryId { get; set; }
            public int NewsStatus { get; set; }
            public List<int>? TagIds { get; set; }
        }

        public class UpdateNewsArticleRequest
        {
            public string NewsTitle { get; set; } = string.Empty;
            public string? Headline { get; set; }
            public string NewsContent { get; set; } = string.Empty;
            public string? NewsSource { get; set; }
            public int CategoryId { get; set; }
            public int NewsStatus { get; set; }
            public List<int>? TagIds { get; set; }
        }

        public class NewsStatisticsRequest
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public class NewsStatisticsResponse
        {
            public DateTime Date { get; set; }
            public int TotalNews { get; set; }
            public int PublishedNews { get; set; }
            public int DraftNews { get; set; }
            public List<CategoryStatistics> CategoryBreakdown { get; set; } = new();
        }

        public class CategoryStatistics
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public int Count { get; set; }
        }
    }
}
