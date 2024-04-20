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
    public class DifficultyRepository : Repository<Difficulty>, IDifficultyRepository
    {

        private readonly NZWalksDbContext _context;

        public DifficultyRepository(NZWalksDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Difficulty difficulty)
        {
            _context.Update(difficulty);
        }

        public void UpdateRange(List<Difficulty> difficulties)
        {
            _context.UpdateRange(difficulties);
        }
    }
}
