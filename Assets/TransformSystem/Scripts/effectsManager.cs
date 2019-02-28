using System.Collections.Generic;
using UnityEngine;
using Assets.EffectsScripts;
using UnityEngine.UI;

public class effectsManager : MonoBehaviour
{
    [SerializeField] private scriptTrailSystem m_trail_system = null;
    [SerializeField] private bool m_is_trail_system_switch_on = false;
    [SerializeField] private int m_test_effect_index = 0;

    private effectsStorage m_effects_storage = null;
    private Dictionary<string, cRunEffect> m_effects = null;
    private Dictionary<string, cRunEffect> m_effects_for_replace = null;
    private Dictionary<string, effectConfig> m_effects_for_break = null;



    public bool Is_trail_system_switch_on
    {
        get
        {
            return m_is_trail_system_switch_on;
        }

        set
        {
            m_is_trail_system_switch_on = value;
        }
    }

    public cRunEffect generateEffectFrom(effectConfig _root_config)
    {
        _root_config.Is_ready_for_remove = false;
        _root_config.Delay = _root_config.m_delay_value;
        if (_root_config.m_type != eEffectType.COMPOUND_TYPE)
        {
            if(_root_config.m_control_object == null)
            {
                Debug.unityLogger.Log("effectsManager", "Not Compound Effect must have control object: " + _root_config.m_root_name + "/" + _root_config.m_type.ToString());
                return null;
            }
            _root_config.Is_visible_state = _root_config.m_control_object.activeSelf;
        }

        if (_root_config.m_type == eEffectType.TERMINAL_MOVE_LINE_LOCAL_POS ||
            _root_config.m_type == eEffectType.TERMINAL_MOVE_ARC_LOCAL_POS)
        {

            if (_root_config.m_obj_finish_pos != null)
            {
                _root_config.m_finish_pos = new Vector2(_root_config.m_obj_finish_pos.transform.localPosition.x,
                    _root_config.m_obj_finish_pos.transform.localPosition.y);
            }

            _root_config.m_start_pos = _root_config.m_control_object.transform.localPosition;
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_MOVE_LINE_GLOBAL_POS ||
            _root_config.m_type == eEffectType.TERMINAL_MOVE_ARC_GLOBAL_POS)
        {

            if (_root_config.m_obj_finish_pos != null)
            {
                _root_config.m_finish_pos = new Vector2(_root_config.m_obj_finish_pos.transform.position.x,
                    _root_config.m_obj_finish_pos.transform.position.y);
            }

            _root_config.m_start_pos = _root_config.m_control_object.transform.position;
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_CHANGE_SPRITE)
        {
            if (_root_config.m_control_object)
            {
                Image img = _root_config.m_control_object.GetComponent<Image>();
                if (img != null)
                {
                    _root_config.Old_sprite = img.sprite;
                }
            }
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_FILL_RECT)
        {
            if (_root_config.m_control_object)
            {

            }
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_ANIMATION)
        {
           
        }

        cRunEffect root_effect = null;
        if (!_root_config.m_is_switch_off)
        {
            root_effect = cRunEffect.create(_root_config, this);

            foreach (Transform child in _root_config.gameObject.transform)
            {
                effectConfig child_config = child.GetComponent<effectConfig>();
                if (child_config != null && !child_config.m_is_switch_off)
                {
                    cRunEffect child_effect = generateEffectFrom(child_config);
                    if (child_effect != null)
                    {
                        root_effect.add(child_effect);
                    }
                }
            }
        }

        return root_effect;
    }

