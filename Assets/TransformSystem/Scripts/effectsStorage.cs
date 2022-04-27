using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effectsStorage : MonoBehaviour {

    [SerializeField] private effectConfig[] m_effects;

    private Dictionary<string, effectConfig> m_storage = new Dictionary<string, effectConfig>();
    private bool m_is_init = false;

    void Start ()
    {
        init();
	}

    private void init()
    {
        if (!m_is_init)
        {
            foreach (var one in m_effects)
            {
                if (one != null)
                {
                    string key = one.m_root_name;
                    m_storage[key] = one;
                }
            }
            m_is_init = true;
        }
    }
	
	void Update ()
    {
		
	}

    public effectConfig getEffect(string _name)
    {
        if (!m_is_init)
            init();

        if(m_storage.ContainsKey(_name))
        {
            return m_storage[_name];
        }
        return null;
    }

    public effectConfig getEffectByIndex(int _id)
    {
        if (!m_is_init)
            init();

        int index = -1;
        foreach (var it in m_storage.Values)
        {
            index++;
            if (index == _id)
                return it;
        }
        return null;
    }

    public bool addEffectToStorage(string _name, effectConfig _config)
    {
        if (!m_storage.ContainsKey(_name))
        {
            m_storage[_name] = _config;
            return true;
        }
        return false;
    }

    public bool removeEffectFromStorage(string _name)
    {
        if (m_storage.ContainsKey(_name))
        {
            m_storage.Remove(_name);
            return true;
        }
        return false;
    }
}
