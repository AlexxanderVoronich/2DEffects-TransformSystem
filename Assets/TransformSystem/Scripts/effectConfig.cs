using UnityEngine;
using Assets.EffectsScripts;


public class effectConfig : MonoBehaviour
{
    private bool m_is_main_effect = false;
    [Tooltip("Effect name (only for launch root effect)")]
    public string m_root_name = "";
    public eEffectMode m_mode = eEffectMode.TERMINAL_MODE;
    public eEffectType m_type = eEffectType.COMPOUND_TYPE;

    private bool m_is_end = false;
    private bool m_is_begin = true;
    private bool m_is_ready_for_remove = false;
    private bool m_is_visible_state = false;

    [Header("Main settings")]
    [Tooltip("Object controlled by effect")]
    public GameObject m_control_object = null;
    [Tooltip("Effect max time")]
    public float m_max_time = 0;
    public float m_current_time = 0;
    private float m_delay = 0;
    [Tooltip("Effect delay time")]
    public float m_delay_value = 0;

    public bool m_is_state_normalize = false;
    public bool m_is_root_reset_permission = false;
    public bool m_is_child_node_reset_sign = false;
    public bool m_is_switch_off = false;
    public bool m_is_loop = false;
    public float m_delta_time_loop = 0;
    public bool m_is_link_to_last_position = false;
    public tweenAlgorithmFactory.eTweensAlgorithms m_tween_algorithm_type = tweenAlgorithmFactory.eTweensAlgorithms.Linear;

    [Header("Move effect fields")]
    public Vector2 m_start_pos;
    public Vector2 m_finish_pos;
    private Vector2 m_current_pos;
    [System.Serializable]
    public class ArcSettings
    {
        [SerializeField]
        public Vector2 m_arc_shift;
        [SerializeField]
        public Vector3 m_parabala_params = new Vector3(-0.4f, 0.4f, 0.0f);
        [SerializeField]
        public double m_last_progress = 0.0f;
    }
    [SerializeField]
    public ArcSettings m_arc_settings;
    public GameObject m_obj_finish_pos = null;

    [Header("Rotate effect fields")]
    public float m_start_rotate_z;
    public float m_finish_rotate_z;
    public float m_rotate_speed = 1.0f;
    private float m_current_rotate_z;

    [Header("Scale effect fields")]
    public Vector2 m_start_scale;
    public Vector2 m_finish_scale;
    private Vector2 m_current_scale;

    [Header("Change sprite effect fields")]
    public Sprite m_new_sprite = null;
    private Sprite m_old_sprite = null;

    [Header("Animate effect fields")]
    public string m_animate_begin_name_state = "";
    public int m_animate_begin_state_value = 0;
    public string m_animate_end_name_state = "";
    public int m_animate_end_state_value = 0;

    private double m_last_progress = 0.0f;

    public delegate void EffectFinalAction(effectConfig _config);
    public event EffectFinalAction m_final_action = null;

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

    public bool Is_main_effect
    {
        get
        {
            return m_is_main_effect;
        }

        set
        {
            m_is_main_effect = value;
        }
    }

    public float Current_rotate_z
    {
        get
        {
            return m_current_rotate_z;
        }

        set
        {
            m_current_rotate_z = value;
        }
    }

    public Vector2 Current_scale
    {
        get
        {
            return m_current_scale;
        }

        set
        {
            m_current_scale = value;
        }
    }

    public Vector2 Current_pos
    {
        get
        {
            return m_current_pos;
        }

        set
        {
            m_current_pos = value;
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
        Current_pos = m_start_pos;
        Current_scale = m_start_scale;
        Current_rotate_z = m_start_rotate_z;
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