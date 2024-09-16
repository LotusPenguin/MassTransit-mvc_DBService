using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadModel;

namespace KSR_Backend
{
    internal class RoomRepository : IRoomsDao
    {
        public ICollection<RoomDisplay> GetAll()
        {
            using (var context = new ApiContext())
            {
                return context.Rooms
                    .Select(r => new RoomDisplay() { Id = r.RoomId, Name = r.Name, Status = r.Status, TypeId = r.RoomType })
                    .ToList();
            }
        }

        public RoomDisplay FindById(int id)
        {
            using (var context = new ApiContext())
            {
                return context.Rooms
                    .Select (r => new RoomDisplay() { Id = r.RoomId, Name = r.Name, Status = r.Status, TypeId = r.RoomType })
                    .Where(r => r.Id == id)
                    .First();
            }
        }
        
        public bool Save(RoomDisplay room)
        {
            using (var context = new ApiContext())
            {
                try
                {
                    if (context.Rooms
                    .Select(r => new RoomDisplay() { Id = r.RoomId, Name = r.Name, Status = r.Status, TypeId = r.RoomType })
                    .Where(r => r.Id == room.Id)
                    .First() == null)
                    {
                        context.Rooms.Add(new Room() { RoomId = room.Id, Name = room.Name, Status = room.Status, RoomType = room.TypeId });
                    }
                    else
                    {
                        context.Rooms.Update(new Room() { RoomId = room.Id, Name = room.Name, Status = room.Status, RoomType = room.TypeId });
                    }

                    if(context.SaveChanges() > 0)
                    {
                        return true;
                    } 
                    else
                    {
                        return false;
                    }
                    
                }
                catch (Exception)
                {
                    return false;
                }
                //primary key behavior?
            }
        }

        public bool Delete(RoomDisplay room)
        {
            using (var context = new ApiContext())
            {
                try
                {
                    var toDelete = context.Rooms.SingleOrDefault(x => x.RoomId == room.Id);
                    if (toDelete != null)
                    {
                        context.Rooms.Remove(toDelete);
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch(Exception) { return false; }
            }
        }


    }
}
