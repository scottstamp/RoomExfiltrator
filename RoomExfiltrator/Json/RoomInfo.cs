using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoomExfiltrator.Json
{
    [DataContract]
    public class RoomInfo
    {
        private string _name;
        private int _state;
        private int _maxUsers;
        private string _description;
        private int _score;
        private int _category;
        private List<string> _tags;

        [DataMember(Name = "name")]
        public string Name { get => _name; set => _name = value; }
        [DataMember(Name = "state")]
        public int State { get => _state; set => _state = value; }
        [DataMember(Name = "max_users")]
        public int MaxUsers { get => _maxUsers; set => _maxUsers = value; }
        [DataMember(Name = "description")]
        public string Description { get => _description; set => _description = value; }
        [DataMember(Name = "score")]
        public int Score { get => _score; set => _score = value; }
        [DataMember(Name = "category_id")]
        public int Category { get => _category; set => _category = value; }
        [DataMember(Name = "tags")]
        public List<string> Tags { get => _tags; set => _tags = value; }
    }
}
