using Repository.DTOs;
using static Repository.DTOs.NewsArticleDTO;
using static Repository.DTOs.TagDTO;

namespace Service.Interfaces
{
    public interface ITagService
    {
        Task<APIResponse<List<TagInfo>>> GetAllTagsAsync();
        Task<APIResponse<List<TagResponse>>> GetAllTagsForManagementAsync();
        Task<APIResponse<TagResponse>> GetTagByIdAsync(int tagId);
        Task<APIResponse<TagResponse>> CreateTagAsync(CreateTagRequest request);
        Task<APIResponse<TagResponse>> UpdateTagAsync(int tagId, UpdateTagRequest request);
        Task<APIResponse<string>> DeleteTagAsync(int tagId);
    }
}
