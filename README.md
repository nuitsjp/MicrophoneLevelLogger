# MicrophoneLevelLogger

MicrophoneLevelLoggerは、マイクの録音レベルを定量的に計測するためのツールです。

私自身の環境では、コロナ禍以降、テレビ会議の機会が劇的に増えました。

このとき、自宅やオフィスから参加した場合に、背後で別の人の話声を拾ってしまうと、聞いている側として大きなストレスを感じてしまいます。とくに同じオフィスから、複数の人が参加した場合に、音声を二重・三重に拾ってしまう場合があり、その場合は聞き取ることも難しくなります。

ヘッドセットやマイクのレビューサイトやレビュー動画をみても、主観による評価が中心で、どれが良いのか選ぶのも困難です。

という訳で、マイクの入力音声を計測して、ノイズキャンセリングのレベルを定量的に評価するツールを作ってみました。

# 参考文献

- [C# Microphone Level Monitor](https://swharden.com/blog/2021-07-03-csharp-microphone/)


# メモ

## 用語

|英語|日本語|説明|
|--|--|--|
|Volume|音量|マイクで録音した音量。単位はDecibel|
|Level|レベル|マイクやスピーカーの入出力設定のレベル|


## ファイアウォールの開放

```cmd
netsh advfirewall firewall add rule name="Http Port 5000" dir=in action=allow protocol=TCP localport=5000
```