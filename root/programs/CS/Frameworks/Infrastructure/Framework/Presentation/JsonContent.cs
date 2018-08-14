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
//* クラス名        ：JsonContent
//* クラス日本語名  ：Json文字列を使用しHttpContentを生成（HttpResponseMessageで返せる）
//*                   https://vivekcek.wordpress.com/2016/06/26/return-json-from-a-web-api-via-httpresponsemessage/
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/08/14  西野 大介         新規作成
//**********************************************************************************

using System.IO;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using Newtonsoft.Json;

namespace Touryo.Infrastructure.Framework.Presentation
{
    /// <summary>JsonContent</summary>
    public class JsonContent : HttpContent
    {
        /// <summary>MemoryStream</summary>
        private readonly MemoryStream _Stream = new MemoryStream();

        /// <summary>constructor</summary>
        /// <param name="obj">object</param>
        /// <param name="settings">JsonSerializerSettings</param>
        public JsonContent(object obj, JsonSerializerSettings settings)
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
            JsonTextWriter jsonTextWriter = new JsonTextWriter(new StreamWriter(_Stream));
            JsonSerializer serializer = JsonSerializer.Create(settings);
            serializer.Serialize(jsonTextWriter, obj);
            jsonTextWriter.Flush();
            _Stream.Position = 0;
        }

        /// <summary>SerializeToStreamAsync</summary>
        /// <param name="stream">Stream</param>
        /// <param name="context">TransportContext</param>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return _Stream.CopyToAsync(stream);
        }

        /// <summary>TryComputeLength</summary>
        /// <param name="length">long</param>
        /// <returns>bool</returns>
        protected override bool TryComputeLength(out long length)
        {
            length = _Stream.Length;
            return true;
        }
    }
}
