using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //Q 一階
    public ActionController.mAction s_0_Q_1;
    public ActionController.mAction s_0_Q_2;
    public ActionController.mAction s_0_Q_3;

    //Q 二階
    public ActionController.mAction s_1_Q_1;
    public ActionController.mAction s_1_Q_2;
    public ActionController.mAction s_1_Q_3;

    //W 一階
    public ActionController.mAction s_0_W_1;
    public ActionController.mAction s_0_W_2;
    public ActionController.mAction s_0_W_3;

    //W 二階
    public ActionController.mAction s_1_W_1;
    public ActionController.mAction s_1_W_2;
    public ActionController.mAction s_1_W_3;

    //E 一階
    public ActionController.mAction s_0_E_1;
    public ActionController.mAction s_0_E_2;
    public ActionController.mAction s_0_E_3;

    //E 二階
    public ActionController.mAction s_1_E_1;
    public ActionController.mAction s_1_E_2;
    public ActionController.mAction s_1_E_3;

    public Dictionary<string, ActionController.mAction> skill_dicts = new Dictionary<string, ActionController.mAction>();

    public void Awake()
    {
        //Q 一階
        skill_dicts.Add("0Q1", s_0_Q_1);
        skill_dicts.Add("0Q2", s_0_Q_2);
        skill_dicts.Add("0Q3", s_0_Q_3);
        //Q 二階
        skill_dicts.Add("1Q1", s_1_Q_1);
        skill_dicts.Add("1Q2", s_1_Q_2);
        skill_dicts.Add("1Q3", s_1_Q_3);

        //W 一階
        skill_dicts.Add("0W1", s_0_W_1);
        skill_dicts.Add("0W2", s_0_W_2);
        skill_dicts.Add("0W3", s_0_W_3);
        //W 二階
        skill_dicts.Add("1W1", s_1_W_1);
        skill_dicts.Add("1W2", s_1_W_2);
        skill_dicts.Add("1W3", s_1_W_3);

        //E 一階
        skill_dicts.Add("0E1", s_0_E_1);
        skill_dicts.Add("0E2", s_0_E_2);
        skill_dicts.Add("0E3", s_0_E_3);
        //E 二階
        skill_dicts.Add("1E1", s_1_E_1);
        skill_dicts.Add("1E2", s_1_E_2);
        skill_dicts.Add("1E3", s_1_E_3);

        s_0_Q_1.action.AddListener(delegate {
            Debug.Log("0Q1");
            GetComponent<SpriteRenderer>().color = Random.ColorHSV();
        });
        s_0_Q_2.action.AddListener(delegate { ChangeColor(); });
        s_0_Q_3.action.AddListener(delegate { ChangeColor(); });

        s_1_Q_1.action.AddListener(delegate { ChangeColor(); });
        s_1_Q_2.action.AddListener(delegate { ChangeColor(); });
        s_1_Q_3.action.AddListener(delegate { ChangeColor(); });

        s_0_W_1.action.AddListener(delegate { ChangeColor(); });
        s_0_W_2.action.AddListener(delegate { ChangeColor(); });
        s_0_W_3.action.AddListener(delegate { ChangeColor(); });

        s_1_W_1.action.AddListener(delegate { ChangeColor(); });
        s_1_W_2.action.AddListener(delegate { ChangeColor(); });
        s_1_W_3.action.AddListener(delegate { ChangeColor(); });

        s_0_E_1.action.AddListener(delegate { ChangeColor(); });
        s_0_E_2.action.AddListener(delegate { ChangeColor(); });
        s_0_E_3.action.AddListener(delegate { ChangeColor(); });

        s_1_E_1.action.AddListener(delegate { ChangeColor(); });
        s_1_E_2.action.AddListener(delegate { ChangeColor(); });
        s_1_E_3.action.AddListener(delegate { ChangeColor(); });
    }



    public void ChangeColor()
    {
        Debug.Log("結果 change color");
        GetComponent<SpriteRenderer>().color = Random.ColorHSV();
    }
}
