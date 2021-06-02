using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{

    public ActionController.mAction decition_act, chant_act, skill_selecting_bar_act, stop_act;
    public int chant_level = 0;

    public Image fill_bar;
    public Image skill_selecting_fill_bar;
    public GameObject skill_full_bar_obj;
    public GameObject perfect_range, great_range, good_range, self_bar;
    public Text bar_result_UI_text;
    float bar_length; //選技能跑條的長度

    public GameObject[] skill_groups;
    string chant_input = "";

    public float chant_time
    {
        get { return chant_act.duration - 0.1f; }
        set { chant_act.duration = value; }
    }

    private int[] values;
    private bool[] keys;

    private KeyCode current_pressing_key
    {
        get
        {
            for (int i = 0; i < values.Length; i++)
            {
                keys[i] = Input.GetKey((KeyCode)values[i]);
                if (keys[i])
                {
                    return (KeyCode)values[i];
                }
            };
            return KeyCode.None;
        }
    }
    private bool space_lock = true;

    Dictionary<KeyCode, ActionController.mAction> skillsDict = new Dictionary<KeyCode, ActionController.mAction>();
    Dictionary<KeyCode, string> numDict = new Dictionary<KeyCode, string>() {
        { KeyCode.Alpha1,"1"},{ KeyCode.Alpha2,"2"},{ KeyCode.Alpha3,"3"}
    };

    ActionController actionController;
    SkillManager skillManager;
    WaitForFixedUpdate _wait = new WaitForFixedUpdate();


    readonly float[] bar_steps = new float[] { 0.025f, 0.1f, 0.15f }; //prefect / great / good 寬度
    readonly string[] bar_result_texts = new string[] { "Miss", "Good", "Great", "Perfect" };

    Coroutine chant_coro, decision_coro;


    public void Awake()
    {
        //Set up skill dictionary 
        skillsDict.Add(KeyCode.Space, chant_act);
        values = (int[])System.Enum.GetValues(typeof(KeyCode));
        keys = new bool[values.Length];

    }

    public void Start()
    {
        actionController = GetComponent<ActionController>();
        skillManager = GetComponent<SkillManager>();

        chant_act.action.AddListener(delegate { chant_coro = StartCoroutine(DoChanting()); });
        decition_act.action.AddListener(delegate { decision_coro = StartCoroutine(DecideAct()); });
        skill_selecting_bar_act.action.AddListener(delegate { StartCoroutine(Skill_selecting()); });

        chant_act.callbackAction.AddListener(delegate
        {
            if (chant_coro != null)
                StopCoroutine(chant_coro);
        });

        decition_act.callbackAction.AddListener(delegate
        {
            if (decision_coro != null)
                StopCoroutine(decision_coro);
        });


        bar_length = skill_selecting_fill_bar.rectTransform.sizeDelta.x;

        //拉霸技能列
        skill_full_bar_obj.SetActive(false);
    }

    public void Update()
    {
        // hold "chant" for chant time.
        if (Input.GetKeyDown(KeyCode.Space) && space_lock)//chant_level == 0)
        {
            actionController.AddAction(chant_act);
        }

        //space lock
        if (!space_lock && Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("詠唱 space lock true");
            space_lock = true;
        }
    }
    //詠唱
    public IEnumerator DoChanting()
    {
        float _t = 0;
        Show_skillGroup(99); //隱藏UI

        while (_t < chant_time && Input.GetKey(KeyCode.Space))//holding space
        {
            //Debug.Log("詠唱ing " + chant_level);
            fill_bar.fillAmount = _t / chant_time;
            _t += Time.fixedDeltaTime;
            yield return _wait;
        }

        if (_t < chant_time)
        {
            chant_level = 0;
            Reset();
            Debug.Log("放棄詠唱 " + chant_level);
        }
        else
        {
            Show_skillGroup(chant_level);
            space_lock = false;
            Debug.Log("詠唱完成 " + chant_level + space_lock);
            actionController.AddAction(decition_act);

        }

    }

    private void FixedUpdate()
    {
        //Debug.Log("current input " + current_pressed_key.ToString());
    }

    //詠唱滿=>決定的猶豫期
    IEnumerator DecideAct()
    {
        float _t = 0;
        while (_t < 2)
        {
            //選擇技能大項
            KeyCode _key = current_pressing_key;
            if (_key == KeyCode.Space && space_lock)
            {
                //繼續詠唱
                chant_level = (chant_level + 1) % 2;
                Debug.Log("繼續詠唱 " + chant_level + space_lock);

                actionController.AddAction(stop_act);
                actionController.AddAction(chant_act);

                //隱藏UI
                Show_skillGroup(99);

            }
            else if (_key == KeyCode.Q)
            {
                chant_input = chant_level.ToString() + _key.ToString();
                Show_skillGroup(chant_level, 0);
                actionController.AddAction(skill_selecting_bar_act);

                chant_level = 0;
            }
            else if (_key == KeyCode.W)
            {
                chant_input = chant_level.ToString() + _key.ToString();
                Show_skillGroup(chant_level, 1);
                actionController.AddAction(skill_selecting_bar_act);

                chant_level = 0;
            }
            else if (_key == KeyCode.E)
            {
                chant_input = chant_level.ToString() + _key.ToString();
                Show_skillGroup(chant_level, 2);
                actionController.AddAction(skill_selecting_bar_act);

                chant_level = 0;
            }

            _t += Time.fixedDeltaTime;
            yield return _wait;
        }

        //time out => cancel
        Reset();
        //chant_level = 0;

    }

    IEnumerator Skill_selecting()
    {
        float _t = 0;

        //reset:
        float _step = bar_length / 2 * Time.fixedDeltaTime;
        skill_selecting_fill_bar.gameObject.SetActive(true);
        self_bar.transform.localPosition = new Vector2(-bar_length * 0.5f, 0);

        //拉霸技能列
        skill_full_bar_obj.SetActive(true);

        while (_t < 2)
        {
            //移動bar
            self_bar.transform.localPosition = new Vector2(_step + self_bar.transform.localPosition.x, 0);

            Debug.Log("select " + _t + " pos: " + self_bar.transform.localPosition + " " +
                Vector2.Distance(self_bar.transform.localPosition, perfect_range.transform.localPosition));

            //選擇數字鍵 1-3 鍵盤 =>選擇技能
            KeyCode _key = current_pressing_key;
            string _s = "";
            if (numDict.TryGetValue(_key, out _s))
            {
                chant_input += _s;

                int _rank = GetBarText(self_bar.transform.localPosition);
                bar_result_UI_text.text = bar_result_texts[_rank];
                //執行技能
                actionController.AddAction(skillManager.skill_dicts[chant_input]);

                Debug.Log("結果: " + chant_input+" "+skillManager.skill_dicts.ContainsKey(chant_input));
                skill_full_bar_obj.SetActive(false);
                break;
            }


            _t += Time.fixedDeltaTime;
            yield return _wait;
        }

        Reset();
    }

    //數字鍵選技能評分
    int GetBarText(Vector2 _current_local_pos)
    {
        int _res = 0;
        //perfect
        if (Vector2.Distance(_current_local_pos, perfect_range.transform.localPosition) < bar_steps[0])
        {
            _res = 3;
        }
        //great
        else if (Vector2.Distance(_current_local_pos, great_range.transform.localPosition) < bar_steps[1])
        {
            _res = 2;
        }
        //good
        else if (Vector2.Distance(_current_local_pos, good_range.transform.localPosition) < bar_steps[2])
        {
            _res = 1;
        }
        //else => miss

        return _res;
    }


    void Show_skillGroup(int _index)
    {
        for (int i = 0; i < skill_groups.Length; i++)
        {
            skill_groups[i].SetActive(false);
        }
        if (_index < skill_groups.Length)
        {
            skill_groups[_index].SetActive(true);

            //open all child
            for (int i = 0; i < skill_groups[_index].transform.childCount; i++)
            {
                skill_groups[_index].transform.GetChild(i).gameObject.SetActive(true);
            }
        }

    }
    void Show_skillGroup(int _index, int child_index)
    {
        for (int i = 0; i < skill_groups.Length; i++)
        {
            skill_groups[i].SetActive(false);
        }
        if (_index < skill_groups.Length)
        {
            skill_groups[_index].SetActive(true);

            //open selected the child:
            for (int i = 0; i < skill_groups[_index].transform.childCount; i++)
            {
                if (i == child_index)
                {
                    skill_groups[_index].transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    skill_groups[_index].transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    public void Reset()
    {
        Debug.Log("詠唱 reset");
        Show_skillGroup(99);
        //chant_level = 0;
        skill_selecting_fill_bar.gameObject.SetActive(false);
        actionController.AddAction(stop_act);

    }
}
