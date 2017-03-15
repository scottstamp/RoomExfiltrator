using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoomExfiltrator.Json
{
    [DataContract]
    public class Paint
    {
        private string _wallpaper = "";
        private string _floor = "";
        private string _landscape = "";

        [DataMember(Name = "wallpaper")]
        public string Wallpaper { get => _wallpaper; set => _wallpaper = value; }
        [DataMember(Name = "floor")]
        public string Floor { get => _floor; set => _floor = value; }
        [DataMember(Name = "landscape")]
        public string Landscape { get => _landscape; set => _landscape = value; }
    }
}
