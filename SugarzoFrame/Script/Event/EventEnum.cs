using Sirenix.OdinInspector;

public enum EventEnum
{ 
    [LabelText("游戏状态改变")]
    GameStatusChange,
    [LabelText("开始加载游戏")]
    GameLoad,
    [LabelText("开始保存游戏")]
    GameSave,

    DialogueStart,
    DialogueEnd,

    SurveyOpen,
    SurveyClose,
    //第二关启动全局视野
    CameraZoomOut,

    EnterLevel3,

    StartDraging,
    EndDraging,
    StartFishing,
    PassFishing,
    EndFishing,
    PlayerMove, //玩家移动
    WaterAudioPlay, 
    WaterAudioStop,

    ShowTutorial,

    UpdateDialogueType,
}
