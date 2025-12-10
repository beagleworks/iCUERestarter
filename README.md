# iCUE Restarter

[English](README.en.md)

Corsair iCUE がスリープ復帰後にキーボードを認識しなくなる問題を解決するためのシステムトレイ常駐アプリです。

## 問題

iCUE はスリープから復帰した後、キーボードを見失い、ライティング制御が効かなくなることがあります。通常、これを解決するには iCUE を手動で終了して再起動する必要があります。

## 解決策

このアプリはシステムトレイに常駐し、ワンクリックで iCUE を再起動できます。

## 使い方

1. `iCUERestarter.exe` を実行
2. システムトレイにアイコンが表示される
3. **左クリック**: iCUE を即座に再起動
4. **右クリック**: メニュー表示
   - 「iCUE を再起動」
   - 「設定ファイルを開く」
   - 「終了」

## 設定

初回起動時に `settings.json` が作成されます。iCUE のパスを変更する場合は、右クリックメニューから「設定ファイルを開く」を選択して編集してください。

```json
{
  "IcuePath": "C:\\Program Files\\Corsair\\CORSAIR iCUE 5 Software\\iCUE.exe"
}
```

設定変更後はアプリを再起動してください。

## 動作要件

- Windows 10/11
- .NET 8.0 Runtime
- Corsair iCUE 5

## ビルド

```bash
dotnet build -c Release
```

出力先: `bin/Release/net8.0-windows/iCUERestarter.exe`

## スタートアップ登録

Windows 起動時に自動起動させる場合:

1. `Win + R` で「ファイル名を指定して実行」を開く
2. `shell:startup` と入力して Enter
3. 開いたフォルダに `iCUERestarter.exe` のショートカットを配置

## ライセンス

MIT License
