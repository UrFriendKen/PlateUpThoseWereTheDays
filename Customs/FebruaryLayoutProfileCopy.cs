using Kitchen.Layouts;
using Kitchen.Layouts.Modules;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using System.Collections.Generic;
using System.Reflection;
using XNode;

namespace ThoseWereTheDays.Customs
{
    public class FebruaryLayoutProfileCopy : CustomLayoutProfile
    {
        public override string UniqueNameID => "testFebruaryLayoutProfile";

        protected struct Connection
        {
            public int FromIndex;
            public string FromPortName;

            public int ToIndex;
            public string ToPortName;

            public Connection(int fromIndex, string fromPortName, int toIndex, string toPortName)
            {
                FromIndex = fromIndex;
                FromPortName = fromPortName;
                ToIndex = toIndex;
                ToPortName = toPortName;
            }
        }

        protected virtual List<Connection> Connections { get; set; } = new List<Connection>()
        {
            new Connection(0, "Output", 1, "Input"),
            new Connection(1, "Output", 2, "Input"),
            new Connection(2, "Output", 3, "Input"),
            new Connection(2, "Output", 4, "Input"),
            new Connection(4, "Output", 5, "Input"),
            new Connection(5, "Output", 6, "Input"),
            new Connection(6, "Output", 7, "Input"),
            new Connection(7, "Output", 8, "Input"),
            new Connection(8, "Output", 9, "Input"),
            new Connection(9, "Output", 10, "Input"),
            new Connection(10, "Output", 11, "Input"),
            new Connection(11, "Output", 12, "Input"),
            new Connection(12, "Output", 13, "Input"),
            new Connection(13, "Output", 14, "Input"),
            new Connection(13, "Output", 15, "Input"),
            new Connection(14, "Output", 21, "Input"),
            new Connection(15, "Output", 16, "Input"),
            new Connection(15, "Output", 18, "Input"),
            new Connection(16, "Output", 17, "Input"),
            new Connection(17, "Output", 21, "AppendFrom"),
            new Connection(18, "Output", 19, "Input"),
            new Connection(19, "Output", 20, "Input"),
            new Connection(20, "Output", 22, "AppendFrom"),
            new Connection(21, "Output", 22, "Input"),
            new Connection(22, "Output", 23, "Input"),
            new Connection(23, "Output", 24, "Input"),
            new Connection(24, "Output", 25, "Input"),
        };

        public override LayoutGraph Graph => new LayoutGraph()
        {
            nodes = new List<XNode.Node>()
            {
                new RoomGrid()  // 0
                {
                    Width = 3,
                    Height = 2,
                    Type = RoomType.NoRoom,
                    SetType = false
                },
                new MergeRoomsByType(), // 1
                new SwapRoomType()  // 2
                {
                    X = 0,
                    Y = 0,
                    Type = RoomType.Kitchen
                },
                new SetRoom()   // 3
                {
                    X = 1,
                    Y = 0,
                    Type = RoomType.Kitchen
                },
                new InsertRandomRoom()  // 4
                {
                    Type = RoomType.Unassigned
                },
                new InsertRandomRoom()  // 5
                {
                    Type = RoomType.Unassigned
                },
                new SplitRooms()    // 6
                {
                    UniformX = 1,
                    UniformY = 1,
                    RandomX = 0,
                    RandomY = 1
                },
                new PadWithRoom()   // 7
                {
                    Type = RoomType.NoRoom,
                    Above = 2
                },
                new PadWithRoom()   // 8
                {
                    Type = RoomType.Kitchen,
                    Right = 4
                },
                new PadWithRoom()   // 9
                {
                    Type = RoomType.Dining,
                    Left = 3,
                    Below = 4
                },
                new MergeRoomsByType(), // 10
                new SplitLine() // 11
                {
                    Position = 5,
                    Count = 2,
                    IsRow = false
                },
                new SplitLine() // 12   // TO CHECK ORDER OF SPLIT LINE
                {
                    Position = 0,
                    Count = 1,
                    IsRow = true
                },
                new RecentreLayout(),   // 13
                new CreateFrontDoor()   // 14
                {
                    Type = RoomType.Dining,
                    ForceFirstHalf = false
                },
                new FindAllFeatures()   // 15
                {
                    Feature = FeatureType.Door
                },
                new FilterByRoom()  // 16
                {
                    RemoveMode = false,
                    Type1 = RoomType.Kitchen,
                    FilterSecond = true,
                    Type2 = RoomType.Dining
                },
                new SwitchFeatures()    // 17
                {
                    SetToFeature = FeatureType.Hatch
                },
                new FilterByRoom()      // 18
                {
                    RemoveMode = true,
                    Type1 = RoomType.NoRoom,
                    FilterSecond = false,
                    Type2 = RoomType.NoRoom
                },
                new FilterByFreeSpace(),    // 19
                new FilterOnePerPair(),     // 20
                new AppendFeatures(),       // 21
                new AppendFeatures(),       // 22
                new RequireAccessible()     // 23
                {
                    AllowGardens = false,
                    ResultStatus = true
                },
                new RequireFeatures()       // 24
                {
                    Type = FeatureType.Hatch,
                    Minimum = 4,
                    ResultStatus = true
                },
                new Output()    // 25
            }
        };