    public static void resetConfig(effectConfig _root_config)
    {
        if(_root_config == null)
        {
            return;
        }

        if (_root_config.m_type == eEffectType.TERMINAL_ROTATE)
        {
            if (_root_config.m_is_child_node_reset_sign)
            {
                var temp = _root_config.m_control_object.transform.eulerAngles;
                temp.z = _root_config.m_start_rotate_z;
                _root_config.m_control_object.transform.eulerAngles = temp;
                _root_config.m_control_object.SetActive(_root_config.Is_visible_state);
            }
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_MOVE_LINE_LOCAL_POS ||
            _root_config.m_type == eEffectType.TERMINAL_MOVE_ARC_LOCAL_POS)
        {
            if (_root_config.m_is_child_node_reset_sign)
            {
                //_root_config.m_control_object.transform.localPosition = _root_config.m_start_pos;

                Vector3 temp = _root_config.m_start_pos;
                temp.z = _root_config.m_control_object.transform.localPosition.z;
                _root_config.m_control_object.transform.localPosition = temp;

                _root_config.m_control_object.SetActive(_root_config.Is_visible_state);
            }
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_MOVE_LINE_GLOBAL_POS ||
            _root_config.m_type == eEffectType.TERMINAL_MOVE_ARC_GLOBAL_POS)
        {
            if (_root_config.m_is_child_node_reset_sign)
            {
                //_root_config.m_control_object.transform.position = _root_config.m_start_pos;

                Vector3 temp = _root_config.m_start_pos;
                temp.z = _root_config.m_control_object.transform.position.z;
                _root_config.m_control_object.transform.position = temp;

                _root_config.m_control_object.SetActive(_root_config.Is_visible_state);
            }
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_SCALE)
        {
            if (_root_config.m_is_child_node_reset_sign)
            {
                _root_config.m_control_object.transform.localScale = _root_config.m_start_scale;
                _root_config.m_control_object.SetActive(_root_config.Is_visible_state);
            }
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_ARRIVE)
        {
            if (_root_config.m_is_child_node_reset_sign)
            {
                _root_config.m_control_object.SetActive(false);
            }
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_HIDE)
        {
            if (_root_config.m_is_child_node_reset_sign)
            {
                _root_config.m_control_object.SetActive(true);
            }
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_CHANGE_SPRITE)
        {
            if (_root_config.m_is_child_node_reset_sign)
            {
                var img = _root_config.m_control_object.GetComponent<Image>();
                if(img!= null && _root_config.Old_sprite != null)
                {
                    img.sprite = _root_config.Old_sprite;
                }
                _root_config.m_control_object.SetActive(_root_config.Is_visible_state);
            }
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_FILL_RECT)
        {
            if (_root_config.m_is_child_node_reset_sign)
            {
                var img = _root_config.m_control_object.GetComponent<Image>();
                if (img != null)
                {
                    img.fillAmount = 0;
                }
                _root_config.m_control_object.SetActive(_root_config.Is_visible_state);
            }
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_ANIMATION)
        {
            /*if (_root_config.m_is_child_node_reset_sign)
            {
                var img = _root_config.m_control_object.GetComponent<Image>();
                if (img != null)
                {
                    img.fillAmount = 0;
                }
                _root_config.m_control_object.SetActive(_root_config.Is_visible_state);
            }*/
        }

        _root_config.clearConfig();

        foreach (Transform child in _root_config.gameObject.transform)
        {
            effectConfig child_config = child.GetComponent<effectConfig>();
            if (child_config != null)
            {
                resetConfig(child_config);
            }
        }
    }

    void Start()
    {
        init();
        m_effects_storage = GetComponent<effectsStorage>();

        Utilities.setTrailSystem(m_trail_system);

    }

    void FixedUpdate()
    {
        if(m_effects_for_replace.Count > 0)
        {
            foreach(var ef in m_effects_for_replace)
            {
                m_effects[ef.Key] = ef.Value;
            }
            m_effects_for_replace.Clear();
        }

        foreach (var ef in m_effects)
        {
            ef.Value.update();

            if (ef.Value.getConfig().Is_end)
            {
                var config = ef.Value.getConfig();
                removeEffect(ef.Key);
                config.invokeLast();
                config.clearConfig();

                if (config.m_is_root_reset_permission)
                {
                    resetConfig(config);
                }   
                return;
            }
        }

        if (m_effects_for_break.Count > 0)
        {
            foreach (var ef in m_effects_for_break)
            {
                removeEffect(ef.Key);
                ef.Value.invokeLast();
                resetConfig(ef.Value);
            }
            m_effects_for_break.Clear();
        }
    }

