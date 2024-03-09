﻿using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext nZWalksDbContext;

        public WalkRepository(NZWalksDbContext nZWalksDbContext)
        {
            this.nZWalksDbContext = nZWalksDbContext;
        }

        public async Task<Walk> AddAsync(Walk walk)
        {
            // assign new id
            walk.Id = Guid.NewGuid();
            await nZWalksDbContext.Walks.AddAsync(walk);
            await nZWalksDbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk> DeleteAsync(Guid id)
        {
            var existingWalk = await nZWalksDbContext.Walks.FindAsync(id);

            if (existingWalk == null)
            {
                return null;
            }
            nZWalksDbContext.Walks.Remove(existingWalk);
            await nZWalksDbContext.SaveChangesAsync();
            return existingWalk;
        }

        public async Task<IEnumerable<Walk>> GetAllAsync()
        {
          return await nZWalksDbContext.Walks
                .Include(X => X.Region)
                .Include(X =>X.WalkDiffculty)
                .ToListAsync();
        }

        public Task<Walk> GetAsync(Guid id)
        {
           return nZWalksDbContext.Walks
                  .Include(X => X.Region)
                  .Include(x => x.WalkDiffculty)
                  .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk> UpdateAsync(Guid id, Walk walk)
        {
         var existingWalk = await nZWalksDbContext.Walks.FindAsync(id);
            
            if (existingWalk != null)
            {
                existingWalk.length = walk.length;
                existingWalk.Name = walk.Name;
                existingWalk.WalkDiffcultyId = walk.WalkDiffcultyId;
                existingWalk.RegionId = walk.RegionId;
                await  nZWalksDbContext.SaveChangesAsync();
                return existingWalk;

            }
            return null;
        }
    }
}