        public override GameDataObject Table => GDOUtils.GetExistingGDO(ApplianceReferences.TableLarge);
        public override GameDataObject Counter => GDOUtils.GetExistingGDO(ApplianceReferences.Countertop);
        public override Appliance ExternalBin => GDOUtils.GetExistingGDO(ApplianceReferences.WheelieBin) as Appliance;
        public override Appliance WallPiece => GDOUtils.GetExistingGDO(ApplianceReferences.WallPiece) as Appliance;
        public override Appliance InternalWallPiece => GDOUtils.GetExistingGDO(ApplianceReferences.InternalWallPiece) as Appliance;
        public override Appliance StreetPiece => GDOUtils.GetExistingGDO(ApplianceReferences.StreetPiece) as Appliance;

        public override void OnRegister(LayoutProfile gameDataObject)
        {
            PopulateConnections(gameDataObject.Graph);
        }

        static FieldInfo f_ports = typeof(Node).GetField("ports", BindingFlags.NonPublic | BindingFlags.Instance);
        private void PopulateConnections(LayoutGraph layoutGraph)
        {
            List<Node> nodes = layoutGraph.nodes;

            foreach (Node node in nodes)
            {
                if (node is LayoutModule layoutModule)
                {
                    layoutModule.graph = layoutGraph;
                }
            }

            foreach (Connection connection in Connections)
            {
                if (!TryGetNodePort(connection.FromIndex, connection.FromPortName, out NodePort fromPort))
                    break;
                if (!TryGetNodePort(connection.ToIndex, connection.ToPortName, out NodePort toPort))
                    break;
                fromPort.Connect(toPort);

                bool TryGetNodePort(int nodeIndex, string portName, out NodePort nodePort)
                {
                    nodePort = null;
                    if (nodeIndex >= nodes.Count)
                    {
                        LogNodeError($"Node index ({nodeIndex}) must be less than the number of nodes ({nodes.Count})");
                        return false;
                    }
                    Node node = nodes[nodeIndex];
                    object obj = f_ports.GetValue(node);
                    if (obj == null || !(obj is Dictionary<string, NodePort> nodeDictionary))
                    {
                        LogNodeError($"Failed to get Node Dictionary from {node.GetType()}");
                        return false;
                    }
                    if (!nodeDictionary.TryGetValue(portName, out nodePort))
                    {
                        Main.LogError($"Failed to get \"{portName}\" port from {node.GetType()}");
                        return false;
                    }
                    return true;
                }

                void LogNodeError(object msg)
                {
                    Main.LogError($"{GetType().FullName} error! {msg}");
                }
            }
        }
    }
}
