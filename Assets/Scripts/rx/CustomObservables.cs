using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class CustomObservables{

    public static IObservable<bool> Latch(
    IObservable<Unit> Tick,
    IObservable<Unit> latchTrue,
    bool initialVal)
    {
        //create custom observable
        return Observable.Create<bool>(observer =>
        {
            bool value = initialVal;

            //create inner subscription to latch
            //whenever latch fires, store true
            var latchSub = latchTrue.Subscribe(_ => value = true);

            //create inner subscription to Tick
            //whenever tick fires, send current value, store false
            var tickSub = Tick.Subscribe(_ =>
            {
                observer.OnNext(value);
                value = false;
            },
            observer.OnError,
            observer.OnCompleted);

            return Disposable.Create(() =>
            {
                latchSub.Dispose();
                tickSub.Dispose();
            });
        });
    }
}
