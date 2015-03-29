using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using System;
using UnityEngine;
using System.ComponentModel;
using ColossalFramework.Math;

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
            //dumpAllBuildings();
            //dumpAllVehicles();
            //spawnVehicle();
            //roads/paths
            //dumpAllNetInfo();
            BuildingInfo cemetery = PrefabCollection<BuildingInfo>.GetPrefab(310);
            BuildingInfo clinic = PrefabCollection<BuildingInfo>.GetPrefab(306);
            BuildingInfo hospital = PrefabCollection<BuildingInfo>.GetPrefab(307);
            BuildingInfo medicalCenter = PrefabCollection<BuildingInfo>.GetPrefab(308);
            BuildingInfo fireStation = PrefabCollection<BuildingInfo>.GetPrefab(92);
            BuildingInfo fireHouse = PrefabCollection<BuildingInfo>.GetPrefab(93);
            NetInfo pedestrianPavement = PrefabCollection<NetInfo>.GetPrefab(4);
            NetInfo basicRoad = PrefabCollection<NetInfo>.GetPrefab(38);
            NetInfo gravelRoad = PrefabCollection<NetInfo>.GetPrefab(41);
            Vector3 startPos = new Vector3(0, 0, 0);
            Vector3 endPos = new Vector3(600, 0, 600);
            buildBuilding(startPos,0,306);
            //buildRoad(startPos, endPos, 38);
            //buildRoad(startPos, new Vector3(0,0,-800), 38);
        }
        public void buildBuilding(Vector3 position, float angle, uint prefabIndex)
        {
            ushort building;
            BuildingManager instance = Singleton<BuildingManager>.instance;
            BuildingInfo buildingInfo = PrefabCollection<BuildingInfo>.GetPrefab(prefabIndex);
            instance.CreateBuilding(out building, ref Singleton<SimulationManager>.instance.m_randomizer, buildingInfo, 
                position, angle, 0, Singleton<SimulationManager>.instance.m_currentBuildIndex);
        }

        public void buildRoad(Vector3 startVector, Vector3 endVector, uint prefabNumber)
        {
            int maxSegments = 3;
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

            NetInfo netInfo = PrefabCollection<NetInfo>.GetPrefab(prefabNumber);
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

