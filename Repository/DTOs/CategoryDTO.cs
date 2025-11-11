namespace Repository.DTOs
{
    public class CategoryDTO
    {
        public class CategoryResponse
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public string? CategoryDesciption { get; set; }
            public int? ParentCategoryId { get; set; }
            public string? ParentCategoryName { get; set; }
            public bool IsActive { get; set; }
        }

        public class CreateCategoryRequest
        {
            public string CategoryName { get; set; } = string.Empty;
            public string? CategoryDesciption { get; set; }
            public int? ParentCategoryId { get; set; }
            public bool IsActive { get; set; } = true;
        }

        public class UpdateCategoryRequest
        {
            public string CategoryName { get; set; } = string.Empty;
            public string? CategoryDesciption { get; set; }
            public int? ParentCategoryId { get; set; }
            // IsActive không được update qua API Update, chỉ có thể thay đổi qua Delete (soft delete)
        }
    }
}
