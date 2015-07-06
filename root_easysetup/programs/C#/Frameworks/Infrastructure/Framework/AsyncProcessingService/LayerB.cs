//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名            :LayerB.cs
//* クラス名クラス名     :
//*
//* 作成者              :Supragyan
//* クラス日本語名       :
//* 更新履歴
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  11/28/2014   Supragyan      Created LayerB class for AsyncProcessing Service
//*  11/28/2014   Supragyan      Created Insert,Update,Select method for AsyncProcessing Service
//**********************************************************************************

// System
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//業務フレームワーク
using Touryo.Infrastructure.Business.Business;

namespace AsyncProcessingService
{
    #region LayerB

    /// <summary>
    /// LayerB class for AsyncProcessing Service
    /// </summary>
    public class LayerB : MyFcBaseLogic
    {
        #region Start

        /// <summary>
        /// Inserts Async Parameter values to Database through LayerD 
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        public void UOC_Start(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.Insert(asyncParameterValue, asyncReturnValue);
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates Async Parameter values to Database through LayerD 
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        private void UOC_Update(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.Update(asyncParameterValue, asyncReturnValue);
        }

        #endregion

        #region UpdateStatus

        /// <summary>
        /// Updates Async Parameter values to Database through LayerD 
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        private void UOC_UpdateStatus(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.UpdateByStatusId(asyncParameterValue, asyncReturnValue);
        }

        #endregion

        #region Select

        /// <summary>
        /// Selects Async Parameter values from Database through LayerD 
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        private void UOC_Select(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.Select(asyncParameterValue, asyncReturnValue);
        }

        #endregion
    }

    #endregion
}

