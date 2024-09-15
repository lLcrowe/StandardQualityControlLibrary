using UnityEngine;

namespace lLCroweTool.Visual.SpriteFlip
{
    public class SpriteFlipModule : MonoBehaviour
    {
        //해당오브젝트의 스프라이트 랜더러의 뒤집기 관련 내용을 명세한다.
        //뒤집기의 기능은 3가지 
        //1. Sprite Filp
        //2. Rotation Flip
        //3. Scale Flip
        //스태딕을 할까 생각중인데
        //나중에 생각해보자
        
        public bool isRightCurrent = false;
        private SpriteRenderer sr;
        public FlipType flipType;

        private void Awake()
        {
            //스프라이트랜더러 컴포넌트를 가져와서 캐싱해줌ㄴ
            sr = GetComponent<SpriteRenderer>();
        }

        //뒤집기를 스케일로 해준다
        private void FlipScale()
        {
            isRightCurrent = !isRightCurrent;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        //뒤집기를 각도로 해준다.
        private void FlipRotation()
        {
            isRightCurrent = !isRightCurrent;
            transform.Rotate(0f, 180f, 0f);
        }

        //스프라이트랜더러의 플립을 이용한다.
        private void FlipSpriteRenderer()
        {
            isRightCurrent = !isRightCurrent;
            sr.flipX = isRightCurrent;
        }

        //외부에서 사용하는 함수
        //파라미터값 _direction의 x축만을 사용할 수 있게함
        public void SpriteChecker(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                return;
            }
            switch (flipType)
            {
                case FlipType.SpriteFlip:
                    if (direction.x > 0 && !isRightCurrent)//왼쪽
                    {
                        FlipSpriteRenderer();
                    }
                    else if (direction.x < 0 && isRightCurrent)//오른쪽
                    {
                        FlipSpriteRenderer();
                    }
                    break;
                case FlipType.RotateFlip:
                    if (direction.x > 0 && !isRightCurrent)//왼쪽
                    {
                        FlipRotation();
                    }
                    else if (direction.x < 0 && isRightCurrent)//오른쪽
                    {
                        FlipRotation();
                    }
                    break;
                case FlipType.ScaleFlip:
                    if (direction.x > 0 && !isRightCurrent)//왼쪽
                    {
                        FlipScale();
                    }
                    else if (direction.x < 0 && isRightCurrent)//오른쪽
                    {
                        FlipScale();
                    }
                    break;
            }
        }

        //플립타입에 대한 종류를 명세함
        public enum FlipType
        {
            SpriteFlip,
            RotateFlip,
            ScaleFlip,
        }
    }

}

