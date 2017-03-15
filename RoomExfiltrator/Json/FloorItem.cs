using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoomExfiltrator.Json
{
    [DataContract]
    public class FloorItem
    {
        //LogText($"INSERT INTO items (user_id, room_id, item_id, x, y, z, rot) VALUES ('8', '90', '{baseId}', '{x}', '{y}', '{height}', '{rot}');");
        private int _baseId;
        //[DataMember(Name = "base_id")]
        public int BaseId { get => _baseId; set => _baseId = value; }

        private int _x;
        [DataMember(Name = "x")]
        public int X { get => _x; set => _x = value; }

        private int _y;
        [DataMember(Name = "y")]
        public int Y { get => _y; set => _y = value; }

        private string _height;
        [DataMember(Name = "height")]
        public string Height { get => _height; set => _height = value; }

        private int _rotation;
        [DataMember(Name = "rotation")]
        public int Rotation { get => _rotation; set => _rotation = value; }

        private string _extradata = "";
        [DataMember(Name = "extradata")]
        public string Extradata { get => _extradata; set => _extradata = value; }

        private int _originalOwnerId;
        //[DataMember(Name = "original_owner_id")]
        public int OriginalOwnerId { get => _originalOwnerId; set => _originalOwnerId = value; }

        private string _ownerId;
        //[DataMember(Name = "owner_id")]
        public string OwnerId { get => _ownerId; set => _ownerId = value; }

        private string _roomId;
        //[DataMember(Name = "room_id")]
        public string RoomId { get => _roomId; set => _roomId = value; }

        private string _interactionType;
        //[DataMember(Name = "interaction_type")]
        public string InteractionType { get => _interactionType; set => _interactionType = value; }

        private int _id;
        //[DataMember(Name = "id")]
        public int Id { get => _id; set => _id = value; }

        private string _itemName;
        [DataMember(Name = "item_name")]
        public string ItemName { get => _itemName; set => _itemName = value; }

        public string ToQuery()
        {
            return $"('{OwnerId}', '{RoomId}', (SELECT id FROM items_base WHERE item_name = '{ItemName}'), '{X}', '{Y}', '{Height}', '{Rotation}', '{Extradata}'),";
        }

        public override string ToString()
        {
            return $"Furni ID: {Id}\r\n" +
                $"Base ID: {BaseId}\r\n" +
                $"Item Name: {ItemName}\r\n" +
                $"X: {X}\r\n" +
                $"Y: {Y}\r\n" +
                $"Height: {Height}\r\n" +
                $"Rotation: {Rotation}\r\n" +
                $"Extradata: {Extradata}\r\n" +
                $"Interaction Type: {InteractionType}\r\n" +
                $"Owner Id: {OriginalOwnerId}";
        }
    }
}
