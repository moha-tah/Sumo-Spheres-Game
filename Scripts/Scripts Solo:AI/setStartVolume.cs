using UnityEngine;
using UnityEngine.UI;

public class setStartVolume : MonoBehaviour
{
    void Start()
    {
        GetComponent<Slider>().value = PlayerPrefs.GetFloat("Volume");
    }
}
