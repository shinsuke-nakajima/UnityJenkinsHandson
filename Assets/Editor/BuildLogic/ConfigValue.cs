using System;
using UniRx;

namespace Assets.Editor.BuildLogic
{
    public class ConfigValue<T> : ConfigValue,IObservable<T>
    {
        /// <summary>
        /// この設定の値を取得/設定します
        /// </summary>
        public readonly ReactiveProperty<T> Property;

        public T Value
        {
            get { return Property.Value; }
            set { Property.Value = value; }
        }

        private Subject<T> subject;


        public ConfigValue(T value = default(T))
        {
            Property = new ReactiveProperty<T>(value);
        }

        /// <summary>
        /// この設定の値を反映させます。
        /// </summary>
        public override void Configure()
        {
            if (subject == null) return;
            subject.OnNext(Property.Value);
        }

        /// <summary>
        /// 反映時のObservableにSubscribeします
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return (subject ?? (subject = new Subject<T>())).Subscribe(observer);
        }

        /// <summary>
        /// 値を取得します
        /// </summary>
        /// <returns></returns>
        public override object GetValue()
        {
            return Property.Value;
        }

        public override object SetValue(object o)
        {
            return Property.Value = (T)o;
        }

        public override Type GetValueType()
        {
            return typeof(T);
        }
    }

    public abstract class ConfigValue
    {
        public abstract object GetValue();
        public abstract object SetValue(object o);

        public abstract void Configure();

        public abstract Type GetValueType();
    }
}
