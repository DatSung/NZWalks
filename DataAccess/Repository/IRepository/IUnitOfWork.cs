using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IDifficultyRepository DifficultyRepository { get; }
        IRegionRepository RegionRepository { get; }
        IWalkRepository WalkRepository { get; }
        IImageRepository ImageRepository { get; }

        Task SaveAsync();
    }
}
