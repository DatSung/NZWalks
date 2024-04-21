using NZWalks.Model.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.DataAccess.Repository.IRepository
{
    public interface IImageRepository : IRepository<Image>
    {
        void Update(Image image);
        void UpdateRange(List<Image> images);
    }
}
