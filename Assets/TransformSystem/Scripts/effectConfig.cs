using UnityEngine;
using Assets.EffectsScripts;

namespace Assets.EffectsScripts
{
    public enum eEffectMode {
        TERMINAL_MODE,
        PARALLEL_MODE,
        DIRECT_MODE };

    public enum eEffectType {
        COMPOUND_TYPE,
        TERMINAL_TYPE1,
        TERMINAL_MOVE_LINE_LOCAL_POS,
        TERMINAL_MOVE_LINE_GLOBAL_POS,
        TERMINAL_MOVE_ARC_LOCAL_POS,
        TERMINAL_MOVE_ARC_GLOBAL_POS,
        TERMINAL_SCALE,
        TERMINAL_ARRIVE,
        TERMINAL_HIDE,
        TERMINAL_CHANGE_SPRITE,
        TERMINAL_FILL_RECT
    };
}

public class effectConfig : MonoBehaviour
{
    public string m_name;
    public eEffectMode m_mode = eEffectMode.TERMINAL_MODE;
    public eEffectType m_type = eEffectType.COMPOUND_TYPE;

    public GameObject m_control_object = null;

    public float m_max_time = 0;
    public float m_current_time = 0;
    private float m_delay = 0;
    public float m_delay_value = 0;

    private bool m_is_end = false;
    private bool m_is_begin = true;
    private bool m_is_ready_for_remove = false;
    private bool m_is_visible_state = false;

    public bool m_is_state_normalize = false;
    public bool m_is_root_reset_permission = false;
    public bool m_is_child_node_reset_sign = false;
    public bool m_is_switch_off = false;

    public Vector2 m_start_scale;
    public Vector2 m_finish_scale;

    public Vector2 m_start_pos;
    public Vector2 m_finish_pos;

    public Vector2 m_current_scale;
    public Vector2 m_current_pos;

    public GameObject m_obj_finish_pos = null;
    public Sprite m_new_sprite = null;
    private Sprite m_old_sprite = null;

    private double m_last_progress = 0.0f;

    [System.Serializable]
    public class ArcSettings
    {
        [SerializeField] public Vector2 m_arc_shift;
        [SerializeField] public Vector3 m_parabala_params = new Vector3(-0.4f, 0.4f, 0.0f);
        [SerializeField] public double m_last_progress = 0.0f;
    }
    [SerializeField] public ArcSettings m_arc_settings;

    public delegate void EffectFinalAction(effectConfig _config);
    public event EffectFinalAction m_final_action = null;

    public tweenAlgorithmFactory.eTweensAlgorithms m_tween_algorithm_type = tweenAlgorithmFactory.eTweensAlgorithms.Linear;

    public float Delay
    {
        get
        {
            return m_delay;
        }

        set
        {
            m_delay = value;
        }
    }

    public bool Is_end
    {
        get
        {
            return m_is_end;
        }

        set
        {
            m_is_end = value;
        }
    }

    public bool Is_begin
    {
        get
        {
            return m_is_begin;
        }

        set
        {
            m_is_begin = value;
        }
    }

    public bool Is_ready_for_remove
    {
        get
        {
            return m_is_ready_for_remove;
        }

        set
        {
            m_is_ready_for_remove = value;
        }
    }

    public bool Is_visible_state
    {
        get
        {
            return m_is_visible_state;
        }

        set
        {
            m_is_visible_state = value;
        }
    }

    public Sprite Old_sprite
    {
        get
        {
            return m_old_sprite;
        }

        set
        {
            m_old_sprite = value;
        }
    }

    public double Last_progress
    {
        get
        {
            return m_last_progress;
        }

        set
        {
            m_last_progress = value;
        }
    }

    public event InternalTweenAlgorithm m_tween_algorithm = null;

    void Start()
    {
    }

    public void clearConfig()
    {
        Is_end = false;
        Is_begin = true;
        m_current_time = 0;
        m_delay = m_delay_value;
        m_last_progress = 0.0f;
        m_current_pos = m_start_pos;
        m_current_scale = m_start_scale;
        m_arc_settings.m_last_progress = 0.0f;
    }

    public void invokeLast()
    {
        if (m_final_action != null)
        {
            m_final_action.Invoke(this);
        }
    }
}