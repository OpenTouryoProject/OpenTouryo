//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
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
//* ƒNƒ‰ƒX–¼        FErrorViewModel
//* ƒNƒ‰ƒX“ú–{Œê–¼  FErrorViewModel
//*
//* ì¬“úŽž        F|
//* ì¬ŽÒ          F¶‹Z
//* XV—š—ð        F
//*
//*  “úŽž        XVŽÒ            “à—e
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ‚w‚w ‚w‚w         ‚w‚w‚w‚w
//**********************************************************************************

using System;

namespace MVC_Sample.Models.ViewModels
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}