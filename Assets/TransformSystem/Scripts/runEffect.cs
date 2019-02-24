﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.EffectsScripts
{
    public class cRunEffect
    {
        public List<cRunEffect> getParts() { return m_parts; }
        public effectConfig getConfig() { return m_config; }
        private effectConfig m_config;
        private delegate bool EffectBehaviour(effectConfig _config);
        private event EffectBehaviour m_behaviour = null;
        private List<cRunEffect> m_parts = null;
        private InternalTweenAlgorithm m_internal_tween_algorithm = null;

        public cRunEffect(effectConfig _config)
        {
            m_config = _config;
            m_parts = new List<cRunEffect>();

            m_internal_tween_algorithm = tweenAlgorithmFactory.getAlgorithm(_config.m_tween_algorithm_type);
        }

        public void update()
        {
            if (m_parts != null)
            {
                foreach (var child_effect in m_parts)
                {
                    if (child_effect.getConfig().Is_end)
                    {
                        child_effect.getConfig().clearConfig();
                        child_effect.getConfig().Is_ready_for_remove = true;
                    }
                }
                m_parts.RemoveAll(x => x.m_config.Is_ready_for_remove);
            }

            if (m_config.m_type == eEffectType.COMPOUND_TYPE)
            {
                if (m_parts.Count == 0)
                {
                    m_config.Is_end = true;
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
                    case eEffectType.TERMINAL_TYPE1:
                        {
                            break;
                        }
                    case eEffectType.TERMINAL_LOCAL_POS:
                        {
                            m_config.m_control_object.transform.localPosition = m_config.m_current_pos;
                            break;
                        }

                    case eEffectType.TERMINAL_GLOBAL_POS:
                        {
                            m_config.m_control_object.transform.position = m_config.m_current_pos;
                            break;
                        }

                    case eEffectType.TERMINAL_SCALE:
                        {
                            m_config.m_control_object.transform.localScale = m_config.m_current_size;
                            break;
                        }

                    case eEffectType.TERMINAL_ARRIVE:
                    case eEffectType.TERMINAL_HIDE:
                    case eEffectType.TERMINAL_CHANGE_SPRITE:
                        {
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

        public static cRunEffect create(effectConfig _config)
        {

            cRunEffect effect = null;
            effect = new cRunEffect(_config);
            if (_config.m_type == eEffectType.COMPOUND_TYPE)
            {

            }
            else if (_config.m_type == eEffectType.TERMINAL_TYPE1)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_type1;
            }
            else if (_config.m_type == eEffectType.TERMINAL_LOCAL_POS)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_move_local_pos;
            }
            else if (_config.m_type == eEffectType.TERMINAL_GLOBAL_POS)
            {
                effect.m_config.m_mode = eEffectMode.TERMINAL_MODE;
                effect.m_behaviour = effect.behaviour_move_global_pos;
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
            return effect;
        }

        public bool behaviour_type1(effectConfig _config)
        {
            return false;
        }

        public bool behaviour_move_local_pos(effectConfig _config)
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

            if (_config.Is_begin)
            {
                _config.Is_begin = false;
                _config.m_control_object.SetActive(true);
            }

            if (_config.m_current_time >= _config.m_max_time)
            {
                _config.m_current_time = _config.m_max_time;
                _config.Is_end = true;

                _config.m_current_size = _config.m_finish_scale;
                _config.m_current_pos = _config.m_finish_pos;
            }
            else
            {
                float delta_time = _config.m_current_time;
                float percents = delta_time / _config.m_max_time;

                if (m_internal_tween_algorithm != null)
                {
                    percents = (float)m_internal_tween_algorithm(percents);
                }

                Vector2 delta_pos = (_config.m_finish_pos - _config.m_start_pos) * percents;
                _config.m_current_pos = _config.m_start_pos + delta_pos;

                Vector2 delta_size = (_config.m_finish_scale - _config.m_start_scale) * percents;
                _config.m_current_size = _config.m_start_scale + delta_size;
            }
            return true;
        }

        public bool behaviour_move_global_pos(effectConfig _config)
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

            if (_config.Is_begin)
            {
                _config.Is_begin = false;
                _config.m_control_object.SetActive(true);
            }

            if (_config.m_current_time >= _config.m_max_time)
            {
                _config.m_current_time = _config.m_max_time;
                _config.Is_end = true;

                _config.m_current_pos = _config.m_finish_pos;
            }
            else
            {
                float delta_time = _config.m_current_time;
                float percents = delta_time / _config.m_max_time;

                if (m_internal_tween_algorithm != null)
                {
                    percents = (float)m_internal_tween_algorithm(percents);
                }

                Vector2 delta_pos = (_config.m_finish_pos - _config.m_start_pos) * percents;
                _config.m_current_pos = _config.m_start_pos + delta_pos;
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

            if (_config.Is_begin)
            {
                _config.Is_begin = false;
                _config.m_control_object.SetActive(true);
            }

            if (_config.m_current_time >= _config.m_max_time)
            {
                _config.m_current_time = _config.m_max_time;
                _config.Is_end = true;

                _config.m_current_size = _config.m_finish_scale;
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
                _config.m_current_size = _config.m_start_scale + delta_size;
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
            _config.Is_begin = false;
            _config.Is_end = true;
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
            _config.Is_begin = false;
            _config.Is_end = true;
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
            _config.Is_begin = false;
            _config.Is_end = true;
            Image img = _config.m_control_object.GetComponent<Image>();
            if(img != null)
            {
                img.sprite = _config.m_new_sprite;
            }
            return true;
        }
    }
}