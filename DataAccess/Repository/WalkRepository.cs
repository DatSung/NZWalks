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
    public class WalkRepository : Repository<Walk>, IWalkRepository
    {

        private readonly NZWalksDbContext _context;

        public WalkRepository(NZWalksDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Walk walk)
        {
            _context.Update(walk);
        }

        public void UpdateRange(List<Walk> walks)
        {
            _context.UpdateRange(walks);
        }
    }
}
