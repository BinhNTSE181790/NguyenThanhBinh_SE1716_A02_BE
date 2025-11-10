using Repository.DTOs;
using static Repository.DTOs.NewsArticleDTO;

namespace Service.Interfaces
{
    public interface ITagService
    {
        Task<APIResponse<List<TagInfo>>> GetAllTagsAsync();
    }
}
