using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleBlobSpeedMonitor : MonoBehaviour
{
    ParticleSystem _particleSystem;
    public int SpeedMultiplier = 0;
    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        var _Burst = _particleSystem.emission;
        var _Speed = _particleSystem.main;
        _Speed.startSpeed = Mathf.Pow(PetBehavior.PetBehav.rb[0].velocity.magnitude, 1.25f) * Random.Range(1.1f,1.4f);
        if (_Speed.startSpeed.constantMax >= 25)
        {
            //Debug.Log("fast enough");
            //_Burst.burstCount = 4;
        }
        //else _Burst.burstCount = 3;
        _particleSystem.Play();
    }
    
}
