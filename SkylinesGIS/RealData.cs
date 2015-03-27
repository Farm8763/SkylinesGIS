using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using System;
using UnityEngine;
using System.ComponentModel;

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
            dumpAllBuildings();
            dumpAllVehicles();
            spawnVehicle();
        }

        public void spawnVehicle()
        {
            ushort newVehicle;
            VehicleManager vehicleManager = Singleton<VehicleManager>.instance;
            SimulationManager simulationManager = Singleton<SimulationManager>.instance;
            VehicleInfo vehicleInfo = PrefabCollection<VehicleInfo>.GetPrefab(0);
       
            bool vehicleCreated = vehicleManager.CreateVehicle(out newVehicle, ref simulationManager.m_randomizer,
                vehicleInfo, Camera.main.transform.position, 
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

