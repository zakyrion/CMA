﻿using Model;
using UnityEngine;

namespace View
{
    public class StarGameManager : MonoBehaviour
    {
        // Use this for initialization
        private void Start()
        {
            Debug.Log("Main: " + Main.Instance);
            Main.Instance.SendMessage(new Main.InitGame(null));
        }
    }
}