using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccAnim : MonoBehaviour
{
    [SerializeField]
    private CraftResult _craftResult;


    void Start()
    {
        
    }

    public void EndAnimation()
    {
        if (_craftResult != null)
        {
            _craftResult.ShowResultText();
        }
    }
}
