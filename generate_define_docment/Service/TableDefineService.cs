using ClosedXML.Excel;
using generate_define_docment.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace generate_define_docment.Service
{
    class TableDefineService
    {
        private const string PHYSIC_TABLE_NAME_CELL = "B3";
        private const string LOGICAL_TABLE_NAME_CELL = "C3";
        private const string START_PHYSIC_TABLE_COLUMN_NAME_CELL = "B";
        private const string START_LOGICAL_TABLE_COLUMN_NAME_CELL = "C";

        /// <summary>
        /// field
        /// </summary>
        public Model.DirectoryInfo DirectoryInfo { get; set; }

        /// <summary>
        /// エクセルファイルを読み込み、テーブル名、カラム名を解析し、FileInfoオブジェクトに変換します
        /// </summary>
        public void ConvertTableDefineDocment()
        {
            var fileList = new List<FIleInfo>();
            foreach(var _excelFile in this.DirectoryInfo.Files)
            {
                var fileInfo = new FIleInfo
                {
                    Columns = new Dictionary<string, string>()
                };
                // TODO ファイル開いていないかチェックする
                var _workbook = new XLWorkbook(_excelFile);
                var _sheet = _workbook.Worksheet("テーブルカラム");
                var _physicTableName = _sheet.Cell(PHYSIC_TABLE_NAME_CELL);
                var _logicalTableName = _sheet.Cell(LOGICAL_TABLE_NAME_CELL);

                fileInfo.PhysicsTableName = _physicTableName.GetString();
                fileInfo.LogicalTableName = _logicalTableName.GetString();

                var _physicColumns = _sheet.Columns(START_PHYSIC_TABLE_COLUMN_NAME_CELL);
                var index = 1;
                foreach(var cel in _physicColumns.Cells())
                {
                    var val = cel.GetValue<string>();
                    // テーブルカラム開始位置以前　または、テーブルカラムの値が空文字の場合はスキップ
                    if (index++ < 4 || string.IsNullOrEmpty(val))
                    {
                        continue;
                    }
                    var row = cel.Address.RowNumber;
                    if (!fileInfo.Columns.ContainsKey(_sheet.Cell("B" + row).GetString()))
                    {
                        fileInfo.Columns.Add(_sheet.Cell("B"+row).GetString(), _sheet.Cell("C" + row).GetString());
                    }
                }
                fileList.Add(fileInfo);
            }
            this.DirectoryInfo.FileInfos = fileList;
        }

        /// <summary>
        /// 解析したテーブル定義情報を使用し、テーブル定義1ファイルごとに、１マークダウンファイルを作成します
        /// </summary>
        public void GenerateMarkDownFileByTable(string targetDirectory)
        {
            Encoding enc = Encoding.GetEncoding("utf-8");
            this.DirectoryInfo.FileInfos.ForEach(fileInfo => {
                using (StreamWriter writer = new StreamWriter(targetDirectory + string.Format("/テーブル定義_{0}_{1}.md", fileInfo.PhysicsTableName,fileInfo.LogicalTableName), false, enc))
                {
                    writer.WriteLine("#  {0} _ {1}", fileInfo.PhysicsTableName, fileInfo.LogicalTableName);
                    writer.WriteLine("  ");
                    writer.WriteLine("  ");
                    int index = 0;
                    foreach (var kvp in fileInfo.Columns)
                    {
                        if (index++ == 0)
                        {
                            writer.WriteLine("|   {0}  |   {1}  |  ", kvp.Key, kvp.Value);
                            writer.WriteLine("| ---- | ---- |  ");
                        }else
                        {
                            writer.WriteLine("|   {0}  |   {1}  |  ", kvp.Key, kvp.Value);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// ファイルが開いているか否かをチェックする
        /// </summary>
        /// <param name="filePath">対象ファイルパス</param>
        /// <returns>開いているかどうか(開いている＝TRUE, それ以外=FALSE)</returns>
        public Boolean IsOpenFile(string filePath)
        {
            return true;
        }

        /// <summary>
        /// カラム名が定義されている行の最終行を特定する
        /// </summary>
        /// <param name="startCell"></param>
        /// <returns>定義されている行の最終行のCELL情報</returns>
        public string GetUndefinedRow(string startCell)
        {
            return "B10";
        }
    }
}
