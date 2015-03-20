using ICities;
using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework.Plugins;
using ColossalFramework;

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
            //DeveloperUI.
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, "stdafas");
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, "Max Map Buildings: " + BuildingManager.MAX_MAP_BUILDINGS.ToString());
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, "Max Building Count: " + BuildingManager.MAX_BUILDING_COUNT.ToString());
            Building myBuilding = (Building) BuildingManager.instance.m_buildings.m_buffer[0];
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, myBuilding.ToString());
            //Utils.OpenInWinFileBrowser("C:/",true); 
            /*BuildingManager myManager = new BuildingManager();
            BuildingInfo myBuildingInfo = new BuildingInfo();
            Vector3 myVector = new Vector3(300,300);
            ColossalFramework.Math.Randomizer myRandomizer = new ColossalFramework.Math.Randomizer(1);
            myManager.AddServiceBuilding(1, ItemClass.Service.FireDepartment);
            ushort myOut;  
            float myAngle = 0;
            int myLenth = 200;
            uint myBuildIndex = 1;
            CemeteryAI myCemetery = new CemeteryAI();
            Building myBuilding = new Building();
            
             */ 

            //myBuilding.
            //myCemetery.CreateBuilding(123456,)
            //myManager.CreateBuilding(myOut, myRandomizer, myBuildingInfo, myVector, myAngle, myLenth, myBuildIndex);
            //BuildingTool.DispatchPlacementEffect()
            // this seems to get the default UIView
            //UIView v = UIView.GetAView();

            //this adds an UIComponent to the view
            //UIComponent uic = v.AddUIComponent(typeof(ExamplePanel));

            // ALTERNATIVELY, this seems to work like the lines above, but is a bit longer:
            // UIView v = UIView.GetAView ();
            // GameObject go = new GameObject ("panelthing", typeof(ExamplePanel));
            // UIComponent uic = v.AttachUIComponent (go);
        }

    }
} 

