using Repository.Context;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FunewsManagementSystemDbContext _context;
        public IAccountRepo AccountRepo { get; private set; }
        public ICategoryRepo CategoryRepo { get; private set; }
        public INewsArticleRepo NewsArticleRepo { get; private set; }
        public ITagRepo TagRepo { get; private set; }

        public UnitOfWork(
            FunewsManagementSystemDbContext context, 
            IAccountRepo accountRepo,
            ICategoryRepo categoryRepo,
            INewsArticleRepo newsArticleRepo,
            ITagRepo tagRepo)
        {
            _context = context;
            AccountRepo = accountRepo;
            CategoryRepo = categoryRepo;
            NewsArticleRepo = newsArticleRepo;
            TagRepo = tagRepo;
        }

        public void Dispose()
        {
            _context.Dispose();
        }  
    }
}
