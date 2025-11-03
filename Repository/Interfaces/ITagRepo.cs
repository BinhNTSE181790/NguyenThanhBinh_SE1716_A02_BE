using Repository.Entities;

namespace Repository.Interfaces
{
    public interface ITagRepo : IGenericRepo<Tag>
    {
        Task<List<Tag>> GetTagsByIdsAsync(List<int> tagIds);
    }
}
