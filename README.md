# UnityJenkinsHandson
UnityをJenkinsでビルドするハンズオン


# 使い方 #
後日公開

# 設定項目の指定方法 #
引数でここにあるConfigValue&lt;T&gt;フィールドの名前そのままキーとして設定します。
[Assets/Editor/BuildLogic/Basic/BasicConfig.cs](https://github.com/shinsuke-nakajima/UnityJenkinsHandson/blob/a9ad0a99725ad2e266fdcd1501f2e43ea8a90a57/Assets/Editor/BuildLogic/Basic/BasicConfig.cs)

Jenkins上では例えばこのようにします。引数の書式は「-キー 値」になります。
```
-batchmode -quit -projectPath "${WORKSPACE}" -buildTarget android -executeMethod Assets.Editor.BuildLogic.Console.BuildWithConfigure -f config.txt -buildnumber ${BUILD_NUMBER} -logFile "${WORKSPACE}/editor.log"
```
この例ではBuildNumberという値に${BUILD_NUMBER}を指定しています。


また特殊なキーとしてf,fileを指定できます。 このキーは以下のjavaのpropertiesのような形式でファイルを読み込むことができます。

* 1行1キーとValueのペア
* key=valueの形式で扱う(valueの改行は対応していない)
* #から始まる行は無視される

このような仕様なのでもし設定項目を追加したいと思ったらBasicConfigにConfigValue&lt;T&gt;型のフィールドを追加すれば勝手に追加されます
読み込める形式は次を参照してください。（ここを改造すればもちろん増えます。）

[Assets/Editor/BuildLogic/Console.cs#L116-L143](https://github.com/shinsuke-nakajima/UnityJenkinsHandson/blob/a9ad0a99725ad2e266fdcd1501f2e43ea8a90a57/Assets/Editor/BuildLogic/Console.cs#L116-L143)

# iOSの対応について #

ハンズオンでは取り上げていませんが、 -buildTarget iosを指定してビルドすればxcodeプロジェクトを吐き出すことはできます。そこからはxcodeをビルドするプラグインもあるのでそれに頼りましょう。
