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
        private const string PHYSIC_TABLE_NAME_CELL = "G3";
        private const string LOGICAL_TABLE_NAME_CELL = "N3";
        private const string START_PHYSIC_TABLE_COLUMN_NAME_CELL = "E";
        private const string START_LOGICAL_TABLE_COLUMN_NAME_CELL = "F";

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
            Console.WriteLine("_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/");
            Console.WriteLine("エクセルファイル読み込み処理を開始します...");
            Console.WriteLine("_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/");
            char[] bars = { '／', '―', '＼', '｜' };
            Console.WriteLine("");
            int cnt = 1;
            int parcent = 100 / this.DirectoryInfo.Files.Length;
            foreach (var _excelFile in this.DirectoryInfo.Files)
            {

                var fileInfo = new FIleInfo
                {
                    Columns = new Dictionary<string, string>()
                };
                // TODO ファイル開いていないかチェックする
                var _workbook = new XLWorkbook(_excelFile);
                var _sheet = _workbook.Worksheet("テーブルレイアウト");
                var _physicTableName = _sheet.Cell(PHYSIC_TABLE_NAME_CELL);
                var _logicalTableName = _sheet.Cell(LOGICAL_TABLE_NAME_CELL);
                // カーソル位置を初期化
                Console.SetCursorPosition(0, Console.CursorTop);

                try
                {

                    fileInfo.PhysicsTableName = _physicTableName.GetString();
                    fileInfo.LogicalTableName = _logicalTableName.GetString();
                }
                catch (Exception)
                {
                    continue;
                }

                // 回転する棒を表示
                Console.Write(bars[cnt % 4]);
                // 進むパーセンテージを表示
                Console.Write("    ____{0} / {1}___ {2} を処理中...", cnt, this.DirectoryInfo.Files.Length, _physicTableName.GetString());
                Console.Write("{0, 4:d0}%", parcent * cnt >= 100 ? 100 : parcent * cnt);

                cnt++;
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
                    if (!fileInfo.Columns.ContainsKey(_sheet.Cell("E" + row).GetString()))
                    {
                        fileInfo.Columns.Add(_sheet.Cell("E"+row).GetString(), _sheet.Cell("F" + row).GetString());
                    }
                }
                fileList.Add(fileInfo);
            }
            this.DirectoryInfo.FileInfos = fileList;
        }

        /// <summary>
        /// 解析したテーブル定義情報を使用し、テーブル定義1ファイルごとに、１マークダウンファイルを作成します
        /// </summary>
        public void GenerateMarkDownFileByTable()
        {
            Console.WriteLine("");
            Console.WriteLine("_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/");
            Console.WriteLine("マークダウンファイル化処理を開始します...");
            Console.WriteLine("_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/");
            Encoding enc = Encoding.GetEncoding("utf-8");
            char[] bars = { '／', '―', '＼', '｜' };
            Console.WriteLine("");
            int cnt = 1;
            int parcent = 100 / this.DirectoryInfo.FileInfos.Count;
            this.DirectoryInfo.FileInfos.ForEach(fileInfo => {
                // 回転する棒を表示
                Console.Write(bars[cnt % 4]);
                // 進むパーセンテージを表示
                Console.Write("    ____{0} / {1} を処理中...", cnt, this.DirectoryInfo.FileInfos.Count);
                Console.Write("{0, 4:d0}%", parcent * cnt >= 100 ? 100 : parcent * cnt);

                cnt++;
                using (StreamWriter writer = new StreamWriter(this.DirectoryInfo.TargetDirectoryPath+ string.Format("/テーブル定義_{0}_{1}.md", fileInfo.PhysicsTableName,fileInfo.LogicalTableName), false, enc))
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
                // カーソル位置を初期化
                Console.SetCursorPosition(0, Console.CursorTop);
                // （進行が見えるように）処理を100ミリ秒間休止
                System.Threading.Thread.Sleep(1000);
            });
            Console.WriteLine("");
            Console.WriteLine("");
        }

        /// <summary>
        /// [gitbook用]README.md作成処理
        /// </summary>
        public void GenerateReadmeForGitBook()
        {
            Encoding enc = Encoding.GetEncoding("utf-8");
            using (StreamWriter writer = new StreamWriter(this.DirectoryInfo.TargetDirectoryPath + string.Format("/README.md"), false, enc))
            {
                writer.WriteLine("#  Instruction");
            }
        }

        public void GenerateSummaryForGitBook()
        {
            Encoding enc = Encoding.GetEncoding("utf-8");

            using (StreamWriter writer = new StreamWriter(this.DirectoryInfo.TargetDirectoryPath + string.Format("/SUMMARY.md"), false, enc))
            {
                writer.WriteLine("#  Summary  ");
                this.DirectoryInfo.FileInfos.ForEach(fileInfo =>
                {
                    writer.WriteLine("* [{0}]({1})", string.Format("テーブル定義_{0}_{1}.md", fileInfo.PhysicsTableName, fileInfo.LogicalTableName), string.Format("テーブル定義_{0}_{1}.md", fileInfo.PhysicsTableName, fileInfo.LogicalTableName));
                });
            }
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