    void Update()
    {

    }

    public void init()
    {
        m_effects = new Dictionary<string, cRunEffect>();
        m_effects_for_replace = new Dictionary<string, cRunEffect>();
        m_effects_for_break = new Dictionary<string, effectConfig>();
    }


    public void startEffectForName(string _effect_name, effectConfig.EffectFinalAction _action, bool _loop_mode = false)
    {
        var effect_config = m_effects_storage.getEffect(_effect_name);

        if(effect_config == null)
        {
            Debug.unityLogger.Log("effectsManager", "Effect was not found: " + _effect_name);
            return;
        }

        if (!isEffectRun(_effect_name))
        {
            effect_config.m_is_loop = _loop_mode;
            cRunEffect root_effect = generateEffectFrom(effect_config);
            effect_config.Is_main_effect = true;

            if (root_effect != null)
            {
                effect_config.m_final_action += _action;
                addEffect(effect_config.m_root_name, root_effect);
            }
        }
    }

    public void restartLoopEffect(effectConfig _effect_config)
    {
        if (_effect_config == null)
        {
            Debug.unityLogger.Log("effectsManager", "Loop effect is null");
            return;
        }

        if (_effect_config.Is_main_effect)
        {
            _effect_config.m_delay_value = _effect_config.m_delta_time_loop;
            cRunEffect run = generateEffectFrom(_effect_config);

            if (run != null)
            {
                //_effect_config.m_final_action += _action;
                replaceEffect(_effect_config.m_root_name, run);
            }
        }
        else
        {
            Debug.unityLogger.Log("effectsManager", "Loop effect is not main");
        }
    }

    public void breakEffectForName(string _effect_name)
    {
        var effect_config = m_effects_storage.getEffect(_effect_name);

        if (effect_config == null)
        {
            Debug.unityLogger.Log("effectsManager", "Effect was not found: " + _effect_name);
            return;
        }

        m_effects_for_break.Add(_effect_name, effect_config);
    }


    public bool isExistRunEffect()
    {
        return m_effects.Count != 0;
    }

    public bool isEffectRun(string _name)
    {
        return m_effects.ContainsKey(_name);
    }

    public void addEffect(string _key, cRunEffect _run)
    {
        if (!m_effects.ContainsKey(_key))
        {
            m_effects.Add(_key, _run);
        }
    }

    private void replaceEffect(string _key, cRunEffect _run)
    {
        if(_key != "" && _run != null)
        {
            if (m_effects.ContainsKey(_key) && !m_effects_for_replace.ContainsKey(_key))
            {
                m_effects_for_replace.Add(_key, _run);
            }
        }

    }

    public void removeEffect(string _key)
    {
        m_effects.Remove(_key);
    }

    public void clearTrailSystem()
    {
        if(m_trail_system != null)
            m_trail_system.ClearSystem();
    }

    public void runTest()
    {
        var effect_config = m_effects_storage.getEffectByIndex(m_test_effect_index);

        if (effect_config == null)
        {
            return;
        }

        startEffectForName(effect_config.m_root_name, null);
    }

    public void runClearTest()
    {
        clearTrailSystem();

        var effect_config = m_effects_storage.getEffectByIndex(m_test_effect_index);

        if (effect_config == null)
        {
            Debug.unityLogger.Log("effectsManager::runClearTest", "Error! Root effect was not found");
            return;
        }

        if (effect_config.m_root_name != "")
        {
            breakEffectForName(effect_config.m_root_name);
            //resetConfig(effect_config);
        }
        else
        {
            Debug.unityLogger.Log("effectsManager::runClearTest", "Error! Root effect without name!");
        }
    }
}

