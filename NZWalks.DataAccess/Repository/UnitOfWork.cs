using NZWalks.DataAccess.Data;
using NZWalks.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly NZWalksDbContext _context;

        public IDifficultyRepository DifficultyRepository { get; set; }

        public IRegionRepository RegionRepository { get; set; }

        public IWalkRepository WalkRepository { get; set; }

        public IImageRepository ImageRepository { get; set; }

        public UnitOfWork(NZWalksDbContext context)
        {
            _context = context;
            DifficultyRepository = new DifficultyRepository(_context);
            RegionRepository = new RegionRepository(_context);
            WalkRepository = new WalkRepository(_context);
            ImageRepository = new ImageRepository(_context);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
