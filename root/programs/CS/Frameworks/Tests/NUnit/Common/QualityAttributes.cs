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
//* クラス名        ：BasisAttribute, ObservationAttribute
//* クラス日本語名  ：期待値の根拠（検証）と、観測の印（QUALITY.md 7 節）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/29  玄人 幸道         新規作成（#640）
//**********************************************************************************

using System;

using NUnit.Framework;

namespace TestCommon
{
    /// <summary>
    /// **検証**：期待値が、何に基づくかを書く（QUALITY.md 7 節）。
    /// </summary>
    /// <remarks>
    /// 書き方は「出典 節（何の例・どの規定か）」。出典は次のどれか。
    ///   仕様   … RFC 7515 付録 A.1、FIPS 180-4 など。**節まで書く**
    ///   別実装 … 「別実装：.NET の SHA256」のように、独立した実装と突き合わせたもの
    ///   設計   … 「設計：GetHash の XML コメント」のように、Open棟梁 自身の文書に書いた約束
    ///   Issue  … 「#634」のように、不具合の回帰
    /// 1 つのテストに複数書いてよい。クラスに付ければ、そのクラスのテスト全部に掛かる。
    /// NUnit のプロパティ（Basis）になる。**trx には出ない**ので、数えるときはソースの属性を数える。
    /// **消すと「期待値の変更」になる**（検証から観測への格下げ。CheckExpectationChange.ps1 が数える）。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class BasisAttribute : PropertyAttribute
    {
        /// <summary>コンストラクタ</summary>
        /// <param name="source">根拠（出典と節）</param>
        public BasisAttribute(string source) : base("Basis", source) { }
    }

    /// <summary>
    /// **観測**：期待値が、今の振る舞いを写したものであることを書く（QUALITY.md 7 節）。
    /// </summary>
    /// <remarks>
    /// 根拠が無い、または仕様が幅を持つ期待値に付ける。理由（なぜ根拠が無いか・何を固定しているか）を書く。
    /// **観測のテストも落ちる。** 振る舞いが変わったことに気付くためである（後方互換性）。
    /// 落ちたら、不具合ではなく「変わってよいか」を判断する。
    /// 観測から検証へ上げる（Basis に替える）のは、人が根拠を確かめてから。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ObservationAttribute : PropertyAttribute
    {
        /// <summary>コンストラクタ</summary>
        /// <param name="reason">観測にした理由</param>
        public ObservationAttribute(string reason) : base("Observation", reason) { }
    }
}
