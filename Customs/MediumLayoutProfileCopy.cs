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
    public class MediumLayoutProfileCopy : CustomLayoutProfile
    {
        public override string UniqueNameID => "testLayoutProfile";

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
            new Connection(5, "Output", 7, "Input"),
            new Connection(7, "Output", 8, "Input"),
            new Connection(8, "Output", 9, "Input"),
            new Connection(7, "Output", 10, "Input"),
            new Connection(10, "Output", 11, "Input"),
            new Connection(11, "Output", 12, "Input"),
            new Connection(6, "Output", 13, "Input"),
            new Connection(9, "Output", 13, "AppendFrom"),
            new Connection(12, "Output", 13, "AppendFrom"),
            new Connection(13, "Output", 14, "Input"),
            new Connection(14, "Output", 15, "Input"),
            new Connection(14, "Output", 16, "Input")
        };

        public override LayoutGraph Graph => new LayoutGraph()
        {
            nodes = new List<XNode.Node>()
            {
                new RoomGrid()  // 0
                {
                    Width = 4,
                    Height = 2,
                    Type = RoomType.NoRoom,
                    SetType = false
                },
                new CreateRoomByJoins() // 1
                {
                    StartX = 0,
                    StartY = 0,
                    Joins = 3,
                    Type = RoomType.Dining,
                    Required = false
                },
                new CreateRoomByJoins() // 2
                {
                    StartX = 3,
                    StartY = 1,
                    Joins = 2,
                    Type = RoomType.Kitchen,
                    Required = false
                },
                new SplitRooms()    // 3
                {
                    UniformX = 2,   // Expands each cell to have height i + 1;
                    UniformY = 2,   // Expands each cell to have height j + 1;
                    RandomX = 0,    // Add rows at n random y positions, pushes all rows above selected row up
                    RandomY = 2     // Insert columns at m random x positions, pushes all columns on right of selected column to the right
                },
                new MergeRoomsByType(), // 4
                new RecentreLayout(),   // 5
                new CreateFrontDoor()   // 6
                {
                    Type = RoomType.Dining,
                    ForceFirstHalf = false
                },
                new FindAllFeatures()   // 7
                {
                    Feature = FeatureType.Door
                },
                new FilterByFreeSpace(),    // 8
                new FilterOnePerPair(),     // 9
                new FilterByRoom()      // 10
                {
                    RemoveMode = false,
                    Type1 = RoomType.Kitchen,
                    FilterSecond = true,
                    Type2 = RoomType.Dining,
                },
                new FilterAdjacentPair(),   // 11
                new SwitchFeatures()        // 12
                {
                    SetToFeature = FeatureType.Hatch
                },
                new AppendFeatures(),   // 13
                new RequireAccessible() // 14
                {
                    AllowGardens = false,
                    ResultStatus = true
                },
                new RequireFeatures()   // 15
                {
                    Type = FeatureType.Hatch,
                    Minimum = 2,
                    ResultStatus = true
                },
                new Output()    // 16
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
