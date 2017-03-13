using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace RoomExfiltrator.Json
{
    [DataContract]
    public class Room
    {
        private static readonly DataContractJsonSerializer _serializer;

        private string _userId = "0";
        [DataMember(Name = "ownerId", Order = 0)]
        internal string OwnerId { get => _userId; set => _userId = value; }

        private string _roomId = "0";
        [DataMember(Name = "roomId", Order = 1)]
        internal string RoomId { get => _roomId; set => _roomId = value; }

        private Heightmap _heightmap = new Heightmap();
        [DataMember(Name = "heightmap", Order = 2)]
        internal Heightmap Heightmap { get => _heightmap; set => _heightmap = value; }

        private Paint _paint = new Paint();
        [DataMember(Name = "paint", Order = 3)]
        internal Paint Paint { get => _paint; set => _paint = value; }

        private List<FloorItem> _floorItems = new List<FloorItem>();
        [DataMember(Name = "floor_items", Order = 6)]
        internal List<FloorItem> FloorItems { get => _floorItems; set => _floorItems = value; }

        private List<WallItem> _wallItems = new List<WallItem>();
        [DataMember(Name = "wall_items", Order = 7)]
        internal List<WallItem> WallItems { get => _wallItems; set => _wallItems = value; }

        static Room()
        {
            _serializer = new DataContractJsonSerializer(typeof(Room));
        }

        public string ToJson()
        {
            using (var jsonStream = new MemoryStream())
            {
                _serializer.WriteObject(jsonStream, this);
                return Encoding.UTF8.GetString(jsonStream.ToArray());
            }
        }
    }
}
