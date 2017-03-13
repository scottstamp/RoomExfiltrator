using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoomExfiltrator.Json
{
    [DataContract]
    public class Heightmap
    {
        private int _wallHeight;
        private string _map;

        [DataMember(Name = "wall_height")]
        public int WallHeight { get => _wallHeight; set => _wallHeight = value; }
        [DataMember(Name = "map")]
        public string Map { get => _map; set => _map = value; }
    }
}
