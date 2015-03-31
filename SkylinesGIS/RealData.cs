using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using System;
using UnityEngine;
using System.ComponentModel;
using ColossalFramework.Math;
using KMLib;
using KMLib.Feature;
using KMLib.Geometry;

namespace SkylinesGIS
{
    public class RealData : IUserMod 
    {

        public string Name 
        {
            get { return "Real Data"; }
        }

        public string Description 
        {
            get { return "This mod allows for the importing of GIS data into Skylines."; }
        }
    }

    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {

            unlockAllTiles();
            //dumpAllBuildings();
            //dumpAllVehicles();
            //spawnVehicle();
            //roads/paths
            //dumpAllNetInfo();
            Vector3 startPos1 = new Vector3(0, 0, 0);
            Vector3 endPos1 = new Vector3(600, 0, 0);
            Vector3 startPos2 = new Vector3(0, 0, 100);
            Vector3 endPos2 = new Vector3(600, 0, 100);
            Vector3 buildingPos1 = new Vector3(1000, 0, 1000);
            Vector3 buildingPos2 = new Vector3(-1000, 0, -1000);
            buildRoad(startPos1, endPos1, PrefabNames.Roads.GravelRoad);
            buildRoad(startPos2, endPos2, PrefabNames.Roads.LargeRoadDecorationTrees);
            buildBuilding(buildingPos1, 0, PrefabNames.Buildings.SpaceElevator);
            buildBuilding(buildingPos2, 0, PrefabNames.Buildings.PoshMall);
            KMLRoot kmlDoc = CreateKmlDoc();
            
            dumpObject(kmlDoc.Document.List[0], "doc");
            debug(0, "The placemark name is: " + kmlDoc.Document.List[0].name);
            //buildRoad(startPos, new Vector3(0,0,-800), 38); 
            KmlPoint myCorner = new KmlPoint((float)-95, (float)60);
            GeoUtils.GpsMap map = new GeoUtils.GpsMap(myCorner);
            debug(0, "map created");
            dumpObject(map.topLeftCorner, "TopLeft");
            dumpObject(map.topRightCorner, "TopRight");
            dumpObject(map.bottomLeftCorner, "BottomLeft");
            dumpObject(map.bottomRightCorner, "BottomRight");
        }

        public KMLRoot loadKml(string path){
            return KMLRoot.Load(path);
        }

        public KMLRoot CreateKmlDoc()
        {
            KMLRoot kml = new KMLRoot();
            Placemark pm = new Placemark();
            pm.name = "foo";
            pm.Point = new KmlPoint(120, 45, 50);
            pm.Snippet = "foo is cool";
            pm.Snippet.maxLines = 1;

            Folder fldr = new Folder("Test Folder");

            kml.Document.Add(pm);
            kml.Document.Add(new Placemark());
            kml.Document.Add(fldr);

            return kml;
        }

        public void unlockAllTiles()
        {
            int maxTiles = 25;
            Singleton<GameAreaManager>.instance.m_maxAreaCount = maxTiles;
            for (int index = 0; index < 4; index += 1)
            {
                for (int index2 = 0; index2 < maxTiles; index2 += 1)
                {
                    Singleton<GameAreaManager>.instance.UnlockArea(index2);
                }
            }
        }

        public void buildBuilding(Vector3 position, float angle, string name)
        {
            ushort building;
            BuildingManager instance = Singleton<BuildingManager>.instance;
            BuildingInfo buildingInfo = PrefabCollection<BuildingInfo>.FindLoaded(name);
            instance.CreateBuilding(out building, ref Singleton<SimulationManager>.instance.m_randomizer, buildingInfo, 
                position, angle, 0, Singleton<SimulationManager>.instance.m_currentBuildIndex);
        }

        public void buildRoad(Vector3 startVector, Vector3 endVector, string name)
        {
            int maxSegments = 100;
            bool test = false;
            bool visualize = false;
            bool autoFix = true;
            bool needMoney = false;
            bool invert = false;
            bool switchDir = false;
            ushort relocateBuildingID = 0;
            ushort firstNode;
            ushort lastNode;
            ushort startNode;
            ushort endNode;
            ushort segment;
            int cost;
            int productionRate;

            NetInfo netInfo = PrefabCollection<NetInfo>.FindLoaded(name);
            float startHeight = NetSegment.SampleTerrainHeight(netInfo, startVector, false);
            float endHeight = NetSegment.SampleTerrainHeight(netInfo, endVector, false);

            NetTool.ControlPoint startControlPt = new NetTool.ControlPoint();
            NetTool.ControlPoint endControlPt = new NetTool.ControlPoint();
            
            startVector.y = startHeight;
            startControlPt.m_position = startVector;
            endVector.y = endHeight;
            endControlPt.m_position = endVector;

            NetTool.CreateNode(netInfo, startControlPt, startControlPt, startControlPt, NetTool.m_nodePositionsSimulation,
                0, false, false, false, false, false, false, (ushort)0, out startNode, out segment, out cost, out productionRate);

            NetTool.CreateNode(netInfo, endControlPt, endControlPt, endControlPt, NetTool.m_nodePositionsSimulation, 
                0, false, false, false, false, false, false, (ushort)0, out endNode, out segment, out cost, out productionRate);

            startControlPt.m_node = startNode;
            endControlPt.m_node = endNode;

            NetTool.ControlPoint midControlPt = endControlPt;
            midControlPt.m_position = (startControlPt.m_position + endControlPt.m_position) * 0.5f;
            midControlPt.m_direction = VectorUtils.NormalizeXZ(midControlPt.m_position - startControlPt.m_position);
            endControlPt.m_direction = VectorUtils.NormalizeXZ(endControlPt.m_position - midControlPt.m_position);

            NetTool.CreateNode(netInfo, startControlPt, midControlPt, endControlPt, NetTool.m_nodePositionsSimulation,
                 maxSegments, test, visualize, autoFix, needMoney, invert, switchDir, relocateBuildingID, out firstNode,
                 out lastNode, out segment, out cost, out productionRate);
        }

