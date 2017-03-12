using Sulakore.Habbo;
using Sulakore.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangine;
using Sulakore.Communication;
using System.IO;

namespace RoomExfiltrator
{
    [Module("RoomExfiltrator", "Extracts furni data from rooms")]
    [Author("Scott Stamp", HabboName = "Iterator", Hotel = HHotel.Com)]
    public partial class frmMain : ExtensionForm
    {
        private Dictionary<int, string> interactionTypes = new Dictionary<int, string>();

        public frmMain()
        {
            InitializeComponent();
            var lines = File.ReadAllLines(@"C:\users\scott\documents\habbent\interactiontypes.txt");
            foreach (var line in lines)
            {
                var baseId = int.Parse(line.Split(':')[1]);
                var interactionType = line.Split(':')[2];

                if (!interactionTypes.ContainsKey(baseId))
                    interactionTypes.Add(int.Parse(line.Split(':')[1]), line.Split(':')[2]);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Triggers.InAttach(3774, ParseFloorItems);
            //foreach (var interactionType in interactionTypes)
            //LogText($"{interactionType.Key}:{interactionType.Value}";
        }

        private void ParseFloorItems(DataInterceptedEventArgs e)
        {
            try {
                txtFurni.Text = "";
                var ownersCount = e.Packet.ReadInteger();
                for (var i = 0; i < ownersCount; i++)
                {
                    e.Packet.ReadInteger();
                    e.Packet.ReadString();
                    //LogText($"Owner - ID: {e.Packet.ReadInteger()} | Name: {e.Packet.ReadString()}");
                }

                LogText("");

                var furniCount = e.Packet.ReadInteger();
                for (var i = 0; i < furniCount; i++)
                {
                    var furniItem = new FurniItem();

                    var furniId = e.Packet.ReadInteger();
                    LogText($"Furni ID: {furniId}");
                    var baseId = e.Packet.ReadInteger();
                    LogText($"Base ID: {baseId}");
                    var x = e.Packet.ReadInteger();
                    LogText($"X: {x}");
                    var y = e.Packet.ReadInteger();
                    LogText($"Y: {y}");
                    var rot = e.Packet.ReadInteger();
                    LogText($"Rot: {rot}");
                    var height = e.Packet.ReadString();
                    LogText($"Height: {height}");
                    var stackheight = e.Packet.ReadString();
                    LogText($"Stack Height: {stackheight}");

                    furniItem.baseId = baseId;
                    furniItem.x = x;
                    furniItem.y = y;
                    furniItem.rot = rot;
                    furniItem.height = height;
                    furniItem.ownerId = txtOwnerId.Text;
                    furniItem.roomId = txtRoomId.Text;

                    //LogText($"INSERT INTO items (user_id, room_id, item_id, x, y, z, rot) VALUES ('8', '90', '{baseId}', '{x}', '{y}', '{height}', '{rot}');");

                    LogText($"Interaction Type: {interactionTypes[baseId]}");

                    e.Packet.ReadInteger();
                    e.Packet.ReadInteger();

                    switch (interactionTypes[baseId])
                    {
                        case "furniture_background_color":
                            var colorsCount = e.Packet.ReadInteger();
                            for (var i2 = 0; i2 < colorsCount; i2++) LogText(e.Packet.ReadInteger());
                            break;
                        case "furniture_change_state_when_step_on":
                        case "furniture_basic":
                        case "furniture_multistate":
                        case "furniture_multiheight":
                        case "furniture_internal_link":
                            furniItem.extradata = e.Packet.ReadString(); // extraData
                            break;

                        case "furniture_present":
                        case "furniture_mannequin":
                        case "furniture_youtube":
                        case "furniture_bg":
                        case "furniture_monsterplant_seed":
                            var paramsCount = e.Packet.ReadInteger(); // parameter count
                            for (var i2 = 0; i2 < paramsCount; i2++)
                            {
                                furniItem.extradata += $"{e.Packet.ReadString()}={e.Packet.ReadString().Replace("images.habbo.com", "client.habbent.pw")};";

                                //LogText($"{e.Packet.ReadString()}: {e.Packet.ReadString()}");
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
                                LogText($"{e.Packet.ReadString()}");
                            }
                            break;

                        case "furniture_crackable":
                            e.Packet.ReadString();
                            e.Packet.ReadInteger();
                            e.Packet.ReadInteger();
                            break;
                    }

                    LogText(furniItem.ToQuery(), true);

                    var something2 = e.Packet.ReadInteger();
                    var canChangeState = e.Packet.ReadInteger();
                    var ownerId = e.Packet.ReadInteger();

                    LogText("");

                    //LogText($"{something2} - {canChangeState} - {ownerId}");
                }
            }
            catch (Exception ex)
            {
                LogText("\r\n" + ex.Message + "\r\n" + e.Packet.ToString());
            }
        }

        private void LogText(object t) => LogText(t, false);

        private void LogText(object t, bool sql)
        {
            if (chkSqlOnly.Checked && sql)
                txtFurni.Text += $"{t}\r\n";
            if (!chkSqlOnly.Checked)
                txtFurni.Text += $"{t}\r\n";
        }
    }

    class FurniItem
    {
        //LogText($"INSERT INTO items (user_id, room_id, item_id, x, y, z, rot) VALUES ('8', '90', '{baseId}', '{x}', '{y}', '{height}', '{rot}');");
        public int baseId { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string height { get; set; }
        public int rot { get; set; }
        public string extradata = "";
        public string ownerId { get; set; }
        public string roomId { get; set; }

        public string ToQuery()
        {
            return $"INSERT INTO items (user_id, room_id, item_id, x, y, z, rot, extra_data) VALUES ('{ownerId}', '{roomId}', (SELECT id FROM items_base WHERE sprite_id = '{baseId}'), '{x}', '{y}', '{height}', '{rot}', '{extradata}');";
        }
    }
}
