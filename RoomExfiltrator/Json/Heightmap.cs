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
        private string _model;
        private int _wallHeight;
        private string _map;

        [DataMember(Name = "model")]
        public string Model { get => _model; set => _model = value; }
        [DataMember(Name = "wall_height")]
        public int WallHeight { get => _wallHeight; set => _wallHeight = value; }
        [DataMember(Name = "map")]
        public string Map { get => _map; set => _map = value; }
    }
}
