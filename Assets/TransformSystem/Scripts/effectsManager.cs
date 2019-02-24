using System.Collections.Generic;
using UnityEngine;
using Assets.EffectsScripts;
using UnityEngine.UI;

public class effectsManager : MonoBehaviour
{
    private effectsStorage m_effects_storage = null;
    private Dictionary<string, cRunEffect> m_effects = null;
    
    public static cRunEffect generateEffectFrom(effectConfig _root_config)
    {
        _root_config.Is_ready_for_remove = false;
        _root_config.Delay = _root_config.m_delay_value;
        if (_root_config.m_type != eEffectType.COMPOUND_TYPE)
        {
            if(_root_config.m_control_object == null)
            {
                Debug.unityLogger.Log("effectsManager", "Not Compound Effect must have control object: " + _root_config.m_name + "/" + _root_config.m_type.ToString());
                return null;
            }
            _root_config.Is_visible_state = _root_config.m_control_object.activeSelf;
        }

        if (_root_config.m_type == eEffectType.TERMINAL_LOCAL_POS)
        {

            if (_root_config.m_obj_finish_pos != null)
            {
                _root_config.m_finish_pos = new Vector2(_root_config.m_obj_finish_pos.transform.localPosition.x,
                    _root_config.m_obj_finish_pos.transform.localPosition.y);
            }

            _root_config.m_start_pos = _root_config.m_control_object.transform.localPosition;
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_GLOBAL_POS)
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

        cRunEffect root_effect = null;
        if (!_root_config.m_is_switch_off)
        {
            root_effect = cRunEffect.create(_root_config);

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

        if (_root_config.m_type == eEffectType.TERMINAL_LOCAL_POS)
        {
            if (_root_config.m_is_child_node_reset_sign)
            {
                _root_config.m_control_object.transform.localPosition = _root_config.m_start_pos;
                _root_config.m_control_object.SetActive(_root_config.Is_visible_state);
            }
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_GLOBAL_POS)
        {
            if (_root_config.m_is_child_node_reset_sign)
            {
                _root_config.m_control_object.transform.position = _root_config.m_start_pos;
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
                _root_config.m_control_object.SetActive(false);
        }
        else if (_root_config.m_type == eEffectType.TERMINAL_HIDE)
        {
            if (_root_config.m_is_child_node_reset_sign)
                _root_config.m_control_object.SetActive(true);
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
    }

    void FixedUpdate()
    {
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
    }

    void Update()
    {

    }

    public void init()
    {
        m_effects = new Dictionary<string, cRunEffect>();
    }


    public void startEffectForName(string _effect_name, effectConfig.EffectFinalAction _action)
    {
        var effect_config = m_effects_storage.getEffect(_effect_name);

        if(effect_config == null)
        {
            Debug.unityLogger.Log("effectsManager", "Effect was not found: " + _effect_name);
            return;
        }

        if (!isEffectRun(_effect_name))
        {
            cRunEffect root_effect = effectsManager.generateEffectFrom(effect_config);

            if (root_effect != null)
            {
                effect_config.m_final_action += _action;
                addEffect(effect_config.m_name, root_effect);
            }
        }
    }

    public bool isExistRunEffect()
    {
        return m_effects.Count != 0;
    }

    public bool isEffectRun(string _name)
    {
        return m_effects.ContainsKey(_name);
    }

    public void addEffect(string _key, cRunEffect _effect)
    {
        if (!m_effects.ContainsKey(_key))
        {
            m_effects.Add(_key, _effect);
        }
    }

    public void removeEffect(string _key)
    {
        m_effects.Remove(_key);
    }

    public bool updateForKey(string _key)
    {
        if (m_effects.ContainsKey(_key))
        {
            m_effects[_key].update();

            if (m_effects[_key].getConfig().Is_end)
            {
                removeEffect(_key);
                return false;
            }
        }
        return true;
    }
}

