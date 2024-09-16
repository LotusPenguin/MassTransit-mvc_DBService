using ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSR_Backend
{
    internal class RoomTypeRepository : IRoomTypesDao
    {
        public ICollection<RoomTypeDisplay> GetAll()
        {
            throw new NotImplementedException();
        }

        public RoomTypeDisplay FindById(int id)
        {
            using (var context = new ApiContext())
            {
                return context.RoomTypes
                    .Select(r => new RoomTypeDisplay() { Id = r.Id, Name = r.Name })
                    .Where(r => r.Id == id)
                    .First();
            }
        }

        public ICollection<RoomTypeDisplay> FindByName(string name)
        {
            using (var context = new ApiContext())
            {
                return context.RoomTypes
                    .Select(r => new RoomTypeDisplay() { Id = r.Id, Name = r.Name })
                    .Where(r => r.Name == name)
                    .ToList();
            }
        }
    }
}
