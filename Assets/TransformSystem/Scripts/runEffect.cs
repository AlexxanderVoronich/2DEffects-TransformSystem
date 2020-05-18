using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.EffectsScripts
{
    public class cRunEffect
    {
        private effectsManager m_effects_manager = null;
        public List<cRunEffect> getParts() { return m_parts; }
        public effectConfig getConfig() { return m_config; }

        //config of effect
        private effectConfig m_config;

        //callback to behavior method
        private delegate bool EffectBehaviour(effectConfig _config);
        private event EffectBehaviour m_behaviour = null;

        //list of derived sun-effects
        private List<cRunEffect> m_parts = null;
        //callback to tween method that calculate time position
        private InternalTweenAlgorithm m_internal_tween_algorithm = null;

        //trail system data
        private scriptTrailSystem m_trail_system = null;
        private int m_trail_id= 0;

        public cRunEffect(effectConfig _config)
        {
            m_config = _config;
            m_parts = new List<cRunEffect>();

            if(_config.m_tween_algorithm_type == tweenAlgorithmFactory.eTweensAlgorithms.ParabolaWithParameters)
            {
                m_internal_tween_algorithm = (_1) => { return tweenAlgorithmFactory.parabolaWithParameters(_1, 
                    m_config.m_arc_settings.m_parabala_params.x,
                    m_config.m_arc_settings.m_parabala_params.y,
                    m_config.m_arc_settings.m_parabala_params.z); };
            }
            else
            { 
                m_internal_tween_algorithm = tweenAlgorithmFactory.getAlgorithm(_config.m_tween_algorithm_type);
            }
        }

        public void setManager(effectsManager _manager)
        {
            m_effects_manager = _manager;
        }

        public void update()
        {
            if (m_parts != null)
            {
                foreach (var child_effect in m_parts)
                {
                    if (child_effect.getConfig().IsEnd)
                    {
                        child_effect.getConfig().clearConfig();
                        child_effect.getConfig().IsReadyForRemove = true;
                    }
                }
                m_parts.RemoveAll(x => x.m_config.IsReadyForRemove);
            }

            if (m_config.m_type == eEffectType.COMPOUND_TYPE)
            {
                var dt = Time.deltaTime;

                if (m_config.Delay > 0)
                {
                    m_config.Delay -= dt;
                    if (m_config.Delay > 0)
                    {
                        return;
                    }
                }

                if (m_parts.Count == 0)
                {
                    //if it is main root effect with sign of loop
                    if (m_config.m_is_loop && m_config.IsMainEffect)
                    {
                        m_effects_manager.restartLoopEffect(m_config);
                    }
                    else
                    {
                        m_config.IsEnd = true;
                    }
                }

                if (m_config.m_mode == eEffectMode.PARALLEL_MODE)
                {
                    foreach (var it in m_parts)
                    {
                        it.update();
                    }
                }
                else if (m_config.m_mode == eEffectMode.DIRECT_MODE)
                {
                    if (m_parts.Count() > 0)
                    {
                        m_parts[0].update();
                        return;
                    }
                }
                else if(m_config.m_mode == eEffectMode.TERMINAL_MODE)
                {
                    //nonsense
                }
            }
            else
            {
                if (m_config.m_mode == eEffectMode.TERMINAL_MODE)
                {
                    execute();
                }
                else
                {
                    //nonsense
                }
            }
        }

        void execute()
        {
            bool behaviour_result = false;
            if (m_behaviour != null)
                behaviour_result = m_behaviour.Invoke(m_config);

            if(!behaviour_result)
            {
                return;
            }

            if (m_config.m_control_object != null)
            {
                switch (m_config.m_type)
                {
                    case eEffectType.TERMINAL_ROTATE:
                        {
                            var temp = m_config.m_control_object.transform.eulerAngles;
                            temp.z = m_config.CurrentRotateZ;
                            m_config.m_control_object.transform.eulerAngles = temp;
                            break;
                        }
                    case eEffectType.TERMINAL_MOVE_LINE_LOCAL_POS:
                    case eEffectType.TERMINAL_MOVE_ARC_LOCAL_POS:
                        {
                            //var temp = m_config.m_control_object.transform.localPosition;
                            Vector3 temp = m_config.CurrentPos;
                            temp.z = m_config.m_control_object.transform.localPosition.z;
                            m_config.m_control_object.transform.localPosition = temp;

                            //var point_on_screen = Camera.main.WorldToScreenPoint(m_config.m_control_object.transform.position);
                            var point_on_screen = m_config.m_control_object.transform.position;
                            point_on_screen.z = 0;
                            if (m_trail_system != null)
                            {
                                m_trail_system.AddPointToTrail(m_trail_id, point_on_screen);
                            }
                            break;
                        }

                    case eEffectType.TERMINAL_MOVE_LINE_GLOBAL_POS:
                    case eEffectType.TERMINAL_MOVE_ARC_GLOBAL_POS:
                        {
                            //var temp = m_config.m_control_object.transform.position;
                            Vector3 temp = m_config.CurrentPos;
                            temp.z = m_config.m_control_object.transform.position.z;
                            m_config.m_control_object.transform.position = temp;

                            //var point_on_screen = Camera.main.WorldToScreenPoint(m_config.m_control_object.transform.position);
                            var point_on_screen = m_config.m_control_object.transform.position;
                            point_on_screen.z = 0;
                            if (m_trail_system != null)
                            {
                                m_trail_system.AddPointToTrail(m_trail_id, point_on_screen);
                            }
                            break;
                        }

                    case eEffectType.TERMINAL_SCALE:
                        {
                            m_config.m_control_object.transform.localScale = m_config.CurrentScale;
                            break;
                        }

                    case eEffectType.TERMINAL_ARRIVE:
                    case eEffectType.TERMINAL_HIDE:
                    case eEffectType.TERMINAL_CHANGE_SPRITE:
                    case eEffectType.TERMINAL_FILL_RECT:
                    case eEffectType.TERMINAL_ANIMATION:
                        {
                            break;
                        }

                    case eEffectType.TERMINAL_CHANGE_COLOR:
                        {
                            var img = m_config.m_control_object.GetComponent<Image>();

                            if (img != null)
                            {
                               img.color = m_config.CurrentColor;
                            }
                            break;
                        }

                    case eEffectType.TERMINAL_TEXT_COUNTER:
                        {
                            string pattern = m_config.m_counter_pattern;
                            string temp = String.Format(pattern, m_config.CurrentCounter);
                            Text control = m_config.m_control_object.GetComponent<Text>();
                            if(control != null)
                            {
                                control.text = temp;
                            }
                            break;
                        }
                }
            }
        }

        public void add(cRunEffect _effect)
        {
            if (m_parts != null)
            {
                m_parts.Add(_effect);
            }
        }

        public static cRunEffect create(effectConfig _config, effectsManager _manager)
        {

            cRunEffect effect = null;
            effect = new cRunEffect(_config);
            effect.setManager(_manager);

            if (_config.m_type == eEffectType.COMPOUND_TYPE)
            {

            }
            else if (_config.m_type == eEffectType.TERMINAL_ROTATE)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_type_rotate;
            }
            else if (_config.m_type == eEffectType.TERMINAL_MOVE_LINE_GLOBAL_POS ||
                _config.m_type == eEffectType.TERMINAL_MOVE_LINE_LOCAL_POS)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_move_along_line;
                effect.m_trail_system = Utilities.getTrailSystem();
                effect.m_trail_id = effect.m_trail_system.addTrail();
            }
            else if (_config.m_type == eEffectType.TERMINAL_MOVE_ARC_GLOBAL_POS ||
                _config.m_type == eEffectType.TERMINAL_MOVE_ARC_LOCAL_POS)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_move_along_arc;
                effect.m_trail_system = Utilities.getTrailSystem();
                effect.m_trail_id = effect.m_trail_system.addTrail();
            }
            else if (_config.m_type == eEffectType.TERMINAL_SCALE)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_type_scale;
            }
            else if (_config.m_type == eEffectType.TERMINAL_ARRIVE)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_type_arrive;
                if (effect.m_config.m_is_state_normalize)
                {
                    effect.m_config.m_control_object.SetActive(false);
                }
            }
            else if (_config.m_type == eEffectType.TERMINAL_HIDE)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_type_hide;
                if (effect.m_config.m_is_state_normalize)
                {
                    effect.m_config.m_control_object.SetActive(true);
                }
            }
            else if (_config.m_type == eEffectType.TERMINAL_CHANGE_SPRITE)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_type_change_sprite;
            }
            else if (_config.m_type == eEffectType.TERMINAL_FILL_RECT)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_type_fill_rect;
                if (effect.m_config.m_is_state_normalize)
                {
                    var img = effect.m_config.m_control_object.GetComponent<Image>();
                    if (img != null)
                    {
                        img.fillAmount = 0;
                    }
                }
            }
            else if (_config.m_type == eEffectType.TERMINAL_ANIMATION)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_type_animation;
                if (effect.m_config.m_is_state_normalize)
                {
                    var img = effect.m_config.m_control_object.GetComponent<Image>();
                    if (img != null)
                    {
                        img.fillAmount = 0;
                    }
                }
            }
            else if (_config.m_type == eEffectType.TERMINAL_CHANGE_COLOR)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_type_change_color;
            }
            else if (_config.m_type == eEffectType.TERMINAL_TEXT_COUNTER)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_type_counter;
            }
            return effect;
        }

        public bool behaviour_type1(effectConfig _config)
        {
            return false;
        }

        public bool behaviour_move_along_line(effectConfig _config)
        {
            var dt = Time.deltaTime;

            if (_config.Delay > 0)
            {
                _config.Delay -= dt;
                if (_config.Delay > 0)
                {
                    return false;
                }
            }

            _config.m_current_time += dt;

            if (_config.IsBegin)
            {
                _config.IsBegin = false;
                _config.m_control_object.SetActive(true);

                if (_config.m_is_link_to_last_position)
                {
                    if (_config.m_type == eEffectType.TERMINAL_MOVE_LINE_LOCAL_POS)
                    {
                        _config.m_start_pos = _config.m_control_object.transform.localPosition;
                        _config.CurrentPos = _config.m_control_object.transform.localPosition;
                    }
                    else if (_config.m_type == eEffectType.TERMINAL_MOVE_LINE_GLOBAL_POS)
                    {
                        _config.m_start_pos = _config.m_control_object.transform.position;
                        _config.CurrentPos = _config.m_control_object.transform.position;
                    }
                }
                else
                {
                    _config.CurrentPos = _config.m_start_pos;
                }
            }

            if (_config.m_current_time >= _config.m_max_time)
            {
                _config.m_current_time = _config.m_max_time;
                _config.IsEnd = true;

                //_config.m_current_scale = _config.m_finish_scale;
                _config.CurrentPos = _config.m_finish_pos;
            }
            else
            {
                float delta_time = _config.m_current_time;
                float percents = delta_time / _config.m_max_time;

                if (m_internal_tween_algorithm != null)
                {
                    percents = (float)m_internal_tween_algorithm(percents);
                }

                var main_move = (_config.m_finish_pos - _config.m_start_pos);
                double diff_progress = percents - _config.LastProgress;
                _config.LastProgress = percents;

                Vector2 delta_pos = main_move * (float)diff_progress;
                _config.CurrentPos += delta_pos;
            }
            return true;
        }

        public bool behaviour_move_along_arc(effectConfig _config)
        {
            var dt = Time.deltaTime;

            if (_config.Delay > 0)
            {
                _config.Delay -= dt;
                if (_config.Delay > 0)
                {
                    return false;
                }
            }

            _config.m_current_time += dt;

            if (_config.IsBegin)
            {
                _config.IsBegin = false;
                _config.m_control_object.SetActive(true);

                if (_config.m_is_link_to_last_position)
                {
                    if (_config.m_type == eEffectType.TERMINAL_MOVE_ARC_LOCAL_POS)
                    {
                        _config.m_start_pos = _config.m_control_object.transform.localPosition;
                        _config.CurrentPos = _config.m_control_object.transform.localPosition;
                    }
                    else if(_config.m_type == eEffectType.TERMINAL_MOVE_ARC_GLOBAL_POS)
                    {
                        _config.m_start_pos = _config.m_control_object.transform.position;
                        _config.CurrentPos = _config.m_control_object.transform.position;
                    }
                }
                else
                {
                    _config.CurrentPos = _config.m_start_pos;
                }
            }

            if (_config.m_current_time >= _config.m_max_time)
            {
                _config.m_current_time = _config.m_max_time;
                _config.IsEnd = true;

                _config.CurrentPos = _config.m_finish_pos;
            }
            else
            {
                float delta_time = _config.m_current_time;
                double percents = delta_time / _config.m_max_time;

                var main_move = (_config.m_finish_pos - _config.m_start_pos);
                double diff_progress_1 = percents - _config.LastProgress;
                _config.LastProgress = percents;

                if (m_internal_tween_algorithm != null)
                {
                    percents = m_internal_tween_algorithm(percents);
                }

                Vector2 delta_pos = main_move * (float)diff_progress_1;
                _config.CurrentPos += delta_pos;


                percents = Math.Sin(percents * Math.PI);

                double diff_progress2 = percents - _config.m_arc_settings.m_last_progress;
                _config.m_arc_settings.m_last_progress = percents;
                _config.CurrentPos = _config.CurrentPos + _config.m_arc_settings.m_arc_shift * (float)diff_progress2;
            }
            return true;
        }

        public bool behaviour_type_rotate(effectConfig _config)
        {
            var dt = Time.deltaTime;

            if (_config.Delay > 0)
            {
                _config.Delay -= dt;
                if (_config.Delay > 0)
                {
                    return false;
                }
            }

            _config.m_current_time += dt;

            if (_config.IsBegin)
            {
                _config.IsBegin = false;
                _config.m_control_object.SetActive(true);
            }

            if (_config.m_current_time >= _config.m_max_time)
            {
                _config.m_current_time = _config.m_max_time;
                _config.IsEnd = true;

                _config.CurrentRotateZ = _config.m_finish_rotate_z;
            }
            else
            {
                float delta_time = _config.m_current_time;
                float percents = delta_time / _config.m_max_time;

                if (m_internal_tween_algorithm != null)
                {
                    percents = (float)m_internal_tween_algorithm(percents);
                }

                var delta_size = (_config.m_finish_rotate_z - _config.m_start_rotate_z) * percents;
                _config.CurrentRotateZ = _config.m_start_rotate_z + delta_size * _config.m_rotate_speed;
            }
            return true;
        }

        public bool behaviour_type_scale(effectConfig _config)
        {
            var dt = Time.deltaTime;

            if (_config.Delay > 0)
            {
                _config.Delay -= dt;
                if (_config.Delay > 0)
                {
                    return false;
                }
            }

            _config.m_current_time += dt;

            if (_config.IsBegin)
            {
                _config.IsBegin = false;
                _config.m_control_object.SetActive(true);
            }

            if (_config.m_current_time >= _config.m_max_time)
            {
                _config.m_current_time = _config.m_max_time;
                _config.IsEnd = true;

                _config.CurrentScale = _config.m_finish_scale;
            }
            else
            {
                float delta_time = _config.m_current_time;
                float percents = delta_time / _config.m_max_time;

                if (m_internal_tween_algorithm != null)
                {
                    percents = (float)m_internal_tween_algorithm(percents);
                }

                Vector2 delta_size = (_config.m_finish_scale - _config.m_start_scale) * percents;
                _config.CurrentScale = _config.m_start_scale + delta_size;
            }
            return true;
        }
        
        public bool behaviour_type_arrive(effectConfig _config)
        {
            var dt = Time.deltaTime;

            if (_config.Delay > 0)
            {
                _config.Delay -= dt;
                if (_config.Delay > 0)
                {
                    return false;
                }
            }

            _config.m_current_time += dt;
            _config.m_control_object.SetActive(true);
            _config.IsBegin = false;
            _config.IsEnd = true;
            return true;
        }


        public bool behaviour_type_hide(effectConfig _config)
        {
            var dt = Time.deltaTime;

            if (_config.Delay > 0)
            {
                _config.Delay -= dt;
                if (_config.Delay > 0)
                {
                    return false;
                }
            }

            _config.m_current_time += dt;
            _config.m_control_object.SetActive(false);
            _config.IsBegin = false;
            _config.IsEnd = true;
            return true;
        }

        public bool behaviour_type_change_sprite(effectConfig _config)
        {
            var dt = Time.deltaTime;

            if (_config.Delay > 0)
            {
                _config.Delay -= dt;
                if (_config.Delay > 0)
                {
                    return false;
                }
            }

            _config.m_current_time += dt;
            _config.IsBegin = false;
            _config.IsEnd = true;
            Image img = _config.m_control_object.GetComponent<Image>();
            if(img != null)
            {
                img.sprite = _config.m_new_sprite;
            }
            return true;
        }

        public bool behaviour_type_fill_rect(effectConfig _config)
        {
            var dt = Time.deltaTime;

            if (_config.Delay > 0)
            {
                _config.Delay -= dt;
                if (_config.Delay > 0)
                {
                    return false;
                }
            }

            _config.m_current_time += dt;
            Image img = _config.m_control_object.GetComponent<Image>();
            
            if (_config.IsBegin)
            {
                _config.IsBegin = false;
                _config.m_control_object.SetActive(true);
            }

            if (_config.m_current_time >= _config.m_max_time)
            {
                _config.m_current_time = _config.m_max_time;
                _config.IsEnd = true;

                if (img != null)
                {
                    img.fillAmount = 1;
                }

            }
            else
            {
                float delta_time = _config.m_current_time;
                float percents = delta_time / _config.m_max_time;

                /*if (m_internal_tween_algorithm != null)
                {
                    percents = (float)m_internal_tween_algorithm(percents);
                }*/


                if (img != null)
                {
                    img.fillAmount = percents;
                }
            }
            return true;
        }

        public bool behaviour_type_animation(effectConfig _config)
        {
            var dt = Time.deltaTime;

            if (_config.Delay > 0)
            {
                _config.Delay -= dt;
                if (_config.Delay > 0)
                {
                    return false;
                }
            }

            _config.m_current_time += dt;

            if (_config.IsBegin)
            {
                _config.IsBegin = false;
                _config.m_control_object.SetActive(true);

                Animator anim_control = _config.m_control_object.GetComponent<Animator>();
                if (anim_control != null && _config.m_animate_begin_name_state != "")
                {
                    anim_control.SetInteger(_config.m_animate_begin_name_state, _config.m_animate_begin_state_value);
                }
            }

            if (_config.m_current_time >= _config.m_max_time)
            {
                _config.m_current_time = _config.m_max_time;
                _config.IsEnd = true;

                Animator anim_control = _config.m_control_object.GetComponent<Animator>();
                if (anim_control != null && _config.m_animate_end_name_state != "")
                {
                    anim_control.SetInteger(_config.m_animate_end_name_state, _config.m_animate_end_state_value);
                }
            }


            return true;
        }

        public bool behaviour_type_change_color(effectConfig _config)
        {
            var dt = Time.deltaTime;

            if (_config.Delay > 0)
            {
                _config.Delay -= dt;
                if (_config.Delay > 0)
                {
                    return false;
                }
            }

            _config.m_current_time += dt;

            if (_config.IsBegin)
            {
                _config.IsBegin = false;
                _config.m_control_object.SetActive(true);
            }

            if (_config.m_current_time >= _config.m_max_time)
            {
                _config.m_current_time = _config.m_max_time;
                _config.IsEnd = true;

                _config.CurrentColor = _config.m_finish_color;
            }
            else
            {
                float delta_time = _config.m_current_time;
                float percents = delta_time / _config.m_max_time;

                if (m_internal_tween_algorithm != null)
                {
                    percents = (float)m_internal_tween_algorithm(percents);
                }

                Color delta_color = (_config.m_finish_color - _config.m_start_color) * percents;
                _config.CurrentColor = _config.m_start_color + delta_color;
            }
            return true;
        }

        public bool behaviour_type_counter(effectConfig _config)
        {
            var dt = Time.deltaTime;

            if (_config.Delay > 0)
            {
                _config.Delay -= dt;
                if (_config.Delay > 0)
                {
                    return false;
                }
            }

            _config.m_current_time += dt;

            if (_config.IsBegin)
            {
                _config.IsBegin = false;
                _config.m_control_object.SetActive(true);
            }

            if (_config.m_current_time >= _config.m_max_time)
            {
                _config.m_current_time = _config.m_max_time;
                _config.IsEnd = true;

                _config.CurrentCounter = _config.m_finish_counter;
            }
            else
            {
                float delta_time = _config.m_current_time;
                float percents = delta_time / _config.m_max_time;

                if (m_internal_tween_algorithm != null)
                {
                    percents = (float)m_internal_tween_algorithm(percents);
                }

                int delta_size = (int)((_config.m_finish_counter - _config.m_start_counter) * percents);
                _config.CurrentCounter= _config.m_start_counter + delta_size;
            }
            return true;
        }
    }
}