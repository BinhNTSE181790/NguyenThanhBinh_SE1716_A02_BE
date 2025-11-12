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
            public int Status { get; set; } // 1 = Active, 0 = Inactive
        }

        public class CreateCategoryRequest
        {
            public string CategoryName { get; set; } = string.Empty;
            public string? CategoryDesciption { get; set; }
            public int? ParentCategoryId { get; set; }
            public int Status { get; set; } = 1; // Default Active
        }

        public class UpdateCategoryRequest
        {
            public string CategoryName { get; set; } = string.Empty;
            public string? CategoryDesciption { get; set; }
            public int? ParentCategoryId { get; set; }
            // Status không được update qua API Update, chỉ có thể thay đổi qua Delete
        }
    }
}
