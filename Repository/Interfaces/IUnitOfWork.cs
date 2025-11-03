using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepo AccountRepo { get; }
        ICategoryRepo CategoryRepo { get; }
        INewsArticleRepo NewsArticleRepo { get; }
        ITagRepo TagRepo { get; }
    }
}
