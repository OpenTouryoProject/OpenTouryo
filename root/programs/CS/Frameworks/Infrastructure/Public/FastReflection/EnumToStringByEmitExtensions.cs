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
//* クラス名        ：EnumToStringByEmitExtensions
//* クラス日本語名  ：EnumToStringByEmitExtensions
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/02/16  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection.Emit;

namespace Touryo.Infrastructure.Public.FastReflection
{
    // 前者を採用
    // - EnumオブジェクトのToStringメソッドはswitch文の100倍以上遅いので
    //   ILGeneratorで動的にswitch文を生成＆コンパイルして高速化する方法 - Qiita
    //   https://qiita.com/higty/items/513296536d3b26fbd033
    // - EnumのToStringが遅いらしいので式木の力を借りた - Qiita
    //   https://qiita.com/Temarin/items/70fc1565c16feeda7303

    // https://twitter.com/xin9le/status/699123907937185792
    // 因みに単なるcache実装をしてみたが、Indexが列挙型と値の2段になるので、
    // ConcurrentDictionaryで上手くスレッドセーフに実装できない。

    /// <summary>EnumToStringByEmitExtensions</summary>
    public static class EnumToStringByEmitExtensions
    {
        /// <summary>スレッドセーフ</summary>
        private static ConcurrentDictionary<Type, MulticastDelegate>
            ToStringMethods = new ConcurrentDictionary<Type, MulticastDelegate>();

        #region public

        /// <summary>GetString（Emit版）</summary>
        /// <typeparam name="T">struct(Enum Field)</typeparam>
        /// <param name="value">値</param>
        /// <returns>列挙型を文字列化</returns>
        public static string ToStringByEmit<T>(this Nullable<T> value) where T : struct
        {
            if (value.HasValue == true)
            {
                return EnumToStringByEmitExtensions.ToStringByEmit(value.Value);
            }
            else
            {
                return "";
            }
        }

        /// <summary>GetString（Emit版）</summary>
        /// <typeparam name="T">struct(Enum Field)</typeparam>
        /// <param name="value">値</param>
        /// <returns>列挙型を文字列化</returns>
        public static string ToStringByEmit<T>(this T value) where T : struct
        {
            // Enum Field
            Type type = typeof(T);

            if (type.IsEnum == false)
            {
                throw new ArgumentException("value must be a enum type");
            }
            else
            {
                MulticastDelegate multicastDelegate = null;

                // MulticastDelegateのロード
                if (!EnumToStringByEmitExtensions.ToStringMethods.TryGetValue(type, out multicastDelegate))
                {
                    if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0)
                    {
                        // FlagsAttributeが無い場合、
                        // MulticastDelegateを生成し、
                        multicastDelegate = EnumToStringByEmitExtensions.CreateToString<T>();
                        // MulticastDelegateをキャッシュ
                        EnumToStringByEmitExtensions.ToStringMethods[type] = multicastDelegate;
                    }
                    else
                    {
                        // FlagsAttributeが有る場合、
                        // MulticastDelegateを生成できないので、
                        // 単なるToString()。// コレが遅いらしい。
                        return value.ToString().Replace(" ", "");
                    }
                }

                // MulticastDelegateでFastReflection
                Func<T, string> f = (Func<T, string>)multicastDelegate;
                return f(value);
            }
        }

        #endregion

        #region private

