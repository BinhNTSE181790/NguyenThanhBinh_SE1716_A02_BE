namespace Repository.DTOs
{
    public class TagDTO
    {
        public class TagResponse
        {
            public int TagId { get; set; }
            public string TagName { get; set; } = string.Empty;
            public string? Note { get; set; }
        }

        public class CreateTagRequest
        {
            public string TagName { get; set; } = string.Empty;
            public string? Note { get; set; }
        }

        public class UpdateTagRequest
        {
            public string TagName { get; set; } = string.Empty;
            public string? Note { get; set; }
        }
    }
}
