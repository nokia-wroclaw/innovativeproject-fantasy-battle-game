using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Player
{
    public class Player
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<GameObject> Champions { get; private set; }
    }
}
