using ReactBikes.Data;
using ReactBikes.Models;

namespace ReactBikes.Services
{
    public interface IBikeService
    {
        public Bike Create(ReactBikesContext _context, Bike bike);
        public Bike? Get(ReactBikesContext _context, int id);
        public List<Bike> List(ReactBikesContext _context);
        public Bike Update(ReactBikesContext _context, Bike bike);
        public bool Delete(ReactBikesContext _context, int id);
    }
}
