using NZWalks.Model.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.DataAccess.Repository.IRepository
{
    public interface IWalkRepository : IRepository<Walk>
    {
        void Update(Walk walk);
        void UpdateRange(List<Walk> walks);
    }
}
