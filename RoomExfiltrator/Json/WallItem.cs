using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoomExfiltrator.Json
{
    [DataContract]
    public class WallItem
    {
        private int _spriteId;
        private string _spriteName;
        private string _position;
        private string _extradata;
        private string _userId;
        private int _state;

        //[DataMember(Name = "sprite_id")]
        public int SpriteId { get => _spriteId; set => _spriteId = value; }
        [DataMember(Name = "sprite_name")]
        public string SpriteName { get => _spriteName; set => _spriteName = value; }
        [DataMember(Name = "position")]
        public string Position { get => _position; set => _position = value; }
        [DataMember(Name = "extradata")]
        public string Extradata { get => _extradata; set => _extradata = value; }
        [DataMember(Name = "user_id")]
        public string UserId { get => _userId; set => _userId = value; }
        [DataMember(Name = "state")]
        public int State { get => _state; set => _state = value; }
    }
}
