# MicrophoneLevelLogger

MicrophoneLevelLoggerは、マイクの録音レベルを定量的に計測するためのツールです。

私自身の環境では、コロナ禍以降、テレビ会議の機会が劇的に増えました。

このとき、自宅やオフィスから参加した場合に、背後で別の人の話声を拾ってしまうと、聞いている側として大きなストレスを感じてしまいます。とくに同じオフィスから、複数の人が参加した場合に、音声を二重・三重に拾ってしまう場合があり、その場合は聞き取ることも難しくなります。

ヘッドセットやマイクのレビューサイトやレビュー動画をみても、主観による評価が中心で、どれが良いのか選ぶのも困難です。

という訳で、マイクの入力音声を計測して、ノイズキャンセリングのレベルを定量的に評価するツールを作ってみました。

## 評価対象

|タイプ|接続|メーカー2|製品|標準価格|
|--|--|--|--|--|
|ヘッドセット|有線|Logicool|H111r|1,540 |
|ヘッドセット|有線|EPOS|PC 5 CHAT|3,080 |
|ヘッドセット|有線|オーディオテクニカ|ATH-102USB|2,826 |
|ヘッドセット|有線|Logicool|H340r|3,300 |
|ヘッドセット|有線|Logicool|H390|3,960 |
|ヘッドセット|有線|Jabra|Evolve 20 SE|3,980 |
|ヘッドセット|有線|Jabra|Evolve2 30|13,200 |
|ヘッドセット|有線|Jabra|Engage 50 II|29,040 |
|ヘッドセット|有線|オーディオテクニカ|ATH-M50xSTS-USB|27,000 |
|ヘッドセット|無線|Anker|PowerConf H700|12,980 |
|ヘッドセット|無線|Shoks|OpenComm|22,880 |
|耳掛け|無線|BUFFALO|BSHSBE205|1,470 |
|耳掛け|無線|Turtle Beach|Recon Air|4,480 |
|耳掛け|無線|Jabra|Talk 45|7,255 |
|イヤホン|有線|Apple|EarPods|2,780 |
|イヤホン|有線|SteelSeries|Tusq|5,280 |
|イヤホン|有線|エレコム|HS-EP101UNCBK|11,980 |
|イヤホン|無線|Jabra|Elite 85t|26,800 |
|イヤホン|無線|SOUNDPEATS|Air3 Deluxe HS|8,980 |
|イヤホン|無線|SOUNDPEATS|SOUNDPEATS Capsule3 Pro|8,480 |
|イヤホン|無線|Anker|Soundcore Liberty Air 2 Pro|12,980 |
|DAC|有線|Creative|Sound Blaster Play! 4|3,380 |
|DAC|有線|XROUND|XRD-XTA-02|4,981 |
|総額||||222,632 |


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

# 参考文献

- [C# Microphone Level Monitor](https://swharden.com/blog/2021-07-03-csharp-microphone/)

