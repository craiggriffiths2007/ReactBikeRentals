using ReactBikes.Data;
using ReactBikes.Models;

namespace ReactBikes.Services
{
    public class BikeService : IBikeService
    {
        public Bike Create(ReactBikesContext _context, Bike bike)
        {
            _context.Bike.Add(bike);
            _context.SaveChangesAsync();
            return bike;
        }
        public Bike? Get(ReactBikesContext _context, int id)
        {
            return _context.Bike.Find(id);
        }
        public List<Bike> List(ReactBikesContext _context)
        {
            return _context.Bike.ToList();
        }

        public Bike Update(ReactBikesContext _context, Bike bike)
        {
            _context.Bike.Update(bike);
            _context.SaveChangesAsync();
            return bike;
        }
        public bool Delete(ReactBikesContext _context, int id)
        {
            var oldBike = _context.Bike.Find(id);
            if(oldBike!=null)
            {
                _context.Bike.Remove(oldBike);
                _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
