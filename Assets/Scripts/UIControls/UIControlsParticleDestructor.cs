/*///////////////////////////////////////////////
{                                              }
{     Деструктор одноразовых систем частиц     }
{       Copyright (c) 2016 UAshota             }
{                                              }
{  Rev A  2016.12.17                           }
{  Rev B  2017.05.15                           }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

public class UIControlsParticleDestructor : MonoBehaviour
{
    // Ссылка на саму систему частиц
    private ParticleSystem FParticleSystem;

    void Start()
    {
        // Запускается одна итерация
        FParticleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // Как только выполнение завершится - уничтожаем объект
        if (FParticleSystem.isStopped)
            Destroy(gameObject);
    }
}