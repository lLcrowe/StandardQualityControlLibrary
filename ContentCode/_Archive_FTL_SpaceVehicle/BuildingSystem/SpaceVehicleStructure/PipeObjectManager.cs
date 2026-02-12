#if Doozy

using lLCroweTool.DesireFuncSystem;

namespace lLCroweTool.BuildingSystem.FuncStructure
{ 
    public class PipeObjectManager
    {
        private static PipeObjectManager instance;
        public static PipeObjectManager Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = new PipeObjectManager();
                }
                return instance;
            }
        }

        private PipeStructureObject refPipeStructure;
        private PipeStructureObject firPipeStructure;
        private PipeStructureObject secPipeStructure;

        /// <summary>
        /// 파이프 업데이트
        /// </summary>
        public void UpdatePipeBridge(PipeBridgeStructureObject pipeBridgeObject)
        {
            //연결된 파이프들을 검색후
            //현재 자기자신보다 낮으면 옮겨줌
            //파이프브릿지에 연결된 파이프길이가 2여야지 작동됨
            if (pipeBridgeObject.GetPipeLength() <= 1)
            {
                return;
            }

            //2개 이상일시만 작동            
            firPipeStructure = pipeBridgeObject.GetPipe();
            secPipeStructure = pipeBridgeObject.GetPipe();

            //같은 욕구상태인지와 없는상태 체크
            if (firPipeStructure.GetDesireKategorie() == secPipeStructure.GetDesireKategorie() || firPipeStructure.GetDesireKategorie() == DesireKategorie.Nothing || secPipeStructure.GetDesireKategorie() == DesireKategorie.Nothing)
            {
                //파이프용량체크후 교환
                //현재용량이 높은곳에서 낮은곳으로 이동                
                //일부려 한쪽만 움직였던거같은데 일단넘어감
                if (firPipeStructure.GetCurValue() < secPipeStructure.GetCurValue())
                {
                    ExchangePipeValue(secPipeStructure, firPipeStructure);
                }
                else
                {
                    ExchangePipeValue(firPipeStructure, secPipeStructure);
                }
            }
        }

        /// <summary>
        /// 파이프현재용량교환
        /// </summary>
        /// <param name="highPipe">높은현재용량파이프</param>
        /// <param name="lowPipe">낮은현재용량파이프</param>
        private void ExchangePipeValue(PipeStructureObject highPipe, PipeStructureObject lowPipe)
        {
            //현재용량이 높은곳에서 낮은곳으로 이동
            int highValue = 0;

            //물체의 움직임은 해당파이프압력에 비례함
            //높은용량을 가진 파이프의 압력을 가져와서 체크
            if (highPipe.GetPipePressure() > highPipe.GetCurValue())
            {
                highValue = highPipe.GetCurValue();
            }
            else
            {
                highValue = highPipe.GetPipePressure();
            }

            lowPipe.AddDesireValueData(ref highValue, highPipe.GetDesireKategorie());//낮은용량파이프에 채워넣음
            int remainValue = highPipe.GetCurValue() - highValue;//높은파이프의 현재량을 남은량에 빼버림
            highPipe.RemoveDesireValueData(ref remainValue);//남은걸 높은용량파이프에 다시줌

            //색깔변환
            highPipe.ChangePipeColor();
            lowPipe.ChangePipeColor();


            ////미리계산
            //lowPipe.SetCurValue(lowPipe.GetCurValue() - temp);


            //highPipe.AddDesireValueData(ref temp, lowPipe.GetDesireKategorie());

            ////계산후 청산
            //lowPipe.SetCurValue(lowPipe.GetCurValue() + temp);

            //if (lowPipe.GetCurValue() <= 0)
            //{
            //    lowPipe.SetDesireKategorie(DesireKategorie.Nothing);
            //}
        }


        public void RefreshPipeRenderer(PipeStructureObject pipe, PipeBridgeStructureObject bridge1, PipeBridgeStructureObject bridge2)
        {
            pipe.pipeLine.Start = bridge1.transform.position;
            pipe.pipeLine.End = bridge2.transform.position;
        }

        //private PipeStructureObject refPipeStructure;

        /////// <summary>
        /////// 파이프 업데이트
        /////// </summary>
        //public virtual void UpdatePipe(PipeStructureObject pipeStructureObject)
        //{
        //    //연결된 파이프들을 검색후
        //    //현재 자기자신보다 낮으면 옮겨줌
        //    if (pipeStructureObject.GetPipeLength() == 0)
        //    {
        //        return;
        //    }

        //    //인덱스 길이체크
        //    if (pipeStructureObject.GetActionIndex() >= pipeStructureObject.GetPipeLength())
        //    {
        //        pipeStructureObject.SetActionIndex(0);
        //    }

        //    //연결여부체크
        //    if (pipeStructureObject.GetPipe(ref refPipeStructure, pipeStructureObject.GetActionIndex()))
        //    {
        //        //같은 욕구상태인자와 없는상태 체크                
        //        if (refPipeStructure.GetDesireKategorie() == pipeStructureObject.GetDesireKategorie() || refPipeStructure.GetDesireKategorie() == DesireKategorie.Nothing)
        //        {
        //            //현재 자기보다 값이 낮으면 옮겨줌
        //            if (refPipeStructure.GetCurValue() < pipeStructureObject.GetCurValue())
        //            {
        //                //변경할 수치
        //                int temp = pipeStructureObject.GetPipePressure();

        //                //미리계산
        //                pipeStructureObject.SetCurValue(pipeStructureObject.GetCurValue() - temp);


        //                refPipeStructure.AddDesireValueData(ref temp, pipeStructureObject.GetDesireKategorie());

        //                //계산후 청산
        //                pipeStructureObject.SetCurValue(pipeStructureObject.GetCurValue() + temp);

        //                if (pipeStructureObject.GetCurValue() <= 0)
        //                {
        //                    pipeStructureObject.SetDesireKategorie(DesireKategorie.Nothing);
        //                }
        //            }

        //            //색깔변환
        //            pipeStructureObject.ChangePipeColor();
        //        }

        //    }

        //    pipeStructureObject.SetActionIndex(pipeStructureObject.GetActionIndex() + 1);
        //}

    }
}

#endif