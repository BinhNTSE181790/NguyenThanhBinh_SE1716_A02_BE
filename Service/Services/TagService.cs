using Repository.DTOs;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;
using static Repository.DTOs.NewsArticleDTO;
using static Repository.DTOs.TagDTO;

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
                var allTags = await _uow.TagRepo.GetAllAsync();
                // Lấy tất cả tags (không có IsActive nữa)
                var tagInfos = allTags.Select(t => new TagInfo
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

        public async Task<APIResponse<List<TagResponse>>> GetAllTagsForManagementAsync()
        {
            try
            {
                var allTags = await _uow.TagRepo.GetAllAsync();
                var tagResponses = allTags.Select(t => new TagResponse
                {
                    TagId = t.TagId,
                    TagName = t.TagName,
                    Note = t.Note
                }).ToList();

                return APIResponse<List<TagResponse>>.Ok(tagResponses, "Tags retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<List<TagResponse>>.Fail($"Error retrieving tags: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<TagResponse>> GetTagByIdAsync(int tagId)
        {
            try
            {
                var tag = await _uow.TagRepo.GetByIdAsync(tagId);
                if (tag == null)
                {
                    return APIResponse<TagResponse>.Fail("Tag not found", "404");
                }

                var tagResponse = new TagResponse
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName,
                    Note = tag.Note
                };

                return APIResponse<TagResponse>.Ok(tagResponse, "Tag retrieved successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<TagResponse>.Fail($"Error retrieving tag: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<TagResponse>> CreateTagAsync(CreateTagRequest request)
        {
            try
            {
                var newTag = new Tag
                {
                    TagName = request.TagName,
                    Note = request.Note
                };

                await _uow.TagRepo.CreateAsync(newTag);

                var tagResponse = new TagResponse
                {
                    TagId = newTag.TagId,
                    TagName = newTag.TagName,
                    Note = newTag.Note
                };

                return APIResponse<TagResponse>.Ok(tagResponse, "Tag created successfully", "201");
            }
            catch (Exception ex)
            {
                return APIResponse<TagResponse>.Fail($"Error creating tag: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<TagResponse>> UpdateTagAsync(int tagId, UpdateTagRequest request)
        {
            try
            {
                var tag = await _uow.TagRepo.GetByIdAsync(tagId);
                if (tag == null)
                {
                    return APIResponse<TagResponse>.Fail("Tag not found", "404");
                }

                tag.TagName = request.TagName;
                tag.Note = request.Note;

                await _uow.TagRepo.UpdateAsync(tag);

                var tagResponse = new TagResponse
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName,
                    Note = tag.Note
                };

                return APIResponse<TagResponse>.Ok(tagResponse, "Tag updated successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<TagResponse>.Fail($"Error updating tag: {ex.Message}", "500");
            }
        }

        public async Task<APIResponse<string>> DeleteTagAsync(int tagId)
        {
            try
            {
                var tag = await _uow.TagRepo.GetByIdAsync(tagId);
                if (tag == null)
                {
                    return APIResponse<string>.Fail("Tag not found", "404");
                }

                // Hard delete - Tag không có soft delete nữa
                var result = await _uow.TagRepo.RemoveAsync(tag);
                
                if (!result)
                {
                    return APIResponse<string>.Fail("Failed to delete tag", "500");
                }

                return APIResponse<string>.Ok("Tag deleted successfully", "Tag deleted successfully", "200");
            }
            catch (Exception ex)
            {
                return APIResponse<string>.Fail($"Error deleting tag: {ex.Message}", "500");
            }
        }
    }
}
