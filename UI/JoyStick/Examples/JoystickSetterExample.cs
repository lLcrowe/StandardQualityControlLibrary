using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.UI.JoyStick.Sample
{
    public class JoystickSetterExample : MonoBehaviour
    {
        public VariableJoystick variableJoystick;
        public Text valueText;
        public Image background;

        public Sprite allAxisSprite;
        public Sprite horizontalSprite;
        public Sprite vertiacalSprite;

        public void ModeChanged(int index)
        {
            switch (index)
            {
                case 0:
                    variableJoystick.SetMode(JoystickType.Fixed);
                    break;
                case 1:
                    variableJoystick.SetMode(JoystickType.Floating);
                    break;
                case 2:
                    variableJoystick.SetMode(JoystickType.Dynamic);
                    break;
                default:
                    break;
            }
        }

        public void AxisChanged(int index)
        {
            switch (index)
            {
                case 0:
                    variableJoystick.axisOptions = AxisOptions.Both;
                    background.sprite = allAxisSprite;
                    break;
                case 1:
                    variableJoystick.axisOptions = AxisOptions.Horizontal;
                    background.sprite = horizontalSprite;
                    break;
                case 2:
                    variableJoystick.axisOptions = AxisOptions.Vertical;
                    background.sprite = vertiacalSprite;
                    break;
                default:
                    break;
            }
        }

        public void SnapX(bool value)
        {
            variableJoystick.snapX = value;
        }

        public void SnapY(bool value)
        {
            variableJoystick.snapY = value;
        }

        private void Update()
        {
            valueText.text = "현재 값: " + variableJoystick.direction;
        }
    }
}