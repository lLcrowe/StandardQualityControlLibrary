#if Doozy
using lLCroweTool.Visual.Nozzle;
using UnityEngine;

namespace lLCroweTool.BuildingSystem.FuncStructure.Terminal
{
    [RequireComponent(typeof(CustomNozzle))]
    public class ThrusterStuctureObject : StructureFuncTerminalObject
    {
        //추진체 건축물
        private CustomNozzle nozzle;

        protected override bool UpdateSatisfactionStructureFuncTerminal()
        {
            //아직 아무행동안함
            return true;
        }

        public CustomNozzle GetNozzle()
        {
            return nozzle;
        }

        protected override void AwakeInitStructure()
        {
            base.AwakeInitStructure();
            nozzle = GetComponent<CustomNozzle>();
        }

       
    }
}
#endif