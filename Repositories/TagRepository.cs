using Microsoft.EntityFrameworkCore;
using TradexSample.Data;
using TradexSample.Models.Domain;

namespace TradexSample.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TagRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<Tag?> AddAsync(Tag tag)
        {
            await _dbContext.Tags.AddAsync(tag);
            await _dbContext.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var existingtag=await _dbContext.Tags.FindAsync(id);

            if (existingtag != null) 
            {
                _dbContext.Tags.Remove(existingtag);
                await _dbContext.SaveChangesAsync();
                return existingtag;
            }
            return null;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            var tags = await _dbContext.Tags.ToListAsync();
            return tags;
        }

        public async Task<Tag?> GetAsync(Guid id)
        {
            var tag = await _dbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);
            return tag;
        }

        public async Task<Tag?> UpdateAsync(Tag tag)
        {
           var existingTag= await _dbContext.Tags.FindAsync(tag.Id);
            if (existingTag != null) 
            {
                existingTag.Name = tag.Name;
                existingTag.DisplayName = tag.DisplayName;

                await _dbContext.SaveChangesAsync();

                return existingTag;
            }
            return null;
        }
    }
}
