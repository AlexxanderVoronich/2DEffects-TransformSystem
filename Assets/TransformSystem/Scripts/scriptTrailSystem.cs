using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptTrailSystem : MonoBehaviour {

    [SerializeField] private effectsManager m_effect_manager = null;
    private Dictionary<int, LineRenderer> m_trails = new Dictionary<int, LineRenderer>();
    private int m_counter = 0;

    [SerializeField] LineRenderer m_prefab = null;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ClearSystem()
    {
        foreach(var one in m_trails)
        {
            Destroy(one.Value.gameObject);
        }
        m_trails.Clear();
        m_counter = 0;
    }

    public int addTrail()
    {
        if (m_effect_manager.Is_trail_system_switch_on)
        {
            ++m_counter;

            if (!m_trails.ContainsKey(m_counter))
            {
                var r = Instantiate(m_prefab, new Vector3(0, 0, 0), Quaternion.identity);
                m_trails[m_counter] = r;
                r.gameObject.transform.SetParent(this.gameObject.transform);

                return m_counter;
            }
        }
        return 0;
    }

    public void AddPointToTrail(int _id, Vector3 _pos)
    {
        if (m_effect_manager.Is_trail_system_switch_on)
        {
            if (m_trails.ContainsKey(_id))
            {
                var r = m_trails[_id];
                int current_count = r.positionCount;
                int new_id = ++current_count;
                r.positionCount = new_id;
                r.SetPosition(new_id-1, _pos);

            }
        }
    }

}
