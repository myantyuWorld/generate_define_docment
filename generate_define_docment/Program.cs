using System;
using System.IO;
using ClosedXML.Excel;
using generate_define_docment.Model;
using generate_define_docment.Service;

namespace generate_define_docment
{
    class Program
    {
        static void Main(string[] args)
        {
            var tableDefineService = new TableDefineService();
            var _excelDirectoryPath = args[0];
            var _directoryInfo = new Model.DirectoryInfo
            {
                Files = Directory.GetFiles(_excelDirectoryPath),
            };
            tableDefineService.DirectoryInfo = _directoryInfo;

            // 引数のディレクトリ内のテーブル定義書を「すべて」"FileInfo"オブジェクトへと変換します
            tableDefineService.ConvertTableDefineDocment();

            // "FileInfo"オブジェクトを1ファイルずつマークダウンに変換します
            tableDefineService.GenerateMarkDownFileByTable(Directory.GetCurrentDirectory() + "\\markdown");

            // TODO GitBook形式用に変換する
            

        }
    }
}
