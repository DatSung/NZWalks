using NZWalks.Model.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.DataAccess.Repository.IRepository
{
    public interface IRegionRepository : IRepository<Region>
    {
        void Update(Region region);
        void UpdateRange(List<Region> regions);
    }
}
