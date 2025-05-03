#if Doozy
using lLCroweTool.EquipmentSystem.ActionEquipment.WeaponSystem;
using System.Collections;
using UnityEngine;

namespace lLCroweTool.BuildingSystem.FuncStructure.Terminal
{
    public class WeaponHangarStructrueObject : StructureFuncTerminalObject
    {
        //우주선무기들을 가진 격납고건축물
        [SerializeField]private WeaponModule[] weaponModules;


        protected override void AwakeInitStructure()
        {
            base.AwakeInitStructure();
            isUseFuncUpdate = false;
        }

        protected override bool UpdateSatisfactionStructureFuncTerminal()
        {
            //아직 아무행동안함
            return true;
        }

        public void WeaponAction()
        {
            if (GetIsActive())
            {
                for (int i = 0; i < weaponModules.Length; i++)
                {
                    weaponModules[i].Use1stActionEquipment();
                }
            }
        }

        public WeaponModule[] GetWeaponModules()
        {
            return weaponModules;
        }

        
    }
}
#endif