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
    public class NorthPoleLayoutProfileCopy : CustomLayoutProfile
    {
        public override string UniqueNameID => "northPoleLayoutProfile";

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
            new Connection(3, "Output", 4, "Input"),
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
            new Connection(13, "Output", 28, "Input"),
            new Connection(13, "Output", 31, "Input"),
            new Connection(14, "Output", 15, "Input"),
            new Connection(15, "Output", 16, "Input"),
            new Connection(15, "Output", 21, "Input"),
            new Connection(16, "Output", 17, "Input"),
            new Connection(17, "Output", 18, "Input"),
            new Connection(18, "Output", 19, "Input"),
            new Connection(19, "Output", 20, "Input"),
            new Connection(20, "Output", 34, "Input"),
            new Connection(21, "Output", 22, "Input"),
            new Connection(21, "Output", 24, "Input"),
            new Connection(21, "Output", 26, "Input"),
            new Connection(22, "Output", 23, "Input"),
            new Connection(23, "Output", 34, "AppendFrom"),
            new Connection(24, "Output", 25, "Input"),
            new Connection(25, "Output", 34, "AppendFrom"),
            new Connection(26, "Output", 27, "Input"),
            new Connection(27, "Output", 34, "AppendFrom"),
            new Connection(28, "Output", 29, "Input"),
            new Connection(29, "Output", 30, "Input"),
            new Connection(30, "Output", 34, "AppendFrom"),
            new Connection(31, "Output", 32, "Input"),
            new Connection(32, "Output", 33, "Input"),
            new Connection(33, "Output", 34, "AppendFrom"),
            new Connection(34, "Output", 35, "Input"),
            new Connection(35, "Output", 36, "Input"),
            new Connection(36, "Output", 37, "Input"),
        };

        public override LayoutGraph Graph => new LayoutGraph()
        {
            nodes = new List<XNode.Node>()
            {
                new RoomGrid()  // 0
                {
                    Width = 2,
                    Height = 2,
                    Type = RoomType.Kitchen,
                    SetType = true
                },
                new SetRoom()   // 1
                {
                    X = 0,
                    Y = 0,
                    Type = RoomType.Garden,
                },
                new SplitLine() // 2
                {
                    Position = 0,
                    Count = 0,
                    IsRow = false
                },
                new SplitLine() // 3
                {
                    Position = 0,
                    Count = 0,
                    IsRow = true
                },
                new SetRoom()   // 4
                {
                    X = 0,
                    Y = 0,
                    Type = RoomType.Dining
                },
                new SplitLine() // 5
                {
                    Position = 0,
                    Count = 1,
                    IsRow = false
                },
                new SplitLine() // 6
                {
                    Position = 0,
                    Count = 1,
                    IsRow = true
                },
                new SplitLine() // 7
                {
                    Position = 4,
                    Count = 3,
                    IsRow = false
                },
                new SplitLine() // 8
                {
                    Position = 4,
                    Count = 1,
                    IsRow = true
                },
                new SplitLine() // 9
                {
                    Position = 3,
                    Count = 0,
                    IsRow = false
                },
                new SplitLine() // 10
                {
                    Position = 3,
                    Count = 0,
                    IsRow = true
                },
                new SplitLine() // 11
                {
                    Position = 0,
                    Count = 1,
                    IsRow = false
                },
                new SplitLine() // 12
                {
                    Position = 0,
                    Count = 1,
                    IsRow = true
                },
                new SplitRooms()    // 13
                {
                    UniformX = 0,
                    UniformY = 0,
                    RandomX = 2,
                    RandomY = 2
                },
                new PadWithRoom()   // 14
                {
                    Type = RoomType.Contracts,
                    Left = 3
                },
                new PadWithRoom()   // 15
                {
                    Type = RoomType.Workshop,
                    Right = 3
                },
                new CreateFrontDoor()   // 16
                {
                    Type = RoomType.Dining,
                    ForceFirstHalf = false
                },
                new MoveFeatureInDirection()    // 17
                {
                    OffsetX = 1,
                    MaxSteps = 10
                },
                new MoveFeatureInDirection()    // 18
                {
                    OffsetX = -1,
                    MaxSteps = 1
                },
                new SwapRoomType()  // 19
                {
                    X = -1,
                    Y = 0,
                    Type = RoomType.Garden
                },
                new SwapRoomType()  // 20
                {
                    X = 15,
                    Y = 0,
                    Type = RoomType.Garden
                },
                new FindAllFeatures()   // 21
                {
                    Feature = FeatureType.Hatch
                },
                new FilterByRoom()  // 22
                {
                    RemoveMode = false,
                    Type1 = RoomType.Dining,
                    FilterSecond = true,
                    Type2 = RoomType.Contracts
                },
                new FilterSelectCount() // 23
                {
                    Count = 2
                },
                new FilterByRoom()  // 24
                {
                    RemoveMode = false,
                    Type1 = RoomType.Kitchen,
                    FilterSecond = true,
                    Type2 = RoomType.Contracts
                },
                new FilterSelectCount() // 25
                {
                    Count = 3
                },
                new FilterByRoom()  // 26
                {
                    RemoveMode = false,
                    Type1 = RoomType.Kitchen,
                    FilterSecond = true,
                    Type2 = RoomType.Workshop
                },
                new FilterSelectCount() // 27
                {
                    Count = 3
                },
                new FindAllFeatures()   // 28
                {
                    Feature = FeatureType.Generic
                },
                new FilterByFreeSpace(),    // 29
                new FilterOppositePair(),   // 30
                new FindAllFeatures()   // 31
                {
                    Feature = FeatureType.Door
                },
                new FilterByFreeSpace(),    // 32
                new FilterOppositePair(),   // 33
                new AppendFeatures(),   // 34
                new RequireFeatureCountEven()   // 35
                {
                    Type = FeatureType.Hatch,
                    RequireEven = true,
                    ResultStatus = true
                },
                new RecentreLayout(),   // 36
                new Output()    // 37
            }
        };

        public override GameDataObject Table => GDOUtils.GetExistingGDO(ApplianceReferences.TableBasicCloth);
        public override GameDataObject Counter => GDOUtils.GetExistingGDO(ApplianceReferences.Countertop);
        public override Appliance ExternalBin => GDOUtils.GetExistingGDO(ApplianceReferences.WheelieBin) as Appliance;
        public override Appliance WallPiece => GDOUtils.GetExistingGDO(ApplianceReferences.GrabberRotatable) as Appliance;
        public override Appliance InternalWallPiece => GDOUtils.GetExistingGDO(ApplianceReferences.GrabberChristmapMap) as Appliance;
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
