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
//* クラス名        ：JIS2k4Checker
//* クラス日本語名  ：JIS2004追加文字をチェックするクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/xx/xx  西野 大介         新規作成
//**********************************************************************************

using System.Text;
using System.Globalization;

namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>JIS2004追加文字をチェックするクラス</summary>
    public class JIS2k4Checker
    {
        #region インスタンス変数

        /// <summary>JIS2004追加文字のキャラクタ配列</summary>
        /// <remarks>
        /// ＜参考資料＞
        /// JIS X 0213:2004 対応と新日本語フォント「メイリオ」について
        /// http://www.microsoft.com/japan/windows/products/windowsvista/jp_font/default.mspx
        /// 「Microsoft Windows Vista および Windows Server 2008 における
        ///  JIS X 0213:2004 (JIS2004) 対応について」(Version 1.2)
        /// ↓
        /// 「MSゴシック明朝V5.00フォントアップデート情報.pdf」
        /// ↓
        /// 「MSゴシックファミリー・MS明朝ファミリー バージョン5.00（Windows Vista）で追加された文字」
        /// ※ ４バイト文字（サロゲート ペア文字）を除く
        /// </remarks>
        private char[] _JIS2k4Chars 
            = new char[] {
                '\u0193', '\u01C2', '\u01F8', '\u01F9', '\u02C8', '\u02CC', '\u02D0', '\u02D1', '\u02DE', '\u02E5',
                '\u02E6', '\u02E7', '\u02E8', '\u02E9', '\u0300', '\u0301', '\u0302', '\u0303', '\u0304', '\u0306',
                '\u0308', '\u030B', '\u030C', '\u030F', '\u0318', '\u0319', '\u031A', '\u031C', '\u031D', '\u031E',
                '\u031F', '\u0320', '\u0324', '\u0325', '\u0329', '\u032A', '\u032C', '\u032F', '\u0330', '\u0334',
                '\u0339', '\u033A', '\u033B', '\u033C', '\u033D', '\u0361', '\u09F2', '\u09F3', '\u0E3F', '\u17DB',
                '\u1E3E', '\u1E3F', '\u1F70', '\u1F71', '\u1F72', '\u1F73', '\u2016', '\u2047', '\u2048', '\u2049',
                '\u2051', '\u20AB', '\u20AC', '\u20AD', '\u20AE', '\u20AF', '\u20B0', '\u20B1', '\u2305', '\u2306',
                '\u2318', '\u23BE', '\u23BF', '\u23C0', '\u23C1', '\u23C2', '\u23C3', '\u23C4', '\u23C5', '\u23C6',
                '\u23C7', '\u23C8', '\u23C9', '\u23CA', '\u23CB', '\u23CC', '\u23CE', '\u2423', '\u24EB', '\u24EC',
                '\u24ED', '\u24EE', '\u24EF', '\u24F0', '\u24F1', '\u24F2', '\u24F3', '\u24F4', '\u24F5', '\u24F6',
                '\u24F7', '\u24F8', '\u24F9', '\u24FA', '\u24FB', '\u24FC', '\u24FD', '\u24FE', '\u2616', '\u2617',
                '\u2934', '\u2935', '\u2985', '\u2986', '\u29BF', '\u29FA', '\u29FB', '\u303B', '\u303C', '\u303D',
                '\u3095', '\u3096', '\u3099', '\u309A', '\u309F', '\u30A0', '\u30FF', '\u31F0', '\u31F1', '\u31F2',
                '\u31F3', '\u31F4', '\u31F5', '\u31F6', '\u31F7', '\u31F8', '\u31F9', '\u31FA', '\u31FB', '\u31FC',
                '\u31FD', '\u31FE', '\u31FF', '\u3251', '\u3252', '\u3253', '\u3254', '\u3255', '\u3256', '\u3257',
                '\u3258', '\u3259', '\u325A', '\u325B', '\u325C', '\u325D', '\u325E', '\u325F', '\u32B1', '\u32B2',
                '\u32B3', '\u32B4', '\u32B5', '\u32B6', '\u32B7', '\u32B8', '\u32B9', '\u32BA', '\u32BB', '\u32BC',
                '\u32BD', '\u32BE', '\u32BF', '\u3402', '\u3406', '\u342C', '\u342E', '\u3468', '\u346A', '\u3492',
                '\u34B5', '\u34BC', '\u34C1', '\u34C7', '\u34DB', '\u351F', '\u355D', '\u355E', '\u3563', '\u356E',
                '\u35A6', '\u35A8', '\u35C5', '\u35DA', '\u35F4', '\u3605', '\u364A', '\u3691', '\u3696', '\u3699',
                '\u36CF', '\u3761', '\u3762', '\u376B', '\u376C', '\u3775', '\u378D', '\u37C1', '\u37E2', '\u37E8',
                '\u37F4', '\u37FD', '\u3800', '\u382F', '\u3836', '\u3840', '\u385C', '\u3861', '\u38FA', '\u3917',
                '\u391A', '\u396F', '\u3A6E', '\u3A73', '\u3AD6', '\u3AD7', '\u3AEA', '\u3B0E', '\u3B1A', '\u3B1C',
                '\u3B22', '\u3B6D', '\u3B77', '\u3B87', '\u3B88', '\u3B8D', '\u3BA4', '\u3BB6', '\u3BC3', '\u3BCD',
                '\u3BF0', '\u3C0F', '\u3C26', '\u3CC3', '\u3CD2', '\u3D11', '\u3D1E', '\u3D64', '\u3D9A', '\u3DC0',
                '\u3DD4', '\u3E05', '\u3E3F', '\u3E60', '\u3E66', '\u3E68', '\u3E83', '\u3E94', '\u3F57', '\u3F72',
                '\u3F75', '\u3F77', '\u3FAE', '\u3FC9', '\u3FD7', '\u4039', '\u4058', '\u4093', '\u4105', '\u4148',
                '\u414F', '\u4163', '\u41B4', '\u41BF', '\u41E6', '\u41EE', '\u41F3', '\u4207', '\u420E', '\u4264',
                '\u42C6', '\u42D6', '\u42DD', '\u4302', '\u432B', '\u4343', '\u43EE', '\u43F0', '\u4408', '\u4417',
                '\u441C', '\u4422', '\u4453', '\u445B', '\u4476', '\u447A', '\u4491', '\u44B3', '\u44BE', '\u44D4',
                '\u4508', '\u450D', '\u4525', '\u4543', '\u459D', '\u45B8', '\u45E5', '\u45EA', '\u460F', '\u4641',
                '\u4665', '\u46A1', '\u46AF', '\u470C', '\u4764', '\u47FD', '\u4816', '\u4844', '\u484E', '\u48B5',
                '\u49B0', '\u49E7', '\u49FA', '\u4A04', '\u4A29', '\u4ABC', '\u4B3B', '\u4BC2', '\u4BCA', '\u4BD2',
                '\u4BE8', '\u4C17', '\u4C20', '\u4CC4', '\u4CD1', '\u4D07', '\u4D77', '\u4E0F', '\u4E29', '\u4E2C',
                '\u4E48', '\u4EBB', '\u4EBC', '\u4EC8', '\u4EEB', '\u4F64', '\u4FE6', '\u4FF1', '\u5002', '\u5088',
                '\u5095', '\u50A3', '\u50B1', '\u50BB', '\u50D9', '\u50E1', '\u50F3', '\u5160', '\u5173', '\u517B',
                '\u51C3', '\u51CA', '\u525D', '\u526C', '\u5284', '\u52CA', '\u52D0', '\u52FB', '\u5367', '\u537A',
                '\u537D', '\u53F4', '\u5412', '\u541E', '\u5424', '\u5455', '\u546C', '\u54A0', '\u54C3', '\u54F1',
                '\u54F3', '\u557D', '\u55DD', '\u5607', '\u5628', '\u5647', '\u5653', '\u5676', '\u56B2', '\u5721',
                '\u57D7', '\u57FB', '\u588B', '\u58AA', '\u58C3', '\u58E0', '\u58F4', '\u590D', '\u593D', '\u59F8',
                '\u5A17', '\u5A84', '\u5AF0', '\u5BCE', '\u5C03', '\u5C12', '\u5C5B', '\u5C5F', '\u5CA7', '\u5CAD',
                '\u5CD0', '\u5D10', '\u5D1D', '\u5D20', '\u5D47', '\u5D97', '\u5DA4', '\u5DD1', '\u5DD7', '\u5DE2',
                '\u5E77', '\u5EB9', '\u5ED9', '\u5EF9', '\u5EFD', '\u5F00', '\u5F1E', '\u5FB5', '\u6022', '\u60EE',
                '\u613A', '\u61F5', '\u623E', '\u6261', '\u627B', '\u6285', '\u6299', '\u6332', '\u633B', '\u6359',
                '\u63EB', '\u63ED', '\u63F7', '\u6479', '\u6532', '\u6544', '\u6584', '\u65B5', '\u65B8', '\u65FC',
                '\u663A', '\u6648', '\u665A', '\u6663', '\u666D', '\u66C6', '\u6701', '\u6712', '\u674D', '\u6792',
                '\u67DB', '\u67FC', '\u6810', '\u6818', '\u683E', '\u6849', '\u6890', '\u6899', '\u68AB', '\u68B4',
                '\u68C3', '\u68E4', '\u68F7', '\u6903', '\u6907', '\u6946', '\u69B0', '\u69C0', '\u69CF', '\u69E3',
                '\u69E9', '\u69EA', '\u69F4', '\u69F6', '\u6A33', '\u6A7A', '\u6A94', '\u6AA1', '\u6AF3', '\u6B0B',
                '\u6B65', '\u6B6C', '\u6B77', '\u6B7A', '\u6B81', '\u6BC7', '\u6BC8', '\u6BCF', '\u6BD7', '\u6C0A',
                '\u6C84', '\u6CAA', '\u6CAD', '\u6CED', '\u6CFB', '\u6D00', '\u6D24', '\u6D34', '\u6D58', '\u6D5B',
                '\u6D60', '\u6D80', '\u6D81', '\u6D89', '\u6D8A', '\u6D8D', '\u6DAB', '\u6DAE', '\u6DC2', '\u6DD0',
                '\u6DDA', '\u6E17', '\u6E34', '\u6E4C', '\u6EAB', '\u6EB4', '\u6ED9', '\u6F10', '\u6F25', '\u6F35',
                '\u6F60', '\u6F98', '\u6FBE', '\u6FC9', '\u700A', '\u703A', '\u7047', '\u7069', '\u709F', '\u70EC',
                '\u7108', '\u712E', '\u7151', '\u7153', '\u7196', '\u71AE', '\u7215', '\u7257', '\u72B0', '\u72C0',
                '\u7333', '\u7339', '\u738A', '\u7394', '\u73A8', '\u7413', '\u7453', '\u7488', '\u7497', '\u74A5',
                '\u74BA', '\u74D6', '\u756C', '\u7572', '\u758C', '\u75B0', '\u75B7', '\u75D3', '\u75DD', '\u7618',
                '\u7628', '\u76A1', '\u76AF', '\u76B6', '\u7758', '\u777C', '\u77A4', '\u77A9', '\u7819', '\u782C',
                '\u784F', '\u7851', '\u78F9', '\u78FE', '\u791B', '\u792E', '\u79CC', '\u79CD', '\u7ABE', '\u7B12',
                '\u7B3B', '\u7B79', '\u7B7F', '\u7BF0', '\u7C1E', '\u7C45', '\u7C57', '\u7C6F', '\u7DC0', '\u7DE3',
                '\u7E75', '\u8002', '\u8043', '\u807B', '\u8099', '\u80A4', '\u80C5', '\u80CA', '\u80E6', '\u80F5',
                '\u80FB', '\u810D', '\u813D', '\u81C1', '\u81D6', '\u8204', '\u823C', '\u8249', '\u8257', '\u8279',
                '\u8293', '\u830C', '\u8363', '\u83E1', '\u83E5', '\u8417', '\u845F', '\u8497', '\u84CE', '\u851B',
                '\u853E', '\u85D9', '\u85E1', '\u8624', '\u8639', '\u865B', '\u8687', '\u8689', '\u869D', '\u86E6',
                '\u8751', '\u877C', '\u87E5', '\u87E6', '\u87EC', '\u87F5', '\u886F', '\u88BC', '\u8937', '\u8980',
                '\u8A21', '\u8AD0', '\u8B0D', '\u8B51', '\u8B69', '\u8B9D', '\u8CF1', '\u8D0E', '\u8E0C', '\u8E98',
                '\u8EB6', '\u8F2B', '\u8F4A', '\u8FB4', '\u90F2', '\u9146', '\u91C4', '\u9217', '\u9256', '\u92F7',
                '\u9304', '\u934A', '\u936B', '\u93F1', '\u93F5', '\u9586', '\u9634', '\u96BD', '\u9714', '\u9736',
                '\u9747', '\u9804', '\u98BC', '\u98C7', '\u98CB', '\u98E0', '\u98F0', '\u98F1', '\u99A3', '\u99FC',
                '\u9A0A', '\u9A1A', '\u9A31', '\u9A52', '\u9A58', '\u9AB7', '\u9B1D', '\u9B76', '\u9BEE', '\u9C1D',
                '\u9C65', '\u9C6D', '\u9C7A', '\u9D52', '\u9D73', '\u9D99', '\u9DBD', '\u9DC0', '\u9DE3', '\u9E0D',
                '\u9EBD', '\u9EC3', '\uF91D', '\uF928', '\uF936', '\uF970', '\uF9D0', '\uFA30', '\uFA31', '\uFA32',
                '\uFA33', '\uFA34', '\uFA35', '\uFA36', '\uFA37', '\uFA38', '\uFA39', '\uFA3A', '\uFA3B', '\uFA3C',
                '\uFA3D', '\uFA3E', '\uFA3F', '\uFA40', '\uFA41', '\uFA42', '\uFA43', '\uFA44', '\uFA45', '\uFA46',
                '\uFA47', '\uFA48', '\uFA49', '\uFA4A', '\uFA4B', '\uFA4C', '\uFA4D', '\uFA4E', '\uFA4F', '\uFA50',
                '\uFA51', '\uFA52', '\uFA53', '\uFA54', '\uFA55', '\uFA56', '\uFA57', '\uFA58', '\uFA59', '\uFA5A',
                '\uFA5B', '\uFA5C', '\uFA5D', '\uFA5E', '\uFA5F', '\uFA60', '\uFA61', '\uFA62', '\uFA63', '\uFA64',
                '\uFA65', '\uFA66', '\uFA67', '\uFA68', '\uFA69', '\uFA6A', '\uFDFC', '\uFE45', '\uFE46', '\uFF5F',
                '\uFF60', '\uFFE6'
            };

        /// <summary>JIS2004追加文字の文字列</summary>
        /// <remarks>４バイト文字（サロゲート ペア文字）を除く</remarks>
        private string _JIS2k4String = "";

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        public JIS2k4Checker()
        {
            // キャラクタ配列を文字列に変換して初期化
            StringBuilder sb = new StringBuilder();
            this._JIS2k4String = sb.Append(this._JIS2k4Chars).ToString();
        }

        #endregion

        #region プロパティ

        /// <summary>JIS2004追加文字のキャラクタ配列を返す。</summary>
        /// <remarks>４バイト文字（サロゲート ペア文字）を除く</remarks>
        public char[] JIS2k4Chars
        {
            get
            {
                return this._JIS2k4Chars;
            }
        }

        /// <summary>JIS2004追加文字の文字列を返す。</summary>
        /// <remarks>４バイト文字（サロゲート ペア文字）を除く</remarks>
        public string JIS2k4String
        {
            get
            {
                return this._JIS2k4String;
            }
        }

        #endregion

        #region ユーティリティ

        /// <summary>指定した文字列の文字列情報を取得する。</summary>
        /// <param name="input">入力文字列</param>
        /// <param name="s_length">指定した文字列の文字数（サロゲート ペア文字未対応）</param>
        /// <param name="si_length">指定した文字列の文字数（サロゲート ペア文字対応）</param>
        /// <param name="byte_length">UTF-16（Unicode）でのバイト長</param>
        public void GetStringInfo(string input, out int s_length, out int si_length, out int byte_length)
        {
            // System.Globalization.StringInfoを使用する。
            StringInfo si = new StringInfo(input);

            // 文字数（Length）
            s_length = input.Length;

            // 文字数（LengthInTextElements）
            si_length = si.LengthInTextElements;

            // UTF-16（Unicode）でのバイト長
            byte_length = Encoding.Unicode.GetBytes(input).Length;
        }

        #endregion

        #region ４バイト文字（サロゲート ペア文字）

        #region チェック処理

        /// <summary>
        /// 指定した文字列中に、JIS2004の
        /// ４バイト文字（サロゲート ペア文字）
        /// が含まれているかどうか確認する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：含まれている。
        /// false：含まれていない。
        /// </returns>
        public bool CheckSurrogatesPairChar(string input)
        {
            // 正規表現を使用した方式

            //// 正規表現を使用する。
            //Regex rg = new Regex("^[^\uD800-\uDBFF\uDC00-\uDFFF]+$");

            ////サロゲート ペア文字が文字列中に含まれているか
            ////Regex.IsMatch() メソッドで判定
            //return !(rg.IsMatch(input));

            // オーバーロードを利用した方式。
            
            // サロゲート ペア文字が文字列中に含まれているか判定
            int temp;

            // オーバーロードを利用。
            return this.CheckSurrogatesPairChar(input, out temp);
        }

        /// <summary>
        /// 指定した文字列中に、JIS2004の
        /// ４バイト文字（サロゲート ペア文字）
        /// が含まれているかどうか確認する。
        /// （最初に見つかった文字列のインデックスを返す）
        /// </summary>
        /// <param name="input">入力文字列</param>        
        /// <param name="index">
        /// ４バイト文字（サロゲート ペア文字）が
        /// 見つかった文字列の最初のインデックス
        /// （見つからなかった場合は、-1を返す。）
        /// </param>
        /// <returns>
        /// true：含まれている。
        /// false：含まれていない。
        /// </returns>
        public bool CheckSurrogatesPairChar(string input, out int index)
        {
            // ４バイト文字（サロゲート ペア文字）が文字列中に
            // 含まれているかchar.IsSurrogate()メソッドで判定
            
            int i = 0;

            foreach (char ch in input.ToCharArray())
            {
                if (char.IsSurrogate(ch))
                {
                    // ４バイト文字（サロゲート ペア文字）
                    // が見つかった。

                    // インデックス
                    index = i;
                    // 見つかった。
                    return true;
                }
                else
                {
                    // ４バイト文字（サロゲート ペア文字）
                    // が見つからなかった。
                    i++;
                }
            }
            
            // インデックス
            index = -1;
            // 見つからなかった。
            return false;
        }

        #endregion

        #region 削除処理

        /// <summary>
        /// 指定した文字列からJIS2004の
        /// ４バイト文字（サロゲート ペア文字）を削除する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// 指定した文字列からJIS2004の４バイト文字
        /// （サロゲート ペア文字）を削除した文字列
        /// </returns>
        public string DeleteSurrogatesPairChar(string input)
        {
            return DeleteSurrogatesPairChar(input, "");
        }

        /// <summary>
        /// 指定した文字列からJIS2004の
        /// ４バイト文字（サロゲート ペア文字）
        /// を指定の文字で置換する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="replaceChar">置換文字</param>
        /// <returns>
        /// 指定した文字列からJIS2004の
        /// ４バイト文字（サロゲート ペア文字）
        /// を指定の文字で置換した文字列
        /// </returns>
        public string DeleteSurrogatesPairChar(string input, char replaceChar)
        {
            return DeleteSurrogatesPairChar(input, replaceChar.ToString());
        }

        /// <summary>
        /// 指定した文字列からJIS2004の
        /// ４バイト文字（サロゲート ペア文字）
        /// を指定の文字列で置換する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="replaceString">置換文字列</param>
        /// <returns>
        /// 指定した文字列からJIS2004の
        /// ４バイト文字（サロゲート ペア文字）
        /// を指定の文字列で置換した文字列
        /// </returns>
        public string DeleteSurrogatesPairChar(string input, string replaceString)
        {
            // 前の文字（ワーク）
            char preCh = 'a';
            char prePreCh = 'a';

            // StringBuilder
            StringBuilder sb = new StringBuilder();

            //サロゲート ペアが文字列中に含まれているか
            //char.IsSurrogate()メソッドで判定
            foreach (char ch in input.ToCharArray())
            {
                if (char.IsSurrogate(ch))
                {
                    // サロゲート ペア文字である。

                    if (char.IsSurrogate(preCh))
                    {
                        // 前の文字もサロゲート ペア文字である。

                        if (char.IsSurrogate(prePreCh))
                        {
                            // 前々の文字もサロゲート ペア文字である。

                            // 指定の文字列を追加
                            foreach (char replaceChar in replaceString)
                            {
                                sb.Append(replaceChar);
                            }

                            // ２文字連続した場合は初期化
                            preCh = 'a';
                            prePreCh = 'a';
                        }
                        else
                        {
                            // 前々の文字はサロゲート ペア文字でない。
                            // → 破棄
                        }                        
                    }
                    else
                    {
                        // 前の文字はサロゲート ペア文字でない。

                        // 指定の文字列を追加
                        foreach (char replaceChar in replaceString)
                        {
                            sb.Append(replaceChar);
                        }
                    }                    
                }
                else
                {
                    // サロゲート ペア文字でない。
                    sb.Append(ch);
                }

                // 前の文字を記憶
                prePreCh = preCh;
                preCh = ch;
            }

            // 結果を返す。
            return sb.ToString();
        }

        #endregion

        #endregion

        #region JIS2004追加文字

        #region チェック処理

        /// <summary>
        /// 指定した文字列中に、JIS2004追加文字
        /// が含まれているかどうか確認する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：含まれている。
        /// false：含まれていない。
        /// </returns>
        public bool CheckCharAddedWithJIS2k4(string input)
        {
            // JIS2004追加文字が文字列中に含まれているか判定
            int temp;

            // オーバーロードを利用。
            return this.CheckCharAddedWithJIS2k4(input, out temp);
        }

        /// <summary>
        /// 指定した文字列中に、JIS2004追加文字
        /// が含まれているかどうか確認する。
        /// （最初に見つかった文字列のインデックスを返す）
        /// </summary>
        /// <param name="input">入力文字列</param>        
        /// <param name="index">
        /// JIS2004追加文字が見つかった文字列の最初のインデックス
        /// （見つからなかった場合は、-1を返す）
        /// </param>
        /// <returns>
        /// true：含まれている。
        /// false：含まれていない。
        /// </returns>
        public bool CheckCharAddedWithJIS2k4(string input, out int index)
        {
            // はじめに、４バイト文字（サロゲート ペア文字）を確認する。
            this.CheckSurrogatesPairChar(input, out index);

            // JIS2004追加が文字列中に含まれているか判定

            int i = 0;

            foreach (char ch in input.ToCharArray())
            {
                if (this._JIS2k4String.IndexOf(ch) == -1)
                {
                    // JIS2004追加が見つからなかった。
                    i++;
                }
                else
                {
                    // JIS2004追加文字が見つかった。

                    if (index == -1)
                    {
                        // サロゲート ペア文字が見つかっていない。

                        // インデックス
                        index = i;
                    }
                    else
                    {
                        // サロゲート ペア文字が見つかっている。
                        if (index < i)
                        {
                            // サロゲート ペアの方が先
                        }
                        else
                        {
                            // サロゲート ペアの方が後（か＝）

                            // インデックス
                            index = i;
                        }
                    }                    

                    // 見つかった。
                    return true;   
                }
            }

            // ４バイト文字（サロゲート ペア文字）の結果をチェック
            if (index == -1)
            {
                // 見つからなかった。
                return false;
            }
            else
            {
                // 見つかった。
                return true;
            }
        }

        #endregion

        #region 削除処理

        /// <summary>
        /// 指定した文字列から
        /// JIS2004追加文字を削除する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// 指定した文字列から
        /// JIS2004追加文字を削除した文字列
        /// </returns>
        public string DeleteCharAddedWithJIS2k4(string input)
        {
            return this.DeleteCharAddedWithJIS2k4(input, "");
        }

        /// <summary>
        /// 指定した文字列からJIS2004追加文字
        /// を指定の文字で置換する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="replaceChar">置換文字</param>
        /// <returns>
        /// 指定した文字列からJIS2004追加文字
        /// を指定の文字で置換した文字列
        /// </returns>
        public string DeleteCharAddedWithJIS2k4(string input, char replaceChar)
        {
            return this.DeleteCharAddedWithJIS2k4(input, replaceChar.ToString());
        }

        /// <summary>
        /// 指定した文字列からJIS2004追加文字
        /// を指定の文字列で置換する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="replaceString">置換文字列</param>
        /// <returns>
        /// 指定した文字列からJIS2004追加文字
        /// を指定の文字列で置換した文字列
        /// </returns>
        public string DeleteCharAddedWithJIS2k4(string input, string replaceString)
        {
            // はじめに、４バイト文字（サロゲート ペア文字）を置換する。
            input = this.DeleteSurrogatesPairChar(input, replaceString);

            StringBuilder sb = new StringBuilder();

            // JIS2004追加文字が文字列中に含まれているか？
            foreach (char ch in input.ToCharArray())
            {
                if (this._JIS2k4String.IndexOf(ch) == -1)
                {
                    // JIS2004追加文字でない。
                    sb.Append(ch);
                }
                else
                {
                    // JIS2004追加文字である。

                    // 指定の文字列を追加
                    foreach (char replaceChar in replaceString)
                    {
                        sb.Append(replaceChar);
                    }
                }
            }

            // 結果を返す。
            return sb.ToString();
        }

        #endregion

        #endregion
    }
}
