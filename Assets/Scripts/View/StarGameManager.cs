using Akka.Actor;
using Model;
using UnityEngine;

namespace View
{
    public class StarGameManager : MonoBehaviour
    {
        public static string Path => "akka://MainSystem/user/";

        public static ActorSystem Context { get; protected set; }

        // Use this for initialization
        private void Awake()
        {
            Context = ActorSystem.Create("MainSystem");
            Context.ActorOf<Main>("Main");
        }

        private void Start()
        {
            Context.ActorSelection(Path + "*").Tell(new Main.InitGame());
        }
    }
}