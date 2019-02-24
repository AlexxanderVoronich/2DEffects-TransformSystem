using Assets.EffectsScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scriptAppLogic : MonoBehaviour {

    
    [SerializeField] private effectsManager m_effect_manager = null;
    private effectsStorage m_effects_storage = null;
    [SerializeField] private GameObject m_final_panel = null;
    [SerializeField] private Button m_button_start_1= null;
    [SerializeField] private Button m_button_start_2= null;
    [SerializeField] private Button m_button_start_3= null;
    [SerializeField] private Button m_button_start_4= null;

    [SerializeField] private GameObject m_panel_select = null;

    [SerializeField] private GameObject m_group_1 = null;
    [SerializeField] private GameObject m_group_2 = null;
    [SerializeField] private GameObject m_group_3 = null;
    [SerializeField] private GameObject m_group_4 = null;
    [SerializeField] private GameObject m_group_5 = null;

    [SerializeField] private Canvas m_canvas_main = null;
    [SerializeField] private Canvas m_canvas_chest = null;
    private int m_current_group_id = -1;


    void Start ()
    {
        m_effects_storage = m_effect_manager.GetComponent<effectsStorage>();
        selectGroup(1);
    }
	
	void Update ()
    {
	}

    public void openCanvas(Canvas _canvas)
    {
        m_canvas_main.enabled = false;
        m_canvas_chest.enabled = false;

        _canvas.enabled = true;
    }

    public void selectGroup(int _id)
    {
        m_group_1.SetActive(false);
        m_group_2.SetActive(false);
        m_group_3.SetActive(false);
        m_group_4.SetActive(false);
        m_group_5.SetActive(false);

        m_current_group_id = _id;

        switch (_id)
        {
            case 1:
                {
                    m_group_1.SetActive(true);
                    break;
                }

            case 2:
                {
                    m_group_2.SetActive(true);
                    break;
                }

            case 3:
                {
                    m_group_3.SetActive(true);
                    break;
                }

            case 4:
                {
                    m_group_4.SetActive(true);
                    break;
                }

            case 5:
                {
                    m_group_5.SetActive(true);
                    break;
                }
        }
    }

    public void clickStartEffect(int _id)
    {
        switch (_id)
        {
            case 1:
                {
                    m_effect_manager.startEffectForName("ExampleEffectCompound1", effectFinalAction);
                    m_effect_manager.startEffectForName("ExampleEffectCompound2", effectFinalAction);
                    m_effect_manager.startEffectForName("ExampleEffectCompound3", effectFinalAction);
                    m_panel_select.SetActive(false);
                    m_button_start_1.gameObject.SetActive(false);
                    break;
                }
            case 2:
                {
                    m_effect_manager.startEffectForName("ExampleEffectCompound_g2", effectFinalAction);
                    m_panel_select.SetActive(false);
                    m_button_start_2.gameObject.SetActive(false);
                    break;
                }
            case 3:
                {
                    m_effect_manager.startEffectForName("ExampleEffectCompound_g3", effectFinalAction);
                    m_panel_select.SetActive(false);
                    m_button_start_2.gameObject.SetActive(false);
                    break;
                }
            case 4:
                {
                    m_effect_manager.startEffectForName("ExampleEffectCompound_g4", effectFinalAction);
                    m_effect_manager.startEffectForName("ExampleEffectCompound_g4_v2", effectFinalAction);

                    m_panel_select.SetActive(false);
                    m_button_start_4.gameObject.SetActive(false);
                    break;
                }
            case 5:
                {
                    openCanvas(m_canvas_chest);
                    m_effect_manager.startEffectForName("EffectCompoundChest", null);          
                    break;
                }
        }
    }

    public static void effectFinalAction(effectConfig _config)
    {
        var main_logic = GameObject.Find("AppLogic").GetComponent<scriptAppLogic>();
        main_logic.doAction(_config);
    }

    public void doAction(effectConfig _config)
    {
        if (!m_effect_manager.isExistRunEffect())
        {
            m_final_panel.SetActive(true);
        }
    }

    public void clickClosePanel()
    {
        switch (m_current_group_id)
        {
            case 1:
                {
                    m_final_panel.SetActive(false);
                    m_button_start_1.gameObject.SetActive(true);
                    m_panel_select.SetActive(true);
                    break;
                }
            case 2:
                {
                    m_final_panel.SetActive(false);
                    m_button_start_2.gameObject.SetActive(true);
                    m_panel_select.SetActive(true);

                    var effect_config = m_effects_storage.getEffect("ExampleEffectCompound_g2");
                    effectsManager.resetConfig(effect_config);
                    break;
                }
            case 3:
                {
                    m_final_panel.SetActive(false);
                    m_button_start_3.gameObject.SetActive(true);
                    m_panel_select.SetActive(true);
                    break;
                }
            case 4:
                {
                    m_final_panel.SetActive(false);
                    m_button_start_4.gameObject.SetActive(true);
                    m_panel_select.SetActive(true);

                    var effect_config = m_effects_storage.getEffect("ExampleEffectCompound_g4");
                    if (effect_config != null)
                    {
                        effectsManager.resetConfig(effect_config);
                    }
                    
                    var effect_config_2 = m_effects_storage.getEffect("ExampleEffectCompound_g4_v2");
                    if (effect_config_2 != null)
                    {
                        effectsManager.resetConfig(effect_config_2);
                    }
                    break;
                }
            case 5:
                {

                    break;
                }
        }

        //m_current_group_id = -1;
    }


    public void getChestContent()
    {
        openCanvas(m_canvas_main);
        selectGroup(5);

        var effect_config = m_effects_storage.getEffect("EffectCompoundChest");
        effectsManager.resetConfig(effect_config);
    }
}
