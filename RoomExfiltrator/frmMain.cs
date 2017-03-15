using Sulakore.Habbo;
using Sulakore.Modules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Tangine;
using Sulakore.Communication;
using System.IO;
using RoomExfiltrator.Json;
using System.Reflection;
using System.Text;

namespace RoomExfiltrator
{
    [Module("RoomExfiltrator", "Extracts furni data from rooms")]
    [Author("Scott Stamp", HabboName = "Iterator", Hotel = HHotel.Com)]
    public partial class frmMain : ExtensionForm
    {
        private List<Tuple<string, int, string>> floorData = new List<Tuple<string, int, string>>();
        private List<Tuple<string, int, string>> wallData = new List<Tuple<string, int, string>>();
        public override bool IsRemoteModule => true;
        private Room room = new Room();

        public frmMain()
        {
            InitializeComponent();

            foreach (var line in ReadEmbeddedResource("RoomExfiltrator.Resources.flooritems.txt"))
                floorData.Add(ParseItemTuple(line));

            foreach (var line in ReadEmbeddedResource("RoomExfiltrator.Resources.wallitems.txt"))
                wallData.Add(ParseItemTuple(line));
        }

        private string[] ReadEmbeddedResource(string name)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd()
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            }
        }

        private Tuple<string, int, string> ParseItemTuple(string line)
        {
            var spriteName = line.Split(':')[0];
            var baseId = int.Parse(line.Split(':')[1]);
            var interactionType = line.Split(':')[2];

            return new Tuple<string, int, string>(spriteName, baseId, interactionType);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            var wallItems = Game.GetHeader("a7f0ca597403c539084a75b096294081");
            var floorItems = Game.GetHeader("540de3e1e0baf1632ce3107fc99780f4");
            var roomData = Game.GetHeader("337d349ac5811943f3beb10273262f94");
            var heightmap = Game.GetHeader("28b93d64f5126a5b304f088c384c974e");
            var roomPaint = Game.GetHeader("b76f0244225ff8de48e8fb34d49d1663");
            var roomModel = Game.GetHeader("6baee118ced0e46a3ca5384b83e6338c");

            Triggers.InAttach(roomData, ParseRoomData);
            Triggers.InAttach(roomModel, ParseRoomModel);
            Triggers.InAttach(floorItems, ParseFloorItems);
            Triggers.InAttach(wallItems, ParseWallItems);
            Triggers.InAttach(heightmap, ParseHeightmap);
            Triggers.InAttach(roomPaint, ParseRoomPaint);
        }

        private void ParseRoomData(DataInterceptedEventArgs e)
        {
            //int mask = 8 | 16;

            e.Packet.ReadBoolean(); // ??
            e.Packet.ReadInteger(); // original room ID
            var name = e.Packet.ReadString(); // original room name
            e.Packet.ReadInteger(); // original room owner ID
            e.Packet.ReadString(); // original room owner name
            var state = e.Packet.ReadInteger(); // room state (open, locked, password, invisible)
            e.Packet.ReadInteger(); // user count
            var maxUsers = e.Packet.ReadInteger(); // user max
            var description = e.Packet.ReadString(); // room description
            e.Packet.ReadInteger(); // ??
            e.Packet.ReadInteger(); // ??
            var score = e.Packet.ReadInteger(); // room score
            var category = e.Packet.ReadInteger(); // room category
            var tagCount = e.Packet.ReadInteger(); // room tag count
            var tags = new List<string>();
            for (var i = 0; i < tagCount; i++)
                tags.Add(e.Packet.ReadString()); // room tag

            // we don't really need to care about the rest of the packet

            /*var roomType = e.Packet.ReadInteger(); // room type

            if (roomType == (mask | 2)) // group room
            {
                e.Packet.ReadInteger(); // group id
                e.Packet.ReadString(); // group name
                e.Packet.ReadString(); // group badge
            }

            if (roomType == (mask | 4)) // promoted room
            {
                e.Packet.ReadString(); // promo title
                e.Packet.ReadString(); // promo description
                e.Packet.ReadInteger(); // promo time remaining
            }

            e.Packet.ReadBoolean(); // is public room
            e.Packet.ReadBoolean(); // is staff promoted
            e.Packet.ReadBoolean(); // is public room again?
            e.Packet.ReadBoolean(); // is room muted

            e.Packet.ReadInteger(); // mute option
            e.Packet.ReadInteger(); // kick option
            e.Packet.ReadInteger(); // ban option

            e.Packet.ReadBoolean(); // mute all button

            e.Packet.ReadInteger(); // chat mode
            e.Packet.ReadInteger(); // chat weight
            e.Packet.ReadInteger(); // chat speed
            e.Packet.ReadInteger(); // chat distance
            e.Packet.ReadInteger(); // chat protection?*/
            
            room.RoomInfo.Name = name;
            room.RoomInfo.State = state;
            room.RoomInfo.MaxUsers = maxUsers;
            room.RoomInfo.Description = description;
            room.RoomInfo.Score = score;
            room.RoomInfo.Category = category;
            room.RoomInfo.Tags = tags;
        }

        private void ParseRoomModel(DataInterceptedEventArgs e)
        {
            room.Heightmap.Model = e.Packet.ReadString();
        }

        private void ParseHeightmap(DataInterceptedEventArgs e)
        {
            e.Packet.ReadBoolean();
            room.Heightmap.WallHeight = e.Packet.ReadInteger();
            room.Heightmap.Map = e.Packet.ReadString().Replace((char)13, '\r');
        }

        private void ParseRoomPaint(DataInterceptedEventArgs e)
        {
            switch (e.Packet.ReadString())
            {
                case "wallpaper":
                    room.Paint.Wallpaper = e.Packet.ReadString();
                    break;
                case "floor":
                    room.Paint.Floor = e.Packet.ReadString();
                    break;
                case "landscape":
                    room.Paint.Landscape = e.Packet.ReadString();
                    break;
            }
        }

        private void ParseWallItems(DataInterceptedEventArgs e)
        {
            var ownersCount = e.Packet.ReadInteger();
            for (var i = 0; i < ownersCount; i++)
            {
                // we don't really care who owns this shit
                e.Packet.ReadInteger();
                e.Packet.ReadString();
            }

            var furniCount = e.Packet.ReadInteger();
            for (var i = 0; i < furniCount; i++)
            {
                var furniId = e.Packet.ReadString();
                var spriteId = e.Packet.ReadInteger();
                var wallPos = e.Packet.ReadString();
                var extradata = e.Packet.ReadString();
                var userId = e.Packet.ReadInteger();
                var state = e.Packet.ReadInteger();

                var wallItem = new WallItem();
                wallItem.SpriteId = spriteId;
                wallItem.SpriteName = wallData.Where(t => t.Item2 == spriteId).First().Item1;
                wallItem.Position = wallPos;
                wallItem.Extradata = extradata;
                wallItem.UserId = txtOwnerId.Text;
                wallItem.State = state;

                room.WallItems.Add(wallItem);

                e.Packet.ReadInteger(); // ?
            }

            LogText(room.ToJson(), true);
            room = new Room();
        }

        private void ParseFloorItems(DataInterceptedEventArgs e)
        {

            Invoke((MethodInvoker)delegate { txtFurni.Text = ""; });
            LogText("---------------------------------------");
            var ownersCount = e.Packet.ReadInteger();
            for (var i = 0; i < ownersCount; i++)
            {
                LogText($"Owner - ID: {e.Packet.ReadInteger()} | Name: {e.Packet.ReadString()}");
            }

            //if (chkSqlOnly.Checked)
                //LogText("INSERT INTO items (user_id, room_id, item_id, x, y, z, rot, extra_data) VALUES", true);

            //string output = "";
            //var room = new Room();
            room.RoomId = txtRoomId.Text;
            room.OwnerId = txtOwnerId.Text;

            var furniCount = e.Packet.ReadInteger();
            for (var i = 0; i < furniCount; i++)
            {
                var furniItem = new FloorItem();

                var furniId = e.Packet.ReadInteger();
                var baseId = e.Packet.ReadInteger();
                var x = e.Packet.ReadInteger();
                var y = e.Packet.ReadInteger();
                var rot = e.Packet.ReadInteger();
                var height = e.Packet.ReadString();
                var stackheight = e.Packet.ReadString();

                Tuple<string, int, string> floorItem = floorData.Where(t => t.Item2 == baseId).First();

                

                furniItem.Id = furniId;
                furniItem.BaseId = baseId;
                furniItem.X = x;
                furniItem.Y = y;
                furniItem.Rotation = rot;
                furniItem.Height = height;
                furniItem.OwnerId = txtOwnerId.Text;
                furniItem.RoomId = txtRoomId.Text;
                furniItem.ItemName = floorItem.Item1;
                furniItem.InteractionType = floorItem.Item3;

                e.Packet.ReadInteger(); // nobody has a fucking clue what this is for apparently

                // this should be a mask, but w/e, we don't care about the value (see above)
                bool isLimited = (e.Packet.ReadInteger() - 256 > -1);

                if (floorItem.Item3 == "furniture_high_score")
                {
                    LogText(e.Packet.ToString(), true);
                }

                switch (floorItem.Item3)
                {
                    // _SafeStr_3703 {String, result = Int}
                    // _SafeStr_4710 {} (no data, weird)


                    // IntArrayStuffData (what the hell was the dev team smoking when they named these?)
                    case "furniture_background_color":
                        var colorsCount = e.Packet.ReadInteger();
                        for (var i2 = 0; i2 < colorsCount; i2++) furniItem.Extradata += $"{e.Packet.ReadInteger()}:";
                        break;

                    // _SafeStr_2428
                    case "furniture_change_state_when_step_on":
                    case "furniture_basic":
                    case "furniture_trophy":
                    case "furniture_multistate":
                    case "furniture_multiheight":
                    case "furniture_internal_link":
                    case "furniture_es":
                    case "furniture_score":
                    case "furniture_purchasable_clothing":
                    case "furniture_counter_clock":
                    case "furniture_credit":
                    case "furniture_custom_stack_height":
                    case "furniture_pushable":
                    case "furniture_one_way_door":
                    case "furniture_jukebox":
                    case "furniture_floor_hole":
                    case "furniture_dice":
                    case "furniture_hockey_score":
                    case "furniture_fireworks":
                    case "furniture_soundblock":
                        furniItem.Extradata = e.Packet.ReadString(); // extraData
                        break;

                    // HighScoreStuffData
                    case "furniture_high_score":
                        e.Packet.ReadString(); // "1"
                        e.Packet.ReadInteger(); // 1
                        e.Packet.ReadInteger(); // 0
                        var count2 = e.Packet.ReadInteger(); // teams count
                        for (var i2 = 0; i2 < count2; i2++)
                        {
                            var score = e.Packet.ReadInteger(); // score?
                            var countNames = e.Packet.ReadInteger(); // names count
                            for (var i3 = 0; i3 < countNames; i3++)
                            {
                                var habboName = e.Packet.ReadString(); // name
                            }
                        }
                        break;

                    // MapStuffData
                    case "furniture_present":
                    case "furniture_mannequin":
                    case "furniture_youtube":
                    case "furniture_bg":
                    case "furniture_bb":
                    case "furniture_monsterplant_seed":
                        var paramsCount = e.Packet.ReadInteger(); // parameter count
                        for (var i2 = 0; i2 < paramsCount; i2++)
                        {
                            furniItem.Extradata += $"{e.Packet.ReadString()}={e.Packet.ReadString().Replace("images.habbo.com", "client.habbent.pw")};";
                        }
                        break;

                    // StringArrayStuffData
                    case "furniture_lovelock":
                    case "furniture_hween_lovelock":
                    case "furniture_badge_display":
                    case "furniture_group_forum_terminal":
                    case "furniture_guild_customized":
                        var count = e.Packet.ReadInteger();
                        LogText($"variables: {count}");
                        for (var i2 = 0; i2 < count; i2++)
                        {
                            var not = $"{e.Packet.ReadString()}";
                        }
                        break;

                    // _SafeStr_4711
                    case "furniture_crackable":
                        e.Packet.ReadString();
                        e.Packet.ReadInteger();
                        e.Packet.ReadInteger();
                        break;
                }

                if (isLimited)
                {
                    var limitedNo = e.Packet.ReadInteger();
                    var limitedTotal = e.Packet.ReadInteger();
                }

                var something2 = e.Packet.ReadInteger();
                var canChangeState = e.Packet.ReadInteger();
                furniItem.OriginalOwnerId = e.Packet.ReadInteger();

                room.FloorItems.Add(furniItem);
                //LogText(furniItem.ToString(), true);
            }

            
        }

        private void LogText(object t) => LogText(t, false);

        private void LogText(object t, bool sql)
        {
            if (chkJsonOnly.Checked && sql)
                Invoke((MethodInvoker)delegate { txtFurni.Text += $"{t}\r\n"; });
            if (!chkJsonOnly.Checked && !sql)
                Invoke((MethodInvoker)delegate { txtFurni.Text += $"{t}\r\n"; });
        }
    }
}
