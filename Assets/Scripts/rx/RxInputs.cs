using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class RxInputs : MonoBehaviour {

    public struct MovementInputs
    {
        public readonly Vector2 Direction2D;
        public readonly bool b_Jump;

        public MovementInputs(Vector2 dir2d, bool jump)
        {
            Direction2D = dir2d;
            b_Jump = jump;
        }
    }

    private static RxInputs s_Instance;
    public static RxInputs Instance
    {
        get
        {
            if (s_Instance == null)
            {
                var go = new GameObject();
                go.name = "RxInputs";
                s_Instance = go.AddComponent<RxInputs>();
            }

            return s_Instance;
        }
    }

    private IObservable<Unit> Jump;

    private IObservable<Vector2> Movement;

    public IObservable<MovementInputs> MoveInputs
    {
        get; private set;
    }

    public IObservable<Vector2> Rotation
    {
        get; private set;
    }

    public ReadOnlyReactiveProperty<bool> Run
    {
        get; private set;
    }

    private void Awake()
    {
        Jump = this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Jump"));
        var jumpLatch = CustomObservables.Latch(this.FixedUpdateAsObservable(), Jump, false);

        Movement = this.FixedUpdateAsObservable()
            .Select(_ =>
           {
               float x = Input.GetAxis("Horizontal");
               float y = Input.GetAxis("Vertical");
               Vector2 dir2D = new Vector2(x, y);
               return dir2D;
           });

        MoveInputs = Movement.Zip(jumpLatch, (movement, jump) => new MovementInputs(movement, jump));

        Rotation = this.UpdateAsObservable()
            .Select(_ =>
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                return new Vector2(mouseX, mouseY);
            });

        Run = this.UpdateAsObservable()
            .Select(_ =>
            {
                return Input.GetButton("Fire3");
            }).ToReadOnlyReactiveProperty();
    }
}
