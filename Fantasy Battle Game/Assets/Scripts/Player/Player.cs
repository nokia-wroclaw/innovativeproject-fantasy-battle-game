using System.Collections.Generic;
using Champions;

namespace Player
{
    public class Player
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<Champion> Champions { get; private set; }

        public Player()
        {
            Champions = new List<Champion>();
        }
    }
}