        /// <summary>MulticastDelegateの生成</summary>
        /// <typeparam name="T">struct(Enum)</typeparam>
        /// <returns>MulticastDelegate</returns>
        private static Func<T, string> CreateToString<T>()
        {
            // [C#][.NET] メタプログラミング入門 - 応用編
            // オブジェクトの文字列変換のメタプログラミング
            // (Reflection.Emit 編) (プログラミング C# - 翔ソフトウェア (Sho's))
            // http://blog.shos.info/archives/2013/11/csharp_metaprogrammingpraxisemit.html
            // 実装したコードを、ILSpyを参照して、

            Type type = typeof(T);

            // メソッドの生成
            DynamicMethod dm = new DynamicMethod(
                "ToStringFromEnum", // メソッド名
                typeof(string),     // 戻値
                new[] { type });    // 引数

            // メソッドのILGenerator
            ILGenerator il = dm.GetILGenerator();

            // Enumをフィールド値（List<long>）を取得
            List<long> values = ((T[])Enum.GetValues(type))
                .Select(el => Convert.ToInt64(el)).ToList();

            // Enumのフィールド名（string[]）を取得
            string[] names = Enum.GetNames(type);

            // Have any value different from index number
            if (values.Where((el, i) => el != i).Any())
            {
                // インデックス番号と異なるフィールド値を持つ

                // 終了ラベル
                Label returnLabel = il.DefineLabel();
                // 戻値定義
                LocalBuilder result = il.DeclareLocal(typeof(string));

                for (int i = 0; i < values.Count; i++)
                {
                    // 命令を命令ストリームに書き込む

                    // 中間ラベル
                    Label tempLabel = il.DefineLabel();

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Conv_I8);
                    il.Emit(OpCodes.Ldc_I8, values[i]);
                    il.Emit(OpCodes.Ceq);                // 比較
                    // ↓一致しない
                    il.Emit(OpCodes.Brfalse, tempLabel); // 中間ラベルへ
                    // ↓一致した
                    il.Emit(OpCodes.Ldstr, names[i]);    // 戻値設定1
                    il.Emit(OpCodes.Stloc, result);      // 戻値設定2
                    il.Emit(OpCodes.Br, returnLabel);    // 終了ラベルへ

                    // 中間ラベルの設定
                    il.MarkLabel(tempLabel);
                }

                // ここまで流れてしまったら、InvalidOperationException
                il.ThrowException(typeof(InvalidOperationException));

                // 終了ラベルの設定
                il.MarkLabel(returnLabel);

                // 戻値設定
                il.Emit(OpCodes.Ldloc, result);
                // 戻る
                il.Emit(OpCodes.Ret);
            }
            else
            {
                // インデックス番号と同じフィールド値を持つ

                // 終了ラベル
                Label returnLabel = il.DefineLabel();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Conv_I8);
                il.Emit(OpCodes.Ldc_I4, 0);
                il.Emit(OpCodes.Conv_I8);
                il.Emit(OpCodes.Clt);
                il.Emit(OpCodes.Brtrue, returnLabel);    // 終了ラベルへ

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Conv_I8);
                il.Emit(OpCodes.Ldc_I4, names.Length - 1);
                il.Emit(OpCodes.Conv_I8);
                il.Emit(OpCodes.Cgt);
                il.Emit(OpCodes.Brtrue, returnLabel);    // 終了ラベルへ

                il.Emit(OpCodes.Ldarg_0);

                // switch-caseラベル
                Label[] caseLabels = new Label[names.Length + 1];
                for (int i = 0; i < names.Length; i++)
                {
                    caseLabels[i] = il.DefineLabel();
                }

                // switch-defaultラベル
                Label defaultCase = il.DefineLabel();
                caseLabels[names.Length] = defaultCase;

                // switchの実装
                il.Emit(OpCodes.Switch, caseLabels);
                for (int i = 0; i < names.Length; i++)
                {
                    // switch-caseラベルの設定
                    il.MarkLabel(caseLabels[i]);
                    il.Emit(OpCodes.Ldstr, names[i]); // 戻る
                    il.Emit(OpCodes.Ret);             // 戻る
                }

                // ここまで流れてしまったら、InvalidOperationException

                // switch-defaultラベルの設定
                il.MarkLabel(defaultCase);
                il.ThrowException(typeof(InvalidOperationException));

                // 終了ラベルの設定
                il.MarkLabel(returnLabel);
                il.ThrowException(typeof(InvalidOperationException));
            }

            // 上記の処理をFunc<T, string>として返す。
            Type f = typeof(Func<,>);
            Type gf = f.MakeGenericType(type, typeof(string));
            return (Func<T, string>)dm.CreateDelegate(gf);
        }

        #endregion
    }
}