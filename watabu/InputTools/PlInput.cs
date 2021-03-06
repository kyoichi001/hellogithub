﻿using UnityEngine;
using System.Collections;
using GamepadInput;
//たぶんこのままだと押してる間ずっと値を返すので面倒。
//あとでおされたときだけに変更したい。
public class PlInput : MonoBehaviour
{
    public const int MaxPlayerNum = 2;//最大プレイ人数
    public const int MaxKey = 5;//キーの数
    public enum ConKind
    {
        NOTHING,//初期状態
        KEYBOARD1,//キーボード１移動がFPS使用のwasd,回転が←、→、ホールドがスペース
        KEYBOARD2,//キーボード２移動が矢印、ｚｘで回転、ホールドがスペース
        JOYCON//joycon
    }
    public enum Key
    {
        KEY_HORIZON,//右が１、左がー１
        KEY_VERTICAL,//上が１、下がー１
        KEY_SUBMIT,//Aボタン　右回転
        KEY_CANCEL,//Bボタン　左回転
        KEY_HOLD//Lボタン or R
    }//
    public class Playerinfo//プレイヤーの情報を持っておく 他に追加するかもしれないので一応クラスにしておく
    {

        public ConKind ConKind;//コントローラーの種類
                               //     int playerNum;//通常配列の添え字と同じプレイヤー番号になるからいらない？
        public Playerinfo()
        {
            ConKind = ConKind.NOTHING;

        }
    }//
   

    static public  Playerinfo[] Player;
    static int[][][] Keystatus;


    public int GetInput(int playerNum, Key key)//Update毎の入力を返す　押されていれば1 or -1 なければ0
    {
        return Keystatus[playerNum][(int)key][0];
    }

    public int GetInput1(int playerNum, int key)//keyをintで渡せる
    {
        if (key < 0 || key >= MaxKey)
        {
            Debug.Log("error in GetInputDown1: keyが範囲外");
            return -2;
        }
        return Keystatus[playerNum][(int)key][0];


    }

    public int GetInputDown(int playerNum, Key key)//押された瞬間だけ Update毎の入力を返す
    {
        if (Keystatus[playerNum][(int)key][0] != Keystatus[playerNum][(int)key][1])//前と違ければ
        {
            return Keystatus[playerNum][(int)key][0];//0-1 なら１を返し1-0なら０を返す、つまり押された時だけ１とか-1を返す
        }
        return 0;
    }

    public int GetInputDown1(int playerNum, int key)//key をint で渡せる関数一応範囲外ならDebug.Logを出します。
    {
        if(key<0 || key >= MaxKey)
        {
            Debug.Log("error in GetInputDown1: keyが範囲外");
            return -2;
        }
        if (Keystatus[playerNum][key][0] != Keystatus[playerNum][key][1])//前と違ければ
        {
            return Keystatus[playerNum][key][0];//0-1 なら１を返し1-0なら０を返す、つまり押された時だけ１とか-1を返す
        }
        return 0;
    }

    public int ChangePlConkind(int playerNum,ConKind ConKind)//ConKindを変更する　変更できれば0、失敗すれば-1
    {
        if (ConKind != ConKind.NOTHING)
        {
            if (Player[playerNum].ConKind == 0)//既に登録されてなければ
            {
                Player[playerNum].ConKind = ConKind;
                return 0;
            }
        }
        else //NOTHINGを代入するとき
        {
            Player[playerNum].ConKind = ConKind.NOTHING;//選びなおすときとか
            return 0;
        }

        return -1;
    }

    public int GetInput2(int playerNum, Key key)//GetInput1した時点での入力状態を返す
    {
        GamepadState state = GamePad.GetState((GamePad.Index)playerNum);//入力状態
        //if (Player[playerNum].ConKind == PlayerInfo.ConKind.KEYBOARD1) {
        switch (Player[playerNum].ConKind)//コントローラーの種類によって
        {
            case ConKind.KEYBOARD1://wasd形式　FPSみたいな左手が移動
                switch (key)
                {
                    case Key.KEY_HORIZON:
                        if (Input.GetKey(KeyCode.D)) return 1;//右
                        if (Input.GetKey(KeyCode.A)) return -1;//左
                        break;
                    case Key.KEY_VERTICAL:
                        if (Input.GetKey(KeyCode.W)) return 1;
                        if (Input.GetKey(KeyCode.S)) return -1;
                        break;
                    case Key.KEY_SUBMIT:
                        if (Input.GetKey(KeyCode.RightArrow)) return 1;
                        break;

                    case Key.KEY_CANCEL:
                        if (Input.GetKey(KeyCode.LeftArrow)) return 1;
                        break;
                    case Key.KEY_HOLD:
                        if (Input.GetKey(KeyCode.Space)) return 1;

                        break;
                }
                return 0;
            //break;   
            case ConKind.KEYBOARD2://矢印形式　右手が移動
                switch (key)
                {
                    case Key.KEY_HORIZON:
                        if (Input.GetKey(KeyCode.RightArrow)) return 1;//右
                        if (Input.GetKey(KeyCode.LeftArrow)) return -1;//左
                        break;
                    case Key.KEY_VERTICAL:
                        if (Input.GetKey(KeyCode.UpArrow)) return 1;
                        if (Input.GetKey(KeyCode.DownArrow)) return -1;
                        break;
                    case Key.KEY_SUBMIT:
                        if (Input.GetKey(KeyCode.X)) return 1;
                        break;

                    case Key.KEY_CANCEL:
                        if (Input.GetKey(KeyCode.Z)) return 1;
                        break;
                    case Key.KEY_HOLD:
                        if (Input.GetKey(KeyCode.Space)) return 1;

                        break;
                }
                return 0;
            //break;  

            case ConKind.JOYCON://joycon
                {
                    switch (key)
                    {
                        case Key.KEY_HORIZON:
                            if (state.LeftTrigger > 0) return 1;//右
                            if (state.LeftTrigger < 0) return -1;//左
                            break;
                        case Key.KEY_VERTICAL:
                            if (state.RightTrigger > 0) return 1;
                            if (state.RightTrigger < 0) return -1;
                            break;
                        case Key.KEY_SUBMIT:
                            if (state.B) return 1;
                            break;

                        case Key.KEY_CANCEL:
                            if (state.A) return 1;
                            break;
                        case Key.KEY_HOLD:
                            if (state.LeftShoulder) return 1;
                            if (state.RightShoulder) return 1;
                            break;
                    }

                    return 0;
                }
                //break;

        }
        return -2;//エラー ConKindがNOTHINGのときとか
    }


    // Use this for initialization
    void Start()
    {
        Keystatus = new int[MaxPlayerNum][][];
        for (int i = 0; i < MaxPlayerNum; i++)
        {
            Keystatus[i] = new int[MaxKey][];
            for(int j = 0; j < MaxKey; j++)
            {
                Keystatus[i][j] = new int[2];
                Keystatus[i][j][0] = new int();
                Keystatus[i][j][1] = new int();
            }
        }
        Player = new Playerinfo[MaxPlayerNum];
        Player[0] = new Playerinfo();
        Player[1] = new Playerinfo();
    }

    // Update is called once per frame
    void Update()//毎秒ボタンを監視する
    {
        for (int i = 0; i < MaxPlayerNum; i++)
        {

            for (int j = 0; j < MaxKey; j++)
            {

                Keystatus[i][j][1] = Keystatus[i][j][0];//前の情報
                Keystatus[i][j][0] = GetInput2(i, (Key)j);//最新
            }
        }
        
    }
}
