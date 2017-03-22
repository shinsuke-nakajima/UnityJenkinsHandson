using System.Collections.Generic;
using Assets.Editor.BuildLogic;
using Assets.Editor.BuildLogic.Basic;
using UnityEditor;
using NUnit.Framework;
using UniRx;

public class ConfigTest {

	[Test]
	public void EditorTest()
	{
        BasicConfig b = new BasicConfig();
        BasicConfig other = new BasicConfig();
        
        Assert.AreEqual(b.ProductName.Property.Value,other.ProductName.Property.Value);
        Assert.IsTrue(b.EqualsValues(other));

	    other.ProductName.Property.Value = "ちがうよ～ん";
        Assert.AreNotEqual(b.ProductName.Property.Value, other.ProductName.Property.Value);
        Assert.IsFalse(b.EqualsValues(other));
    }

    [Test]
    public void ObaservableTest()
    {
        BasicConfig b = new BasicConfig();
        string target = "目標";

        b.ProductName.Property.Subscribe(t => target = t);

        //Subscribeした瞬間にもOnNextが発火する
        Assert.AreEqual(b.ProductName.Value,target);
        b.ProductName.Value = "メルメルメー";
        Assert.AreEqual("メルメルメー", target);
    }

    [Test]
    public void ConfigureTest()
    {
        var an = PlayerSettings.productName;
        BasicConfig b = new BasicConfig();
        b.ProductName.Property.Value = "おためし人形劇";
        b.ProductName.Configure();
        Assert.AreEqual(PlayerSettings.productName, "おためし人形劇");

        //ロードしたら設定を引き継ぐ
        BasicConfig load = new BasicConfig();
        load.Load();
        Assert.AreEqual(load.ProductName.Value, "おためし人形劇");

        PlayerSettings.productName = an;
    }

    [Test]
    public void ConsoleTest()
    {
        Console<BasicConfig> c = new Console<BasicConfig>();
        var config = c.Load(new Dictionary<string, string>() {{"p", "果物屋さん"}});
        Assert.AreEqual(config.ProductName.Value, "果物屋さん");

    }
}
