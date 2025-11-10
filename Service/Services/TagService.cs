using Repository.DTOs;
using Repository.Interfaces;
using Service.Interfaces;
using static Repository.DTOs.NewsArticleDTO;

namespace Service.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _uow;

        public TagService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task<APIResponse<List<TagInfo>>> GetAllTagsAsync()
        {
            try
            {
                var tags = await _uow.TagRepo.GetAllAsync();
                var tagInfos = tags.Select(t => new TagInfo
                {
                    TagId = t.TagId,
                    TagName = t.TagName
                }).ToList();

                return APIResponse<List<TagInfo>>.Ok(tagInfos, "Tags retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<List<TagInfo>>.Fail($"Error retrieving tags: {ex.Message}", "500");
            }
        }
    }
}
