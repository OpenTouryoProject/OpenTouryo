using System;
using System.Text;
using System.IO;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestDeflateCompression
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            TestDeflateCompression.FileStream();
            MyDebug.OutputDebugAndConsole("--------------------------------------------------");
            TestDeflateCompression.MemoryStream();
        }
        #endregion

        #region private

        private static string hoge = "hogehogehogehogehogehogehogehogehogehogehogehogehogehogehogehoge";

        /// <summary>FileStream</summary>
        private static void FileStream()
        {
            StreamWriter sr = null;
            string inFilePath = "";
            string outFilePath = "";
            string resultFilePath = "";

            #region outFilePathを明示しないパターン

            // inFilePathの作成
            inFilePath = Path.GetTempFileName();
            sr = File.CreateText(inFilePath);
            sr.WriteLine(TestDeflateCompression.hoge);
            sr.Close();

            // 圧縮（outFilePath
            outFilePath = DeflateCompression.Compress(inFilePath);
            // 解凍（resultFilePath
            resultFilePath = DeflateCompression.Decompress(outFilePath);

            // ファイルの比較
            if (ResourceLoader.LoadAsString(inFilePath, Encoding.UTF8) 
                == ResourceLoader.LoadAsString(resultFilePath, Encoding.UTF8))
            {
                MyDebug.OutputDebugAndConsole("DeflateCompression(FileStream)-1", "is working properly.");
            }
            else
            {
                MyDebug.OutputDebugAndConsole("DeflateCompression(FileStream)-1", "is not working properly.");
            }

            // ファイルの削除
            File.Delete(inFilePath);
            File.Delete(outFilePath);
            File.Delete(resultFilePath);
            #endregion

            #region outFilePathを明示するパターン

            // inFilePathの作成
            inFilePath = Path.GetTempFileName();
            sr = File.CreateText(inFilePath);
            sr.WriteLine(TestDeflateCompression.hoge);
            sr.Close();

            // 圧縮（outFilePath
            outFilePath = Path.GetTempFileName();
            outFilePath = outFilePath.Remove(outFilePath.Length - 4); // 拡張し無し
            outFilePath = DeflateCompression.Compress(inFilePath, "zz", outFilePath);

            // 解凍（resultFilePath
            resultFilePath = Path.GetTempFileName();
            resultFilePath = resultFilePath.Remove(outFilePath.Length - 4) + ".dcmp"; // 拡張し有り
            resultFilePath = DeflateCompression.Decompress(outFilePath, "", resultFilePath);

            // ファイルの比較
            if (ResourceLoader.LoadAsString(inFilePath, Encoding.UTF8)
                == ResourceLoader.LoadAsString(resultFilePath, Encoding.UTF8))
            {
                MyDebug.OutputDebugAndConsole("DeflateCompression(FileStream)-2", "is working properly.");
            }
            else
            {
                MyDebug.OutputDebugAndConsole("DeflateCompression(FileStream)-2", "is not working properly.");
            }

            // ファイルの削除
            File.Delete(inFilePath);
            File.Delete(outFilePath);
            File.Delete(resultFilePath);
            #endregion
        }

        /// <summary>MemoryStream</summary>
        private static void MemoryStream()
        {
            // DeflateCompression
            byte[] input = CustomEncode.StringToByte(TestDeflateCompression.hoge, CustomEncode.UTF_8);
            byte[] compressed = DeflateCompression.Compress(input);
            byte[] decompressed = DeflateCompression.Decompress(compressed);
            if (TestDeflateCompression.hoge == CustomEncode.ByteToString(decompressed, CustomEncode.UTF_8))
            {
                MyDebug.OutputDebugAndConsole("DeflateCompression(MemoryStream)", "is working properly.");
            }
            else
            {
                MyDebug.OutputDebugAndConsole("DeflateCompression(MemoryStream)", "is not working properly.");
            }
        }

        #endregion
    }
}