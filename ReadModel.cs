using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadModel
{
    public interface IRoomsDao
    {
        ICollection<RoomDisplay> GetAll();
        RoomDisplay? FindById(int id);
    }

    public interface IRoomTypesDao
    {
        ICollection<RoomTypeDisplay> GetAll();
    }

    public class RoomDisplay
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string Status { get; set; }
    }

    public class RoomAdd
    {
        public string Name { get; set; }
        public string TypeId { get; set; }
        public string Status { get; set; }
    }

    public class RoomChangeType
    {
        public string RoomId { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
    }

    public class RoomTypeDisplay
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RoomTypeAdd
    {
        public string Name { get; set; }
    }
}
