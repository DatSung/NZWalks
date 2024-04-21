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
    public class ImageRepository : Repository<Image>, IImageRepository
    {
        private readonly NZWalksDbContext _context; 

        public ImageRepository(NZWalksDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Image image)
        {
            _context.Update(image);
        }

        public void UpdateRange(List<Image> images)
        {
            _context.UpdateRange(images);
        }
    }
}
