# generate_define_docment

テーブル定義書を一気に取込み、GitBook形式に変換、  
全文検索もできるテーブルカラム早見表アプリを開発します。  
生成するドキュメント自体は、GitBook形式になっているので、  
作成自体はIISでよくて、たとえば管理室から有用性を  
認められて、社内公式サーバーに置きなおしても、ただのHTMLになっているだけなのでよいはず

# 構成

- テーブル定義取込
  - バッチ
    - 所定フォルダにテーブル定義をまとめて配置
    - 所定ルールで読み込み、JSONファイルを作成する

  - Webアプリ
    - 生成したJSONファイルを使用して、markdownファイルを1テーブル1ファイルで作成
    - 作成後、GitBookに変換し、所定位置にデプロイ、ツール化する

# 機能

## テーブル定義JSONファイル作成機能

- 所定フォルダに配置されたテーブル定義書を読み込む
- ただし、テーブルが更新され、そのテーブルだけドキュメントを更新しようとしたときの考慮はしない
  - ※ JSONファイルは保存しておき、JSONファイル作成時に、同じテーブルがあるならその情報だけ編集して、JSONファイルを保存すればできそう
- 以下の形式でJSONファイルを作成する
```
{
  "create_date" : "yyyyMMdd",
  "tables": [
    {
      "table_name": "hogehoge",
      "table_logical_name": "hogehoge",
      "columns": [
        {
          "physical_name": "AAA",
          "logical_name": "カラムA"
        },
        {
          "physical_name": "ZZZ",
          "logical_name": "カラムZ"
        }
      ]
    },
    {
      "table_name": "hogehoge",
      "table_logical_name": "hogehoge",
      "columns": [
        {
          "physical_name": "AAA",
          "logical_name": "カラムA"
        },
        {
          "physical_name": "ZZZ",
          "logical_name": "カラムZ"
        }
      ]
    }
  ]
}
```

## GitBook用markdownファイル更新処理

- Summary.md更新処理
  - 読み込んだテーブルの数分のmarkdownファイルを作成するため、まず、Summary.mdの更新を行う
  - "gitbook init"コマンドをプログラムからたたく
  
- {table_name}.md内容編集処理
  - 読み込んだJSONの内容で、{table_name}.mdファイルを編集する

- GitBook最新化処理
  - 最後に、"gitbook build"して、公開用ページを固める

- GitBookデプロイ処理
  - "gitbook build"してできたHTMLファイル群を所定フォルダに配置して完成