        public void spawnVehicle()
        {
            ushort newVehicle;
            VehicleManager vehicleManager = Singleton<VehicleManager>.instance;
            SimulationManager simulationManager = Singleton<SimulationManager>.instance;
            VehicleInfo vehicleInfo = PrefabCollection<VehicleInfo>.GetPrefab(0);
            Vector3 position = new Vector3(0, 0, 0);
            bool vehicleCreated = vehicleManager.CreateVehicle(out newVehicle, ref simulationManager.m_randomizer,
                vehicleInfo, position, 
                TransferManager.TransferReason.Single1, false, false);

            //VehicleInfo newInfo = vehicleManager.m_vehicles.m_buffer[(int)newVehicle].Info;
            vehicleManager.m_vehicles.m_buffer[(int)newVehicle].m_flags |= Vehicle.Flags.Spawned;
            bool vehicleSpawned = vehicleInfo.m_vehicleAI.TrySpawn(newVehicle, ref vehicleManager.m_vehicles.m_buffer[(int)newVehicle]);

            debug(0, newVehicle);
            debug(0, vehicleInfo);
            debug(0, "Spawned: " + vehicleSpawned);
            debug(0, "Created: " + vehicleCreated);
        }

        public class UnlockAllMilestones : MilestonesExtensionBase
        {

            public override void OnRefreshMilestones()
            {
                milestonesManager.UnlockMilestone("Basic Road Created");
            }

            public override int OnGetPopulationTarget(int originalTarget, int scaledTarget)
            {
                return 0;
            }

        }

        public class UnlimitedMoneyEconomy : EconomyExtensionBase
        {

            public override long OnUpdateMoneyAmount(long internalMoneyAmount)
            {
                return long.MaxValue;
            }

            public override bool OverrideDefaultPeekResource
            {
                get { return true; }
            }

            public override int OnPeekResource(EconomyResource resource, int amount)
            {
                return amount;
            }

        }

        public void debug(int index, object message)
        {
            PluginManager.MessageType type;
            if (index == (int)PluginManager.MessageType.Message){
                type = PluginManager.MessageType.Message;
            }
            else if (index == (int)PluginManager.MessageType.Warning)
            {
                type = PluginManager.MessageType.Warning;
            }
            else
            {
                type = PluginManager.MessageType.Error;
            }

            DebugOutputPanel.AddMessage(type, message.ToString());
        }
        public void printExceptionDetails(Exception exception)
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, exception.ToString());
        }

        public void dumpObject(object myObject, string variableName)
        {
            string myObjectDetails = variableName + "\n";
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(myObject))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(myObject);
                myObjectDetails += name + ": " + value + "\n";
            }
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, myObjectDetails);
        }
        public void dumpAllBuildings()
        {
            int count = PrefabCollection<BuildingInfo>.LoadedCount();
            for (uint x = 0; x < count; x += 1)
            {
                BuildingInfo aBuildingInfo = PrefabCollection<BuildingInfo>.GetPrefab(x);
                dumpObject(aBuildingInfo, "BuildingInfo " + x + ":");
            }
        }
        public void dumpAllNetInfo()
        {
            int count = PrefabCollection<NetInfo>.LoadedCount();
            for (uint x = 0; x < count; x += 1)
            {
                NetInfo aNetInfo = PrefabCollection<NetInfo>.GetPrefab(x);
                dumpObject(aNetInfo, "aNetInfo " + x + ":");
            }
        }
        public void dumpAllVehicles()
        {
            int count = PrefabCollection<VehicleInfo>.LoadedCount();
            for (uint x = 0; x < count; x += 1)
            {
                VehicleInfo aVehicleInfo = PrefabCollection<VehicleInfo>.GetPrefab(x);
                dumpObject(aVehicleInfo, "VehicleInfo " + x + ":");
            }
        }


    }
} 

