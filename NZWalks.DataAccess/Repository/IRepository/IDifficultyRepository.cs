using NZWalks.Model.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.DataAccess.Repository.IRepository
{
    public interface IDifficultyRepository : IRepository<Difficulty>
    {
        void Update(Difficulty difficulty);
        void UpdateRange(List<Difficulty> difficulties);
    }
}
