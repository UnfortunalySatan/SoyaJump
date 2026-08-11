using System;
using UnityEngine;

public static class EventBus
{
    //СМЕРТЬ ИГРОКА
    public static Action isPlayerDead; //Вызывается в Border для GameManager
    public static Action isPlayerContinue;
    public static Action isPlayerSurrender;
    //Пауза
    public static Action isPause;
    public static Action isResume;
}
