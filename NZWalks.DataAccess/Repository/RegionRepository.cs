using NZWalks.DataAccess.Data;
using NZWalks.DataAccess.Repository.IRepository;
using NZWalks.Model.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.DataAccess.Repository
{
    public class RegionRepository : Repository<Region>, IRegionRepository
    {
        private readonly NZWalksDbContext _context;

        public RegionRepository(NZWalksDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Region region)
        {
            _context.Update(region);
        }

        public void UpdateRange(List<Region> regions)
        {
            _context.UpdateRange(regions);
        }
    }
}
