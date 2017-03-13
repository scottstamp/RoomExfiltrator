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
            Triggers.InAttach(448, ParseFloorItems);
            Triggers.InAttach(1810, ParseWallItems);
            Triggers.InAttach(1015, ParseHeightmap);
            Triggers.InAttach(20, ParseRoomPaint);
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

            LogText(JsonHelper.FormatJson(room.ToJson()), true);
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

                //LogText($"Interaction Type: {floorItem.Item3}");

                e.Packet.ReadInteger(); // nobody has a fucking clue what this is for apparently
                bool isLimited = (e.Packet.ReadInteger() - 256 > -1);

                switch (floorItem.Item3)
                {
                    case "furniture_background_color":
                        var colorsCount = e.Packet.ReadInteger();
                        for (var i2 = 0; i2 < colorsCount; i2++) furniItem.Extradata += $"{e.Packet.ReadInteger()}:";
                        break;
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
                        furniItem.Extradata = e.Packet.ReadString(); // extraData
                        break;

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

                    case "furniture_crackable":
                        e.Packet.ReadString();
                        e.Packet.ReadInteger();
                        e.Packet.ReadInteger();
                        break;
                }

                //LogText(furniItem.ToQuery(), true);

                if (isLimited)
                {
                    var limitedNo = e.Packet.ReadInteger();
                    var limitedTotal = e.Packet.ReadInteger();
                }

                var something2 = e.Packet.ReadInteger();
                var canChangeState = e.Packet.ReadInteger();
                furniItem.OriginalOwnerId = e.Packet.ReadInteger();

                room.FloorItems.Add(furniItem);
            }

            
        }

        private void LogText(object t) => LogText(t, false);

        private void LogText(object t, bool sql)
        {
            if (chkSqlOnly.Checked && sql)
                Invoke((MethodInvoker)delegate { txtFurni.Text += $"{t}\r\n"; });
            if (!chkSqlOnly.Checked && !sql)
                Invoke((MethodInvoker)delegate { txtFurni.Text += $"{t}\r\n"; });
        }
    }
}
